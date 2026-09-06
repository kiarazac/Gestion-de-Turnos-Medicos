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
            pnlDatos = new Panel();
            lblTitulo = new Label();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblInfo = new Label();
            btnGuardar = new Button();
            btnDesactivar = new Button();
            dgvEspecialidades = new DataGridView();
            pnlDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEspecialidades).BeginInit();
            SuspendLayout();
            // 
            // pnlDatos
            // 
            pnlDatos.BackColor = Color.FromArgb(225, 242, 240);
            pnlDatos.Controls.Add(lblTitulo);
            pnlDatos.Controls.Add(lblNombre);
            pnlDatos.Controls.Add(txtNombre);
            pnlDatos.Controls.Add(lblInfo);
            pnlDatos.Controls.Add(btnGuardar);
            pnlDatos.Controls.Add(btnDesactivar);
            pnlDatos.Dock = DockStyle.Top;
            pnlDatos.Location = new Point(0, 0);
            pnlDatos.Name = "pnlDatos";
            pnlDatos.Size = new Size(1133, 230);
            pnlDatos.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Black;
            lblTitulo.Location = new Point(20, 12);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(307, 32);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Gestión de Especialidades";
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNombre.Location = new Point(24, 70);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(147, 15);
            lblNombre.TabIndex = 1;
            lblNombre.Text = "Nombre de Especialidad:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(190, 67);
            txtNombre.MaxLength = 100;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(250, 23);
            txtNombre.TabIndex = 2;
            // 
            // lblInfo
            // 
            lblInfo.BackColor = Color.FromArgb(225, 242, 240);
            lblInfo.BorderStyle = BorderStyle.FixedSingle;
            lblInfo.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblInfo.Location = new Point(490, 50);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(500, 75);
            lblInfo.TabIndex = 5;
            lblInfo.Text = "Ingrese el nombre de la especialidad médica y presione 'Guardar'.\r\nPara dar de baja una especialidad existente, selecciónela en la tabla inferior y presione 'Desactivar'.";
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(190, 115);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(100, 28);
            btnGuardar.TabIndex = 3;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnDesactivar
            // 
            btnDesactivar.Location = new Point(310, 115);
            btnDesactivar.Name = "btnDesactivar";
            btnDesactivar.Size = new Size(100, 28);
            btnDesactivar.TabIndex = 4;
            btnDesactivar.Text = "Desactivar";
            btnDesactivar.UseVisualStyleBackColor = true;
            btnDesactivar.Click += btnDesactivar_Click;
            // 
            // dgvEspecialidades
            // 
            dgvEspecialidades.AllowUserToAddRows = false;
            dgvEspecialidades.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvEspecialidades.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEspecialidades.Location = new Point(0, 230);
            dgvEspecialidades.Name = "dgvEspecialidades";
            dgvEspecialidades.RowHeadersWidth = 25;
            dgvEspecialidades.Size = new Size(1133, 271);
            dgvEspecialidades.TabIndex = 1;
            // 
            // FrmGestionEspecialidades
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            ClientSize = new Size(1133, 501);
            Controls.Add(dgvEspecialidades);
            Controls.Add(pnlDatos);
            Name = "FrmGestionEspecialidades";
            Text = "Gestión de Especialidades";
            pnlDatos.ResumeLayout(false);
            pnlDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEspecialidades).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlDatos;
        private Label lblTitulo;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblInfo;
        private Button btnGuardar;
        private Button btnDesactivar;
        private DataGridView dgvEspecialidades;
    }
}
