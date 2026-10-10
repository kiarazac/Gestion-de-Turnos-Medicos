namespace Gestion_de_Turnos_Medicos
{
    partial class FrmTurnoEmergencia
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTurnoEmergencia));
            panel1 = new Panel();
            LNombrePantalla = new Label();
            LNuevoPaciente = new Label();
            LNuevoPaciente2 = new Label();
            panel2 = new Panel();
            cmbObraSocial = new ComboBox();
            LObraSocial = new Label();
            Condicionales = new GroupBox();
            button1 = new Button();
            checkBoxBaja = new CheckBox();
            checkedListMedia = new CheckedListBox();
            checkedListAlta = new CheckedListBox();
            label6 = new Label();
            label4 = new Label();
            txtDNI = new TextBox();
            label3 = new Label();
            txtApellido = new TextBox();
            label2 = new Label();
            txtNombre = new TextBox();
            label1 = new Label();
            LdescripTurno = new Label();
            panel3 = new Panel();
            Ldescrip_turno_especialidad = new Label();
            Lid_turno = new Label();
            lblMontoCobro = new Label();
            btnDescargarTxt = new Button();
            gbCancelacionEmergencia = new GroupBox();
            txtBuscarTurnoCancelacion = new TextBox();
            btnBuscarTurnoCancelacion = new Button();
            lblTurnoInfoPaciente = new Label();
            lblTurnoInfoTriage = new Label();
            lblClaveCancelacionEmergencia = new Label();
            txtClaveCancelacionEmergencia = new TextBox();
            btnConfirmarCancelacionEmergencia = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            Condicionales.SuspendLayout();
            panel3.SuspendLayout();
            gbCancelacionEmergencia.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Highlight;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(LNombrePantalla);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1115, 58);
            panel1.TabIndex = 0;
            // 
            // LNombrePantalla
            // 
            LNombrePantalla.AutoSize = true;
            LNombrePantalla.Font = new Font("Britannic Bold", 24.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LNombrePantalla.Location = new Point(80, 9);
            LNombrePantalla.Name = "LNombrePantalla";
            LNombrePantalla.Size = new Size(447, 37);
            LNombrePantalla.TabIndex = 0;
            LNombrePantalla.Text = "Turnos Sector de Emergencia";
            // 
            // LNuevoPaciente
            // 
            LNuevoPaciente.AutoSize = true;
            LNuevoPaciente.Font = new Font("Arial Black", 18F, FontStyle.Bold);
            LNuevoPaciente.ForeColor = SystemColors.ActiveCaptionText;
            LNuevoPaciente.Location = new Point(80, 84);
            LNuevoPaciente.Name = "LNuevoPaciente";
            LNuevoPaciente.Size = new Size(220, 33);
            LNuevoPaciente.TabIndex = 1;
            LNuevoPaciente.Text = "Nuevo Paciente";
            // 
            // LNuevoPaciente2
            // 
            LNuevoPaciente2.AutoSize = true;
            LNuevoPaciente2.ForeColor = SystemColors.Highlight;
            LNuevoPaciente2.Location = new Point(80, 129);
            LNuevoPaciente2.Name = "LNuevoPaciente2";
            LNuevoPaciente2.Size = new Size(241, 15);
            LNuevoPaciente2.TabIndex = 2;
            LNuevoPaciente2.Text = "Complete los datos para registrar al paciente";
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(cmbObraSocial);
            panel2.Controls.Add(LObraSocial);
            panel2.Controls.Add(Condicionales);
            panel2.Controls.Add(txtDNI);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(txtApellido);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(txtNombre);
            panel2.Controls.Add(label1);
            panel2.Location = new Point(36, 156);
            panel2.Name = "panel2";
            panel2.Size = new Size(740, 494);
            panel2.TabIndex = 7;
            // 
            // cmbObraSocial
            // 
            cmbObraSocial.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbObraSocial.Font = new Font("Segoe UI", 10F);
            cmbObraSocial.Location = new Point(373, 110);
            cmbObraSocial.Name = "cmbObraSocial";
            cmbObraSocial.Size = new Size(326, 25);
            cmbObraSocial.TabIndex = 12;
            // 
            // LObraSocial
            // 
            LObraSocial.AutoSize = true;
            LObraSocial.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            LObraSocial.ForeColor = SystemColors.InactiveCaptionText;
            LObraSocial.Location = new Point(373, 76);
            LObraSocial.Name = "LObraSocial";
            LObraSocial.Size = new Size(97, 21);
            LObraSocial.TabIndex = 11;
            LObraSocial.Text = "Obra Social";
            // 
            // Condicionales
            // 
            Condicionales.Controls.Add(button1);
            Condicionales.Controls.Add(checkBoxBaja);
            Condicionales.Controls.Add(checkedListMedia);
            Condicionales.Controls.Add(checkedListAlta);
            Condicionales.Controls.Add(label6);
            Condicionales.Controls.Add(label4);
            Condicionales.Location = new Point(31, 158);
            Condicionales.Name = "Condicionales";
            Condicionales.Size = new Size(687, 331);
            Condicionales.TabIndex = 10;
            Condicionales.TabStop = false;
            Condicionales.Text = "Especificación de Condiciones ";
            // 
            // button1
            // 
            button1.BackColor = Color.SteelBlue;
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ButtonHighlight;
            button1.Location = new Point(507, 265);
            button1.Name = "button1";
            button1.Size = new Size(145, 49);
            button1.TabIndex = 14;
            button1.Text = "GENERAR TURNO";
            button1.UseVisualStyleBackColor = false;
            // 
            // checkBoxBaja
            // 
            checkBoxBaja.AutoSize = true;
            checkBoxBaja.BackColor = Color.MediumTurquoise;
            checkBoxBaja.ForeColor = SystemColors.ActiveCaptionText;
            checkBoxBaja.Location = new Point(477, 132);
            checkBoxBaja.Name = "checkBoxBaja";
            checkBoxBaja.Size = new Size(159, 19);
            checkBoxBaja.TabIndex = 11;
            checkBoxBaja.Text = "Otro (No es de gravedad)";
            checkBoxBaja.UseVisualStyleBackColor = false;
            // 
            // checkedListMedia
            // 
            checkedListMedia.BackColor = Color.Moccasin;
            checkedListMedia.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkedListMedia.FormattingEnabled = true;
            checkedListMedia.Location = new Point(28, 199);
            checkedListMedia.Name = "checkedListMedia";
            checkedListMedia.Size = new Size(389, 76);
            checkedListMedia.TabIndex = 12;
            // 
            // checkedListAlta
            // 
            checkedListAlta.BackColor = Color.MistyRose;
            checkedListAlta.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkedListAlta.FormattingEnabled = true;
            checkedListAlta.Location = new Point(28, 117);
            checkedListAlta.Name = "checkedListAlta";
            checkedListAlta.Size = new Size(389, 76);
            checkedListAlta.TabIndex = 11;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 10F);
            label6.ForeColor = Color.Navy;
            label6.Location = new Point(28, 64);
            label6.MaximumSize = new Size(325, 0);
            label6.Name = "label6";
            label6.Size = new Size(318, 38);
            label6.TabIndex = 4;
            label6.Text = "Seleccione uno o más síntomas del paciente (Solo se muestran síntomas de prioridad MEDIA o ALTA)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label4.ForeColor = SystemColors.ControlText;
            label4.Location = new Point(28, 34);
            label4.Name = "label4";
            label4.Size = new Size(173, 21);
            label4.TabIndex = 9;
            label4.Text = "Síntomas Principales ";
            // 
            // txtDNI
            // 
            txtDNI.Location = new Point(33, 50);
            txtDNI.Name = "txtDNI";
            txtDNI.Size = new Size(326, 23);
            txtDNI.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label3.ForeColor = SystemColors.InactiveCaptionText;
            label3.Location = new Point(43, 16);
            label3.Name = "label3";
            label3.Size = new Size(40, 21);
            label3.TabIndex = 7;
            label3.Text = "DNI";
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(373, 50);
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(326, 23);
            txtApellido.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label2.ForeColor = SystemColors.InactiveCaptionText;
            label2.Location = new Point(373, 16);
            label2.Name = "label2";
            label2.Size = new Size(75, 21);
            label2.TabIndex = 5;
            label2.Text = "Apellido";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(31, 110);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(326, 23);
            txtNombre.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.ForeColor = SystemColors.ControlText;
            label1.Location = new Point(33, 86);
            label1.Name = "label1";
            label1.Size = new Size(73, 21);
            label1.TabIndex = 3;
            label1.Text = "Nombre";
            // 
            // LdescripTurno
            // 
            LdescripTurno.AutoSize = true;
            LdescripTurno.Font = new Font("Arial Black", 18F, FontStyle.Bold);
            LdescripTurno.ForeColor = SystemColors.ControlDarkDark;
            LdescripTurno.Location = new Point(890, 204);
            LdescripTurno.Name = "LdescripTurno";
            LdescripTurno.Size = new Size(226, 33);
            LdescripTurno.TabIndex = 10;
            LdescripTurno.Text = "Turno Generado";
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.ActiveCaption;
            panel3.BorderStyle = BorderStyle.Fixed3D;
            panel3.Controls.Add(lblMontoCobro);
            panel3.Controls.Add(Ldescrip_turno_especialidad);
            panel3.Controls.Add(Lid_turno);
            panel3.Location = new Point(814, 255);
            panel3.Name = "panel3";
            panel3.Size = new Size(367, 191);
            panel3.TabIndex = 9;
            // 
            // Lid_turno
            // 
            Lid_turno.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            Lid_turno.ForeColor = SystemColors.ControlText;
            Lid_turno.Location = new Point(10, 10);
            Lid_turno.Name = "Lid_turno";
            Lid_turno.Size = new Size(345, 48);
            Lid_turno.TabIndex = 18;
            Lid_turno.Text = "# --------";
            Lid_turno.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Ldescrip_turno_especialidad
            // 
            Ldescrip_turno_especialidad.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Ldescrip_turno_especialidad.ForeColor = SystemColors.Highlight;
            Ldescrip_turno_especialidad.Location = new Point(10, 60);
            Ldescrip_turno_especialidad.Name = "Ldescrip_turno_especialidad";
            Ldescrip_turno_especialidad.Size = new Size(345, 32);
            Ldescrip_turno_especialidad.TabIndex = 19;
            Ldescrip_turno_especialidad.Text = "Especialidad";
            Ldescrip_turno_especialidad.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblMontoCobro
            // 
            lblMontoCobro.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblMontoCobro.ForeColor = Color.DarkGreen;
            lblMontoCobro.Location = new Point(10, 102);
            lblMontoCobro.Name = "lblMontoCobro";
            lblMontoCobro.Size = new Size(345, 78);
            lblMontoCobro.TabIndex = 22;
            lblMontoCobro.Text = "Arancel: $ --";
            lblMontoCobro.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnDescargarTxt
            // 
            btnDescargarTxt.BackColor = Color.SeaGreen;
            btnDescargarTxt.Enabled = false;
            btnDescargarTxt.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnDescargarTxt.ForeColor = SystemColors.ButtonHighlight;
            btnDescargarTxt.Location = new Point(814, 460);
            btnDescargarTxt.Name = "btnDescargarTxt";
            btnDescargarTxt.Size = new Size(367, 45);
            btnDescargarTxt.TabIndex = 20;
            btnDescargarTxt.Text = "DESCARGAR COMPROBANTE (.TXT)";
            btnDescargarTxt.UseVisualStyleBackColor = false;
            // 
            // gbCancelacionEmergencia
            // 
            gbCancelacionEmergencia.Controls.Add(btnConfirmarCancelacionEmergencia);
            gbCancelacionEmergencia.Controls.Add(txtClaveCancelacionEmergencia);
            gbCancelacionEmergencia.Controls.Add(lblClaveCancelacionEmergencia);
            gbCancelacionEmergencia.Controls.Add(lblTurnoInfoTriage);
            gbCancelacionEmergencia.Controls.Add(lblTurnoInfoPaciente);
            gbCancelacionEmergencia.Controls.Add(btnBuscarTurnoCancelacion);
            gbCancelacionEmergencia.Controls.Add(txtBuscarTurnoCancelacion);
            gbCancelacionEmergencia.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbCancelacionEmergencia.ForeColor = SystemColors.ControlText;
            gbCancelacionEmergencia.Location = new Point(814, 515);
            gbCancelacionEmergencia.Name = "gbCancelacionEmergencia";
            gbCancelacionEmergencia.Size = new Size(367, 185);
            gbCancelacionEmergencia.TabIndex = 21;
            gbCancelacionEmergencia.TabStop = false;
            gbCancelacionEmergencia.Text = "Cancelación Ágil de Turno de Guardia (2FA)";
            // 
            // txtBuscarTurnoCancelacion
            // 
            txtBuscarTurnoCancelacion.CharacterCasing = CharacterCasing.Upper;
            txtBuscarTurnoCancelacion.Font = new Font("Segoe UI", 9F);
            txtBuscarTurnoCancelacion.Location = new Point(12, 22);
            txtBuscarTurnoCancelacion.Name = "txtBuscarTurnoCancelacion";
            txtBuscarTurnoCancelacion.PlaceholderText = "N° Orden (E-001) o DNI...";
            txtBuscarTurnoCancelacion.Size = new Size(230, 23);
            txtBuscarTurnoCancelacion.TabIndex = 0;
            // 
            // btnBuscarTurnoCancelacion
            // 
            btnBuscarTurnoCancelacion.BackColor = Color.SteelBlue;
            btnBuscarTurnoCancelacion.Cursor = Cursors.Hand;
            btnBuscarTurnoCancelacion.FlatStyle = FlatStyle.Flat;
            btnBuscarTurnoCancelacion.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnBuscarTurnoCancelacion.ForeColor = SystemColors.ButtonHighlight;
            btnBuscarTurnoCancelacion.Location = new Point(248, 21);
            btnBuscarTurnoCancelacion.Name = "btnBuscarTurnoCancelacion";
            btnBuscarTurnoCancelacion.Size = new Size(107, 26);
            btnBuscarTurnoCancelacion.TabIndex = 1;
            btnBuscarTurnoCancelacion.Text = "BUSCAR";
            btnBuscarTurnoCancelacion.UseVisualStyleBackColor = false;
            // 
            // lblTurnoInfoPaciente
            // 
            lblTurnoInfoPaciente.AutoSize = true;
            lblTurnoInfoPaciente.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTurnoInfoPaciente.ForeColor = SystemColors.ControlText;
            lblTurnoInfoPaciente.Location = new Point(12, 51);
            lblTurnoInfoPaciente.Name = "lblTurnoInfoPaciente";
            lblTurnoInfoPaciente.Size = new Size(130, 15);
            lblTurnoInfoPaciente.TabIndex = 2;
            lblTurnoInfoPaciente.Text = "Paciente: (Sin búsqueda)";
            // 
            // lblTurnoInfoTriage
            // 
            lblTurnoInfoTriage.AutoSize = true;
            lblTurnoInfoTriage.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTurnoInfoTriage.ForeColor = Color.MidnightBlue;
            lblTurnoInfoTriage.Location = new Point(12, 70);
            lblTurnoInfoTriage.Name = "lblTurnoInfoTriage";
            lblTurnoInfoTriage.Size = new Size(147, 15);
            lblTurnoInfoTriage.TabIndex = 3;
            lblTurnoInfoTriage.Text = "Triage: -- | Estado: --";
            // 
            // lblClaveCancelacionEmergencia
            // 
            lblClaveCancelacionEmergencia.AutoSize = true;
            lblClaveCancelacionEmergencia.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblClaveCancelacionEmergencia.ForeColor = Color.DarkRed;
            lblClaveCancelacionEmergencia.Location = new Point(12, 92);
            lblClaveCancelacionEmergencia.Name = "lblClaveCancelacionEmergencia";
            lblClaveCancelacionEmergencia.Size = new Size(153, 15);
            lblClaveCancelacionEmergencia.TabIndex = 4;
            lblClaveCancelacionEmergencia.Text = "Clave 2FA (Ticket):";
            // 
            // txtClaveCancelacionEmergencia
            // 
            txtClaveCancelacionEmergencia.CharacterCasing = CharacterCasing.Upper;
            txtClaveCancelacionEmergencia.Font = new Font("Segoe UI", 9F);
            txtClaveCancelacionEmergencia.Location = new Point(12, 110);
            txtClaveCancelacionEmergencia.Name = "txtClaveCancelacionEmergencia";
            txtClaveCancelacionEmergencia.PlaceholderText = "Ingrese código 2FA (ej. CAN-XXXX)";
            txtClaveCancelacionEmergencia.Size = new Size(343, 23);
            txtClaveCancelacionEmergencia.TabIndex = 5;
            // 
            // btnConfirmarCancelacionEmergencia
            // 
            btnConfirmarCancelacionEmergencia.BackColor = Color.Crimson;
            btnConfirmarCancelacionEmergencia.Cursor = Cursors.Hand;
            btnConfirmarCancelacionEmergencia.FlatStyle = FlatStyle.Flat;
            btnConfirmarCancelacionEmergencia.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnConfirmarCancelacionEmergencia.ForeColor = SystemColors.ButtonHighlight;
            btnConfirmarCancelacionEmergencia.Location = new Point(12, 140);
            btnConfirmarCancelacionEmergencia.Name = "btnConfirmarCancelacionEmergencia";
            btnConfirmarCancelacionEmergencia.Size = new Size(343, 34);
            btnConfirmarCancelacionEmergencia.TabIndex = 6;
            btnConfirmarCancelacionEmergencia.Text = "CANCELAR TURNO (2FA)";
            btnConfirmarCancelacionEmergencia.UseVisualStyleBackColor = false;
            // 
            // FrmTurnoEmergencia
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1195, 715);
            Controls.Add(gbCancelacionEmergencia);
            Controls.Add(btnDescargarTxt);
            Controls.Add(LdescripTurno);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(LNuevoPaciente2);
            Controls.Add(LNuevoPaciente);
            Controls.Add(panel1);
            ForeColor = SystemColors.ButtonFace;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmTurnoEmergencia";
            Text = "FrmTurnoEmergencia";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            Condicionales.ResumeLayout(false);
            Condicionales.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            gbCancelacionEmergencia.ResumeLayout(false);
            gbCancelacionEmergencia.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label LNombrePantalla;
        private Label LNuevoPaciente;
        private Label LNuevoPaciente2;
        private Panel panel2;
        private GroupBox Condicionales;
        private Button button1;
        private CheckBox checkBoxBaja;
        private CheckedListBox checkedListMedia;
        private CheckedListBox checkedListAlta;
        private Label label6;
        private Label label4;
        private TextBox txtDNI;
        private Label label3;
        private TextBox txtApellido;
        private Label label2;
        private TextBox txtNombre;
        private Label label1;
        private ComboBox cmbObraSocial;
        private Label LObraSocial;
        private Label LdescripTurno;
        private Panel panel3;
        private Label Ldescrip_turno_especialidad;
        private Label Lid_turno;
        private Label lblMontoCobro;
        private Button btnDescargarTxt;
        private GroupBox gbCancelacionEmergencia;
        private TextBox txtBuscarTurnoCancelacion;
        private Button btnBuscarTurnoCancelacion;
        private Label lblTurnoInfoPaciente;
        private Label lblTurnoInfoTriage;
        private Label lblClaveCancelacionEmergencia;
        private TextBox txtClaveCancelacionEmergencia;
        private Button btnConfirmarCancelacionEmergencia;
    }
}