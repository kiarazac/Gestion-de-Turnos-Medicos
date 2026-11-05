using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Perfiles con panel de operaciones (el Usuario Ventana usa su visor fijo <see cref="FrmUsuarioVentana"/>).
    /// </summary>
    public enum RolSistema
    {
        Recepcionista,
        Medico,
        Administrador,
        Gerente
    }

    /// <summary>
    /// Contenedor principal único para todos los perfiles: barra lateral con las operaciones del sistema y panel central
    /// donde se embebe el formulario hijo. Las operaciones del rol logueado quedan habilitadas; las de los demás roles
    /// se muestran deshabilitadas (sin evento Click) agrupadas con la etiqueta de su rol.
    /// </summary>
    public partial class FrmPrincipal : Form
    {
        private sealed record Operacion(RolSistema Rol, string Texto, Color Fondo, Color Hover, Color Presionado, Action Abrir);

        private sealed record TemaRol(string Etiqueta, string Titulo, string Clinica, string TextoVentana, string UsuarioPorDefecto,
            Color Sidebar, Color Header, Color Separador, Color ColorUsuario, Color ColorTitulo, Color ColorClinica,
            Image Fondo, ImageLayout LayoutFondo, bool FondoEnFormulario);

        private static readonly Dictionary<RolSistema, TemaRol> Temas = new()
        {
            [RolSistema.Recepcionista] = new("Recepción", "📋 RECEPCIÓN Y CAJA", "GESTIÓN HOSPITALARIA", "Pantalla Principal |RECEPCIONISTA|", "Recepcionista",
                Color.FromArgb(23, 56, 89), Color.FromArgb(15, 38, 62), Color.FromArgb(37, 99, 235),
                Color.FromArgb(240, 249, 255), Color.FromArgb(56, 189, 248), Color.FromArgb(186, 230, 253),
                Properties.Resources.fondo_recepcionista1, ImageLayout.Stretch, false),
            [RolSistema.Medico] = new("Personal Médico", "🩺 CUERPO MÉDICO", "PORTAL CLÍNICO", "Pantalla Principal |PERSONAL MÉDICO|", "Personal Médico",
                Color.FromArgb(19, 78, 74), Color.FromArgb(10, 46, 44), Color.FromArgb(15, 118, 110),
                Color.FromArgb(240, 253, 250), Color.FromArgb(45, 212, 191), Color.FromArgb(153, 246, 228),
                Properties.Resources.fondo_personalMedico, ImageLayout.Stretch, false),
            [RolSistema.Administrador] = new("Administración", "🛡️ ADMINISTRACIÓN", "GESTIÓN HOSPITALARIA", "Administracion", "Administrador",
                Color.FromArgb(15, 23, 42), Color.FromArgb(10, 15, 29), Color.FromArgb(51, 65, 85),
                Color.FromArgb(241, 245, 249), Color.FromArgb(56, 189, 248), Color.FromArgb(148, 163, 184),
                Properties.Resources.fondo_admin, ImageLayout.Tile, true),
            [RolSistema.Gerente] = new("Gerencia", "👔 DIRECCIÓN GERENCIAL", "GESTIÓN HOSPITALARIA", "Panel de Dirección Gerencial", "Gerente",
                Color.FromArgb(13, 37, 34), Color.FromArgb(7, 22, 20), Color.FromArgb(20, 83, 76),
                Color.FromArgb(240, 253, 244), Color.FromArgb(52, 211, 153), Color.FromArgb(110, 231, 183),
                Properties.Resources.fondo_admin, ImageLayout.Tile, true)
        };

        // Botones de operaciones deshabilitadas: fondo claro para que el texto gris de Windows se lea sobre la barra oscura
        private static readonly Color FondoDeshabilitado = Color.FromArgb(203, 213, 225);

        private readonly UsuarioLoginResult _usuarioActual;
        private readonly RolSistema _rol;
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly SalaBLL _salaBLL = new SalaBLL();
        private readonly EspecialidadBLL _especialidadBLL = new EspecialidadBLL();
        private readonly BackupBLL _backupBLL = new BackupBLL();
        private Form? formularioActivo = null;

        /// <summary>
        /// Inicializa el contenedor principal para el usuario autenticado y su perfil.
        /// </summary>
        /// <param name="usuario">Contexto de la sesión (<see cref="UsuarioLoginResult"/>).</param>
        /// <param name="rol">Perfil que determina las operaciones habilitadas y la apariencia.</param>
        public FrmPrincipal(UsuarioLoginResult usuario, RolSistema rol)
        {
            InitializeComponent();
            _usuarioActual = usuario;
            _rol = rol;

            this.Load += FrmPrincipal_Load;
            this.FormClosing += FrmPrincipal_FormClosing;
            this.btnSalir.Click += (s, e) => this.Close(); // El login reaparece en FormClosed (FrmLogin)
        }

        private void FrmPrincipal_Load(object? sender, EventArgs e)
        {
            AplicarTema(Temas[_rol]);
            CrearBotonesOperaciones();

            if (_rol == RolSistema.Medico)
                AvisarEspecialidadInactiva();

            // El gerente entra directo a los reportes gerenciales
            if (_rol == RolSistema.Gerente)
                AbrirFormularioHijo(() => new FrmReportesGerente(_usuarioActual));
        }

        /// <summary>
        /// Catálogo completo de operaciones del sistema, en el orden en que se muestran dentro de cada rol.
        /// </summary>
        private List<Operacion> ObtenerOperaciones()
        {
            Color rojo = Color.FromArgb(185, 28, 28), rojoHover = Color.FromArgb(220, 38, 38), rojoDown = Color.FromArgb(153, 27, 27);
            Color azul = Color.FromArgb(30, 78, 124), azulHover = Color.FromArgb(2, 132, 199), azulDown = Color.FromArgb(3, 105, 161);
            Color teal = Color.FromArgb(15, 118, 110), tealHover = Color.FromArgb(13, 148, 136);
            Color pizarra = Color.FromArgb(30, 41, 59), pizarraHover = Color.FromArgb(51, 65, 85), pizarraDown = Color.FromArgb(37, 99, 235);
            Color verde = Color.FromArgb(19, 56, 52);

            return new List<Operacion>
            {
                new(RolSistema.Recepcionista, "🚨  Turno Emergencia", rojo, rojoHover, rojoDown, () => AbrirFormularioHijo(() => new FrmTurnoEmergencia())),
                new(RolSistema.Recepcionista, "🩺  Turno Programado", azul, azulHover, azulDown, () => AbrirFormularioHijo(() => new FrmTurnoEspecialidad())),
                new(RolSistema.Recepcionista, "📋  Listado de Turnos", azul, azulHover, azulDown, () => AbrirFormularioHijo(() => new FrmListaTurnos())),
                new(RolSistema.Recepcionista, "📺  Visor Sala Espera", azul, azulHover, azulDown, AbrirVisorSalaEspera),
                new(RolSistema.Recepcionista, "📊  Reportes y Caja", teal, tealHover, teal, () => AbrirFormularioHijo(() => new FrmCierreCaja(_usuarioActual))),

                new(RolSistema.Medico, "🏥  Mis Consultorios", teal, tealHover, Color.FromArgb(20, 184, 166), () => AbrirFormularioHijo(() => new MisSalas_PM(_usuarioActual))),
                new(RolSistema.Medico, "🩺  Atención de Turnos", teal, tealHover, Color.FromArgb(20, 184, 166), () => AbrirFormularioHijo(() => new FrmListaTurnosAtencion(_usuarioActual))),
                new(RolSistema.Medico, "📋  Historial Atenciones", teal, tealHover, Color.FromArgb(20, 184, 166), () => AbrirFormularioHijo(() => new FrmMisAtenciones(_usuarioActual))),
                new(RolSistema.Medico, "📅  Mis Ausencias", teal, tealHover, Color.FromArgb(20, 184, 166), () => AbrirFormularioHijo(() => new FrmMisAusencias(_usuarioActual))),

                new(RolSistema.Administrador, "👥  Usuarios y Médicos", pizarra, pizarraHover, pizarraDown, () => AbrirFormularioHijo(() => new FrmGestionUsuarios2(_usuarioActual))),
                new(RolSistema.Administrador, "🩺  Especialidades", pizarra, pizarraHover, pizarraDown, () => AbrirFormularioHijo(() => new FrmGestionEspecialidades())),
                new(RolSistema.Administrador, "💳  Obras Sociales", pizarra, pizarraHover, pizarraDown, () => AbrirFormularioHijo(() => new FrmGestionObrasSociales())),
                new(RolSistema.Administrador, "💾  Backup y Restore", pizarra, pizarraHover, pizarraDown, () => AbrirFormularioHijo(() => new FrmBackupRestore(_usuarioActual))),

                new(RolSistema.Gerente, "📊  Auditoría y Reportes", verde, teal, tealHover, () => AbrirFormularioHijo(() => new FrmReportesGerente(_usuarioActual))),
                new(RolSistema.Gerente, "🏥  Salas y Consultorios", verde, teal, tealHover, () => AbrirFormularioHijo(() => new FrmSalasGerente()))
            };
        }

        private void AplicarTema(TemaRol tema)
        {
            Text = tema.TextoVentana;
            panel1.BackColor = tema.Sidebar;
            pnlHeaderSidebar.BackColor = tema.Header;
            pnlFooterSidebar.BackColor = tema.Header;
            pnlSeparador.BackColor = tema.Separador;
            lblClinica.Text = tema.Clinica;
            lblClinica.ForeColor = tema.ColorClinica;
            lblRolTitulo.Text = tema.Titulo;
            lblRolTitulo.ForeColor = tema.ColorTitulo;
            lblUsuario.ForeColor = tema.ColorUsuario;
            lblUsuario.Text = tema.UsuarioPorDefecto;

            if (_usuarioActual != null && !string.IsNullOrWhiteSpace(_usuarioActual.Nombre))
            {
                string nombre = $"{_usuarioActual.Nombre} {_usuarioActual.Apellido}".Trim();
                lblUsuario.Text = _rol == RolSistema.Medico ? $"Dr./a. {nombre}" : nombre;
            }

            pnlContenedor.BackgroundImage = tema.Fondo;
            pnlContenedor.BackgroundImageLayout = tema.LayoutFondo;
            if (tema.FondoEnFormulario)
                BackgroundImage = tema.Fondo;
        }

        /// <summary>
        /// Arma la barra lateral: primero las operaciones habilitadas del rol logueado y debajo,
        /// agrupadas por rol, las del resto del sistema deshabilitadas.
        /// </summary>
        private void CrearBotonesOperaciones()
        {
            var operaciones = ObtenerOperaciones();
            int y = 12;

            foreach (var op in operaciones.Where(o => o.Rol == _rol))
            {
                var btn = CrearBoton(op.Texto, y);
                btn.BackColor = op.Fondo;
                btn.FlatAppearance.MouseOverBackColor = op.Hover;
                btn.FlatAppearance.MouseDownBackColor = op.Presionado;
                btn.Click += (s, e) => op.Abrir();
                y += 54;
            }

            y += 6;
            y = AgregarEtiqueta("OTRAS OPERACIONES DEL SISTEMA", Temas[_rol].ColorClinica, 8.25F, y);

            foreach (var grupo in operaciones.Where(o => o.Rol != _rol).GroupBy(o => o.Rol))
            {
                y = AgregarEtiqueta($"🔒 {Temas[grupo.Key].Etiqueta}", Temas[_rol].ColorUsuario, 8.25F, y);

                foreach (var op in grupo)
                {
                    // Sin evento Click: la operación solo se muestra, no se puede ejecutar
                    var btn = CrearBoton(op.Texto, y);
                    btn.BackColor = FondoDeshabilitado;
                    btn.Enabled = false;
                    btn.Cursor = Cursors.No;
                    y += 54;
                }

                y += 4;
            }
        }

        private Button CrearBoton(string texto, int y)
        {
            var btn = new Button
            {
                Text = texto,
                Location = new Point(15, y),
                // 205 de ancho para que la barra de desplazamiento vertical no fuerce también una horizontal
                Size = new Size(205, 46),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                ForeColor = Color.White,
                Padding = new Padding(12, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            btn.FlatAppearance.BorderSize = 0;
            pnlNav.Controls.Add(btn);
            return btn;
        }

        private int AgregarEtiqueta(string texto, Color color, float tamanio, int y)
        {
            pnlNav.Controls.Add(new Label
            {
                Text = texto,
                AutoSize = true,
                Location = new Point(15, y),
                Font = new Font("Segoe UI", tamanio, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent
            });
            return y + 24;
        }

        /// <summary>
        /// Embebe un formulario hijo en el panel central, cerrando el anterior.
        /// El formulario se crea recién después de validar que se puede salir del módulo actual.
        /// </summary>
        /// <param name="crearForm">Fábrica del formulario hijo a mostrar.</param>
        private void AbrirFormularioHijo(Func<Form> crearForm)
        {
            if (!PuedeSalirDelModuloActual())
                return;

            formularioActivo?.Close();

            var formHijo = crearForm();
            formularioActivo = formHijo;
            pnlContenedor.Tag = formHijo;
            EscaladoFormularios.Embeber(formHijo, pnlContenedor); // Adapta la vista al tamaño de la pantalla
        }

        /// <summary>
        /// Abre la pantalla pública de llamados. Con Shift o Ctrl se abre en ventana independiente (monitor secundario o TV).
        /// </summary>
        private void AbrirVisorSalaEspera()
        {
            if (ModifierKeys.HasFlag(Keys.Shift) || ModifierKeys.HasFlag(Keys.Control))
                new FrmUsuarioVentana().Show();
            else
                AbrirFormularioHijo(() => new FrmUsuarioVentana());
        }

        /// <summary>
        /// Indica (y avisa) si el módulo abierto tiene una operación en curso que impide cambiar de módulo o cerrar sesión.
        /// </summary>
        private bool PuedeSalirDelModuloActual()
        {
            if (formularioActivo is FrmTurnoEspecialidad frmEspecialidad && frmEspecialidad.TieneReprogramacionPendiente)
            {
                MessageBox.Show("Hay una reprogramación de turno en curso. Genere el nuevo turno o cancele la reprogramación antes de cambiar de módulo o cerrar sesión.",
                    "Reprogramación en Curso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (formularioActivo is FrmListaTurnosAtencion frmAtencion && frmAtencion.TieneAtencionIniciada)
            {
                MessageBox.Show("Tiene una atención médica iniciada en curso. Debe finalizar la consulta antes de cambiar de módulo.",
                    "Atención en Curso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void FrmPrincipal_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;

            if (_rol == RolSistema.Medico)
            {
                if (!PuedeCerrarSesionMedico(out string motivo))
                {
                    MessageBox.Show(motivo, "Cierre de Sesión Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
                return;
            }

            if (!PuedeSalirDelModuloActual())
                e.Cancel = true;
        }

        /// <summary>
        /// Valida si el profesional médico puede cerrar la sesión: sin atención iniciada ('En Consulta'),
        /// sin paciente llamado pendiente y con su consultorio cerrado.
        /// </summary>
        /// <param name="motivoBloqueo">Mensaje que explica la causa del bloqueo si no puede cerrar sesión.</param>
        private bool PuedeCerrarSesionMedico(out string motivoBloqueo)
        {
            motivoBloqueo = string.Empty;
            int idUsuario = _usuarioActual?.IdUsuario ?? 0;

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

                var salaAbierta = _salaBLL.ObtenerSalaAbiertaPorMedico(idUsuario);
                if (salaAbierta != null)
                {
                    motivoBloqueo = $"No puede cerrar sesión: La sala '{salaAbierta.NombreSala}' continúa abierta (Estado: '{salaAbierta.EstadoSala}').\n\nDebe ingresar al módulo 'Mis Salas' y cerrar el consultorio antes de cerrar la sesión.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Si el médico tiene alguna especialidad inactiva, le informa que solo podrá atender los turnos
        /// ya registrados y le muestra el correo de un gerente activo para contactarse.
        /// </summary>
        private void AvisarEspecialidadInactiva()
        {
            int idUsuario = _usuarioActual?.IdUsuario ?? 0;
            if (idUsuario <= 0)
                return;

            try
            {
                var inactivas = _especialidadBLL.ObtenerEspecialidadesPorMedico(idUsuario)
                    .Where(esp => !esp.Activo)
                    .Select(esp => esp.Nombre)
                    .ToList();

                if (inactivas.Count == 0)
                    return;

                string correoGerente = _backupBLL.ObtenerGerentesActivos().FirstOrDefault()?.Correo
                    ?? "No hay gerentes activos registrados";

                MessageBox.Show(
                    $"Su especialidad '{string.Join(", ", inactivas)}' se encuentra inactiva en el sistema.\n\n" +
                    "Podrá atender los turnos que ya tenía registrados, pero no se asignarán turnos nuevos.\n\n" +
                    $"Por favor, póngase en contacto con el gerente: {correoGerente}",
                    "Especialidad Inactiva", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                // El aviso es informativo: si falla la consulta no se bloquea el ingreso del médico
            }
        }
    }
}
