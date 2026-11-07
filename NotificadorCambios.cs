using Microsoft.Data.SqlClient;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Avisa a las pantallas abiertas qué tablas cambiaron en la base de datos para que recarguen sus listados al instante.
    /// </summary>
    /// <remarks>
    /// Los triggers <c>trg_&lt;Tabla&gt;_Notificar</c> incrementan la versión de cada tabla en <c>dbo.CambiosSistema</c>.
    /// Esa consulta se vigila con <see cref="SqlDependency"/> (Service Broker): SQL Server avisa apenas cambia una versión.
    /// Si el aviso no está disponible o se corta la conexión, se consultan las versiones cada segundo hasta poder volver a suscribirse.
    /// </remarks>
    public static class NotificadorCambios
    {
        private const string ConsultaVersiones = "SELECT Tabla, Version FROM dbo.CambiosSistema";
        private const int IntervaloRespaldoMs = 1000;  // Sondeo mientras no hay aviso de SQL Server
        private const int IntervaloLatidoMs = 10000;   // Control de que el canal de avisos sigue vivo

        private static event Action<IReadOnlySet<string>>? Cambio;

        private static readonly object _lock = new object();
        private static readonly Dictionary<string, long> _versiones = new Dictionary<string, long>();
        private static SynchronizationContext? _ui;
        private static System.Threading.Timer? _temporizador;
        private static bool _brokerIniciado;
        private static bool _suscripto;
        private static bool _soloSondeo; // SQL Server rechazó la consulta para avisos: reintentar entraría en bucle

        /// <summary>
        /// Arranca la vigilancia de cambios. Debe invocarse desde el hilo de la interfaz: los avisos se entregan en ese hilo.
        /// </summary>
        public static void Iniciar()
        {
            _ui = new WindowsFormsSynchronizationContext();
            _temporizador = new System.Threading.Timer(_ => Sincronizar());
            _temporizador.Change(0, Timeout.Infinite); // Primera lectura en segundo plano para no demorar el inicio si el servidor no responde
        }

        public static void Detener()
        {
            lock (_lock)
            {
                _temporizador?.Dispose();
                _temporizador = null;
                DetenerBroker();
            }
        }

        /// <summary>
        /// Ejecuta <paramref name="recargar"/> en el hilo de la interfaz cada vez que cambia alguna de las <paramref name="tablas"/>,
        /// mientras <paramref name="form"/> exista.
        /// </summary>
        public static void Escuchar(Form form, Action recargar, params string[] tablas)
        {
            Action<IReadOnlySet<string>> manejador = cambiadas =>
            {
                if (form.IsDisposed || !tablas.Any(cambiadas.Contains))
                    return;

                try
                {
                    recargar();
                }
                catch
                {
                    // Recarga en segundo plano: un fallo puntual no debe interrumpir al usuario; el próximo cambio vuelve a intentarlo
                }
            };

            Cambio += manejador;
            form.Disposed += (s, e) => Cambio -= manejador;
        }

        /// <summary>
        /// Recarga una grilla manteniendo seleccionada la misma fila (identificada por <paramref name="clave"/>).
        /// Si esa fila ya no está, la grilla queda sin selección para que ninguna acción caiga sobre otro registro.
        /// </summary>
        public static void RecargarConservandoSeleccion(DataGridView grilla, Func<DataGridViewRow, object?> clave, Action recargar)
        {
            object? seleccionada = grilla.CurrentRow != null ? clave(grilla.CurrentRow) : null;

            recargar();

            var fila = seleccionada == null ? null
                : grilla.Rows.Cast<DataGridViewRow>().FirstOrDefault(f => f.Visible && Equals(clave(f), seleccionada));
            var celda = fila?.Cells.Cast<DataGridViewCell>().FirstOrDefault(c => c.Visible);

            grilla.CurrentCell = celda;
            if (celda == null)
                grilla.ClearSelection();
        }

        /// <summary>
        /// Lee las versiones, avisa las tablas que cambiaron y deja programado el próximo control:
        /// latido si hay suscripción activa, sondeo cada segundo si no.
        /// </summary>
        private static void Sincronizar()
        {
            lock (_lock)
            {
                if (_temporizador == null)
                    return; // Detenido

                bool suscribir = !_suscripto && AvisoDisponible();

                try
                {
                    using var conexion = Conexion.ObtenerConexion();
                    using var comando = new SqlCommand(ConsultaVersiones, conexion);

                    if (suscribir)
                        new SqlDependency(comando).OnChange += AlCambiar;

                    conexion.Open();

                    var cambiadas = new HashSet<string>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            string tabla = lector.GetString(0);
                            long version = lector.GetInt64(1);

                            if (_versiones.TryGetValue(tabla, out long anterior) && anterior != version)
                                cambiadas.Add(tabla);

                            _versiones[tabla] = version;
                        }
                    }

                    // Un cambio detectado por el latido significa que el aviso no llegó: se vuelve a suscribir en el próximo ciclo
                    if (_suscripto && cambiadas.Count > 0)
                        _suscripto = false;
                    else if (suscribir)
                        _suscripto = true;

                    if (cambiadas.Count > 0)
                        _ui?.Post(_ => Cambio?.Invoke(cambiadas), null);
                }
                catch
                {
                    _suscripto = false; // Servidor caído o tabla inexistente: se sigue sondeando hasta que responda
                }

                _temporizador.Change(_suscripto ? IntervaloLatidoMs : IntervaloRespaldoMs, Timeout.Infinite);
            }
        }

        private static void AlCambiar(object? sender, SqlNotificationEventArgs e)
        {
            // Se procesa fuera del hilo del aviso: SqlClient puede invocarlo dentro de ExecuteReader mientras se tiene el lock
            Task.Run(() =>
            {
                lock (_lock)
                {
                    _suscripto = false;

                    if (e.Type == SqlNotificationType.Subscribe)
                        _soloSondeo = true;
                    else if (e.Info == SqlNotificationInfo.Error)
                        DetenerBroker(); // Canal de avisos caído: se reinicia en la próxima suscripción

                    Sincronizar();
                }
            });
        }

        private static bool AvisoDisponible()
        {
            if (_soloSondeo)
                return false;

            if (!_brokerIniciado)
            {
                try
                {
                    SqlDependency.Start(Conexion.CadenaConexion);
                    _brokerIniciado = true;
                }
                catch
                {
                    // Service Broker deshabilitado o servidor caído: se sigue por sondeo
                }
            }

            return _brokerIniciado;
        }

        private static void DetenerBroker()
        {
            if (!_brokerIniciado)
                return;

            try { SqlDependency.Stop(Conexion.CadenaConexion); } catch { }
            _brokerIniciado = false;
        }
    }
}
