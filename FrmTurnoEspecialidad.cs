using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario para la gestión y programación de turnos médicos por especialidad.
    /// Permite la búsqueda de pacientes por DNI, selección de especialidad médica, consulta de horarios disponibles y emisión del ticket de turno.
    /// </summary>
    public partial class FrmTurnoEspecialidad : Form
    {
        private readonly EspecialidadBLL _especialidadBLL = new EspecialidadBLL();
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly PacienteBLL _pacienteBLL = new PacienteBLL();

        private int? idPacienteActual = null;
        private DatosComprobanteTurno? _ultimoTurnoEmitido = null; // Almacena los datos del último turno generado para exportar

        public FrmTurnoEspecialidad()
        {
            InitializeComponent();

            this.Load += FrmTurnoEspecialidad_Load;
            this.button1.Click += BtnGenerarTurno_Click;
            this.btnDescargarTxt.Click += BtnDescargarTxt_Click;

            txtDNI.Leave += TxtDNI_Leave;
            txtDNI.KeyDown += TxtDNI_KeyDown;
        }

        private void FrmTurnoEspecialidad_Load(object? sender, EventArgs e)
        {
            calFechaTurno.MinDate = DateTime.Today;
            CargarEspecialidades();

            Lid_turno.Text = "# --";
            Ldescrip_turno_especialidad.Text = "Especialidad";
        }

        private void TxtDNI_Leave(object sender, EventArgs e)
        {
            BuscarYAutocompletarPaciente();
        }

        private void TxtDNI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BuscarYAutocompletarPaciente();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void BuscarYAutocompletarPaciente()
        {
            string dniBuscado = txtDNI.Text.Trim();
            if (string.IsNullOrEmpty(dniBuscado)) return;

            try
            {
                var paciente = _turnoBLL.BuscarPacientePorDNI(dniBuscado);
                if (paciente != null)
                {
                    // CASO 1: Paciente existente -> Autocompleta y bloquea edición
                    idPacienteActual = paciente.IdPaciente;
                    txtNombre.Text = paciente.Nombre;
                    txtApellido.Text = paciente.Apellido;
                    txtObraSocial.Text = paciente.ObraSocial;

                    txtNombre.ReadOnly = true;
                    txtApellido.ReadOnly = true;
                    txtObraSocial.ReadOnly = true;
                }
                else
                {
                    // CASO 2: Paciente nuevo -> Limpia y habilita los campos para el registro
                    idPacienteActual = null;
                    txtNombre.Clear();
                    txtApellido.Clear();
                    txtObraSocial.Clear();

                    txtNombre.ReadOnly = false;
                    txtApellido.ReadOnly = false;
                    txtObraSocial.ReadOnly = false;
                    txtNombre.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el paciente:\n{ex.Message}", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CargarEspecialidades()
        {
            cmbEspecialidad.Items.Clear();
            cmbEspecialidad.Items.Add("Seleccione especialidad...");

            try
            {
                var especialidades = _especialidadBLL.ObtenerEspecialidades();
                if (especialidades != null)
                {
                    foreach (var esp in especialidades)
                    {
                        if (!string.IsNullOrWhiteSpace(esp.Nombre))
                            cmbEspecialidad.Items.Add(esp.Nombre);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las especialidades médicas:\n" + ex.Message, "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            cmbEspecialidad.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            calFechaTurno.RemoveAllBoldedDates();
            cmbHorarios.Items.Clear();

            if (cmbEspecialidad.SelectedIndex <= 0)
                return;

            DateTime hoy = DateTime.Today;
            calFechaTurno.BoldedDates = new DateTime[] { hoy.AddDays(1), hoy.AddDays(2), hoy.AddDays(3), hoy.AddDays(4), hoy.AddDays(5) };
            calFechaTurno.UpdateBoldedDates();

            // Si ya hay una fecha seleccionada en el calendario, cargamos los horarios disponibles
            if (calFechaTurno.SelectionStart.Date >= DateTime.Today)
            {
                string especialidad = cmbEspecialidad.SelectedItem?.ToString() ?? string.Empty;
                CargarHorariosDisponibles(especialidad, calFechaTurno.SelectionStart.Date);
            }
        }

        private void calFechaTurno_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (cmbEspecialidad.SelectedIndex <= 0)
            {
                cmbHorarios.Items.Clear();
                MessageBox.Show("Por favor, seleccione una especialidad antes de elegir la fecha.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string especialidad = cmbEspecialidad.SelectedItem?.ToString() ?? string.Empty;
            CargarHorariosDisponibles(especialidad, e.Start.Date);
        }

        /// <summary>
        /// Consulta y carga en el selector <see cref="cmbHorarios"/> las franjas horarias libres para la especialidad y fecha especificadas.
        /// </summary>
        /// <param name="especialidad">Nombre de la especialidad seleccionada.</param>
        /// <param name="fechaElegida">Fecha seleccionada en el calendario.</param>
        private void CargarHorariosDisponibles(string especialidad, DateTime fechaElegida)
        {
            cmbHorarios.Items.Clear();

            if (string.IsNullOrWhiteSpace(especialidad) || cmbEspecialidad.SelectedIndex <= 0)
                return;

            try
            {
                var horarios = _turnoBLL.ObtenerHorariosDisponibles(especialidad, fechaElegida);

                if (horarios != null && horarios.Count > 0)
                {
                    foreach (var h in horarios)
                    {
                        if (!string.IsNullOrWhiteSpace(h.Horario))
                            cmbHorarios.Items.Add(h.Horario);
                    }
                    cmbHorarios.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("No hay turnos disponibles para esta fecha y especialidad.", "Sin disponibilidad", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron consultar los horarios disponibles:\n" + ex.Message, "Error de Horarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGenerarTurno_Click(object? sender, EventArgs e)
        {
            if (!ValidarDatos())
                return;

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string dni = txtDNI.Text.Trim();
            string obraSocial = txtObraSocial.Text.Trim();
            string especialidad = cmbEspecialidad.SelectedItem!.ToString()!;
            DateTime fecha = calFechaTurno.SelectionStart.Date;
            string horario = cmbHorarios.SelectedItem!.ToString()!;

            try
            {
                int idPaciente;

                // Si el paciente ya existe usamos su ID; si es nuevo, lo guardamos automáticamente en la BD
                if (idPacienteActual.HasValue)
                {
                    idPaciente = idPacienteActual.Value;
                }
                else
                {
                    idPaciente = _pacienteBLL.GuardarPaciente(nombre, apellido, dni, obraSocial);
                    idPacienteActual = idPaciente;
                }

                // Registro del turno con la nueva inicial dinámica en el SP
                var resultadoTurno = _turnoBLL.CrearTurnoEspecialidad(idPaciente, especialidad, fecha, horario, "En Espera");
                string nroOrden = resultadoTurno.NroOrden ?? $"T-{resultadoTurno.IdNuevoTurno:D3}";

                Lid_turno.Text = $"# {nroOrden}";
                Ldescrip_turno_especialidad.Text = especialidad;

                // Guardamos los datos del comprobante para su exportación a .txt
                _ultimoTurnoEmitido = new DatosComprobanteTurno
                {
                    NroOrden = nroOrden,
                    Prioridad = "NORMAL",
                    Seccion = especialidad,
                    FechaEmision = DateTime.Now,
                    NombrePaciente = $"{apellido}, {nombre}",
                    DniPaciente = dni,
                    ObraSocial = string.IsNullOrWhiteSpace(obraSocial) ? "Particular / Ninguna" : obraSocial,
                    FechaTurnoProgramado = fecha.ToString("dd/MM/yyyy"),
                    HorarioTurnoProgramado = horario
                };

                // Habilitamos el botón de descarga del comprobante
                btnDescargarTxt.Enabled = true;

                MessageBox.Show(
                    $"¡Turno programado con éxito!\n\n" +
                    $"Paciente: {apellido}, {nombre}\n" +
                    $"Especialidad: {especialidad}\n" +
                    $"Fecha: {fecha:dd/MM/yyyy} a las {horario} hs\n" +
                    $"N° Turno: {nroOrden}",
                    "Turno Generado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LimpiarCampos();
            }
            catch (InvalidOperationException invEx)
            {
                MessageBox.Show(invEx.Message, "Turno No Disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CargarHorariosDisponibles(especialidad, fecha);
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Ya existe un turno reservado") || ex.InnerException?.Message.Contains("Ya existe un turno reservado") == true)
                {
                    MessageBox.Show("Ya existe un turno reservado para la misma fecha, horario y especialidad médica.\nPor favor, elija otro horario disponible.", "Turno No Disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CargarHorariosDisponibles(especialidad, fecha);
                }
                else
                {
                    MessageBox.Show("Ocurrió un error al registrar el turno de especialidad:\n" + ex.Message, "Error Inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Manejador del evento click para generar y descargar el archivo .txt con los datos del turno programado emitido.
        /// </summary>
        private void BtnDescargarTxt_Click(object? sender, EventArgs e)
        {
            if (_ultimoTurnoEmitido == null)
            {
                MessageBox.Show("No hay ningún turno emitido recientemente para descargar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Archivo de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                    sfd.FileName = $"Turno_{_ultimoTurnoEmitido.NroOrden}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    sfd.Title = "Guardar Comprobante de Turno de Especialidad";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(sfd.FileName, _ultimoTurnoEmitido.GenerarContenidoTxt());
                        MessageBox.Show("¡Comprobante de turno generado y guardado exitosamente!", "Descarga Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el comprobante de turno:\n{ex.Message}", "Error de Archivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtDNI.Text) ||
                string.IsNullOrWhiteSpace(txtObraSocial.Text))
            {
                MessageBox.Show("Por favor, complete todos los datos del paciente.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Validaciones.EsNombreValido(txtNombre.Text.Trim()))
            {
                MessageBox.Show("El Nombre contiene caracteres inválidos. Solo se admiten letras.", "Nombre inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (!Validaciones.EsNombreValido(txtApellido.Text.Trim()))
            {
                MessageBox.Show("El Apellido contiene caracteres inválidos. Solo se admiten letras.", "Apellido inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtDNI.Text.Trim(), @"^\d{7,8}$"))
            {
                MessageBox.Show("El DNI debe tener entre 7 y 8 números.", "DNI inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDNI.Focus();
                return false;
            }

            if (cmbEspecialidad.SelectedIndex <= 0)
            {
                MessageBox.Show("Por favor, seleccione una especialidad.", "Falta especialidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbEspecialidad.Focus();
                return false;
            }

            if (cmbHorarios.SelectedItem == null || string.IsNullOrWhiteSpace(cmbHorarios.SelectedItem.ToString()))
            {
                MessageBox.Show("Por favor, seleccione un horario para el turno.", "Falta horario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbHorarios.Focus();
                return false;
            }

            // Control de concurrencia/duplicidad: Validar que no exista un turno registrado con la misma fecha, horario y especialidad
            string especialidad = cmbEspecialidad.SelectedItem?.ToString() ?? string.Empty;
            DateTime fecha = calFechaTurno.SelectionStart.Date;
            string horario = cmbHorarios.SelectedItem?.ToString() ?? string.Empty;

            if (_turnoBLL.ExisteTurnoEspecialidad(especialidad, fecha, horario))
            {
                MessageBox.Show(
                    $"Ya existe un turno reservado para la especialidad '{especialidad}' en la fecha {fecha:dd/MM/yyyy} a las {horario} hs.\n\nPor favor, seleccione otro horario o fecha disponible.",
                    "Turno No Disponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                CargarHorariosDisponibles(especialidad, fecha);
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtDNI.Clear();
            txtObraSocial.Clear();

            txtNombre.ReadOnly = false;
            txtApellido.ReadOnly = false;
            txtObraSocial.ReadOnly = false;

            idPacienteActual = null;
            cmbEspecialidad.SelectedIndex = 0;
            cmbHorarios.Items.Clear();
            txtDNI.Focus();
        }
    }
}