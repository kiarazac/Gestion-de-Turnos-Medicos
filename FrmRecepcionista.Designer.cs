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
            pnlNav = new Panel();
            asign_turnosEmergencia = new Button();
            asign_turnosEspecialidad = new Button();
            lista_turnos = new Button();
            btnUsuarioVentana = new Button();
            btnCierreCaja = new Button();
            pnlFooterSidebar = new Panel();
            salir = new Button();
            pnlHeaderSidebar = new Panel();
            pnlSeparador = new Panel();
            lblUsuarioRecep = new Label();
            lblRolTitulo = new Label();
            lblClinica = new Label();
            panelContenedor = new Panel();
            panel1.SuspendLayout();
            pnlNav.SuspendLayout();
            pnlFooterSidebar.SuspendLayout();
            pnlHeaderSidebar.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BackColor = Color.FromArgb(23, 56, 89);
            panel1.Controls.Add(pnlNav);
            panel1.Controls.Add(pnlFooterSidebar);
            panel1.Controls.Add(pnlHeaderSidebar);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(240, 540);
            panel1.TabIndex = 0;
            // 
            // pnlNav
            // 
            pnlNav.AutoScroll = true;
            pnlNav.BackColor = Color.Transparent;
            pnlNav.Controls.Add(asign_turnosEmergencia);
            pnlNav.Controls.Add(asign_turnosEspecialidad);
            pnlNav.Controls.Add(lista_turnos);
            pnlNav.Controls.Add(btnUsuarioVentana);
            pnlNav.Controls.Add(btnCierreCaja);
            pnlNav.Dock = DockStyle.Fill;
            pnlNav.Location = new Point(0, 105);
            pnlNav.Name = "pnlNav";
            pnlNav.Size = new Size(240, 365);
            pnlNav.TabIndex = 2;
            // 
            // asign_turnosEmergencia
            // 
            asign_turnosEmergencia.BackColor = Color.FromArgb(185, 28, 28);
            asign_turnosEmergencia.Cursor = Cursors.Hand;
            asign_turnosEmergencia.FlatAppearance.BorderSize = 0;
            asign_turnosEmergencia.FlatAppearance.MouseDownBackColor = Color.FromArgb(153, 27, 27);
            asign_turnosEmergencia.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 38, 38);
            asign_turnosEmergencia.FlatStyle = FlatStyle.Flat;
            asign_turnosEmergencia.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            asign_turnosEmergencia.ForeColor = Color.White;
            asign_turnosEmergencia.Location = new Point(15, 12);
            asign_turnosEmergencia.Name = "asign_turnosEmergencia";
            asign_turnosEmergencia.Padding = new Padding(12, 0, 0, 0);
            asign_turnosEmergencia.Size = new Size(210, 46);
            asign_turnosEmergencia.TabIndex = 0;
            asign_turnosEmergencia.Text = "🚨  Turno Emergencia";
            asign_turnosEmergencia.TextAlign = ContentAlignment.MiddleLeft;
            asign_turnosEmergencia.UseVisualStyleBackColor = false;
            asign_turnosEmergencia.Click += asign_turnosEmergencia_Click;
            // 
            // asign_turnosEspecialidad
            // 
            asign_turnosEspecialidad.BackColor = Color.FromArgb(30, 78, 124);
            asign_turnosEspecialidad.Cursor = Cursors.Hand;
            asign_turnosEspecialidad.FlatAppearance.BorderSize = 0;
            asign_turnosEspecialidad.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 105, 161);
            asign_turnosEspecialidad.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 132, 199);
            asign_turnosEspecialidad.FlatStyle = FlatStyle.Flat;
            asign_turnosEspecialidad.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            asign_turnosEspecialidad.ForeColor = Color.White;
            asign_turnosEspecialidad.Location = new Point(15, 66);
            asign_turnosEspecialidad.Name = "asign_turnosEspecialidad";
            asign_turnosEspecialidad.Padding = new Padding(12, 0, 0, 0);
            asign_turnosEspecialidad.Size = new Size(210, 46);
            asign_turnosEspecialidad.TabIndex = 1;
            asign_turnosEspecialidad.Text = "🩺  Turno Programado";
            asign_turnosEspecialidad.TextAlign = ContentAlignment.MiddleLeft;
            asign_turnosEspecialidad.UseVisualStyleBackColor = false;
            asign_turnosEspecialidad.Click += asign_turnosEspecialidad_Click_1;
            // 
            // lista_turnos
            // 
            lista_turnos.BackColor = Color.FromArgb(30, 78, 124);
            lista_turnos.Cursor = Cursors.Hand;
            lista_turnos.FlatAppearance.BorderSize = 0;
            lista_turnos.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 105, 161);
            lista_turnos.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 132, 199);
            lista_turnos.FlatStyle = FlatStyle.Flat;
            lista_turnos.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            lista_turnos.ForeColor = Color.White;
            lista_turnos.Location = new Point(15, 120);
            lista_turnos.Name = "lista_turnos";
            lista_turnos.Padding = new Padding(12, 0, 0, 0);
            lista_turnos.Size = new Size(210, 46);
            lista_turnos.TabIndex = 2;
            lista_turnos.Text = "📋  Listado de Turnos";
            lista_turnos.TextAlign = ContentAlignment.MiddleLeft;
            lista_turnos.UseVisualStyleBackColor = false;
            lista_turnos.Click += lista_turnos_Click;
            // 
            // btnUsuarioVentana
            // 
            btnUsuarioVentana.BackColor = Color.FromArgb(30, 78, 124);
            btnUsuarioVentana.Cursor = Cursors.Hand;
            btnUsuarioVentana.FlatAppearance.BorderSize = 0;
            btnUsuarioVentana.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 105, 161);
            btnUsuarioVentana.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 132, 199);
            btnUsuarioVentana.FlatStyle = FlatStyle.Flat;
            btnUsuarioVentana.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnUsuarioVentana.ForeColor = Color.White;
            btnUsuarioVentana.Location = new Point(15, 174);
            btnUsuarioVentana.Name = "btnUsuarioVentana";
            btnUsuarioVentana.Padding = new Padding(12, 0, 0, 0);
            btnUsuarioVentana.Size = new Size(210, 46);
            btnUsuarioVentana.TabIndex = 3;
            btnUsuarioVentana.Text = "📺  Visor Sala Espera";
            btnUsuarioVentana.TextAlign = ContentAlignment.MiddleLeft;
            btnUsuarioVentana.UseVisualStyleBackColor = false;
            btnUsuarioVentana.Click += btnUsuarioVentana_Click;
            // 
            // btnCierreCaja
            // 
            btnCierreCaja.BackColor = Color.FromArgb(15, 118, 110);
            btnCierreCaja.Cursor = Cursors.Hand;
            btnCierreCaja.FlatAppearance.BorderSize = 0;
            btnCierreCaja.FlatAppearance.MouseDownBackColor = Color.FromArgb(15, 118, 110);
            btnCierreCaja.FlatAppearance.MouseOverBackColor = Color.FromArgb(13, 148, 136);
            btnCierreCaja.FlatStyle = FlatStyle.Flat;
            btnCierreCaja.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnCierreCaja.ForeColor = Color.White;
            btnCierreCaja.Location = new Point(15, 228);
            btnCierreCaja.Name = "btnCierreCaja";
            btnCierreCaja.Padding = new Padding(12, 0, 0, 0);
            btnCierreCaja.Size = new Size(210, 46);
            btnCierreCaja.TabIndex = 4;
            btnCierreCaja.Text = "📊  Reportes y Caja";
            btnCierreCaja.TextAlign = ContentAlignment.MiddleLeft;
            btnCierreCaja.UseVisualStyleBackColor = false;
            btnCierreCaja.Click += btnCierreCaja_Click;
            // 
            // pnlFooterSidebar
            // 
            pnlFooterSidebar.BackColor = Color.FromArgb(15, 38, 62);
            pnlFooterSidebar.Controls.Add(salir);
            pnlFooterSidebar.Dock = DockStyle.Bottom;
            pnlFooterSidebar.Location = new Point(0, 470);
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
            pnlHeaderSidebar.BackColor = Color.FromArgb(15, 38, 62);
            pnlHeaderSidebar.Controls.Add(pnlSeparador);
            pnlHeaderSidebar.Controls.Add(lblUsuarioRecep);
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
            pnlSeparador.BackColor = Color.FromArgb(37, 99, 235);
            pnlSeparador.Location = new Point(15, 96);
            pnlSeparador.Name = "pnlSeparador";
            pnlSeparador.Size = new Size(210, 1);
            pnlSeparador.TabIndex = 3;
            // 
            // lblUsuarioRecep
            // 
            lblUsuarioRecep.AutoEllipsis = true;
            lblUsuarioRecep.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUsuarioRecep.ForeColor = Color.FromArgb(240, 249, 255);
            lblUsuarioRecep.Location = new Point(15, 65);
            lblUsuarioRecep.Name = "lblUsuarioRecep";
            lblUsuarioRecep.Size = new Size(210, 20);
            lblUsuarioRecep.TabIndex = 2;
            lblUsuarioRecep.Text = "Recepcionista";
            // 
            // lblRolTitulo
            // 
            lblRolTitulo.AutoSize = true;
            lblRolTitulo.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRolTitulo.ForeColor = Color.FromArgb(56, 189, 248);
            lblRolTitulo.Location = new Point(15, 38);
            lblRolTitulo.Name = "lblRolTitulo";
            lblRolTitulo.Size = new Size(169, 20);
            lblRolTitulo.TabIndex = 1;
            lblRolTitulo.Text = "📋 RECEPCIÓN Y CAJA";
            // 
            // lblClinica
            // 
            lblClinica.AutoSize = true;
            lblClinica.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblClinica.ForeColor = Color.FromArgb(186, 230, 253);
            lblClinica.Location = new Point(15, 16);
            lblClinica.Name = "lblClinica";
            lblClinica.Size = new Size(125, 13);
            lblClinica.TabIndex = 0;
            lblClinica.Text = "GESTIÓN HOSPITALARIA";
            // 
            // panelContenedor
            // 
            panelContenedor.BackgroundImage = Properties.Resources.fondo_recepcionista1;
            panelContenedor.BackgroundImageLayout = ImageLayout.Stretch;
            panelContenedor.Dock = DockStyle.Fill;
            panelContenedor.Location = new Point(240, 0);
            panelContenedor.Name = "panelContenedor";
            panelContenedor.Size = new Size(679, 540);
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
            pnlNav.ResumeLayout(false);
            pnlFooterSidebar.ResumeLayout(false);
            pnlHeaderSidebar.ResumeLayout(false);
            pnlHeaderSidebar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel pnlHeaderSidebar;
        private Label lblClinica;
        private Label lblRolTitulo;
        private Label lblUsuarioRecep;
        private Panel pnlSeparador;
        private Panel pnlNav;
        private Button asign_turnosEmergencia;
        private Button asign_turnosEspecialidad;
        private Button lista_turnos;
        private Button btnUsuarioVentana;
        private Button btnCierreCaja;
        private Panel pnlFooterSidebar;
        private Button salir;
        private Panel panelContenedor;
    }
}