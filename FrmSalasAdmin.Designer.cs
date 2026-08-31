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
            pnlDatos = new Panel();
            btnEliminar = new Button();
            btnGuardar = new Button();
            clbPersonal = new CheckedListBox();
            lblPersonal = new Label();
            cmbEstadoSala = new ComboBox();
            lblEstadoSala = new Label();
            txtNombreSala = new TextBox();
            lblNombreSala = new Label();
            lblTitulo = new Label();
            dgvSalas = new DataGridView();
            pnlDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSalas).BeginInit();
            SuspendLayout();
            // 
            // pnlDatos
            // 
            pnlDatos.BackColor = Color.FromArgb(225, 242, 240);
            pnlDatos.Controls.Add(btnEliminar);
            pnlDatos.Controls.Add(btnGuardar);
            pnlDatos.Controls.Add(clbPersonal);
            pnlDatos.Controls.Add(lblPersonal);
            pnlDatos.Controls.Add(cmbEstadoSala);
            pnlDatos.Controls.Add(lblEstadoSala);
            pnlDatos.Controls.Add(txtNombreSala);
            pnlDatos.Controls.Add(lblNombreSala);
            pnlDatos.Controls.Add(lblTitulo);
            pnlDatos.Dock = DockStyle.Top;
            pnlDatos.Location = new Point(0, 0);
            pnlDatos.Name = "pnlDatos";
            pnlDatos.Size = new Size(1133, 290);
            pnlDatos.TabIndex = 0;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(688, 131);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(75, 23);
            btnEliminar.TabIndex = 8;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(688, 70);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 23);
            btnGuardar.TabIndex = 7;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // clbPersonal
            // 
            clbPersonal.FormattingEnabled = true;
            clbPersonal.Items.AddRange(new object[] { "Dr. Garcia", "Enfermero Medina" });
            clbPersonal.Location = new Point(435, 66);
            clbPersonal.Name = "clbPersonal";
            clbPersonal.Size = new Size(220, 112);
            clbPersonal.TabIndex = 6;
            // 
            // lblPersonal
            // 
            lblPersonal.AutoSize = true;
            lblPersonal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPersonal.Location = new Point(310, 70);
            lblPersonal.Name = "lblPersonal";
            lblPersonal.Size = new Size(110, 15);
            lblPersonal.TabIndex = 5;
            lblPersonal.Text = "Personal Asignado:";
            // 
            // cmbEstadoSala
            // 
            cmbEstadoSala.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstadoSala.FormattingEnabled = true;
            cmbEstadoSala.Items.AddRange(new object[] { "Ocupado", "Disponible", "Mantenimiento" });
            cmbEstadoSala.Location = new Point(110, 106);
            cmbEstadoSala.Name = "cmbEstadoSala";
            cmbEstadoSala.Size = new Size(150, 23);
            cmbEstadoSala.TabIndex = 4;
            // 
            // lblEstadoSala
            // 
            lblEstadoSala.AutoSize = true;
            lblEstadoSala.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEstadoSala.Location = new Point(24, 110);
            lblEstadoSala.Name = "lblEstadoSala";
            lblEstadoSala.Size = new Size(46, 15);
            lblEstadoSala.TabIndex = 3;
            lblEstadoSala.Text = "Estado:";
            // 
            // txtNombreSala
            // 
            txtNombreSala.Location = new Point(110, 66);
            txtNombreSala.Name = "txtNombreSala";
            txtNombreSala.Size = new Size(150, 23);
            txtNombreSala.TabIndex = 2;
            // 
            // lblNombreSala
            // 
            lblNombreSala.AutoSize = true;
            lblNombreSala.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNombreSala.Location = new Point(24, 70);
            lblNombreSala.Name = "lblNombreSala";
            lblNombreSala.Size = new Size(56, 15);
            lblNombreSala.TabIndex = 1;
            lblNombreSala.Text = "Nombre:";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Black;
            lblTitulo.Location = new Point(20, 12);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(200, 32);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Gestión de Salas";
            // 
            // dgvSalas
            // 
            dgvSalas.AllowUserToAddRows = false;
            dgvSalas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvSalas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSalas.Location = new Point(0, 290);
            dgvSalas.Name = "dgvSalas";
            dgvSalas.RowHeadersWidth = 25;
            dgvSalas.Size = new Size(1133, 165);
            dgvSalas.TabIndex = 9;
            // 
            // FrmSalasAdmin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            ClientSize = new Size(1133, 501);
            Controls.Add(dgvSalas);
            Controls.Add(pnlDatos);
            Name = "FrmSalasAdmin";
            Text = "Gestión de Salas";
            pnlDatos.ResumeLayout(false);
            pnlDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSalas).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlDatos;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblNombreSala;
        private System.Windows.Forms.TextBox txtNombreSala;
        private System.Windows.Forms.Label lblEstadoSala;
        private System.Windows.Forms.ComboBox cmbEstadoSala;
        private System.Windows.Forms.Label lblPersonal;
        private System.Windows.Forms.CheckedListBox clbPersonal;
        private System.Windows.Forms.DataGridView dgvSalas;
        private Button btnGuardar;
        private Button btnEliminar;
    }
}