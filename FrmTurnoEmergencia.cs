using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmTurnoEmergencia : Form
    {
        // =========================================================================
        // CONFIGURACIÓN DE CONEXIÓN A SQL SERVER (DOCKER / LOCAL)
        // Reemplazar "TU_PASSWORD" y las credenciales según tu contenedor Docker.
        // =========================================================================
        private readonly string connectionString = "Server=localhost,1433;Database=GestionTurnosMedicos;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True;";

        public FrmTurnoEmergencia()
        {
            InitializeComponent();

            // Cableado manual de eventos en el constructor (sin modificar FrmTurnoEmergencia.Designer.cs)
            this.Load += FrmTurnoEmergencia_Load;
            this.button1.Click += BtnGenerarTurno_Click;
        }

        private void FrmTurnoEmergencia_Load(object sender, EventArgs e)
        {
            // Inicializar el panel de "Turno Generado"
            Lid_turno.Text = "# --";
            Ldescrip_turno_especialidad.Text = "Emergencia";
        }

        /// <summary>
        /// Evento del botón "GENERAR TURNO" (button1).
        /// Valida campos, determina prioridad, guarda paciente y turno en BD con SPs y actualiza la UI.
        /// </summary>
        private void BtnGenerarTurno_Click(object sender, EventArgs e)
        {
            // 1. Validar entradas del formulario
            if (!ValidarDatos())
                return;

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string dni = txtDNI.Text.Trim();
            string obraSocial = txtObraSocial.Text.Trim();

            // 2. Determinar la prioridad según los síntomas y condiciones tildadas
            string prioridad = CalcularPrioridad();

            // 3. Recopilar la lista de síntomas y condiciones seleccionadas
            List<string> sintomasSeleccionados = ObtenerSintomasSeleccionados();

            // 4. Guardar en Base de Datos mediante Stored Procedures y ADO.NET
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Uso de SqlTransaction para garantizar atomicidad (todo se guarda o nada se guarda)
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            // -------------------------------------------------------------
                            // PASO 1: Guardar o recuperar al Paciente (sp_GuardarPaciente)
                            // -------------------------------------------------------------
                            int idPaciente;
                            using (SqlCommand cmdPaciente = new SqlCommand("sp_GuardarPaciente", con, tran))
                            {
                                cmdPaciente.CommandType = CommandType.StoredProcedure;
                                cmdPaciente.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = nombre;
                                cmdPaciente.Parameters.Add("@Apellido", SqlDbType.VarChar, 100).Value = apellido;
                                cmdPaciente.Parameters.Add("@Dni", SqlDbType.VarChar, 20).Value = dni;
                                cmdPaciente.Parameters.Add("@ObraSocial", SqlDbType.VarChar, 100).Value = obraSocial;

                                SqlParameter paramIdPaciente = new SqlParameter("@IdPaciente", SqlDbType.Int)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                cmdPaciente.Parameters.Add(paramIdPaciente);

                                cmdPaciente.ExecuteNonQuery();

                                idPaciente = Convert.ToInt32(paramIdPaciente.Value);
                            }

                            // -------------------------------------------------------------
                            // PASO 2: Crear el Turno de Emergencia (sp_CrearTurno)
                            // -------------------------------------------------------------
                            int idTurno;
                            string nroOrden;
                            using (SqlCommand cmdTurno = new SqlCommand("sp_CrearTurno", con, tran))
                            {
                                cmdTurno.CommandType = CommandType.StoredProcedure;
                                cmdTurno.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = idPaciente;
                                cmdTurno.Parameters.Add("@Prioridad", SqlDbType.VarChar, 50).Value = prioridad;
                                cmdTurno.Parameters.Add("@TipoTurno", SqlDbType.VarChar, 50).Value = "Emergencia";
                                cmdTurno.Parameters.Add("@Estado", SqlDbType.VarChar, 50).Value = "En Espera";

                                SqlParameter paramIdTurno = new SqlParameter("@IdTurno", SqlDbType.Int)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                SqlParameter paramNroOrden = new SqlParameter("@NroOrden", SqlDbType.VarChar, 20)
                                {
                                    Direction = ParameterDirection.Output
                                };

                                cmdTurno.Parameters.Add(paramIdTurno);
                                cmdTurno.Parameters.Add(paramNroOrden);

                                cmdTurno.ExecuteNonQuery();

                                idTurno = Convert.ToInt32(paramIdTurno.Value);
                                nroOrden = paramNroOrden.Value?.ToString() ?? $"E-{idTurno:D3}";
                            }

                            // -------------------------------------------------------------
                            // PASO 3: Registrar los síntomas asociados al turno (sp_RegistrarTurnoSintoma)
                            // -------------------------------------------------------------
                            foreach (string sintoma in sintomasSeleccionados)
                            {
                                using (SqlCommand cmdSintoma = new SqlCommand("sp_RegistrarTurnoSintoma", con, tran))
                                {
                                    cmdSintoma.CommandType = CommandType.StoredProcedure;
                                    cmdSintoma.Parameters.Add("@IdTurno", SqlDbType.Int).Value = idTurno;
                                    cmdSintoma.Parameters.Add("@DescripcionSintoma", SqlDbType.VarChar, 100).Value = sintoma;
                                    cmdSintoma.Parameters.Add("@EstadoActual", SqlDbType.VarChar, 50).Value = "Presente";

                                    cmdSintoma.ExecuteNonQuery();
                                }
                            }

                            // Si todo salió bien, confirmamos la transacción
                            tran.Commit();

                            // 5. Actualizar la interfaz con el turno generado
                            MostrarTurnoGenerado(nroOrden, prioridad);

                            MessageBox.Show(
                                $"¡Turno generado con éxito!\n\n" +
                                $"Paciente: {apellido}, {nombre}\n" +
                                $"N° de Orden: {nroOrden}\n" +
                                $"Prioridad: {prioridad}",
                                "Turno Generado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            // Limpiar los controles de entrada dejando a la vista el número generado
                            LimpiarCampos();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show(
                    $"Error al conectar o ejecutar en la base de datos SQL Server:\n{sqlEx.Message}",
                    "Error de Base de Datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ocurrió un error inesperado al generar el turno:\n{ex.Message}",
                    "Error Inesperado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Valida que los datos obligatorios del paciente y al menos una condición/síntoma estén completos.
        /// </summary>
        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtDNI.Text) ||
                string.IsNullOrWhiteSpace(txtObraSocial.Text))
            {
                MessageBox.Show(
                    "Por favor, complete todos los datos del paciente (Nombre, Apellido, DNI y Obra Social).",
                    "Faltan datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            if (!Validaciones.EsNombreValido(txtNombre.Text.Trim()))
            {
                MessageBox.Show(
                    "El Nombre contiene caracteres inválidos. Solo se admiten letras.",
                    "Nombre inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtNombre.Focus();
                return false;
            }

            if (!Validaciones.EsNombreValido(txtApellido.Text.Trim()))
            {
                MessageBox.Show(
                    "El Apellido contiene caracteres inválidos. Solo se admiten letras.",
                    "Apellido inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtApellido.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtDNI.Text.Trim(), @"^\d{7,8}$"))
            {
                MessageBox.Show(
                    "El DNI debe tener entre 7 y 8 números (sin puntos ni letras).",
                    "DNI inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtDNI.Focus();
                return false;
            }

            // Validar que al menos haya un síntoma, condición o tilde marcado
            bool tieneSintomasAlta = checkedListBox1.CheckedItems.Count > 0;
            bool tieneSintomasMedia = checkedListBox2.CheckedItems.Count > 0;
            bool tieneCondicionEspecial = checkedListBox3.CheckedItems.Count > 0;
            bool tieneOtro = checkBox1.Checked;

            if (!tieneSintomasAlta && !tieneSintomasMedia && !tieneCondicionEspecial && !tieneOtro)
            {
                MessageBox.Show(
                    "Debe seleccionar al menos un síntoma o condición del paciente para clasificar el turno.",
                    "Atención",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calcula la prioridad (Triage) según la gravedad de los síntomas seleccionados:
        /// - ALTA: Cualquier síntoma grave de la lista checkedListBox1.
        /// - MEDIA: Algún síntoma intermedio de checkedListBox2, o condición especial si no hay alta.
        /// - BAJA: "Otro (No es de gravedad)" o consultas leves.
        /// </summary>
        private string CalcularPrioridad()
        {
            if (checkedListBox1.CheckedItems.Count > 0)
            {
                return "ALTA";
            }
            if (checkedListBox2.CheckedItems.Count > 0)
            {
                return "MEDIA";
            }
            if (checkedListBox3.CheckedItems.Count > 0)
            {
                return "MEDIA"; // Pacientes con condición especial tienen prioridad media si no tienen síntomas agudos
            }

            return "BAJA";
        }

        /// <summary>
        /// Extrae la descripción limpia de todos los síntomas y condiciones marcados.
        /// </summary>
        private List<string> ObtenerSintomasSeleccionados()
        {
            List<string> sintomas = new List<string>();

            // Síntomas ALTA
            foreach (var item in checkedListBox1.CheckedItems)
            {
                if (item != null)
                    sintomas.Add(item.ToString()!.Trim());
            }

            // Síntomas MEDIA
            foreach (var item in checkedListBox2.CheckedItems)
            {
                if (item != null)
                    sintomas.Add(item.ToString()!.Trim());
            }

            // Condiciones Especiales
            foreach (var item in checkedListBox3.CheckedItems)
            {
                if (item != null)
                    sintomas.Add(item.ToString()!.Trim());
            }

            // Otro
            if (checkBox1.Checked)
            {
                sintomas.Add(checkBox1.Text.Trim());
            }

            return sintomas;
        }

        /// <summary>
        /// Muestra visualmente el número de orden y la especialidad/prioridad asignada en el panel derecho.
        /// </summary>
        private void MostrarTurnoGenerado(string nroOrden, string prioridad)
        {
            Lid_turno.Text = $"# {nroOrden}";
            Ldescrip_turno_especialidad.Text = $"Guardia ({prioridad})";

            // Diferenciar color según la prioridad asignada
            switch (prioridad)
            {
                case "ALTA":
                    Ldescrip_turno_especialidad.ForeColor = Color.FromArgb(214, 39, 40);
                    break;
                case "MEDIA":
                    Ldescrip_turno_especialidad.ForeColor = Color.FromArgb(204, 102, 0);
                    break;
                case "BAJA":
                    Ldescrip_turno_especialidad.ForeColor = Color.FromArgb(46, 139, 87);
                    break;
                default:
                    Ldescrip_turno_especialidad.ForeColor = SystemColors.Highlight;
                    break;
            }
        }

        /// <summary>
        /// Limpia los campos de entrada de texto y desmarca las casillas de selección.
        /// </summary>
        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtDNI.Clear();
            txtObraSocial.Clear();

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, false);

            for (int i = 0; i < checkedListBox2.Items.Count; i++)
                checkedListBox2.SetItemChecked(i, false);

            for (int i = 0; i < checkedListBox3.Items.Count; i++)
                checkedListBox3.SetItemChecked(i, false);

            checkBox1.Checked = false;

            txtNombre.Focus();
        }
    }
}
