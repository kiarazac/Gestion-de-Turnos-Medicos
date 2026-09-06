using System;
using System.Data;
using Microsoft.Data.SqlClient; // O System.Data.SqlClient según tu versión
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmLogin : Form
    {
        // AJUSTA TU CADENA DE CONEXIÓN AQUÍ:
        // Si usas autenticación de Windows: "Server=TU_SERVIDOR;Database=TU_BD;Integrated Security=True;TrustServerCertificate=True;"
        // Si usas usuario y contraseña SQL: "Server=TU_SERVIDOR;Database=TU_BD;User Id=sa;Password=tu_clave;TrustServerCertificate=True;"
        private readonly string connectionString = "Server=localhost;Database=GestionTurnosMedicos;Integrated Security=True;TrustServerCertificate=True;";

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Obtenemos los valores de las cajas de texto
            string email = txtCorreo.Text.Trim();
            string contrasena = txtContraseña.Text.Trim();

            // 2. Validamos campos vacíos
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Por favor, completa todos los campos.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Validamos el formato del correo
            if (!Validaciones.EsEmailValido(email))
            {
                MessageBox.Show("Por favor, ingresa un correo válido (@gmail.com, @hotmail.com o @outlook.com).", "Correo inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. Verificación y obtención del Rol desde SQL Server
            try
            {
                string rol = ValidarUsuarioEnBD(email, contrasena);

                if (string.IsNullOrEmpty(rol))
                {
                    MessageBox.Show("Correo o contraseña incorrectos.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 5. Redirección según el tipo de usuario / rol
                AbrirFormularioSegunRol(rol);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos:\n" + ex.Message, "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Consulta a SQL Server para validar credenciales y devolver el rol del usuario.
        /// </summary>
        private string ValidarUsuarioEnBD(string correo, string clave)
        {
            // Opción A: Si creaste una Función Escalar en SQL Server: SELECT dbo.fn_ObtenerRolUsuario(@correo, @clave)
            // Opción B: Si consultas directo a una tabla: SELECT Rol FROM Usuarios WHERE Correo = @correo AND Contrasena = @clave
            string query = "SELECT dbo.fn_ObtenerRolUsuario(@correo, @clave)";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Se usan parámetros para evitar inyección SQL
                    cmd.Parameters.Add("@correo", SqlDbType.VarChar, 100).Value = correo;
                    cmd.Parameters.Add("@clave", SqlDbType.VarChar, 100).Value = clave;

                    con.Open();
                    object resultado = cmd.ExecuteScalar();

                    // Si no encontró coincidencia o devolvió NULL
                    if (resultado == null || resultado == DBNull.Value)
                    {
                        return null;
                    }

                    return resultado.ToString().Trim();
                }
            }
        }

        /// <summary>
        /// Instancia y muestra el Form correspondiente según el rol.
        /// </summary>
        private void AbrirFormularioSegunRol(string rol)
        {
            Form formularioDestino = null;

            // Normalizamos texto a minúsculas y sin espacios para evitar errores de tipeo
            switch (rol.ToLower())
            {
                case "personal medico":
                case "Personal médico":
                case "Personal Médico":
                case "Personal Medico":
                case "Personal_Medico":
                case "Personal_Médico":
                case "personal_medico": 
                case "personal_médico":
                case "personal médico":
                case "medico":
                case "médico":
                    formularioDestino = new Pantalla_Principal_PERSONAL_MEDICO();
                    break;

                case "administrador":
                case "admin":
                    formularioDestino = new FrmAdmin();
                    break;

                case "recepcionista":
                case "recepcion":
                case "recepción":
                    formularioDestino = new FrmRecepcionista();
                    break;

                default:
                    MessageBox.Show($"El rol '{rol}' no tiene un formulario asignado.", "Rol desconocido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }

            // Al cerrar la nueva ventana, cerramos también el Login para que el proceso no quede colgado en segundo plano
            formularioDestino.FormClosed += (s, args) => this.Close();

            // Mostramos la ventana correspondiente y ocultamos el Login
            formularioDestino.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}