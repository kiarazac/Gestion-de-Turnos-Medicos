using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmLogin : Form
    {
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
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al conectar con la base de datos SQL Server:\n" + sqlEx.Message, "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado:\n" + ex.Message, "Error General", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Consulta a SQL Server vía Stored Procedure para validar credenciales y devolver el rol del usuario.
        /// </summary>
        private string ValidarUsuarioEnBD(string correo, string clave)
        {
            // Stored Procedure: sp_ValidarUsuario
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Correo", SqlDbType.VarChar, 100).Value = correo;
                    cmd.Parameters.Add("@Clave", SqlDbType.VarChar, 100).Value = clave;

                    SqlParameter paramRol = new SqlParameter("@Rol", SqlDbType.VarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(paramRol);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    if (paramRol.Value == null || paramRol.Value == DBNull.Value)
                    {
                        return null;
                    }

                    return paramRol.Value.ToString()?.Trim();
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
                case "personal médico":
                case "personal_medico": 
                case "personal_médico":
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