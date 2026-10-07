namespace Gestion_de_Turnos_Medicos
{
    partial class FrmReportesGerente
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
            lblSubtitulo = new Label();
            lblTitulo = new Label();
            pnlFiltros = new Panel();
            btnExportar = new Button();
            btnFiltrar = new Button();
            dtpHasta = new DateTimePicker();
            lblHasta = new Label();
            dtpDesde = new DateTimePicker();
            lblDesde = new Label();
            cmbPeriodo = new ComboBox();
            lblPeriodo = new Label();
            pnlKpis = new Panel();
            pnlKpiParticulares = new Panel();
            lblKpiParticulares = new Label();
            lblTituloParticulares = new Label();
            pnlKpiOS = new Panel();
            lblKpiObrasSociales = new Label();
            lblTituloOS = new Label();
            pnlKpiFact = new Panel();
            lblKpiFacturacion = new Label();
            lblTituloFact = new Label();
            pnlKpiCons = new Panel();
            lblKpiConsultas = new Label();
            lblTituloCons = new Label();
            tabReportes = new TabControl();
            tabIngresos = new TabPage();
            dgvIngresos = new DataGridView();
            tabCoberturas = new TabPage();
            dgvCoberturas = new DataGridView();
            tabDemanda = new TabPage();
            dgvDemanda = new DataGridView();
            pnlHeader.SuspendLayout();
            pnlFiltros.SuspendLayout();
            pnlKpis.SuspendLayout();
            pnlKpiParticulares.SuspendLayout();
            pnlKpiOS.SuspendLayout();
            pnlKpiFact.SuspendLayout();
            pnlKpiCons.SuspendLayout();
            tabReportes.SuspendLayout();
            tabIngresos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvIngresos).BeginInit();
            tabCoberturas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCoberturas).BeginInit();
            tabDemanda.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDemanda).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(15, 118, 110);
            pnlHeader.Controls.Add(lblSubtitulo);
            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1008, 65);
            pnlHeader.TabIndex = 0;
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 9.25F);
            lblSubtitulo.ForeColor = Color.FromArgb(204, 251, 241);
            lblSubtitulo.Location = new Point(20, 36);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(495, 17);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Facturación por médico, distribución de obras sociales y análisis de demanda";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(18, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(405, 25);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Panel Gerencial de Facturación y Demanda";
            // 
            // pnlFiltros
            // 
            pnlFiltros.BackColor = Color.White;
            pnlFiltros.Controls.Add(btnExportar);
            pnlFiltros.Controls.Add(btnFiltrar);
            pnlFiltros.Controls.Add(dtpHasta);
            pnlFiltros.Controls.Add(lblHasta);
            pnlFiltros.Controls.Add(dtpDesde);
            pnlFiltros.Controls.Add(lblDesde);
            pnlFiltros.Controls.Add(cmbPeriodo);
            pnlFiltros.Controls.Add(lblPeriodo);
            pnlFiltros.Dock = DockStyle.Top;
            pnlFiltros.Location = new Point(0, 65);
            pnlFiltros.Name = "pnlFiltros";
            pnlFiltros.Size = new Size(1008, 52);
            pnlFiltros.TabIndex = 1;
            // 
            // btnExportar
            // 
            btnExportar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportar.BackColor = Color.FromArgb(15, 118, 110);
            btnExportar.Cursor = Cursors.Hand;
            btnExportar.FlatStyle = FlatStyle.Flat;
            btnExportar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportar.ForeColor = Color.White;
            btnExportar.Location = new Point(840, 11);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(150, 30);
            btnExportar.TabIndex = 7;
            btnExportar.Text = "📄 Exportar a PDF";
            btnExportar.UseVisualStyleBackColor = false;
            // 
            // btnFiltrar
            // 
            btnFiltrar.BackColor = Color.FromArgb(30, 41, 59);
            btnFiltrar.Cursor = Cursors.Hand;
            btnFiltrar.FlatStyle = FlatStyle.Flat;
            btnFiltrar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnFiltrar.ForeColor = Color.White;
            btnFiltrar.Location = new Point(610, 11);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(95, 30);
            btnFiltrar.TabIndex = 6;
            btnFiltrar.Text = "🔍 Filtrar";
            btnFiltrar.UseVisualStyleBackColor = false;
            // 
            // dtpHasta
            // 
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(480, 15);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(110, 23);
            dtpHasta.TabIndex = 5;
            // 
            // lblHasta
            // 
            lblHasta.AutoSize = true;
            lblHasta.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHasta.Location = new Point(435, 18);
            lblHasta.Name = "lblHasta";
            lblHasta.Size = new Size(41, 15);
            lblHasta.TabIndex = 4;
            lblHasta.Text = "Hasta:";
            // 
            // dtpDesde
            // 
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(310, 15);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(110, 23);
            dtpDesde.TabIndex = 3;
            // 
            // lblDesde
            // 
            lblDesde.AutoSize = true;
            lblDesde.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDesde.Location = new Point(260, 18);
            lblDesde.Name = "lblDesde";
            lblDesde.Size = new Size(44, 15);
            lblDesde.TabIndex = 2;
            lblDesde.Text = "Desde:";
            // 
            // cmbPeriodo
            // 
            cmbPeriodo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPeriodo.FormattingEnabled = true;
            cmbPeriodo.Items.AddRange(new object[] { "Hoy", "Últimos 7 días", "Mes actual", "Mes anterior", "Histórico / Todo" });
            cmbPeriodo.Location = new Point(80, 15);
            cmbPeriodo.Name = "cmbPeriodo";
            cmbPeriodo.Size = new Size(160, 23);
            cmbPeriodo.TabIndex = 1;
            // 
            // lblPeriodo
            // 
            lblPeriodo.AutoSize = true;
            lblPeriodo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPeriodo.Location = new Point(20, 18);
            lblPeriodo.Name = "lblPeriodo";
            lblPeriodo.Size = new Size(54, 15);
            lblPeriodo.TabIndex = 0;
            lblPeriodo.Text = "Período:";
            // 
            // pnlKpis
            // 
            pnlKpis.BackColor = Color.FromArgb(241, 245, 249);
            pnlKpis.Controls.Add(pnlKpiParticulares);
            pnlKpis.Controls.Add(pnlKpiOS);
            pnlKpis.Controls.Add(pnlKpiFact);
            pnlKpis.Controls.Add(pnlKpiCons);
            pnlKpis.Dock = DockStyle.Top;
            pnlKpis.Location = new Point(0, 117);
            pnlKpis.Name = "pnlKpis";
            pnlKpis.Padding = new Padding(10);
            pnlKpis.Size = new Size(1008, 75);
            pnlKpis.TabIndex = 2;
            // 
            // pnlKpiParticulares
            // 
            pnlKpiParticulares.BackColor = Color.White;
            pnlKpiParticulares.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiParticulares.Controls.Add(lblKpiParticulares);
            pnlKpiParticulares.Controls.Add(lblTituloParticulares);
            pnlKpiParticulares.Location = new Point(730, 8);
            pnlKpiParticulares.Name = "pnlKpiParticulares";
            pnlKpiParticulares.Size = new Size(220, 58);
            pnlKpiParticulares.TabIndex = 3;
            // 
            // lblKpiParticulares
            // 
            lblKpiParticulares.AutoSize = true;
            lblKpiParticulares.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            lblKpiParticulares.ForeColor = Color.FromArgb(71, 85, 105);
            lblKpiParticulares.Location = new Point(12, 26);
            lblKpiParticulares.Name = "lblKpiParticulares";
            lblKpiParticulares.Size = new Size(59, 23);
            lblKpiParticulares.TabIndex = 1;
            lblKpiParticulares.Text = "$ 0,00";
            // 
            // lblTituloParticulares
            // 
            lblTituloParticulares.AutoSize = true;
            lblTituloParticulares.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloParticulares.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloParticulares.Location = new Point(12, 8);
            lblTituloParticulares.Name = "lblTituloParticulares";
            lblTituloParticulares.Size = new Size(125, 12);
            lblTituloParticulares.TabIndex = 0;
            lblTituloParticulares.Text = "PARTICULARES (100%)";
            // 
            // pnlKpiOS
            // 
            pnlKpiOS.BackColor = Color.White;
            pnlKpiOS.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiOS.Controls.Add(lblKpiObrasSociales);
            pnlKpiOS.Controls.Add(lblTituloOS);
            pnlKpiOS.Location = new Point(490, 8);
            pnlKpiOS.Name = "pnlKpiOS";
            pnlKpiOS.Size = new Size(220, 58);
            pnlKpiOS.TabIndex = 2;
            // 
            // lblKpiObrasSociales
            // 
            lblKpiObrasSociales.AutoSize = true;
            lblKpiObrasSociales.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            lblKpiObrasSociales.ForeColor = Color.FromArgb(3, 105, 161);
            lblKpiObrasSociales.Location = new Point(12, 26);
            lblKpiObrasSociales.Name = "lblKpiObrasSociales";
            lblKpiObrasSociales.Size = new Size(59, 23);
            lblKpiObrasSociales.TabIndex = 1;
            lblKpiObrasSociales.Text = "$ 0,00";
            // 
            // lblTituloOS
            // 
            lblTituloOS.AutoSize = true;
            lblTituloOS.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloOS.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloOS.Location = new Point(12, 8);
            lblTituloOS.Name = "lblTituloOS";
            lblTituloOS.Size = new Size(130, 12);
            lblTituloOS.TabIndex = 0;
            lblTituloOS.Text = "OBRAS SOCIALES (TOTAL)";
            // 
            // pnlKpiFact
            // 
            pnlKpiFact.BackColor = Color.White;
            pnlKpiFact.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiFact.Controls.Add(lblKpiFacturacion);
            pnlKpiFact.Controls.Add(lblTituloFact);
            pnlKpiFact.Location = new Point(250, 8);
            pnlKpiFact.Name = "pnlKpiFact";
            pnlKpiFact.Size = new Size(220, 58);
            pnlKpiFact.TabIndex = 1;
            // 
            // lblKpiFacturacion
            // 
            lblKpiFacturacion.AutoSize = true;
            lblKpiFacturacion.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            lblKpiFacturacion.ForeColor = Color.FromArgb(22, 101, 52);
            lblKpiFacturacion.Location = new Point(12, 26);
            lblKpiFacturacion.Name = "lblKpiFacturacion";
            lblKpiFacturacion.Size = new Size(59, 23);
            lblKpiFacturacion.TabIndex = 1;
            lblKpiFacturacion.Text = "$ 0,00";
            // 
            // lblTituloFact
            // 
            lblTituloFact.AutoSize = true;
            lblTituloFact.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloFact.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloFact.Location = new Point(12, 8);
            lblTituloFact.Name = "lblTituloFact";
            lblTituloFact.Size = new Size(142, 12);
            lblTituloFact.TabIndex = 0;
            lblTituloFact.Text = "FACTURACIÓN GLOBAL BRUTA";
            // 
            // pnlKpiCons
            // 
            pnlKpiCons.BackColor = Color.White;
            pnlKpiCons.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiCons.Controls.Add(lblKpiConsultas);
            pnlKpiCons.Controls.Add(lblTituloCons);
            pnlKpiCons.Location = new Point(10, 8);
            pnlKpiCons.Name = "pnlKpiCons";
            pnlKpiCons.Size = new Size(220, 58);
            pnlKpiCons.TabIndex = 0;
            // 
            // lblKpiConsultas
            // 
            lblKpiConsultas.AutoSize = true;
            lblKpiConsultas.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
            lblKpiConsultas.ForeColor = Color.FromArgb(15, 118, 110);
            lblKpiConsultas.Location = new Point(12, 25);
            lblKpiConsultas.Name = "lblKpiConsultas";
            lblKpiConsultas.Size = new Size(22, 25);
            lblKpiConsultas.TabIndex = 1;
            lblKpiConsultas.Text = "0";
            // 
            // lblTituloCons
            // 
            lblTituloCons.AutoSize = true;
            lblTituloCons.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloCons.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloCons.Location = new Point(12, 8);
            lblTituloCons.Name = "lblTituloCons";
            lblTituloCons.Size = new Size(130, 12);
            lblTituloCons.TabIndex = 0;
            lblTituloCons.Text = "CONSULTAS ATENDIDAS";
            // 
            // tabReportes
            // 
            tabReportes.Controls.Add(tabIngresos);
            tabReportes.Controls.Add(tabCoberturas);
            tabReportes.Controls.Add(tabDemanda);
            tabReportes.Dock = DockStyle.Fill;
            tabReportes.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            tabReportes.Location = new Point(0, 192);
            tabReportes.Name = "tabReportes";
            tabReportes.SelectedIndex = 0;
            tabReportes.Size = new Size(1008, 488);
            tabReportes.TabIndex = 3;
            // 
            // tabIngresos
            // 
            tabIngresos.Controls.Add(dgvIngresos);
            tabIngresos.Location = new Point(4, 25);
            tabIngresos.Name = "tabIngresos";
            tabIngresos.Padding = new Padding(5);
            tabIngresos.Size = new Size(1000, 459);
            tabIngresos.TabIndex = 0;
            tabIngresos.Text = "  💰 Ingresos por Médico  ";
            tabIngresos.UseVisualStyleBackColor = true;
            // 
            // dgvIngresos
            // 
            dgvIngresos.BackgroundColor = Color.White;
            dgvIngresos.BorderStyle = BorderStyle.None;
            dgvIngresos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvIngresos.Dock = DockStyle.Fill;
            dgvIngresos.Location = new Point(5, 5);
            dgvIngresos.Name = "dgvIngresos";
            dgvIngresos.Size = new Size(990, 449);
            dgvIngresos.TabIndex = 0;
            // 
            // tabCoberturas
            // 
            tabCoberturas.Controls.Add(dgvCoberturas);
            tabCoberturas.Location = new Point(4, 25);
            tabCoberturas.Name = "tabCoberturas";
            tabCoberturas.Padding = new Padding(5);
            tabCoberturas.Size = new Size(1000, 459);
            tabCoberturas.TabIndex = 1;
            tabCoberturas.Text = "  🏥 Obras Sociales vs. Particulares  ";
            tabCoberturas.UseVisualStyleBackColor = true;
            // 
            // dgvCoberturas
            // 
            dgvCoberturas.BackgroundColor = Color.White;
            dgvCoberturas.BorderStyle = BorderStyle.None;
            dgvCoberturas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCoberturas.Dock = DockStyle.Fill;
            dgvCoberturas.Location = new Point(5, 5);
            dgvCoberturas.Name = "dgvCoberturas";
            dgvCoberturas.Size = new Size(990, 449);
            dgvCoberturas.TabIndex = 0;
            // 
            // tabDemanda
            // 
            tabDemanda.Controls.Add(dgvDemanda);
            tabDemanda.Location = new Point(4, 25);
            tabDemanda.Name = "tabDemanda";
            tabDemanda.Padding = new Padding(5);
            tabDemanda.Size = new Size(1000, 459);
            tabDemanda.TabIndex = 2;
            tabDemanda.Text = "  📊 Ranking de Demanda  ";
            tabDemanda.UseVisualStyleBackColor = true;
            // 
            // dgvDemanda
            // 
            dgvDemanda.BackgroundColor = Color.White;
            dgvDemanda.BorderStyle = BorderStyle.None;
            dgvDemanda.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDemanda.Dock = DockStyle.Fill;
            dgvDemanda.Location = new Point(5, 5);
            dgvDemanda.Name = "dgvDemanda";
            dgvDemanda.Size = new Size(990, 449);
            dgvDemanda.TabIndex = 0;
            // 
            // FrmReportesGerente
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(1008, 680);
            Controls.Add(tabReportes);
            Controls.Add(pnlKpis);
            Controls.Add(pnlFiltros);
            Controls.Add(pnlHeader);
            Name = "FrmReportesGerente";
            Text = "Panel Gerencial de Facturación y Demanda Médica";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFiltros.ResumeLayout(false);
            pnlFiltros.PerformLayout();
            pnlKpis.ResumeLayout(false);
            pnlKpiParticulares.ResumeLayout(false);
            pnlKpiParticulares.PerformLayout();
            pnlKpiOS.ResumeLayout(false);
            pnlKpiOS.PerformLayout();
            pnlKpiFact.ResumeLayout(false);
            pnlKpiFact.PerformLayout();
            pnlKpiCons.ResumeLayout(false);
            pnlKpiCons.PerformLayout();
            tabReportes.ResumeLayout(false);
            tabIngresos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvIngresos).EndInit();
            tabCoberturas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvCoberturas).EndInit();
            tabDemanda.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvDemanda).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Panel pnlFiltros;
        private Label lblPeriodo;
        private ComboBox cmbPeriodo;
        private Label lblDesde;
        private DateTimePicker dtpDesde;
        private Label lblHasta;
        private DateTimePicker dtpHasta;
        private Button btnFiltrar;
        private Button btnExportar;
        private Panel pnlKpis;
        private Panel pnlKpiCons;
        private Label lblTituloCons;
        private Label lblKpiConsultas;
        private Panel pnlKpiFact;
        private Label lblTituloFact;
        private Label lblKpiFacturacion;
        private Panel pnlKpiOS;
        private Label lblTituloOS;
        private Label lblKpiObrasSociales;
        private Panel pnlKpiParticulares;
        private Label lblTituloParticulares;
        private Label lblKpiParticulares;
        private TabControl tabReportes;
        private TabPage tabIngresos;
        private DataGridView dgvIngresos;
        private TabPage tabCoberturas;
        private DataGridView dgvCoberturas;
        private TabPage tabDemanda;
        private DataGridView dgvDemanda;
    }
}
