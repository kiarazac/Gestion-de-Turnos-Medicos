using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmListaTurnosAtencion : Form
    {
        // Estados posibles del puesto de trabajo (médico + consultorio).
        private enum EstadoPuesto
        {
            SinPaciente,   // Nadie en el panel de atención actual.
            Llamado,       // Se llamó al paciente (Siguiente Paciente) pero todavía no entró a consulta.
            EnConsulta     // Iniciar Atención ya fue presionado.
        }

        // Datos del médico logueado / sala asignada (llegan desde FrmPersonalMedico).
        private readonly string _nombreMedico;
        private readonly string _matriculaMedico;
        private readonly string _salaAsignada;

        // "Base de datos" simulada. En la implementación real esto se reemplaza
        // por consultas a la base (SELECT de turnos en espera por servicio).
        private List<Turno> _todosLosTurnos;
        private List<Turno> _historialAtendidos;

        private BindingList<Turno> _turnosVisibles;
        private Turno _turnoActual;
        private EstadoPuesto _estadoActual = EstadoPuesto.SinPaciente;

        private int _indiceServicioAnterior = 0;
        private bool _bloqueandoCombo = false;

        public FrmListaTurnosAtencion() : this("Dr. Juan Pérez", "12345", "Consultorio 3 (Piso 1)")
        {
            // Constructor sin parámetros solo para poder previsualizar el form.
            // En la app real siempre se debería usar el constructor con los datos del médico logueado.
        }

        public FrmListaTurnosAtencion(string nombreMedico, string matricula, string salaAsignada)
        {
            InitializeComponent();

            _nombreMedico = nombreMedico;
            _matriculaMedico = matricula;
            _salaAsignada = salaAsignada;

            this.Load += FrmListaTurnosAtencion_Load;
        }

        private void FrmListaTurnosAtencion_Load(object sender, EventArgs e)
        {
            this.Text = $"FrmListaTurnosAtencion - {_nombreMedico}";
            lblMedicoInfo.Text = $"{_nombreMedico} (M.N. {_matriculaMedico})  |  Sala: {_salaAsignada}";
            lblTrazabilidad.Text = $"Trazabilidad: {_nombreMedico} | {_salaAsignada}";

            ConfigurarGrid();
            CargarServiciosDelMedico();
            CargarDatosDeEjemplo();

            RefrescarListado();
            LimpiarPanelAtencion();
            CambiarEstadoPuesto(EstadoPuesto.SinPaciente);
        }

        // ---------------------------------------------------------------
        // Configuración inicial
        // ---------------------------------------------------------------

        private void ConfigurarGrid()
        {
            dgvTurnos.AutoGenerateColumns = false;
            dgvTurnos.AllowUserToAddRows = false;
            dgvTurnos.AllowUserToDeleteRows = false;
            dgvTurnos.ReadOnly = true;
            dgvTurnos.MultiSelect = false;
            dgvTurnos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTurnos.Columns.Clear();

            dgvTurnos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = "N° Turno",
                DataPropertyName = "Id",
                Width = 70
            });

            dgvTurnos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHora",
                HeaderText = "Hora Registro",
                DataPropertyName = "HoraRegistroTexto",
                Width = 100
            });

            dgvTurnos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaciente",
                HeaderText = "Paciente (DNI)",
                DataPropertyName = "PacienteTexto",
                Width = 230
            });

            // Solo se muestra cuando el servicio activo es "Emergencias / Guardia".
            dgvTurnos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTriage",
                HeaderText = "Triage",
                DataPropertyName = "Triage",
                Width = 90
            });

            dgvTurnos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstado",
                HeaderText = "Estado",
                DataPropertyName = "Estado",
                Width = 100
            });
        }

        private void CargarServiciosDelMedico()
        {
            cboServicio.Items.Clear();

            // Opción fija, disponible para cualquier médico.
            cboServicio.Items.Add("Emergencias / Guardia");

            // TODO: reemplazar esto por la carga real de las especialidades asignadas
            // al médico logueado según sus permisos (consulta a la tabla de asignaciones).
            cboServicio.Items.Add("Cardiología");
            cboServicio.Items.Add("Traumatología");

            cboServicio.SelectedIndex = 0;
            _indiceServicioAnterior = 0;
        }

        private void CargarDatosDeEjemplo()
        {
            // Datos de ejemplo para poder ver el formulario funcionando.
            // En la implementación real, este método se reemplaza por la carga
            // de turnos en espera desde la base de datos.
            var hoy = DateTime.Today;

            _todosLosTurnos = new List<Turno>
            {
                new Turno { Id = 101, HoraRegistro = hoy.AddHours(8).AddMinutes(30), Nombre = "Juan",  Apellido = "Pérez",     DNI = "30.123.456", Edad = 42, Cobertura = "PAMI",          Motivo = "Dolor torácico",          Triage = "Alta",  Estado = "En Espera", Servicio = "Emergencias / Guardia" },
                new Turno { Id = 102, HoraRegistro = hoy.AddHours(8).AddMinutes(31), Nombre = "María", Apellido = "García",    DNI = "25.987.654", Edad = 35, Cobertura = "OSDE",          Motivo = "Fiebre y malestar",        Triage = "Media", Estado = "En Espera", Servicio = "Emergencias / Guardia" },
                new Turno { Id = 103, HoraRegistro = hoy.AddHours(8).AddMinutes(32), Nombre = "Luis",  Apellido = "Martínez",  DNI = "28.321.098", Edad = 51, Cobertura = "IOMA",          Motivo = "Dolor lumbar",             Triage = "Media", Estado = "En Espera", Servicio = "Emergencias / Guardia" },
                new Turno { Id = 104, HoraRegistro = hoy.AddHours(8).AddMinutes(33), Nombre = "Ana",   Apellido = "Rodríguez", DNI = "31.765.432", Edad = 29, Cobertura = "Swiss Medical", Motivo = "Control de rutina",        Triage = "Baja",  Estado = "En Espera", Servicio = "Emergencias / Guardia" },

                new Turno { Id = 201, HoraRegistro = hoy.AddHours(9).AddMinutes(0),  Nombre = "Carlos", Apellido = "Fernández", DNI = "22.456.789", Edad = 60, Cobertura = "PAMI", Motivo = "Control cardiológico",     Triage = "", Estado = "En Espera", Servicio = "Cardiología" },
                new Turno { Id = 202, HoraRegistro = hoy.AddHours(9).AddMinutes(15), Nombre = "Lucía",  Apellido = "Gómez",     DNI = "27.654.321", Edad = 47, Cobertura = "OSDE", Motivo = "Chequeo post-operatorio",  Triage = "", Estado = "En Espera", Servicio = "Cardiología" },
            };

            _historialAtendidos = new List<Turno>();
        }

        // ---------------------------------------------------------------
        // Filtro / orden de la cola
        // ---------------------------------------------------------------

        private void RefrescarListado()
        {
            string servicio = cboServicio.SelectedItem?.ToString() ?? string.Empty;
            bool esEmergencia = servicio == "Emergencias / Guardia";

            var filtrados = _todosLosTurnos.Where(t => t.Servicio == servicio && t.Estado == "En Espera");

            IEnumerable<Turno> ordenados;
            if (esEmergencia)
            {
                // Modo Emergencias: prioridad (Alta -> Media -> Baja) y, a igual prioridad, FIFO.
                ordenados = filtrados
                    .OrderBy(t => PrioridadNumerica(t.Triage))
                    .ThenBy(t => t.HoraRegistro);
            }
            else
            {
                // Modo Especialidades: orden estricto de llegada (FIFO).
                ordenados = filtrados.OrderBy(t => t.HoraRegistro);
            }

            _turnosVisibles = new BindingList<Turno>(ordenados.ToList());
            dgvTurnos.DataSource = _turnosVisibles;

            if (dgvTurnos.Columns["colTriage"] != null)
                dgvTurnos.Columns["colTriage"].Visible = esEmergencia;

            if (_estadoActual == EstadoPuesto.SinPaciente)
                btnSiguientePaciente.Enabled = _turnosVisibles.Count > 0;

            ActualizarBarraEstado();
        }

        private int PrioridadNumerica(string triage)
        {
            switch (triage)
            {
                case "Alta": return 1;
                case "Media": return 2;
                case "Baja": return 3;
                default: return 4;
            }
        }

        private void cboServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_bloqueandoCombo)
                return;

            // Bloqueo por atención en curso: no se puede cambiar de servicio
            // si hay un paciente llamado o en consulta sin finalizar.
            if (_estadoActual != EstadoPuesto.SinPaciente)
            {
                _bloqueandoCombo = true;
                cboServicio.SelectedIndex = _indiceServicioAnterior;
                _bloqueandoCombo = false;

                MessageBox.Show(
                    "Hay una atención en curso. Terminá la atención actual antes de cambiar de servicio.",
                    "Atención en curso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _indiceServicioAnterior = cboServicio.SelectedIndex;
            RefrescarListado();
        }

        // ---------------------------------------------------------------
        // Flujo de estados: Siguiente Paciente -> Iniciar Atención -> Terminar Atención
        // ---------------------------------------------------------------

        private void btnSiguientePaciente_Click(object sender, EventArgs e)
        {
            if (_turnosVisibles == null || _turnosVisibles.Count == 0)
                return;

            _turnoActual = _turnosVisibles[0];
            _turnoActual.Estado = "Llamado";

            // Se remueve tanto de la cola general como del listado visible.
            _todosLosTurnos.Remove(_turnoActual);
            _turnosVisibles.RemoveAt(0);

            LlenarPanelAtencion(_turnoActual);
            tmrTiempoTranscurrido.Start();

            CambiarEstadoPuesto(EstadoPuesto.Llamado);
        }

        private void btnIniciarAtencion_Click(object sender, EventArgs e)
        {
            if (_turnoActual == null)
                return;

            _turnoActual.HoraInicioAtencion = DateTime.Now;
            _turnoActual.Estado = "En Consulta";

            CambiarEstadoPuesto(EstadoPuesto.EnConsulta);
        }

        private void btnTerminarAtencion_Click(object sender, EventArgs e)
        {
            if (_turnoActual == null)
                return;

            // Acá es donde en un caso real se persistirían en la base de datos:
            // observaciones, diagnóstico rápido, hora de cierre, médico y sala (trazabilidad).
            _turnoActual.Observaciones = txtObservaciones.Text.Trim();
            _turnoActual.DiagnosticoRapido = txtDiagnostico.Text.Trim();
            _turnoActual.HoraFinAtencion = DateTime.Now;
            _turnoActual.Estado = "Atendido";
            _turnoActual.MedicoQueAtendio = _nombreMedico;
            _turnoActual.SalaDeAtencion = _salaAsignada;

            _historialAtendidos.Add(_turnoActual);

            tmrTiempoTranscurrido.Stop();
            _turnoActual = null;

            LimpiarPanelAtencion();
            CambiarEstadoPuesto(EstadoPuesto.SinPaciente);
        }

        private void CambiarEstadoPuesto(EstadoPuesto nuevoEstado)
        {
            _estadoActual = nuevoEstado;

            switch (nuevoEstado)
            {
                case EstadoPuesto.SinPaciente:
                    btnSiguientePaciente.Enabled = _turnosVisibles != null && _turnosVisibles.Count > 0;
                    btnIniciarAtencion.Enabled = false;
                    btnTerminarAtencion.Enabled = false;
                    cboServicio.Enabled = true;
                    lblAvisoBloqueo.Visible = false;
                    break;

                case EstadoPuesto.Llamado:
                    btnSiguientePaciente.Enabled = false;
                    btnIniciarAtencion.Enabled = true;
                    btnTerminarAtencion.Enabled = false;
                    cboServicio.Enabled = false;
                    lblAvisoBloqueo.Visible = true;
                    break;

                case EstadoPuesto.EnConsulta:
                    btnSiguientePaciente.Enabled = false;
                    btnIniciarAtencion.Enabled = false;
                    btnTerminarAtencion.Enabled = true;
                    cboServicio.Enabled = false;
                    lblAvisoBloqueo.Visible = true;
                    break;
            }

            ActualizarBarraEstado();
        }

        // ---------------------------------------------------------------
        // Panel de Atención Actual
        // ---------------------------------------------------------------

        private void LlenarPanelAtencion(Turno t)
        {
            lblInfoTurno.Text = $"N° Turno: {t.Id}";
            lblInfoPaciente.Text = $"Paciente: {t.PacienteCorto}";
            lblInfoDni.Text = $"DNI / Edad / Cobertura: {t.DNI} / {t.Edad} años / {t.Cobertura}";
            lblInfoMotivo.Text = $"Motivo / Prioridad: {(string.IsNullOrEmpty(t.Motivo) ? "-" : t.Motivo)} /";

            lblInfoPrioridadValor.Text = string.IsNullOrEmpty(t.Triage) ? "-" : t.Triage;
            lblInfoPrioridadValor.ForeColor = ColorSegunTriage(t.Triage);
            // Se reposiciona a mano al lado del label anterior, porque su ancho
            // cambia según el largo del texto (AutoSize no alinea dos labels solo).
            lblInfoPrioridadValor.Location = new Point(lblInfoMotivo.Right + 4, lblInfoMotivo.Top);

            ActualizarTiempoTranscurrido();

            txtObservaciones.Clear();
            txtDiagnostico.Clear();
        }

        private void LimpiarPanelAtencion()
        {
            lblInfoTurno.Text = "N° Turno: -";
            lblInfoPaciente.Text = "Paciente: -";
            lblInfoDni.Text = "DNI / Edad / Cobertura: -";
            lblInfoMotivo.Text = "Motivo / Prioridad: -";
            lblInfoPrioridadValor.Text = "";
            lblInfoPrioridadValor.Location = new Point(lblInfoMotivo.Right + 4, lblInfoMotivo.Top);
            lblInfoTiempo.Text = "Hora de Entrada / Tiempo: -";

            txtObservaciones.Clear();
            txtDiagnostico.Clear();
        }

        private Color ColorSegunTriage(string triage)
        {
            switch (triage)
            {
                case "Alta": return Color.FromArgb(214, 39, 40);
                case "Media": return Color.FromArgb(184, 134, 11);
                case "Baja": return Color.FromArgb(46, 139, 87);
                default: return Color.Black;
            }
        }

        private void tmrTiempoTranscurrido_Tick(object sender, EventArgs e)
        {
            ActualizarTiempoTranscurrido();
        }

        private void ActualizarTiempoTranscurrido()
        {
            if (_turnoActual == null)
            {
                lblInfoTiempo.Text = "Hora de Entrada / Tiempo: -";
                return;
            }

            TimeSpan transcurrido = DateTime.Now - _turnoActual.HoraRegistro;
            int minutos = Math.Max(0, (int)transcurrido.TotalMinutes);

            lblInfoTiempo.Text = $"Hora de Entrada / Tiempo: {_turnoActual.HoraRegistro:HH:mm} / {minutos} min.";
        }

        // ---------------------------------------------------------------
        // Barra de estado inferior
        // ---------------------------------------------------------------

        private void ActualizarBarraEstado()
        {
            int enEspera = _turnosVisibles?.Count ?? 0;
            string mensaje;

            switch (_estadoActual)
            {
                case EstadoPuesto.SinPaciente:
                    mensaje = enEspera > 0 ? "Listo para llamar al siguiente paciente." : "No hay pacientes en espera.";
                    break;
                case EstadoPuesto.Llamado:
                    mensaje = $"Esperando inicio de atención para {_turnoActual?.NombreCompleto}.";
                    break;
                case EstadoPuesto.EnConsulta:
                    mensaje = $"Atención en curso con {_turnoActual?.NombreCompleto}.";
                    break;
                default:
                    mensaje = string.Empty;
                    break;
            }

            lblEstadoInferior.Text = $"Total pacientes en espera: {enEspera}  |  Estado: {mensaje}";
        }

        // ---------------------------------------------------------------
        // Coloreado de la columna Triage en el grid
        // ---------------------------------------------------------------

        private void dgvTurnos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvTurnos.Columns[e.ColumnIndex].Name != "colTriage" || e.Value == null)
                return;

            string triage = e.Value.ToString();

            switch (triage)
            {
                case "Alta":
                    e.CellStyle.BackColor = Color.FromArgb(214, 39, 40);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.Font = new Font(dgvTurnos.Font, FontStyle.Bold);
                    break;
                case "Media":
                    e.CellStyle.BackColor = Color.FromArgb(255, 204, 0);
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.Font = new Font(dgvTurnos.Font, FontStyle.Bold);
                    break;
                case "Baja":
                    e.CellStyle.BackColor = Color.FromArgb(46, 139, 87);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.Font = new Font(dgvTurnos.Font, FontStyle.Bold);
                    break;
            }
        }
    }

    // ---------------------------------------------------------------
    // Modelo de datos de un turno.
    // En la implementación real esta clase probablemente ya exista
    // en otra parte del proyecto (o se reemplace por una fila de DataTable).
    // ---------------------------------------------------------------
    public class Turno
    {
        public int Id { get; set; }
        public DateTime HoraRegistro { get; set; }
        public string HoraRegistroTexto => HoraRegistro.ToString("HH:mm");

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }

        // "Pérez, Juan (30.123.456)" -> como se muestra en el grid y en el panel.
        public string PacienteTexto => $"{Apellido}, {Nombre} ({DNI})";
        // "Pérez, Juan" -> como se muestra en el panel (sin repetir el DNI).
        public string PacienteCorto => $"{Apellido}, {Nombre}";
        // "Juan Pérez" -> como se usa en la barra de estado inferior.
        public string NombreCompleto => $"{Nombre} {Apellido}";

        public int Edad { get; set; }
        public string Cobertura { get; set; }
        public string Motivo { get; set; }

        // "Alta" / "Media" / "Baja" (vacío si el servicio no es de emergencias).
        public string Triage { get; set; }

        // "En Espera" -> "Llamado" -> "En Consulta" -> "Atendido"
        public string Estado { get; set; }

        public string Servicio { get; set; }

        public DateTime? HoraInicioAtencion { get; set; }
        public DateTime? HoraFinAtencion { get; set; }

        public string Observaciones { get; set; }
        public string DiagnosticoRapido { get; set; }

        // Trazabilidad: quién y dónde se hizo la atención.
        public string MedicoQueAtendio { get; set; }
        public string SalaDeAtencion { get; set; }
    }
}