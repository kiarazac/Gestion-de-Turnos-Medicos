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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRecepcionista));
            panel1 = new Panel();
            asign_turnosEspecialidad = new Button();
            salir = new Button();
            lista_turnos = new Button();
            asign_turnosEmergencia = new Button();
            panelContenedor = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.SteelBlue;
            panel1.Controls.Add(asign_turnosEspecialidad);
            panel1.Controls.Add(salir);
            panel1.Controls.Add(lista_turnos);
            panel1.Controls.Add(asign_turnosEmergencia);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(161, 540);
            panel1.TabIndex = 0;
            // 
            // asign_turnosEspecialidad
            // 
            asign_turnosEspecialidad.BackColor = Color.White;
            asign_turnosEspecialidad.BackgroundImage = Properties.Resources.logo_asignar_turno;
            asign_turnosEspecialidad.BackgroundImageLayout = ImageLayout.Center;
            asign_turnosEspecialidad.Location = new Point(25, 166);
            asign_turnosEspecialidad.Name = "asign_turnosEspecialidad";
            asign_turnosEspecialidad.Size = new Size(102, 102);
            asign_turnosEspecialidad.TabIndex = 3;
            asign_turnosEspecialidad.UseVisualStyleBackColor = false;
            asign_turnosEspecialidad.Click += asign_turnosEspecialidad_Click_1;
            // 
            // salir
            // 
            salir.BackgroundImage = Properties.Resources.salir;
            salir.BackgroundImageLayout = ImageLayout.Stretch;
            salir.Location = new Point(25, 418);
            salir.Name = "salir";
            salir.Size = new Size(102, 91);
            salir.TabIndex = 2;
            salir.UseVisualStyleBackColor = true;
            salir.Click += salir_Click;
            // 
            // lista_turnos
            // 
            lista_turnos.BackColor = Color.DarkCyan;
            lista_turnos.BackgroundImage = Properties.Resources.lista_turnos;
            lista_turnos.BackgroundImageLayout = ImageLayout.Stretch;
            lista_turnos.Location = new Point(25, 292);
            lista_turnos.Name = "lista_turnos";
            lista_turnos.Size = new Size(102, 97);
            lista_turnos.TabIndex = 1;
            lista_turnos.UseVisualStyleBackColor = false;
            lista_turnos.Click += lista_turnos_Click;
            // 
            // asign_turnosEmergencia
            // 
            asign_turnosEmergencia.BackColor = Color.White;
            asign_turnosEmergencia.BackgroundImage = Properties.Resources.turnos_emergencia;
            asign_turnosEmergencia.BackgroundImageLayout = ImageLayout.Stretch;
            asign_turnosEmergencia.Location = new Point(25, 41);
            asign_turnosEmergencia.Name = "asign_turnosEmergencia";
            asign_turnosEmergencia.Size = new Size(102, 100);
            asign_turnosEmergencia.TabIndex = 0;
            asign_turnosEmergencia.UseVisualStyleBackColor = false;
            asign_turnosEmergencia.Click += asign_turnosEmergencia_Click;
            // 
            // panelContenedor
            // 
            panelContenedor.BackgroundImage = Properties.Resources.fondo_recepcionista;
            panelContenedor.BackgroundImageLayout = ImageLayout.Stretch;
            panelContenedor.Dock = DockStyle.Fill;
            panelContenedor.Location = new Point(161, 0);
            panelContenedor.Name = "panelContenedor";
            panelContenedor.Size = new Size(758, 540);
            panelContenedor.TabIndex = 1;
            // 
            // FrmRecepcionista
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(919, 540);
            Controls.Add(panelContenedor);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmRecepcionista";
            Text = "Pantalla Principal |RECEPCIONISTA|";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button lista_turnos;
        private Button asign_turnosEmergencia;
        private Button salir;
        private Panel panelContenedor;
        private Button asign_turnosEspecialidad;
    }
}