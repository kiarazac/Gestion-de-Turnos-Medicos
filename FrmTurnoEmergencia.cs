using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de recepción y emisión de turnos de emergencia y triage médico.
    /// Permite autocompletar pacientes por DNI, registrar pacientes nuevos y clasificar la urgencia médica según síntomas.
    /// </summary>
    public partial class FrmTurnoEmergencia : Form
    {
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly PacienteBLL _pacienteBLL = new PacienteBLL(); // Instanciamos para guardar pacientes nuevos
        private int? idPacienteActual = null; // Almacena el ID si el paciente ya existe en la BD

        public FrmTurnoEmergencia()
        {
            InitializeComponent();
            ConfigurarEventosAdicionales();
        }

        private void ConfigurarEventosAdicionales()
        {
            // Suscribimos los eventos de búsqueda por DNI
            txtDNI.Leave += TxtDNI_Leave;
            txtDNI.KeyDown += TxtDNI_KeyDown;

            // Suscribimos el evento del botón Generar Turno
            button1.Click += Button1_Click;
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
                    // CASO 1: El paciente YA existe en la base de datos
                    idPacienteActual = paciente.IdPaciente;
                    txtNombre.Text = paciente.Nombre;
                    txtApellido.Text = paciente.Apellido;
                    txtObraSocial.Text = paciente.ObraSocial;

                    // Bloqueamos edición para evitar modificar registros existentes por error
                    txtNombre.ReadOnly = true;
                    txtApellido.ReadOnly = true;
                    txtObraSocial.ReadOnly = true;
                }
                else
                {
                    // CASO 2: El paciente NO existe. Permitimos cargar sus datos desde cero.
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

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();
                string apellido = txtApellido.Text.Trim();
                string dni = txtDNI.Text.Trim();
                string obraSocial = txtObraSocial.Text.Trim();

                if (string.IsNullOrEmpty(dni) || string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido))
                {
                    MessageBox.Show("Por favor, complete los datos obligatorios del paciente (DNI, Nombre y Apellido).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idPacienteFinal;

                // Si el paciente no estaba registrado, lo guardamos automáticamente en la BD antes de crear el turno
                if (!idPacienteActual.HasValue)
                {
                    idPacienteFinal = _pacienteBLL.GuardarPaciente(nombre, apellido, dni, obraSocial);
                    idPacienteActual = idPacienteFinal; // Actualizamos la referencia local
                }
                else
                {
                    idPacienteFinal = idPacienteActual.Value;
                }

                bool esOtro = checkBoxBaja.Checked;
                List<int> sintomasSeleccionados = new List<int>();

                // Si no marcó "Otro", evaluamos los síntomas de los CheckedListBox
                if (!esOtro)
                {
                    foreach (var item in checkedListAlta.CheckedItems)
                    {
                        // Lógica de mapeo de ID de síntoma si aplica en tu DAL
                    }

                    foreach (var item in checkedListMedia.CheckedItems)
                    {
                        // Lógica de mapeo de ID de síntoma si aplica en tu DAL
                    }

                    if (checkedListAlta.CheckedItems.Count == 0 && checkedListMedia.CheckedItems.Count == 0)
                    {
                        MessageBox.Show("Debe seleccionar al menos un síntoma principal o marcar la opción 'Otro'.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Llamada a la Capa de Negocio pasando el ID del paciente final, los síntomas y el estado de "Otro"
                string nroOrden = _turnoBLL.CrearTurnoEmergenciaConSintomas(idPacienteFinal, sintomasSeleccionados, esOtro);

                // Mostramos el resultado visual en pantalla
                Lid_turno.Text = $"# {nroOrden}";
                Ldescrip_turno_especialidad.Text = "Emergencia";

                MessageBox.Show($"¡Turno de emergencia generado correctamente!\nNúmero de Orden: {nroOrden}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                // Atrapa los mensajes limpios lanzados desde la Base de Datos (SQL THROW/RAISERROR) o de las validaciones de BLL
                MessageBox.Show($"No se pudo completar la operación:\n\n{ex.Message}", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            txtDNI.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtObraSocial.Clear();

            txtNombre.ReadOnly = false;
            txtApellido.ReadOnly = false;
            txtObraSocial.ReadOnly = false;

            idPacienteActual = null;
            checkBoxBaja.Checked = false;

            for (int i = 0; i < checkedListAlta.Items.Count; i++) checkedListAlta.SetItemChecked(i, false);
            for (int i = 0; i < checkedListMedia.Items.Count; i++) checkedListMedia.SetItemChecked(i, false);

            txtDNI.Focus();
        }
    }
}