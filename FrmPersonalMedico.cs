using System;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Contenedor principal para el panel de operaciones del perfil Personal Médico.
    /// Permite al profesional de la salud gestionar sus consultorios habilitados y atender las colas de turnos asignados.
    /// </summary>
    public partial class Pantalla_Principal_PERSONAL_MEDICO : Form
    {
        private readonly UsuarioLoginResult _usuarioActual;
        private Form? formularioActivo = null;

        /// <summary>
        /// Inicializa el formulario de personal médico inyectando los datos de la sesión del facultativo autenticado.
        /// </summary>
        /// <param name="usuario">Contexto de sesión del médico (<see cref="UsuarioLoginResult"/>).</param>
        public Pantalla_Principal_PERSONAL_MEDICO(UsuarioLoginResult usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;
        }

        /// <summary>
        /// Embebe un formulario hijo dentro del panel central (<c>panelContenedor</c>), cerrando la vista previa.
        /// </summary>
        /// <param name="formHijo">Instancia del formulario hijo a incrustar.</param>
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

            panelContenedor.Controls.Add(formHijo);
            panelContenedor.Tag = formHijo;
            formHijo.BringToFront();
            formHijo.Show();
        }

        /// <summary>
        /// Abre el módulo de gestión y control de apertura de las salas asignadas al médico autenticado.
        /// </summary>
        private void mis_Salas_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new MisSalas_PM(_usuarioActual));
        }

        /// <summary>
        /// Abre el tablero de llamados, atención clínica y prescripción médica para los turnos en espera.
        /// </summary>
        private void lista_turnos_atención_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmListaTurnosAtencion(_usuarioActual));
        }

        /// <summary>
        /// Cierra la sesión activa del profesional médico y regresa a la pantalla de Login.
        /// </summary>
        private void salir_Click(object sender, EventArgs e)
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