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
            btnSalas = new Button();
            btnRecepcionistas = new Button();
            btnPersonalMedico = new Button();
            pnlContenedor = new Panel();
            LPersonalMedico = new Label();
            btnAdministradores = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnAdministradores);
            panel1.Controls.Add(LPersonalMedico);
            panel1.Controls.Add(btnSalas);
            panel1.Controls.Add(btnRecepcionistas);
            panel1.Controls.Add(btnPersonalMedico);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 538);
            panel1.TabIndex = 0;
            // 
            // btnSalas
            // 
            btnSalas.BackgroundImage = Properties.Resources.Salas;
            btnSalas.Location = new Point(46, 442);
            btnSalas.Name = "btnSalas";
            btnSalas.Size = new Size(92, 73);
            btnSalas.TabIndex = 2;
            btnSalas.UseVisualStyleBackColor = true;
            // 
            // btnRecepcionistas
            // 
            btnRecepcionistas.BackgroundImage = Properties.Resources.Recepcion;
            btnRecepcionistas.Location = new Point(46, 317);
            btnRecepcionistas.Name = "btnRecepcionistas";
            btnRecepcionistas.Size = new Size(92, 73);
            btnRecepcionistas.TabIndex = 1;
            btnRecepcionistas.UseVisualStyleBackColor = true;
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
            pnlContenedor.Size = new Size(695, 538);
            pnlContenedor.TabIndex = 1;
            // 
            // LPersonalMedico
            // 
            LPersonalMedico.AutoSize = true;
            LPersonalMedico.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            LPersonalMedico.Location = new Point(12, 21);
            LPersonalMedico.Name = "LPersonalMedico";
            LPersonalMedico.Size = new Size(157, 23);
            LPersonalMedico.TabIndex = 3;
            LPersonalMedico.Text = "Personal Medico";
            // 
            // btnAdministradores
            // 
            btnAdministradores.BackgroundImage = Properties.Resources.Administrador;
            btnAdministradores.Location = new Point(46, 188);
            btnAdministradores.Name = "btnAdministradores";
            btnAdministradores.Size = new Size(92, 73);
            btnAdministradores.TabIndex = 4;
            btnAdministradores.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 147);
            label1.Name = "label1";
            label1.Size = new Size(155, 23);
            label1.TabIndex = 5;
            label1.Text = "Administradores";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            label2.Location = new Point(22, 278);
            label2.Name = "label2";
            label2.Size = new Size(147, 23);
            label2.TabIndex = 6;
            label2.Text = "Recepcionistas";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            label3.Location = new Point(63, 406);
            label3.Name = "label3";
            label3.Size = new Size(59, 23);
            label3.TabIndex = 7;
            label3.Text = "Salas";
            // 
            // FrmAdmin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.fondo_admin;
            ClientSize = new Size(895, 538);
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
        private Button btnRecepcionistas;
        private Panel pnlContenedor;
        private Label LPersonalMedico;
        private Label label1;
        private Button btnAdministradores;
        private Label label2;
        private Label label3;
    }
}