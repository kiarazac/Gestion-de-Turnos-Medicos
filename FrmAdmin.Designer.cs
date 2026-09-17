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
            LEspecialidades = new Label();
            btnEspecialidades = new Button();
            label3 = new Label();
            LPersonalMedico = new Label();
            btnSalas = new Button();
            btnPersonalMedico = new Button();
            LUsuarios2 = new Label();
            btnUsuarios2 = new Button();
            pnlContenedor = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.Controls.Add(LUsuarios2);
            panel1.Controls.Add(btnUsuarios2);
            panel1.Controls.Add(Lsalir);
            panel1.Controls.Add(btnSalir);
            panel1.Controls.Add(LEspecialidades);
            panel1.Controls.Add(btnEspecialidades);
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
            // LUsuarios2
            // 
            LUsuarios2.AutoSize = true;
            LUsuarios2.Font = new Font("Arial Black", 10F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            LUsuarios2.ForeColor = Color.DarkGreen;
            LUsuarios2.Location = new Point(25, 345);
            LUsuarios2.Name = "LUsuarios2";
            LUsuarios2.Size = new Size(150, 19);
            LUsuarios2.TabIndex = 8;
            LUsuarios2.Text = "Usuarios 2.0 (Prueba)";
            LUsuarios2.Visible = false;
            // 
            // btnUsuarios2
            // 
            btnUsuarios2.BackColor = Color.FromArgb(15, 118, 110);
            btnUsuarios2.Cursor = Cursors.Hand;
            btnUsuarios2.FlatStyle = FlatStyle.Flat;
            btnUsuarios2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnUsuarios2.ForeColor = Color.White;
            btnUsuarios2.Location = new Point(46, 368);
            btnUsuarios2.Name = "btnUsuarios2";
            btnUsuarios2.Size = new Size(92, 73);
            btnUsuarios2.TabIndex = 9;
            btnUsuarios2.Text = "⚡ V 2.0\n(Prueba)";
            btnUsuarios2.UseVisualStyleBackColor = false;
            btnUsuarios2.Visible = false;
            btnUsuarios2.Click += btnUsuarios2_Click;
            // 
            // Lsalir
            // 
            Lsalir.AutoSize = true;
            Lsalir.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            Lsalir.ImageAlign = ContentAlignment.BottomCenter;
            Lsalir.Location = new Point(65, 345);
            Lsalir.Name = "Lsalir";
            Lsalir.Size = new Size(50, 23);
            Lsalir.TabIndex = 7;
            Lsalir.Text = "Salir";
            // 
            // btnSalir
            // 
            btnSalir.BackgroundImage = Properties.Resources.salir;
            btnSalir.BackgroundImageLayout = ImageLayout.Stretch;
            btnSalir.Location = new Point(46, 370);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(92, 73);
            btnSalir.TabIndex = 6;
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += btnSalir_Click;
            // 
            // LEspecialidades
            // 
            LEspecialidades.AutoSize = true;
            LEspecialidades.Font = new Font("Arial Black", 10.5F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            LEspecialidades.Location = new Point(30, 235);
            LEspecialidades.Name = "LEspecialidades";
            LEspecialidades.Size = new Size(125, 21);
            LEspecialidades.TabIndex = 5;
            LEspecialidades.Text = "Especialidades";
            // 
            // btnEspecialidades
            // 
            btnEspecialidades.BackgroundImage = Properties.Resources.logo_asignar_turno;
            btnEspecialidades.BackgroundImageLayout = ImageLayout.Zoom;
            btnEspecialidades.Location = new Point(46, 262);
            btnEspecialidades.Name = "btnEspecialidades";
            btnEspecialidades.Size = new Size(92, 73);
            btnEspecialidades.TabIndex = 4;
            btnEspecialidades.UseVisualStyleBackColor = true;
            btnEspecialidades.Click += btnEspecialidades_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            label3.Location = new Point(60, 125);
            label3.Name = "label3";
            label3.Size = new Size(59, 23);
            label3.TabIndex = 3;
            label3.Text = "Salas";
            // 
            // LPersonalMedico
            // 
            LPersonalMedico.AutoSize = true;
            LPersonalMedico.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            LPersonalMedico.Location = new Point(46, 15);
            LPersonalMedico.Name = "LPersonalMedico";
            LPersonalMedico.Size = new Size(88, 23);
            LPersonalMedico.TabIndex = 1;
            LPersonalMedico.Text = "Usuarios";
            // 
            // btnSalas
            // 
            btnSalas.BackgroundImage = Properties.Resources.Salas;
            btnSalas.Location = new Point(46, 152);
            btnSalas.Name = "btnSalas";
            btnSalas.Size = new Size(92, 73);
            btnSalas.TabIndex = 2;
            btnSalas.UseVisualStyleBackColor = true;
            btnSalas.Click += btnSalas_Click;
            // 
            // btnPersonalMedico
            // 
            btnPersonalMedico.BackgroundImage = Properties.Resources.Personal_Medico;
            btnPersonalMedico.Location = new Point(46, 42);
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
        private Label LEspecialidades;
        private Button btnEspecialidades;
        private Panel pnlContenedor;
        private Label LPersonalMedico;
        private Label label3;
        private Label Lsalir;
        private Button btnSalir;
        private Label LUsuarios2;
        private Button btnUsuarios2;
    }
}