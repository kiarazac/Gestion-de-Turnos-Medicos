using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmTurnoEspecialidad : Form
    {
        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly EspecialidadBLL _especialidadBLL = new EspecialidadBLL();
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly PacienteBLL _pacienteBLL = new PacienteBLL();

        public FrmTurnoEspecialidad()
        {
            InitializeComponent();

            this.Load += FrmTurnoEspecialidad_Load;
            this.button1.Click += BtnGenerarTurno_Click;
        }

        private void FrmTurnoEspecialidad_Load(object? sender, EventArgs e)
        {
            calFechaTurno.MinDate = DateTime.Today;

            CargarEspecialidades();

            Lid_turno.Text = "# --";
            Ldescrip_turno_especialidad.Text = "Especialidad";
        }

        /// <summary>
        /// Obtiene las especialidades activas desde la Capa de Negocio (BLL), sin datos simulados.
        /// </summary>
        private void CargarEspecialidades()
        {
            cmbEspecialidad.Items.Clear();
            cmbEspecialidad.Items.Add("Seleccione especialidad...");

            try
            {
                // BLL delega a EspecialidadDAL -> sp_ObtenerEspecialidades / sp_ListarEspecialidades
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
                MessageBox.Show("No se pudieron cargar las especialidades médicas:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        }

        private void calFechaTurno_DateChanged(object sender, DateRangeEventArgs e)
        {
            cmbHorarios.Items.Clear();

            if (cmbEspecialidad.SelectedIndex <= 0)
            {
                MessageBox.Show("Por favor, seleccione una especialidad antes de elegir la fecha.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string especialidad = cmbEspecialidad.SelectedItem?.ToString() ?? string.Empty;
            DateTime fechaElegida = e.Start.Date;

            try
            {
                // BLL delega a TurnoDAL -> sp_ObtenerHorariosDisponibles (sin listas hardcodeadas en catch)
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
                    MessageBox.Show("No hay turnos disponibles para esta fecha y especialidad.",
                        "Sin disponibilidad", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron consultar los horarios disponibles:\n" + ex.Message,
                    "Error de Horarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // 1. Guardar o recuperar paciente mediante PacienteBLL
                int idPaciente = _pacienteBLL.GuardarPaciente(nombre, apellido, dni, obraSocial);

                // 2. Registrar turno de especialidad mediante TurnoBLL (sp_CrearTurnoEspecialidad / sp_InsertarTurno)
                var resultadoTurno = _turnoBLL.CrearTurnoEspecialidad(idPaciente, especialidad, fecha, horario, "En Espera");

                string nroOrden = resultadoTurno.NroOrden ?? $"T-{resultadoTurno.IdNuevoTurno:D3}";

                // 3. Actualizar interfaz visual
                Lid_turno.Text = $"# {nroOrden}";
                Ldescrip_turno_especialidad.Text = especialidad;

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
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al registrar el turno de especialidad:\n" + ex.Message,
                    "Error Inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtDNI.Text) ||
                string.IsNullOrWhiteSpace(txtObraSocial.Text))
            {
                MessageBox.Show("Por favor, complete todos los datos del paciente.",
                    "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Validaciones.EsNombreValido(txtNombre.Text.Trim()))
            {
                MessageBox.Show("El Nombre contiene caracteres inválidos. Solo se admiten letras.",
                    "Nombre inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (!Validaciones.EsNombreValido(txtApellido.Text.Trim()))
            {
                MessageBox.Show("El Apellido contiene caracteres inválidos. Solo se admiten letras.",
                    "Apellido inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtDNI.Text.Trim(), @"^\d{7,8}$"))
            {
                MessageBox.Show("El DNI debe tener entre 7 y 8 números.",
                    "DNI inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDNI.Focus();
                return false;
            }

            if (cmbEspecialidad.SelectedIndex <= 0)
            {
                MessageBox.Show("Por favor, seleccione una especialidad.",
                    "Falta especialidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbEspecialidad.Focus();
                return false;
            }

            if (cmbHorarios.SelectedItem == null || string.IsNullOrWhiteSpace(cmbHorarios.SelectedItem.ToString()))
            {
                MessageBox.Show("Por favor, seleccione un horario para el turno.",
                    "Falta horario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbHorarios.Focus();
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
            cmbEspecialidad.SelectedIndex = 0;
            cmbHorarios.Items.Clear();
            txtNombre.Focus();
        }
    }
}
