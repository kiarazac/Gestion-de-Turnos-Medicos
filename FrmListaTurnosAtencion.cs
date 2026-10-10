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
    /// <summary>
    /// Formulario de atención médica integral en consultorio.
    /// Gestiona la llamada al siguiente paciente en espera, el inicio y finalización del acto médico,
    /// la redacción de diagnósticos y prescripción de fármacos, y la persistencia en historia clínica.
    /// </summary>
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
        private readonly SalaBLL _salaBLL = new SalaBLL();
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();

        // Datos del médico autenticado / sala asignada
        private readonly UsuarioLoginResult? _usuarioActual;
        private readonly string _nombreMedico;
        private readonly string _matriculaMedico;
        private string _salaAsignada;

        // Lista de turnos cargados desde la base de datos
        private List<Turno> _todosLosTurnos = new List<Turno>();
        private List<Turno> _historialAtendidos = new List<Turno>();

        private DateTime? _horaInicioAtencion;
        private DateTime? _horaFinAtencion;
        private string _diagnosticoRapido = string.Empty;
        private string _medicoQueAtendio = string.Empty;
        private string _salaDeAtencion = string.Empty;

        private BindingList<Turno>? _turnosVisibles;
        private Turno? _turnoActual;
        private EstadoPuesto _estadoActual = EstadoPuesto.SinPaciente;

        private int _indiceServicioAnterior = 0;
        private bool _bloqueandoCombo = false;

        public FrmListaTurnosAtencion() : this(null)
        {
        }

        public FrmListaTurnosAtencion(UsuarioLoginResult? usuario)
            : this(
                usuario != null ? $"Dr. {usuario.Nombre} {usuario.Apellido}".Trim() : "Médico de Turno",
                "M.N. General",
                string.Empty
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

            bool tieneSala = VerificarYActualizarSalaAbierta();

            ConfigurarGrid();
            CargarServiciosDelMedico();
            CargarTurnosDesdeBD();

            RefrescarListado();
            LimpiarPanelAtencion();
            CambiarEstadoPuesto(EstadoPuesto.SinPaciente);

            if (!tieneSala)
            {
                MessageBox.Show("Aviso: No tienes ninguna sala de atención abierta en este momento.\n\nPara poder llamar y atender pacientes, debes abrir tu consultorio asignado desde el menú 'Mis Salas'.",
                    "Sala Requerida para Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Comprueba si el médico tiene una sala de atención abierta y actualiza las etiquetas visuales.
        /// </summary>
        /// <returns><c>true</c> si tiene una sala abierta válida; de lo contrario, <c>false</c>.</returns>
        private bool VerificarYActualizarSalaAbierta()
        {
            if (_usuarioActual != null && _usuarioActual.IdUsuario > 0)
            {
                var salaAbierta = _salaBLL.ObtenerSalaAbiertaPorMedico(_usuarioActual.IdUsuario);
                if (salaAbierta != null)
                {
                    _salaAsignada = salaAbierta.NombreSala;
                    bool esDisponible = salaAbierta.EstadoSala.Equals("Disponible", StringComparison.OrdinalIgnoreCase) ||
                                       salaAbierta.EstadoSala.Equals("Libre", StringComparison.OrdinalIgnoreCase);

                    string estadoTexto = esDisponible ? string.Empty : $" ({salaAbierta.EstadoSala})";
                    lblMedicoInfo.Text = $"{_nombreMedico} ({_matriculaMedico})  |  Sala: {_salaAsignada}{estadoTexto}";
                    lblTrazabilidad.Text = $"Trazabilidad: {_nombreMedico} | {_salaAsignada}{estadoTexto}";
                    return true;
                }
            }
            else if (!string.IsNullOrWhiteSpace(_salaAsignada) && 
                     !_salaAsignada.Equals("(Sin sala abierta)", StringComparison.OrdinalIgnoreCase) &&
                     !_salaAsignada.Equals("Consultorio de Atención", StringComparison.OrdinalIgnoreCase))
            {
                bool esDisponible = _salaBLL.EsSalaDisponible(_salaAsignada);
                string estadoTexto = esDisponible ? string.Empty : " (Ocupada)";
                lblMedicoInfo.Text = $"{_nombreMedico} ({_matriculaMedico})  |  Sala: {_salaAsignada}{estadoTexto}";
                lblTrazabilidad.Text = $"Trazabilidad: {_nombreMedico} | {_salaAsignada}{estadoTexto}";
                return true;
            }

            _salaAsignada = "(Sin sala abierta)";
            lblMedicoInfo.Text = $"{_nombreMedico} ({_matriculaMedico})  |  Sala: (Sin sala abierta)";
            lblTrazabilidad.Text = $"Trazabilidad: {_nombreMedico} | (Sin sala abierta)";
            return false;
        }

        /// <summary>
        /// Indica si el puesto cuenta con una sala abierta activa.
        /// </summary>
        private bool TieneSalaAbierta()
        {
            return !string.IsNullOrWhiteSpace(_salaAsignada) && 
                   !_salaAsignada.Equals("(Sin sala abierta)", StringComparison.OrdinalIgnoreCase) &&
                   !_salaAsignada.Equals("Consultorio de Atención", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Comprueba si la sala asignada se encuentra actualmente disponible ('Disponible' o 'Libre')
        /// para llamar y recibir nuevos pacientes.
        /// </summary>
        private bool TieneSalaDisponible()
        {
            if (!TieneSalaAbierta())
                return false;

            if (_usuarioActual != null && _usuarioActual.IdUsuario > 0)
            {
                var salaDisponible = _salaBLL.ObtenerSalaDisponiblePorMedico(_usuarioActual.IdUsuario);
                return salaDisponible != null;
            }

            return _salaBLL.EsSalaDisponible(_salaAsignada);
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
        /// Restringe el servicio de guardia/emergencia exclusivamente a médicos con especialidad Clínico.
        /// </summary>
        private void CargarServiciosDelMedico()
        {
            cboServicio.Items.Clear();

            int idUsuario = ObtenerIdUsuarioMedico();
            bool esClinico = false;

            try
            {
                if (idUsuario > 0)
                {
                    esClinico = _turnoBLL.PuedeAtenderEmergencias(idUsuario);
                }

                // Regla de Negocio: Solo profesionales médicos con especialidad Clínico pueden atender turnos de emergencia
                if (esClinico)
                {
                    cboServicio.Items.Add("Emergencias / Guardia");
                }

                // Si tenemos un usuario logueado y es un médico
                if (_usuarioActual != null)
                {
                    // Consultamos solo las especialidades asignadas a este médico en la BD
                    var especialidadesMedico = _especialidadBLL.ObtenerEspecialidadesPorMedico(_usuarioActual.IdUsuario);

                    if (especialidadesMedico != null)
                    {
                        foreach (var esp in especialidadesMedico)
                        {
                            if (!string.IsNullOrWhiteSpace(esp.Nombre))
                            {
                                if (!cboServicio.Items.Contains(esp.Nombre))
                                    cboServicio.Items.Add(esp.Nombre);
                            }
                        }
                    }
                }
                else
                {
                    // Fallback por si entra sin sesión (modo pruebas)
                    var especialidades = _especialidadBLL.ObtenerEspecialidades();
                    if (especialidades != null)
                    {
                        foreach (var esp in especialidades)
                        {
                            if (!string.IsNullOrWhiteSpace(esp.Nombre))
                            {
                                if (!cboServicio.Items.Contains(esp.Nombre))
                                    cboServicio.Items.Add(esp.Nombre);
                            }
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
                btnSiguientePaciente.Enabled = true;
            }
            else
            {
                btnSiguientePaciente.Enabled = false;
                btnIniciarAtencion.Enabled = false;
                btnTerminarAtencion.Enabled = false;
                lblEstadoInferior.Text = "Aviso: No posee servicios médicos ni especialidades activas para atención.";
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
                        bool esEmergenciaTurno = dto.NroOrden?.StartsWith("E-", StringComparison.OrdinalIgnoreCase) == true
                            || (dto.Especialidad?.StartsWith("Emergencia", StringComparison.OrdinalIgnoreCase) == true);

                        Turno t = new Turno
                        {
                            IdTurno = dto.IdTurno,
                            NroOrden = !string.IsNullOrWhiteSpace(dto.NroOrden) ? dto.NroOrden : dto.IdTurno.ToString(),
                            Fecha = dto.Fecha,
                            Horario = dto.Fecha.TimeOfDay,
                            Estado = !string.IsNullOrWhiteSpace(dto.Estado) ? dto.Estado : "En Espera",
                            TipoTurno = esEmergenciaTurno ? "Emergencia" : "Especialidad",
                            Especialidad = new Especialidad { Nombre = !string.IsNullOrWhiteSpace(dto.Especialidad) ? dto.Especialidad : "Emergencias / Guardia" },
                            Prioridad = new Prioridad { Descripcion = !string.IsNullOrWhiteSpace(dto.Triage) ? dto.Triage : "MEDIA" },
                            Paciente = new Paciente
                            {
                                Nombre = dto.NombrePaciente ?? string.Empty,
                                Apellido = dto.ApellidoPaciente ?? string.Empty,
                                Dni = dto.DniPaciente ?? string.Empty,
                                ObraSocial = new ObraSocial { Nombre = dto.ObraSocial ?? string.Empty }
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

            // 1. Identificamos si el filtro activo es de Guardia / Emergencias
            bool esEmergencia = servicio.StartsWith("Emergencia", StringComparison.OrdinalIgnoreCase);

            // 2. Filtramos los turnos en espera: para emergencias admitimos tanto 'Emergencia' como 'Emergencias / Guardia'
            var filtrados = _todosLosTurnos.Where(t =>
                t.Estado == "En Espera" &&
                (esEmergencia
                    ? (t.Especialidad?.Nombre?.StartsWith("Emergencia", StringComparison.OrdinalIgnoreCase) == true)
                    : (t.Especialidad?.Nombre ?? string.Empty).Equals(servicio, StringComparison.OrdinalIgnoreCase))
            );

            // 3. Ordenamos: emergencias según gravedad de triage y orden de llegada; especialidades según fecha y hora
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
            if (!VerificarYActualizarSalaAbierta())
            {
                MessageBox.Show("No puedes llamar pacientes porque no tienes ninguna sala abierta activa.\n\nPor favor, dirígete al módulo 'Mis Salas' y abre tu consultorio asignado antes de iniciar la atención.",
                    "Sala Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CambiarEstadoPuesto(EstadoPuesto.SinPaciente);
                return;
            }

            if (!TieneSalaDisponible())
            {
                MessageBox.Show($"No puedes llamar a un nuevo paciente porque la sala '{_salaAsignada}' se encuentra actualmente ocupada por otra atención médica en curso.\n\nEspera a que finalice la consulta o solicita su liberación.",
                    "Sala Ocupada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CambiarEstadoPuesto(EstadoPuesto.SinPaciente);
                return;
            }

            if (_turnosVisibles == null || _turnosVisibles.Count == 0)
                return;

            _turnoActual = _turnosVisibles[0];

            int idUsuario = ObtenerIdUsuarioMedico();
            bool esTurnoEmergencia = _turnoActual.TipoTurno?.Equals("Emergencia", StringComparison.OrdinalIgnoreCase) == true
                || _turnoActual.Especialidad?.Nombre?.StartsWith("Emergencia", StringComparison.OrdinalIgnoreCase) == true
                || _turnoActual.NroOrden?.StartsWith("E-", StringComparison.OrdinalIgnoreCase) == true;

            if (esTurnoEmergencia && idUsuario > 0 && !_turnoBLL.PuedeAtenderEmergencias(idUsuario))
            {
                MessageBox.Show("Acción Denegada: Solo los profesionales médicos con especialidad en Clínica Médica (Clínico) están autorizados para atender turnos de guardia/emergencia.",
                    "Restricción de Especialidad Médica", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _turnoActual = null;
                CambiarEstadoPuesto(EstadoPuesto.SinPaciente);
                return;
            }

            _turnoActual.Estado = "Llamado";

            try
            {
                // BLL delega a TurnoDAL y ejecuta sp_LlamarSiguienteTurno con la sala real abierta y el id del médico
                _turnoBLL.LlamarSiguientePaciente(_turnoActual.IdTurno, _nombreMedico, _salaAsignada, idUsuario > 0 ? idUsuario : null);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Acción Denegada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _turnoActual = null;
                VerificarYActualizarSalaAbierta();
                CambiarEstadoPuesto(EstadoPuesto.SinPaciente);
                return;
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
            if (!VerificarYActualizarSalaAbierta())
            {
                MessageBox.Show("No puedes iniciar la atención porque no tienes ninguna sala abierta activa.\n\nPor favor, abre tu sala asignada desde el módulo 'Mis Salas'.",
                    "Sala Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CambiarEstadoPuesto(EstadoPuesto.SinPaciente);
                return;
            }

            if (_turnoActual == null)
                return;

            int idUsuario = ObtenerIdUsuarioMedico();
            bool esTurnoEmergencia = _turnoActual.TipoTurno?.Equals("Emergencia", StringComparison.OrdinalIgnoreCase) == true
                || _turnoActual.Especialidad?.Nombre?.StartsWith("Emergencia", StringComparison.OrdinalIgnoreCase) == true
                || _turnoActual.NroOrden?.StartsWith("E-", StringComparison.OrdinalIgnoreCase) == true;

            if (esTurnoEmergencia && idUsuario > 0 && !_turnoBLL.PuedeAtenderEmergencias(idUsuario))
            {
                MessageBox.Show("Acción Denegada: Solo los profesionales médicos con especialidad en Clínica Médica (Clínico) están autorizados para atender turnos de guardia/emergencia.",
                    "Restricción de Especialidad Médica", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CambiarEstadoPuesto(EstadoPuesto.SinPaciente);
                return;
            }

            _horaInicioAtencion = DateTime.Now;
            _turnoActual.Estado = "En Consulta";

            try
            {
                // Pasamos el Id del turno y el texto de la sala real del médico logueado
                _turnoBLL.IniciarAtencionTurno(_turnoActual.IdTurno, _salaAsignada);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Aviso al registrar inicio de consulta en la base de datos:\n" + ex.Message,
                    "Aviso de Comunicación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Consultar antecedentes y antecedentes previos de la historia clínica
            CargarHistoriaClinicaPrevia(_turnoActual);

            CambiarEstadoPuesto(EstadoPuesto.EnConsulta);

            txtDiagnostico.Focus();
        }

        /// <summary>
        /// Consulta y formatea en el visor los antecedentes clínicos previos del paciente correspondiente al turno iniciado.
        /// </summary>
        /// <param name="turno">Entidad del turno en curso.</param>
        private void CargarHistoriaClinicaPrevia(Turno turno)
        {
            txtHistoriaPrevia.Clear();

            int idPaciente = ObtenerIdPacienteDelTurno(turno);
            if (idPaciente <= 0)
            {
                txtHistoriaPrevia.Text = "(No se pudo determinar el identificador del paciente)";
                return;
            }

            try
            {
                var antecedentes = _historiaClinicaBLL.ObtenerHistoriaClinicaPaciente(idPaciente);

                if (antecedentes != null && antecedentes.Count > 0)
                {
                    var sb = new System.Text.StringBuilder();
                    foreach (var h in antecedentes)
                    {
                        string medico = !string.IsNullOrWhiteSpace(h.ApellidoMedico)
                            ? $"Dr. {h.ApellidoMedico} {h.NombreMedico}".Trim()
                            : "Médico tratante";

                        sb.AppendLine($"• [{h.Fecha:dd/MM/yyyy HH:mm}] {medico} ({h.TipoTurno})");
                        sb.AppendLine($"  Diagnóstico: {h.DiagRapido}");
                        if (!string.IsNullOrWhiteSpace(h.DescripHistoriaClinica) && !h.DescripHistoriaClinica.Equals(h.DiagRapido, StringComparison.OrdinalIgnoreCase))
                        {
                            sb.AppendLine($"  Evolución: {h.DescripHistoriaClinica}");
                        }
                        if (!string.IsNullOrWhiteSpace(h.RecetaMedicamentos))
                        {
                            sb.AppendLine($"  Receta: {h.RecetaMedicamentos}");
                        }
                        sb.AppendLine(new string('-', 35));
                    }
                    txtHistoriaPrevia.Text = sb.ToString();
                    txtHistoriaPrevia.SelectionStart = 0;
                    txtHistoriaPrevia.ScrollToCaret();
                }
                else
                {
                    txtHistoriaPrevia.Text = "(El paciente no registra historia clínica previa cargada)";
                }
            }
            catch (Exception ex)
            {
                txtHistoriaPrevia.Text = $"(Error al consultar historia clínica previa: {ex.Message})";
            }
        }

        /// <summary>
        /// Resuelve el identificador único de paciente a partir del turno o de su DNI.
        /// </summary>
        /// <param name="turno">Objeto turno analizado.</param>
        /// <returns>ID numérico del paciente o 0 si no se pudo determinar.</returns>
        private int ObtenerIdPacienteDelTurno(Turno? turno)
        {
            if (turno == null) return 0;
            if (turno.IdPaciente > 0) return turno.IdPaciente;
            if (turno.Paciente != null && turno.Paciente.IdPaciente > 0) return turno.Paciente.IdPaciente;

            try
            {
                using (var context = new dbTurnosMedicos())
                {
                    var id = context.Turnos.Where(x => x.IdTurno == turno.IdTurno).Select(x => x.IdPaciente).FirstOrDefault();
                    if (id > 0)
                    {
                        turno.IdPaciente = id;
                        if (turno.Paciente != null) turno.Paciente.IdPaciente = id;
                        return id;
                    }

                    if (turno.Paciente != null && !string.IsNullOrWhiteSpace(turno.Paciente.Dni))
                    {
                        id = context.Pacientes.Where(p => p.Dni == turno.Paciente.Dni).Select(p => p.IdPaciente).FirstOrDefault();
                        if (id > 0)
                        {
                            turno.IdPaciente = id;
                            turno.Paciente.IdPaciente = id;
                            return id;
                        }
                    }
                }
            }
            catch
            {
                // Fallback silencioso
            }

            return 0;
        }

        /// <summary>
        /// Obtiene el identificador del médico actuante a partir de la sesión autenticada o de la base de datos como contingencia.
        /// </summary>
        /// <returns>ID del usuario médico.</returns>
        private int ObtenerIdUsuarioMedico()
        {
            if (_usuarioActual != null && _usuarioActual.IdUsuario > 0)
                return _usuarioActual.IdUsuario;

            try
            {
                var medicos = _usuarioBLL.ObtenerPersonalMedico();
                if (medicos != null && medicos.Count > 0)
                {
                    return medicos[0].IdUsuario;
                }
            }
            catch
            {
            }

            return 1;
        }

        private void btnTerminarAtencion_Click(object sender, EventArgs e)
        {
            if (_turnoActual == null)
                return;

            _diagnosticoRapido = txtDiagnostico.Text.Trim();
            string receta = txtReceta.Text.Trim();

            if (string.IsNullOrWhiteSpace(_diagnosticoRapido))
            {
                MessageBox.Show("Por favor, ingrese el diagnóstico médico antes de finalizar la atención.",
                    "Diagnóstico Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiagnostico.Focus();
                return;
            }

            _horaFinAtencion = DateTime.Now;
            _turnoActual.Estado = "Atendido";
            _medicoQueAtendio = _nombreMedico;
            _salaDeAtencion = _salaAsignada;

            int idPaciente = ObtenerIdPacienteDelTurno(_turnoActual);
            int idUsuario = ObtenerIdUsuarioMedico();

            bool esTurnoEmergencia = _turnoActual.TipoTurno?.Equals("Emergencia", StringComparison.OrdinalIgnoreCase) == true
                || _turnoActual.Especialidad?.Nombre?.StartsWith("Emergencia", StringComparison.OrdinalIgnoreCase) == true
                || _turnoActual.NroOrden?.StartsWith("E-", StringComparison.OrdinalIgnoreCase) == true;

            if (esTurnoEmergencia && idUsuario > 0 && !_turnoBLL.PuedeAtenderEmergencias(idUsuario))
            {
                MessageBox.Show("Acción Denegada: Solo los profesionales médicos con especialidad en Clínica Médica (Clínico) están autorizados para atender turnos de guardia/emergencia.",
                    "Restricción de Especialidad Médica", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. BLL delega a TurnoDAL y ejecuta sp_FinalizarAtencionTurno
                _turnoBLL.FinalizarAtencionTurno(_turnoActual.IdTurno, _diagnosticoRapido, _medicoQueAtendio, _salaDeAtencion);

                // 2. Registrar la evolución en la Historia Clínica del paciente
                if (idPaciente > 0 && idUsuario > 0)
                {
                    string servicio = cboServicio.SelectedItem?.ToString() ?? "Consulta";
                    string tipoTurno = servicio.StartsWith("Emergencia", StringComparison.OrdinalIgnoreCase) ? "Emergencia" : "Especialidad";

                    _historiaClinicaBLL.RegistrarHistoria(
                        tipoTurno: tipoTurno,
                        diagRapido: _diagnosticoRapido,
                        descripHistoriaClinica: _diagnosticoRapido,
                        recetaMedicamentos: receta,
                        idPaciente: idPaciente,
                        idTurno: _turnoActual.IdTurno,
                        idUsuario: idUsuario
                    );
                }

                MessageBox.Show("Atención médica finalizada e Historia Clínica registrada con éxito.",
                    "Turno Atendido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo registrar la finalización de la atención o historia clínica:\n" + ex.Message,
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
                    bool salaDisponible = TieneSalaDisponible();
                    btnSiguientePaciente.Enabled = salaDisponible && _turnosVisibles != null && _turnosVisibles.Count > 0;
                    btnIniciarAtencion.Enabled = false;
                    btnTerminarAtencion.Enabled = false;
                    cboServicio.Enabled = true;

                    if (TieneSalaAbierta() && !salaDisponible)
                    {
                        lblAvisoBloqueo.Text = $"Atención: La sala '{_salaAsignada}' se encuentra actualmente ocupada por otra consulta.";
                        lblAvisoBloqueo.Visible = true;
                    }
                    else
                    {
                        lblAvisoBloqueo.Visible = false;
                    }
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
            string cobertura = t.Paciente?.ObraSocial?.Nombre ?? "-";
            lblInfoDni.Text = $"DNI / Edad / Cobertura: {dni} / {edad} / {cobertura}";

            string prioridad = t.Prioridad?.Descripcion ?? "MEDIA";
            lblInfoMotivo.Text = $"Prioridad: {prioridad}";
            lblInfoPrioridadValor.Text = prioridad;
            lblInfoPrioridadValor.ForeColor = ColorSegunTriage(prioridad);
            lblInfoPrioridadValor.Location = new Point(lblInfoMotivo.Right + 4, lblInfoMotivo.Top);

            ActualizarTiempoTranscurrido();
            txtHistoriaPrevia.Clear();
            txtDiagnostico.Clear();
            txtReceta.Clear();
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
            txtHistoriaPrevia.Clear();
            txtDiagnostico.Clear();
            txtReceta.Clear();
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
            if (horaEntrada.TimeOfDay == TimeSpan.Zero && _turnoActual.Horario != TimeSpan.Zero)
            {
                horaEntrada = horaEntrada.Date + _turnoActual.Horario;
            }
            else if (horaEntrada.TimeOfDay == TimeSpan.Zero && _turnoActual.FechaCreacion != default)
            {
                horaEntrada = _turnoActual.FechaCreacion;
            }

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
            else if (colName == "colHora")
            {
                if (dgvTurnos.Rows[e.RowIndex].DataBoundItem is Turno t)
                {
                    if (t.Fecha.TimeOfDay != TimeSpan.Zero)
                    {
                        e.Value = t.Fecha.ToString("HH:mm");
                        e.FormattingApplied = true;
                    }
                    else if (t.Horario != TimeSpan.Zero)
                    {
                        e.Value = t.Horario.ToString(@"hh\:mm");
                        e.FormattingApplied = true;
                    }
                    else if (t.FechaCreacion != default && t.FechaCreacion.TimeOfDay != TimeSpan.Zero)
                    {
                        e.Value = t.FechaCreacion.ToString("HH:mm");
                        e.FormattingApplied = true;
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