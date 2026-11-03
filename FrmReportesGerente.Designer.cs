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
            tlpKpis = new TableLayoutPanel();
            tabReportes = new TabControl();
            tabEvolucion = new TabPage();
            dgvEvolucion = new DataGridView();
            tabRankingMedicos = new TabPage();
            dgvRankingMedicos = new DataGridView();
            pnlFiltroEspecialidad = new Panel();
            cmbEspecialidad = new ComboBox();
            lblEspecialidad = new Label();
            tabRankingEspecialidades = new TabPage();
            dgvRankingEspecialidades = new DataGridView();
            tabRankingObrasSociales = new TabPage();
            dgvRankingObrasSociales = new DataGridView();
            lblLeyendaCobertura = new Label();
            tabSegmentos = new TabPage();
            dgvSegmentos = new DataGridView();
            pnlHeader.SuspendLayout();
            pnlFiltros.SuspendLayout();
            tabReportes.SuspendLayout();
            tabEvolucion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEvolucion).BeginInit();
            tabRankingMedicos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRankingMedicos).BeginInit();
            pnlFiltroEspecialidad.SuspendLayout();
            tabRankingEspecialidades.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRankingEspecialidades).BeginInit();
            tabRankingObrasSociales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRankingObrasSociales).BeginInit();
            tabSegmentos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSegmentos).BeginInit();
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
            lblSubtitulo.Size = new Size(640, 17);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Control del flujo de dinero: caja, obras sociales, evolución diaria y rankings de facturación";
            //
            // lblTitulo
            //
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(18, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(460, 25);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Panel Gerencial de Recaudación";
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
            // tlpKpis
            //
            tlpKpis.BackColor = Color.FromArgb(241, 245, 249);
            tlpKpis.ColumnCount = 6;
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.7F));
            tlpKpis.Dock = DockStyle.Top;
            tlpKpis.Location = new Point(0, 117);
            tlpKpis.Name = "tlpKpis";
            tlpKpis.Padding = new Padding(6);
            tlpKpis.RowCount = 1;
            tlpKpis.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpKpis.Size = new Size(1008, 96);
            tlpKpis.TabIndex = 2;
            //
            // tabReportes
            //
            tabReportes.Controls.Add(tabEvolucion);
            tabReportes.Controls.Add(tabRankingMedicos);
            tabReportes.Controls.Add(tabRankingEspecialidades);
            tabReportes.Controls.Add(tabRankingObrasSociales);
            tabReportes.Controls.Add(tabSegmentos);
            tabReportes.Dock = DockStyle.Fill;
            tabReportes.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            tabReportes.Location = new Point(0, 213);
            tabReportes.Name = "tabReportes";
            tabReportes.SelectedIndex = 0;
            tabReportes.Size = new Size(1008, 467);
            tabReportes.TabIndex = 3;
            //
            // tabEvolucion
            //
            tabEvolucion.Controls.Add(dgvEvolucion);
            tabEvolucion.Location = new Point(4, 26);
            tabEvolucion.Name = "tabEvolucion";
            tabEvolucion.Padding = new Padding(5);
            tabEvolucion.Size = new Size(1000, 437);
            tabEvolucion.TabIndex = 0;
            tabEvolucion.Text = "  📈 Evolución de la Recaudación  ";
            tabEvolucion.UseVisualStyleBackColor = true;
            //
            // dgvEvolucion
            //
            dgvEvolucion.BackgroundColor = Color.White;
            dgvEvolucion.BorderStyle = BorderStyle.None;
            dgvEvolucion.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEvolucion.Dock = DockStyle.Fill;
            dgvEvolucion.Location = new Point(5, 5);
            dgvEvolucion.Name = "dgvEvolucion";
            dgvEvolucion.Size = new Size(990, 427);
            dgvEvolucion.TabIndex = 0;
            //
            // tabRankingMedicos
            //
            tabRankingMedicos.Controls.Add(dgvRankingMedicos);
            tabRankingMedicos.Controls.Add(pnlFiltroEspecialidad);
            tabRankingMedicos.Location = new Point(4, 26);
            tabRankingMedicos.Name = "tabRankingMedicos";
            tabRankingMedicos.Padding = new Padding(5);
            tabRankingMedicos.Size = new Size(1000, 437);
            tabRankingMedicos.TabIndex = 1;
            tabRankingMedicos.Text = "  🏆 Ranking de Médicos  ";
            tabRankingMedicos.UseVisualStyleBackColor = true;
            //
            // dgvRankingMedicos
            //
            dgvRankingMedicos.BackgroundColor = Color.White;
            dgvRankingMedicos.BorderStyle = BorderStyle.None;
            dgvRankingMedicos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRankingMedicos.Dock = DockStyle.Fill;
            dgvRankingMedicos.Location = new Point(5, 43);
            dgvRankingMedicos.Name = "dgvRankingMedicos";
            dgvRankingMedicos.Size = new Size(990, 389);
            dgvRankingMedicos.TabIndex = 1;
            //
            // pnlFiltroEspecialidad
            //
            pnlFiltroEspecialidad.Controls.Add(cmbEspecialidad);
            pnlFiltroEspecialidad.Controls.Add(lblEspecialidad);
            pnlFiltroEspecialidad.Dock = DockStyle.Top;
            pnlFiltroEspecialidad.Location = new Point(5, 5);
            pnlFiltroEspecialidad.Name = "pnlFiltroEspecialidad";
            pnlFiltroEspecialidad.Size = new Size(990, 38);
            pnlFiltroEspecialidad.TabIndex = 0;
            //
            // cmbEspecialidad
            //
            cmbEspecialidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEspecialidad.Font = new Font("Segoe UI", 9F);
            cmbEspecialidad.FormattingEnabled = true;
            cmbEspecialidad.Location = new Point(185, 6);
            cmbEspecialidad.Name = "cmbEspecialidad";
            cmbEspecialidad.Size = new Size(220, 23);
            cmbEspecialidad.TabIndex = 1;
            //
            // lblEspecialidad
            //
            lblEspecialidad.AutoSize = true;
            lblEspecialidad.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEspecialidad.Location = new Point(3, 9);
            lblEspecialidad.Name = "lblEspecialidad";
            lblEspecialidad.Size = new Size(176, 15);
            lblEspecialidad.TabIndex = 0;
            lblEspecialidad.Text = "Especialidad de los turnos:";
            //
            // tabRankingEspecialidades
            //
            tabRankingEspecialidades.Controls.Add(dgvRankingEspecialidades);
            tabRankingEspecialidades.Location = new Point(4, 26);
            tabRankingEspecialidades.Name = "tabRankingEspecialidades";
            tabRankingEspecialidades.Padding = new Padding(5);
            tabRankingEspecialidades.Size = new Size(1000, 437);
            tabRankingEspecialidades.TabIndex = 2;
            tabRankingEspecialidades.Text = "  🩺 Ranking de Especialidades  ";
            tabRankingEspecialidades.UseVisualStyleBackColor = true;
            //
            // dgvRankingEspecialidades
            //
            dgvRankingEspecialidades.BackgroundColor = Color.White;
            dgvRankingEspecialidades.BorderStyle = BorderStyle.None;
            dgvRankingEspecialidades.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRankingEspecialidades.Dock = DockStyle.Fill;
            dgvRankingEspecialidades.Location = new Point(5, 5);
            dgvRankingEspecialidades.Name = "dgvRankingEspecialidades";
            dgvRankingEspecialidades.Size = new Size(990, 427);
            dgvRankingEspecialidades.TabIndex = 0;
            //
            // tabRankingObrasSociales
            //
            tabRankingObrasSociales.Controls.Add(dgvRankingObrasSociales);
            tabRankingObrasSociales.Controls.Add(lblLeyendaCobertura);
            tabRankingObrasSociales.Location = new Point(4, 26);
            tabRankingObrasSociales.Name = "tabRankingObrasSociales";
            tabRankingObrasSociales.Padding = new Padding(5);
            tabRankingObrasSociales.Size = new Size(1000, 437);
            tabRankingObrasSociales.TabIndex = 3;
            tabRankingObrasSociales.Text = "  🏥 Ranking de Obras Sociales  ";
            tabRankingObrasSociales.UseVisualStyleBackColor = true;
            //
            // dgvRankingObrasSociales
            //
            dgvRankingObrasSociales.BackgroundColor = Color.White;
            dgvRankingObrasSociales.BorderStyle = BorderStyle.None;
            dgvRankingObrasSociales.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRankingObrasSociales.Dock = DockStyle.Fill;
            dgvRankingObrasSociales.Location = new Point(5, 5);
            dgvRankingObrasSociales.Name = "dgvRankingObrasSociales";
            dgvRankingObrasSociales.Size = new Size(990, 399);
            dgvRankingObrasSociales.TabIndex = 0;
            //
            // lblLeyendaCobertura
            //
            lblLeyendaCobertura.Dock = DockStyle.Bottom;
            lblLeyendaCobertura.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            lblLeyendaCobertura.ForeColor = Color.FromArgb(100, 116, 139);
            lblLeyendaCobertura.Location = new Point(5, 404);
            lblLeyendaCobertura.Name = "lblLeyendaCobertura";
            lblLeyendaCobertura.Size = new Size(990, 28);
            lblLeyendaCobertura.TabIndex = 1;
            lblLeyendaCobertura.Text = "⚠ La cobertura aplicada (según lo cobrado en caja) difiere de la vigente: el % de la obra social se modificó en el período o se cobró un monto distinto al configurado.";
            lblLeyendaCobertura.TextAlign = ContentAlignment.MiddleLeft;
            //
            // tabSegmentos
            //
            tabSegmentos.Controls.Add(dgvSegmentos);
            tabSegmentos.Location = new Point(4, 26);
            tabSegmentos.Name = "tabSegmentos";
            tabSegmentos.Padding = new Padding(5);
            tabSegmentos.Size = new Size(1000, 437);
            tabSegmentos.TabIndex = 4;
            tabSegmentos.Text = "  📊 Modalidad y Cobertura  ";
            tabSegmentos.UseVisualStyleBackColor = true;
            //
            // dgvSegmentos
            //
            dgvSegmentos.BackgroundColor = Color.White;
            dgvSegmentos.BorderStyle = BorderStyle.None;
            dgvSegmentos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSegmentos.Dock = DockStyle.Fill;
            dgvSegmentos.Location = new Point(5, 5);
            dgvSegmentos.Name = "dgvSegmentos";
            dgvSegmentos.Size = new Size(990, 427);
            dgvSegmentos.TabIndex = 0;
            //
            // FrmReportesGerente
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(1008, 680);
            Controls.Add(tabReportes);
            Controls.Add(tlpKpis);
            Controls.Add(pnlFiltros);
            Controls.Add(pnlHeader);
            Name = "FrmReportesGerente";
            Text = "Panel Gerencial de Recaudación";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFiltros.ResumeLayout(false);
            pnlFiltros.PerformLayout();
            tabReportes.ResumeLayout(false);
            tabEvolucion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvEvolucion).EndInit();
            tabRankingMedicos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRankingMedicos).EndInit();
            pnlFiltroEspecialidad.ResumeLayout(false);
            pnlFiltroEspecialidad.PerformLayout();
            tabRankingEspecialidades.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRankingEspecialidades).EndInit();
            tabRankingObrasSociales.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRankingObrasSociales).EndInit();
            tabSegmentos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvSegmentos).EndInit();
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
        private TableLayoutPanel tlpKpis;
        private TabControl tabReportes;
        private TabPage tabEvolucion;
        private DataGridView dgvEvolucion;
        private TabPage tabRankingMedicos;
        private DataGridView dgvRankingMedicos;
        private Panel pnlFiltroEspecialidad;
        private Label lblEspecialidad;
        private ComboBox cmbEspecialidad;
        private TabPage tabRankingEspecialidades;
        private DataGridView dgvRankingEspecialidades;
        private TabPage tabRankingObrasSociales;
        private DataGridView dgvRankingObrasSociales;
        private Label lblLeyendaCobertura;
        private TabPage tabSegmentos;
        private DataGridView dgvSegmentos;
    }
}
