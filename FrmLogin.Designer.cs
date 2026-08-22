namespace Gestion_de_Turnos_Medicos
{
    partial class FrmLogin
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogin));
            label1 = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            textContraseña = new TextBox();
            textCorreo = new TextBox();
            textNombre = new TextBox();
            label3 = new Label();
            label2 = new Label();
            LNombre = new Label();
            button1 = new Button();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Showcard Gothic", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.DarkGreen;
            label1.Location = new Point(227, 54);
            label1.Name = "label1";
            label1.Size = new Size(316, 46);
            label1.TabIndex = 0;
            label1.Text = "Inicio de Sesión";
            // 
            // panel1
            // 
            panel1.BackColor = Color.DarkGreen;
            panel1.Location = new Point(108, 157);
            panel1.Name = "panel1";
            panel1.Size = new Size(587, 327);
            panel1.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.Controls.Add(button1);
            panel2.Controls.Add(textContraseña);
            panel2.Controls.Add(textCorreo);
            panel2.Controls.Add(textNombre);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(LNombre);
            panel2.Location = new Point(89, 139);
            panel2.Name = "panel2";
            panel2.Size = new Size(587, 327);
            panel2.TabIndex = 2;
            // 
            // textContraseña
            // 
            textContraseña.Location = new Point(62, 235);
            textContraseña.Name = "textContraseña";
            textContraseña.Size = new Size(411, 23);
            textContraseña.TabIndex = 5;
            textContraseña.UseSystemPasswordChar = true;
            // 
            // textCorreo
            // 
            textCorreo.Location = new Point(62, 156);
            textCorreo.Name = "textCorreo";
            textCorreo.Size = new Size(411, 23);
            textCorreo.TabIndex = 4;
            // 
            // textNombre
            // 
            textNombre.Location = new Point(62, 76);
            textNombre.Name = "textNombre";
            textNombre.Size = new Size(411, 23);
            textNombre.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Cooper Black", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(55, 189);
            label3.Name = "label3";
            label3.Size = new Size(169, 31);
            label3.TabIndex = 2;
            label3.Text = "Contraseña";
            label3.UseWaitCursor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Cooper Black", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(55, 110);
            label2.Name = "label2";
            label2.Size = new Size(107, 31);
            label2.TabIndex = 1;
            label2.Text = "Correo";
            label2.UseWaitCursor = true;
            // 
            // LNombre
            // 
            LNombre.AutoSize = true;
            LNombre.Font = new Font("Cooper Black", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LNombre.Location = new Point(55, 40);
            LNombre.Name = "LNombre";
            LNombre.Size = new Size(123, 31);
            LNombre.TabIndex = 0;
            LNombre.Text = "Nombre";
            LNombre.UseWaitCursor = true;
            // 
            // button1
            // 
            button1.BackColor = Color.LightGreen;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(202, 269);
            button1.Name = "button1";
            button1.Size = new Size(165, 46);
            button1.TabIndex = 6;
            button1.Text = "Iniciar Sesión";
            button1.UseVisualStyleBackColor = false;
            // 
            // FrmLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.fondo_login;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(755, 511);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmLogin";
            Text = "LoginUsuario";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Panel panel1;
        private Panel panel2;
        private Label label3;
        private Label label2;
        private Label LNombre;
        private TextBox textContraseña;
        private TextBox textCorreo;
        private TextBox textNombre;
        private Button button1;
    }
}
