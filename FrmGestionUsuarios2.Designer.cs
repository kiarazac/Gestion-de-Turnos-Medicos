namespace Gestion_de_Turnos_Medicos
{
    partial class FrmGestionUsuarios2
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblSubtituloHeader = new Label();
            lblTituloHeader = new Label();
            pnlContenedorPrincipal = new Panel();
            pnlGrillaCard = new Panel();
            chkMostrarInactivos = new CheckBox();
            lblTituloGrilla = new Label();
            dgvPersonal = new DataGridView();
            pnlFormularioDatos = new Panel();
            pnlAcciones = new Panel();
            txtBuscarUsuario = new TextBox();
            btnLimpiar = new Button();
            btnReactivar = new Button();
            btnEliminar = new Button();
            btnModificar = new Button();
            btnGuardar = new Button();
            pnlDatosMedicos = new Panel();
            txtNotaSala = new TextBox();
            lblNotaSala = new Label();
            clbSala = new CheckedListBox();
            lblSala = new Label();
            clbEspecialidades = new CheckedListBox();
            lblEspecialidades = new Label();
            txtMatricula = new TextBox();
            lblMatricula = new Label();
            lblTituloMedicos = new Label();
            pnlDatosPersonales = new Panel();
            lblAvisoContrasena = new Label();
            cmbRol = new ComboBox();
            lblRol = new Label();
            txtContrasena = new TextBox();
            lblContrasena = new Label();
            txtCorreo = new TextBox();
            lblCorreo = new Label();
            txtTelefono = new TextBox();
            lblTelefono = new Label();
            txtDniVerificado = new TextBox();
            lblDniVerificado = new Label();
            txtApellido = new TextBox();
            lblApellido = new Label();
            txtNombre = new TextBox();
            lblNombre = new Label();
            lblTituloDatosPersonales = new Label();
            pnlPaso1Dni = new Panel();
            btnReiniciarDni = new Button();
            lblEstadoDni = new Label();
            btnVerificarDni = new Button();
            txtDniBusqueda = new TextBox();
            lblInstruccionDni = new Label();
            lblPaso1Badge = new Label();
            pnlHeader.SuspendLayout();
            pnlContenedorPrincipal.SuspendLayout();
            pnlGrillaCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPersonal).BeginInit();
            pnlFormularioDatos.SuspendLayout();
            pnlAcciones.SuspendLayout();
            pnlDatosMedicos.SuspendLayout();
            pnlDatosPersonales.SuspendLayout();
            pnlPaso1Dni.SuspendLayout();
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
            pnlHeader.Size = new Size(1293, 60);
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
            lblSubtituloHeader.Text = "Sistema de Verificación Previa por DNI y Gestión Clínica (v2.0)";
            // 
            // lblTituloHeader
            // 
            lblTituloHeader.AutoSize = true;
            lblTituloHeader.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloHeader.ForeColor = Color.White;
            lblTituloHeader.Location = new Point(16, 8);
            lblTituloHeader.Name = "lblTituloHeader";
            lblTituloHeader.Size = new Size(317, 25);
            lblTituloHeader.TabIndex = 0;
            lblTituloHeader.Text = "Gestión de Personal y Usuarios 2.0";
            // 
            // pnlContenedorPrincipal
            // 
            pnlContenedorPrincipal.AutoScroll = true;
            pnlContenedorPrincipal.BackColor = Color.FromArgb(241, 245, 249);
            pnlContenedorPrincipal.Controls.Add(pnlGrillaCard);
            pnlContenedorPrincipal.Controls.Add(pnlFormularioDatos);
            pnlContenedorPrincipal.Controls.Add(pnlPaso1Dni);
            pnlContenedorPrincipal.Dock = DockStyle.Fill;
            pnlContenedorPrincipal.Location = new Point(0, 60);
            pnlContenedorPrincipal.Name = "pnlContenedorPrincipal";
            pnlContenedorPrincipal.Padding = new Padding(16);
            pnlContenedorPrincipal.Size = new Size(1293, 689);
            pnlContenedorPrincipal.TabIndex = 1;
            // 
            // pnlGrillaCard
            // 
            pnlGrillaCard.BackColor = Color.White;
            pnlGrillaCard.BorderStyle = BorderStyle.FixedSingle;
            pnlGrillaCard.Controls.Add(chkMostrarInactivos);
            pnlGrillaCard.Controls.Add(lblTituloGrilla);
            pnlGrillaCard.Controls.Add(dgvPersonal);
            pnlGrillaCard.Dock = DockStyle.Top;
            pnlGrillaCard.Location = new Point(16, 442);
            pnlGrillaCard.Name = "pnlGrillaCard";
            pnlGrillaCard.Padding = new Padding(12);
            pnlGrillaCard.Size = new Size(1261, 230);
            pnlGrillaCard.TabIndex = 2;
            // 
            // chkMostrarInactivos
            // 
            chkMostrarInactivos.AutoSize = true;
            chkMostrarInactivos.Cursor = Cursors.Hand;
            chkMostrarInactivos.Font = new Font("Segoe UI", 9.25F);
            chkMostrarInactivos.ForeColor = Color.FromArgb(71, 85, 105);
            chkMostrarInactivos.Location = new Point(300, 11);
            chkMostrarInactivos.Name = "chkMostrarInactivos";
            chkMostrarInactivos.Size = new Size(278, 21);
            chkMostrarInactivos.TabIndex = 2;
            chkMostrarInactivos.Text = "Mostrar usuarios dados de baja (inactivos)";
            chkMostrarInactivos.UseVisualStyleBackColor = true;
            chkMostrarInactivos.CheckedChanged += chkMostrarInactivos_CheckedChanged;
            // 
            // lblTituloGrilla
            // 
            lblTituloGrilla.AutoSize = true;
            lblTituloGrilla.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloGrilla.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloGrilla.Location = new Point(12, 10);
            lblTituloGrilla.Name = "lblTituloGrilla";
            lblTituloGrilla.Size = new Size(262, 20);
            lblTituloGrilla.TabIndex = 0;
            lblTituloGrilla.Text = "Listado de Personal Registrado (BD)";
            // 
            // dgvPersonal
            // 
            dgvPersonal.AllowUserToAddRows = false;
            dgvPersonal.AllowUserToDeleteRows = false;
            dgvPersonal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPersonal.BackgroundColor = Color.White;
            dgvPersonal.BorderStyle = BorderStyle.None;
            dgvPersonal.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPersonal.Location = new Point(12, 38);
            dgvPersonal.MultiSelect = false;
            dgvPersonal.Name = "dgvPersonal";
            dgvPersonal.ReadOnly = true;
            dgvPersonal.RowHeadersVisible = false;
            dgvPersonal.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPersonal.Size = new Size(1235, 178);
            dgvPersonal.TabIndex = 1;
            dgvPersonal.CellClick += dgvPersonal_CellClick;
            // 
            // pnlFormularioDatos
            // 
            pnlFormularioDatos.BackColor = Color.White;
            pnlFormularioDatos.BorderStyle = BorderStyle.FixedSingle;
            pnlFormularioDatos.Controls.Add(pnlAcciones);
            pnlFormularioDatos.Controls.Add(pnlDatosMedicos);
            pnlFormularioDatos.Controls.Add(pnlDatosPersonales);
            pnlFormularioDatos.Dock = DockStyle.Top;
            pnlFormularioDatos.Location = new Point(16, 88);
            pnlFormularioDatos.Name = "pnlFormularioDatos";
            pnlFormularioDatos.Padding = new Padding(12);
            pnlFormularioDatos.Size = new Size(1261, 354);
            pnlFormularioDatos.TabIndex = 1;
            // 
            // pnlAcciones
            // 
            pnlAcciones.BackColor = Color.FromArgb(248, 250, 252);
            pnlAcciones.BorderStyle = BorderStyle.FixedSingle;
            pnlAcciones.Controls.Add(txtBuscarUsuario);
            pnlAcciones.Controls.Add(btnLimpiar);
            pnlAcciones.Controls.Add(btnReactivar);
            pnlAcciones.Controls.Add(btnEliminar);
            pnlAcciones.Controls.Add(btnModificar);
            pnlAcciones.Controls.Add(btnGuardar);
            pnlAcciones.Dock = DockStyle.Bottom;
            pnlAcciones.Location = new Point(12, 288);
            pnlAcciones.Name = "pnlAcciones";
            pnlAcciones.Size = new Size(1235, 52);
            pnlAcciones.TabIndex = 2;
            // 
            // txtBuscarUsuario
            // 
            txtBuscarUsuario.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtBuscarUsuario.Font = new Font("Segoe UI", 9.75F);
            txtBuscarUsuario.Location = new Point(829, 14);
            txtBuscarUsuario.Name = "txtBuscarUsuario";
            txtBuscarUsuario.PlaceholderText = "🔍 Buscar personal / usuario...";
            txtBuscarUsuario.Size = new Size(235, 25);
            txtBuscarUsuario.TabIndex = 5;
            txtBuscarUsuario.TextChanged += txtBuscarUsuario_TextChanged;
            txtBuscarUsuario.KeyDown += txtBuscarUsuario_KeyDown;
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
            btnLimpiar.Location = new Point(1126, 8);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(96, 34);
            btnLimpiar.TabIndex = 3;
            btnLimpiar.Text = "🔄 Reiniciar / Limpiar";
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
            btnReactivar.Location = new Point(585, 8);
            btnReactivar.Name = "btnReactivar";
            btnReactivar.Size = new Size(160, 34);
            btnReactivar.TabIndex = 4;
            btnReactivar.Text = "♻ Reactivar Usuario";
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
            btnEliminar.Location = new Point(410, 8);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(160, 34);
            btnEliminar.TabIndex = 2;
            btnEliminar.Text = "🗑 Desactivar Usuario";
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
            btnModificar.Location = new Point(205, 8);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(190, 34);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "💾 Guardar Modificaciones";
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
            btnGuardar.Size = new Size(175, 34);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "➕ Registrar Usuario";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // pnlDatosMedicos
            // 
            pnlDatosMedicos.BackColor = Color.FromArgb(240, 253, 250);
            pnlDatosMedicos.BorderStyle = BorderStyle.FixedSingle;
            pnlDatosMedicos.Controls.Add(txtNotaSala);
            pnlDatosMedicos.Controls.Add(lblNotaSala);
            pnlDatosMedicos.Controls.Add(clbSala);
            pnlDatosMedicos.Controls.Add(lblSala);
            pnlDatosMedicos.Controls.Add(clbEspecialidades);
            pnlDatosMedicos.Controls.Add(lblEspecialidades);
            pnlDatosMedicos.Controls.Add(txtMatricula);
            pnlDatosMedicos.Controls.Add(lblMatricula);
            pnlDatosMedicos.Controls.Add(lblTituloMedicos);
            pnlDatosMedicos.Location = new Point(570, 12);
            pnlDatosMedicos.Name = "pnlDatosMedicos";
            pnlDatosMedicos.Padding = new Padding(10);
            pnlDatosMedicos.Size = new Size(560, 266);
            pnlDatosMedicos.TabIndex = 1;
            // 
            // txtNotaSala
            // 
            txtNotaSala.Font = new Font("Segoe UI", 9F);
            txtNotaSala.Location = new Point(292, 196);
            txtNotaSala.Name = "txtNotaSala";
            txtNotaSala.Size = new Size(250, 23);
            txtNotaSala.TabIndex = 8;
            // 
            // lblNotaSala
            // 
            lblNotaSala.AutoSize = true;
            lblNotaSala.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblNotaSala.ForeColor = Color.FromArgb(15, 118, 110);
            lblNotaSala.Location = new Point(292, 178);
            lblNotaSala.Name = "lblNotaSala";
            lblNotaSala.Size = new Size(132, 13);
            lblNotaSala.TabIndex = 7;
            lblNotaSala.Text = "Observación / Atención:";
            // 
            // clbSala
            // 
            clbSala.CheckOnClick = true;
            clbSala.Font = new Font("Segoe UI", 8.5F);
            clbSala.FormattingEnabled = true;
            clbSala.Location = new Point(292, 58);
            clbSala.Name = "clbSala";
            clbSala.Size = new Size(250, 112);
            clbSala.TabIndex = 6;
            // 
            // lblSala
            // 
            lblSala.AutoSize = true;
            lblSala.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblSala.ForeColor = Color.FromArgb(15, 118, 110);
            lblSala.Location = new Point(292, 38);
            lblSala.Name = "lblSala";
            lblSala.Size = new Size(165, 13);
            lblSala.TabIndex = 5;
            lblSala.Text = "Consultorio(s) / Sala(s) Asign.:";
            // 
            // clbEspecialidades
            // 
            clbEspecialidades.CheckOnClick = true;
            clbEspecialidades.Font = new Font("Segoe UI", 8.5F);
            clbEspecialidades.FormattingEnabled = true;
            clbEspecialidades.Location = new Point(16, 112);
            clbEspecialidades.Name = "clbEspecialidades";
            clbEspecialidades.Size = new Size(250, 130);
            clbEspecialidades.TabIndex = 4;
            // 
            // lblEspecialidades
            // 
            lblEspecialidades.AutoSize = true;
            lblEspecialidades.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblEspecialidades.ForeColor = Color.FromArgb(15, 118, 110);
            lblEspecialidades.Location = new Point(16, 92);
            lblEspecialidades.Name = "lblEspecialidades";
            lblEspecialidades.Size = new Size(131, 13);
            lblEspecialidades.TabIndex = 3;
            lblEspecialidades.Text = "Especialidades Médicas:";
            // 
            // txtMatricula
            // 
            txtMatricula.Font = new Font("Segoe UI", 9F);
            txtMatricula.Location = new Point(16, 58);
            txtMatricula.Name = "txtMatricula";
            txtMatricula.Size = new Size(250, 23);
            txtMatricula.TabIndex = 2;
            // 
            // lblMatricula
            // 
            lblMatricula.AutoSize = true;
            lblMatricula.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblMatricula.ForeColor = Color.FromArgb(15, 118, 110);
            lblMatricula.Location = new Point(16, 38);
            lblMatricula.Name = "lblMatricula";
            lblMatricula.Size = new Size(137, 13);
            lblMatricula.TabIndex = 1;
            lblMatricula.Text = "Matrícula Nacional/Prov:";
            // 
            // lblTituloMedicos
            // 
            lblTituloMedicos.AutoSize = true;
            lblTituloMedicos.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblTituloMedicos.ForeColor = Color.FromArgb(15, 118, 110);
            lblTituloMedicos.Location = new Point(12, 10);
            lblTituloMedicos.Name = "lblTituloMedicos";
            lblTituloMedicos.Size = new Size(328, 19);
            lblTituloMedicos.TabIndex = 0;
            lblTituloMedicos.Text = "⚕️ Configuración Exclusiva de Personal Médico";
            // 
            // pnlDatosPersonales
            // 
            pnlDatosPersonales.BackColor = Color.FromArgb(248, 250, 252);
            pnlDatosPersonales.BorderStyle = BorderStyle.FixedSingle;
            pnlDatosPersonales.Controls.Add(lblAvisoContrasena);
            pnlDatosPersonales.Controls.Add(cmbRol);
            pnlDatosPersonales.Controls.Add(lblRol);
            pnlDatosPersonales.Controls.Add(txtContrasena);
            pnlDatosPersonales.Controls.Add(lblContrasena);
            pnlDatosPersonales.Controls.Add(txtCorreo);
            pnlDatosPersonales.Controls.Add(lblCorreo);
            pnlDatosPersonales.Controls.Add(txtTelefono);
            pnlDatosPersonales.Controls.Add(lblTelefono);
            pnlDatosPersonales.Controls.Add(txtDniVerificado);
            pnlDatosPersonales.Controls.Add(lblDniVerificado);
            pnlDatosPersonales.Controls.Add(txtApellido);
            pnlDatosPersonales.Controls.Add(lblApellido);
            pnlDatosPersonales.Controls.Add(txtNombre);
            pnlDatosPersonales.Controls.Add(lblNombre);
            pnlDatosPersonales.Controls.Add(lblTituloDatosPersonales);
            pnlDatosPersonales.Location = new Point(12, 12);
            pnlDatosPersonales.Name = "pnlDatosPersonales";
            pnlDatosPersonales.Padding = new Padding(10);
            pnlDatosPersonales.Size = new Size(540, 266);
            pnlDatosPersonales.TabIndex = 0;
            // 
            // lblAvisoContrasena
            // 
            lblAvisoContrasena.AutoSize = true;
            lblAvisoContrasena.Font = new Font("Segoe UI", 7.5F, FontStyle.Italic);
            lblAvisoContrasena.ForeColor = Color.FromArgb(100, 116, 139);
            lblAvisoContrasena.Location = new Point(275, 178);
            lblAvisoContrasena.Name = "lblAvisoContrasena";
            lblAvisoContrasena.Size = new Size(185, 12);
            lblAvisoContrasena.TabIndex = 15;
            lblAvisoContrasena.Text = "(En blanco para conservar la clave actual)";
            // 
            // cmbRol
            // 
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRol.Font = new Font("Segoe UI", 9F);
            cmbRol.FormattingEnabled = true;
            cmbRol.Location = new Point(14, 218);
            cmbRol.Name = "cmbRol";
            cmbRol.Size = new Size(240, 23);
            cmbRol.TabIndex = 14;
            cmbRol.SelectedIndexChanged += cmbRol_SelectedIndexChanged;
            // 
            // lblRol
            // 
            lblRol.AutoSize = true;
            lblRol.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblRol.ForeColor = Color.FromArgb(51, 65, 85);
            lblRol.Location = new Point(14, 200);
            lblRol.Name = "lblRol";
            lblRol.Size = new Size(80, 13);
            lblRol.TabIndex = 13;
            lblRol.Text = "Rol Asignado:";
            // 
            // txtContrasena
            // 
            txtContrasena.Font = new Font("Segoe UI", 9F);
            txtContrasena.Location = new Point(275, 150);
            txtContrasena.Name = "txtContrasena";
            txtContrasena.Size = new Size(245, 23);
            txtContrasena.TabIndex = 12;
            txtContrasena.UseSystemPasswordChar = true;
            // 
            // lblContrasena
            // 
            lblContrasena.AutoSize = true;
            lblContrasena.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblContrasena.ForeColor = Color.FromArgb(51, 65, 85);
            lblContrasena.Location = new Point(275, 132);
            lblContrasena.Name = "lblContrasena";
            lblContrasena.Size = new Size(69, 13);
            lblContrasena.TabIndex = 11;
            lblContrasena.Text = "Contraseña:";
            // 
            // txtCorreo
            // 
            txtCorreo.Font = new Font("Segoe UI", 9F);
            txtCorreo.Location = new Point(14, 150);
            txtCorreo.Name = "txtCorreo";
            txtCorreo.Size = new Size(240, 23);
            txtCorreo.TabIndex = 10;
            // 
            // lblCorreo
            // 
            lblCorreo.AutoSize = true;
            lblCorreo.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblCorreo.ForeColor = Color.FromArgb(51, 65, 85);
            lblCorreo.Location = new Point(14, 132);
            lblCorreo.Name = "lblCorreo";
            lblCorreo.Size = new Size(105, 13);
            lblCorreo.TabIndex = 9;
            lblCorreo.Text = "Correo Electrónico:";
            // 
            // txtTelefono
            // 
            txtTelefono.Font = new Font("Segoe UI", 9F);
            txtTelefono.Location = new Point(275, 96);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(245, 23);
            txtTelefono.TabIndex = 8;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblTelefono.ForeColor = Color.FromArgb(51, 65, 85);
            lblTelefono.Location = new Point(275, 78);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(55, 13);
            lblTelefono.TabIndex = 7;
            lblTelefono.Text = "Teléfono:";
            // 
            // txtDniVerificado
            // 
            txtDniVerificado.BackColor = Color.FromArgb(241, 245, 249);
            txtDniVerificado.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtDniVerificado.Location = new Point(14, 96);
            txtDniVerificado.Name = "txtDniVerificado";
            txtDniVerificado.ReadOnly = true;
            txtDniVerificado.Size = new Size(240, 23);
            txtDniVerificado.TabIndex = 6;
            // 
            // lblDniVerificado
            // 
            lblDniVerificado.AutoSize = true;
            lblDniVerificado.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblDniVerificado.ForeColor = Color.FromArgb(51, 65, 85);
            lblDniVerificado.Location = new Point(14, 78);
            lblDniVerificado.Name = "lblDniVerificado";
            lblDniVerificado.Size = new Size(84, 13);
            lblDniVerificado.TabIndex = 5;
            lblDniVerificado.Text = "DNI Verificado:";
            // 
            // txtApellido
            // 
            txtApellido.Font = new Font("Segoe UI", 9F);
            txtApellido.Location = new Point(275, 48);
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(245, 23);
            txtApellido.TabIndex = 4;
            // 
            // lblApellido
            // 
            lblApellido.AutoSize = true;
            lblApellido.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblApellido.ForeColor = Color.FromArgb(51, 65, 85);
            lblApellido.Location = new Point(275, 30);
            lblApellido.Name = "lblApellido";
            lblApellido.Size = new Size(54, 13);
            lblApellido.TabIndex = 3;
            lblApellido.Text = "Apellido:";
            // 
            // txtNombre
            // 
            txtNombre.Font = new Font("Segoe UI", 9F);
            txtNombre.Location = new Point(14, 48);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(240, 23);
            txtNombre.TabIndex = 2;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblNombre.ForeColor = Color.FromArgb(51, 65, 85);
            lblNombre.Location = new Point(14, 30);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(53, 13);
            lblNombre.TabIndex = 1;
            lblNombre.Text = "Nombre:";
            // 
            // lblTituloDatosPersonales
            // 
            lblTituloDatosPersonales.AutoSize = true;
            lblTituloDatosPersonales.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblTituloDatosPersonales.ForeColor = Color.FromArgb(30, 41, 59);
            lblTituloDatosPersonales.Location = new Point(10, 8);
            lblTituloDatosPersonales.Name = "lblTituloDatosPersonales";
            lblTituloDatosPersonales.Size = new Size(227, 19);
            lblTituloDatosPersonales.TabIndex = 0;
            lblTituloDatosPersonales.Text = "👤 Datos Personales y de Cuenta";
            // 
            // pnlPaso1Dni
            // 
            pnlPaso1Dni.BackColor = Color.White;
            pnlPaso1Dni.BorderStyle = BorderStyle.FixedSingle;
            pnlPaso1Dni.Controls.Add(btnReiniciarDni);
            pnlPaso1Dni.Controls.Add(lblEstadoDni);
            pnlPaso1Dni.Controls.Add(btnVerificarDni);
            pnlPaso1Dni.Controls.Add(txtDniBusqueda);
            pnlPaso1Dni.Controls.Add(lblInstruccionDni);
            pnlPaso1Dni.Controls.Add(lblPaso1Badge);
            pnlPaso1Dni.Dock = DockStyle.Top;
            pnlPaso1Dni.Location = new Point(16, 16);
            pnlPaso1Dni.Name = "pnlPaso1Dni";
            pnlPaso1Dni.Padding = new Padding(12);
            pnlPaso1Dni.Size = new Size(1261, 72);
            pnlPaso1Dni.TabIndex = 0;
            // 
            // btnReiniciarDni
            // 
            btnReiniciarDni.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReiniciarDni.BackColor = Color.FromArgb(148, 163, 184);
            btnReiniciarDni.Cursor = Cursors.Hand;
            btnReiniciarDni.FlatAppearance.BorderSize = 0;
            btnReiniciarDni.FlatStyle = FlatStyle.Flat;
            btnReiniciarDni.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnReiniciarDni.ForeColor = Color.White;
            btnReiniciarDni.Location = new Point(1108, 18);
            btnReiniciarDni.Name = "btnReiniciarDni";
            btnReiniciarDni.Size = new Size(138, 34);
            btnReiniciarDni.TabIndex = 5;
            btnReiniciarDni.Text = "🔄 Cambiar DNI";
            btnReiniciarDni.UseVisualStyleBackColor = false;
            btnReiniciarDni.Click += btnReiniciarDni_Click;
            // 
            // lblEstadoDni
            // 
            lblEstadoDni.AutoSize = true;
            lblEstadoDni.BackColor = Color.FromArgb(241, 245, 249);
            lblEstadoDni.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblEstadoDni.ForeColor = Color.FromArgb(71, 85, 105);
            lblEstadoDni.Location = new Point(479, 24);
            lblEstadoDni.Name = "lblEstadoDni";
            lblEstadoDni.Padding = new Padding(8, 4, 8, 4);
            lblEstadoDni.Size = new Size(197, 25);
            lblEstadoDni.TabIndex = 4;
            lblEstadoDni.Text = "Esperando ingreso de DNI...";
            // 
            // btnVerificarDni
            // 
            btnVerificarDni.BackColor = Color.FromArgb(2, 132, 199);
            btnVerificarDni.Cursor = Cursors.Hand;
            btnVerificarDni.FlatAppearance.BorderSize = 0;
            btnVerificarDni.FlatStyle = FlatStyle.Flat;
            btnVerificarDni.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnVerificarDni.ForeColor = Color.White;
            btnVerificarDni.Location = new Point(348, 21);
            btnVerificarDni.Name = "btnVerificarDni";
            btnVerificarDni.Size = new Size(125, 34);
            btnVerificarDni.TabIndex = 3;
            btnVerificarDni.Text = "🔍 Verificar";
            btnVerificarDni.UseVisualStyleBackColor = false;
            btnVerificarDni.Click += btnVerificarDni_Click;
            // 
            // txtDniBusqueda
            // 
            txtDniBusqueda.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            txtDniBusqueda.Location = new Point(180, 22);
            txtDniBusqueda.MaxLength = 10;
            txtDniBusqueda.Name = "txtDniBusqueda";
            txtDniBusqueda.PlaceholderText = "Ej: 35123456";
            txtDniBusqueda.Size = new Size(150, 29);
            txtDniBusqueda.TabIndex = 2;
            txtDniBusqueda.KeyDown += txtDniBusqueda_KeyDown;
            // 
            // lblInstruccionDni
            // 
            lblInstruccionDni.AutoSize = true;
            lblInstruccionDni.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblInstruccionDni.ForeColor = Color.FromArgb(30, 41, 59);
            lblInstruccionDni.Location = new Point(78, 29);
            lblInstruccionDni.Name = "lblInstruccionDni";
            lblInstruccionDni.Size = new Size(90, 15);
            lblInstruccionDni.TabIndex = 1;
            lblInstruccionDni.Text = "Ingrese el DNI:";
            // 
            // lblPaso1Badge
            // 
            lblPaso1Badge.AutoSize = true;
            lblPaso1Badge.BackColor = Color.FromArgb(15, 118, 110);
            lblPaso1Badge.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPaso1Badge.ForeColor = Color.White;
            lblPaso1Badge.Location = new Point(14, 26);
            lblPaso1Badge.Name = "lblPaso1Badge";
            lblPaso1Badge.Padding = new Padding(6, 3, 6, 3);
            lblPaso1Badge.Size = new Size(59, 21);
            lblPaso1Badge.TabIndex = 0;
            lblPaso1Badge.Text = "PASO 1";
            // 
            // FrmGestionUsuarios2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1293, 749);
            Controls.Add(pnlContenedorPrincipal);
            Controls.Add(pnlHeader);
            Name = "FrmGestionUsuarios2";
            Text = "Gestión de Usuarios 2.0 (Prueba)";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContenedorPrincipal.ResumeLayout(false);
            pnlGrillaCard.ResumeLayout(false);
            pnlGrillaCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPersonal).EndInit();
            pnlFormularioDatos.ResumeLayout(false);
            pnlAcciones.ResumeLayout(false);
            pnlAcciones.PerformLayout();
            pnlDatosMedicos.ResumeLayout(false);
            pnlDatosMedicos.PerformLayout();
            pnlDatosPersonales.ResumeLayout(false);
            pnlDatosPersonales.PerformLayout();
            pnlPaso1Dni.ResumeLayout(false);
            pnlPaso1Dni.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblSubtituloHeader;
        private Label lblTituloHeader;
        private Panel pnlContenedorPrincipal;
        private Panel pnlPaso1Dni;
        private Label lblPaso1Badge;
        private Label lblInstruccionDni;
        private TextBox txtDniBusqueda;
        private Button btnVerificarDni;
        private Label lblEstadoDni;
        private Button btnReiniciarDni;
        private Panel pnlFormularioDatos;
        private Panel pnlDatosPersonales;
        private Label lblTituloDatosPersonales;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblApellido;
        private TextBox txtApellido;
        private Label lblDniVerificado;
        private TextBox txtDniVerificado;
        private Label lblTelefono;
        private TextBox txtTelefono;
        private Label lblCorreo;
        private TextBox txtCorreo;
        private Label lblContrasena;
        private TextBox txtContrasena;
        private Label lblAvisoContrasena;
        private Label lblRol;
        private ComboBox cmbRol;
        private Panel pnlDatosMedicos;
        private Label lblTituloMedicos;
        private Label lblMatricula;
        private TextBox txtMatricula;
        private Label lblEspecialidades;
        private CheckedListBox clbEspecialidades;
        private Label lblSala;
        private CheckedListBox clbSala;
        private Label lblNotaSala;
        private TextBox txtNotaSala;
        private Panel pnlAcciones;
        private Button btnGuardar;
        private Button btnModificar;
        private Button btnEliminar;
        private Button btnReactivar;
        private TextBox txtBuscarUsuario;
        private Button btnLimpiar;
        private Panel pnlGrillaCard;
        private Label lblTituloGrilla;
        private CheckBox chkMostrarInactivos;
        private DataGridView dgvPersonal;
    }
}
