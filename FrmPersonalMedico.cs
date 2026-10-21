using System;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
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
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly SalaBLL _salaBLL = new SalaBLL();
        private Form? formularioActivo = null;

        /// <summary>
        /// Inicializa el formulario de personal médico inyectando los datos de la sesión del facultativo autenticado.
        /// </summary>
        /// <param name="usuario">Contexto de sesión del médico (<see cref="UsuarioLoginResult"/>).</param>
        public Pantalla_Principal_PERSONAL_MEDICO(UsuarioLoginResult usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;
            this.Load += Pantalla_Principal_PERSONAL_MEDICO_Load;
            this.FormClosing += Pantalla_Principal_PERSONAL_MEDICO_FormClosing;
        }

        private void Pantalla_Principal_PERSONAL_MEDICO_Load(object? sender, EventArgs e)
        {
            if (_usuarioActual != null && !string.IsNullOrWhiteSpace(_usuarioActual.Nombre))
            {
                lblUsuarioMedico.Text = $"Dr./a. {_usuarioActual.Nombre} {_usuarioActual.Apellido}".Trim();
            }
        }

        /// <summary>
        /// Valida si el profesional médico está en condiciones de cerrar la sesión.
        /// Exige estrictamente haber finalizado cualquier turno iniciado ('En Consulta')
        /// y haber cerrado el consultorio o sala asignada.
        /// </summary>
        /// <param name="motivoBloqueo">Mensaje descriptivo que explica la causa del bloqueo si no puede cerrar sesión.</param>
        /// <returns><c>true</c> si puede cerrar sesión; <c>false</c> si tiene atenciones pendientes o sala abierta.</returns>
        private bool PuedeCerrarSesion(out string motivoBloqueo)
        {
            motivoBloqueo = string.Empty;
            int idUsuario = _usuarioActual?.IdUsuario ?? 0;

            // 1. Validar si el médico tiene un turno en atención médica iniciada sin terminar ('En Consulta')
            if (formularioActivo is FrmListaTurnosAtencion frmAtencion && frmAtencion.TieneAtencionIniciada)
            {
                string nroOrden = frmAtencion.TurnoActual?.NroOrden ?? "en curso";
                motivoBloqueo = $"No puede cerrar sesión: Tiene una atención médica iniciada en curso (Turno #{nroOrden}).\n\nDebe finalizar la atención del paciente y registrar el diagnóstico correspondiente antes de salir.";
                return false;
            }

            if (idUsuario > 0)
            {
                var turnoEnConsulta = _turnoBLL.ObtenerTurnoEnConsultaPorMedico(idUsuario);
                if (turnoEnConsulta != null)
                {
                    motivoBloqueo = $"No puede cerrar sesión: Tiene una atención médica iniciada sin finalizar (Turno #{turnoEnConsulta.NroOrden} - Paciente: {turnoEnConsulta.PacienteCompleto}).\n\nDebe concluir la atención y registrar el diagnóstico antes de salir.";
                    return false;
                }

                // Validar si el médico tiene un turno llamado pendiente de ingresar a consulta o de marcar como ausente
                if (formularioActivo is FrmListaTurnosAtencion frmAtencionLlamado && frmAtencionLlamado.TieneTurnoLlamado)
                {
                    string nroOrden = frmAtencionLlamado.TurnoActual?.NroOrden ?? "llamado";
                    motivoBloqueo = $"No puede cerrar sesión: Tiene el turno #{nroOrden} en estado 'Llamado'.\n\nDebe iniciar y concluir la atención, o marcarlo como 'No se presentó' antes de cerrar la sesión.";
                    return false;
                }

                var turnoLlamado = _turnoBLL.ObtenerTurnoLlamadoPorMedico(idUsuario);
                if (turnoLlamado != null)
                {
                    motivoBloqueo = $"No puede cerrar sesión: Tiene un paciente llamado a consultorio (Turno #{turnoLlamado.NroOrden} - Paciente: {turnoLlamado.PacienteCompleto}).\n\nDebe iniciar y concluir la atención, o marcarlo como 'No se presentó' antes de cerrar la sesión.";
                    return false;
                }

                // 2. Validar si el médico tiene su sala o consultorio aún abierto ('Disponible', 'Libre', 'Ocupada')
                var salaAbierta = _salaBLL.ObtenerSalaAbiertaPorMedico(idUsuario);
                if (salaAbierta != null)
                {
                    motivoBloqueo = $"No puede cerrar sesión: La sala '{salaAbierta.NombreSala}' continúa abierta (Estado: '{salaAbierta.EstadoSala}').\n\nDebe ingresar al módulo 'Mis Salas' y cerrar el consultorio antes de cerrar la sesión.";
                    return false;
                }
            }

            return true;
        }

        private void Pantalla_Principal_PERSONAL_MEDICO_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!PuedeCerrarSesion(out string motivo))
                {
                    MessageBox.Show(motivo, "Cierre de Sesión Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                var frmLogin = Application.OpenForms["FrmLogin"];
                if (frmLogin != null && !frmLogin.Visible)
                {
                    frmLogin.Show();
                }
            }
        }

        /// <summary>
        /// Embebe un formulario hijo dentro del panel central (<c>panelContenedor</c>), cerrando la vista previa.
        /// </summary>
        /// <param name="formHijo">Instancia del formulario hijo a incrustar.</param>
        private void AbrirFormularioHijo(Form formHijo)
        {
            if (formularioActivo is FrmListaTurnosAtencion frmAtencion && frmAtencion.TieneAtencionIniciada)
            {
                MessageBox.Show("Tiene una atención médica iniciada en curso. Debe finalizar la consulta antes de cambiar de módulo.",
                    "Atención en Curso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
        /// Abre el módulo de consulta histórica y reporte de pacientes atendidos por el médico autenticado.
        /// </summary>
        private void btnMisAtenciones_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmMisAtenciones(_usuarioActual));
        }

        /// <summary>
        /// Cierra la sesión activa del profesional médico y regresa a la pantalla de Login.
        /// </summary>
        private void salir_Click(object sender, EventArgs e)
        {
            if (!PuedeCerrarSesion(out string motivo))
            {
                MessageBox.Show(motivo, "Cierre de Sesión Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var frmLogin = Application.OpenForms["FrmLogin"];
            if (frmLogin != null)
            {
                frmLogin.Show();
            }
            this.Close();
        }
    }
}