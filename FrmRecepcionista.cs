using System;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Contenedor principal para el panel de operaciones del perfil Recepcionista.
    /// Centraliza la emisión de turnos de emergencia, asignación de turnos por especialidad y proyección del visor de espera.
    /// </summary>
    public partial class FrmRecepcionista : Form
    {
        private readonly UsuarioLoginResult _usuarioActual;
        private Form? formularioActivo = null;

        /// <summary>
        /// Inicializa el formulario de recepción inyectando la sesión del recepcionista autenticado.
        /// </summary>
        /// <param name="usuario">Contexto de la sesión del recepcionista (<see cref="UsuarioLoginResult"/>).</param>
        public FrmRecepcionista(UsuarioLoginResult usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;
        }

        /// <summary>
        /// Embebe un formulario hijo dentro del panel central (<c>panelContenedor</c>), cerrando el anterior para liberar recursos.
        /// </summary>
        /// <param name="formHijo">Instancia del formulario hijo a mostrar.</param>
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
        /// Abre el formulario de admisión de pacientes y triage para turnos de urgencia.
        /// </summary>
        private void asign_turnosEmergencia_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmTurnoEmergencia());
        }

        /// <summary>
        /// Abre el formulario para agendar turnos programados por especialidad médica.
        /// </summary>
        private void asign_turnosEspecialidad_Click_1(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmTurnoEspecialidad());
        }

        /// <summary>
        /// Abre el tablero general de monitoreo de turnos emitidos.
        /// </summary>
        private void lista_turnos_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmListaTurnos());
        }

        /// <summary>
        /// Abre la pantalla pública de llamados en sala de espera.
        /// Si se mantiene presionada la tecla Shift o Ctrl, se abre en ventana independiente para monitores secundarios o TV.
        /// </summary>
        private void btnUsuarioVentana_Click(object sender, EventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Shift) || ModifierKeys.HasFlag(Keys.Control))
            {
                FrmUsuarioVentana ventanaIndependiente = new FrmUsuarioVentana();
                ventanaIndependiente.Show();
            }
            else
            {
                AbrirFormularioHijo(new FrmUsuarioVentana());
            }
        }

        /// <summary>
        /// Cierra la sesión activa de recepción y regresa a la pantalla de Login.
        /// </summary>
        private void salir_Click(object sender, EventArgs e)
        {
            Application.OpenForms["FrmLogin"]?.Show();
            this.Close();
        }
    }
}