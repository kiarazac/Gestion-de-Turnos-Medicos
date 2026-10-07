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
            pnlNav = new Panel();
            mis_Salas = new Button();
            lista_turnos_atención = new Button();
            btnMisAtenciones = new Button();
            pnlFooterSidebar = new Panel();
            salir = new Button();
            pnlHeaderSidebar = new Panel();
            pnlSeparador = new Panel();
            lblUsuarioMedico = new Label();
            lblRolTitulo = new Label();
            lblClinica = new Label();
            panel1.SuspendLayout();
            pnlNav.SuspendLayout();
            pnlFooterSidebar.SuspendLayout();
            pnlHeaderSidebar.SuspendLayout();
            SuspendLayout();
            // 
            // panelContenedor
            // 
            panelContenedor.BackgroundImage = Properties.Resources.fondo_personalMedico;
            panelContenedor.BackgroundImageLayout = ImageLayout.Stretch;
            panelContenedor.Dock = DockStyle.Fill;
            panelContenedor.Location = new Point(240, 0);
            panelContenedor.Name = "panelContenedor";
            panelContenedor.Size = new Size(875, 587);
            panelContenedor.TabIndex = 3;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BackColor = Color.FromArgb(19, 78, 74);
            panel1.Controls.Add(pnlNav);
            panel1.Controls.Add(pnlFooterSidebar);
            panel1.Controls.Add(pnlHeaderSidebar);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(240, 587);
            panel1.TabIndex = 2;
            // 
            // pnlNav
            // 
            pnlNav.AutoScroll = true;
            pnlNav.BackColor = Color.Transparent;
            pnlNav.Controls.Add(mis_Salas);
            pnlNav.Controls.Add(lista_turnos_atención);
            pnlNav.Controls.Add(btnMisAtenciones);
            pnlNav.Dock = DockStyle.Fill;
            pnlNav.Location = new Point(0, 105);
            pnlNav.Name = "pnlNav";
            pnlNav.Size = new Size(240, 412);
            pnlNav.TabIndex = 2;
            // 
            // mis_Salas
            // 
            mis_Salas.BackColor = Color.FromArgb(15, 118, 110);
            mis_Salas.Cursor = Cursors.Hand;
            mis_Salas.FlatAppearance.BorderSize = 0;
            mis_Salas.FlatAppearance.MouseDownBackColor = Color.FromArgb(20, 184, 166);
            mis_Salas.FlatAppearance.MouseOverBackColor = Color.FromArgb(13, 148, 136);
            mis_Salas.FlatStyle = FlatStyle.Flat;
            mis_Salas.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            mis_Salas.ForeColor = Color.White;
            mis_Salas.Location = new Point(15, 12);
            mis_Salas.Name = "mis_Salas";
            mis_Salas.Padding = new Padding(12, 0, 0, 0);
            mis_Salas.Size = new Size(210, 46);
            mis_Salas.TabIndex = 0;
            mis_Salas.Text = "🏥  Mis Consultorios";
            mis_Salas.TextAlign = ContentAlignment.MiddleLeft;
            mis_Salas.UseVisualStyleBackColor = false;
            mis_Salas.Click += mis_Salas_Click;
            // 
            // lista_turnos_atención
            // 
            lista_turnos_atención.BackColor = Color.FromArgb(15, 118, 110);
            lista_turnos_atención.Cursor = Cursors.Hand;
            lista_turnos_atención.FlatAppearance.BorderSize = 0;
            lista_turnos_atención.FlatAppearance.MouseDownBackColor = Color.FromArgb(20, 184, 166);
            lista_turnos_atención.FlatAppearance.MouseOverBackColor = Color.FromArgb(13, 148, 136);
            lista_turnos_atención.FlatStyle = FlatStyle.Flat;
            lista_turnos_atención.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            lista_turnos_atención.ForeColor = Color.White;
            lista_turnos_atención.Location = new Point(15, 66);
            lista_turnos_atención.Name = "lista_turnos_atención";
            lista_turnos_atención.Padding = new Padding(12, 0, 0, 0);
            lista_turnos_atención.Size = new Size(210, 46);
            lista_turnos_atención.TabIndex = 1;
            lista_turnos_atención.Text = "🩺  Atención de Turnos";
            lista_turnos_atención.TextAlign = ContentAlignment.MiddleLeft;
            lista_turnos_atención.UseVisualStyleBackColor = false;
            lista_turnos_atención.Click += lista_turnos_atención_Click;
            // 
            // btnMisAtenciones
            // 
            btnMisAtenciones.BackColor = Color.FromArgb(15, 118, 110);
            btnMisAtenciones.Cursor = Cursors.Hand;
            btnMisAtenciones.FlatAppearance.BorderSize = 0;
            btnMisAtenciones.FlatAppearance.MouseDownBackColor = Color.FromArgb(20, 184, 166);
            btnMisAtenciones.FlatAppearance.MouseOverBackColor = Color.FromArgb(13, 148, 136);
            btnMisAtenciones.FlatStyle = FlatStyle.Flat;
            btnMisAtenciones.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnMisAtenciones.ForeColor = Color.White;
            btnMisAtenciones.Location = new Point(15, 120);
            btnMisAtenciones.Name = "btnMisAtenciones";
            btnMisAtenciones.Padding = new Padding(12, 0, 0, 0);
            btnMisAtenciones.Size = new Size(210, 46);
            btnMisAtenciones.TabIndex = 2;
            btnMisAtenciones.Text = "📋  Historial Atenciones";
            btnMisAtenciones.TextAlign = ContentAlignment.MiddleLeft;
            btnMisAtenciones.UseVisualStyleBackColor = false;
            btnMisAtenciones.Click += btnMisAtenciones_Click;
            // 
            // pnlFooterSidebar
            // 
            pnlFooterSidebar.BackColor = Color.FromArgb(10, 46, 44);
            pnlFooterSidebar.Controls.Add(salir);
            pnlFooterSidebar.Dock = DockStyle.Bottom;
            pnlFooterSidebar.Location = new Point(0, 517);
            pnlFooterSidebar.Name = "pnlFooterSidebar";
            pnlFooterSidebar.Size = new Size(240, 70);
            pnlFooterSidebar.TabIndex = 1;
            // 
            // salir
            // 
            salir.BackColor = Color.FromArgb(127, 29, 29);
            salir.Cursor = Cursors.Hand;
            salir.FlatAppearance.BorderSize = 0;
            salir.FlatAppearance.MouseDownBackColor = Color.FromArgb(153, 27, 27);
            salir.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 38, 38);
            salir.FlatStyle = FlatStyle.Flat;
            salir.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            salir.ForeColor = Color.White;
            salir.Location = new Point(15, 12);
            salir.Name = "salir";
            salir.Padding = new Padding(12, 0, 0, 0);
            salir.Size = new Size(210, 46);
            salir.TabIndex = 0;
            salir.Text = "🚪  Cerrar Sesión";
            salir.TextAlign = ContentAlignment.MiddleLeft;
            salir.UseVisualStyleBackColor = false;
            salir.Click += salir_Click;
            // 
            // pnlHeaderSidebar
            // 
            pnlHeaderSidebar.BackColor = Color.FromArgb(10, 46, 44);
            pnlHeaderSidebar.Controls.Add(pnlSeparador);
            pnlHeaderSidebar.Controls.Add(lblUsuarioMedico);
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
            pnlSeparador.BackColor = Color.FromArgb(15, 118, 110);
            pnlSeparador.Location = new Point(15, 96);
            pnlSeparador.Name = "pnlSeparador";
            pnlSeparador.Size = new Size(210, 1);
            pnlSeparador.TabIndex = 3;
            // 
            // lblUsuarioMedico
            // 
            lblUsuarioMedico.AutoEllipsis = true;
            lblUsuarioMedico.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUsuarioMedico.ForeColor = Color.FromArgb(240, 253, 250);
            lblUsuarioMedico.Location = new Point(15, 65);
            lblUsuarioMedico.Name = "lblUsuarioMedico";
            lblUsuarioMedico.Size = new Size(210, 20);
            lblUsuarioMedico.TabIndex = 2;
            lblUsuarioMedico.Text = "Personal Médico";
            // 
            // lblRolTitulo
            // 
            lblRolTitulo.AutoSize = true;
            lblRolTitulo.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRolTitulo.ForeColor = Color.FromArgb(45, 212, 191);
            lblRolTitulo.Location = new Point(15, 38);
            lblRolTitulo.Name = "lblRolTitulo";
            lblRolTitulo.Size = new Size(160, 20);
            lblRolTitulo.TabIndex = 1;
            lblRolTitulo.Text = "🩺 CUERPO MÉDICO";
            // 
            // lblClinica
            // 
            lblClinica.AutoSize = true;
            lblClinica.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblClinica.ForeColor = Color.FromArgb(153, 246, 228);
            lblClinica.Location = new Point(15, 16);
            lblClinica.Name = "lblClinica";
            lblClinica.Size = new Size(100, 13);
            lblClinica.TabIndex = 0;
            lblClinica.Text = "PORTAL CLÍNICO";
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
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            pnlNav.ResumeLayout(false);
            pnlFooterSidebar.ResumeLayout(false);
            pnlHeaderSidebar.ResumeLayout(false);
            pnlHeaderSidebar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelContenedor;
        private Panel panel1;
        private Panel pnlHeaderSidebar;
        private Label lblClinica;
        private Label lblRolTitulo;
        private Label lblUsuarioMedico;
        private Panel pnlSeparador;
        private Panel pnlNav;
        private Button mis_Salas;
        private Button lista_turnos_atención;
        private Button btnMisAtenciones;
        private Panel pnlFooterSidebar;
        private Button salir;
    }
}