namespace Gestion_de_Turnos_Medicos
{
    partial class FrmListaTurnosAtencion
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

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            tmrTiempoTranscurrido = new System.Windows.Forms.Timer(components);
            pnlHeader = new Panel();
            lblTituloPrincipal = new Label();
            pnlSubHeader = new Panel();
            lblMedicoInfo = new Label();
            pnlContenido = new Panel();
            lblServicio = new Label();
            cboServicio = new ComboBox();
            dgvTurnos = new DataGridView();
            pnlAtencionActual = new Panel();
            lblPanelTitulo = new Label();
            lblInfoTurno = new Label();
            lblInfoPaciente = new Label();
            lblInfoDni = new Label();
            lblInfoMotivo = new Label();
            lblInfoPrioridadValor = new Label();
            lblInfoTiempo = new Label();
            lblObservaciones = new Label();
            lblDiagnostico = new Label();
            txtDiagnostico = new TextBox();
            btnSiguientePaciente = new Button();
            btnIniciarAtencion = new Button();
            btnTerminarAtencion = new Button();
            lblAvisoBloqueo = new Label();
            lblTrazabilidad = new Label();
            pnlEstado = new Panel();
            lblEstadoInferior = new Label();
            pnlHeader.SuspendLayout();
            pnlSubHeader.SuspendLayout();
            pnlContenido.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTurnos).BeginInit();
            pnlAtencionActual.SuspendLayout();
            pnlEstado.SuspendLayout();
            SuspendLayout();
            // 
            // tmrTiempoTranscurrido
            // 
            tmrTiempoTranscurrido.Interval = 1000;
            tmrTiempoTranscurrido.Tick += tmrTiempoTranscurrido_Tick;
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(21, 101, 178);
            pnlHeader.Controls.Add(lblTituloPrincipal);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1030, 72);
            pnlHeader.TabIndex = 0;
            // 
            // lblTituloPrincipal
            // 
            lblTituloPrincipal.AutoSize = true;
            lblTituloPrincipal.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTituloPrincipal.ForeColor = Color.White;
            lblTituloPrincipal.Location = new Point(24, 14);
            lblTituloPrincipal.Name = "lblTituloPrincipal";
            lblTituloPrincipal.Size = new Size(291, 41);
            lblTituloPrincipal.TabIndex = 0;
            lblTituloPrincipal.Text = "Turnos por Atender";
            // 
            // pnlSubHeader
            // 
            pnlSubHeader.BackColor = Color.FromArgb(189, 214, 238);
            pnlSubHeader.Controls.Add(lblMedicoInfo);
            pnlSubHeader.Dock = DockStyle.Top;
            pnlSubHeader.Location = new Point(0, 72);
            pnlSubHeader.Name = "pnlSubHeader";
            pnlSubHeader.Size = new Size(1030, 34);
            pnlSubHeader.TabIndex = 1;
            // 
            // lblMedicoInfo
            // 
            lblMedicoInfo.AutoSize = true;
            lblMedicoInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblMedicoInfo.ForeColor = Color.FromArgb(20, 40, 70);
            lblMedicoInfo.Location = new Point(24, 8);
            lblMedicoInfo.Name = "lblMedicoInfo";
            lblMedicoInfo.Size = new Size(394, 19);
            lblMedicoInfo.TabIndex = 0;
            lblMedicoInfo.Text = "Dr. Juan Pérez (M.N. 12345)  |  Sala: Consultorio 3 (Piso 1)";
            // 
            // pnlContenido
            // 
            pnlContenido.BackColor = Color.FromArgb(191, 191, 191);
            pnlContenido.Controls.Add(lblServicio);
            pnlContenido.Controls.Add(cboServicio);
            pnlContenido.Controls.Add(dgvTurnos);
            pnlContenido.Controls.Add(pnlAtencionActual);
            pnlContenido.Controls.Add(btnSiguientePaciente);
            pnlContenido.Controls.Add(btnIniciarAtencion);
            pnlContenido.Controls.Add(btnTerminarAtencion);
            pnlContenido.Controls.Add(lblAvisoBloqueo);
            pnlContenido.Controls.Add(lblTrazabilidad);
            pnlContenido.Dock = DockStyle.Fill;
            pnlContenido.Location = new Point(0, 106);
            pnlContenido.Name = "pnlContenido";
            pnlContenido.Size = new Size(1030, 568);
            pnlContenido.TabIndex = 2;
            // 
            // lblServicio
            // 
            lblServicio.AutoSize = true;
            lblServicio.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblServicio.Location = new Point(20, 16);
            lblServicio.Name = "lblServicio";
            lblServicio.Size = new Size(164, 15);
            lblServicio.TabIndex = 0;
            lblServicio.Text = "Servicio / Puesto de Trabajo:";
            // 
            // cboServicio
            // 
            cboServicio.DropDownStyle = ComboBoxStyle.DropDownList;
            cboServicio.FormattingEnabled = true;
            cboServicio.Location = new Point(210, 12);
            cboServicio.Name = "cboServicio";
            cboServicio.Size = new Size(260, 23);
            cboServicio.TabIndex = 0;
            cboServicio.SelectedIndexChanged += cboServicio_SelectedIndexChanged;
            // 
            // dgvTurnos
            // 
            dgvTurnos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dgvTurnos.BackgroundColor = Color.White;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvTurnos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvTurnos.EnableHeadersVisualStyles = false;
            dgvTurnos.Location = new Point(20, 48);
            dgvTurnos.Name = "dgvTurnos";
            dgvTurnos.RowTemplate.Height = 26;
            dgvTurnos.Size = new Size(616, 430);
            dgvTurnos.TabIndex = 1;
            dgvTurnos.CellFormatting += dgvTurnos_CellFormatting;
            // 
            // pnlAtencionActual
            // 
            pnlAtencionActual.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlAtencionActual.BackColor = Color.White;
            pnlAtencionActual.BorderStyle = BorderStyle.FixedSingle;
            pnlAtencionActual.Controls.Add(lblPanelTitulo);
            pnlAtencionActual.Controls.Add(lblInfoTurno);
            pnlAtencionActual.Controls.Add(lblInfoPaciente);
            pnlAtencionActual.Controls.Add(lblInfoDni);
            pnlAtencionActual.Controls.Add(lblInfoMotivo);
            pnlAtencionActual.Controls.Add(lblInfoPrioridadValor);
            pnlAtencionActual.Controls.Add(lblInfoTiempo);
            pnlAtencionActual.Controls.Add(lblObservaciones);
            pnlAtencionActual.Controls.Add(lblDiagnostico);
            pnlAtencionActual.Controls.Add(txtDiagnostico);
            pnlAtencionActual.Location = new Point(656, 10);
            pnlAtencionActual.Name = "pnlAtencionActual";
            pnlAtencionActual.Size = new Size(340, 380);
            pnlAtencionActual.TabIndex = 2;
            // 
            // lblPanelTitulo
            // 
            lblPanelTitulo.AutoSize = true;
            lblPanelTitulo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblPanelTitulo.Location = new Point(16, 10);
            lblPanelTitulo.Name = "lblPanelTitulo";
            lblPanelTitulo.Size = new Size(184, 20);
            lblPanelTitulo.TabIndex = 0;
            lblPanelTitulo.Text = "Panel de Atención Actual";
            // 
            // lblInfoTurno
            // 
            lblInfoTurno.AutoSize = true;
            lblInfoTurno.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblInfoTurno.Location = new Point(16, 38);
            lblInfoTurno.Name = "lblInfoTurno";
            lblInfoTurno.Size = new Size(76, 17);
            lblInfoTurno.TabIndex = 1;
            lblInfoTurno.Text = "N° Turno: -";
            // 
            // lblInfoPaciente
            // 
            lblInfoPaciente.AutoSize = true;
            lblInfoPaciente.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblInfoPaciente.Location = new Point(16, 60);
            lblInfoPaciente.Name = "lblInfoPaciente";
            lblInfoPaciente.Size = new Size(73, 17);
            lblInfoPaciente.TabIndex = 2;
            lblInfoPaciente.Text = "Paciente: -";
            // 
            // lblInfoDni
            // 
            lblInfoDni.AutoSize = true;
            lblInfoDni.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblInfoDni.Location = new Point(16, 82);
            lblInfoDni.Name = "lblInfoDni";
            lblInfoDni.Size = new Size(164, 17);
            lblInfoDni.TabIndex = 3;
            lblInfoDni.Text = "DNI / Edad / Cobertura: -";
            // 
            // lblInfoMotivo
            // 
            lblInfoMotivo.AutoSize = true;
            lblInfoMotivo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblInfoMotivo.Location = new Point(16, 104);
            lblInfoMotivo.Name = "lblInfoMotivo";
            lblInfoMotivo.Size = new Size(136, 17);
            lblInfoMotivo.TabIndex = 4;
            lblInfoMotivo.Text = "Motivo / Prioridad: -";
            // 
            // lblInfoPrioridadValor
            // 
            lblInfoPrioridadValor.AutoSize = true;
            lblInfoPrioridadValor.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblInfoPrioridadValor.ForeColor = Color.FromArgb(214, 39, 40);
            lblInfoPrioridadValor.Location = new Point(170, 104);
            lblInfoPrioridadValor.Name = "lblInfoPrioridadValor";
            lblInfoPrioridadValor.Size = new Size(0, 17);
            lblInfoPrioridadValor.TabIndex = 5;
            // 
            // lblInfoTiempo
            // 
            lblInfoTiempo.AutoSize = true;
            lblInfoTiempo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblInfoTiempo.Location = new Point(16, 126);
            lblInfoTiempo.Name = "lblInfoTiempo";
            lblInfoTiempo.Size = new Size(182, 17);
            lblInfoTiempo.TabIndex = 6;
            lblInfoTiempo.Text = "Hora de Entrada / Tiempo: -";
            // 
            // lblObservaciones
            // 
            lblObservaciones.AutoSize = true;
            lblObservaciones.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblObservaciones.Location = new Point(16, 154);
            lblObservaciones.Name = "lblObservaciones";
            lblObservaciones.Size = new Size(136, 15);
            lblObservaciones.TabIndex = 7;
            lblObservaciones.Text = "Observaciones Médicas";
            lblObservaciones.Click += lblObservaciones_Click;
            // 
            // lblDiagnostico
            // 
            lblDiagnostico.AutoSize = true;
            lblDiagnostico.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDiagnostico.Location = new Point(16, 187);
            lblDiagnostico.Name = "lblDiagnostico";
            lblDiagnostico.Size = new Size(113, 15);
            lblDiagnostico.TabIndex = 9;
            lblDiagnostico.Text = "Diagnóstico Rápido";
            // 
            // txtDiagnostico
            // 
            txtDiagnostico.Location = new Point(16, 207);
            txtDiagnostico.Multiline = true;
            txtDiagnostico.Name = "txtDiagnostico";
            txtDiagnostico.ScrollBars = ScrollBars.Vertical;
            txtDiagnostico.Size = new Size(308, 70);
            txtDiagnostico.TabIndex = 1;
            // 
            // btnSiguientePaciente
            // 
            btnSiguientePaciente.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSiguientePaciente.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSiguientePaciente.Location = new Point(656, 400);
            btnSiguientePaciente.Name = "btnSiguientePaciente";
            btnSiguientePaciente.Size = new Size(340, 38);
            btnSiguientePaciente.TabIndex = 3;
            btnSiguientePaciente.Text = "➜  Siguiente Paciente";
            btnSiguientePaciente.UseVisualStyleBackColor = true;
            btnSiguientePaciente.Click += btnSiguientePaciente_Click;
            // 
            // btnIniciarAtencion
            // 
            btnIniciarAtencion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnIniciarAtencion.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnIniciarAtencion.ForeColor = Color.FromArgb(21, 101, 178);
            btnIniciarAtencion.Location = new Point(656, 442);
            btnIniciarAtencion.Name = "btnIniciarAtencion";
            btnIniciarAtencion.Size = new Size(340, 38);
            btnIniciarAtencion.TabIndex = 4;
            btnIniciarAtencion.Text = "🕐  Iniciar Atención";
            btnIniciarAtencion.UseVisualStyleBackColor = true;
            btnIniciarAtencion.Click += btnIniciarAtencion_Click;
            // 
            // btnTerminarAtencion
            // 
            btnTerminarAtencion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTerminarAtencion.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnTerminarAtencion.Location = new Point(656, 484);
            btnTerminarAtencion.Name = "btnTerminarAtencion";
            btnTerminarAtencion.Size = new Size(340, 38);
            btnTerminarAtencion.TabIndex = 5;
            btnTerminarAtencion.Text = "☑  Terminar Atención";
            btnTerminarAtencion.UseVisualStyleBackColor = true;
            btnTerminarAtencion.Click += btnTerminarAtencion_Click;
            // 
            // lblAvisoBloqueo
            // 
            lblAvisoBloqueo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAvisoBloqueo.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            lblAvisoBloqueo.ForeColor = Color.DimGray;
            lblAvisoBloqueo.Location = new Point(656, 526);
            lblAvisoBloqueo.Name = "lblAvisoBloqueo";
            lblAvisoBloqueo.Size = new Size(340, 16);
            lblAvisoBloqueo.TabIndex = 6;
            lblAvisoBloqueo.Text = "Atención en curso, cambio de servicio bloqueado.";
            lblAvisoBloqueo.TextAlign = ContentAlignment.MiddleRight;
            lblAvisoBloqueo.Visible = false;
            // 
            // lblTrazabilidad
            // 
            lblTrazabilidad.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTrazabilidad.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            lblTrazabilidad.ForeColor = Color.DimGray;
            lblTrazabilidad.Location = new Point(656, 544);
            lblTrazabilidad.Name = "lblTrazabilidad";
            lblTrazabilidad.Size = new Size(340, 16);
            lblTrazabilidad.TabIndex = 7;
            lblTrazabilidad.Text = "Trazabilidad: Dr. Juan Pérez | Consultorio 3";
            lblTrazabilidad.TextAlign = ContentAlignment.MiddleRight;
            // 
            // pnlEstado
            // 
            pnlEstado.BackColor = Color.FromArgb(230, 230, 230);
            pnlEstado.Controls.Add(lblEstadoInferior);
            pnlEstado.Dock = DockStyle.Bottom;
            pnlEstado.Location = new Point(0, 674);
            pnlEstado.Name = "pnlEstado";
            pnlEstado.Size = new Size(1030, 26);
            pnlEstado.TabIndex = 3;
            // 
            // lblEstadoInferior
            // 
            lblEstadoInferior.AutoSize = true;
            lblEstadoInferior.Location = new Point(10, 6);
            lblEstadoInferior.Name = "lblEstadoInferior";
            lblEstadoInferior.Size = new Size(150, 15);
            lblEstadoInferior.TabIndex = 0;
            lblEstadoInferior.Text = "Total pacientes en espera: 0";
            // 
            // FrmListaTurnosAtencion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1030, 700);
            Controls.Add(pnlContenido);
            Controls.Add(pnlEstado);
            Controls.Add(pnlSubHeader);
            Controls.Add(pnlHeader);
            MinimumSize = new Size(1046, 700);
            Name = "FrmListaTurnosAtencion";
            Text = "Lista Turnos Atencion";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlSubHeader.ResumeLayout(false);
            pnlSubHeader.PerformLayout();
            pnlContenido.ResumeLayout(false);
            pnlContenido.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTurnos).EndInit();
            pnlAtencionActual.ResumeLayout(false);
            pnlAtencionActual.PerformLayout();
            pnlEstado.ResumeLayout(false);
            pnlEstado.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrTiempoTranscurrido;

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTituloPrincipal;

        private System.Windows.Forms.Panel pnlSubHeader;
        private System.Windows.Forms.Label lblMedicoInfo;

        private System.Windows.Forms.Panel pnlContenido;
        private System.Windows.Forms.Label lblServicio;
        private System.Windows.Forms.ComboBox cboServicio;
        private System.Windows.Forms.DataGridView dgvTurnos;

        private System.Windows.Forms.Panel pnlAtencionActual;
        private System.Windows.Forms.Label lblPanelTitulo;
        private System.Windows.Forms.Label lblInfoTurno;
        private System.Windows.Forms.Label lblInfoPaciente;
        private System.Windows.Forms.Label lblInfoDni;
        private System.Windows.Forms.Label lblInfoMotivo;
        private System.Windows.Forms.Label lblInfoPrioridadValor;
        private System.Windows.Forms.Label lblInfoTiempo;
        private System.Windows.Forms.Label lblObservaciones;
        // txtObservaciones removed
        private System.Windows.Forms.Label lblDiagnostico;
        private System.Windows.Forms.TextBox txtDiagnostico;

        private System.Windows.Forms.Button btnSiguientePaciente;
        private System.Windows.Forms.Button btnIniciarAtencion;
        private System.Windows.Forms.Button btnTerminarAtencion;
        private System.Windows.Forms.Label lblAvisoBloqueo;
        private System.Windows.Forms.Label lblTrazabilidad;

        private System.Windows.Forms.Panel pnlEstado;
        private System.Windows.Forms.Label lblEstadoInferior;
    }
}