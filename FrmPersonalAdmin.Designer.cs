namespace Gestion_de_Turnos_Medicos
{
    partial class FrmPersonalAdmin
    {
        /// <summary>
        /// Variable necesaria para el diseñador.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
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
            lblApellido = new Label();
            txtApellido = new TextBox();
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            lblContrasenia = new Label();
            txtContrasenia = new TextBox();
            lblEmail = new Label();
            txtEmail = new TextBox();
            lblDNI = new Label();
            txtDNI = new TextBox();
            lblDireccion = new Label();
            txtDireccion = new TextBox();
            lblFecha = new Label();
            dtpFecha = new DateTimePicker();
            lblTelefono = new Label();
            txtTelefono = new TextBox();
            lblSexo = new Label();
            rbHombre = new RadioButton();
            rbMujer = new RadioButton();
            lblMatricula = new Label();
            txtMatricula = new TextBox();
            btnGuardar = new Button();
            btnEliminar = new Button();
            dgvPersonal = new DataGridView();
            pnlDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPersonal).BeginInit();
            SuspendLayout();
            // 
            // pnlDatos
            // 
            pnlDatos.BackColor = Color.FromArgb(77, 208, 196);
            pnlDatos.Controls.Add(lblTitulo);
            pnlDatos.Controls.Add(lblNombre);
            pnlDatos.Controls.Add(txtNombre);
            pnlDatos.Controls.Add(lblApellido);
            pnlDatos.Controls.Add(txtApellido);
            pnlDatos.Controls.Add(lblUsuario);
            pnlDatos.Controls.Add(txtUsuario);
            pnlDatos.Controls.Add(lblContrasenia);
            pnlDatos.Controls.Add(txtContrasenia);
            pnlDatos.Controls.Add(lblEmail);
            pnlDatos.Controls.Add(txtEmail);
            pnlDatos.Controls.Add(lblDNI);
            pnlDatos.Controls.Add(txtDNI);
            pnlDatos.Controls.Add(lblDireccion);
            pnlDatos.Controls.Add(txtDireccion);
            pnlDatos.Controls.Add(lblFecha);
            pnlDatos.Controls.Add(dtpFecha);
            pnlDatos.Controls.Add(lblTelefono);
            pnlDatos.Controls.Add(txtTelefono);
            pnlDatos.Controls.Add(lblSexo);
            pnlDatos.Controls.Add(rbHombre);
            pnlDatos.Controls.Add(rbMujer);
            pnlDatos.Controls.Add(lblMatricula);
            pnlDatos.Controls.Add(txtMatricula);
            pnlDatos.Controls.Add(btnGuardar);
            pnlDatos.Controls.Add(btnEliminar);
            pnlDatos.Dock = DockStyle.Top;
            pnlDatos.Location = new Point(0, 0);
            pnlDatos.Name = "pnlDatos";
            pnlDatos.Size = new Size(828, 290);
            pnlDatos.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Black;
            lblTitulo.Location = new Point(20, 12);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(204, 32);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Personal Medico";
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNombre.Location = new Point(24, 70);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(56, 15);
            lblNombre.TabIndex = 1;
            lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(110, 66);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(130, 23);
            txtNombre.TabIndex = 0;
            // 
            // lblApellido
            // 
            lblApellido.AutoSize = true;
            lblApellido.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblApellido.Location = new Point(20, 100);
            lblApellido.Name = "lblApellido";
            lblApellido.Size = new Size(55, 15);
            lblApellido.TabIndex = 3;
            lblApellido.Text = "Apellido:";
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(110, 96);
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(130, 23);
            txtApellido.TabIndex = 1;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUsuario.Location = new Point(25, 130);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(52, 15);
            lblUsuario.TabIndex = 5;
            lblUsuario.Text = "Usuario:";
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(110, 126);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(130, 23);
            txtUsuario.TabIndex = 2;
            // 
            // lblContrasenia
            // 
            lblContrasenia.AutoSize = true;
            lblContrasenia.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblContrasenia.Location = new Point(5, 160);
            lblContrasenia.Name = "lblContrasenia";
            lblContrasenia.Size = new Size(72, 15);
            lblContrasenia.TabIndex = 7;
            lblContrasenia.Text = "Contraseña:";
            // 
            // txtContrasenia
            // 
            txtContrasenia.Location = new Point(110, 156);
            txtContrasenia.Name = "txtContrasenia";
            txtContrasenia.PasswordChar = '*';
            txtContrasenia.Size = new Size(130, 23);
            txtContrasenia.TabIndex = 3;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEmail.Location = new Point(35, 190);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(39, 15);
            lblEmail.TabIndex = 9;
            lblEmail.Text = "Email:";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(110, 186);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(130, 23);
            txtEmail.TabIndex = 4;
            // 
            // lblDNI
            // 
            lblDNI.AutoSize = true;
            lblDNI.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDNI.Location = new Point(270, 70);
            lblDNI.Name = "lblDNI";
            lblDNI.Size = new Size(32, 15);
            lblDNI.TabIndex = 11;
            lblDNI.Text = "DNI:";
            // 
            // txtDNI
            // 
            txtDNI.Location = new Point(320, 66);
            txtDNI.Name = "txtDNI";
            txtDNI.Size = new Size(130, 23);
            txtDNI.TabIndex = 5;
            // 
            // lblDireccion
            // 
            lblDireccion.AutoSize = true;
            lblDireccion.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDireccion.Location = new Point(245, 100);
            lblDireccion.Name = "lblDireccion";
            lblDireccion.Size = new Size(63, 15);
            lblDireccion.TabIndex = 13;
            lblDireccion.Text = "Direccion:";
            // 
            // txtDireccion
            // 
            txtDireccion.Location = new Point(320, 96);
            txtDireccion.Name = "txtDireccion";
            txtDireccion.Size = new Size(130, 23);
            txtDireccion.TabIndex = 6;
            // 
            // lblFecha
            // 
            lblFecha.AutoSize = true;
            lblFecha.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFecha.Location = new Point(260, 142);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new Size(42, 15);
            lblFecha.TabIndex = 15;
            lblFecha.Text = "Fecha:";
            // 
            // dtpFecha
            // 
            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.Location = new Point(320, 138);
            dtpFecha.Name = "dtpFecha";
            dtpFecha.Size = new Size(140, 23);
            dtpFecha.TabIndex = 7;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTelefono.Location = new Point(245, 180);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(59, 15);
            lblTelefono.TabIndex = 17;
            lblTelefono.Text = "Telefono:";
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(320, 176);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(150, 23);
            txtTelefono.TabIndex = 8;
            // 
            // lblSexo
            // 
            lblSexo.AutoSize = true;
            lblSexo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSexo.Location = new Point(265, 213);
            lblSexo.Name = "lblSexo";
            lblSexo.Size = new Size(38, 15);
            lblSexo.TabIndex = 19;
            lblSexo.Text = "Sexo:";
            // 
            // rbHombre
            // 
            rbHombre.AutoSize = true;
            rbHombre.Location = new Point(320, 211);
            rbHombre.Name = "rbHombre";
            rbHombre.Size = new Size(69, 19);
            rbHombre.TabIndex = 9;
            rbHombre.Text = "Hombre";
            rbHombre.UseVisualStyleBackColor = true;
            // 
            // rbMujer
            // 
            rbMujer.AutoSize = true;
            rbMujer.Location = new Point(400, 211);
            rbMujer.Name = "rbMujer";
            rbMujer.Size = new Size(56, 19);
            rbMujer.TabIndex = 10;
            rbMujer.Text = "Mujer";
            rbMujer.UseVisualStyleBackColor = true;
            // 
            // lblMatricula
            // 
            lblMatricula.AutoSize = true;
            lblMatricula.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMatricula.Location = new Point(460, 62);
            lblMatricula.Name = "lblMatricula";
            lblMatricula.Size = new Size(83, 15);
            lblMatricula.TabIndex = 21;
            lblMatricula.Text = "Nro Matricula";
            // 
            // txtMatricula
            // 
            txtMatricula.Location = new Point(565, 58);
            txtMatricula.Name = "txtMatricula";
            txtMatricula.Size = new Size(140, 23);
            txtMatricula.TabIndex = 11;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(565, 96);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(90, 28);
            btnGuardar.TabIndex = 12;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(565, 136);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(90, 28);
            btnEliminar.TabIndex = 13;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // dgvPersonal
            // 
            dgvPersonal.AllowUserToAddRows = false;
            dgvPersonal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPersonal.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPersonal.Location = new Point(0, 290);
            dgvPersonal.Name = "dgvPersonal";
            dgvPersonal.RowHeadersWidth = 25;
            dgvPersonal.Size = new Size(828, 90);
            dgvPersonal.TabIndex = 14;
            // 
            // FrmPersonal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            ClientSize = new Size(828, 426);
            Controls.Add(dgvPersonal);
            Controls.Add(pnlDatos);
            Name = "FrmPersonal";
            Text = "Personal Medico";
            pnlDatos.ResumeLayout(false);
            pnlDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPersonal).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlDatos;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblApellido;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblContrasenia;
        private System.Windows.Forms.TextBox txtContrasenia;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblDNI;
        private System.Windows.Forms.TextBox txtDNI;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label lblSexo;
        private System.Windows.Forms.RadioButton rbHombre;
        private System.Windows.Forms.RadioButton rbMujer;
        private System.Windows.Forms.Label lblMatricula;
        private System.Windows.Forms.TextBox txtMatricula;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.DataGridView dgvPersonal;
    }
}