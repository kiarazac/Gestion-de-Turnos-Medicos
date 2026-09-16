using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Ventana de visualización de turnos para pacientes (pantalla pública de sala de espera).
    /// Muestra dos listados en tiempo real: Emergencias y General.
    /// Conectada a la arquitectura de 3 capas a través de TurnoBLL.
    /// </summary>
    public partial class FrmUsuarioVentana : Form
    {
        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();

        // 2. Contexto de sesión opcional (cuando se inicia sesión con rol Usuario Ventana)
        private readonly UsuarioLoginResult? _usuarioActual;

        // Nombre de la institución médica
        private readonly string nombreClinica = "Hospital San Martín";

        // Color usado para resaltar el estado "en atención" o "llamado" en ambas grillas
        private static readonly Color ColorEstadoResaltado = Color.FromArgb(91, 155, 213);

        // Contador de ciclos para refresco automático cada 5 segundos
        private int _segundosTranscurridos = 0;
        private const int IntervaloRefrescoSegundos = 5;

        /// <summary>
        /// Constructor sin parámetros (utilizado desde la interfaz de Recepción o apertura directa del visor).
        /// </summary>
        public FrmUsuarioVentana()
        {
            InitializeComponent();
            _usuarioActual = null;
            ConfigurarFormulario();
        }

        /// <summary>
        /// Constructor con sesión de usuario autenticado (utilizado desde FrmLogin con rol Usuario Ventana).
        /// </summary>
        public FrmUsuarioVentana(UsuarioLoginResult usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            this.Load += FrmUsuarioVentana_Load;
            ActualizarPieDePagina();
        }

        private void FrmUsuarioVentana_Load(object? sender, EventArgs e)
        {
            CargarTurnosDesdeBD();
        }

        /// <summary>
        /// Obtiene y proyecta los turnos en tiempo real invocando a la Capa de Negocio (TurnoBLL).
        /// Carga de manera aislada e independiente el sector de Emergencias (panel izquierdo)
        /// y el sector de Consultorio General (panel derecho), garantizando que un fallo o demora
        /// en un sector no bloquee la visualización del otro.
        /// </summary>
        public void CargarTurnosDesdeBD()
        {
            // =========================================================================
            // 1. Carga del Sector de Emergencias / Guardia (Panel Izquierdo)
            // =========================================================================
            try
            {
                dgvEmergencias.Rows.Clear();

                // Invoca TurnoBLL.ListarTurnosEmergencia() -> TurnoDAL -> sp_ListarTurnosEmergencia
                var turnosEmergencia = _turnoBLL.ListarTurnosEmergencia();

                if (turnosEmergencia != null)
                {
                    foreach (var t in turnosEmergencia)
                    {
                        // Mapeamos los datos de cada turno de guardia con valores por defecto defensivos
                        string codigoTurno = !string.IsNullOrWhiteSpace(t.Turno) 
                            ? t.Turno 
                            : (!string.IsNullOrWhiteSpace(t.NroOrden) ? t.NroOrden : $"E-{t.IdTurno:D3}");

                        string prioridad = !string.IsNullOrWhiteSpace(t.Prioridad) 
                            ? t.Prioridad 
                            : (!string.IsNullOrWhiteSpace(t.Triage) ? t.Triage : "MEDIA");

                        string hora = !string.IsNullOrWhiteSpace(t.Hora) ? t.Hora : "--:--";
                        string estado = !string.IsNullOrWhiteSpace(t.Estado) ? t.Estado : "En Espera";
                        string sala = !string.IsNullOrWhiteSpace(t.Sala) ? t.Sala : "--";

                        dgvEmergencias.Rows.Add(codigoTurno, prioridad, hora, estado, sala);
                    }
                }

                // Resaltamos visualmente los pacientes llamados por guardia
                ResaltarEstados(dgvEmergencias, colEstadoEmer.Index, "LLAMADO");
            }
            catch (Exception ex)
            {
                // Registramos o informamos en el pie de página sin interrumpir la experiencia de sala de espera
                lblFooter.Text = $"[Guardia] Error de sincronización: {ex.Message}";
            }

            // =========================================================================
            // 2. Carga del Sector de Consultorios / General (Panel Derecho)
            // =========================================================================
            try
            {
                dgvGeneral.Rows.Clear();

                // Invoca TurnoBLL.ObtenerTurnosPantallaGeneral() -> TurnoDAL -> sp_ListarTurnosGeneralesPantalla
                var turnosGenerales = _turnoBLL.ObtenerTurnosPantallaGeneral();

                if (turnosGenerales != null)
                {
                    foreach (var g in turnosGenerales)
                    {
                        // Mapeamos los datos de los turnos programados para las diferentes especialidades
                        string codigoTurno = !string.IsNullOrWhiteSpace(g.Turno) ? g.Turno : "--";
                        string hora = !string.IsNullOrWhiteSpace(g.Hora) ? g.Hora : "--:--";
                        string fecha = !string.IsNullOrWhiteSpace(g.Fecha) ? g.Fecha : "--/--/----";
                        string especialidad = !string.IsNullOrWhiteSpace(g.Especialidad) ? g.Especialidad : "General";
                        string estado = !string.IsNullOrWhiteSpace(g.Estado) ? g.Estado : "En Espera";
                        string sala = !string.IsNullOrWhiteSpace(g.Sala) ? g.Sala : "--";

                        dgvGeneral.Rows.Add(codigoTurno, hora, fecha, especialidad, estado, sala);
                    }
                }

                // Resaltamos estados activos en consultorios
                ResaltarEstados(dgvGeneral, colEstadoGen.Index, "EN CURSO");
                ResaltarEstados(dgvGeneral, colEstadoGen.Index, "LLAMADO");
            }
            catch (Exception ex)
            {
                // Registramos en el pie de página sin bloquear la interfaz
                lblFooter.Text = $"[Consultorios] Error de sincronización: {ex.Message}";
            }
        }

        /// <summary>
        /// Resalta con un color de fondo la celda de "Estado" en las filas cuyo
        /// estado coincide con el valor indicado (ej: "LLAMADO" o "EN CURSO").
        /// </summary>
        private void ResaltarEstados(DataGridView grilla, int columnaEstadoIndex, string estadoAResaltar)
        {
            foreach (DataGridViewRow fila in grilla.Rows)
            {
                if (fila.Cells.Count > columnaEstadoIndex)
                {
                    var celdaEstado = fila.Cells[columnaEstadoIndex];
                    bool coincide = string.Equals(
                        Convert.ToString(celdaEstado.Value),
                        estadoAResaltar,
                        StringComparison.OrdinalIgnoreCase);

                    if (coincide)
                    {
                        celdaEstado.Style.BackColor = ColorEstadoResaltado;
                        celdaEstado.Style.ForeColor = Color.White;
                        celdaEstado.Style.Font = new Font(grilla.Font, FontStyle.Bold);
                    }
                }
            }
        }

        /// <summary>
        /// Refresca el texto del pie de página con el nombre de la clínica, operador (si aplica) y fecha/hora actual.
        /// </summary>
        private void ActualizarPieDePagina()
        {
            string infoUsuario = _usuarioActual != null ? $"Operador: {_usuarioActual.Nombre} {_usuarioActual.Apellido} | " : string.Empty;
            lblFooter.Text = $"{infoUsuario}Clínica contacto: {nombreClinica}    {DateTime.Now:dd/MM/yyyy}    {DateTime.Now:HH:mm:ss}";
        }

        /// <summary>
        /// Tick del Timer (cada 1000 ms):
        /// 1. Actualiza el reloj del footer.
        /// 2. Cada 5 segundos refresca automáticamente los turnos desde la base de datos.
        /// </summary>
        private void timerReloj_Tick(object sender, EventArgs e)
        {
            ActualizarPieDePagina();

            _segundosTranscurridos++;
            if (_segundosTranscurridos >= IntervaloRefrescoSegundos)
            {
                _segundosTranscurridos = 0;
                CargarTurnosDesdeBD();
            }
        }
    }
}