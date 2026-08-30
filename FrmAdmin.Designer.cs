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
            button2 = new Button();
            button1 = new Button();
            BUsuarios = new Button();
            pnlContenedor = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.Controls.Add(button2);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(BUsuarios);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 538);
            panel1.TabIndex = 0;
            // 
            // button2
            // 
            button2.Location = new Point(54, 376);
            button2.Name = "button2";
            button2.Size = new Size(84, 73);
            button2.TabIndex = 2;
            button2.Text = "Salas";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(54, 222);
            button1.Name = "button1";
            button1.Size = new Size(84, 73);
            button1.TabIndex = 1;
            button1.Text = "Recepcionistas";
            button1.UseVisualStyleBackColor = true;
            // 
            // BUsuarios
            // 
            BUsuarios.Location = new Point(54, 47);
            BUsuarios.Name = "BUsuarios";
            BUsuarios.Size = new Size(84, 73);
            BUsuarios.TabIndex = 0;
            BUsuarios.Text = "Personal medico";
            BUsuarios.UseVisualStyleBackColor = true;
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
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button BUsuarios;
        private Button button2;
        private Button button1;
        private Panel pnlContenedor;
    }
}