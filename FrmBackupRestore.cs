using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario administrativo para la gestión de copias de seguridad (Backup a demanda)
    /// y restauración de la base de datos con doble autorización obligatoria.
    /// </summary>
    public partial class FrmBackupRestore : Form
    {
        private readonly UsuarioLoginResult _usuarioActual;
        private readonly BackupBLL _backupBLL = new BackupBLL();
        private List<UsuarioLoginResult> _gerentesDisponibles = new List<UsuarioLoginResult>();

        /// <summary>
        /// Inicializa el formulario inyectando el contexto del Administrador en sesión.
        /// </summary>
        public FrmBackupRestore(UsuarioLoginResult usuarioActual)
        {
            InitializeComponent();
            _usuarioActual = usuarioActual ?? throw new ArgumentNullException(nameof(usuarioActual));

            this.Load += FrmBackupRestore_Load;
            this.btnExaminarBackup.Click += BtnExaminarBackup_Click;
            this.btnGenerarBackup.Click += BtnGenerarBackup_Click;
            this.btnExaminarRestore.Click += BtnExaminarRestore_Click;
            this.btnIniciarRestauracion.Click += BtnIniciarRestauracion_Click;
            this.tabContenedor.SelectedIndexChanged += TabContenedor_SelectedIndexChanged;
        }

        private void FrmBackupRestore_Load(object? sender, EventArgs e)
        {
            // 1. Directorio por defecto en el escritorio del usuario o carpeta segura
            string rutaDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (string.IsNullOrWhiteSpace(rutaDesktop) || !Directory.Exists(rutaDesktop))
            {
                rutaDesktop = @"C:\Backups";
            }
            txtRutaDirectorioBackup.Text = rutaDesktop;
            ActualizarNomenclaturaSugerida();

            // 2. Cargar contexto del Administrador en sesión
            txtAdminInfo.Text = $"{_usuarioActual.Nombre} {_usuarioActual.Apellido} ({_usuarioActual.Correo})";

            // 3. Cargar catálogo de Gerentes autorizantes
            CargarGerentesDesdeBD();
        }

        private void TabContenedor_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabContenedor.SelectedTab == tabBackup)
            {
                ActualizarNomenclaturaSugerida();
            }
        }

        private void ActualizarNomenclaturaSugerida()
        {
            lblNomenclaturaPreview.Text = _backupBLL.GenerarNombreSugeridoBackup();
        }

        private void CargarGerentesDesdeBD()
        {
            try
            {
                _gerentesDisponibles = _backupBLL.ObtenerGerentesActivos();
                cmbGerentes.Items.Clear();

                foreach (var gerente in _gerentesDisponibles)
                {
                    cmbGerentes.Items.Add($"{gerente.Nombre} {gerente.Apellido} ({gerente.Correo})");
                }

                if (cmbGerentes.Items.Count > 0)
                {
                    cmbGerentes.SelectedIndex = 0;
                }
                else
                {
                    cmbGerentes.Items.Add("No hay gerentes activos registrados");
                    cmbGerentes.SelectedIndex = 0;
                    cmbGerentes.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo obtener el listado de Gerentes autorizantes:\n{ex.Message}",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnExaminarBackup_Click(object? sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Seleccione la carpeta donde desea guardar el archivo de copia de seguridad (.bak):";
                fbd.UseDescriptionForTitle = true;

                if (!string.IsNullOrWhiteSpace(txtRutaDirectorioBackup.Text) && Directory.Exists(txtRutaDirectorioBackup.Text))
                {
                    fbd.SelectedPath = txtRutaDirectorioBackup.Text;
                }

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtRutaDirectorioBackup.Text = fbd.SelectedPath;
                }
            }
        }

        private void BtnGenerarBackup_Click(object? sender, EventArgs e)
        {
            string directorio = txtRutaDirectorioBackup.Text.Trim();
            if (string.IsNullOrWhiteSpace(directorio))
            {
                MessageBox.Show("Por favor, seleccione o ingrese el directorio de destino para la copia de seguridad.",
                    "Directorio requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRutaDirectorioBackup.Focus();
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                lblEstadoBackup.Text = "Generando copia de seguridad... Por favor espere.";
                lblEstadoBackup.ForeColor = Color.FromArgb(15, 118, 110);
                this.Refresh();

                _backupBLL.CrearBackup(directorio, out string rutaArchivoFinal);

                lblEstadoBackup.Text = $"Última copia generada con éxito a las {DateTime.Now:HH:mm:ss}: {Path.GetFileName(rutaArchivoFinal)}";
                lblEstadoBackup.ForeColor = Color.FromArgb(22, 101, 52);

                MessageBox.Show(
                    $"¡La copia de seguridad se ha generado con éxito!\n\n" +
                    $"Nombre del archivo:\n{Path.GetFileName(rutaArchivoFinal)}\n\n" +
                    $"Ubicación física:\n{rutaArchivoFinal}",
                    "Copia de Seguridad Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ActualizarNomenclaturaSugerida();
            }
            catch (ArgumentException argEx)
            {
                lblEstadoBackup.Text = "Operación cancelada por error de validación.";
                lblEstadoBackup.ForeColor = Color.FromArgb(185, 28, 28);
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                lblEstadoBackup.Text = "Ocurrió un error al generar la copia de seguridad.";
                lblEstadoBackup.ForeColor = Color.FromArgb(185, 28, 28);
                MessageBox.Show($"No se pudo generar la copia de seguridad:\n{ex.Message}",
                    "Error de Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void BtnExaminarRestore_Click(object? sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccione el archivo de copia de seguridad a restaurar";
                ofd.Filter = "Copias de Seguridad SQL (*.bak)|*.bak|Todos los archivos (*.*)|*.*";
                ofd.FilterIndex = 1;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtRutaArchivoRestore.Text = ofd.FileName;
                }
            }
        }

        private void BtnIniciarRestauracion_Click(object? sender, EventArgs e)
        {
            string rutaBak = txtRutaArchivoRestore.Text.Trim();
            if (string.IsNullOrWhiteSpace(rutaBak))
            {
                MessageBox.Show("Por favor, seleccione el archivo de copia de seguridad (.bak) que desea restaurar.",
                    "Archivo Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRutaArchivoRestore.Focus();
                return;
            }

            if (!File.Exists(rutaBak))
            {
                MessageBox.Show("El archivo seleccionado no existe en la ruta especificada. Verifique la ubicación.",
                    "Archivo No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string passAdmin = txtPasswordAdmin.Text;
            if (string.IsNullOrWhiteSpace(passAdmin))
            {
                MessageBox.Show("Debe ingresar obligatoriamente la contraseña del Administrador actual.",
                    "Contraseña de Administrador Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPasswordAdmin.Focus();
                return;
            }

            if (cmbGerentes.SelectedIndex < 0 || _gerentesDisponibles.Count == 0 || cmbGerentes.SelectedIndex >= _gerentesDisponibles.Count)
            {
                MessageBox.Show("Debe seleccionar un Gerente válido habilitado en el sistema.",
                    "Gerente Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGerentes.Focus();
                return;
            }

            UsuarioLoginResult gerenteSeleccionado = _gerentesDisponibles[cmbGerentes.SelectedIndex];

            string passGerente = txtPasswordGerente.Text;
            if (string.IsNullOrWhiteSpace(passGerente))
            {
                MessageBox.Show("Debe ingresar obligatoriamente la contraseña del Gerente autorizante seleccionado.",
                    "Contraseña de Gerente Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPasswordGerente.Focus();
                return;
            }

            // Confirmación explícita ante operación destructiva
            DialogResult dr = MessageBox.Show(
                "⚠️ ADVERTENCIA CRÍTICA:\n\n" +
                "Esta acción restaurará la base de datos del centro médico y sobrescribirá permanentemente todos los datos actuales con la copia seleccionada.\n\n" +
                "¿Está totalmente seguro de que desea proceder con la restauración?",
                "Confirmación de Restauración",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr != DialogResult.Yes)
            {
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                lblEstadoRestore.Text = "Verificando doble autorización e iniciando restauración... Espere un momento.";
                lblEstadoRestore.ForeColor = Color.FromArgb(185, 28, 28);
                this.Refresh();

                _backupBLL.RestaurarBaseDatos(rutaBak, _usuarioActual.Correo, passAdmin, gerenteSeleccionado.Correo, passGerente);

                lblEstadoRestore.Text = "Restauración completada con éxito.";
                lblEstadoRestore.ForeColor = Color.FromArgb(22, 101, 52);

                MessageBox.Show(
                    "¡La base de datos fue restaurada exitosamente!\n\n" +
                    "Por motivos de seguridad e integridad del sistema, se cerrará la sesión actual para reiniciar la conexión con los datos restaurados.",
                    "Restauración Exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Limpiar campos de contraseñas
                txtPasswordAdmin.Clear();
                txtPasswordGerente.Clear();

                // Cerrar la ventana del Administrador y regresar a FrmLogin
                Form? formPadre = this.ParentForm;
                if (formPadre != null && formPadre is FrmAdmin)
                {
                    formPadre.Close();
                }
                else
                {
                    this.Close();
                }

                var frmLogin = Application.OpenForms["FrmLogin"];
                if (frmLogin != null)
                {
                    frmLogin.Show();
                }
            }
            catch (UnauthorizedAccessException authEx)
            {
                lblEstadoRestore.Text = "Doble autorización rechazada.";
                lblEstadoRestore.ForeColor = Color.FromArgb(185, 28, 28);
                MessageBox.Show(authEx.Message, "Autorización Denegada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPasswordAdmin.Clear();
                txtPasswordGerente.Clear();
                txtPasswordAdmin.Focus();
            }
            catch (ArgumentException argEx)
            {
                lblEstadoRestore.Text = "Validación fallida.";
                lblEstadoRestore.ForeColor = Color.FromArgb(185, 28, 28);
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                lblEstadoRestore.Text = "Error al restaurar la base de datos.";
                lblEstadoRestore.ForeColor = Color.FromArgb(185, 28, 28);
                MessageBox.Show($"Ocurrió un error crítico durante la restauración de la base de datos:\n{ex.Message}",
                    "Error de Restauración", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
