namespace Gestion_de_Turnos_Medicos
{
    partial class FrmGestionUsuarios
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
            lblRol = new Label();
            cmbRol = new ComboBox();
            lblInfoMedico = new Label();
            pnlDatosMedicos = new Panel();
            lblDatosMedicosHeader = new Label();
            lblEspecialidades = new Label();
            clbEspecialidades = new CheckedListBox();
            lblSala = new Label();
            clbSala = new CheckedListBox();
            lblMatricula = new Label();
            txtMatricula = new TextBox();
            lblNotaSala = new Label();
            txtNotaSala = new TextBox();
            lblTitulo = new Label();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblApellido = new Label();
            txtApellido = new TextBox();
            lblCorreo = new Label();
            txtCorreo = new TextBox();
            lblContrasena = new Label();
            txtContrasena = new TextBox();
            lblDNI = new Label();
            txtDNI = new TextBox();
            lblTelefono = new Label();
            txtTelefono = new TextBox();
            btnGuardar = new Button();
            btnModificar = new Button();
            btnEliminar = new Button();
            btnLimpiar = new Button();
            dgvPersonal = new DataGridView();
            pnlDatos.SuspendLayout();
            pnlDatosMedicos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPersonal).BeginInit();
            SuspendLayout();
            // 
            // pnlDatos
            // 
            pnlDatos.BackColor = Color.FromArgb(225, 242, 240);
            pnlDatos.Controls.Add(lblRol);
            pnlDatos.Controls.Add(cmbRol);
            pnlDatos.Controls.Add(lblInfoMedico);
            pnlDatos.Controls.Add(pnlDatosMedicos);
            pnlDatos.Controls.Add(lblTitulo);
            pnlDatos.Controls.Add(lblNombre);
            pnlDatos.Controls.Add(txtNombre);
            pnlDatos.Controls.Add(lblApellido);
            pnlDatos.Controls.Add(txtApellido);
            pnlDatos.Controls.Add(lblCorreo);
            pnlDatos.Controls.Add(txtCorreo);
            pnlDatos.Controls.Add(lblContrasena);
            pnlDatos.Controls.Add(txtContrasena);
            pnlDatos.Controls.Add(lblDNI);
            pnlDatos.Controls.Add(txtDNI);
            pnlDatos.Controls.Add(lblTelefono);
            pnlDatos.Controls.Add(txtTelefono);
            pnlDatos.Controls.Add(btnGuardar);
            pnlDatos.Controls.Add(btnModificar);
            pnlDatos.Controls.Add(btnEliminar);
            pnlDatos.Controls.Add(btnLimpiar);
            pnlDatos.Dock = DockStyle.Top;
            pnlDatos.Location = new Point(0, 0);
            pnlDatos.Name = "pnlDatos";
            pnlDatos.Size = new Size(1133, 290);
            pnlDatos.TabIndex = 0;
            // 
            // lblRol
            // 
            lblRol.AutoSize = true;
            lblRol.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRol.Location = new Point(545, 18);
            lblRol.Name = "lblRol";
            lblRol.Size = new Size(28, 15);
            lblRol.TabIndex = 20;
            lblRol.Text = "Rol:";
            // 
            // cmbRol
            // 
            // Los ítems se cargan en tiempo de ejecución desde sp_ListarRoles
            // (ver FrmGestionUsuarios.cs -> CargarRolesDesdeBD). No se harcodea nada acá.
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRol.FormattingEnabled = true;
            cmbRol.Location = new Point(595, 15);
            cmbRol.Name = "cmbRol";
            cmbRol.Size = new Size(180, 23);
            cmbRol.TabIndex = 6;
            cmbRol.SelectedIndexChanged += cmbRol_SelectedIndexChanged;
            // 
            // lblInfoMedico
            // 
            lblInfoMedico.BackColor = Color.FromArgb(225, 242, 240);
            lblInfoMedico.BorderStyle = BorderStyle.FixedSingle;
            lblInfoMedico.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblInfoMedico.Location = new Point(545, 50);
            lblInfoMedico.Name = "lblInfoMedico";
            lblInfoMedico.Size = new Size(470, 135);
            lblInfoMedico.TabIndex = 21;
            lblInfoMedico.Text = "Sección reservada solo para Personal Médico.\n(Habilitar seleccionando 'Personal Médico' en el rol)";
            lblInfoMedico.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlDatosMedicos
            // 
            pnlDatosMedicos.BackColor = Color.FromArgb(225, 242, 240);
            pnlDatosMedicos.BorderStyle = BorderStyle.FixedSingle;
            pnlDatosMedicos.Controls.Add(lblDatosMedicosHeader);
            pnlDatosMedicos.Controls.Add(lblEspecialidades);
            pnlDatosMedicos.Controls.Add(clbEspecialidades);
            pnlDatosMedicos.Controls.Add(lblSala);
            pnlDatosMedicos.Controls.Add(clbSala);
            pnlDatosMedicos.Controls.Add(lblMatricula);
            pnlDatosMedicos.Controls.Add(txtMatricula);
            pnlDatosMedicos.Controls.Add(lblNotaSala);
            pnlDatosMedicos.Controls.Add(txtNotaSala);
            pnlDatosMedicos.Location = new Point(545, 50);
            pnlDatosMedicos.Name = "pnlDatosMedicos";
            pnlDatosMedicos.Size = new Size(470, 135);
            pnlDatosMedicos.TabIndex = 22;
            pnlDatosMedicos.Visible = false;
            // 
            // lblDatosMedicosHeader
            // 
            lblDatosMedicosHeader.AutoSize = true;
            lblDatosMedicosHeader.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDatosMedicosHeader.Location = new Point(12, 8);
            lblDatosMedicosHeader.Name = "lblDatosMedicosHeader";
            lblDatosMedicosHeader.Size = new Size(107, 19);
            lblDatosMedicosHeader.TabIndex = 0;
            lblDatosMedicosHeader.Text = "Datos Médicos";
            // 
            // lblEspecialidades
            // 
            lblEspecialidades.AutoSize = true;
            lblEspecialidades.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEspecialidades.Location = new Point(12, 34);
            lblEspecialidades.Name = "lblEspecialidades";
            lblEspecialidades.Size = new Size(85, 15);
            lblEspecialidades.TabIndex = 1;
            lblEspecialidades.Text = "Especialidades";
            // 
            // clbEspecialidades
            // 
            // Los ítems se cargan desde sp_ListarEspecialidades (ver CargarEspecialidadesDesdeBD).
            clbEspecialidades.FormattingEnabled = true;
            clbEspecialidades.Location = new Point(12, 51);
            clbEspecialidades.Name = "clbEspecialidades";
            clbEspecialidades.Size = new Size(140, 58);
            clbEspecialidades.TabIndex = 2;
            // 
            // lblSala
            // 
            lblSala.AutoSize = true;
            lblSala.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSala.Location = new Point(165, 34);
            lblSala.Name = "lblSala";
            lblSala.Size = new Size(79, 15);
            lblSala.TabIndex = 3;
            lblSala.Text = "Sala asignada";
            // 
            // clbSala
            // 
            // Los ítems se cargan desde sp_ListarSalas (ver CargarSalasDesdeBD).
            clbSala.FormattingEnabled = true;
            clbSala.Location = new Point(165, 51);
            clbSala.Name = "clbSala";
            clbSala.Size = new Size(90, 58);
            clbSala.TabIndex = 4;
            // 
            // lblMatricula
            // 
            lblMatricula.AutoSize = true;
            lblMatricula.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMatricula.Location = new Point(268, 34);
            lblMatricula.Name = "lblMatricula";
            lblMatricula.Size = new Size(83, 15);
            lblMatricula.TabIndex = 5;
            lblMatricula.Text = "Nro Matricula";
            // 
            // txtMatricula
            // 
            txtMatricula.Location = new Point(268, 51);
            txtMatricula.Name = "txtMatricula";
            txtMatricula.Size = new Size(150, 23);
            txtMatricula.TabIndex = 6;
            // 
            // lblNotaSala
            // 
            lblNotaSala.AutoSize = true;
            lblNotaSala.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNotaSala.Location = new Point(268, 78);
            lblNotaSala.Name = "lblNotaSala";
            lblNotaSala.Size = new Size(120, 15);
            lblNotaSala.TabIndex = 7;
            lblNotaSala.Text = "Nota sala (opcional)";
            // 
            // txtNotaSala
            // 
            // Se guarda como DetalleSala.DescripcionAtencion para cada sala marcada.
            txtNotaSala.Location = new Point(268, 95);
            txtNotaSala.Name = "txtNotaSala";
            txtNotaSala.Size = new Size(150, 23);
            txtNotaSala.TabIndex = 8;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Black;
            lblTitulo.Location = new Point(20, 12);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(113, 32);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Usuarios";
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
            // lblCorreo
            // 
            lblCorreo.AutoSize = true;
            lblCorreo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCorreo.Location = new Point(28, 130);
            lblCorreo.Name = "lblCorreo";
            lblCorreo.Size = new Size(50, 15);
            lblCorreo.TabIndex = 5;
            lblCorreo.Text = "Correo:";
            // 
            // txtCorreo
            // 
            txtCorreo.Location = new Point(110, 126);
            txtCorreo.Name = "txtCorreo";
            txtCorreo.Size = new Size(130, 23);
            txtCorreo.TabIndex = 2;
            // 
            // lblContrasena
            // 
            lblContrasena.AutoSize = true;
            lblContrasena.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblContrasena.Location = new Point(5, 160);
            lblContrasena.Name = "lblContrasena";
            lblContrasena.Size = new Size(72, 15);
            lblContrasena.TabIndex = 7;
            lblContrasena.Text = "Contraseña:";
            // 
            // txtContrasena
            // 
            txtContrasena.Location = new Point(110, 156);
            txtContrasena.Name = "txtContrasena";
            txtContrasena.PasswordChar = '*';
            txtContrasena.Size = new Size(130, 23);
            txtContrasena.TabIndex = 3;
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
            txtDNI.Location = new Point(334, 70);
            txtDNI.Name = "txtDNI";
            txtDNI.Size = new Size(130, 23);
            txtDNI.TabIndex = 5;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTelefono.Location = new Point(255, 103);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(59, 15);
            lblTelefono.TabIndex = 17;
            lblTelefono.Text = "Telefono:";
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(334, 100);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(150, 23);
            txtTelefono.TabIndex = 8;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(689, 193);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(90, 28);
            btnGuardar.TabIndex = 12;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnModificar
            // 
            btnModificar.Location = new Point(789, 193);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(95, 28);
            btnModificar.TabIndex = 13;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = true;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(894, 193);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(90, 28);
            btnEliminar.TabIndex = 14;
            btnEliminar.Text = "Desactivar";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnLimpiar
            // 
            btnLimpiar.Location = new Point(994, 193);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(90, 28);
            btnLimpiar.TabIndex = 15;
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.UseVisualStyleBackColor = true;
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // dgvPersonal
            // 
            dgvPersonal.AllowUserToAddRows = false;
            dgvPersonal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPersonal.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPersonal.Location = new Point(0, 290);
            dgvPersonal.Name = "dgvPersonal";
            dgvPersonal.RowHeadersWidth = 25;
            dgvPersonal.Size = new Size(1133, 165);
            dgvPersonal.TabIndex = 16;
            dgvPersonal.CellClick += dgvPersonal_CellClick;
            dgvPersonal.CellEndEdit += dgvPersonal_CellEndEdit;
            // 
            // FrmGestionUsuarios
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            ClientSize = new Size(1133, 501);
            Controls.Add(dgvPersonal);
            Controls.Add(pnlDatos);
            Name = "FrmGestionUsuarios";
            Text = "Gestión de Usuarios";
            pnlDatos.ResumeLayout(false);
            pnlDatos.PerformLayout();
            pnlDatosMedicos.ResumeLayout(false);
            pnlDatosMedicos.PerformLayout();
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
        private System.Windows.Forms.Label lblCorreo;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.Label lblContrasena;
        private System.Windows.Forms.TextBox txtContrasena;
        private System.Windows.Forms.Label lblDNI;
        private System.Windows.Forms.TextBox txtDNI;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnModificar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.DataGridView dgvPersonal;

        // --- Rol y sección exclusiva de Personal Médico ---
        private Label lblRol;
        private ComboBox cmbRol;
        private Label lblInfoMedico;
        private Panel pnlDatosMedicos;
        private Label lblDatosMedicosHeader;
        private Label lblEspecialidades;
        private CheckedListBox clbEspecialidades;
        private Label lblSala;
        private CheckedListBox clbSala;
        private Label lblMatricula;
        private TextBox txtMatricula;
        private Label lblNotaSala;
        private TextBox txtNotaSala;
    }
}