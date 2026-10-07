namespace Gestion_de_Turnos_Medicos
{
    partial class FrmAdmin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            pnlNav = new Panel();
            btnPersonalMedico = new Button();
            btnSalas = new Button();
            btnEspecialidades = new Button();
            btnObrasSociales = new Button();
            btnBackup = new Button();
            pnlFooterSidebar = new Panel();
            btnSalir = new Button();
            pnlHeaderSidebar = new Panel();
            pnlSeparador = new Panel();
            lblUsuarioAdmin = new Label();
            lblRolTitulo = new Label();
            lblClinica = new Label();
            pnlContenedor = new Panel();
            panel1.SuspendLayout();
            pnlNav.SuspendLayout();
            pnlFooterSidebar.SuspendLayout();
            pnlHeaderSidebar.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BackColor = Color.FromArgb(15, 23, 42);
            panel1.Controls.Add(pnlNav);
            panel1.Controls.Add(pnlFooterSidebar);
            panel1.Controls.Add(pnlHeaderSidebar);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(240, 524);
            panel1.TabIndex = 0;
            // 
            // pnlNav
            // 
            pnlNav.AutoScroll = true;
            pnlNav.BackColor = Color.Transparent;
            pnlNav.Controls.Add(btnPersonalMedico);
            pnlNav.Controls.Add(btnSalas);
            pnlNav.Controls.Add(btnEspecialidades);
            pnlNav.Controls.Add(btnObrasSociales);
            pnlNav.Controls.Add(btnBackup);
            pnlNav.Dock = DockStyle.Fill;
            pnlNav.Location = new Point(0, 105);
            pnlNav.Name = "pnlNav";
            pnlNav.Size = new Size(240, 349);
            pnlNav.TabIndex = 2;
            // 
            // btnPersonalMedico
            // 
            btnPersonalMedico.BackColor = Color.FromArgb(30, 41, 59);
            btnPersonalMedico.Cursor = Cursors.Hand;
            btnPersonalMedico.FlatAppearance.BorderSize = 0;
            btnPersonalMedico.FlatAppearance.MouseDownBackColor = Color.FromArgb(37, 99, 235);
            btnPersonalMedico.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            btnPersonalMedico.FlatStyle = FlatStyle.Flat;
            btnPersonalMedico.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnPersonalMedico.ForeColor = Color.White;
            btnPersonalMedico.Location = new Point(15, 12);
            btnPersonalMedico.Name = "btnPersonalMedico";
            btnPersonalMedico.Padding = new Padding(12, 0, 0, 0);
            btnPersonalMedico.Size = new Size(210, 46);
            btnPersonalMedico.TabIndex = 0;
            btnPersonalMedico.Text = "👥  Usuarios y Médicos";
            btnPersonalMedico.TextAlign = ContentAlignment.MiddleLeft;
            btnPersonalMedico.UseVisualStyleBackColor = false;
            btnPersonalMedico.Click += btnPersonalMedico_Click;
            // 
            // btnSalas
            // 
            btnSalas.BackColor = Color.FromArgb(30, 41, 59);
            btnSalas.Cursor = Cursors.Hand;
            btnSalas.FlatAppearance.BorderSize = 0;
            btnSalas.FlatAppearance.MouseDownBackColor = Color.FromArgb(37, 99, 235);
            btnSalas.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            btnSalas.FlatStyle = FlatStyle.Flat;
            btnSalas.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnSalas.ForeColor = Color.White;
            btnSalas.Location = new Point(15, 66);
            btnSalas.Name = "btnSalas";
            btnSalas.Padding = new Padding(12, 0, 0, 0);
            btnSalas.Size = new Size(210, 46);
            btnSalas.TabIndex = 1;
            btnSalas.Text = "🏥  Salas y Consultorios";
            btnSalas.TextAlign = ContentAlignment.MiddleLeft;
            btnSalas.UseVisualStyleBackColor = false;
            btnSalas.Click += btnSalas_Click;
            // 
            // btnEspecialidades
            // 
            btnEspecialidades.BackColor = Color.FromArgb(30, 41, 59);
            btnEspecialidades.Cursor = Cursors.Hand;
            btnEspecialidades.FlatAppearance.BorderSize = 0;
            btnEspecialidades.FlatAppearance.MouseDownBackColor = Color.FromArgb(37, 99, 235);
            btnEspecialidades.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            btnEspecialidades.FlatStyle = FlatStyle.Flat;
            btnEspecialidades.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnEspecialidades.ForeColor = Color.White;
            btnEspecialidades.Location = new Point(15, 120);
            btnEspecialidades.Name = "btnEspecialidades";
            btnEspecialidades.Padding = new Padding(12, 0, 0, 0);
            btnEspecialidades.Size = new Size(210, 46);
            btnEspecialidades.TabIndex = 2;
            btnEspecialidades.Text = "🩺  Especialidades";
            btnEspecialidades.TextAlign = ContentAlignment.MiddleLeft;
            btnEspecialidades.UseVisualStyleBackColor = false;
            btnEspecialidades.Click += btnEspecialidades_Click;
            // 
            // btnObrasSociales
            // 
            btnObrasSociales.BackColor = Color.FromArgb(30, 41, 59);
            btnObrasSociales.Cursor = Cursors.Hand;
            btnObrasSociales.FlatAppearance.BorderSize = 0;
            btnObrasSociales.FlatAppearance.MouseDownBackColor = Color.FromArgb(37, 99, 235);
            btnObrasSociales.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            btnObrasSociales.FlatStyle = FlatStyle.Flat;
            btnObrasSociales.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnObrasSociales.ForeColor = Color.White;
            btnObrasSociales.Location = new Point(15, 174);
            btnObrasSociales.Name = "btnObrasSociales";
            btnObrasSociales.Padding = new Padding(12, 0, 0, 0);
            btnObrasSociales.Size = new Size(210, 46);
            btnObrasSociales.TabIndex = 3;
            btnObrasSociales.Text = "💳  Obras Sociales";
            btnObrasSociales.TextAlign = ContentAlignment.MiddleLeft;
            btnObrasSociales.UseVisualStyleBackColor = false;
            btnObrasSociales.Click += btnObrasSociales_Click;
            // 
            // btnBackup
            // 
            btnBackup.BackColor = Color.FromArgb(30, 41, 59);
            btnBackup.Cursor = Cursors.Hand;
            btnBackup.FlatAppearance.BorderSize = 0;
            btnBackup.FlatAppearance.MouseDownBackColor = Color.FromArgb(37, 99, 235);
            btnBackup.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            btnBackup.FlatStyle = FlatStyle.Flat;
            btnBackup.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnBackup.ForeColor = Color.White;
            btnBackup.Location = new Point(15, 228);
            btnBackup.Name = "btnBackup";
            btnBackup.Padding = new Padding(12, 0, 0, 0);
            btnBackup.Size = new Size(210, 46);
            btnBackup.TabIndex = 4;
            btnBackup.Text = "💾  Backup y Restore";
            btnBackup.TextAlign = ContentAlignment.MiddleLeft;
            btnBackup.UseVisualStyleBackColor = false;
            btnBackup.Click += btnBackup_Click;
            // 
            // pnlFooterSidebar
            // 
            pnlFooterSidebar.BackColor = Color.FromArgb(10, 15, 29);
            pnlFooterSidebar.Controls.Add(btnSalir);
            pnlFooterSidebar.Dock = DockStyle.Bottom;
            pnlFooterSidebar.Location = new Point(0, 454);
            pnlFooterSidebar.Name = "pnlFooterSidebar";
            pnlFooterSidebar.Size = new Size(240, 70);
            pnlFooterSidebar.TabIndex = 1;
            // 
            // btnSalir
            // 
            btnSalir.BackColor = Color.FromArgb(127, 29, 29);
            btnSalir.Cursor = Cursors.Hand;
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.FlatAppearance.MouseDownBackColor = Color.FromArgb(153, 27, 27);
            btnSalir.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 38, 38);
            btnSalir.FlatStyle = FlatStyle.Flat;
            btnSalir.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnSalir.ForeColor = Color.White;
            btnSalir.Location = new Point(15, 12);
            btnSalir.Name = "btnSalir";
            btnSalir.Padding = new Padding(12, 0, 0, 0);
            btnSalir.Size = new Size(210, 46);
            btnSalir.TabIndex = 0;
            btnSalir.Text = "🚪  Cerrar Sesión";
            btnSalir.TextAlign = ContentAlignment.MiddleLeft;
            btnSalir.UseVisualStyleBackColor = false;
            btnSalir.Click += btnSalir_Click;
            // 
            // pnlHeaderSidebar
            // 
            pnlHeaderSidebar.BackColor = Color.FromArgb(10, 15, 29);
            pnlHeaderSidebar.Controls.Add(pnlSeparador);
            pnlHeaderSidebar.Controls.Add(lblUsuarioAdmin);
            pnlHeaderSidebar.Controls.Add(lblRolTitulo);
            pnlHeaderSidebar.Controls.Add(lblClinica);
            pnlHeaderSidebar.Dock = DockStyle.Top;
            pnlHeaderSidebar.Location = new Point(0, 0);
            pnlHeaderSidebar.Name = "pnlHeaderSidebar";
            pnlHeaderSidebar.Size = new Size(240, 105);
            pnlHeaderSidebar.TabIndex = 0;
            // 
            // pnlSeparador
            // 
            pnlSeparador.BackColor = Color.FromArgb(51, 65, 85);
            pnlSeparador.Location = new Point(15, 96);
            pnlSeparador.Name = "pnlSeparador";
            pnlSeparador.Size = new Size(210, 1);
            pnlSeparador.TabIndex = 3;
            // 
            // lblUsuarioAdmin
            // 
            lblUsuarioAdmin.AutoEllipsis = true;
            lblUsuarioAdmin.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUsuarioAdmin.ForeColor = Color.FromArgb(241, 245, 249);
            lblUsuarioAdmin.Location = new Point(15, 65);
            lblUsuarioAdmin.Name = "lblUsuarioAdmin";
            lblUsuarioAdmin.Size = new Size(210, 20);
            lblUsuarioAdmin.TabIndex = 2;
            lblUsuarioAdmin.Text = "Administrador";
            // 
            // lblRolTitulo
            // 
            lblRolTitulo.AutoSize = true;
            lblRolTitulo.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRolTitulo.ForeColor = Color.FromArgb(56, 189, 248);
            lblRolTitulo.Location = new Point(15, 38);
            lblRolTitulo.Name = "lblRolTitulo";
            lblRolTitulo.Size = new Size(160, 20);
            lblRolTitulo.TabIndex = 1;
            lblRolTitulo.Text = "🛡️ ADMINISTRACIÓN";
            // 
            // lblClinica
            // 
            lblClinica.AutoSize = true;
            lblClinica.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblClinica.ForeColor = Color.FromArgb(148, 163, 184);
            lblClinica.Location = new Point(15, 16);
            lblClinica.Name = "lblClinica";
            lblClinica.Size = new Size(125, 13);
            lblClinica.TabIndex = 0;
            lblClinica.Text = "GESTIÓN HOSPITALARIA";
            // 
            // pnlContenedor
            // 
            pnlContenedor.BackgroundImage = Properties.Resources.fondo_admin;
            pnlContenedor.Dock = DockStyle.Fill;
            pnlContenedor.Location = new Point(240, 0);
            pnlContenedor.Name = "pnlContenedor";
            pnlContenedor.Size = new Size(925, 524);
            pnlContenedor.TabIndex = 1;
            // 
            // FrmAdmin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.fondo_admin;
            ClientSize = new Size(1165, 524);
            Controls.Add(pnlContenedor);
            Controls.Add(panel1);
            Name = "FrmAdmin";
            Text = "Administracion";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            pnlNav.ResumeLayout(false);
            pnlFooterSidebar.ResumeLayout(false);
            pnlHeaderSidebar.ResumeLayout(false);
            pnlHeaderSidebar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel pnlHeaderSidebar;
        private Label lblClinica;
        private Label lblRolTitulo;
        private Label lblUsuarioAdmin;
        private Panel pnlSeparador;
        private Panel pnlNav;
        private Button btnPersonalMedico;
        private Button btnSalas;
        private Button btnEspecialidades;
        private Button btnObrasSociales;
        private Button btnBackup;
        private Panel pnlFooterSidebar;
        private Button btnSalir;
        private Panel pnlContenedor;
    }
}