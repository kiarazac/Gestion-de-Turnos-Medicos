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
        private readonly ObraSocialBLL _obraSocialBLL = new ObraSocialBLL();
        private int? idPacienteActual = null; // Almacena el ID si el paciente ya existe en la BD
        private DatosComprobanteTurno? _ultimoTurnoEmitido = null; // Almacena los datos del último turno generado para exportar
        private TurnoEmergenciaCancelacionDTO? _turnoACancelar = null; // Almacena el turno localizado para cancelar con 2FA

        public FrmTurnoEmergencia()
        {
            InitializeComponent();
            ConfigurarEventosAdicionales();
        }

        private void ConfigurarEventosAdicionales()
        {
            // Cargar el catálogo dinámico de síntomas y obras sociales al inicializar el formulario
            this.Load += (s, e) =>
            {
                CargarCatalogoSintomas();
                CargarObrasSociales();
            };

            // Suscribimos los eventos de búsqueda por DNI
            txtDNI.Leave += TxtDNI_Leave;
            txtDNI.KeyDown += TxtDNI_KeyDown;

            // Suscribimos el evento del botón Generar Turno
            button1.Click += Button1_Click;

            // Suscribimos el evento de descarga de comprobante .txt
            btnDescargarTxt.Click += BtnDescargarTxt_Click;

            // Suscribimos los eventos del panel de Cancelación Ágil 2FA
            btnBuscarTurnoCancelacion.Click += BtnBuscarTurnoCancelacion_Click;
            txtBuscarTurnoCancelacion.KeyDown += TxtBuscarTurnoCancelacion_KeyDown;
            btnConfirmarCancelacionEmergencia.Click += BtnConfirmarCancelacionEmergencia_Click;
        }

        /// <summary>
        /// Obtiene el catálogo de síntomas activos desde la base de datos a través de TurnoBLL,
        /// deduplica defensivamente por descripción clínica y los distribuye dinámicamente
        /// en los CheckedListBox según su nivel de gravedad (Alta o Media).
        /// </summary>
        private void CargarCatalogoSintomas()
        {
            try
            {
                // Limpiar siempre las listas antes de cargar para evitar acumulación de elementos
                checkedListAlta.Items.Clear();
                checkedListMedia.Items.Clear();

                var sintomas = _turnoBLL.ObtenerSintomas();
                if (sintomas != null && sintomas.Count > 0)
                {
                    // Deduplicación defensiva por descripción clínica (insensible a mayúsculas/minúsculas y espacios)
                    var sintomasUnicos = sintomas
                        .GroupBy(s => (s.Descripcion ?? string.Empty).Trim(), StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.First())
                        .ToList();

                    foreach (var s in sintomasUnicos)
                    {
                        // Categorización según gravedad clínica
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

                    if (paciente.IdObraSocial.HasValue && paciente.IdObraSocial.Value > 0)
                    {
                        cmbObraSocial.SelectedValue = paciente.IdObraSocial.Value;
                    }
                    else if (!string.IsNullOrWhiteSpace(paciente.ObraSocial))
                    {
                        int idx = cmbObraSocial.FindStringExact(paciente.ObraSocial);
                        if (idx >= 0) cmbObraSocial.SelectedIndex = idx;
                    }

                    // Bloqueamos edición para evitar modificar registros existentes por error
                    txtNombre.ReadOnly = true;
                    txtApellido.ReadOnly = true;
                    cmbObraSocial.Enabled = false;
                }
                else
                {
                    // CASO 2: El paciente NO existe. Permitimos cargar sus datos desde cero.
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

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();
                string apellido = txtApellido.Text.Trim();
                string dni = txtDNI.Text.Trim();

                if (string.IsNullOrEmpty(dni) || string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido))
                {
                    MessageBox.Show("Por favor, complete los datos obligatorios del paciente (DNI, Nombre y Apellido).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idObraSocial = cmbObraSocial.SelectedValue is int val ? val : 1;
                string obraSocial = cmbObraSocial.Text;

                int idPacienteFinal;

                // Si el paciente no estaba registrado, lo guardamos automáticamente en la BD antes de crear el turno
                if (!idPacienteActual.HasValue)
                {
                    idPacienteFinal = _pacienteBLL.GuardarPaciente(nombre, apellido, dni, idObraSocial);
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

                // 3. Cálculo de arancel según reglas de negocio (fijo $25.000 emergencia, 30% $7.500 con Obra Social)
                decimal montoArancel = TurnoBLL.CalcularArancelSugerido("Emergencia", obraSocial);

                // Llamada a la Capa de Negocio pasando el ID del paciente, los síntomas seleccionados, el estado de "Otro" y el arancel.
                // TurnoBLL evalúa todas las gravedades asignando la prioridad más alta (1=Alta, 2=Media, 3=Baja) y autogenera la clave 2FA (CAN-XXXX).
                string nroOrden = _turnoBLL.CrearTurnoEmergenciaConSintomas(idPacienteFinal, sintomasSeleccionados, esOtro, out string prioridadTexto, out string codigoCancelacion, montoArancel);

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

                // Guardamos los datos completos del turno emitido para la descarga del comprobante .txt, incluyendo la clave 2FA
                _ultimoTurnoEmitido = new DatosComprobanteTurno
                {
                    NroOrden = nroOrden,
                    Prioridad = prioridadTexto,
                    Seccion = "Emergencia",
                    FechaEmision = DateTime.Now,
                    NombrePaciente = $"{apellido}, {nombre}",
                    DniPaciente = dni,
                    ObraSocial = string.IsNullOrWhiteSpace(obraSocial) ? "Particular / Ninguna" : obraSocial,
                    CodigoCancelacion = codigoCancelacion
                };

                // Habilitamos el botón de descarga ubicado debajo del número de orden
                btnDescargarTxt.Enabled = true;

                // Precargamos los datos del turno en el panel de cancelación directa por si se emitió por error
                _turnoACancelar = new TurnoEmergenciaCancelacionDTO
                {
                    NroOrden = nroOrden,
                    Apellido = apellido,
                    Nombre = nombre,
                    Dni = dni,
                    Prioridad = prioridadTexto,
                    Estado = "En Espera",
                    CodigoCancelacion = codigoCancelacion
                };
                txtBuscarTurnoCancelacion.Text = nroOrden;
                lblTurnoInfoPaciente.Text = $"Paciente: {apellido}, {nombre} (DNI: {dni})";
                lblTurnoInfoTriage.Text = $"Triage: {prioridadTexto} | Estado: En Espera";
                txtClaveCancelacionEmergencia.Text = codigoCancelacion;

                MessageBox.Show(
                    $"¡Turno de emergencia generado correctamente!\n\n" +
                    $"Número de Orden: {nroOrden}\n" +
                    $"Prioridad Triage: {prioridadTexto}\n" +
                    $"Arancel en Caja: $ {montoArancel:N2} ({(TurnoBLL.TieneObraSocial(obraSocial) ? "30% con Obra Social" : "Particular")})\n\n" +
                    $"CLAVE DE CANCELACIÓN (2FA): {codigoCancelacion}\n" +
                    $"(Conserve esta clave. Se incluyó en el comprobante descargable para cancelaciones)",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                // Atrapa los mensajes limpios lanzados desde la Base de Datos (SQL THROW/RAISERROR) o de las validaciones de BLL
                MessageBox.Show($"No se pudo completar la operación:\n\n{ex.Message}", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador de pulsación de teclas en el campo de búsqueda de cancelación. Al pulsar Enter, ejecuta la búsqueda.
        /// </summary>
        private void TxtBuscarTurnoCancelacion_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evitar sonido beep de Windows
                BuscarTurnoParaCancelacion();
            }
        }

        /// <summary>
        /// Manejador del botón BUSCAR del panel de cancelación rápida de guardia.
        /// </summary>
        private void BtnBuscarTurnoCancelacion_Click(object? sender, EventArgs e)
        {
            BuscarTurnoParaCancelacion();
        }

        /// <summary>
        /// Localiza un turno de guardia activo mediante Nro de Orden (ej. E-001) o DNI del paciente.
        /// Despliega la ficha en pantalla en 1 solo paso y prepara el campo de clave 2FA.
        /// </summary>
        private void BuscarTurnoParaCancelacion()
        {
            string termino = txtBuscarTurnoCancelacion.Text.Trim();
            if (string.IsNullOrWhiteSpace(termino))
            {
                MessageBox.Show("Por favor, ingrese un Número de Orden (ej. E-001) o el DNI del paciente para buscar.", "Búsqueda Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBuscarTurnoCancelacion.Focus();
                return;
            }

            try
            {
                // Invocamos a la capa de negocio BLL
                var turno = _turnoBLL.BuscarTurnoActivoEmergencia(termino);
                if (turno != null)
                {
                    _turnoACancelar = turno;
                    lblTurnoInfoPaciente.Text = $"Paciente: {turno.PacienteCompleto} (DNI: {turno.Dni})";
                    lblTurnoInfoTriage.Text = $"Turno: #{turno.NroOrden} | Triage: {turno.Prioridad} | Estado: {turno.Estado}";
                    txtClaveCancelacionEmergencia.Clear();
                    txtClaveCancelacionEmergencia.Focus();
                }
                else
                {
                    _turnoACancelar = null;
                    lblTurnoInfoPaciente.Text = "Paciente: No se encontró ningún turno activo";
                    lblTurnoInfoTriage.Text = "Triage: -- | Estado: --";
                    txtClaveCancelacionEmergencia.Clear();
                    MessageBox.Show($"No se encontró ningún turno de guardia activo (En Espera o Llamado) para el término '{termino}'.\n\nVerifique el número de ticket o DNI ingresado.", "Turno No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtBuscarTurnoCancelacion.Focus();
                    txtBuscarTurnoCancelacion.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar turno de guardia:\n{ex.Message}", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Manejador del botón CANCELAR TURNO (2FA) en guardia médica.
        /// Valida en memoria y en base de datos la clave 2FA para procesar la baja inmediata en 1 solo paso.
        /// </summary>
        private void BtnConfirmarCancelacionEmergencia_Click(object? sender, EventArgs e)
        {
            if (_turnoACancelar == null)
            {
                MessageBox.Show("Primero debe buscar y seleccionar un turno de guardia activo para cancelar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBuscarTurnoCancelacion.Focus();
                return;
            }

            string claveIngresada = txtClaveCancelacionEmergencia.Text.Trim();
            if (string.IsNullOrWhiteSpace(claveIngresada))
            {
                MessageBox.Show("Debe ingresar la palabra clave alfanumérica (2FA) emitida en el ticket del paciente (ej. CAN-XXXX).", "Clave 2FA Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClaveCancelacionEmergencia.Focus();
                return;
            }

            // Validación previa en memoria si el DTO ya traía la clave cargada
            if (!string.IsNullOrWhiteSpace(_turnoACancelar.CodigoCancelacion) &&
                !string.Equals(claveIngresada, _turnoACancelar.CodigoCancelacion, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("La palabra clave / código 2FA ingresado no coincide con el ticket emitido para este turno.\n\nVerifique el comprobante impreso del paciente e intente nuevamente.", "Confirmación 2FA Fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClaveCancelacionEmergencia.Focus();
                txtClaveCancelacionEmergencia.SelectAll();
                return;
            }

            // Confirmación de seguridad
            var confirmacion = MessageBox.Show(
                $"¿Confirma la cancelación inmediata del turno #{_turnoACancelar.NroOrden} perteneciente al paciente {_turnoACancelar.PacienteCompleto}?\n\nEsta acción removerá al paciente de la lista de espera de guardia.",
                "Confirmar Cancelación de Emergencia (2FA)",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                // Invocamos a la capa de negocio BLL para persistir la baja
                _turnoBLL.CancelarTurnoEmergencia(_turnoACancelar.IdTurno, claveIngresada);

                MessageBox.Show(
                    $"¡El turno de guardia #{_turnoACancelar.NroOrden} fue cancelado exitosamente!\n\nEl paciente ha sido retirado de la guardia.",
                    "Cancelación Exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Si el turno recién cancelado era el que se mostraba en pantalla grande, limpiamos el display
                if (Lid_turno.Text.Contains(_turnoACancelar.NroOrden))
                {
                    Lid_turno.Text = "# --------";
                    Ldescrip_turno_especialidad.Text = "Especialidad";
                    Ldescrip_turno_especialidad.ForeColor = SystemColors.Highlight;
                    btnDescargarTxt.Enabled = false;
                    _ultimoTurnoEmitido = null;
                }

                // Limpiamos los campos del panel de cancelación
                _turnoACancelar = null;
                txtBuscarTurnoCancelacion.Clear();
                lblTurnoInfoPaciente.Text = "Paciente: (Sin búsqueda)";
                lblTurnoInfoTriage.Text = "Triage: -- | Estado: --";
                txtClaveCancelacionEmergencia.Clear();
                txtBuscarTurnoCancelacion.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al procesar la cancelación:\n{ex.Message}", "Error de Cancelación", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (cmbObraSocial.Items.Count > 0) cmbObraSocial.SelectedIndex = 0;

            txtNombre.ReadOnly = false;
            txtApellido.ReadOnly = false;
            cmbObraSocial.Enabled = true;

            idPacienteActual = null;
            checkBoxBaja.Checked = false;

            for (int i = 0; i < checkedListAlta.Items.Count; i++) checkedListAlta.SetItemChecked(i, false);
            for (int i = 0; i < checkedListMedia.Items.Count; i++) checkedListMedia.SetItemChecked(i, false);

            txtDNI.Focus();
        }
    }
}