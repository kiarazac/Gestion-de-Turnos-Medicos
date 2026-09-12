namespace Gestion_de_Turnos_Medicos
{
    partial class FrmUsuarioVentana
    {
        /// <summary>
        /// Variable de diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario.</param>
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
        /// Método necesario para admitir el Diseñador. No se debe modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            pnlHeader = new Panel();
            lblTituloPrincipal = new Label();
            pnlFooter = new Panel();
            lblFooter = new Label();
            pnlContenido = new Panel();
            pnlGeneral = new Panel();
            dgvGeneral = new DataGridView();
            colTurnoGen = new DataGridViewTextBoxColumn();
            colHoraGen = new DataGridViewTextBoxColumn();
            colFechaGen = new DataGridViewTextBoxColumn();
            colEspecialidadGen = new DataGridViewTextBoxColumn();
            colEstadoGen = new DataGridViewTextBoxColumn();
            colSalaGen = new DataGridViewTextBoxColumn();
            lblTituloGeneral = new Label();
            pnlEmergencias = new Panel();
            dgvEmergencias = new DataGridView();
            colTurnoEmer = new DataGridViewTextBoxColumn();
            colPrioridadEmer = new DataGridViewTextBoxColumn();
            colHoraEmer = new DataGridViewTextBoxColumn();
            colEstadoEmer = new DataGridViewTextBoxColumn();
            colSalaEmer = new DataGridViewTextBoxColumn();
            lblTituloEmergencias = new Label();
            timerReloj = new System.Windows.Forms.Timer(components);
            pnlHeader.SuspendLayout();
            pnlFooter.SuspendLayout();
            pnlContenido.SuspendLayout();
            pnlGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGeneral).BeginInit();
            pnlEmergencias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEmergencias).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(20, 87, 153);
            pnlHeader.Controls.Add(lblTituloPrincipal);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1024, 60);
            pnlHeader.TabIndex = 0;
            // 
            // lblTituloPrincipal
            // 
            lblTituloPrincipal.Dock = DockStyle.Fill;
            lblTituloPrincipal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTituloPrincipal.ForeColor = Color.White;
            lblTituloPrincipal.Location = new Point(0, 0);
            lblTituloPrincipal.Name = "lblTituloPrincipal";
            lblTituloPrincipal.Size = new Size(1024, 60);
            lblTituloPrincipal.TabIndex = 0;
            lblTituloPrincipal.Text = "Lista de Turnos";
            lblTituloPrincipal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.White;
            pnlFooter.Controls.Add(lblFooter);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 588);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(1024, 32);
            pnlFooter.TabIndex = 1;
            // 
            // lblFooter
            // 
            lblFooter.Dock = DockStyle.Fill;
            lblFooter.Font = new Font("Segoe UI", 9F);
            lblFooter.ForeColor = Color.DimGray;
            lblFooter.Location = new Point(0, 0);
            lblFooter.Name = "lblFooter";
            lblFooter.Padding = new Padding(0, 0, 12, 0);
            lblFooter.Size = new Size(1024, 32);
            lblFooter.TabIndex = 0;
            lblFooter.Text = "Clínica contacto: -";
            lblFooter.TextAlign = ContentAlignment.MiddleRight;
            // 
            // pnlContenido
            // 
            pnlContenido.BackColor = Color.White;
            pnlContenido.Controls.Add(pnlGeneral);
            pnlContenido.Controls.Add(pnlEmergencias);
            pnlContenido.Dock = DockStyle.Fill;
            pnlContenido.Location = new Point(0, 60);
            pnlContenido.Name = "pnlContenido";
            pnlContenido.Size = new Size(1024, 528);
            pnlContenido.TabIndex = 2;
            // 
            // pnlGeneral
            // 
            pnlGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pnlGeneral.BackColor = Color.White;
            pnlGeneral.BorderStyle = BorderStyle.FixedSingle;
            pnlGeneral.Controls.Add(dgvGeneral);
            pnlGeneral.Controls.Add(lblTituloGeneral);
            pnlGeneral.Location = new Point(519, 15);
            pnlGeneral.Name = "pnlGeneral";
            pnlGeneral.Size = new Size(490, 498);
            pnlGeneral.TabIndex = 1;
            // 
            // dgvGeneral
            // 
            dgvGeneral.AllowUserToAddRows = false;
            dgvGeneral.AllowUserToDeleteRows = false;
            dgvGeneral.AllowUserToResizeColumns = false;
            dgvGeneral.AllowUserToResizeRows = false;
            dgvGeneral.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGeneral.BackgroundColor = Color.White;
            dgvGeneral.BorderStyle = BorderStyle.None;
            dgvGeneral.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = Color.White;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.Black;
            dgvGeneral.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvGeneral.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvGeneral.Columns.AddRange(new DataGridViewColumn[] { colTurnoGen, colHoraGen, colFechaGen, colEspecialidadGen, colEstadoGen, colSalaGen });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.White;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.75F);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = Color.White;
            dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvGeneral.DefaultCellStyle = dataGridViewCellStyle2;
            dgvGeneral.Dock = DockStyle.Fill;
            dgvGeneral.EnableHeadersVisualStyles = false;
            dgvGeneral.GridColor = Color.FromArgb(225, 225, 225);
            dgvGeneral.Location = new Point(0, 45);
            dgvGeneral.MultiSelect = false;
            dgvGeneral.Name = "dgvGeneral";
            dgvGeneral.ReadOnly = true;
            dgvGeneral.RowHeadersVisible = false;
            dgvGeneral.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvGeneral.RowTemplate.Height = 32;
            dgvGeneral.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGeneral.Size = new Size(488, 451);
            dgvGeneral.TabIndex = 1;
            // 
            // colTurnoGen
            // 
            colTurnoGen.HeaderText = "Turno";
            colTurnoGen.Name = "colTurnoGen";
            colTurnoGen.ReadOnly = true;
            // 
            // colHoraGen
            // 
            colHoraGen.HeaderText = "Hora";
            colHoraGen.Name = "colHoraGen";
            colHoraGen.ReadOnly = true;
            // 
            // colFechaGen
            // 
            colFechaGen.HeaderText = "Fecha";
            colFechaGen.Name = "colFechaGen";
            colFechaGen.ReadOnly = true;
            // 
            // colEspecialidadGen
            // 
            colEspecialidadGen.HeaderText = "Especialidad";
            colEspecialidadGen.Name = "colEspecialidadGen";
            colEspecialidadGen.ReadOnly = true;
            // 
            // colEstadoGen
            // 
            colEstadoGen.HeaderText = "Estado";
            colEstadoGen.Name = "colEstadoGen";
            colEstadoGen.ReadOnly = true;
            // 
            // colSalaGen
            // 
            colSalaGen.HeaderText = "Sala";
            colSalaGen.Name = "colSalaGen";
            colSalaGen.ReadOnly = true;
            // 
            // lblTituloGeneral
            // 
            lblTituloGeneral.Dock = DockStyle.Top;
            lblTituloGeneral.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTituloGeneral.Location = new Point(0, 0);
            lblTituloGeneral.Name = "lblTituloGeneral";
            lblTituloGeneral.Size = new Size(488, 45);
            lblTituloGeneral.TabIndex = 0;
            lblTituloGeneral.Text = "GENERAL";
            lblTituloGeneral.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlEmergencias
            // 
            pnlEmergencias.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            pnlEmergencias.BackColor = Color.White;
            pnlEmergencias.BorderStyle = BorderStyle.FixedSingle;
            pnlEmergencias.Controls.Add(dgvEmergencias);
            pnlEmergencias.Controls.Add(lblTituloEmergencias);
            pnlEmergencias.Location = new Point(15, 15);
            pnlEmergencias.Name = "pnlEmergencias";
            pnlEmergencias.Size = new Size(490, 498);
            pnlEmergencias.TabIndex = 0;
            // 
            // dgvEmergencias
            // 
            dgvEmergencias.AllowUserToAddRows = false;
            dgvEmergencias.AllowUserToDeleteRows = false;
            dgvEmergencias.AllowUserToResizeColumns = false;
            dgvEmergencias.AllowUserToResizeRows = false;
            dgvEmergencias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEmergencias.BackgroundColor = Color.White;
            dgvEmergencias.BorderStyle = BorderStyle.None;
            dgvEmergencias.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dgvEmergencias.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvEmergencias.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvEmergencias.Columns.AddRange(new DataGridViewColumn[] { colTurnoEmer, colPrioridadEmer, colHoraEmer, colEstadoEmer, colSalaEmer });
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = Color.White;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9.75F);
            dataGridViewCellStyle4.ForeColor = Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = Color.White;
            dataGridViewCellStyle4.SelectionForeColor = Color.Black;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvEmergencias.DefaultCellStyle = dataGridViewCellStyle4;
            dgvEmergencias.Dock = DockStyle.Fill;
            dgvEmergencias.EnableHeadersVisualStyles = false;
            dgvEmergencias.GridColor = Color.FromArgb(225, 225, 225);
            dgvEmergencias.Location = new Point(0, 45);
            dgvEmergencias.MultiSelect = false;
            dgvEmergencias.Name = "dgvEmergencias";
            dgvEmergencias.ReadOnly = true;
            dgvEmergencias.RowHeadersVisible = false;
            dgvEmergencias.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvEmergencias.RowTemplate.Height = 32;
            dgvEmergencias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmergencias.Size = new Size(488, 451);
            dgvEmergencias.TabIndex = 1;
            // 
            // colTurnoEmer
            // 
            colTurnoEmer.HeaderText = "Turno";
            colTurnoEmer.Name = "colTurnoEmer";
            colTurnoEmer.ReadOnly = true;
            // 
            // colPrioridadEmer
            // 
            colPrioridadEmer.HeaderText = "Prioridad";
            colPrioridadEmer.Name = "colPrioridadEmer";
            colPrioridadEmer.ReadOnly = true;
            // 
            // colHoraEmer
            // 
            colHoraEmer.HeaderText = "Hora";
            colHoraEmer.Name = "colHoraEmer";
            colHoraEmer.ReadOnly = true;
            // 
            // colEstadoEmer
            // 
            colEstadoEmer.HeaderText = "Estado";
            colEstadoEmer.Name = "colEstadoEmer";
            colEstadoEmer.ReadOnly = true;
            // 
            // colSalaEmer
            // 
            colSalaEmer.HeaderText = "Sala";
            colSalaEmer.Name = "colSalaEmer";
            colSalaEmer.ReadOnly = true;
            // 
            // lblTituloEmergencias
            // 
            lblTituloEmergencias.Dock = DockStyle.Top;
            lblTituloEmergencias.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTituloEmergencias.Location = new Point(0, 0);
            lblTituloEmergencias.Name = "lblTituloEmergencias";
            lblTituloEmergencias.Size = new Size(488, 45);
            lblTituloEmergencias.TabIndex = 0;
            lblTituloEmergencias.Text = "EMERGENCIAS";
            lblTituloEmergencias.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // timerReloj
            // 
            timerReloj.Enabled = true;
            timerReloj.Interval = 1000;
            timerReloj.Tick += timerReloj_Tick;
            // 
            // FrmUsuarioVentana
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1024, 620);
            Controls.Add(pnlContenido);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            MinimumSize = new Size(800, 500);
            Name = "FrmUsuarioVentana";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Visualizador de Turnos - Pacientes";
            pnlHeader.ResumeLayout(false);
            pnlFooter.ResumeLayout(false);
            pnlContenido.ResumeLayout(false);
            pnlGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvGeneral).EndInit();
            pnlEmergencias.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvEmergencias).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTituloPrincipal;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.Panel pnlContenido;
        private System.Windows.Forms.Panel pnlEmergencias;
        private System.Windows.Forms.Label lblTituloEmergencias;
        private System.Windows.Forms.DataGridView dgvEmergencias;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTurnoEmer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrioridadEmer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHoraEmer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstadoEmer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSalaEmer;
        private System.Windows.Forms.Panel pnlGeneral;
        private System.Windows.Forms.Label lblTituloGeneral;
        private System.Windows.Forms.DataGridView dgvGeneral;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTurnoGen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHoraGen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFechaGen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEspecialidadGen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstadoGen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSalaGen;
        private System.Windows.Forms.Timer timerReloj;
    }
}