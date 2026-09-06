using Gestion_de_Turnos_Medicos.Negocio; // Importamos la capa de negocio
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.CapaDeDatos; // Asegúrate de poner el namespace correcto donde guardaste UsuarioLoginResult

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
            // 1. Obtenemos los valores. (No le hacemos Trim() a la contraseña por si el usuario le puso un espacio al final a propósito)
            string email = txtCorreo.Text.Trim();
            string contrasena = txtContraseña.Text;

            // 2. Validamos con TU clase de validaciones
            if (!Validaciones.EsEmailValido(email))
            {
                MessageBox.Show("Por favor, ingresa un correo válido (@gmail.com, @hotmail.com o @outlook.com).", "Correo inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Verificación pasando por la Capa de Negocio
            try
            {
                UsuarioBLL negocio = new UsuarioBLL();

                // El método Login ahora nos devuelve todos los datos del usuario (DTO), no solo el rol
                UsuarioLoginResult usuarioLogueado = negocio.Login(email, contrasena);

                if (usuarioLogueado == null)
                {
                    MessageBox.Show("Correo o contraseña incorrectos, o el usuario está inactivo.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 4. Redirección pasándole TODOS los datos del usuario logueado
                AbrirFormularioSegunRol(usuarioLogueado);
            }
            catch (Exception ex)
            {
                // El BLL nos devolverá errores si los campos están vacíos, o si falla la conexión a BD
                MessageBox.Show("Ocurrió un error:\n" + ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Instancia y muestra el Form correspondiente según el rol del usuario logueado.
        /// </summary>
        private void AbrirFormularioSegunRol(UsuarioLoginResult usuario)
        {
            Form formularioDestino = null;

            // Evaluamos directamente el IdRol (Asumiendo 1=Admin, 2=Médico, 3=Recepcionista según el orden en que los creamos)
            switch (usuario.IdRol)
            {
                case 3: // Administrador
                    formularioDestino = new FrmAdmin(usuario);
                    break;

                case 1: // Personal médico
                    formularioDestino = new Pantalla_Principal_PERSONAL_MEDICO(usuario);
                    break;

                case 2: // Recepcionista
                    formularioDestino = new FrmRecepcionista(usuario);
                    break;

                default:
                    MessageBox.Show($"El ID de rol '{usuario.IdRol}' no está configurado en el sistema.", "Rol desconocido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }

            // Al cerrar la nueva ventana, cerramos también el Login
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