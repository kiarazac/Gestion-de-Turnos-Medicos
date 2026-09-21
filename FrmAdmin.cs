using System;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Contenedor principal para el panel de control del perfil Administrador.
    /// Implementa un patrón de navegación por formularios hijos embebidos en el panel central.
    /// </summary>
    public partial class FrmAdmin : Form
    {
        private readonly UsuarioLoginResult _usuarioActual;
        private Form? formularioActivo = null;

        /// <summary>
        /// Inicializa el formulario principal de administración inyectando la sesión del usuario autenticado.
        /// </summary>
        /// <param name="usuario">Contexto de la sesión del usuario administrador (<see cref="UsuarioLoginResult"/>).</param>
        public FrmAdmin(UsuarioLoginResult usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;
        }

        /// <summary>
        /// Embebe un formulario hijo dentro del panel central (<c>pnlContenedor</c>), cerrando previamente el formulario activo si existiese.
        /// </summary>
        /// <param name="formHijo">Instancia del formulario a incrustar.</param>
        private void AbrirFormularioHijo(Form formHijo)
        {
            if (formularioActivo != null)
            {
                formularioActivo.Close();
            }

            formularioActivo = formHijo;
            formHijo.TopLevel = false;
            formHijo.FormBorderStyle = FormBorderStyle.None;
            formHijo.Dock = DockStyle.Fill;

            pnlContenedor.Controls.Add(formHijo);
            pnlContenedor.Tag = formHijo;
            formHijo.BringToFront();
            formHijo.Show();
        }

        /// <summary>
        /// Abre el módulo de gestión de usuarios y personal médico pasando el contexto de sesión.
        /// </summary>
        private void btnPersonalMedico_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmGestionUsuarios2(_usuarioActual));
        }

        /// <summary>
        /// Abre el módulo de administración de salas y consultorios físicos.
        /// </summary>
        private void btnSalas_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmSalasAdmin());
        }

        /// <summary>
        /// Abre el módulo de administración de especialidades médicas.
        /// </summary>
        private void btnEspecialidades_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmGestionEspecialidades());
        }

        /// <summary>
        /// Abre la versión 2.0 del módulo de gestión de usuarios con auditoría de bajas.
        /// </summary>
        private void btnUsuarios2_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmGestionUsuarios2(_usuarioActual));
        }

        /// <summary>
        /// Cierra la sesión activa del administrador y reabre el formulario de login.
        /// </summary>
        private void btnSalir_Click(object sender, EventArgs e)
        {
            var frmLogin = Application.OpenForms["FrmLogin"];
            if (frmLogin != null)
            {
                frmLogin.Show();
            }
            this.Close();
        }
    }
}