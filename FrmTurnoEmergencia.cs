using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmTurnoEmergencia : Form
    {
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
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
                    idPacienteActual = paciente.IdPaciente;
                    txtNombre.Text = paciente.Nombre;
                    txtApellido.Text = paciente.Apellido;
                    txtObraSocial.Text = paciente.ObraSocial;

                    // Bloqueamos edición de nombre y apellido si ya está registrado
                    txtNombre.ReadOnly = true;
                    txtApellido.ReadOnly = true;
                }
                else
                {
                    // Si no existe, permitimos cargar sus datos nuevos
                    idPacienteActual = null;
                    txtNombre.Clear();
                    txtApellido.Clear();
                    txtObraSocial.Clear();
                    txtNombre.ReadOnly = false;
                    txtApellido.ReadOnly = false;
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
                // Validación estricta de existencia de paciente
                if (!idPacienteActual.HasValue)
                {
                    MessageBox.Show("El DNI ingresado no corresponde a un paciente registrado en la base de datos o falta cargar sus datos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool esOtro = checkBoxBaja.Checked;
                List<int> sintomasSeleccionados = new List<int>();

                // Si no marcó "Otro", evaluamos los síntomas de los CheckedListBox
                if (!esOtro)
                {
                    // Nota: Asegúrate de que los ítems en tus CheckedListBox guarden relación o mapeen a sus IDs correspondientes en la BD.
                    // Aquí simulamos la recolección de los IDs según los índices o valores seleccionados.
                    foreach (var item in checkedListAlta.CheckedItems)
                    {
                        // Lógica para obtener el IdSintoma correspondiente a checkedListAlta (Alta)
                        // Ejemplo: si manejas objetos o índices, adáptalo a tu mapeo de DAL.
                    }

                    foreach (var item in checkedListMedia.CheckedItems)
                    {
                        // Lógica para obtener el IdSintoma correspondiente a checkedListMedia (Media)
                    }

                    if (checkedListAlta.CheckedItems.Count == 0 && checkedListMedia.CheckedItems.Count == 0)
                    {
                        MessageBox.Show("Debe seleccionar al menos un síntoma principal o marcar la opción 'Otro'.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Llamada a la Capa de Negocio pasando el estado de "Otro" y los síntomas
                string nroOrden = _turnoBLL.CrearTurnoEmergenciaConSintomas(idPacienteActual.Value, sintomasSeleccionados, esOtro);

                // Mostramos el resultado visual en pantalla
                Lid_turno.Text = $"# {nroOrden}";
                Ldescrip_turno_especialidad.Text = "Emergencia";

                MessageBox.Show($"¡Turno generado correctamente!\nNúmero de Orden: {nroOrden}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            idPacienteActual = null;
            checkBoxBaja.Checked = false;

            for (int i = 0; i < checkedListAlta.Items.Count; i++) checkedListAlta.SetItemChecked(i, false);
            for (int i = 0; i < checkedListMedia.Items.Count; i++) checkedListMedia.SetItemChecked(i, false);
        }
    }
}