using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de recepción y emisión de turnos de emergencia y triage médico.
    /// Permite autocompletar pacientes por DNI, registrar pacientes nuevos y clasificar la urgencia médica según síntomas.
    /// Garantiza que el síntoma de mayor gravedad tildado determine el nivel de prioridad asignado (Alta, Media, Baja).
    /// </summary>
    public partial class FrmTurnoEmergencia : Form
    {
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly PacienteBLL _pacienteBLL = new PacienteBLL(); // Instanciamos para guardar pacientes nuevos
        private int? idPacienteActual = null; // Almacena el ID si el paciente ya existe en la BD
        private DatosComprobanteTurno? _ultimoTurnoEmitido = null; // Almacena los datos del último turno generado para exportar

        public FrmTurnoEmergencia()
        {
            InitializeComponent();
            ConfigurarEventosAdicionales();
        }

        private void ConfigurarEventosAdicionales()
        {
            // Cargar el catálogo dinámico de síntomas al inicializar el formulario
            this.Load += (s, e) => CargarCatalogoSintomas();

            // Suscribimos los eventos de búsqueda por DNI
            txtDNI.Leave += TxtDNI_Leave;
            txtDNI.KeyDown += TxtDNI_KeyDown;

            // Suscribimos el evento del botón Generar Turno
            button1.Click += Button1_Click;

            // Suscribimos el evento de descarga de comprobante .txt
            btnDescargarTxt.Click += BtnDescargarTxt_Click;
        }

        /// <summary>
        /// Obtiene el catálogo de síntomas activos desde la base de datos a través de TurnoBLL
        /// y los distribuye dinámicamente en los CheckedListBox según su nivel de gravedad (Alta o Media).
        /// </summary>
        private void CargarCatalogoSintomas()
        {
            try
            {
                var sintomas = _turnoBLL.ObtenerSintomas();
                if (sintomas != null && sintomas.Count > 0)
                {
                    checkedListAlta.Items.Clear();
                    checkedListMedia.Items.Clear();

                    foreach (var s in sintomas)
                    {
                        if (s.Gravedad.Equals("Alta", StringComparison.OrdinalIgnoreCase))
                        {
                            checkedListAlta.Items.Add(s);
                        }
                        else if (s.Gravedad.Equals("Media", StringComparison.OrdinalIgnoreCase))
                        {
                            checkedListMedia.Items.Add(s);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo cargar el catálogo de síntomas desde la base de datos:\n{ex.Message}",
                    "Aviso de Triage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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

                // 1. Recolectamos los IDs de los síntomas tildados en la lista de ALTA gravedad
                foreach (var item in checkedListAlta.CheckedItems)
                {
                    if (item is SintomaDTO s)
                    {
                        sintomasSeleccionados.Add(s.IdSintoma);
                    }
                    else if (item is string str)
                    {
                        // Fallback defensivo si los elementos vinieron como cadenas de texto
                        int id = MapearIdSintomaPorTexto(str);
                        if (id > 0) sintomasSeleccionados.Add(id);
                    }
                }

                // 2. Recolectamos los IDs de los síntomas tildados en la lista de MEDIA gravedad
                foreach (var item in checkedListMedia.CheckedItems)
                {
                    if (item is SintomaDTO s)
                    {
                        sintomasSeleccionados.Add(s.IdSintoma);
                    }
                    else if (item is string str)
                    {
                        // Fallback defensivo si los elementos vinieron como cadenas de texto
                        int id = MapearIdSintomaPorTexto(str);
                        if (id > 0) sintomasSeleccionados.Add(id);
                    }
                }

                // Validación: Se debe haber marcado al menos un síntoma o seleccionado la opción "Otro"
                if (sintomasSeleccionados.Count == 0 && !esOtro)
                {
                    MessageBox.Show("Debe seleccionar al menos un síntoma principal o marcar la opción 'Otro'.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Llamada a la Capa de Negocio pasando el ID del paciente, los síntomas seleccionados y el estado de "Otro".
                // TurnoBLL evalúa todas las gravedades asignando la prioridad más alta (1=Alta, 2=Media, 3=Baja).
                string nroOrden = _turnoBLL.CrearTurnoEmergenciaConSintomas(idPacienteFinal, sintomasSeleccionados, esOtro, out string prioridadTexto);

                // 4. Mostramos el resultado visual en pantalla con el número de turno y la prioridad asignada
                Lid_turno.Text = $"# {nroOrden}";
                Ldescrip_turno_especialidad.Text = $"Prioridad ({prioridadTexto})";

                // Coloreamos el texto según el nivel de urgencia del triage
                switch (prioridadTexto.ToUpperInvariant())
                {
                    case "ALTA":
                        Ldescrip_turno_especialidad.ForeColor = Color.FromArgb(214, 39, 40); // Rojo urgencia vital
                        break;
                    case "MEDIA":
                        Ldescrip_turno_especialidad.ForeColor = Color.FromArgb(204, 102, 0); // Naranja urgencia moderada
                        break;
                    case "BAJA":
                        Ldescrip_turno_especialidad.ForeColor = Color.FromArgb(46, 139, 87);  // Verde guardia regular
                        break;
                    default:
                        Ldescrip_turno_especialidad.ForeColor = SystemColors.Highlight;
                        break;
                }

                // Guardamos los datos completos del turno emitido para la descarga del comprobante .txt
                _ultimoTurnoEmitido = new DatosComprobanteTurno
                {
                    NroOrden = nroOrden,
                    Prioridad = prioridadTexto,
                    Seccion = "Emergencia",
                    FechaEmision = DateTime.Now,
                    NombrePaciente = $"{apellido}, {nombre}",
                    DniPaciente = dni,
                    ObraSocial = string.IsNullOrWhiteSpace(obraSocial) ? "Particular / Ninguna" : obraSocial
                };

                // Habilitamos el botón de descarga ubicado debajo del número de orden
                btnDescargarTxt.Enabled = true;

                MessageBox.Show($"¡Turno de emergencia generado correctamente!\n\nNúmero de Orden: {nroOrden}\nPrioridad Triage: {prioridadTexto}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                // Atrapa los mensajes limpios lanzados desde la Base de Datos (SQL THROW/RAISERROR) o de las validaciones de BLL
                MessageBox.Show($"No se pudo completar la operación:\n\n{ex.Message}", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento click para generar y descargar el archivo .txt con los datos del turno emitido.
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
                    sfd.Title = "Guardar Comprobante de Turno de Emergencia";

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

        /// <summary>
        /// Mapea descripciones o textos de contingencia a los identificadores numéricos de síntomas en base de datos.
        /// </summary>
        /// <param name="texto">Cadena textual del síntoma.</param>
        /// <returns>ID del síntoma reconocido o 0 si no se encontró coincidencia.</returns>
        private int MapearIdSintomaPorTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return 0;
            string t = texto.ToLowerInvariant();

            if (t.Contains("pecho")) return 1;
            if (t.Contains("respirar") || t.Contains("respiratoria")) return 2;
            if (t.Contains("conocimiento")) return 3;
            if (t.Contains("sangrado") || t.Contains("hemorragia")) return 4;
            if (t.Contains("fiebre")) return 5;
            if (t.Contains("abdominal") || t.Contains("abdomen") || t.Contains("vómito") || t.Contains("vomito")) return 6;
            if (t.Contains("expuesta") || t.Contains("fractura") || t.Contains("trauma")) return 7;
            if (t.Contains("alerg") || t.Contains("alérg") || t.Contains("cabeza")) return 8;

            return 0;
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