using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

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

        // Lista de turnos cargados desde la base de datos
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
            cboServicio.Items.Add("Cardiología");
            cboServicio.Items.Add("Traumatología");
            cboServicio.Items.Add("Pediatría");

            cboServicio.SelectedIndex = 0;
            _indiceServicioAnterior = 0;
        }

        /// <summary>
        /// Obtiene los turnos en espera desde la base de datos SQL Server mediante Stored Procedure.
        /// </summary>
        private void CargarTurnosDesdeBD()
        {
            _todosLosTurnos = new List<Turno>();
            _historialAtendidos = new List<Turno>();

            try
            {
                // Stored Procedure: sp_ListarTurnosAtencion
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ListarTurnosAtencion", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int idTurno = Convert.ToInt32(reader["IdTurno"]);
                                string nroOrden = reader["NroOrden"]?.ToString() ?? idTurno.ToString();
                                DateTime fecha = Convert.ToDateTime(reader["Fecha"]);
                                string estado = reader["Estado"]?.ToString() ?? "En Espera";
                                string nomEsp = reader["Especialidad"]?.ToString() ?? "Emergencias / Guardia";

                                string nomPac = reader["NombrePaciente"]?.ToString() ?? "";
                                string apePac = reader["ApellidoPaciente"]?.ToString() ?? "";
                                string dniPac = reader["DniPaciente"]?.ToString() ?? "";
                                string obraPac = reader["ObraSocial"]?.ToString() ?? "";
                                string triage = reader["Triage"]?.ToString() ?? "MEDIA";

                                Turno t = new Turno
                                {
                                    IdTurno = idTurno,
                                    NroOrden = nroOrden,
                                    Fecha = fecha,
                                    Estado = estado,
                                    Especialidad = new Especialidad { Nombre = nomEsp },
                                    Prioridad = new Prioridad { Descripcion = triage },
                                    Paciente = new Paciente
                                    {
                                        Nombre = nomPac,
                                        Apellido = apePac,
                                        Dni = dniPac,
                                        ObraSocial = obraPac
                                    }
                                };

                                _todosLosTurnos.Add(t);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Si la BD aún no tiene turnos o no está accesible, cargar datos iniciales de prueba
                CargarDatosDeEjemplo();
            }
        }

        private void CargarDatosDeEjemplo()
        {
            var hoy = DateTime.Today;
            _todosLosTurnos = new List<Turno>
            {
                new Turno
                {
                    IdTurno = 101,
                    NroOrden = "E-001",
                    Fecha = hoy.AddHours(8).AddMinutes(30),
                    Estado = "En Espera",
                    Especialidad = new Especialidad { Nombre = "Emergencias / Guardia" },
                    Prioridad = new Prioridad { Descripcion = "ALTA" },
                    Paciente = new Paciente { Nombre = "Juan", Apellido = "Pérez", Dni = "30.123.456", ObraSocial = "PAMI" }
                },
                new Turno
                {
                    IdTurno = 102,
                    NroOrden = "E-002",
                    Fecha = hoy.AddHours(8).AddMinutes(31),
                    Estado = "En Espera",
                    Especialidad = new Especialidad { Nombre = "Emergencias / Guardia" },
                    Prioridad = new Prioridad { Descripcion = "MEDIA" },
                    Paciente = new Paciente { Nombre = "María", Apellido = "García", Dni = "25.987.654", ObraSocial = "OSDE" }
                },
                new Turno
                {
                    IdTurno = 201,
                    NroOrden = "C-010",
                    Fecha = hoy.AddHours(9).AddMinutes(0),
                    Estado = "En Espera",
                    Especialidad = new Especialidad { Nombre = "Cardiología" },
                    Prioridad = new Prioridad { Descripcion = "BAJA" },
                    Paciente = new Paciente { Nombre = "Carlos", Apellido = "Fernández", Dni = "22.456.789", ObraSocial = "PAMI" }
                }
            };
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
                // Stored Procedure: sp_LlamarSiguientePaciente
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_LlamarSiguientePaciente", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IdTurno", SqlDbType.Int).Value = _turnoActual.IdTurno;
                        cmd.Parameters.Add("@NombreMedico", SqlDbType.VarChar, 100).Value = _nombreMedico;
                        cmd.Parameters.Add("@SalaAsignada", SqlDbType.VarChar, 100).Value = _salaAsignada;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al registrar llamado en base de datos:\n" + sqlEx.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                // Continuar en memoria si no hay conexión
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
                // Stored Procedure: sp_IniciarAtencionTurno
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_IniciarAtencionTurno", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IdTurno", SqlDbType.Int).Value = _turnoActual.IdTurno;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al registrar inicio de consulta:\n" + sqlEx.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
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
                // Stored Procedure: sp_FinalizarAtencionTurno
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_FinalizarAtencionTurno", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IdTurno", SqlDbType.Int).Value = _turnoActual.IdTurno;
                        cmd.Parameters.Add("@Diagnostico", SqlDbType.VarChar, -1).Value = string.IsNullOrEmpty(_diagnosticoRapido) ? (object)DBNull.Value : _diagnosticoRapido;
                        cmd.Parameters.Add("@NombreMedico", SqlDbType.VarChar, 100).Value = _medicoQueAtendio;
                        cmd.Parameters.Add("@SalaAsignada", SqlDbType.VarChar, 100).Value = _salaDeAtencion;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Atención finalizada con éxito.", "Turno Atendido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al guardar la atención en la base de datos:\n" + sqlEx.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al finalizar atención:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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