using System;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmLogin : Form
    {
        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string email = txtCorreo.Text.Trim();
            string contrasena = txtContraseña.Text;

            // 2. Validaciones visuales en Capa de Presentación (UI)
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

            // 3. Comunicación exclusiva con la Capa de Negocio (BLL)
            try
            {
                // El método Login delega en DAL y ejecuta el Stored Procedure sp_ValidarLogin
                UsuarioLoginResult usuarioLogueado = _usuarioBLL.Login(email, contrasena);

                if (usuarioLogueado == null)
                {
                    MessageBox.Show("Correo o contraseña incorrectos, o el usuario se encuentra inactivo.",
                        "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContraseña.Clear();
                    txtContraseña.Focus();
                    return;
                }

                // 4. Redirección al contenedor correspondiente pasando el contexto del usuario autenticado
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
        /// Instancia y muestra el Form principal correspondiente según el rol del usuario autenticado.
        /// Evalúa tanto NombreRol como IdRol para compatibilidad total con la base de datos.
        /// </summary>
        private void AbrirFormularioSegunRol(UsuarioLoginResult usuario)
        {
            Form formularioDestino = null;
            string rolNombre = usuario.NombreRol?.Trim().ToLowerInvariant() ?? string.Empty;

            // Enrutamiento seguro: se prioriza la descripción del rol (NombreRol) y se contempla IdRol como respaldo
            if (rolNombre.Contains("admin") || usuario.IdRol == 1 || usuario.IdRol == 3 && rolNombre.Contains("admin"))
            {
                formularioDestino = new FrmAdmin(usuario);
            }
            else if (rolNombre.Contains("médic") || rolNombre.Contains("medic") || usuario.IdRol == 2)
            {
                formularioDestino = new Pantalla_Principal_PERSONAL_MEDICO(usuario);
            }
            else if (rolNombre.Contains("recep") || usuario.IdRol == 3)
            {
                formularioDestino = new FrmRecepcionista(usuario);
            }
            else if (rolNombre.Contains("ventan") || rolNombre.Contains("pantalla") || rolNombre.Contains("visor") || rolNombre.Contains("totem") || usuario.IdRol == 4)
            {
                formularioDestino = new FrmUsuarioVentana(usuario);
            }
            else
            {
                // Respaldo por ID si la descripción viene vacía
                switch (usuario.IdRol)
                {
                    case 1:
                        formularioDestino = new FrmAdmin(usuario);
                        break;
                    case 2:
                        formularioDestino = new Pantalla_Principal_PERSONAL_MEDICO(usuario);
                        break;
                    case 3:
                        formularioDestino = new FrmRecepcionista(usuario);
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

            // Al cerrar la ventana principal de la sesión, se cierra también el Login
            formularioDestino.FormClosed += (s, args) => this.Close();

            formularioDestino.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}