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
        private readonly ObraSocialBLL _obraSocialBLL = new ObraSocialBLL();

        private int? idPacienteActual = null;
        private DatosComprobanteTurno? _ultimoTurnoEmitido = null; // Almacena los datos del último turno generado para exportar

        public FrmTurnoEspecialidad()
        {
            InitializeComponent();

            this.Load += FrmTurnoEspecialidad_Load;
            this.button1.Click += BtnGenerarTurno_Click;
            this.btnDescargarTxt.Click += BtnDescargarTxt_Click;
            this.btnCancelarTurno.Click += BtnCancelarTurno_Click;

            txtDNI.Leave += TxtDNI_Leave;
            txtDNI.KeyDown += TxtDNI_KeyDown;
        }

        private void FrmTurnoEspecialidad_Load(object? sender, EventArgs e)
        {
            calFechaTurno.MinDate = DateTime.Today;
            CargarEspecialidades();
            CargarObrasSociales();

            Lid_turno.Text = "# --";
            Ldescrip_turno_especialidad.Text = "Especialidad";
            lblMontoCobro.Text = "Arancel: $ --";
            lblMontoCobro.ForeColor = SystemColors.ControlDarkDark;
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

                    if (paciente.IdObraSocial.HasValue && paciente.IdObraSocial.Value > 0)
                    {
                        cmbObraSocial.SelectedValue = paciente.IdObraSocial.Value;
                    }
                    else if (!string.IsNullOrWhiteSpace(paciente.ObraSocial))
                    {
                        int idx = cmbObraSocial.FindStringExact(paciente.ObraSocial);
                        if (idx >= 0) cmbObraSocial.SelectedIndex = idx;
                    }

                    txtNombre.ReadOnly = true;
                    txtApellido.ReadOnly = true;
                    cmbObraSocial.Enabled = true; // Permite actualizar o asignar obra social si el paciente cambió de cobertura
                }
                else
                {
                    // CASO 2: Paciente nuevo -> Limpia y habilita los campos para el registro
                    idPacienteActual = null;
                    txtNombre.Clear();
                    txtApellido.Clear();
                    if (cmbObraSocial.Items.Count > 0) cmbObraSocial.SelectedIndex = 0;

                    txtNombre.ReadOnly = false;
                    txtApellido.ReadOnly = false;
                    cmbObraSocial.Enabled = true;
                    txtNombre.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el paciente:\n{ex.Message}", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CargarObrasSociales()
        {
            try
            {
                var lista = _obraSocialBLL.ObtenerObrasSociales();
                cmbObraSocial.DataSource = lista;
                cmbObraSocial.DisplayMember = "Nombre";
                cmbObraSocial.ValueMember = "IdObraSocial";
                if (cmbObraSocial.Items.Count > 0)
                {
                    cmbObraSocial.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el catálogo de obras sociales:\n{ex.Message}", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        /// <summary>
        /// Consulta y carga en el selector <see cref="cmbHorarios"/> todas las franjas horarias con su estado (disponible u ocupado)
        /// y el detalle asociado para la especialidad y fecha especificadas.
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
                    DateTime ahora = DateTime.Now;
                    bool esHoy = fechaElegida.Date == ahora.Date;

                    foreach (var h in horarios)
                    {
                        if (string.IsNullOrWhiteSpace(h.Horario))
                            continue;

                        // Si la fecha elegida es hoy, solo se admiten horarios posteriores a la hora actual
                        if (esHoy && TimeSpan.TryParse(h.Horario.Trim(), out TimeSpan tsSlot))
                        {
                            if (tsSlot <= ahora.TimeOfDay)
                                continue; // Horario ya transcurrido
                        }

                        cmbHorarios.Items.Add(h);
                    }

                    if (cmbHorarios.Items.Count > 0)
                    {
                        cmbHorarios.SelectedIndex = 0;
                        ActualizarEstadoHorarioSeleccionado();
                    }
                    else
                    {
                        if (esHoy)
                        {
                            MessageBox.Show(
                                "No quedan más horarios de atención disponibles para el día de hoy.\nPor favor, seleccione una fecha posterior en el calendario.",
                                "Jornada Finalizada",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else
                        {
                            MessageBox.Show(
                                "No hay turnos disponibles para esta fecha y especialidad.",
                                "Sin Turnos",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        ActualizarEstadoHorarioSeleccionado();
                    }
                }
                else
                {
                    MessageBox.Show("No hay turnos configurados para esta fecha y especialidad.", "Sin turnos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ActualizarEstadoHorarioSeleccionado();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron consultar los horarios:\n" + ex.Message, "Error de Horarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ActualizarEstadoHorarioSeleccionado();
            }
        }

        /// <summary>
        /// Manejador del cambio de selección de horario para alternar entre reserva y detalle/cancelación por 2FA.
        /// </summary>
        private void cmbHorarios_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ActualizarEstadoHorarioSeleccionado();
        }

        /// <summary>
        /// Actualiza la visibilidad de los paneles y botones según si el horario elegido está disponible u ocupado.
        /// </summary>
        private void ActualizarEstadoHorarioSeleccionado()
        {
            if (cmbHorarios.SelectedItem is not HorarioDisponibleDTO slot)
            {
                gbDetalleTurno.Visible = false;
                button1.Enabled = true;
                return;
            }

            if (slot.EstaDisponible)
            {
                // Horario libre para reservar
                gbDetalleTurno.Visible = false;
                button1.Enabled = true;
                txtClaveCancelacion.Clear();
            }
            else
            {
                // Horario ocupado: mostrar datos del turno y activar cancelación con 2FA
                button1.Enabled = false;
                gbDetalleTurno.Visible = true;
                lblDetallePaciente.Text = $"Paciente: {slot.Paciente ?? "--"}";
                lblDetalleDni.Text = $"DNI: {slot.Dni ?? "--"} | O.S.: {slot.ObraSocial ?? "--"}";
                lblDetalleTurnoNro.Text = $"Turno: #{slot.NroOrden ?? "--"} | Estado: {slot.Estado ?? "En Espera"}";
                txtClaveCancelacion.Clear();
            }
        }

        /// <summary>
        /// Manejador del evento click para cancelar un turno ocupado validando la palabra clave (2FA) del comprobante.
        /// </summary>
        private void BtnCancelarTurno_Click(object? sender, EventArgs e)
        {
            if (cmbHorarios.SelectedItem is not HorarioDisponibleDTO slot || slot.EstaDisponible || !slot.IdTurno.HasValue)
            {
                MessageBox.Show("Por favor, seleccione un horario ocupado para cancelar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string claveIngresada = txtClaveCancelacion.Text.Trim();
            if (string.IsNullOrWhiteSpace(claveIngresada))
            {
                MessageBox.Show(
                    "Debe ingresar la palabra clave alfanumérica (2FA) emitida en el comprobante del paciente para confirmar la cancelación.",
                    "Clave 2FA Requerida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtClaveCancelacion.Focus();
                return;
            }

            // Validación de doble factor (2FA)
            if (!string.Equals(claveIngresada, slot.CodigoCancelacion, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "La palabra clave / código 2FA ingresado no es válido para este turno.\n\nVerifique el código en el comprobante impreso del paciente e intente nuevamente.",
                    "Confirmación 2FA Fallida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                txtClaveCancelacion.Focus();
                txtClaveCancelacion.SelectAll();
                return;
            }

            // Confirmación de seguridad
            var confirmacion = MessageBox.Show(
                $"¿Confirma la cancelación del turno #{slot.NroOrden} del paciente {slot.Paciente}?\n\nEsta acción liberará el horario de las {slot.Horario} hs para nuevas reservas.",
                "Confirmar Cancelación (2FA)",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                string especialidad = cmbEspecialidad.SelectedItem?.ToString() ?? string.Empty;
                DateTime fecha = calFechaTurno.SelectionStart.Date;

                _turnoBLL.CancelarTurnoEspecialidad(slot.IdTurno.Value, claveIngresada);

                MessageBox.Show(
                    $"¡El turno #{slot.NroOrden} fue cancelado exitosamente!\n\nEl horario de las {slot.Horario} hs ha quedado liberado.",
                    "Cancelación Exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                txtClaveCancelacion.Clear();
                CargarHorariosDisponibles(especialidad, fecha);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al procesar la cancelación:\n{ex.Message}", "Error de Cancelación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGenerarTurno_Click(object? sender, EventArgs e)
        {
            if (!ValidarDatos())
                return;

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string dni = txtDNI.Text.Trim();
            int idObraSocial = cmbObraSocial.SelectedValue is int val ? val : 1;
            string obraSocial = cmbObraSocial.Text;
            string especialidad = cmbEspecialidad.SelectedItem!.ToString()!;
            DateTime fecha = calFechaTurno.SelectionStart.Date;

            var slotSeleccionado = cmbHorarios.SelectedItem as HorarioDisponibleDTO;
            string horario = slotSeleccionado?.Horario ?? cmbHorarios.SelectedItem!.ToString()!;

            try
            {
                int idPaciente;

                // Guarda al paciente nuevo o actualiza su cobertura médica si fue modificada en el sistema
                idPaciente = _pacienteBLL.GuardarPaciente(nombre, apellido, dni, idObraSocial);
                idPacienteActual = idPaciente;

                // Cálculo del arancel según reglas de negocio (fijo $15.000 Particular, 30% $4.500 con Obra Social)
                decimal montoArancel = TurnoBLL.CalcularArancelSugerido("Especialidad", obraSocial);
                bool tieneOS = TurnoBLL.TieneObraSocial(obraSocial);
                string condicionCobro = TurnoBLL.ObtenerCondicionCobroTexto(obraSocial);

                // Registro del turno con generación automática de palabra clave 2FA y arancel
                var resultadoTurno = _turnoBLL.CrearTurnoEspecialidad(idPaciente, especialidad, fecha, horario, "En Espera", null, montoArancel);
                string nroOrden = resultadoTurno.NroOrden ?? $"T-{resultadoTurno.IdNuevoTurno:D3}";
                string codigoCancelacion = resultadoTurno.CodigoCancelacion ?? string.Empty;

                Lid_turno.Text = $"# {nroOrden}";
                Ldescrip_turno_especialidad.Text = especialidad;
                lblMontoCobro.Text = $"COBRAR: $ {montoArancel:N2}\n({(tieneOS ? "Copago 30%" : "Particular 100%")})";
                lblMontoCobro.ForeColor = tieneOS ? Color.FromArgb(0, 70, 140) : Color.FromArgb(178, 34, 34);

                // Guardamos los datos del comprobante para su exportación a .txt incluyendo la clave 2FA y arancel
                _ultimoTurnoEmitido = new DatosComprobanteTurno
                {
                    NroOrden = nroOrden,
                    Prioridad = "NORMAL",
                    Seccion = especialidad,
                    FechaEmision = DateTime.Now,
                    NombrePaciente = $"{apellido}, {nombre}",
                    DniPaciente = dni,
                    ObraSocial = string.IsNullOrWhiteSpace(obraSocial) ? "Particular / Sin Obra Social" : obraSocial,
                    FechaTurnoProgramado = fecha.ToString("dd/MM/yyyy"),
                    HorarioTurnoProgramado = horario,
                    CodigoCancelacion = codigoCancelacion,
                    MontoCobrado = montoArancel,
                    DetalleArancel = condicionCobro
                };

                // Habilitamos el botón de descarga del comprobante
                btnDescargarTxt.Enabled = true;

                string mensajeCobro =
                    "==============================================\n" +
                    $" 💰 COBRAR EN CAJA: $ {montoArancel:N2}\n" +
                    $" Condición: {condicionCobro}\n" +
                    "==============================================\n\n" +
                    $"¡Turno programado con éxito!\n\n" +
                    $"• Paciente: {apellido}, {nombre}\n" +
                    $"• Especialidad: {especialidad}\n" +
                    $"• Fecha y Hora: {fecha:dd/MM/yyyy} a las {horario} hs\n" +
                    $"• N° Turno: {nroOrden}\n\n" +
                    $"CLAVE DE CANCELACIÓN (2FA): {codigoCancelacion}\n" +
                    $"(Conserve esta clave. Se incluyó en el comprobante descargable para cancelaciones)";

                MessageBox.Show(
                    mensajeCobro,
                    "Turno Generado - Cobro en Caja",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LimpiarCampos();
                CargarHorariosDisponibles(especialidad, fecha);
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
                cmbObraSocial.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, complete todos los datos del paciente y seleccione una obra social.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            if (cmbHorarios.SelectedItem is not HorarioDisponibleDTO slot || string.IsNullOrWhiteSpace(slot.Horario))
            {
                MessageBox.Show("Por favor, seleccione un horario para el turno.", "Falta horario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbHorarios.Focus();
                return false;
            }

            if (!slot.EstaDisponible)
            {
                MessageBox.Show(
                    $"El horario de las {slot.Horario} hs ya se encuentra reservado (Turno #{slot.NroOrden}).\n\nPor favor, seleccione un horario disponible o proceda a cancelar dicho turno con su clave 2FA.",
                    "Horario Ocupado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            // Control de fecha y horario: el turno debe ser estrictamente posterior al momento actual
            DateTime fechaSeleccionada = calFechaTurno.SelectionStart.Date;
            if (TimeSpan.TryParse(slot.Horario.Trim(), out TimeSpan tsSeleccionado))
            {
                DateTime fechaHoraTurno = fechaSeleccionada.Add(tsSeleccionado);
                if (fechaHoraTurno <= DateTime.Now)
                {
                    MessageBox.Show(
                        $"El horario seleccionado ({fechaHoraTurno:dd/MM/yyyy HH:mm} hs) ya ha transcurrido.\n\nSolo se permite programar turnos para un momento posterior al actual.",
                        "Horario Pasado No Permitido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    string esp = cmbEspecialidad.SelectedItem?.ToString() ?? string.Empty;
                    CargarHorariosDisponibles(esp, fechaSeleccionada);
                    return false;
                }
            }

            // Control de concurrencia/duplicidad: Validar que no exista un turno registrado con la misma fecha, horario y especialidad
            string especialidad = cmbEspecialidad.SelectedItem?.ToString() ?? string.Empty;
            DateTime fecha = calFechaTurno.SelectionStart.Date;
            string horario = slot.Horario;

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
            if (cmbObraSocial.Items.Count > 0) cmbObraSocial.SelectedIndex = 0;

            txtNombre.ReadOnly = false;
            txtApellido.ReadOnly = false;
            cmbObraSocial.Enabled = true;

            idPacienteActual = null;
            txtClaveCancelacion.Clear();
            txtDNI.Focus();
        }
    }
}