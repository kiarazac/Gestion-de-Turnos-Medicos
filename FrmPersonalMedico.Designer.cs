namespace Gestion_de_Turnos_Medicos
{
    partial class Pantalla_Principal_PERSONAL_MEDICO
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Pantalla_Principal_PERSONAL_MEDICO));
            panelContenedor = new Panel();
            panel1 = new Panel();
            mis_Salas = new Button();
            salir = new Button();
            lista_turnos_atención = new Button();
            LPersonalMedico = new Label();
            label1 = new Label();
            label2 = new Label();
            btnMisAtenciones = new Button();
            lblMisAtenciones = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panelContenedor
            // 
            panelContenedor.BackgroundImage = Properties.Resources.fondo_personalMedico;
            panelContenedor.BackgroundImageLayout = ImageLayout.Stretch;
            panelContenedor.Dock = DockStyle.Fill;
            panelContenedor.Location = new Point(161, 0);
            panelContenedor.Name = "panelContenedor";
            panelContenedor.Size = new Size(954, 587);
            panelContenedor.TabIndex = 3;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BackColor = Color.LightSkyBlue;
            panel1.Controls.Add(label2);
            panel1.Controls.Add(salir);
            panel1.Controls.Add(lblMisAtenciones);
            panel1.Controls.Add(btnMisAtenciones);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(lista_turnos_atención);
            panel1.Controls.Add(LPersonalMedico);
            panel1.Controls.Add(mis_Salas);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(161, 587);
            panel1.TabIndex = 2;
            // 
            // mis_Salas
            // 
            mis_Salas.BackColor = Color.White;
            mis_Salas.BackgroundImage = Properties.Resources.misSalas;
            mis_Salas.BackgroundImageLayout = ImageLayout.Center;
            mis_Salas.Location = new Point(28, 48);
            mis_Salas.Name = "mis_Salas";
            mis_Salas.Size = new Size(102, 85);
            mis_Salas.TabIndex = 3;
            mis_Salas.UseVisualStyleBackColor = false;
            mis_Salas.Click += mis_Salas_Click;
            // 
            // salir
            // 
            salir.BackgroundImage = Properties.Resources.salir;
            salir.BackgroundImageLayout = ImageLayout.Stretch;
            salir.Location = new Point(28, 423);
            salir.Name = "salir";
            salir.Size = new Size(102, 85);
            salir.TabIndex = 2;
            salir.UseVisualStyleBackColor = true;
            salir.Click += salir_Click;
            // 
            // lista_turnos_atención
            // 
            lista_turnos_atención.BackColor = Color.DarkCyan;
            lista_turnos_atención.BackgroundImage = Properties.Resources.lista_turnos;
            lista_turnos_atención.BackgroundImageLayout = ImageLayout.Stretch;
            lista_turnos_atención.Location = new Point(28, 173);
            lista_turnos_atención.Name = "lista_turnos_atención";
            lista_turnos_atención.Size = new Size(102, 85);
            lista_turnos_atención.TabIndex = 1;
            lista_turnos_atención.UseVisualStyleBackColor = false;
            lista_turnos_atención.Click += lista_turnos_atención_Click;
            // 
            // btnMisAtenciones
            // 
            btnMisAtenciones.BackColor = Color.FromArgb(15, 118, 110);
            btnMisAtenciones.FlatStyle = FlatStyle.Flat;
            btnMisAtenciones.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnMisAtenciones.ForeColor = Color.White;
            btnMisAtenciones.Location = new Point(28, 298);
            btnMisAtenciones.Name = "btnMisAtenciones";
            btnMisAtenciones.Size = new Size(102, 85);
            btnMisAtenciones.TabIndex = 7;
            btnMisAtenciones.Text = "📋 Mis\nAtenciones";
            btnMisAtenciones.UseVisualStyleBackColor = false;
            btnMisAtenciones.Click += btnMisAtenciones_Click;
            // 
            // lblMisAtenciones
            // 
            lblMisAtenciones.Font = new Font("Arial Black", 11F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            lblMisAtenciones.Location = new Point(10, 270);
            lblMisAtenciones.Name = "lblMisAtenciones";
            lblMisAtenciones.Size = new Size(140, 23);
            lblMisAtenciones.TabIndex = 8;
            lblMisAtenciones.Text = "Atenciones";
            lblMisAtenciones.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LPersonalMedico
            // 
            LPersonalMedico.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            LPersonalMedico.Location = new Point(20, 20);
            LPersonalMedico.Name = "LPersonalMedico";
            LPersonalMedico.Size = new Size(120, 23);
            LPersonalMedico.TabIndex = 4;
            LPersonalMedico.Text = "Mis Salas";
            LPersonalMedico.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            label1.Location = new Point(20, 145);
            label1.Name = "label1";
            label1.Size = new Size(120, 23);
            label1.TabIndex = 5;
            label1.Text = "Turnos";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Font = new Font("Arial Black", 12F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            label2.Location = new Point(20, 395);
            label2.Name = "label2";
            label2.Size = new Size(120, 23);
            label2.TabIndex = 6;
            label2.Text = "Salir";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Pantalla_Principal_PERSONAL_MEDICO
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1115, 587);
            Controls.Add(panelContenedor);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Pantalla_Principal_PERSONAL_MEDICO";
            Text = "Pantalla Principal |PERSONAL MÉDICO|";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelContenedor;
        private Panel panel1;
        private Button mis_Salas;
        private Button salir;
        private Button lista_turnos_atención;
        private Button btnMisAtenciones;
        private Label lblMisAtenciones;
        private Label label2;
        private Label label1;
        private Label LPersonalMedico;
    }
}