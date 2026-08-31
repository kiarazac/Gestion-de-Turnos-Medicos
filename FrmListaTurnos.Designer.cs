namespace Gestion_de_Turnos_Medicos
{
    partial class FrmListaTurnos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmListaTurnos));
            LNombrePantalla = new Label();
            panel1 = new Panel();
            LNuevoPaciente = new Label();
            panel2 = new Panel();
            LtotalAlta = new Label();
            LAlta = new Label();
            panel3 = new Panel();
            LtotalMedia = new Label();
            LMedia = new Label();
            panel4 = new Panel();
            LtotalBaja = new Label();
            LBaja = new Label();
            dataGridView1 = new DataGridView();
            Turno = new DataGridViewTextBoxColumn();
            Prioridad = new DataGridViewTextBoxColumn();
            Hora = new DataGridViewTextBoxColumn();
            Estado = new DataGridViewTextBoxColumn();
            Sala = new DataGridViewTextBoxColumn();
            label1 = new Label();
            cmbEspecialidades = new ComboBox();
            dgvEspecialidades = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            Fecha = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvEspecialidades).BeginInit();
            SuspendLayout();
            // 
            // LNombrePantalla
            // 
            LNombrePantalla.AutoSize = true;
            LNombrePantalla.Font = new Font("Britannic Bold", 24.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LNombrePantalla.ForeColor = SystemColors.ButtonFace;
            LNombrePantalla.Location = new Point(239, 7);
            LNombrePantalla.Name = "LNombrePantalla";
            LNombrePantalla.Size = new Size(254, 37);
            LNombrePantalla.TabIndex = 0;
            LNombrePantalla.Text = "Lista de Turnos ";
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Highlight;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(LNombrePantalla);
            panel1.Location = new Point(-157, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1383, 63);
            panel1.TabIndex = 1;
            // 
            // LNuevoPaciente
            // 
            LNuevoPaciente.AutoSize = true;
            LNuevoPaciente.Font = new Font("Arial Black", 18F, FontStyle.Bold);
            LNuevoPaciente.ForeColor = SystemColors.ActiveCaptionText;
            LNuevoPaciente.Location = new Point(104, 86);
            LNuevoPaciente.Name = "LNuevoPaciente";
            LNuevoPaciente.Size = new Size(267, 33);
            LNuevoPaciente.TabIndex = 2;
            LNuevoPaciente.Text = "Sector Emergencia";
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(LtotalAlta);
            panel2.Controls.Add(LAlta);
            panel2.Location = new Point(31, 143);
            panel2.Name = "panel2";
            panel2.Size = new Size(116, 91);
            panel2.TabIndex = 3;
            // 
            // LtotalAlta
            // 
            LtotalAlta.AutoSize = true;
            LtotalAlta.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            LtotalAlta.ForeColor = Color.IndianRed;
            LtotalAlta.Location = new Point(41, 37);
            LtotalAlta.Name = "LtotalAlta";
            LtotalAlta.Size = new Size(23, 25);
            LtotalAlta.TabIndex = 1;
            LtotalAlta.Text = "0";
            // 
            // LAlta
            // 
            LAlta.AutoSize = true;
            LAlta.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            LAlta.ForeColor = Color.IndianRed;
            LAlta.Location = new Point(1, 9);
            LAlta.Name = "LAlta";
            LAlta.Size = new Size(110, 19);
            LAlta.TabIndex = 0;
            LAlta.Text = "Prioridad ALTA";
            // 
            // panel3
            // 
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(LtotalMedia);
            panel3.Controls.Add(LMedia);
            panel3.Location = new Point(208, 143);
            panel3.Name = "panel3";
            panel3.Size = new Size(116, 91);
            panel3.TabIndex = 4;
            // 
            // LtotalMedia
            // 
            LtotalMedia.AutoSize = true;
            LtotalMedia.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            LtotalMedia.ForeColor = Color.Orange;
            LtotalMedia.Location = new Point(46, 37);
            LtotalMedia.Name = "LtotalMedia";
            LtotalMedia.Size = new Size(23, 25);
            LtotalMedia.TabIndex = 2;
            LtotalMedia.Text = "0";
            // 
            // LMedia
            // 
            LMedia.AutoSize = true;
            LMedia.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            LMedia.ForeColor = Color.Orange;
            LMedia.Location = new Point(-1, 7);
            LMedia.Name = "LMedia";
            LMedia.Size = new Size(121, 19);
            LMedia.TabIndex = 1;
            LMedia.Text = "Prioridad MEDIA";
            // 
            // panel4
            // 
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel4.Controls.Add(LtotalBaja);
            panel4.Controls.Add(LBaja);
            panel4.Location = new Point(374, 143);
            panel4.Name = "panel4";
            panel4.Size = new Size(116, 91);
            panel4.TabIndex = 4;
            // 
            // LtotalBaja
            // 
            LtotalBaja.AutoSize = true;
            LtotalBaja.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            LtotalBaja.ForeColor = Color.ForestGreen;
            LtotalBaja.Location = new Point(44, 37);
            LtotalBaja.Name = "LtotalBaja";
            LtotalBaja.Size = new Size(23, 25);
            LtotalBaja.TabIndex = 6;
            LtotalBaja.Text = "0";
            // 
            // LBaja
            // 
            LBaja.AutoSize = true;
            LBaja.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            LBaja.ForeColor = Color.ForestGreen;
            LBaja.Location = new Point(-1, 9);
            LBaja.Name = "LBaja";
            LBaja.Size = new Size(113, 19);
            LBaja.TabIndex = 5;
            LBaja.Text = "Prioridad BAJA";
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.BackgroundColor = SystemColors.ActiveCaption;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Turno, Prioridad, Hora, Estado, Sala });
            dataGridView1.Location = new Point(12, 250);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(503, 339);
            dataGridView1.TabIndex = 5;
            // 
            // Turno
            // 
            Turno.HeaderText = "Turno";
            Turno.Name = "Turno";
            Turno.ReadOnly = true;
            // 
            // Prioridad
            // 
            Prioridad.HeaderText = "Prioridad";
            Prioridad.Name = "Prioridad";
            Prioridad.ReadOnly = true;
            // 
            // Hora
            // 
            Hora.HeaderText = "Hora";
            Hora.Name = "Hora";
            Hora.ReadOnly = true;
            // 
            // Estado
            // 
            Estado.HeaderText = "Estado";
            Estado.Name = "Estado";
            Estado.ReadOnly = true;
            // 
            // Sala
            // 
            Sala.HeaderText = "Sala";
            Sala.Name = "Sala";
            Sala.ReadOnly = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Arial Black", 18F, FontStyle.Bold);
            label1.ForeColor = SystemColors.ActiveCaptionText;
            label1.Location = new Point(775, 86);
            label1.Name = "label1";
            label1.Size = new Size(309, 33);
            label1.TabIndex = 6;
            label1.Text = "Sector Especialidades";
            // 
            // cmbEspecialidades
            // 
            cmbEspecialidades.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEspecialidades.FormattingEnabled = true;
            cmbEspecialidades.Location = new Point(681, 146);
            cmbEspecialidades.Name = "cmbEspecialidades";
            cmbEspecialidades.Size = new Size(485, 23);
            cmbEspecialidades.TabIndex = 7;
            cmbEspecialidades.SelectedIndexChanged += cmbEspecialidades_SelectedIndexChanged;
            // 
            // dgvEspecialidades
            // 
            dgvEspecialidades.AllowUserToAddRows = false;
            dgvEspecialidades.AllowUserToDeleteRows = false;
            dgvEspecialidades.BackgroundColor = SystemColors.ActiveCaption;
            dgvEspecialidades.BorderStyle = BorderStyle.None;
            dgvEspecialidades.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEspecialidades.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn3, Fecha, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5 });
            dgvEspecialidades.Location = new Point(612, 250);
            dgvEspecialidades.Name = "dgvEspecialidades";
            dgvEspecialidades.ReadOnly = true;
            dgvEspecialidades.RowHeadersVisible = false;
            dgvEspecialidades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEspecialidades.Size = new Size(576, 339);
            dgvEspecialidades.TabIndex = 8;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Turno";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Hora";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // Fecha
            // 
            Fecha.HeaderText = "Fecha";
            Fecha.Name = "Fecha";
            Fecha.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Estado";
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.HeaderText = "Sala";
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // FrmListaTurnos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1222, 601);
            Controls.Add(dgvEspecialidades);
            Controls.Add(cmbEspecialidades);
            Controls.Add(label1);
            Controls.Add(dataGridView1);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(LNuevoPaciente);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmListaTurnos";
            Text = "FrmListaTurnos";
            WindowState = FormWindowState.Maximized;
            Load += FrmListaTurnos_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvEspecialidades).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LNombrePantalla;
        private Panel panel1;
        private Label LNuevoPaciente;
        private Panel panel2;
        private Label LAlta;
        private Panel panel3;
        private Label LMedia;
        private Panel panel4;
        private Label LBaja;
        private Label LtotalAlta;
        private Label LtotalMedia;
        private Label LtotalBaja;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Turno;
        private DataGridViewTextBoxColumn Prioridad;
        private DataGridViewTextBoxColumn Hora;
        private DataGridViewTextBoxColumn Estado;
        private DataGridViewTextBoxColumn Sala;
        private Label label1;
        private ComboBox cmbEspecialidades;
        private DataGridView dgvEspecialidades;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn Fecha;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    }
}