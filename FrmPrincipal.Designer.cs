namespace Gestion_de_Turnos_Medicos
{
    partial class FrmPrincipal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPrincipal));
            panel1 = new Panel();
            pnlNav = new Panel();
            pnlFooterSidebar = new Panel();
            btnSalir = new Button();
            pnlHeaderSidebar = new Panel();
            pnlSeparador = new Panel();
            lblUsuario = new Label();
            lblRolTitulo = new Label();
            lblClinica = new Label();
            pnlContenedor = new Panel();
            panel1.SuspendLayout();
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
            pnlNav.Dock = DockStyle.Fill;
            pnlNav.Location = new Point(0, 105);
            pnlNav.Name = "pnlNav";
            pnlNav.Size = new Size(240, 349);
            pnlNav.TabIndex = 2;
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
            //
            // pnlHeaderSidebar
            //
            pnlHeaderSidebar.BackColor = Color.FromArgb(10, 15, 29);
            pnlHeaderSidebar.Controls.Add(pnlSeparador);
            pnlHeaderSidebar.Controls.Add(lblUsuario);
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
            // lblUsuario
            //
            lblUsuario.AutoEllipsis = true;
            lblUsuario.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUsuario.ForeColor = Color.FromArgb(241, 245, 249);
            lblUsuario.Location = new Point(15, 65);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(210, 20);
            lblUsuario.TabIndex = 2;
            lblUsuario.Text = "Usuario";
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
            lblRolTitulo.Text = "ROL";
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
            pnlContenedor.Dock = DockStyle.Fill;
            pnlContenedor.Location = new Point(240, 0);
            pnlContenedor.Name = "pnlContenedor";
            pnlContenedor.Size = new Size(925, 524);
            pnlContenedor.TabIndex = 1;
            //
            // FrmPrincipal
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1165, 524);
            Controls.Add(pnlContenedor);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmPrincipal";
            Text = "Pantalla Principal";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
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
        private Label lblUsuario;
        private Panel pnlSeparador;
        private Panel pnlNav;
        private Panel pnlFooterSidebar;
        private Button btnSalir;
        private Panel pnlContenedor;
    }
}
