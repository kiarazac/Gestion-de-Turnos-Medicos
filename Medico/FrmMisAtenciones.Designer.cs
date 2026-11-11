namespace Gestion_de_Turnos_Medicos
{
    partial class FrmMisAtenciones
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
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
            pnlHeader = new Panel();
            lblSubtituloHeader = new Label();
            lblTituloHeader = new Label();
            pnlContenedorPrincipal = new Panel();
            pnlCardDetalle = new Panel();
            txtDetalleReceta = new TextBox();
            lblTituloReceta = new Label();
            txtDetalleEvolucion = new TextBox();
            lblTituloEvolucion = new Label();
            lblSubtituloDetalle = new Label();
            pnlCardGrilla = new Panel();
            lblTotalAtenciones = new Label();
            dgvAtenciones = new DataGridView();
            pnlFiltros = new Panel();
            btnExportar = new Button();
            btnFiltrar = new Button();
            txtBuscar = new TextBox();
            lblBuscar = new Label();
            dtpHasta = new DateTimePicker();
            lblHasta = new Label();
            dtpDesde = new DateTimePicker();
            lblDesde = new Label();
            cmbPeriodo = new ComboBox();
            lblPeriodo = new Label();
            tabMisAtenciones = new TabControl();
            tabConsultas = new TabPage();
            tabTriageClinico = new TabPage();
            splitTriage = new SplitContainer();
            pnlGravedad = new Panel();
            dgvRankingGravedad = new DataGridView();
            lblTituloGravedad = new Label();
            pnlSintomas = new Panel();
            dgvRankingSintomas = new DataGridView();
            lblTituloSintomas = new Label();
            pnlHeader.SuspendLayout();
            pnlContenedorPrincipal.SuspendLayout();
            tabMisAtenciones.SuspendLayout();
            tabConsultas.SuspendLayout();
            tabTriageClinico.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitTriage).BeginInit();
            splitTriage.Panel1.SuspendLayout();
            splitTriage.Panel2.SuspendLayout();
            splitTriage.SuspendLayout();
            pnlGravedad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRankingGravedad).BeginInit();
            pnlSintomas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRankingSintomas).BeginInit();
            pnlCardDetalle.SuspendLayout();
            pnlCardGrilla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAtenciones).BeginInit();
            pnlFiltros.SuspendLayout();
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
            pnlHeader.Size = new Size(1100, 60);
            pnlHeader.TabIndex = 0;
            // 
            // lblSubtituloHeader
            // 
            lblSubtituloHeader.AutoSize = true;
            lblSubtituloHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSubtituloHeader.ForeColor = Color.FromArgb(204, 251, 241);
            lblSubtituloHeader.Location = new Point(18, 34);
            lblSubtituloHeader.Name = "lblSubtituloHeader";
            lblSubtituloHeader.Size = new Size(420, 15);
            lblSubtituloHeader.TabIndex = 1;
            lblSubtituloHeader.Text = "Historial cronológico de pacientes atendidos, diagnósticos y recetas emitidas";
            // 
            // lblTituloHeader
            // 
            lblTituloHeader.AutoSize = true;
            lblTituloHeader.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloHeader.ForeColor = Color.White;
            lblTituloHeader.Location = new Point(16, 8);
            lblTituloHeader.Name = "lblTituloHeader";
            lblTituloHeader.Size = new Size(275, 25);
            lblTituloHeader.TabIndex = 0;
            lblTituloHeader.Text = "Mis Atenciones Realizadas";
            // 
            // pnlContenedorPrincipal
            // 
            pnlContenedorPrincipal.BackColor = Color.FromArgb(241, 245, 249);
            pnlContenedorPrincipal.Controls.Add(tabMisAtenciones);
            pnlContenedorPrincipal.Controls.Add(pnlFiltros);
            pnlContenedorPrincipal.Dock = DockStyle.Fill;
            pnlContenedorPrincipal.Location = new Point(0, 60);
            pnlContenedorPrincipal.Name = "pnlContenedorPrincipal";
            pnlContenedorPrincipal.Padding = new Padding(12);
            pnlContenedorPrincipal.Size = new Size(1100, 540);
            pnlContenedorPrincipal.TabIndex = 1;
            // 
            // tabMisAtenciones
            // 
            tabMisAtenciones.Controls.Add(tabConsultas);
            tabMisAtenciones.Controls.Add(tabTriageClinico);
            tabMisAtenciones.Dock = DockStyle.Fill;
            tabMisAtenciones.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            tabMisAtenciones.Location = new Point(12, 68);
            tabMisAtenciones.Name = "tabMisAtenciones";
            tabMisAtenciones.SelectedIndex = 0;
            tabMisAtenciones.Size = new Size(1076, 460);
            tabMisAtenciones.TabIndex = 1;
            // 
            // tabConsultas
            // 
            tabConsultas.Controls.Add(pnlCardGrilla);
            tabConsultas.Controls.Add(pnlCardDetalle);
            tabConsultas.Location = new Point(4, 25);
            tabConsultas.Name = "tabConsultas";
            tabConsultas.Padding = new Padding(5);
            tabConsultas.Size = new Size(1068, 431);
            tabConsultas.TabIndex = 0;
            tabConsultas.Text = "  📋 Mis Consultas y Pacientes Atendidos  ";
            tabConsultas.UseVisualStyleBackColor = true;
            // 
            // tabTriageClinico
            // 
            tabTriageClinico.Controls.Add(splitTriage);
            tabTriageClinico.Location = new Point(4, 25);
            tabTriageClinico.Name = "tabTriageClinico";
            tabTriageClinico.Padding = new Padding(8);
            tabTriageClinico.Size = new Size(1068, 431);
            tabTriageClinico.TabIndex = 1;
            tabTriageClinico.Text = "  🚨 Urgencias y Triage Clínico  ";
            tabTriageClinico.UseVisualStyleBackColor = true;
            // 
            // splitTriage
            // 
            splitTriage.Dock = DockStyle.Fill;
            splitTriage.Location = new Point(8, 8);
            splitTriage.Name = "splitTriage";
            // 
            // splitTriage.Panel1
            // 
            splitTriage.Panel1.Controls.Add(pnlGravedad);
            // 
            // splitTriage.Panel2
            // 
            splitTriage.Panel2.Controls.Add(pnlSintomas);
            splitTriage.Size = new Size(1052, 415);
            splitTriage.SplitterDistance = 450;
            splitTriage.TabIndex = 0;
            // 
            // pnlGravedad
            // 
            pnlGravedad.BackColor = Color.White;
            pnlGravedad.BorderStyle = BorderStyle.FixedSingle;
            pnlGravedad.Controls.Add(dgvRankingGravedad);
            pnlGravedad.Controls.Add(lblTituloGravedad);
            pnlGravedad.Dock = DockStyle.Fill;
            pnlGravedad.Location = new Point(0, 0);
            pnlGravedad.Name = "pnlGravedad";
            pnlGravedad.Padding = new Padding(8);
            pnlGravedad.Size = new Size(450, 415);
            pnlGravedad.TabIndex = 0;
            // 
            // lblTituloGravedad
            // 
            lblTituloGravedad.Dock = DockStyle.Top;
            lblTituloGravedad.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTituloGravedad.ForeColor = Color.FromArgb(15, 118, 110);
            lblTituloGravedad.Location = new Point(8, 8);
            lblTituloGravedad.Name = "lblTituloGravedad";
            lblTituloGravedad.Size = new Size(432, 28);
            lblTituloGravedad.TabIndex = 0;
            lblTituloGravedad.Text = "Distribución y Ranking por Gravedad de Triage";
            // 
            // dgvRankingGravedad
            // 
            dgvRankingGravedad.AllowUserToAddRows = false;
            dgvRankingGravedad.AllowUserToDeleteRows = false;
            dgvRankingGravedad.BackgroundColor = Color.FromArgb(248, 250, 252);
            dgvRankingGravedad.BorderStyle = BorderStyle.None;
            dgvRankingGravedad.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRankingGravedad.Dock = DockStyle.Fill;
            dgvRankingGravedad.Location = new Point(8, 36);
            dgvRankingGravedad.MultiSelect = false;
            dgvRankingGravedad.Name = "dgvRankingGravedad";
            dgvRankingGravedad.ReadOnly = true;
            dgvRankingGravedad.RowHeadersVisible = false;
            dgvRankingGravedad.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRankingGravedad.Size = new Size(432, 369);
            dgvRankingGravedad.TabIndex = 1;
            // 
            // pnlSintomas
            // 
            pnlSintomas.BackColor = Color.White;
            pnlSintomas.BorderStyle = BorderStyle.FixedSingle;
            pnlSintomas.Controls.Add(dgvRankingSintomas);
            pnlSintomas.Controls.Add(lblTituloSintomas);
            pnlSintomas.Dock = DockStyle.Fill;
            pnlSintomas.Location = new Point(0, 0);
            pnlSintomas.Name = "pnlSintomas";
            pnlSintomas.Padding = new Padding(8);
            pnlSintomas.Size = new Size(598, 415);
            pnlSintomas.TabIndex = 0;
            // 
            // lblTituloSintomas
            // 
            lblTituloSintomas.Dock = DockStyle.Top;
            lblTituloSintomas.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTituloSintomas.ForeColor = Color.FromArgb(15, 118, 110);
            lblTituloSintomas.Location = new Point(8, 8);
            lblTituloSintomas.Name = "lblTituloSintomas";
            lblTituloSintomas.Size = new Size(580, 28);
            lblTituloSintomas.TabIndex = 0;
            lblTituloSintomas.Text = "Prevalencia de Síntomas de Pacientes Atendidos";
            // 
            // dgvRankingSintomas
            // 
            dgvRankingSintomas.AllowUserToAddRows = false;
            dgvRankingSintomas.AllowUserToDeleteRows = false;
            dgvRankingSintomas.BackgroundColor = Color.FromArgb(248, 250, 252);
            dgvRankingSintomas.BorderStyle = BorderStyle.None;
            dgvRankingSintomas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRankingSintomas.Dock = DockStyle.Fill;
            dgvRankingSintomas.Location = new Point(8, 36);
            dgvRankingSintomas.MultiSelect = false;
            dgvRankingSintomas.Name = "dgvRankingSintomas";
            dgvRankingSintomas.ReadOnly = true;
            dgvRankingSintomas.RowHeadersVisible = false;
            dgvRankingSintomas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRankingSintomas.Size = new Size(580, 369);
            dgvRankingSintomas.TabIndex = 1;
            // 
            // pnlFiltros
            // 
            pnlFiltros.BackColor = Color.White;
            pnlFiltros.BorderStyle = BorderStyle.FixedSingle;
            pnlFiltros.Controls.Add(btnExportar);
            pnlFiltros.Controls.Add(btnFiltrar);
            pnlFiltros.Controls.Add(txtBuscar);
            pnlFiltros.Controls.Add(lblBuscar);
            pnlFiltros.Controls.Add(dtpHasta);
            pnlFiltros.Controls.Add(lblHasta);
            pnlFiltros.Controls.Add(dtpDesde);
            pnlFiltros.Controls.Add(lblDesde);
            pnlFiltros.Controls.Add(cmbPeriodo);
            pnlFiltros.Controls.Add(lblPeriodo);
            pnlFiltros.Dock = DockStyle.Top;
            pnlFiltros.Location = new Point(12, 12);
            pnlFiltros.Name = "pnlFiltros";
            pnlFiltros.Size = new Size(1076, 56);
            pnlFiltros.TabIndex = 0;
            // 
            // lblPeriodo
            // 
            lblPeriodo.AutoSize = true;
            lblPeriodo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPeriodo.ForeColor = Color.FromArgb(30, 41, 59);
            lblPeriodo.Location = new Point(10, 19);
            lblPeriodo.Name = "lblPeriodo";
            lblPeriodo.Size = new Size(54, 15);
            lblPeriodo.TabIndex = 0;
            lblPeriodo.Text = "Período:";
            // 
            // cmbPeriodo
            // 
            cmbPeriodo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPeriodo.FormattingEnabled = true;
            cmbPeriodo.Items.AddRange(new object[] { "Hoy", "Últimos 7 días", "Este mes", "Todos los registros", "Rango personalizado" });
            cmbPeriodo.Location = new Point(68, 16);
            cmbPeriodo.Name = "cmbPeriodo";
            cmbPeriodo.Size = new Size(140, 23);
            cmbPeriodo.TabIndex = 1;
            // 
            // lblDesde
            // 
            lblDesde.AutoSize = true;
            lblDesde.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDesde.ForeColor = Color.FromArgb(30, 41, 59);
            lblDesde.Location = new Point(220, 19);
            lblDesde.Name = "lblDesde";
            lblDesde.Size = new Size(44, 15);
            lblDesde.TabIndex = 2;
            lblDesde.Text = "Desde:";
            // 
            // dtpDesde
            // 
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(266, 16);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(95, 23);
            dtpDesde.TabIndex = 3;
            // 
            // lblHasta
            // 
            lblHasta.AutoSize = true;
            lblHasta.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHasta.ForeColor = Color.FromArgb(30, 41, 59);
            lblHasta.Location = new Point(370, 19);
            lblHasta.Name = "lblHasta";
            lblHasta.Size = new Size(41, 15);
            lblHasta.TabIndex = 4;
            lblHasta.Text = "Hasta:";
            // 
            // dtpHasta
            // 
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(413, 16);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(95, 23);
            dtpHasta.TabIndex = 5;
            // 
            // lblBuscar
            // 
            lblBuscar.AutoSize = true;
            lblBuscar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblBuscar.ForeColor = Color.FromArgb(30, 41, 59);
            lblBuscar.Location = new Point(520, 19);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(47, 15);
            lblBuscar.TabIndex = 6;
            lblBuscar.Text = "Buscar:";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(570, 16);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.PlaceholderText = "Nombre o DNI...";
            txtBuscar.Size = new Size(130, 23);
            txtBuscar.TabIndex = 7;
            // 
            // btnFiltrar
            // 
            btnFiltrar.BackColor = Color.FromArgb(15, 118, 110);
            btnFiltrar.FlatStyle = FlatStyle.Flat;
            btnFiltrar.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnFiltrar.ForeColor = Color.White;
            btnFiltrar.Location = new Point(712, 13);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(95, 29);
            btnFiltrar.TabIndex = 8;
            btnFiltrar.Text = "Filtrar";
            btnFiltrar.UseVisualStyleBackColor = false;
            // 
            // btnExportar
            // 
            btnExportar.BackColor = Color.FromArgb(30, 41, 59);
            btnExportar.FlatStyle = FlatStyle.Flat;
            btnExportar.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnExportar.ForeColor = Color.White;
            btnExportar.Location = new Point(818, 13);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(135, 29);
            btnExportar.TabIndex = 9;
            btnExportar.Text = "📄 Exportar Reporte";
            btnExportar.UseVisualStyleBackColor = false;
            // 
            // pnlCardGrilla
            // 
            pnlCardGrilla.BackColor = Color.White;
            pnlCardGrilla.BorderStyle = BorderStyle.FixedSingle;
            pnlCardGrilla.Controls.Add(dgvAtenciones);
            pnlCardGrilla.Controls.Add(lblTotalAtenciones);
            pnlCardGrilla.Dock = DockStyle.Fill;
            pnlCardGrilla.Location = new Point(12, 74);
            pnlCardGrilla.Name = "pnlCardGrilla";
            pnlCardGrilla.Padding = new Padding(10);
            pnlCardGrilla.Size = new Size(1076, 290);
            pnlCardGrilla.TabIndex = 1;
            // 
            // dgvAtenciones
            // 
            dgvAtenciones.AllowUserToAddRows = false;
            dgvAtenciones.AllowUserToDeleteRows = false;
            dgvAtenciones.BackgroundColor = Color.FromArgb(248, 250, 252);
            dgvAtenciones.BorderStyle = BorderStyle.None;
            dgvAtenciones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAtenciones.Dock = DockStyle.Fill;
            dgvAtenciones.Location = new Point(10, 10);
            dgvAtenciones.MultiSelect = false;
            dgvAtenciones.Name = "dgvAtenciones";
            dgvAtenciones.ReadOnly = true;
            dgvAtenciones.RowHeadersVisible = false;
            dgvAtenciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAtenciones.Size = new Size(1054, 245);
            dgvAtenciones.TabIndex = 0;
            // 
            // lblTotalAtenciones
            // 
            lblTotalAtenciones.Dock = DockStyle.Bottom;
            lblTotalAtenciones.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTotalAtenciones.ForeColor = Color.FromArgb(71, 85, 105);
            lblTotalAtenciones.Location = new Point(10, 255);
            lblTotalAtenciones.Name = "lblTotalAtenciones";
            lblTotalAtenciones.Size = new Size(1054, 23);
            lblTotalAtenciones.TabIndex = 1;
            lblTotalAtenciones.Text = "Total de atenciones: 0";
            lblTotalAtenciones.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlCardDetalle
            // 
            pnlCardDetalle.BackColor = Color.White;
            pnlCardDetalle.BorderStyle = BorderStyle.FixedSingle;
            pnlCardDetalle.Controls.Add(txtDetalleReceta);
            pnlCardDetalle.Controls.Add(lblTituloReceta);
            pnlCardDetalle.Controls.Add(txtDetalleEvolucion);
            pnlCardDetalle.Controls.Add(lblTituloEvolucion);
            pnlCardDetalle.Controls.Add(lblSubtituloDetalle);
            pnlCardDetalle.Dock = DockStyle.Bottom;
            pnlCardDetalle.Location = new Point(12, 370);
            pnlCardDetalle.Name = "pnlCardDetalle";
            pnlCardDetalle.Padding = new Padding(10);
            pnlCardDetalle.Size = new Size(1076, 158);
            pnlCardDetalle.TabIndex = 2;
            // 
            // lblSubtituloDetalle
            // 
            lblSubtituloDetalle.AutoSize = true;
            lblSubtituloDetalle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSubtituloDetalle.ForeColor = Color.FromArgb(15, 118, 110);
            lblSubtituloDetalle.Location = new Point(10, 8);
            lblSubtituloDetalle.Name = "lblSubtituloDetalle";
            lblSubtituloDetalle.Size = new Size(318, 15);
            lblSubtituloDetalle.TabIndex = 0;
            lblSubtituloDetalle.Text = "Detalle Clínico y Prescripción de la Consulta Seleccionada";
            // 
            // lblTituloEvolucion
            // 
            lblTituloEvolucion.AutoSize = true;
            lblTituloEvolucion.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblTituloEvolucion.ForeColor = Color.FromArgb(71, 85, 105);
            lblTituloEvolucion.Location = new Point(10, 28);
            lblTituloEvolucion.Name = "lblTituloEvolucion";
            lblTituloEvolucion.Size = new Size(114, 13);
            lblTituloEvolucion.TabIndex = 1;
            lblTituloEvolucion.Text = "Diagnóstico Médico:";
            // 
            // txtDetalleEvolucion
            // 
            txtDetalleEvolucion.BackColor = Color.FromArgb(248, 250, 252);
            txtDetalleEvolucion.Font = new Font("Segoe UI", 8.5F);
            txtDetalleEvolucion.Location = new Point(10, 45);
            txtDetalleEvolucion.Multiline = true;
            txtDetalleEvolucion.Name = "txtDetalleEvolucion";
            txtDetalleEvolucion.ReadOnly = true;
            txtDetalleEvolucion.ScrollBars = ScrollBars.Vertical;
            txtDetalleEvolucion.Size = new Size(510, 95);
            txtDetalleEvolucion.TabIndex = 2;
            // 
            // lblTituloReceta
            // 
            lblTituloReceta.AutoSize = true;
            lblTituloReceta.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblTituloReceta.ForeColor = Color.FromArgb(71, 85, 105);
            lblTituloReceta.Location = new Point(530, 28);
            lblTituloReceta.Name = "lblTituloReceta";
            lblTituloReceta.Size = new Size(211, 13);
            lblTituloReceta.TabIndex = 3;
            lblTituloReceta.Text = "Receta / Indicaciones Farmacológicas:";
            // 
            // txtDetalleReceta
            // 
            txtDetalleReceta.BackColor = Color.FromArgb(248, 250, 252);
            txtDetalleReceta.Font = new Font("Segoe UI", 8.5F);
            txtDetalleReceta.Location = new Point(530, 45);
            txtDetalleReceta.Multiline = true;
            txtDetalleReceta.Name = "txtDetalleReceta";
            txtDetalleReceta.ReadOnly = true;
            txtDetalleReceta.ScrollBars = ScrollBars.Vertical;
            txtDetalleReceta.Size = new Size(530, 95);
            txtDetalleReceta.TabIndex = 4;
            // 
            // FrmMisAtenciones
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 600);
            Controls.Add(pnlContenedorPrincipal);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmMisAtenciones";
            Text = "Mis Atenciones Realizadas";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContenedorPrincipal.ResumeLayout(false);
            pnlCardDetalle.ResumeLayout(false);
            pnlCardDetalle.PerformLayout();
            pnlCardGrilla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvAtenciones).EndInit();
            pnlFiltros.ResumeLayout(false);
            pnlFiltros.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblSubtituloHeader;
        private Label lblTituloHeader;
        private Panel pnlContenedorPrincipal;
        private Panel pnlFiltros;
        private Label lblPeriodo;
        private ComboBox cmbPeriodo;
        private Label lblDesde;
        private DateTimePicker dtpDesde;
        private Label lblHasta;
        private DateTimePicker dtpHasta;
        private Label lblBuscar;
        private TextBox txtBuscar;
        private Button btnFiltrar;
        private Button btnExportar;
        private Panel pnlCardGrilla;
        private DataGridView dgvAtenciones;
        private Label lblTotalAtenciones;
        private Panel pnlCardDetalle;
        private Label lblSubtituloDetalle;
        private Label lblTituloEvolucion;
        private TextBox txtDetalleEvolucion;
        private Label lblTituloReceta;
        private TextBox txtDetalleReceta;
        private TabControl tabMisAtenciones;
        private TabPage tabConsultas;
        private TabPage tabTriageClinico;
        private SplitContainer splitTriage;
        private Panel pnlGravedad;
        private Label lblTituloGravedad;
        private DataGridView dgvRankingGravedad;
        private Panel pnlSintomas;
        private Label lblTituloSintomas;
        private DataGridView dgvRankingSintomas;
    }
}
