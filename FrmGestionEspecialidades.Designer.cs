namespace Gestion_de_Turnos_Medicos
{
    partial class FrmGestionEspecialidades
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
            lblTituloGrilla = new Label();
            dgvEspecialidades = new DataGridView();
            pnlCardDatos = new Panel();
            pnlAcciones = new Panel();
            btnDesactivar = new Button();
            btnGuardar = new Button();
            lblInfo = new Label();
            txtNombre = new TextBox();
            lblNombre = new Label();
            lblTituloDatos = new Label();
            pnlHeader.SuspendLayout();
            pnlContenedorPrincipal.SuspendLayout();
            pnlCardGrilla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEspecialidades).BeginInit();
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
            lblSubtituloHeader.Text = "Catálogo clínico de especialidades y ramas médicas de atención";
            // 
            // lblTituloHeader
            // 
            lblTituloHeader.AutoSize = true;
            lblTituloHeader.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloHeader.ForeColor = Color.White;
            lblTituloHeader.Location = new Point(16, 8);
            lblTituloHeader.Name = "lblTituloHeader";
            lblTituloHeader.Size = new Size(328, 25);
            lblTituloHeader.TabIndex = 0;
            lblTituloHeader.Text = "Gestión de Especialidades Médicas";
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
            pnlCardGrilla.Controls.Add(lblTituloGrilla);
            pnlCardGrilla.Controls.Add(dgvEspecialidades);
            pnlCardGrilla.Dock = DockStyle.Fill;
            pnlCardGrilla.Location = new Point(16, 206);
            pnlCardGrilla.Name = "pnlCardGrilla";
            pnlCardGrilla.Padding = new Padding(12);
            pnlCardGrilla.Size = new Size(1101, 219);
            pnlCardGrilla.TabIndex = 1;
            // 
            // lblTituloGrilla
            // 
            lblTituloGrilla.AutoSize = true;
            lblTituloGrilla.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloGrilla.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloGrilla.Location = new Point(12, 8);
            lblTituloGrilla.Name = "lblTituloGrilla";
            lblTituloGrilla.Size = new Size(341, 20);
            lblTituloGrilla.TabIndex = 0;
            lblTituloGrilla.Text = "📋 Especialidades Registradas en el Sistema (BD)";
            // 
            // dgvEspecialidades
            // 
            dgvEspecialidades.AllowUserToAddRows = false;
            dgvEspecialidades.AllowUserToDeleteRows = false;
            dgvEspecialidades.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvEspecialidades.BackgroundColor = Color.White;
            dgvEspecialidades.BorderStyle = BorderStyle.None;
            dgvEspecialidades.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEspecialidades.Location = new Point(12, 34);
            dgvEspecialidades.MultiSelect = false;
            dgvEspecialidades.Name = "dgvEspecialidades";
            dgvEspecialidades.ReadOnly = true;
            dgvEspecialidades.RowHeadersVisible = false;
            dgvEspecialidades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEspecialidades.Size = new Size(1075, 171);
            dgvEspecialidades.TabIndex = 1;
            // 
            // pnlCardDatos
            // 
            pnlCardDatos.BackColor = Color.White;
            pnlCardDatos.BorderStyle = BorderStyle.FixedSingle;
            pnlCardDatos.Controls.Add(pnlAcciones);
            pnlCardDatos.Controls.Add(lblInfo);
            pnlCardDatos.Controls.Add(txtNombre);
            pnlCardDatos.Controls.Add(lblNombre);
            pnlCardDatos.Controls.Add(lblTituloDatos);
            pnlCardDatos.Dock = DockStyle.Top;
            pnlCardDatos.Location = new Point(16, 16);
            pnlCardDatos.Name = "pnlCardDatos";
            pnlCardDatos.Padding = new Padding(12);
            pnlCardDatos.Size = new Size(1101, 190);
            pnlCardDatos.TabIndex = 0;
            // 
            // pnlAcciones
            // 
            pnlAcciones.BackColor = Color.FromArgb(248, 250, 252);
            pnlAcciones.BorderStyle = BorderStyle.FixedSingle;
            pnlAcciones.Controls.Add(btnDesactivar);
            pnlAcciones.Controls.Add(btnGuardar);
            pnlAcciones.Dock = DockStyle.Bottom;
            pnlAcciones.Location = new Point(12, 126);
            pnlAcciones.Name = "pnlAcciones";
            pnlAcciones.Size = new Size(1075, 50);
            pnlAcciones.TabIndex = 6;
            // 
            // btnDesactivar
            // 
            btnDesactivar.BackColor = Color.FromArgb(220, 38, 38);
            btnDesactivar.Cursor = Cursors.Hand;
            btnDesactivar.FlatAppearance.BorderSize = 0;
            btnDesactivar.FlatStyle = FlatStyle.Flat;
            btnDesactivar.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnDesactivar.ForeColor = Color.White;
            btnDesactivar.Location = new Point(230, 8);
            btnDesactivar.Name = "btnDesactivar";
            btnDesactivar.Size = new Size(200, 32);
            btnDesactivar.TabIndex = 1;
            btnDesactivar.Text = "🗑 Desactivar Especialidad";
            btnDesactivar.UseVisualStyleBackColor = false;
            btnDesactivar.Click += btnDesactivar_Click;
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
            btnGuardar.Size = new Size(200, 32);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "➕ Guardar Especialidad";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // lblInfo
            // 
            lblInfo.BackColor = Color.FromArgb(240, 253, 250);
            lblInfo.BorderStyle = BorderStyle.FixedSingle;
            lblInfo.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblInfo.ForeColor = Color.FromArgb(15, 118, 110);
            lblInfo.Location = new Point(480, 38);
            lblInfo.Name = "lblInfo";
            lblInfo.Padding = new Padding(8);
            lblInfo.Size = new Size(540, 70);
            lblInfo.TabIndex = 5;
            lblInfo.Text = "• Para registrar: Ingrese el nombre de la especialidad y presione 'Guardar'.\r\n• Para dar de baja: Seleccione la especialidad en la tabla inferior y presione 'Desactivar'.";
            lblInfo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtNombre
            // 
            txtNombre.Font = new Font("Segoe UI", 9.5F);
            txtNombre.Location = new Point(16, 68);
            txtNombre.MaxLength = 100;
            txtNombre.Name = "txtNombre";
            txtNombre.PlaceholderText = "Ej: Cardiología, Pediatría, Traumatología...";
            txtNombre.Size = new Size(420, 24);
            txtNombre.TabIndex = 2;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNombre.ForeColor = Color.FromArgb(51, 65, 85);
            lblNombre.Location = new Point(16, 46);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(157, 15);
            lblNombre.TabIndex = 1;
            lblNombre.Text = "Nombre de la Especialidad:";
            // 
            // lblTituloDatos
            // 
            lblTituloDatos.AutoSize = true;
            lblTituloDatos.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblTituloDatos.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloDatos.Location = new Point(12, 10);
            lblTituloDatos.Name = "lblTituloDatos";
            lblTituloDatos.Size = new Size(276, 19);
            lblTituloDatos.TabIndex = 0;
            lblTituloDatos.Text = "🩺 Registro de Especialidades Médicas";
            // 
            // FrmGestionEspecialidades
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1133, 501);
            Controls.Add(pnlContenedorPrincipal);
            Controls.Add(pnlHeader);
            Name = "FrmGestionEspecialidades";
            Text = "Gestión de Especialidades";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContenedorPrincipal.ResumeLayout(false);
            pnlCardGrilla.ResumeLayout(false);
            pnlCardGrilla.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEspecialidades).EndInit();
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
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblInfo;
        private Panel pnlAcciones;
        private Button btnGuardar;
        private Button btnDesactivar;
        private Panel pnlCardGrilla;
        private Label lblTituloGrilla;
        private DataGridView dgvEspecialidades;
    }
}
