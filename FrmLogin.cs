using System;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de autenticación e inicio de sesión para operadores y profesionales del sistema.
    /// Valida entradas del usuario, invoca <see cref="UsuarioBLL.Login"/> y redirige a la vista principal según el rol.
    /// </summary>
    public partial class FrmLogin : Form
    {
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();

        /// <summary>
        /// Inicializa los componentes visuales del formulario y configura el botón por defecto de aceptación.
        /// </summary>
        public FrmLogin()
        {
            InitializeComponent();

            // Configura el botón 'Iniciar Sesión' (button1) como botón de aceptación por defecto
            this.AcceptButton = button1;
        }

        /// <summary>
        /// Manejador de evento del botón 'Iniciar Sesión' (button1).
        /// Realiza validaciones en capa visual, solicita la autenticación a la BLL y enruta al formulario de destino.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            string email = txtCorreo.Text.Trim();
            string contrasena = txtContraseña.Text;

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Por favor, ingresá tu correo electrónico.",
                    "Falta correo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCorreo.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Por favor, ingresá tu contraseña.",
                    "Falta contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContraseña.Focus();
                return;
            }

            if (!Validaciones.EsEmailValido(email))
            {
                MessageBox.Show("Por favor, ingresá un formato de correo válido (@gmail.com, @hotmail.com o @outlook.com).",
                    "Correo inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCorreo.Focus();
                return;
            }

            try
            {
                UsuarioLoginResult usuarioLogueado = _usuarioBLL.Login(email, contrasena);

                if (usuarioLogueado == null)
                {
                    MessageBox.Show("Correo o contraseña incorrectos, o el usuario se encuentra inactivo.",
                        "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContraseña.Clear();
                    txtContraseña.Focus();
                    return;
                }

                AbrirFormularioSegunRol(usuarioLogueado);
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo iniciar sesión. Error de comunicación:\n" + ex.Message,
                    "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Instancia y muestra el Form principal correspondiente según el perfil o rol del usuario autenticado.
        /// </summary>
        /// <param name="usuario">Contexto de datos del usuario autenticado (<see cref="UsuarioLoginResult"/>).</param>
        private void AbrirFormularioSegunRol(UsuarioLoginResult usuario)
        {
            Form? formularioDestino = null;
            string rolNombre = usuario.NombreRol?.Trim().ToLowerInvariant() ?? string.Empty;

            if (rolNombre.Contains("admin") || usuario.IdRol == 3)
            {
                formularioDestino = new FrmAdmin(usuario);
            }
            else if (rolNombre.Contains("médic") || rolNombre.Contains("medic") || usuario.IdRol == 1)
            {
                formularioDestino = new Pantalla_Principal_PERSONAL_MEDICO(usuario);
            }
            else if (rolNombre.Contains("recep") || usuario.IdRol == 2)
            {
                formularioDestino = new FrmRecepcionista(usuario);
            }
            else if (rolNombre.Contains("ventan") || rolNombre.Contains("pantalla") || rolNombre.Contains("visor") || rolNombre.Contains("totem") || usuario.IdRol == 4)
            {
                formularioDestino = new FrmUsuarioVentana(usuario);
            }
            else
            {
                switch (usuario.IdRol)
                {
                    case 1:
                        formularioDestino = new Pantalla_Principal_PERSONAL_MEDICO(usuario);
                        break;
                    case 2:
                        formularioDestino = new FrmRecepcionista(usuario);
                        break;
                    case 3:
                        formularioDestino = new FrmAdmin(usuario);
                        break;
                    case 4:
                        formularioDestino = new FrmUsuarioVentana(usuario);
                        break;
                    default:
                        MessageBox.Show($"El rol '{usuario.NombreRol}' (ID {usuario.IdRol}) no cuenta con una pantalla asignada en el sistema.",
                            "Rol no configurado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }
            }

            formularioDestino.FormClosed += (s, args) => this.Close();
            formularioDestino.Show();
            this.Hide();
        }

        /// <summary>
        /// Cierra la aplicación por completo al hacer clic en el botón 'Cancelar' o 'Salir'.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}