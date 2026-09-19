namespace Gestion_de_Turnos_Medicos
{
    partial class FrmSalasAdmin
    {
        /// <summary>
        /// Variable necesaria para el diseñador.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblSubtituloHeader = new Label();
            lblTituloHeader = new Label();
            pnlContenedorPrincipal = new Panel();
            pnlCardGrilla = new Panel();
            chkMostrarInactivas = new CheckBox();
            lblTituloGrilla = new Label();
            dgvSalas = new DataGridView();
            pnlCardDatos = new Panel();
            pnlAcciones = new Panel();
            btnLimpiar = new Button();
            btnReactivar = new Button();
            btnEliminar = new Button();
            btnModificar = new Button();
            btnGuardar = new Button();
            clbPersonal = new CheckedListBox();
            lblPersonal = new Label();
            cmbEstadoSala = new ComboBox();
            lblEstadoSala = new Label();
            txtNombreSala = new TextBox();
            lblNombreSala = new Label();
            lblTituloDatos = new Label();
            pnlHeader.SuspendLayout();
            pnlContenedorPrincipal.SuspendLayout();
            pnlCardGrilla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSalas).BeginInit();
            pnlCardDatos.SuspendLayout();
            pnlAcciones.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(15, 118, 110);
            pnlHeader.Controls.Add(lblSubtituloHeader);
            pnlHeader.Controls.Add(lblTituloHeader);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1133, 60);
            pnlHeader.TabIndex = 0;
            // 
            // lblSubtituloHeader
            // 
            lblSubtituloHeader.AutoSize = true;
            lblSubtituloHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSubtituloHeader.ForeColor = Color.FromArgb(204, 251, 241);
            lblSubtituloHeader.Location = new Point(18, 34);
            lblSubtituloHeader.Name = "lblSubtituloHeader";
            lblSubtituloHeader.Size = new Size(330, 15);
            lblSubtituloHeader.TabIndex = 1;
            lblSubtituloHeader.Text = "Administración de consultorios y asignación de profesionales";
            // 
            // lblTituloHeader
            // 
            lblTituloHeader.AutoSize = true;
            lblTituloHeader.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloHeader.ForeColor = Color.White;
            lblTituloHeader.Location = new Point(16, 8);
            lblTituloHeader.Name = "lblTituloHeader";
            lblTituloHeader.Size = new Size(289, 25);
            lblTituloHeader.TabIndex = 0;
            lblTituloHeader.Text = "Gestión de Consultorios y Salas";
            // 
            // pnlContenedorPrincipal
            // 
            pnlContenedorPrincipal.AutoScroll = true;
            pnlContenedorPrincipal.BackColor = Color.FromArgb(241, 245, 249);
            pnlContenedorPrincipal.Controls.Add(pnlCardGrilla);
            pnlContenedorPrincipal.Controls.Add(pnlCardDatos);
            pnlContenedorPrincipal.Dock = DockStyle.Fill;
            pnlContenedorPrincipal.Location = new Point(0, 60);
            pnlContenedorPrincipal.Name = "pnlContenedorPrincipal";
            pnlContenedorPrincipal.Padding = new Padding(16);
            pnlContenedorPrincipal.Size = new Size(1133, 441);
            pnlContenedorPrincipal.TabIndex = 1;
            // 
            // pnlCardGrilla
            // 
            pnlCardGrilla.BackColor = Color.White;
            pnlCardGrilla.BorderStyle = BorderStyle.FixedSingle;
            pnlCardGrilla.Controls.Add(chkMostrarInactivas);
            pnlCardGrilla.Controls.Add(lblTituloGrilla);
            pnlCardGrilla.Controls.Add(dgvSalas);
            pnlCardGrilla.Dock = DockStyle.Fill;
            pnlCardGrilla.Location = new Point(16, 226);
            pnlCardGrilla.Name = "pnlCardGrilla";
            pnlCardGrilla.Padding = new Padding(12);
            pnlCardGrilla.Size = new Size(1101, 199);
            pnlCardGrilla.TabIndex = 1;
            // 
            // chkMostrarInactivas
            // 
            chkMostrarInactivas.AutoSize = true;
            chkMostrarInactivas.Cursor = Cursors.Hand;
            chkMostrarInactivas.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkMostrarInactivas.ForeColor = Color.FromArgb(71, 85, 105);
            chkMostrarInactivas.Location = new Point(310, 9);
            chkMostrarInactivas.Name = "chkMostrarInactivas";
            chkMostrarInactivas.Size = new Size(149, 19);
            chkMostrarInactivas.TabIndex = 2;
            chkMostrarInactivas.Text = "Mostrar salas inactivas";
            chkMostrarInactivas.UseVisualStyleBackColor = true;
            // 
            // lblTituloGrilla
            // 
            lblTituloGrilla.AutoSize = true;
            lblTituloGrilla.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloGrilla.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloGrilla.Location = new Point(12, 8);
            lblTituloGrilla.Name = "lblTituloGrilla";
            lblTituloGrilla.Size = new Size(285, 20);
            lblTituloGrilla.TabIndex = 0;
            lblTituloGrilla.Text = "📋 Salas Registradas en el Sistema (BD)";
            // 
            // dgvSalas
            // 
            dgvSalas.AllowUserToAddRows = false;
            dgvSalas.AllowUserToDeleteRows = false;
            dgvSalas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvSalas.BackgroundColor = Color.White;
            dgvSalas.BorderStyle = BorderStyle.None;
            dgvSalas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSalas.Location = new Point(12, 34);
            dgvSalas.MultiSelect = false;
            dgvSalas.Name = "dgvSalas";
            dgvSalas.ReadOnly = true;
            dgvSalas.RowHeadersVisible = false;
            dgvSalas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSalas.Size = new Size(1075, 151);
            dgvSalas.TabIndex = 1;
            // 
            // pnlCardDatos
            // 
            pnlCardDatos.BackColor = Color.White;
            pnlCardDatos.BorderStyle = BorderStyle.FixedSingle;
            pnlCardDatos.Controls.Add(pnlAcciones);
            pnlCardDatos.Controls.Add(clbPersonal);
            pnlCardDatos.Controls.Add(lblPersonal);
            pnlCardDatos.Controls.Add(cmbEstadoSala);
            pnlCardDatos.Controls.Add(lblEstadoSala);
            pnlCardDatos.Controls.Add(txtNombreSala);
            pnlCardDatos.Controls.Add(lblNombreSala);
            pnlCardDatos.Controls.Add(lblTituloDatos);
            pnlCardDatos.Dock = DockStyle.Top;
            pnlCardDatos.Location = new Point(16, 16);
            pnlCardDatos.Name = "pnlCardDatos";
            pnlCardDatos.Padding = new Padding(12);
            pnlCardDatos.Size = new Size(1101, 210);
            pnlCardDatos.TabIndex = 0;
            // 
            // pnlAcciones
            // 
            pnlAcciones.BackColor = Color.FromArgb(248, 250, 252);
            pnlAcciones.BorderStyle = BorderStyle.FixedSingle;
            pnlAcciones.Controls.Add(btnLimpiar);
            pnlAcciones.Controls.Add(btnReactivar);
            pnlAcciones.Controls.Add(btnEliminar);
            pnlAcciones.Controls.Add(btnModificar);
            pnlAcciones.Controls.Add(btnGuardar);
            pnlAcciones.Dock = DockStyle.Bottom;
            pnlAcciones.Location = new Point(12, 146);
            pnlAcciones.Name = "pnlAcciones";
            pnlAcciones.Size = new Size(1075, 50);
            pnlAcciones.TabIndex = 7;
            // 
            // btnLimpiar
            // 
            btnLimpiar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLimpiar.BackColor = Color.FromArgb(100, 116, 139);
            btnLimpiar.Cursor = Cursors.Hand;
            btnLimpiar.FlatAppearance.BorderSize = 0;
            btnLimpiar.FlatStyle = FlatStyle.Flat;
            btnLimpiar.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnLimpiar.ForeColor = Color.White;
            btnLimpiar.Location = new Point(910, 8);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(150, 32);
            btnLimpiar.TabIndex = 3;
            btnLimpiar.Text = "🔄 Limpiar Campos";
            btnLimpiar.UseVisualStyleBackColor = false;
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // btnReactivar
            // 
            btnReactivar.BackColor = Color.FromArgb(13, 148, 136);
            btnReactivar.Cursor = Cursors.Hand;
            btnReactivar.FlatAppearance.BorderSize = 0;
            btnReactivar.FlatStyle = FlatStyle.Flat;
            btnReactivar.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnReactivar.ForeColor = Color.White;
            btnReactivar.Location = new Point(535, 8);
            btnReactivar.Name = "btnReactivar";
            btnReactivar.Size = new Size(175, 32);
            btnReactivar.TabIndex = 4;
            btnReactivar.Text = "♻ Reactivar";
            btnReactivar.UseVisualStyleBackColor = false;
            btnReactivar.Click += btnReactivar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = Color.FromArgb(220, 38, 38);
            btnEliminar.Cursor = Cursors.Hand;
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnEliminar.ForeColor = Color.White;
            btnEliminar.Location = new Point(360, 8);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(160, 32);
            btnEliminar.TabIndex = 2;
            btnEliminar.Text = "🗑 Desactivar Sala";
            btnEliminar.UseVisualStyleBackColor = false;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnModificar
            // 
            btnModificar.BackColor = Color.FromArgb(37, 99, 235);
            btnModificar.Cursor = Cursors.Hand;
            btnModificar.FlatAppearance.BorderSize = 0;
            btnModificar.FlatStyle = FlatStyle.Flat;
            btnModificar.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnModificar.ForeColor = Color.White;
            btnModificar.Location = new Point(185, 8);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(160, 32);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "💾 Modificar Sala";
            btnModificar.UseVisualStyleBackColor = false;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.FromArgb(5, 150, 105);
            btnGuardar.Cursor = Cursors.Hand;
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(15, 8);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(155, 32);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "➕ Guardar Sala";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // clbPersonal
            // 
            clbPersonal.CheckOnClick = true;
            clbPersonal.Font = new Font("Segoe UI", 9F);
            clbPersonal.FormattingEnabled = true;
            clbPersonal.Location = new Point(460, 48);
            clbPersonal.Name = "clbPersonal";
            clbPersonal.Size = new Size(360, 76);
            clbPersonal.TabIndex = 6;
            // 
            // lblPersonal
            // 
            lblPersonal.AutoSize = true;
            lblPersonal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPersonal.ForeColor = Color.FromArgb(15, 118, 110);
            lblPersonal.Location = new Point(460, 30);
            lblPersonal.Name = "lblPersonal";
            lblPersonal.Size = new Size(168, 15);
            lblPersonal.TabIndex = 5;
            lblPersonal.Text = "👨‍⚕️ Personal Médico Asignado:";
            // 
            // cmbEstadoSala
            // 
            cmbEstadoSala.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstadoSala.Font = new Font("Segoe UI", 9F);
            cmbEstadoSala.FormattingEnabled = true;
            cmbEstadoSala.Location = new Point(16, 105);
            cmbEstadoSala.Name = "cmbEstadoSala";
            cmbEstadoSala.Size = new Size(380, 23);
            cmbEstadoSala.TabIndex = 4;
            // 
            // lblEstadoSala
            // 
            lblEstadoSala.AutoSize = true;
            lblEstadoSala.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEstadoSala.ForeColor = Color.FromArgb(51, 65, 85);
            lblEstadoSala.Location = new Point(16, 87);
            lblEstadoSala.Name = "lblEstadoSala";
            lblEstadoSala.Size = new Size(143, 15);
            lblEstadoSala.TabIndex = 3;
            lblEstadoSala.Text = "Estado de Disponibilidad:";
            // 
            // txtNombreSala
            // 
            txtNombreSala.Font = new Font("Segoe UI", 9F);
            txtNombreSala.Location = new Point(16, 52);
            txtNombreSala.Name = "txtNombreSala";
            txtNombreSala.PlaceholderText = "Ej: Consultorio 1, Sala de Rayos...";
            txtNombreSala.Size = new Size(380, 23);
            txtNombreSala.TabIndex = 2;
            // 
            // lblNombreSala
            // 
            lblNombreSala.AutoSize = true;
            lblNombreSala.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNombreSala.ForeColor = Color.FromArgb(51, 65, 85);
            lblNombreSala.Location = new Point(16, 34);
            lblNombreSala.Name = "lblNombreSala";
            lblNombreSala.Size = new Size(184, 15);
            lblNombreSala.TabIndex = 1;
            lblNombreSala.Text = "Nombre de la Sala / Consultorio:";
            // 
            // lblTituloDatos
            // 
            lblTituloDatos.AutoSize = true;
            lblTituloDatos.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblTituloDatos.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloDatos.Location = new Point(12, 10);
            lblTituloDatos.Name = "lblTituloDatos";
            lblTituloDatos.Size = new Size(275, 19);
            lblTituloDatos.TabIndex = 0;
            lblTituloDatos.Text = "🏢 Configuración del Consultorio / Sala";
            // 
            // FrmSalasAdmin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1133, 501);
            Controls.Add(pnlContenedorPrincipal);
            Controls.Add(pnlHeader);
            Name = "FrmSalasAdmin";
            Text = "Gestión de Consultorios y Salas";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContenedorPrincipal.ResumeLayout(false);
            pnlCardGrilla.ResumeLayout(false);
            pnlCardGrilla.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSalas).EndInit();
            pnlCardDatos.ResumeLayout(false);
            pnlCardDatos.PerformLayout();
            pnlAcciones.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblSubtituloHeader;
        private Label lblTituloHeader;
        private Panel pnlContenedorPrincipal;
        private Panel pnlCardDatos;
        private Label lblTituloDatos;
        private Label lblNombreSala;
        private TextBox txtNombreSala;
        private Label lblEstadoSala;
        private ComboBox cmbEstadoSala;
        private Label lblPersonal;
        private CheckedListBox clbPersonal;
        private Panel pnlAcciones;
        private Button btnGuardar;
        private Button btnModificar;
        private Button btnEliminar;
        private Button btnReactivar;
        private Button btnLimpiar;
        private Panel pnlCardGrilla;
        private Label lblTituloGrilla;
        private CheckBox chkMostrarInactivas;
        private DataGridView dgvSalas;
    }
}