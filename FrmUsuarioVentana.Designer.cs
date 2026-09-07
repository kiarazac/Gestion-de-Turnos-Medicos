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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTituloPrincipal = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lblFooter = new System.Windows.Forms.Label();
            this.pnlContenido = new System.Windows.Forms.Panel();
            this.pnlGeneral = new System.Windows.Forms.Panel();
            this.dgvGeneral = new System.Windows.Forms.DataGridView();
            this.colTurnoGen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHoraGen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFechaGen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEspecialidadGen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstadoGen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSalaGen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTituloGeneral = new System.Windows.Forms.Label();
            this.pnlEmergencias = new System.Windows.Forms.Panel();
            this.dgvEmergencias = new System.Windows.Forms.DataGridView();
            this.colTurnoEmer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrioridadEmer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHoraEmer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstadoEmer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSalaEmer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTituloEmergencias = new System.Windows.Forms.Label();
            this.timerReloj = new System.Windows.Forms.Timer(this.components);
            this.pnlHeader.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.pnlContenido.SuspendLayout();
            this.pnlGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGeneral)).BeginInit();
            this.pnlEmergencias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmergencias)).BeginInit();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(87)))), ((int)(((byte)(153)))));
            this.pnlHeader.Controls.Add(this.lblTituloPrincipal);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1024, 60);
            this.pnlHeader.TabIndex = 0;
            //
            // lblTituloPrincipal
            //
            this.lblTituloPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTituloPrincipal.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTituloPrincipal.ForeColor = System.Drawing.Color.White;
            this.lblTituloPrincipal.Location = new System.Drawing.Point(0, 0);
            this.lblTituloPrincipal.Name = "lblTituloPrincipal";
            this.lblTituloPrincipal.Size = new System.Drawing.Size(1024, 60);
            this.lblTituloPrincipal.TabIndex = 0;
            this.lblTituloPrincipal.Text = "Lista de Turnos";
            this.lblTituloPrincipal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlFooter
            //
            this.pnlFooter.BackColor = System.Drawing.Color.White;
            this.pnlFooter.Controls.Add(this.lblFooter);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 588);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1024, 32);
            this.pnlFooter.TabIndex = 1;
            //
            // lblFooter
            //
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFooter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFooter.ForeColor = System.Drawing.Color.DimGray;
            this.lblFooter.Location = new System.Drawing.Point(0, 0);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblFooter.Size = new System.Drawing.Size(1024, 32);
            this.lblFooter.TabIndex = 0;
            this.lblFooter.Text = "Clínica contacto: -";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlContenido
            //
            this.pnlContenido.BackColor = System.Drawing.Color.White;
            this.pnlContenido.Controls.Add(this.pnlGeneral);
            this.pnlContenido.Controls.Add(this.pnlEmergencias);
            this.pnlContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenido.Location = new System.Drawing.Point(0, 60);
            this.pnlContenido.Name = "pnlContenido";
            this.pnlContenido.Size = new System.Drawing.Size(1024, 528);
            this.pnlContenido.TabIndex = 2;
            //
            // pnlGeneral
            //
            this.pnlGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.pnlGeneral.BackColor = System.Drawing.Color.White;
            this.pnlGeneral.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGeneral.Controls.Add(this.dgvGeneral);
            this.pnlGeneral.Controls.Add(this.lblTituloGeneral);
            this.pnlGeneral.Location = new System.Drawing.Point(519, 15);
            this.pnlGeneral.Name = "pnlGeneral";
            this.pnlGeneral.Size = new System.Drawing.Size(490, 498);
            this.pnlGeneral.TabIndex = 1;
            //
            // dgvGeneral
            //
            this.dgvGeneral.AllowUserToAddRows = false;
            this.dgvGeneral.AllowUserToDeleteRows = false;
            this.dgvGeneral.AllowUserToResizeColumns = false;
            this.dgvGeneral.AllowUserToResizeRows = false;
            this.dgvGeneral.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvGeneral.BackgroundColor = System.Drawing.Color.White;
            this.dgvGeneral.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvGeneral.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvGeneral.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvGeneral.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvGeneral.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTurnoGen,
            this.colHoraGen,
            this.colFechaGen,
            this.colEspecialidadGen,
            this.colEstadoGen,
            this.colSalaGen});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvGeneral.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGeneral.EnableHeadersVisualStyles = false;
            this.dgvGeneral.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.dgvGeneral.Location = new System.Drawing.Point(0, 45);
            this.dgvGeneral.MultiSelect = false;
            this.dgvGeneral.Name = "dgvGeneral";
            this.dgvGeneral.ReadOnly = true;
            this.dgvGeneral.RowHeadersVisible = false;
            this.dgvGeneral.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvGeneral.RowTemplate.Height = 32;
            this.dgvGeneral.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGeneral.Size = new System.Drawing.Size(488, 451);
            this.dgvGeneral.TabIndex = 1;
            //
            // colTurnoGen
            //
            this.colTurnoGen.HeaderText = "Turno";
            this.colTurnoGen.Name = "colTurnoGen";
            this.colTurnoGen.ReadOnly = true;
            //
            // colHoraGen
            //
            this.colHoraGen.HeaderText = "Hora";
            this.colHoraGen.Name = "colHoraGen";
            this.colHoraGen.ReadOnly = true;
            //
            // colFechaGen
            //
            this.colFechaGen.HeaderText = "Fecha";
            this.colFechaGen.Name = "colFechaGen";
            this.colFechaGen.ReadOnly = true;
            //
            // colEspecialidadGen
            //
            this.colEspecialidadGen.HeaderText = "Especialidad";
            this.colEspecialidadGen.Name = "colEspecialidadGen";
            this.colEspecialidadGen.ReadOnly = true;
            //
            // colEstadoGen
            //
            this.colEstadoGen.HeaderText = "Estado";
            this.colEstadoGen.Name = "colEstadoGen";
            this.colEstadoGen.ReadOnly = true;
            //
            // colSalaGen
            //
            this.colSalaGen.HeaderText = "Sala";
            this.colSalaGen.Name = "colSalaGen";
            this.colSalaGen.ReadOnly = true;
            //
            // lblTituloGeneral
            //
            this.lblTituloGeneral.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTituloGeneral.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTituloGeneral.Location = new System.Drawing.Point(0, 0);
            this.lblTituloGeneral.Name = "lblTituloGeneral";
            this.lblTituloGeneral.Size = new System.Drawing.Size(488, 45);
            this.lblTituloGeneral.TabIndex = 0;
            this.lblTituloGeneral.Text = "GENERAL";
            this.lblTituloGeneral.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlEmergencias
            //
            this.pnlEmergencias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left))));
            this.pnlEmergencias.BackColor = System.Drawing.Color.White;
            this.pnlEmergencias.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlEmergencias.Controls.Add(this.dgvEmergencias);
            this.pnlEmergencias.Controls.Add(this.lblTituloEmergencias);
            this.pnlEmergencias.Location = new System.Drawing.Point(15, 15);
            this.pnlEmergencias.Name = "pnlEmergencias";
            this.pnlEmergencias.Size = new System.Drawing.Size(490, 498);
            this.pnlEmergencias.TabIndex = 0;
            //
            // dgvEmergencias
            //
            this.dgvEmergencias.AllowUserToAddRows = false;
            this.dgvEmergencias.AllowUserToDeleteRows = false;
            this.dgvEmergencias.AllowUserToResizeColumns = false;
            this.dgvEmergencias.AllowUserToResizeRows = false;
            this.dgvEmergencias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEmergencias.BackgroundColor = System.Drawing.Color.White;
            this.dgvEmergencias.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEmergencias.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            this.dgvEmergencias.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvEmergencias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvEmergencias.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTurnoEmer,
            this.colPrioridadEmer,
            this.colHoraEmer,
            this.colEstadoEmer,
            this.colSalaEmer});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvEmergencias.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvEmergencias.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmergencias.EnableHeadersVisualStyles = false;
            this.dgvEmergencias.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.dgvEmergencias.Location = new System.Drawing.Point(0, 45);
            this.dgvEmergencias.MultiSelect = false;
            this.dgvEmergencias.Name = "dgvEmergencias";
            this.dgvEmergencias.ReadOnly = true;
            this.dgvEmergencias.RowHeadersVisible = false;
            this.dgvEmergencias.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvEmergencias.RowTemplate.Height = 32;
            this.dgvEmergencias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmergencias.Size = new System.Drawing.Size(488, 451);
            this.dgvEmergencias.TabIndex = 1;
            //
            // colTurnoEmer
            //
            this.colTurnoEmer.HeaderText = "Turno";
            this.colTurnoEmer.Name = "colTurnoEmer";
            this.colTurnoEmer.ReadOnly = true;
            //
            // colPrioridadEmer
            //
            this.colPrioridadEmer.HeaderText = "Prioridad";
            this.colPrioridadEmer.Name = "colPrioridadEmer";
            this.colPrioridadEmer.ReadOnly = true;
            //
            // colHoraEmer
            //
            this.colHoraEmer.HeaderText = "Hora";
            this.colHoraEmer.Name = "colHoraEmer";
            this.colHoraEmer.ReadOnly = true;
            //
            // colEstadoEmer
            //
            this.colEstadoEmer.HeaderText = "Estado";
            this.colEstadoEmer.Name = "colEstadoEmer";
            this.colEstadoEmer.ReadOnly = true;
            //
            // colSalaEmer
            //
            this.colSalaEmer.HeaderText = "Sala";
            this.colSalaEmer.Name = "colSalaEmer";
            this.colSalaEmer.ReadOnly = true;
            //
            // lblTituloEmergencias
            //
            this.lblTituloEmergencias.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTituloEmergencias.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTituloEmergencias.Location = new System.Drawing.Point(0, 0);
            this.lblTituloEmergencias.Name = "lblTituloEmergencias";
            this.lblTituloEmergencias.Size = new System.Drawing.Size(488, 45);
            this.lblTituloEmergencias.TabIndex = 0;
            this.lblTituloEmergencias.Text = "EMERGENCIAS";
            this.lblTituloEmergencias.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // timerReloj
            //
            this.timerReloj.Enabled = true;
            this.timerReloj.Interval = 1000;
            this.timerReloj.Tick += new System.EventHandler(this.timerReloj_Tick);
            //
            // FrmUsuarioVentana
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1024, 620);
            this.Controls.Add(this.pnlContenido);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FrmUsuarioVentana";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Visualizador de Turnos - Pacientes";
            this.pnlHeader.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.pnlContenido.ResumeLayout(false);
            this.pnlGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGeneral)).EndInit();
            this.pnlEmergencias.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmergencias)).EndInit();
            this.ResumeLayout(false);

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