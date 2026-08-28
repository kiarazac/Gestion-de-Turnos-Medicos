namespace Gestion_de_Turnos_Medicos
{
    partial class FrmRecepcionista
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
            salir = new Button();
            lista_turnos = new Button();
            asign_turnos = new Button();
            panelContenedor = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.SteelBlue;
            panel1.Controls.Add(salir);
            panel1.Controls.Add(lista_turnos);
            panel1.Controls.Add(asign_turnos);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(127, 450);
            panel1.TabIndex = 0;
            // 
            // salir
            // 
            salir.BackgroundImage = Properties.Resources.salir;
            salir.BackgroundImageLayout = ImageLayout.Stretch;
            salir.Location = new Point(12, 281);
            salir.Name = "salir";
            salir.Size = new Size(97, 84);
            salir.TabIndex = 2;
            salir.UseVisualStyleBackColor = true;
            salir.Click += salir_Click;
            // 
            // lista_turnos
            // 
            lista_turnos.BackColor = Color.DarkCyan;
            lista_turnos.BackgroundImage = Properties.Resources.lista_turnos;
            lista_turnos.BackgroundImageLayout = ImageLayout.Stretch;
            lista_turnos.Location = new Point(12, 189);
            lista_turnos.Name = "lista_turnos";
            lista_turnos.Size = new Size(97, 86);
            lista_turnos.TabIndex = 1;
            lista_turnos.UseVisualStyleBackColor = false;
            // 
            // asign_turnos
            // 
            asign_turnos.BackColor = Color.White;
            asign_turnos.BackgroundImage = Properties.Resources.logo_asignar_turno;
            asign_turnos.BackgroundImageLayout = ImageLayout.Center;
            asign_turnos.Location = new Point(12, 88);
            asign_turnos.Name = "asign_turnos";
            asign_turnos.Size = new Size(97, 95);
            asign_turnos.TabIndex = 0;
            asign_turnos.UseVisualStyleBackColor = false;
            asign_turnos.Click += asign_turnos_Click;
            // 
            // panelContenedor
            // 
            panelContenedor.BackgroundImage = Properties.Resources.fondo_recepcionista;
            panelContenedor.BackgroundImageLayout = ImageLayout.Stretch;
            panelContenedor.Dock = DockStyle.Fill;
            panelContenedor.Location = new Point(127, 0);
            panelContenedor.Name = "panelContenedor";
            panelContenedor.Size = new Size(673, 450);
            panelContenedor.TabIndex = 1;
            // 
            // FrmRecepcionista
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panelContenedor);
            Controls.Add(panel1);
            Name = "FrmRecepcionista";
            Text = "Pantalla Principal |RECEPCIONISTA|";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button lista_turnos;
        private Button asign_turnos;
        private Button salir;
        private Panel panelContenedor;
    }
}