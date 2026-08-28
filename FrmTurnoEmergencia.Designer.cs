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
            Condicionales = new GroupBox();
            button1 = new Button();
            checkBox1 = new CheckBox();
            checkedListBox3 = new CheckedListBox();
            checkedListBox2 = new CheckedListBox();
            checkedListBox1 = new CheckedListBox();
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
            panel2.Controls.Add(Condicionales);
            panel2.Controls.Add(txtDNI);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(txtApellido);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(txtNombre);
            panel2.Controls.Add(label1);
            panel2.Location = new Point(33, 156);
            panel2.Name = "panel2";
            panel2.Size = new Size(740, 494);
            panel2.TabIndex = 3;
            // 
            // Condicionales
            // 
            Condicionales.Controls.Add(button1);
            Condicionales.Controls.Add(checkBox1);
            Condicionales.Controls.Add(checkedListBox3);
            Condicionales.Controls.Add(checkedListBox2);
            Condicionales.Controls.Add(checkedListBox1);
            Condicionales.Controls.Add(label6);
            Condicionales.Controls.Add(label5);
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
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.BackColor = Color.MediumTurquoise;
            checkBox1.ForeColor = SystemColors.ActiveCaptionText;
            checkBox1.Location = new Point(126, 295);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(159, 19);
            checkBox1.TabIndex = 11;
            checkBox1.Text = "Otro (No es de gravedad)";
            checkBox1.UseVisualStyleBackColor = false;
            // 
            // checkedListBox3
            // 
            checkedListBox3.BackColor = Color.LightCyan;
            checkedListBox3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkedListBox3.FormattingEnabled = true;
            checkedListBox3.Items.AddRange(new object[] { "Discapacidad", "Adulto Mayor", "Embarazadas " });
            checkedListBox3.Location = new Point(436, 117);
            checkedListBox3.Name = "checkedListBox3";
            checkedListBox3.Size = new Size(232, 76);
            checkedListBox3.TabIndex = 13;
            // 
            // checkedListBox2
            // 
            checkedListBox2.BackColor = Color.Moccasin;
            checkedListBox2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkedListBox2.FormattingEnabled = true;
            checkedListBox2.Items.AddRange(new object[] { "Fiebre Alta (MEDIA)", "Dolores de Cabeza Intensos (MEDIA)", "Dolores Abdominales (MEDIA)", "Nauseas/Vómitos (MEDIA)" });
            checkedListBox2.Location = new Point(28, 199);
            checkedListBox2.Name = "checkedListBox2";
            checkedListBox2.Size = new Size(389, 76);
            checkedListBox2.TabIndex = 12;
            // 
            // checkedListBox1
            // 
            checkedListBox1.BackColor = Color.MistyRose;
            checkedListBox1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "Dolor de Pecho (ALTA)", "Dificultad Para Respirar (ALTA)", "Sangrado (ALTA)", "Lesión Expuesta (ALTA)" });
            checkedListBox1.Location = new Point(28, 117);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(389, 76);
            checkedListBox1.TabIndex = 11;
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
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label5.ForeColor = SystemColors.ControlText;
            label5.Location = new Point(482, 34);
            label5.Name = "label5";
            label5.Size = new Size(154, 21);
            label5.TabIndex = 10;
            label5.Text = "Condición Especial";
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
            // FrmTurnoEmergencia
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1109, 662);
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label LNombrePantalla;
        private Label LNuevoPaciente;
        private Label LNuevoPaciente2;
        private Panel panel2;
        private TextBox txtApellido;
        private Label label2;
        private TextBox txtNombre;
        private Label label1;
        private TextBox txtDNI;
        private Label label3;
        private GroupBox Condicionales;
        private Label label4;
        private Label label6;
        private Label label5;
        private CheckedListBox checkedListBox1;
        private CheckedListBox checkedListBox2;
        private CheckedListBox checkedListBox3;
        private CheckBox checkBox1;
        private Button button1;
    }
}