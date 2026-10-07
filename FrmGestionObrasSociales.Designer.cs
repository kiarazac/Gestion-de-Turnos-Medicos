namespace Gestion_de_Turnos_Medicos
{
    partial class FrmGestionObrasSociales
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
            dgvObrasSociales = new DataGridView();
            pnlCardDatos = new Panel();
            pnlAcciones = new Panel();
            btnLimpiar = new Button();
            btnReactivar = new Button();
            btnDesactivar = new Button();
            btnModificar = new Button();
            btnGuardar = new Button();
            lblInfo = new Label();
            txtSigla = new TextBox();
            lblSigla = new Label();
            txtNombre = new TextBox();
            lblNombre = new Label();
            lblTituloDatos = new Label();
            pnlHeader.SuspendLayout();
            pnlContenedorPrincipal.SuspendLayout();
            pnlCardGrilla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvObrasSociales).BeginInit();
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
            lblSubtituloHeader.Size = new Size(378, 15);
            lblSubtituloHeader.TabIndex = 1;
            lblSubtituloHeader.Text = "Catálogo oficial de obras sociales y medicina prepaga del centro médico";
            // 
            // lblTituloHeader
            // 
            lblTituloHeader.AutoSize = true;
            lblTituloHeader.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloHeader.ForeColor = Color.White;
            lblTituloHeader.Location = new Point(16, 8);
            lblTituloHeader.Name = "lblTituloHeader";
            lblTituloHeader.Size = new Size(390, 25);
            lblTituloHeader.TabIndex = 0;
            lblTituloHeader.Text = "Gestión de Obras Sociales y Prepagas";
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
            pnlContenedorPrincipal.Size = new Size(1133, 490);
            pnlContenedorPrincipal.TabIndex = 1;
            // 
            // pnlCardGrilla
            // 
            pnlCardGrilla.BackColor = Color.White;
            pnlCardGrilla.BorderStyle = BorderStyle.FixedSingle;
            pnlCardGrilla.Controls.Add(chkMostrarInactivas);
            pnlCardGrilla.Controls.Add(lblTituloGrilla);
            pnlCardGrilla.Controls.Add(dgvObrasSociales);
            pnlCardGrilla.Dock = DockStyle.Fill;
            pnlCardGrilla.Location = new Point(16, 216);
            pnlCardGrilla.Name = "pnlCardGrilla";
            pnlCardGrilla.Padding = new Padding(12);
            pnlCardGrilla.Size = new Size(1101, 258);
            pnlCardGrilla.TabIndex = 1;
            // 
            // chkMostrarInactivas
            // 
            chkMostrarInactivas.AutoSize = true;
            chkMostrarInactivas.Cursor = Cursors.Hand;
            chkMostrarInactivas.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkMostrarInactivas.ForeColor = Color.FromArgb(71, 85, 105);
            chkMostrarInactivas.Location = new Point(390, 10);
            chkMostrarInactivas.Name = "chkMostrarInactivas";
            chkMostrarInactivas.Size = new Size(195, 19);
            chkMostrarInactivas.TabIndex = 2;
            chkMostrarInactivas.Text = "Mostrar coberturas inactivas";
            chkMostrarInactivas.UseVisualStyleBackColor = true;
            chkMostrarInactivas.CheckedChanged += chkMostrarInactivas_CheckedChanged;
            // 
            // lblTituloGrilla
            // 
            lblTituloGrilla.AutoSize = true;
            lblTituloGrilla.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloGrilla.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloGrilla.Location = new Point(12, 8);
            lblTituloGrilla.Name = "lblTituloGrilla";
            lblTituloGrilla.Size = new Size(355, 20);
            lblTituloGrilla.TabIndex = 0;
            lblTituloGrilla.Text = "📋 Obras Sociales Registradas en el Sistema (BD)";
            // 
            // dgvObrasSociales
            // 
            dgvObrasSociales.AllowUserToAddRows = false;
            dgvObrasSociales.AllowUserToDeleteRows = false;
            dgvObrasSociales.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvObrasSociales.BackgroundColor = Color.White;
            dgvObrasSociales.BorderStyle = BorderStyle.None;
            dgvObrasSociales.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvObrasSociales.Location = new Point(12, 34);
            dgvObrasSociales.MultiSelect = false;
            dgvObrasSociales.Name = "dgvObrasSociales";
            dgvObrasSociales.ReadOnly = true;
            dgvObrasSociales.RowHeadersVisible = false;
            dgvObrasSociales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvObrasSociales.Size = new Size(1075, 210);
            dgvObrasSociales.TabIndex = 1;
            // 
            // pnlCardDatos
            // 
            pnlCardDatos.BackColor = Color.White;
            pnlCardDatos.BorderStyle = BorderStyle.FixedSingle;
            pnlCardDatos.Controls.Add(pnlAcciones);
            pnlCardDatos.Controls.Add(lblInfo);
            pnlCardDatos.Controls.Add(txtSigla);
            pnlCardDatos.Controls.Add(lblSigla);
            pnlCardDatos.Controls.Add(txtNombre);
            pnlCardDatos.Controls.Add(lblNombre);
            pnlCardDatos.Controls.Add(lblTituloDatos);
            pnlCardDatos.Dock = DockStyle.Top;
            pnlCardDatos.Location = new Point(16, 16);
            pnlCardDatos.Name = "pnlCardDatos";
            pnlCardDatos.Padding = new Padding(12);
            pnlCardDatos.Size = new Size(1101, 200);
            pnlCardDatos.TabIndex = 0;
            // 
            // pnlAcciones
            // 
            pnlAcciones.BackColor = Color.FromArgb(248, 250, 252);
            pnlAcciones.BorderStyle = BorderStyle.FixedSingle;
            pnlAcciones.Controls.Add(btnLimpiar);
            pnlAcciones.Controls.Add(btnReactivar);
            pnlAcciones.Controls.Add(btnDesactivar);
            pnlAcciones.Controls.Add(btnModificar);
            pnlAcciones.Controls.Add(btnGuardar);
            pnlAcciones.Dock = DockStyle.Bottom;
            pnlAcciones.Location = new Point(12, 136);
            pnlAcciones.Name = "pnlAcciones";
            pnlAcciones.Size = new Size(1075, 50);
            pnlAcciones.TabIndex = 6;
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
            btnLimpiar.Location = new Point(930, 8);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(130, 32);
            btnLimpiar.TabIndex = 4;
            btnLimpiar.Text = "🔄 Limpiar";
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
            btnReactivar.Location = new Point(601, 8);
            btnReactivar.Name = "btnReactivar";
            btnReactivar.Size = new Size(185, 32);
            btnReactivar.TabIndex = 3;
            btnReactivar.Text = "♻ Reactivar";
            btnReactivar.UseVisualStyleBackColor = false;
            btnReactivar.Click += btnReactivar_Click;
            // 
            // btnDesactivar
            // 
            btnDesactivar.BackColor = Color.FromArgb(220, 38, 38);
            btnDesactivar.Cursor = Cursors.Hand;
            btnDesactivar.FlatAppearance.BorderSize = 0;
            btnDesactivar.FlatStyle = FlatStyle.Flat;
            btnDesactivar.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnDesactivar.ForeColor = Color.White;
            btnDesactivar.Location = new Point(402, 8);
            btnDesactivar.Name = "btnDesactivar";
            btnDesactivar.Size = new Size(190, 32);
            btnDesactivar.TabIndex = 2;
            btnDesactivar.Text = "🗑 Desactivar Cobertura";
            btnDesactivar.UseVisualStyleBackColor = false;
            btnDesactivar.Click += btnDesactivar_Click;
            // 
            // btnModificar
            // 
            btnModificar.BackColor = Color.FromArgb(37, 99, 235);
            btnModificar.Cursor = Cursors.Hand;
            btnModificar.FlatAppearance.BorderSize = 0;
            btnModificar.FlatStyle = FlatStyle.Flat;
            btnModificar.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnModificar.ForeColor = Color.White;
            btnModificar.Location = new Point(208, 8);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(185, 32);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "💾 Modificar Cobertura";
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
            btnGuardar.Location = new Point(14, 8);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(185, 32);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "➕ Guardar Cobertura";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // lblInfo
            // 
            lblInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblInfo.BackColor = Color.FromArgb(240, 253, 250);
            lblInfo.BorderStyle = BorderStyle.FixedSingle;
            lblInfo.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblInfo.ForeColor = Color.FromArgb(15, 118, 110);
            lblInfo.Location = new Point(560, 36);
            lblInfo.Name = "lblInfo";
            lblInfo.Padding = new Padding(8);
            lblInfo.Size = new Size(525, 84);
            lblInfo.TabIndex = 5;
            lblInfo.Text = "💡 Consejos:\r\n• Para dar de alta una nueva cobertura ingrese Nombre y Sigla (opcional) y haga clic en 'Guardar'.\r\n• Para editar o dar de baja, haga clic sobre la fila en la tabla inferior.\r\n• 'Particular / Sin Obra Social' es la cobertura por defecto del sistema y no puede darse de baja.";
            // 
            // txtSigla
            // 
            txtSigla.Font = new Font("Segoe UI", 10F);
            txtSigla.Location = new Point(390, 60);
            txtSigla.MaxLength = 20;
            txtSigla.Name = "txtSigla";
            txtSigla.PlaceholderText = "Ej. IOSCOR, OSDE";
            txtSigla.Size = new Size(150, 25);
            txtSigla.TabIndex = 4;
            // 
            // lblSigla
            // 
            lblSigla.AutoSize = true;
            lblSigla.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSigla.ForeColor = Color.FromArgb(51, 65, 85);
            lblSigla.Location = new Point(390, 40);
            lblSigla.Name = "lblSigla";
            lblSigla.Size = new Size(149, 15);
            lblSigla.TabIndex = 3;
            lblSigla.Text = "Sigla / Acrónimo (opcional):";
            // 
            // txtNombre
            // 
            txtNombre.Font = new Font("Segoe UI", 10F);
            txtNombre.Location = new Point(14, 60);
            txtNombre.MaxLength = 100;
            txtNombre.Name = "txtNombre";
            txtNombre.PlaceholderText = "Ej. Obra Social de Empleados de Comercio";
            txtNombre.Size = new Size(360, 25);
            txtNombre.TabIndex = 2;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNombre.ForeColor = Color.FromArgb(51, 65, 85);
            lblNombre.Location = new Point(14, 40);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(244, 15);
            lblNombre.TabIndex = 1;
            lblNombre.Text = "Nombre de la Obra Social / Prepaga: *";
            // 
            // lblTituloDatos
            // 
            lblTituloDatos.AutoSize = true;
            lblTituloDatos.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloDatos.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloDatos.Location = new Point(12, 10);
            lblTituloDatos.Name = "lblTituloDatos";
            lblTituloDatos.Size = new Size(245, 20);
            lblTituloDatos.TabIndex = 0;
            lblTituloDatos.Text = "📝 Datos de la Cobertura Médica";
            // 
            // FrmGestionObrasSociales
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(241, 245, 249);
            ClientSize = new Size(1133, 550);
            Controls.Add(pnlContenedorPrincipal);
            Controls.Add(pnlHeader);
            Name = "FrmGestionObrasSociales";
            Text = "Gestión de Obras Sociales";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContenedorPrincipal.ResumeLayout(false);
            pnlCardGrilla.ResumeLayout(false);
            pnlCardGrilla.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvObrasSociales).EndInit();
            pnlCardDatos.ResumeLayout(false);
            pnlCardDatos.PerformLayout();
            pnlAcciones.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblTituloHeader;
        private Label lblSubtituloHeader;
        private Panel pnlContenedorPrincipal;
        private Panel pnlCardDatos;
        private Label lblTituloDatos;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblSigla;
        private TextBox txtSigla;
        private Label lblInfo;
        private Panel pnlAcciones;
        private Button btnGuardar;
        private Button btnModificar;
        private Button btnDesactivar;
        private Button btnReactivar;
        private Button btnLimpiar;
        private Panel pnlCardGrilla;
        private Label lblTituloGrilla;
        private CheckBox chkMostrarInactivas;
        private DataGridView dgvObrasSociales;
    }
}
