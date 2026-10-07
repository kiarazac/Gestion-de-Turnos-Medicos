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
            Lsalir = new Label();
            btnSalir = new Button();
            LGuardia = new Label();
            btnGuardia = new Button();
            LReportes = new Label();
            btnReportes = new Button();
            LUsuarioInfo = new Label();
            LTituloRol = new Label();
            pnlContenedor = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.Controls.Add(Lsalir);
            panel1.Controls.Add(btnSalir);
            panel1.Controls.Add(LGuardia);
            panel1.Controls.Add(btnGuardia);
            panel1.Controls.Add(LReportes);
            panel1.Controls.Add(btnReportes);
            panel1.Controls.Add(LUsuarioInfo);
            panel1.Controls.Add(LTituloRol);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 560);
            panel1.TabIndex = 0;
            // 
            // LTituloRol
            // 
            LTituloRol.AutoSize = true;
            LTituloRol.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            LTituloRol.Location = new Point(48, 20);
            LTituloRol.Name = "LTituloRol";
            LTituloRol.Size = new Size(100, 23);
            LTituloRol.TabIndex = 0;
            LTituloRol.Text = "GERENCIA";
            // 
            // LUsuarioInfo
            // 
            LUsuarioInfo.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            LUsuarioInfo.Location = new Point(12, 48);
            LUsuarioInfo.Name = "LUsuarioInfo";
            LUsuarioInfo.Size = new Size(175, 40);
            LUsuarioInfo.TabIndex = 1;
            LUsuarioInfo.Text = "Roberto Gerente";
            LUsuarioInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LReportes
            // 
            LReportes.AutoSize = true;
            LReportes.Font = new Font("Arial Black", 10.5F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            LReportes.Location = new Point(25, 115);
            LReportes.Name = "LReportes";
            LReportes.Size = new Size(147, 20);
            LReportes.TabIndex = 2;
            LReportes.Text = "Reportes Globales";
            // 
            // btnReportes
            // 
            btnReportes.BackgroundImage = Properties.Resources.lista_turnos;
            btnReportes.BackgroundImageLayout = ImageLayout.Zoom;
            btnReportes.Cursor = Cursors.Hand;
            btnReportes.Location = new Point(54, 140);
            btnReportes.Name = "btnReportes";
            btnReportes.Size = new Size(92, 73);
            btnReportes.TabIndex = 3;
            btnReportes.UseVisualStyleBackColor = true;
            // 
            // LGuardia
            // 
            LGuardia.AutoSize = true;
            LGuardia.Font = new Font("Arial Black", 10.5F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            LGuardia.Location = new Point(32, 235);
            LGuardia.Name = "LGuardia";
            LGuardia.Size = new Size(135, 20);
            LGuardia.TabIndex = 4;
            LGuardia.Text = "Auditoría Triage";
            // 
            // btnGuardia
            // 
            btnGuardia.BackgroundImage = Properties.Resources.turnos_emergencia;
            btnGuardia.BackgroundImageLayout = ImageLayout.Zoom;
            btnGuardia.Cursor = Cursors.Hand;
            btnGuardia.Location = new Point(54, 260);
            btnGuardia.Name = "btnGuardia";
            btnGuardia.Size = new Size(92, 73);
            btnGuardia.TabIndex = 5;
            btnGuardia.UseVisualStyleBackColor = true;
            // 
            // Lsalir
            // 
            Lsalir.AutoSize = true;
            Lsalir.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            Lsalir.ImageAlign = ContentAlignment.BottomCenter;
            Lsalir.Location = new Point(73, 360);
            Lsalir.Name = "Lsalir";
            Lsalir.Size = new Size(50, 23);
            Lsalir.TabIndex = 6;
            Lsalir.Text = "Salir";
            // 
            // btnSalir
            // 
            btnSalir.BackgroundImage = Properties.Resources.salir;
            btnSalir.BackgroundImageLayout = ImageLayout.Stretch;
            btnSalir.Cursor = Cursors.Hand;
            btnSalir.Location = new Point(54, 385);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(92, 73);
            btnSalir.TabIndex = 7;
            btnSalir.UseVisualStyleBackColor = true;
            // 
            // pnlContenedor
            // 
            pnlContenedor.BackgroundImage = Properties.Resources.fondo_admin;
            pnlContenedor.Dock = DockStyle.Fill;
            pnlContenedor.Location = new Point(200, 0);
            pnlContenedor.Name = "pnlContenedor";
            pnlContenedor.Size = new Size(808, 560);
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
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label LTituloRol;
        private Label LUsuarioInfo;
        private Label LReportes;
        private Button btnReportes;
        private Label LGuardia;
        private Button btnGuardia;
        private Label Lsalir;
        private Button btnSalir;
        private Panel pnlContenedor;
    }
}
