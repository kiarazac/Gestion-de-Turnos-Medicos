namespace Gestion_de_Turnos_Medicos
{
    partial class FrmTurnoEspecialidad
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
            panel1 = new Panel();
            LNombrePantalla = new Label();
            LNuevoPaciente2 = new Label();
            LNuevoPaciente = new Label();
            panel2 = new Panel();
            LEspecialidad = new Label();
            cmbEspecialidad = new ComboBox();
            Condicionales = new GroupBox();
            cmbHorarios = new ComboBox();
            calFechaTurno = new MonthCalendar();
            label7 = new Label();
            button1 = new Button();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            txtDNI = new TextBox();
            label3 = new Label();
            txtApellido = new TextBox();
            label2 = new Label();
            txtNombre = new TextBox();
            label1 = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            Condicionales.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Highlight;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(LNombrePantalla);
            panel1.Location = new Point(-157, -3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1198, 63);
            panel1.TabIndex = 1;
            // 
            // LNombrePantalla
            // 
            LNombrePantalla.AutoSize = true;
            LNombrePantalla.Font = new Font("Britannic Bold", 24.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LNombrePantalla.ForeColor = SystemColors.ButtonHighlight;
            LNombrePantalla.Location = new Point(233, 10);
            LNombrePantalla.Name = "LNombrePantalla";
            LNombrePantalla.Size = new Size(446, 37);
            LNombrePantalla.TabIndex = 0;
            LNombrePantalla.Text = "Turnos Sector Especialidades";
            // 
            // LNuevoPaciente2
            // 
            LNuevoPaciente2.AutoSize = true;
            LNuevoPaciente2.ForeColor = SystemColors.Highlight;
            LNuevoPaciente2.Location = new Point(80, 133);
            LNuevoPaciente2.Name = "LNuevoPaciente2";
            LNuevoPaciente2.Size = new Size(241, 15);
            LNuevoPaciente2.TabIndex = 5;
            LNuevoPaciente2.Text = "Complete los datos para registrar al paciente";
            // 
            // LNuevoPaciente
            // 
            LNuevoPaciente.AutoSize = true;
            LNuevoPaciente.Font = new Font("Arial Black", 18F, FontStyle.Bold);
            LNuevoPaciente.ForeColor = SystemColors.ActiveCaptionText;
            LNuevoPaciente.Location = new Point(80, 88);
            LNuevoPaciente.Name = "LNuevoPaciente";
            LNuevoPaciente.Size = new Size(220, 33);
            LNuevoPaciente.TabIndex = 4;
            LNuevoPaciente.Text = "Nuevo Paciente";
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(LEspecialidad);
            panel2.Controls.Add(cmbEspecialidad);
            panel2.Controls.Add(Condicionales);
            panel2.Controls.Add(txtDNI);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(txtApellido);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(txtNombre);
            panel2.Controls.Add(label1);
            panel2.Location = new Point(29, 166);
            panel2.Name = "panel2";
            panel2.Size = new Size(740, 494);
            panel2.TabIndex = 6;
            // 
            // LEspecialidad
            // 
            LEspecialidad.AutoSize = true;
            LEspecialidad.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            LEspecialidad.ForeColor = SystemColors.InactiveCaptionText;
            LEspecialidad.Location = new Point(390, 76);
            LEspecialidad.Name = "LEspecialidad";
            LEspecialidad.Size = new Size(106, 21);
            LEspecialidad.TabIndex = 19;
            LEspecialidad.Text = "Especialidad";
            // 
            // cmbEspecialidad
            // 
            cmbEspecialidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEspecialidad.FormattingEnabled = true;
            cmbEspecialidad.Location = new Point(387, 110);
            cmbEspecialidad.Name = "cmbEspecialidad";
            cmbEspecialidad.Size = new Size(268, 23);
            cmbEspecialidad.TabIndex = 18;
            cmbEspecialidad.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // Condicionales
            // 
            Condicionales.Controls.Add(cmbHorarios);
            Condicionales.Controls.Add(calFechaTurno);
            Condicionales.Controls.Add(label7);
            Condicionales.Controls.Add(button1);
            Condicionales.Controls.Add(label6);
            Condicionales.Controls.Add(label5);
            Condicionales.Controls.Add(label4);
            Condicionales.Location = new Point(31, 158);
            Condicionales.Name = "Condicionales";
            Condicionales.Size = new Size(687, 322);
            Condicionales.TabIndex = 10;
            Condicionales.TabStop = false;
            Condicionales.Text = "Especificación Fecha y Hora";
            // 
            // cmbHorarios
            // 
            cmbHorarios.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHorarios.FormattingEnabled = true;
            cmbHorarios.Location = new Point(356, 180);
            cmbHorarios.Name = "cmbHorarios";
            cmbHorarios.Size = new Size(268, 23);
            cmbHorarios.TabIndex = 17;
            // 
            // calFechaTurno
            // 
            calFechaTurno.Location = new Point(43, 133);
            calFechaTurno.Name = "calFechaTurno";
            calFechaTurno.TabIndex = 16;
            calFechaTurno.DateChanged += calFechaTurno_DateChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 10F);
            label7.ForeColor = Color.Navy;
            label7.Location = new Point(359, 135);
            label7.MaximumSize = new Size(325, 0);
            label7.Name = "label7";
            label7.Size = new Size(265, 19);
            label7.TabIndex = 15;
            label7.Text = "Seleccione uno de los horarios disponibles";
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
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 10F);
            label6.ForeColor = Color.Navy;
            label6.Location = new Point(28, 64);
            label6.MaximumSize = new Size(325, 0);
            label6.Name = "label6";
            label6.Size = new Size(325, 38);
            label6.TabIndex = 4;
            label6.Text = "Seleccione una de las fechas resaltadas en negro del Calendario";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label5.ForeColor = SystemColors.ControlText;
            label5.Location = new Point(452, 103);
            label5.Name = "label5";
            label5.Size = new Size(68, 21);
            label5.TabIndex = 10;
            label5.Text = "Horario";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label4.ForeColor = SystemColors.ControlText;
            label4.Location = new Point(28, 34);
            label4.Name = "label4";
            label4.Size = new Size(130, 21);
            label4.TabIndex = 9;
            label4.Text = "Fecha del Turno";
            // 
            // txtDNI
            // 
            txtDNI.Location = new Point(33, 110);
            txtDNI.Name = "txtDNI";
            txtDNI.Size = new Size(326, 23);
            txtDNI.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label3.ForeColor = SystemColors.InactiveCaptionText;
            label3.Location = new Point(33, 76);
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
            txtNombre.Location = new Point(33, 50);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(326, 23);
            txtNombre.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.ForeColor = SystemColors.ControlText;
            label1.Location = new Point(33, 16);
            label1.Name = "label1";
            label1.Size = new Size(73, 21);
            label1.TabIndex = 3;
            label1.Text = "Nombre";
            // 
            // FrmTurnoEspecialidad
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1025, 668);
            Controls.Add(panel2);
            Controls.Add(LNuevoPaciente2);
            Controls.Add(LNuevoPaciente);
            Controls.Add(panel1);
            Name = "FrmTurnoEspecialidad";
            Text = "FrmTurnoEspecialidad";
            Load += FrmTurnoEspecialidad_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            Condicionales.ResumeLayout(false);
            Condicionales.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label LNombrePantalla;
        private Label LNuevoPaciente2;
        private Label LNuevoPaciente;
        private Panel panel2;
        private GroupBox Condicionales;
        private Label label7;
        private Button button1;
        private Label label6;
        private Label label5;
        private Label label4;
        private TextBox txtDNI;
        private Label label3;
        private TextBox txtApellido;
        private Label label2;
        private TextBox txtNombre;
        private Label label1;
        private MonthCalendar calFechaTurno;
        private ComboBox cmbHorarios;
        private Label LEspecialidad;
        private ComboBox cmbEspecialidad;
    }
}