namespace Gestion_de_Turnos_Medicos
{
    partial class FrmBackupRestore
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se debe modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblSubtituloHeader = new Label();
            lblTituloHeader = new Label();
            tabContenedor = new TabControl();
            tabBackup = new TabPage();
            pnlCardBackup = new Panel();
            lblEstadoBackup = new Label();
            btnGenerarBackup = new Button();
            pnlNomenclatura = new Panel();
            lblEjemploNomenclatura = new Label();
            lblTituloNomenclatura = new Label();
            lblNomenclaturaPreview = new Label();
            pnlRutaBackup = new Panel();
            btnExaminarBackup = new Button();
            txtRutaDirectorioBackup = new TextBox();
            lblRutaDirectorio = new Label();
            pnlBannerInfoBackup = new Panel();
            lblBannerBackup = new Label();
            tabRestore = new TabPage();
            pnlCardRestore = new Panel();
            lblEstadoRestore = new Label();
            btnIniciarRestauracion = new Button();
            gbDobleAutorizacion = new GroupBox();
            pnlGerenteAuth = new Panel();
            lblPasswordGerente = new Label();
            txtPasswordGerente = new TextBox();
            lblSeleccionarGerente = new Label();
            cmbGerentes = new ComboBox();
            lblTituloGerente = new Label();
            pnlAdminAuth = new Panel();
            lblPasswordAdmin = new Label();
            txtPasswordAdmin = new TextBox();
            lblAdminActual = new Label();
            txtAdminInfo = new TextBox();
            lblTituloAdmin = new Label();
            pnlRutaRestore = new Panel();
            btnExaminarRestore = new Button();
            txtRutaArchivoRestore = new TextBox();
            lblRutaArchivoRestore = new Label();
            pnlBannerAlertaRestore = new Panel();
            lblAlertaRestore = new Label();
            pnlHeader.SuspendLayout();
            tabContenedor.SuspendLayout();
            tabBackup.SuspendLayout();
            pnlCardBackup.SuspendLayout();
            pnlNomenclatura.SuspendLayout();
            pnlRutaBackup.SuspendLayout();
            pnlBannerInfoBackup.SuspendLayout();
            tabRestore.SuspendLayout();
            pnlCardRestore.SuspendLayout();
            gbDobleAutorizacion.SuspendLayout();
            pnlGerenteAuth.SuspendLayout();
            pnlAdminAuth.SuspendLayout();
            pnlRutaRestore.SuspendLayout();
            pnlBannerAlertaRestore.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(15, 23, 42);
            pnlHeader.Controls.Add(lblSubtituloHeader);
            pnlHeader.Controls.Add(lblTituloHeader);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1000, 64);
            pnlHeader.TabIndex = 0;
            // 
            // lblSubtituloHeader
            // 
            lblSubtituloHeader.AutoSize = true;
            lblSubtituloHeader.Font = new Font("Segoe UI", 9F);
            lblSubtituloHeader.ForeColor = Color.FromArgb(148, 163, 184);
            lblSubtituloHeader.Location = new Point(20, 36);
            lblSubtituloHeader.Name = "lblSubtituloHeader";
            lblSubtituloHeader.Size = new Size(618, 15);
            lblSubtituloHeader.TabIndex = 1;
            lblSubtituloHeader.Text = "Resguardo a demanda y recuperación integral de la base de datos dbGestionTurnos con doble autorización obligatoria";
            // 
            // lblTituloHeader
            // 
            lblTituloHeader.AutoSize = true;
            lblTituloHeader.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            lblTituloHeader.ForeColor = Color.White;
            lblTituloHeader.Location = new Point(18, 9);
            lblTituloHeader.Name = "lblTituloHeader";
            lblTituloHeader.Size = new Size(491, 25);
            lblTituloHeader.TabIndex = 0;
            lblTituloHeader.Text = "Copia de Seguridad y Restauración de Base de Datos";
            // 
            // tabContenedor
            // 
            tabContenedor.Controls.Add(tabBackup);
            tabContenedor.Controls.Add(tabRestore);
            tabContenedor.Dock = DockStyle.Fill;
            tabContenedor.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            tabContenedor.Location = new Point(0, 64);
            tabContenedor.Name = "tabContenedor";
            tabContenedor.Padding = new Point(16, 8);
            tabContenedor.SelectedIndex = 0;
            tabContenedor.Size = new Size(1000, 586);
            tabContenedor.TabIndex = 1;
            // 
            // tabBackup
            // 
            tabBackup.BackColor = Color.FromArgb(241, 245, 249);
            tabBackup.Controls.Add(pnlCardBackup);
            tabBackup.Location = new Point(4, 36);
            tabBackup.Name = "tabBackup";
            tabBackup.Padding = new Padding(20);
            tabBackup.Size = new Size(992, 546);
            tabBackup.TabIndex = 0;
            tabBackup.Text = "  💾 Copia de Seguridad (Backup)  ";
            // 
            // pnlCardBackup
            // 
            pnlCardBackup.BackColor = Color.White;
            pnlCardBackup.BorderStyle = BorderStyle.FixedSingle;
            pnlCardBackup.Controls.Add(lblEstadoBackup);
            pnlCardBackup.Controls.Add(btnGenerarBackup);
            pnlCardBackup.Controls.Add(pnlNomenclatura);
            pnlCardBackup.Controls.Add(pnlRutaBackup);
            pnlCardBackup.Controls.Add(pnlBannerInfoBackup);
            pnlCardBackup.Dock = DockStyle.Fill;
            pnlCardBackup.Location = new Point(20, 20);
            pnlCardBackup.Name = "pnlCardBackup";
            pnlCardBackup.Padding = new Padding(24);
            pnlCardBackup.Size = new Size(952, 506);
            pnlCardBackup.TabIndex = 0;
            // 
            // lblEstadoBackup
            // 
            lblEstadoBackup.AutoSize = true;
            lblEstadoBackup.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblEstadoBackup.ForeColor = Color.FromArgb(100, 116, 139);
            lblEstadoBackup.Location = new Point(24, 400);
            lblEstadoBackup.Name = "lblEstadoBackup";
            lblEstadoBackup.Size = new Size(244, 15);
            lblEstadoBackup.TabIndex = 4;
            lblEstadoBackup.Text = "Listo para iniciar la generación de la copia.";
            // 
            // btnGenerarBackup
            // 
            btnGenerarBackup.BackColor = Color.FromArgb(15, 118, 110);
            btnGenerarBackup.Cursor = Cursors.Hand;
            btnGenerarBackup.FlatAppearance.BorderSize = 0;
            btnGenerarBackup.FlatStyle = FlatStyle.Flat;
            btnGenerarBackup.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnGenerarBackup.ForeColor = Color.White;
            btnGenerarBackup.Location = new Point(24, 340);
            btnGenerarBackup.Name = "btnGenerarBackup";
            btnGenerarBackup.Size = new Size(320, 46);
            btnGenerarBackup.TabIndex = 3;
            btnGenerarBackup.Text = "💾 Generar Copia de Seguridad";
            btnGenerarBackup.UseVisualStyleBackColor = false;
            // 
            // pnlNomenclatura
            // 
            pnlNomenclatura.BackColor = Color.FromArgb(248, 250, 252);
            pnlNomenclatura.BorderStyle = BorderStyle.FixedSingle;
            pnlNomenclatura.Controls.Add(lblEjemploNomenclatura);
            pnlNomenclatura.Controls.Add(lblTituloNomenclatura);
            pnlNomenclatura.Controls.Add(lblNomenclaturaPreview);
            pnlNomenclatura.Location = new Point(24, 215);
            pnlNomenclatura.Name = "pnlNomenclatura";
            pnlNomenclatura.Padding = new Padding(16);
            pnlNomenclatura.Size = new Size(890, 95);
            pnlNomenclatura.TabIndex = 2;
            // 
            // lblEjemploNomenclatura
            // 
            lblEjemploNomenclatura.AutoSize = true;
            lblEjemploNomenclatura.Font = new Font("Segoe UI", 8.5F);
            lblEjemploNomenclatura.ForeColor = Color.FromArgb(100, 116, 139);
            lblEjemploNomenclatura.Location = new Point(16, 62);
            lblEjemploNomenclatura.Name = "lblEjemploNomenclatura";
            lblEjemploNomenclatura.Size = new Size(475, 15);
            lblEjemploNomenclatura.TabIndex = 2;
            lblEjemploNomenclatura.Text = "La estampa de fecha y hora se calculará con precisión de segundo al momento de crearlo.";
            // 
            // lblTituloNomenclatura
            // 
            lblTituloNomenclatura.AutoSize = true;
            lblTituloNomenclatura.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblTituloNomenclatura.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloNomenclatura.Location = new Point(16, 12);
            lblTituloNomenclatura.Name = "lblTituloNomenclatura";
            lblTituloNomenclatura.Size = new Size(335, 17);
            lblTituloNomenclatura.TabIndex = 0;
            lblTituloNomenclatura.Text = "Nomenclatura reglamentaria generada con fecha y hora:";
            // 
            // lblNomenclaturaPreview
            // 
            lblNomenclaturaPreview.AutoSize = true;
            lblNomenclaturaPreview.Font = new Font("Consolas", 11.25F, FontStyle.Bold);
            lblNomenclaturaPreview.ForeColor = Color.FromArgb(15, 118, 110);
            lblNomenclaturaPreview.Location = new Point(16, 36);
            lblNomenclaturaPreview.Name = "lblNomenclaturaPreview";
            lblNomenclaturaPreview.Size = new Size(384, 18);
            lblNomenclaturaPreview.TabIndex = 1;
            lblNomenclaturaPreview.Text = "dbGestionTurnos_Backup_AAAA-MM-DD_HHMMSS.bak";
            // 
            // pnlRutaBackup
            // 
            pnlRutaBackup.Controls.Add(btnExaminarBackup);
            pnlRutaBackup.Controls.Add(txtRutaDirectorioBackup);
            pnlRutaBackup.Controls.Add(lblRutaDirectorio);
            pnlRutaBackup.Location = new Point(24, 110);
            pnlRutaBackup.Name = "pnlRutaBackup";
            pnlRutaBackup.Size = new Size(890, 85);
            pnlRutaBackup.TabIndex = 1;
            // 
            // btnExaminarBackup
            // 
            btnExaminarBackup.BackColor = Color.FromArgb(226, 232, 240);
            btnExaminarBackup.Cursor = Cursors.Hand;
            btnExaminarBackup.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnExaminarBackup.FlatStyle = FlatStyle.Flat;
            btnExaminarBackup.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnExaminarBackup.ForeColor = Color.FromArgb(30, 41, 59);
            btnExaminarBackup.Location = new Point(710, 32);
            btnExaminarBackup.Name = "btnExaminarBackup";
            btnExaminarBackup.Size = new Size(180, 34);
            btnExaminarBackup.TabIndex = 2;
            btnExaminarBackup.Text = "📂 Examinar carpeta...";
            btnExaminarBackup.UseVisualStyleBackColor = false;
            // 
            // txtRutaDirectorioBackup
            // 
            txtRutaDirectorioBackup.Font = new Font("Segoe UI", 10F);
            txtRutaDirectorioBackup.Location = new Point(0, 34);
            txtRutaDirectorioBackup.Name = "txtRutaDirectorioBackup";
            txtRutaDirectorioBackup.Size = new Size(700, 25);
            txtRutaDirectorioBackup.TabIndex = 1;
            // 
            // lblRutaDirectorio
            // 
            lblRutaDirectorio.AutoSize = true;
            lblRutaDirectorio.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblRutaDirectorio.ForeColor = Color.FromArgb(30, 41, 59);
            lblRutaDirectorio.Location = new Point(0, 8);
            lblRutaDirectorio.Name = "lblRutaDirectorio";
            lblRutaDirectorio.Size = new Size(335, 17);
            lblRutaDirectorio.TabIndex = 0;
            lblRutaDirectorio.Text = "Directorio de destino para la copia de seguridad:";
            // 
            // pnlBannerInfoBackup
            // 
            pnlBannerInfoBackup.BackColor = Color.FromArgb(240, 253, 250);
            pnlBannerInfoBackup.BorderStyle = BorderStyle.FixedSingle;
            pnlBannerInfoBackup.Controls.Add(lblBannerBackup);
            pnlBannerInfoBackup.Dock = DockStyle.Top;
            pnlBannerInfoBackup.Location = new Point(24, 24);
            pnlBannerInfoBackup.Name = "pnlBannerInfoBackup";
            pnlBannerInfoBackup.Padding = new Padding(12);
            pnlBannerInfoBackup.Size = new Size(902, 60);
            pnlBannerInfoBackup.TabIndex = 0;
            // 
            // lblBannerBackup
            // 
            lblBannerBackup.AutoSize = true;
            lblBannerBackup.Font = new Font("Segoe UI", 9.5F);
            lblBannerBackup.ForeColor = Color.FromArgb(13, 148, 136);
            lblBannerBackup.Location = new Point(12, 12);
            lblBannerBackup.Name = "lblBannerBackup";
            lblBannerBackup.Size = new Size(734, 34);
            lblBannerBackup.TabIndex = 0;
            lblBannerBackup.Text = "ℹ️ La función a demanda genera una copia física íntegra de la base de datos SQL Server 'dbGestionTurnos'.\r\nPuede seleccionar cualquier carpeta de su disco o unidad extraíble.";
            // 
            // tabRestore
            // 
            tabRestore.BackColor = Color.FromArgb(241, 245, 249);
            tabRestore.Controls.Add(pnlCardRestore);
            tabRestore.Location = new Point(4, 36);
            tabRestore.Name = "tabRestore";
            tabRestore.Padding = new Padding(20);
            tabRestore.Size = new Size(992, 546);
            tabRestore.TabIndex = 1;
            tabRestore.Text = "  🔄 Restauración (Restore)  ";
            // 
            // pnlCardRestore
            // 
            pnlCardRestore.AutoScroll = true;
            pnlCardRestore.BackColor = Color.White;
            pnlCardRestore.BorderStyle = BorderStyle.FixedSingle;
            pnlCardRestore.Controls.Add(lblEstadoRestore);
            pnlCardRestore.Controls.Add(btnIniciarRestauracion);
            pnlCardRestore.Controls.Add(gbDobleAutorizacion);
            pnlCardRestore.Controls.Add(pnlRutaRestore);
            pnlCardRestore.Controls.Add(pnlBannerAlertaRestore);
            pnlCardRestore.Dock = DockStyle.Fill;
            pnlCardRestore.Location = new Point(20, 20);
            pnlCardRestore.Name = "pnlCardRestore";
            pnlCardRestore.Padding = new Padding(24);
            pnlCardRestore.Size = new Size(952, 506);
            pnlCardRestore.TabIndex = 0;
            // 
            // lblEstadoRestore
            // 
            lblEstadoRestore.AutoSize = true;
            lblEstadoRestore.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblEstadoRestore.ForeColor = Color.FromArgb(100, 116, 139);
            lblEstadoRestore.Location = new Point(24, 460);
            lblEstadoRestore.Name = "lblEstadoRestore";
            lblEstadoRestore.Size = new Size(331, 15);
            lblEstadoRestore.TabIndex = 4;
            lblEstadoRestore.Text = "Seleccione el archivo y complete ambas autorizaciones para restaurar.";
            // 
            // btnIniciarRestauracion
            // 
            btnIniciarRestauracion.BackColor = Color.FromArgb(185, 28, 28);
            btnIniciarRestauracion.Cursor = Cursors.Hand;
            btnIniciarRestauracion.FlatAppearance.BorderSize = 0;
            btnIniciarRestauracion.FlatStyle = FlatStyle.Flat;
            btnIniciarRestauracion.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnIniciarRestauracion.ForeColor = Color.White;
            btnIniciarRestauracion.Location = new Point(24, 400);
            btnIniciarRestauracion.Name = "btnIniciarRestauracion";
            btnIniciarRestauracion.Size = new Size(360, 46);
            btnIniciarRestauracion.TabIndex = 3;
            btnIniciarRestauracion.Text = "⚠️ Iniciar Restauración de Base de Datos";
            btnIniciarRestauracion.UseVisualStyleBackColor = false;
            // 
            // gbDobleAutorizacion
            // 
            gbDobleAutorizacion.Controls.Add(pnlGerenteAuth);
            gbDobleAutorizacion.Controls.Add(pnlAdminAuth);
            gbDobleAutorizacion.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            gbDobleAutorizacion.ForeColor = Color.FromArgb(15, 23, 42);
            gbDobleAutorizacion.Location = new Point(24, 200);
            gbDobleAutorizacion.Name = "gbDobleAutorizacion";
            gbDobleAutorizacion.Padding = new Padding(16);
            gbDobleAutorizacion.Size = new Size(890, 180);
            gbDobleAutorizacion.TabIndex = 2;
            gbDobleAutorizacion.TabStop = false;
            gbDobleAutorizacion.Text = "🔐 Doble Autorización de Seguridad Obligatoria";
            // 
            // pnlGerenteAuth
            // 
            pnlGerenteAuth.BackColor = Color.FromArgb(248, 250, 252);
            pnlGerenteAuth.BorderStyle = BorderStyle.FixedSingle;
            pnlGerenteAuth.Controls.Add(lblPasswordGerente);
            pnlGerenteAuth.Controls.Add(txtPasswordGerente);
            pnlGerenteAuth.Controls.Add(lblSeleccionarGerente);
            pnlGerenteAuth.Controls.Add(cmbGerentes);
            pnlGerenteAuth.Controls.Add(lblTituloGerente);
            pnlGerenteAuth.Location = new Point(450, 30);
            pnlGerenteAuth.Name = "pnlGerenteAuth";
            pnlGerenteAuth.Padding = new Padding(12);
            pnlGerenteAuth.Size = new Size(420, 135);
            pnlGerenteAuth.TabIndex = 1;
            // 
            // lblPasswordGerente
            // 
            lblPasswordGerente.AutoSize = true;
            lblPasswordGerente.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblPasswordGerente.ForeColor = Color.FromArgb(71, 85, 105);
            lblPasswordGerente.Location = new Point(12, 78);
            lblPasswordGerente.Name = "lblPasswordGerente";
            lblPasswordGerente.Size = new Size(139, 15);
            lblPasswordGerente.TabIndex = 4;
            lblPasswordGerente.Text = "Contraseña del Gerente:";
            // 
            // txtPasswordGerente
            // 
            txtPasswordGerente.Font = new Font("Segoe UI", 9.5F);
            txtPasswordGerente.Location = new Point(12, 97);
            txtPasswordGerente.Name = "txtPasswordGerente";
            txtPasswordGerente.PasswordChar = '●';
            txtPasswordGerente.Size = new Size(390, 24);
            txtPasswordGerente.TabIndex = 3;
            // 
            // lblSeleccionarGerente
            // 
            lblSeleccionarGerente.AutoSize = true;
            lblSeleccionarGerente.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblSeleccionarGerente.ForeColor = Color.FromArgb(71, 85, 105);
            lblSeleccionarGerente.Location = new Point(12, 32);
            lblSeleccionarGerente.Name = "lblSeleccionarGerente";
            lblSeleccionarGerente.Size = new Size(122, 15);
            lblSeleccionarGerente.TabIndex = 2;
            lblSeleccionarGerente.Text = "Gerente Autorizante:";
            // 
            // cmbGerentes
            // 
            cmbGerentes.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGerentes.Font = new Font("Segoe UI", 9.5F);
            cmbGerentes.FormattingEnabled = true;
            cmbGerentes.Location = new Point(12, 50);
            cmbGerentes.Name = "cmbGerentes";
            cmbGerentes.Size = new Size(390, 24);
            cmbGerentes.TabIndex = 1;
            // 
            // lblTituloGerente
            // 
            lblTituloGerente.AutoSize = true;
            lblTituloGerente.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblTituloGerente.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloGerente.Location = new Point(10, 8);
            lblTituloGerente.Name = "lblTituloGerente";
            lblTituloGerente.Size = new Size(164, 17);
            lblTituloGerente.TabIndex = 0;
            lblTituloGerente.Text = "Autorización 2: Gerente";
            // 
            // pnlAdminAuth
            // 
            pnlAdminAuth.BackColor = Color.FromArgb(248, 250, 252);
            pnlAdminAuth.BorderStyle = BorderStyle.FixedSingle;
            pnlAdminAuth.Controls.Add(lblPasswordAdmin);
            pnlAdminAuth.Controls.Add(txtPasswordAdmin);
            pnlAdminAuth.Controls.Add(lblAdminActual);
            pnlAdminAuth.Controls.Add(txtAdminInfo);
            pnlAdminAuth.Controls.Add(lblTituloAdmin);
            pnlAdminAuth.Location = new Point(16, 30);
            pnlAdminAuth.Name = "pnlAdminAuth";
            pnlAdminAuth.Padding = new Padding(12);
            pnlAdminAuth.Size = new Size(420, 135);
            pnlAdminAuth.TabIndex = 0;
            // 
            // lblPasswordAdmin
            // 
            lblPasswordAdmin.AutoSize = true;
            lblPasswordAdmin.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblPasswordAdmin.ForeColor = Color.FromArgb(71, 85, 105);
            lblPasswordAdmin.Location = new Point(12, 78);
            lblPasswordAdmin.Name = "lblPasswordAdmin";
            lblPasswordAdmin.Size = new Size(174, 15);
            lblPasswordAdmin.TabIndex = 4;
            lblPasswordAdmin.Text = "Contraseña del Administrador:";
            // 
            // txtPasswordAdmin
            // 
            txtPasswordAdmin.Font = new Font("Segoe UI", 9.5F);
            txtPasswordAdmin.Location = new Point(12, 97);
            txtPasswordAdmin.Name = "txtPasswordAdmin";
            txtPasswordAdmin.PasswordChar = '●';
            txtPasswordAdmin.Size = new Size(390, 24);
            txtPasswordAdmin.TabIndex = 3;
            // 
            // lblAdminActual
            // 
            lblAdminActual.AutoSize = true;
            lblAdminActual.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblAdminActual.ForeColor = Color.FromArgb(71, 85, 105);
            lblAdminActual.Location = new Point(12, 32);
            lblAdminActual.Name = "lblAdminActual";
            lblAdminActual.Size = new Size(133, 15);
            lblAdminActual.TabIndex = 2;
            lblAdminActual.Text = "Administrador Actual:";
            // 
            // txtAdminInfo
            // 
            txtAdminInfo.BackColor = Color.FromArgb(241, 245, 249);
            txtAdminInfo.Font = new Font("Segoe UI", 9.5F);
            txtAdminInfo.Location = new Point(12, 50);
            txtAdminInfo.Name = "txtAdminInfo";
            txtAdminInfo.ReadOnly = true;
            txtAdminInfo.Size = new Size(390, 24);
            txtAdminInfo.TabIndex = 1;
            // 
            // lblTituloAdmin
            // 
            lblTituloAdmin.AutoSize = true;
            lblTituloAdmin.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblTituloAdmin.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloAdmin.Location = new Point(10, 8);
            lblTituloAdmin.Name = "lblTituloAdmin";
            lblTituloAdmin.Size = new Size(207, 17);
            lblTituloAdmin.TabIndex = 0;
            lblTituloAdmin.Text = "Autorización 1: Administrador";
            // 
            // pnlRutaRestore
            // 
            pnlRutaRestore.Controls.Add(btnExaminarRestore);
            pnlRutaRestore.Controls.Add(txtRutaArchivoRestore);
            pnlRutaRestore.Controls.Add(lblRutaArchivoRestore);
            pnlRutaRestore.Location = new Point(24, 105);
            pnlRutaRestore.Name = "pnlRutaRestore";
            pnlRutaRestore.Size = new Size(890, 85);
            pnlRutaRestore.TabIndex = 1;
            // 
            // btnExaminarRestore
            // 
            btnExaminarRestore.BackColor = Color.FromArgb(226, 232, 240);
            btnExaminarRestore.Cursor = Cursors.Hand;
            btnExaminarRestore.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnExaminarRestore.FlatStyle = FlatStyle.Flat;
            btnExaminarRestore.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnExaminarRestore.ForeColor = Color.FromArgb(30, 41, 59);
            btnExaminarRestore.Location = new Point(710, 32);
            btnExaminarRestore.Name = "btnExaminarRestore";
            btnExaminarRestore.Size = new Size(180, 34);
            btnExaminarRestore.TabIndex = 2;
            btnExaminarRestore.Text = "📂 Examinar archivo...";
            btnExaminarRestore.UseVisualStyleBackColor = false;
            // 
            // txtRutaArchivoRestore
            // 
            txtRutaArchivoRestore.Font = new Font("Segoe UI", 10F);
            txtRutaArchivoRestore.Location = new Point(0, 34);
            txtRutaArchivoRestore.Name = "txtRutaArchivoRestore";
            txtRutaArchivoRestore.Size = new Size(700, 25);
            txtRutaArchivoRestore.TabIndex = 1;
            // 
            // lblRutaArchivoRestore
            // 
            lblRutaArchivoRestore.AutoSize = true;
            lblRutaArchivoRestore.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblRutaArchivoRestore.ForeColor = Color.FromArgb(30, 41, 59);
            lblRutaArchivoRestore.Location = new Point(0, 8);
            lblRutaArchivoRestore.Name = "lblRutaArchivoRestore";
            lblRutaArchivoRestore.Size = new Size(333, 17);
            lblRutaArchivoRestore.TabIndex = 0;
            lblRutaArchivoRestore.Text = "Archivo de copia de seguridad (.bak) a restaurar:";
            // 
            // pnlBannerAlertaRestore
            // 
            pnlBannerAlertaRestore.BackColor = Color.FromArgb(254, 242, 242);
            pnlBannerAlertaRestore.BorderStyle = BorderStyle.FixedSingle;
            pnlBannerAlertaRestore.Controls.Add(lblAlertaRestore);
            pnlBannerAlertaRestore.Dock = DockStyle.Top;
            pnlBannerAlertaRestore.Location = new Point(24, 24);
            pnlBannerAlertaRestore.Name = "pnlBannerAlertaRestore";
            pnlBannerAlertaRestore.Padding = new Padding(12);
            pnlBannerAlertaRestore.Size = new Size(902, 65);
            pnlBannerAlertaRestore.TabIndex = 0;
            // 
            // lblAlertaRestore
            // 
            lblAlertaRestore.AutoSize = true;
            lblAlertaRestore.Font = new Font("Segoe UI", 9.5F);
            lblAlertaRestore.ForeColor = Color.FromArgb(185, 28, 28);
            lblAlertaRestore.Location = new Point(12, 10);
            lblAlertaRestore.Name = "lblAlertaRestore";
            lblAlertaRestore.Size = new Size(826, 34);
            lblAlertaRestore.TabIndex = 0;
            lblAlertaRestore.Text = "⚠️ ATENCIÓN: La restauración es una operación destructiva que sobrescribirá la base de datos actual con el contenido de la copia.\r\nPor seguridad del centro médico, el sistema exige autorización explícita de un Administrador y un Gerente mediante contraseña.";
            // 
            // FrmBackupRestore
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(241, 245, 249);
            ClientSize = new Size(1000, 650);
            Controls.Add(tabContenedor);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmBackupRestore";
            Text = "Copia de Seguridad y Restauración";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            tabContenedor.ResumeLayout(false);
            tabBackup.ResumeLayout(false);
            pnlCardBackup.ResumeLayout(false);
            pnlCardBackup.PerformLayout();
            pnlNomenclatura.ResumeLayout(false);
            pnlNomenclatura.PerformLayout();
            pnlRutaBackup.ResumeLayout(false);
            pnlRutaBackup.PerformLayout();
            pnlBannerInfoBackup.ResumeLayout(false);
            pnlBannerInfoBackup.PerformLayout();
            tabRestore.ResumeLayout(false);
            pnlCardRestore.ResumeLayout(false);
            pnlCardRestore.PerformLayout();
            gbDobleAutorizacion.ResumeLayout(false);
            pnlGerenteAuth.ResumeLayout(false);
            pnlGerenteAuth.PerformLayout();
            pnlAdminAuth.ResumeLayout(false);
            pnlAdminAuth.PerformLayout();
            pnlRutaRestore.ResumeLayout(false);
            pnlRutaRestore.PerformLayout();
            pnlBannerAlertaRestore.ResumeLayout(false);
            pnlBannerAlertaRestore.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblTituloHeader;
        private Label lblSubtituloHeader;
        private TabControl tabContenedor;
        private TabPage tabBackup;
        private TabPage tabRestore;
        private Panel pnlCardBackup;
        private Panel pnlBannerInfoBackup;
        private Label lblBannerBackup;
        private Panel pnlRutaBackup;
        private Label lblRutaDirectorio;
        private TextBox txtRutaDirectorioBackup;
        private Button btnExaminarBackup;
        private Panel pnlNomenclatura;
        private Label lblTituloNomenclatura;
        private Label lblNomenclaturaPreview;
        private Label lblEjemploNomenclatura;
        private Button btnGenerarBackup;
        private Label lblEstadoBackup;
        private Panel pnlCardRestore;
        private Panel pnlBannerAlertaRestore;
        private Label lblAlertaRestore;
        private Panel pnlRutaRestore;
        private Label lblRutaArchivoRestore;
        private TextBox txtRutaArchivoRestore;
        private Button btnExaminarRestore;
        private GroupBox gbDobleAutorizacion;
        private Panel pnlAdminAuth;
        private Label lblTituloAdmin;
        private TextBox txtAdminInfo;
        private Label lblAdminActual;
        private Label lblPasswordAdmin;
        private TextBox txtPasswordAdmin;
        private Panel pnlGerenteAuth;
        private Label lblTituloGerente;
        private Label lblSeleccionarGerente;
        private ComboBox cmbGerentes;
        private Label lblPasswordGerente;
        private TextBox txtPasswordGerente;
        private Button btnIniciarRestauracion;
        private Label lblEstadoRestore;
    }
}
