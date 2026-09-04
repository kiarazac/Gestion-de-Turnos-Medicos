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
            Lsalir = new Label();
            btnSalir = new Button();
            label3 = new Label();
            LPersonalMedico = new Label();
            btnSalas = new Button();
            btnPersonalMedico = new Button();
            pnlContenedor = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.Controls.Add(Lsalir);
            panel1.Controls.Add(btnSalir);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(LPersonalMedico);
            panel1.Controls.Add(btnSalas);
            panel1.Controls.Add(btnPersonalMedico);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 425);
            panel1.TabIndex = 0;
            // 
            // Lsalir
            // 
            Lsalir.AutoSize = true;
            Lsalir.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            Lsalir.ImageAlign = ContentAlignment.BottomCenter;
            Lsalir.Location = new Point(63, 297);
            Lsalir.Name = "Lsalir";
            Lsalir.Size = new Size(50, 23);
            Lsalir.TabIndex = 9;
            Lsalir.Text = "Salir";
            // 
            // btnSalir
            // 
            btnSalir.BackgroundImage = Properties.Resources.salir;
            btnSalir.BackgroundImageLayout = ImageLayout.Stretch;
            btnSalir.Location = new Point(46, 323);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(92, 73);
            btnSalir.TabIndex = 8;
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += btnSalir_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            label3.Location = new Point(63, 156);
            label3.Name = "label3";
            label3.Size = new Size(59, 23);
            label3.TabIndex = 7;
            label3.Text = "Salas";
            // 
            // LPersonalMedico
            // 
            LPersonalMedico.AutoSize = true;
            LPersonalMedico.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            LPersonalMedico.Location = new Point(46, 21);
            LPersonalMedico.Name = "LPersonalMedico";
            LPersonalMedico.Size = new Size(88, 23);
            LPersonalMedico.TabIndex = 3;
            LPersonalMedico.Text = "Usuarios";
            // 
            // btnSalas
            // 
            btnSalas.BackgroundImage = Properties.Resources.Salas;
            btnSalas.Location = new Point(46, 192);
            btnSalas.Name = "btnSalas";
            btnSalas.Size = new Size(92, 73);
            btnSalas.TabIndex = 2;
            btnSalas.UseVisualStyleBackColor = true;
            btnSalas.Click += btnSalas_Click;
            // 
            // btnPersonalMedico
            // 
            btnPersonalMedico.BackgroundImage = Properties.Resources.Personal_Medico;
            btnPersonalMedico.Location = new Point(46, 60);
            btnPersonalMedico.Name = "btnPersonalMedico";
            btnPersonalMedico.Size = new Size(92, 73);
            btnPersonalMedico.TabIndex = 0;
            btnPersonalMedico.UseVisualStyleBackColor = true;
            btnPersonalMedico.Click += btnPersonalMedico_Click;
            // 
            // pnlContenedor
            // 
            pnlContenedor.BackgroundImage = Properties.Resources.fondo_admin;
            pnlContenedor.Dock = DockStyle.Fill;
            pnlContenedor.Location = new Point(200, 0);
            pnlContenedor.Name = "pnlContenedor";
            pnlContenedor.Size = new Size(693, 425);
            pnlContenedor.TabIndex = 1;
            // 
            // FrmAdmin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.fondo_admin;
            ClientSize = new Size(893, 425);
            Controls.Add(pnlContenedor);
            Controls.Add(panel1);
            Name = "FrmAdmin";
            Text = "Administracion";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnPersonalMedico;
        private Button btnSalas;
        private Panel pnlContenedor;
        private Label LPersonalMedico;
        private Label label3;
        private Label Lsalir;
        private Button btnSalir;
    }
}