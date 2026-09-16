using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

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

        // Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly EspecialidadBLL _especialidadBLL = new EspecialidadBLL();
        private readonly HistoriaClinicaBLL _historiaClinicaBLL = new HistoriaClinicaBLL();

        // Datos del médico autenticado / sala asignada
        private readonly UsuarioLoginResult? _usuarioActual;
        private readonly string _nombreMedico;
        private readonly string _matriculaMedico;
        private readonly string _salaAsignada;

        // Lista de turnos cargados desde la base de datos
        private List<Turno> _todosLosTurnos = new List<Turno>();
        private List<Turno> _historialAtendidos = new List<Turno>();

        private DateTime? _horaInicioAtencion;
        private DateTime? _horaFinAtencion;
        private string _diagnosticoRapido = string.Empty;
        private string _medicoQueAtendio = string.Empty;
        private string _salaDeAtencion = string.Empty;

        private BindingList<Turno> _turnosVisibles;
        private Turno _turnoActual;
        private EstadoPuesto _estadoActual = EstadoPuesto.SinPaciente;

        private int _indiceServicioAnterior = 0;
        private bool _bloqueandoCombo = false;

        public FrmListaTurnosAtencion() : this(null)
        {
        }

        public FrmListaTurnosAtencion(UsuarioLoginResult? usuario)
            : this(
                usuario != null ? $"Dr. {usuario.Nombre} {usuario.Apellido}" : "Médico de Turno",
                "M.N. General",
                "Consultorio de Atención"
            )
        {
            _usuarioActual = usuario;
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
            lblMedicoInfo.Text = $"{_nombreMedico} ({_matriculaMedico})  |  Sala: {_salaAsignada}";
            lblTrazabilidad.Text = $"Trazabilidad: {_nombreMedico} | {_salaAsignada}";

            ConfigurarGrid();
            CargarServiciosDelMedico();
            CargarTurnosDesdeBD();

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
                DataPropertyName = "NroOrden",
                Width = 80
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
                DataPropertyName = "", // formateado en CellFormatting
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

        /// <summary>
        /// Obtiene el catálogo de especialidades disponibles desde la Capa de Negocio (BLL).
        /// </summary>
        private void CargarServiciosDelMedico()
        {
            cboServicio.Items.Clear();

            // Opción fija siempre disponible para guardia/emergencias
            cboServicio.Items.Add("Emergencias / Guardia");

            try
            {
                // Si tenemos un usuario logueado y es un médico (puedes validar por IdRol o simplemente si _usuarioActual no es nulo)
                if (_usuarioActual != null)
                {
                    // Consultamos solo las especialidades asignadas a este médico en la BD
                    var especialidadesMedico = _especialidadBLL.ObtenerEspecialidadesPorMedico(_usuarioActual.IdUsuario);

                    if (especialidadesMedico != null)
                    {
                        foreach (var esp in especialidadesMedico)
                        {
                            if (!string.IsNullOrWhiteSpace(esp.Nombre))
                                cboServicio.Items.Add(esp.Nombre);
                        }
                    }
                }
                else
                {
                    // Fallback por si entra sin sesión (modo pruebas): Carga todas
                    var especialidades = _especialidadBLL.ObtenerEspecialidades();
                    if (especialidades != null)
                    {
                        foreach (var esp in especialidades)
                        {
                            if (!string.IsNullOrWhiteSpace(esp.Nombre))
                                cboServicio.Items.Add(esp.Nombre);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los servicios médicos del profesional:\n" + ex.Message,
                    "Error de Servicios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (cboServicio.Items.Count > 0)
            {
                cboServicio.SelectedIndex = 0;
                _indiceServicioAnterior = 0;
            }
        }

        /// <summary>
        /// Obtiene los turnos en espera desde la Capa de Negocio (BLL), delegando al SP sp_ListarTurnosAtencion.
        /// </summary>
        private void CargarTurnosDesdeBD()
        {
            _todosLosTurnos = new List<Turno>();
            _historialAtendidos = new List<Turno>();

            try
            {
                // BLL delega en TurnoDAL -> sp_ListarTurnosAtencion
                var turnosAtencion = _turnoBLL.ListarTurnosAtencion();

                if (turnosAtencion != null)
                {
                    foreach (var dto in turnosAtencion)
                    {
                        Turno t = new Turno
                        {
                            IdTurno = dto.IdTurno,
                            NroOrden = !string.IsNullOrWhiteSpace(dto.NroOrden) ? dto.NroOrden : dto.IdTurno.ToString(),
                            Fecha = dto.Fecha,
                            Estado = !string.IsNullOrWhiteSpace(dto.Estado) ? dto.Estado : "En Espera",
                            Especialidad = new Especialidad { Nombre = !string.IsNullOrWhiteSpace(dto.Especialidad) ? dto.Especialidad : "Emergencias / Guardia" },
                            Prioridad = new Prioridad { Descripcion = !string.IsNullOrWhiteSpace(dto.Triage) ? dto.Triage : "MEDIA" },
                            Paciente = new Paciente
                            {
                                Nombre = dto.NombrePaciente ?? string.Empty,
                                Apellido = dto.ApellidoPaciente ?? string.Empty,
                                Dni = dto.DniPaciente ?? string.Empty,
                                ObraSocial = dto.ObraSocial ?? string.Empty
                            }
                        };

                        _todosLosTurnos.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron consultar los turnos en espera desde la base de datos:\n" + ex.Message,
                    "Error de Turnos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                ordenados = filtrados.OrderBy(t => PrioridadNumerica(t.Prioridad?.Descripcion ?? "MEDIA")).ThenBy(t => t.Fecha);
            }
            else
            {
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
            switch (triage?.ToUpperInvariant())
            {
                case "ALTA": return 1;
                case "MEDIA": return 2;
                case "BAJA": return 3;
                default: return 4;
            }
        }

        private void cboServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_bloqueandoCombo)
                return;

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

            try
            {
                // BLL delega a TurnoDAL y ejecuta sp_LlamarSiguientePaciente
                _turnoBLL.LlamarSiguientePaciente(_turnoActual.IdTurno, _nombreMedico, _salaAsignada);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Aviso al registrar el llamado en la base de datos:\n" + ex.Message,
                    "Aviso de Comunicación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

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

            _horaInicioAtencion = DateTime.Now;
            _turnoActual.Estado = "En Consulta";

            try
            {
                // BLL delega a TurnoDAL y ejecuta sp_IniciarAtencionTurno
                _turnoBLL.IniciarAtencionTurno(_turnoActual.IdTurno);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Aviso al registrar inicio de consulta en la base de datos:\n" + ex.Message,
                    "Aviso de Comunicación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            CambiarEstadoPuesto(EstadoPuesto.EnConsulta);
        }

        private void btnTerminarAtencion_Click(object sender, EventArgs e)
        {
            if (_turnoActual == null)
                return;

            _diagnosticoRapido = txtDiagnostico.Text.Trim();
            _horaFinAtencion = DateTime.Now;
            _turnoActual.Estado = "Atendido";
            _medicoQueAtendio = _nombreMedico;
            _salaDeAtencion = _salaAsignada;

            try
            {
                // BLL delega a TurnoDAL y ejecuta sp_FinalizarAtencionTurno
                _turnoBLL.FinalizarAtencionTurno(_turnoActual.IdTurno, _diagnosticoRapido, _medicoQueAtendio, _salaDeAtencion);

                MessageBox.Show("Atención médica finalizada con éxito.", "Turno Atendido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo registrar la finalización de la atención:\n" + ex.Message,
                    "Error al finalizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _historialAtendidos.Add(_turnoActual);
            tmrTiempoTranscurrido.Stop();
            _turnoActual = null;

            LimpiarPanelAtencion();
            CambiarEstadoPuesto(EstadoPuesto.SinPaciente);

            // Refrescar lista de turnos desde BD
            CargarTurnosDesdeBD();
            RefrescarListado();
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
            lblInfoTurno.Text = $"N° Turno: {t.NroOrden ?? t.IdTurno.ToString()}";
            lblInfoPaciente.Text = t.Paciente != null ? $"Paciente: {t.Paciente.Apellido}, {t.Paciente.Nombre}" : "Paciente: -";
            string dni = t.Paciente?.Dni ?? "-";
            string edad = "-";
            string cobertura = t.Paciente?.ObraSocial ?? "-";
            lblInfoDni.Text = $"DNI / Edad / Cobertura: {dni} / {edad} / {cobertura}";

            string prioridad = t.Prioridad?.Descripcion ?? "MEDIA";
            lblInfoMotivo.Text = $"Prioridad: {prioridad}";
            lblInfoPrioridadValor.Text = prioridad;
            lblInfoPrioridadValor.ForeColor = ColorSegunTriage(prioridad);
            lblInfoPrioridadValor.Location = new Point(lblInfoMotivo.Right + 4, lblInfoMotivo.Top);

            ActualizarTiempoTranscurrido();
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
            switch (triage?.ToUpperInvariant())
            {
                case "ALTA": return Color.FromArgb(214, 39, 40);
                case "MEDIA": return Color.FromArgb(184, 134, 11);
                case "BAJA": return Color.FromArgb(46, 139, 87);
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

            DateTime horaEntrada = _turnoActual.Fecha;
            TimeSpan transcurrido = DateTime.Now - horaEntrada;
            int minutos = Math.Max(0, (int)transcurrido.TotalMinutes);

            lblInfoTiempo.Text = $"Hora de Entrada / Tiempo: {horaEntrada:HH:mm} / {minutos} min.";
        }

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

        private void dgvTurnos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string colName = dgvTurnos.Columns[e.ColumnIndex].Name;

            if (colName == "colTriage")
            {
                if (dgvTurnos.Rows[e.RowIndex].DataBoundItem is Turno t && t.Prioridad != null)
                {
                    string triage = t.Prioridad.Descripcion ?? "MEDIA";
                    e.Value = triage;
                    e.FormattingApplied = true;

                    switch (triage.ToUpperInvariant())
                    {
                        case "ALTA":
                            e.CellStyle.BackColor = Color.FromArgb(214, 39, 40);
                            e.CellStyle.ForeColor = Color.White;
                            e.CellStyle.Font = new Font(dgvTurnos.Font, FontStyle.Bold);
                            break;
                        case "MEDIA":
                            e.CellStyle.BackColor = Color.FromArgb(255, 204, 0);
                            e.CellStyle.ForeColor = Color.Black;
                            e.CellStyle.Font = new Font(dgvTurnos.Font, FontStyle.Bold);
                            break;
                        case "BAJA":
                            e.CellStyle.BackColor = Color.FromArgb(46, 139, 87);
                            e.CellStyle.ForeColor = Color.White;
                            e.CellStyle.Font = new Font(dgvTurnos.Font, FontStyle.Bold);
                            break;
                    }
                }
            }
            else if (colName == "colPaciente")
            {
                if (e.Value is Paciente p)
                {
                    e.Value = $"{p.Apellido}, {p.Nombre} ({p.Dni})";
                    e.FormattingApplied = true;
                }
            }
        }

        private void lblObservaciones_Click(object? sender, EventArgs e)
        {
        }
    }
}