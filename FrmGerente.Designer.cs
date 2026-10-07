namespace Gestion_de_Turnos_Medicos
{
    partial class FrmGerente
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panel1 = new Panel();
            pnlNav = new Panel();
            btnReportes = new Button();
            btnGuardia = new Button();
            pnlFooterSidebar = new Panel();
            btnSalir = new Button();
            pnlHeaderSidebar = new Panel();
            pnlSeparador = new Panel();
            LUsuarioInfo = new Label();
            LTituloRol = new Label();
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
            panel1.BackColor = Color.FromArgb(13, 37, 34);
            panel1.Controls.Add(pnlNav);
            panel1.Controls.Add(pnlFooterSidebar);
            panel1.Controls.Add(pnlHeaderSidebar);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(240, 560);
            panel1.TabIndex = 0;
            // 
            // pnlNav
            // 
            pnlNav.AutoScroll = true;
            pnlNav.BackColor = Color.Transparent;
            pnlNav.Controls.Add(btnReportes);
            pnlNav.Controls.Add(btnGuardia);
            pnlNav.Dock = DockStyle.Fill;
            pnlNav.Location = new Point(0, 105);
            pnlNav.Name = "pnlNav";
            pnlNav.Size = new Size(240, 385);
            pnlNav.TabIndex = 2;
            // 
            // btnReportes
            // 
            btnReportes.BackColor = Color.FromArgb(19, 56, 52);
            btnReportes.Cursor = Cursors.Hand;
            btnReportes.FlatAppearance.BorderSize = 0;
            btnReportes.FlatAppearance.MouseDownBackColor = Color.FromArgb(13, 148, 136);
            btnReportes.FlatAppearance.MouseOverBackColor = Color.FromArgb(15, 118, 110);
            btnReportes.FlatStyle = FlatStyle.Flat;
            btnReportes.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnReportes.ForeColor = Color.White;
            btnReportes.Location = new Point(15, 12);
            btnReportes.Name = "btnReportes";
            btnReportes.Padding = new Padding(12, 0, 0, 0);
            btnReportes.Size = new Size(210, 46);
            btnReportes.TabIndex = 0;
            btnReportes.Text = "📊  Reportes y Finanzas";
            btnReportes.TextAlign = ContentAlignment.MiddleLeft;
            btnReportes.UseVisualStyleBackColor = false;
            // 
            // btnGuardia
            // 
            btnGuardia.BackColor = Color.FromArgb(19, 56, 52);
            btnGuardia.Cursor = Cursors.Hand;
            btnGuardia.FlatAppearance.BorderSize = 0;
            btnGuardia.FlatAppearance.MouseDownBackColor = Color.FromArgb(13, 148, 136);
            btnGuardia.FlatAppearance.MouseOverBackColor = Color.FromArgb(15, 118, 110);
            btnGuardia.FlatStyle = FlatStyle.Flat;
            btnGuardia.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnGuardia.ForeColor = Color.White;
            btnGuardia.Location = new Point(15, 66);
            btnGuardia.Name = "btnGuardia";
            btnGuardia.Padding = new Padding(12, 0, 0, 0);
            btnGuardia.Size = new Size(210, 46);
            btnGuardia.TabIndex = 1;
            btnGuardia.Text = "🚨  Auditoría de Triage";
            btnGuardia.TextAlign = ContentAlignment.MiddleLeft;
            btnGuardia.UseVisualStyleBackColor = false;
            // 
            // pnlFooterSidebar
            // 
            pnlFooterSidebar.BackColor = Color.FromArgb(7, 22, 20);
            pnlFooterSidebar.Controls.Add(btnSalir);
            pnlFooterSidebar.Dock = DockStyle.Bottom;
            pnlFooterSidebar.Location = new Point(0, 490);
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
            // 
            // pnlHeaderSidebar
            // 
            pnlHeaderSidebar.BackColor = Color.FromArgb(7, 22, 20);
            pnlHeaderSidebar.Controls.Add(pnlSeparador);
            pnlHeaderSidebar.Controls.Add(LUsuarioInfo);
            pnlHeaderSidebar.Controls.Add(LTituloRol);
            pnlHeaderSidebar.Controls.Add(lblClinica);
            pnlHeaderSidebar.Dock = DockStyle.Top;
            pnlHeaderSidebar.Location = new Point(0, 0);
            pnlHeaderSidebar.Name = "pnlHeaderSidebar";
            pnlHeaderSidebar.Size = new Size(240, 105);
            pnlHeaderSidebar.TabIndex = 0;
            // 
            // pnlSeparador
            // 
            pnlSeparador.BackColor = Color.FromArgb(20, 83, 76);
            pnlSeparador.Location = new Point(15, 96);
            pnlSeparador.Name = "pnlSeparador";
            pnlSeparador.Size = new Size(210, 1);
            pnlSeparador.TabIndex = 3;
            // 
            // LUsuarioInfo
            // 
            LUsuarioInfo.AutoEllipsis = true;
            LUsuarioInfo.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LUsuarioInfo.ForeColor = Color.FromArgb(240, 253, 244);
            LUsuarioInfo.Location = new Point(15, 65);
            LUsuarioInfo.Name = "LUsuarioInfo";
            LUsuarioInfo.Size = new Size(210, 20);
            LUsuarioInfo.TabIndex = 2;
            LUsuarioInfo.Text = "Gerente";
            // 
            // LTituloRol
            // 
            LTituloRol.AutoSize = true;
            LTituloRol.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LTituloRol.ForeColor = Color.FromArgb(52, 211, 153);
            LTituloRol.Location = new Point(15, 38);
            LTituloRol.Name = "LTituloRol";
            LTituloRol.Size = new Size(183, 20);
            LTituloRol.TabIndex = 1;
            LTituloRol.Text = "👔 DIRECCIÓN GERENCIAL";
            // 
            // lblClinica
            // 
            lblClinica.AutoSize = true;
            lblClinica.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblClinica.ForeColor = Color.FromArgb(110, 231, 183);
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
            pnlContenedor.Size = new Size(768, 560);
            pnlContenedor.TabIndex = 1;
            // 
            // FrmGerente
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.fondo_admin;
            ClientSize = new Size(1008, 560);
            Controls.Add(pnlContenedor);
            Controls.Add(panel1);
            Name = "FrmGerente";
            Text = "Panel de Dirección Gerencial";
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
        private Label LTituloRol;
        private Label LUsuarioInfo;
        private Panel pnlSeparador;
        private Panel pnlNav;
        private Button btnReportes;
        private Button btnGuardia;
        private Panel pnlFooterSidebar;
        private Button btnSalir;
        private Panel pnlContenedor;
    }
}
