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

        // Campos locales para almacenar datos de atención en curso sin modificar
        // la entidad Turno (respetando el diseño de Modelos.cs).
        private DateTime? _horaInicioAtencion;
        private DateTime? _horaFinAtencion;
        private string _diagnosticoRapido;
        private string _medicoQueAtendio;
        private string _salaDeAtencion;

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
                DataPropertyName = "IdTurno",
                Width = 70
            });

            dgvTurnos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHora",
                HeaderText = "Hora Registro",
                DataPropertyName = "Fecha",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm" },
                Width = 100
            });

            dgvTurnos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaciente",
                HeaderText = "Paciente (DNI)",
                DataPropertyName = "Paciente",
                Width = 230
            });

            // Solo se muestra cuando el servicio activo es "Emergencias / Guardia".
            dgvTurnos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTriage",
                HeaderText = "Triage",
                DataPropertyName = "", // no binding; se formatea en CellFormatting
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

            // Crear datos de ejemplo respetando las entidades del modelo.
            _todosLosTurnos = new List<Turno>
            {
                new Turno
                {
                    IdTurno = 101,
                    Fecha = hoy.AddHours(8).AddMinutes(30),
                    Estado = "En Espera",
                    Especialidad = new Especialidad { Nombre = "Emergencias / Guardia" },
                    Paciente = new Paciente { Nombre = "Juan", Apellido = "Pérez", Dni = "30.123.456", ObraSocial = "PAMI" }
                },
                new Turno
                {
                    IdTurno = 102,
                    Fecha = hoy.AddHours(8).AddMinutes(31),
                    Estado = "En Espera",
                    Especialidad = new Especialidad { Nombre = "Emergencias / Guardia" },
                    Paciente = new Paciente { Nombre = "María", Apellido = "García", Dni = "25.987.654", ObraSocial = "OSDE" }
                },
                new Turno
                {
                    IdTurno = 103,
                    Fecha = hoy.AddHours(8).AddMinutes(32),
                    Estado = "En Espera",
                    Especialidad = new Especialidad { Nombre = "Emergencias / Guardia" },
                    Paciente = new Paciente { Nombre = "Luis", Apellido = "Martínez", Dni = "28.321.098", ObraSocial = "IOMA" }
                },
                new Turno
                {
                    IdTurno = 104,
                    Fecha = hoy.AddHours(8).AddMinutes(33),
                    Estado = "En Espera",
                    Especialidad = new Especialidad { Nombre = "Emergencias / Guardia" },
                    Paciente = new Paciente { Nombre = "Ana", Apellido = "Rodríguez", Dni = "31.765.432", ObraSocial = "Swiss Medical" }
                },
                new Turno
                {
                    IdTurno = 201,
                    Fecha = hoy.AddHours(9).AddMinutes(0),
                    Estado = "En Espera",
                    Especialidad = new Especialidad { Nombre = "Cardiología" },
                    Paciente = new Paciente { Nombre = "Carlos", Apellido = "Fernández", Dni = "22.456.789", ObraSocial = "PAMI" }
                },
                new Turno
                {
                    IdTurno = 202,
                    Fecha = hoy.AddHours(9).AddMinutes(15),
                    Estado = "En Espera",
                    Especialidad = new Especialidad { Nombre = "Cardiología" },
                    Paciente = new Paciente { Nombre = "Lucía", Apellido = "Gómez", Dni = "27.654.321", ObraSocial = "OSDE" }
                }
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

            var filtrados = _todosLosTurnos.Where(t => (t.Especialidad?.Nombre ?? string.Empty) == servicio && t.Estado == "En Espera");

            IEnumerable<Turno> ordenados;
            if (esEmergencia)
            {
                // Modo Emergencias: prioridad (Alta -> Media -> Baja) y, a igual prioridad, FIFO.
                ordenados = filtrados
                    .OrderBy(t => t.Fecha);
            }
            else
            {
                // Modo Especialidades: orden estricto de llegada (FIFO).
                ordenados = filtrados.OrderBy(t => t.Fecha);
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

            // Guardamos la hora de inicio localmente para no alterar la entidad
            _horaInicioAtencion = DateTime.Now;
            _turnoActual.Estado = "En Consulta";

            CambiarEstadoPuesto(EstadoPuesto.EnConsulta);
        }

        private void btnTerminarAtencion_Click(object sender, EventArgs e)
        {
            if (_turnoActual == null)
                return;

            // Acá es donde en un caso real se persistirían en la base de datos:
            // observaciones, diagnóstico rápido, hora de cierre, médico y sala (trazabilidad).
            _diagnosticoRapido = txtDiagnostico.Text.Trim();
            _horaFinAtencion = DateTime.Now;
            _turnoActual.Estado = "Atendido";
            _medicoQueAtendio = _nombreMedico;
            _salaDeAtencion = _salaAsignada;

            // En la implementación real guardaríamos un registro en HistoriaClinica
            // y/o actualizaríamos el Turno en la base. Aquí sólo movemos el turno a historial.
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
            // Mostrar información basada en las entidades relacionadas (Paciente, Especialidad).
            lblInfoTurno.Text = $"N° Turno: {t.IdTurno}";
            lblInfoPaciente.Text = t.Paciente != null ? $"Paciente: {t.Paciente.Apellido}, {t.Paciente.Nombre}" : "Paciente: -";
            string dni = t.Paciente?.Dni ?? "-";
            string edad = "-"; // El modelo Paciente actual no contiene Edad; mostrar placeholder
            string cobertura = t.Paciente?.ObraSocial ?? "-";
            lblInfoDni.Text = $"DNI / Edad / Cobertura: {dni} / {edad} años / {cobertura}";

            // Motivo/Triage no están modelados en la entidad Turno actual; mostrar placeholder.
            lblInfoMotivo.Text = "Motivo / Prioridad: - /";
            lblInfoPrioridadValor.Text = "-";
            lblInfoPrioridadValor.ForeColor = Color.Black;
            // Se reposiciona a mano al lado del label anterior, porque su ancho
            // cambia según el largo del texto (AutoSize no alinea dos labels solo).
            lblInfoPrioridadValor.Location = new Point(lblInfoMotivo.Right + 4, lblInfoMotivo.Top);

            ActualizarTiempoTranscurrido();

            // Limpiar cuadro de diagnóstico
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

            // Usamos la Fecha del turno como hora de entrada al panel
            DateTime horaEntrada = _turnoActual.Fecha;
            TimeSpan transcurrido = DateTime.Now - horaEntrada;
            int minutos = Math.Max(0, (int)transcurrido.TotalMinutes);

            lblInfoTiempo.Text = $"Hora de Entrada / Tiempo: {horaEntrada:HH:mm} / {minutos} min.";
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
                    mensaje = _turnoActual != null ? $"Esperando inicio de atención para {_turnoActual.Paciente?.Nombre} {_turnoActual.Paciente?.Apellido}." : "";
                    break;
                case EstadoPuesto.EnConsulta:
                    mensaje = _turnoActual != null ? $"Atención en curso con {_turnoActual.Paciente?.Nombre} {_turnoActual.Paciente?.Apellido}." : "";
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
            // Formateo personalizado para columnas: triage (colTriage) y paciente (colPaciente).
            string colName = dgvTurnos.Columns[e.ColumnIndex].Name;

            if (colName == "colTriage")
            {
                // No tenemos Triage persistido en el modelo; dejar vacío o usar datos de TurnoSintomas si los hubiera.
                if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
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
            else if (colName == "colPaciente")
            {
                // Mostrar "Apellido, Nombre (DNI)" en la columna paciente.
                if (e.Value is Paciente p)
                {
                    e.Value = $"{p.Apellido}, {p.Nombre} ({p.Dni})";
                    e.FormattingApplied = true;
                }
            }
        }

        private void lblObservaciones_Click(object sender, EventArgs e)
        {

        }
    }

    // (El modelo Turno fue movido a Modelos.cs) 
}