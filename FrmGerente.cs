using System;
using System.Drawing;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Contenedor principal para el panel de dirección del perfil Gerente / Dueño de Negocio.
    /// Emplea el mismo fondo visual del Administrador (Properties.Resources.fondo_admin)
    /// y centraliza el acceso exclusivo a los reportes de facturación, monetización y demanda.
    /// </summary>
    public partial class FrmGerente : Form
    {
        private readonly UsuarioLoginResult _usuarioActual;
        private Form? formularioActivo = null;

        public FrmGerente(UsuarioLoginResult usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            this.Load += FrmGerente_Load;
            this.btnReportes.Click += BtnReportes_Click;
            this.btnGuardia.Click += BtnGuardia_Click;
            this.btnSalir.Click += BtnSalir_Click;
        }

        private void FrmGerente_Load(object? sender, EventArgs e)
        {
            if (_usuarioActual != null && !string.IsNullOrWhiteSpace(_usuarioActual.Nombre))
            {
                LUsuarioInfo.Text = $"{_usuarioActual.Nombre} {_usuarioActual.Apellido}".Trim();
            }

            // Abre automáticamente el módulo de reportes gerenciales en el panel principal
            AbrirFormularioHijo(new FrmReportesGerente(_usuarioActual));
        }

        /// <summary>
        /// Embebe un formulario hijo dentro del panel central (<c>pnlContenedor</c>), cerrando previamente el activo.
        /// </summary>
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

        private void BtnReportes_Click(object? sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmReportesGerente(_usuarioActual));
        }

        private void BtnGuardia_Click(object? sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmReporteGuardiaAdmin(_usuarioActual));
        }

        private void BtnSalir_Click(object? sender, EventArgs e)
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
