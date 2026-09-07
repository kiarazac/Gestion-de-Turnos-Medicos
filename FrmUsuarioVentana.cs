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

        private void FrmUsuarioVentana_Load(object sender, EventArgs e)
        {
            CargarTurnosDesdeBD();
        }

        /// <summary>
        /// Obtiene y proyecta los turnos en tiempo real invocando a la Capa de Negocio (TurnoBLL).
        /// Reemplaza completamente los datos de prueba y se ejecuta tanto en la carga inicial
        /// como de forma periódica mediante el Timer.
        /// </summary>
        public void CargarTurnosDesdeBD()
        {
            try
            {
                // 1. Carga de Turnos de Guardia / Emergencias
                // Invoca TurnoBLL.ListarTurnosEmergencia() -> sp_ListarTurnosEmergencia
                dgvEmergencias.Rows.Clear();
                var turnosEmergencia = _turnoBLL.ListarTurnosEmergencia();

                if (turnosEmergencia != null)
                {
                    foreach (var t in turnosEmergencia)
                    {
                        dgvEmergencias.Rows.Add(
                            t.Turno ?? string.Empty,
                            t.Prioridad ?? "MEDIA",
                            t.Hora ?? string.Empty,
                            t.Estado ?? "ESPERANDO",
                            t.Sala ?? "--"
                        );
                    }
                }
                ResaltarEstados(dgvEmergencias, colEstadoEmer.Index, "LLAMADO");

                // 2. Carga de Turnos de Consultorio / Especialidades Generales
                // Invoca TurnoBLL.ObtenerTurnosPantallaGeneral() -> sp_ListarTurnosGeneralesPantalla
                dgvGeneral.Rows.Clear();
                var turnosGenerales = _turnoBLL.ObtenerTurnosPantallaGeneral();

                if (turnosGenerales != null)
                {
                    foreach (var g in turnosGenerales)
                    {
                        dgvGeneral.Rows.Add(
                            g.Turno ?? string.Empty,
                            g.Hora ?? string.Empty,
                            g.Fecha ?? string.Empty,
                            g.Especialidad ?? "General",
                            g.Estado ?? "ESPERANDO",
                            g.Sala ?? "--"
                        );
                    }
                }
                ResaltarEstados(dgvGeneral, colEstadoGen.Index, "EN CURSO");
                ResaltarEstados(dgvGeneral, colEstadoGen.Index, "LLAMADO");
            }
            catch (Exception ex)
            {
                // En una pantalla de sala de espera pública no se interrumpe la visualización
                // con cuadros modales bloqueantes repetitivos; se actualiza el pie de estado discretamente.
                lblFooter.Text = $"[Aviso de sincronización] {DateTime.Now:HH:mm:ss} - Reintentando conexión con servidor...";
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