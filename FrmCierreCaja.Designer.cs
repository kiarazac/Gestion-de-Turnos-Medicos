namespace Gestion_de_Turnos_Medicos
{
    partial class FrmCierreCaja
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
            btnExportarPdf = new Button();
            btnConsultar = new Button();
            dtpFechaCaja = new DateTimePicker();
            lblFechaCaja = new Label();
            pnlKpis = new Panel();
            pnlKpiFact = new Panel();
            lblSubKpiFacturacion = new Label();
            lblKpiFacturacion = new Label();
            lblTituloFact = new Label();
            pnlKpiCob = new Panel();
            lblSubKpiCobertura = new Label();
            lblKpiCobertura = new Label();
            lblTituloCob = new Label();
            pnlKpiRec = new Panel();
            lblSubKpiRecaudacion = new Label();
            lblKpiRecaudacion = new Label();
            lblTituloRec = new Label();
            pnlKpiTurnos = new Panel();
            lblSubKpiTurnos = new Label();
            lblKpiTurnos = new Label();
            lblTituloTurnos = new Label();
            pnlGrilla = new Panel();
            dgvTurnosCaja = new DataGridView();
            pnlHeader.SuspendLayout();
            pnlFiltros.SuspendLayout();
            pnlKpis.SuspendLayout();
            pnlKpiFact.SuspendLayout();
            pnlKpiCob.SuspendLayout();
            pnlKpiRec.SuspendLayout();
            pnlKpiTurnos.SuspendLayout();
            pnlGrilla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTurnosCaja).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.SteelBlue;
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
            lblSubtitulo.ForeColor = Color.FromArgb(224, 242, 254);
            lblSubtitulo.Location = new Point(20, 36);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(475, 17);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Control diario de turnos emitidos, cobros en ventanilla y coberturas de obras sociales";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(18, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(335, 25);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Cierre Diario de Caja — Recepción";
            // 
            // pnlFiltros
            // 
            pnlFiltros.BackColor = Color.White;
            pnlFiltros.Controls.Add(btnExportarPdf);
            pnlFiltros.Controls.Add(btnConsultar);
            pnlFiltros.Controls.Add(dtpFechaCaja);
            pnlFiltros.Controls.Add(lblFechaCaja);
            pnlFiltros.Dock = DockStyle.Top;
            pnlFiltros.Location = new Point(0, 65);
            pnlFiltros.Name = "pnlFiltros";
            pnlFiltros.Size = new Size(1008, 52);
            pnlFiltros.TabIndex = 1;
            // 
            // btnExportarPdf
            // 
            btnExportarPdf.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportarPdf.BackColor = Color.FromArgb(15, 118, 110);
            btnExportarPdf.Cursor = Cursors.Hand;
            btnExportarPdf.FlatStyle = FlatStyle.Flat;
            btnExportarPdf.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportarPdf.ForeColor = Color.White;
            btnExportarPdf.Location = new Point(780, 11);
            btnExportarPdf.Name = "btnExportarPdf";
            btnExportarPdf.Size = new Size(210, 30);
            btnExportarPdf.TabIndex = 3;
            btnExportarPdf.Text = "📄 Exportar Cierre a PDF";
            btnExportarPdf.UseVisualStyleBackColor = false;
            // 
            // btnConsultar
            // 
            btnConsultar.BackColor = Color.SteelBlue;
            btnConsultar.Cursor = Cursors.Hand;
            btnConsultar.FlatStyle = FlatStyle.Flat;
            btnConsultar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnConsultar.ForeColor = Color.White;
            btnConsultar.Location = new Point(275, 11);
            btnConsultar.Name = "btnConsultar";
            btnConsultar.Size = new Size(160, 30);
            btnConsultar.TabIndex = 2;
            btnConsultar.Text = "🔄 Consultar Fecha";
            btnConsultar.UseVisualStyleBackColor = false;
            // 
            // dtpFechaCaja
            // 
            dtpFechaCaja.Format = DateTimePickerFormat.Short;
            dtpFechaCaja.Location = new Point(135, 15);
            dtpFechaCaja.Name = "dtpFechaCaja";
            dtpFechaCaja.Size = new Size(125, 23);
            dtpFechaCaja.TabIndex = 1;
            // 
            // lblFechaCaja
            // 
            lblFechaCaja.AutoSize = true;
            lblFechaCaja.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblFechaCaja.Location = new Point(20, 17);
            lblFechaCaja.Name = "lblFechaCaja";
            lblFechaCaja.Size = new Size(106, 17);
            lblFechaCaja.TabIndex = 0;
            lblFechaCaja.Text = "Fecha del Cierre:";
            // 
            // pnlKpis
            // 
            pnlKpis.BackColor = Color.FromArgb(241, 245, 249);
            pnlKpis.Controls.Add(pnlKpiFact);
            pnlKpis.Controls.Add(pnlKpiCob);
            pnlKpis.Controls.Add(pnlKpiRec);
            pnlKpis.Controls.Add(pnlKpiTurnos);
            pnlKpis.Dock = DockStyle.Top;
            pnlKpis.Location = new Point(0, 117);
            pnlKpis.Name = "pnlKpis";
            pnlKpis.Padding = new Padding(10);
            pnlKpis.Size = new Size(1008, 80);
            pnlKpis.TabIndex = 2;
            // 
            // pnlKpiFact
            // 
            pnlKpiFact.BackColor = Color.White;
            pnlKpiFact.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiFact.Controls.Add(lblSubKpiFacturacion);
            pnlKpiFact.Controls.Add(lblKpiFacturacion);
            pnlKpiFact.Controls.Add(lblTituloFact);
            pnlKpiFact.Location = new Point(730, 8);
            pnlKpiFact.Name = "pnlKpiFact";
            pnlKpiFact.Size = new Size(230, 64);
            pnlKpiFact.TabIndex = 3;
            // 
            // lblSubKpiFacturacion
            // 
            lblSubKpiFacturacion.AutoSize = true;
            lblSubKpiFacturacion.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiFacturacion.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiFacturacion.Location = new Point(12, 45);
            lblSubKpiFacturacion.Name = "lblSubKpiFacturacion";
            lblSubKpiFacturacion.Size = new Size(142, 12);
            lblSubKpiFacturacion.TabIndex = 2;
            lblSubKpiFacturacion.Text = "Promedio Caja: $ 0,00 / turno";
            // 
            // lblKpiFacturacion
            // 
            lblKpiFacturacion.AutoSize = true;
            lblKpiFacturacion.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblKpiFacturacion.ForeColor = Color.FromArgb(30, 41, 59);
            lblKpiFacturacion.Location = new Point(12, 23);
            lblKpiFacturacion.Name = "lblKpiFacturacion";
            lblKpiFacturacion.Size = new Size(55, 21);
            lblKpiFacturacion.TabIndex = 1;
            lblKpiFacturacion.Text = "$ 0,00";
            // 
            // lblTituloFact
            // 
            lblTituloFact.AutoSize = true;
            lblTituloFact.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloFact.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloFact.Location = new Point(12, 7);
            lblTituloFact.Name = "lblTituloFact";
            lblTituloFact.Size = new Size(142, 12);
            lblTituloFact.TabIndex = 0;
            lblTituloFact.Text = "FACTURACIÓN BRUTA TOTAL";
            // 
            // pnlKpiCob
            // 
            pnlKpiCob.BackColor = Color.White;
            pnlKpiCob.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiCob.Controls.Add(lblSubKpiCobertura);
            pnlKpiCob.Controls.Add(lblKpiCobertura);
            pnlKpiCob.Controls.Add(lblTituloCob);
            pnlKpiCob.Location = new Point(490, 8);
            pnlKpiCob.Name = "pnlKpiCob";
            pnlKpiCob.Size = new Size(230, 64);
            pnlKpiCob.TabIndex = 2;
            // 
            // lblSubKpiCobertura
            // 
            lblSubKpiCobertura.AutoSize = true;
            lblSubKpiCobertura.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiCobertura.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiCobertura.Location = new Point(12, 45);
            lblSubKpiCobertura.Name = "lblSubKpiCobertura";
            lblSubKpiCobertura.Size = new Size(153, 12);
            lblSubKpiCobertura.TabIndex = 2;
            lblSubKpiCobertura.Text = "70% aportado por Obras Sociales";
            // 
            // lblKpiCobertura
            // 
            lblKpiCobertura.AutoSize = true;
            lblKpiCobertura.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblKpiCobertura.ForeColor = Color.FromArgb(3, 105, 161);
            lblKpiCobertura.Location = new Point(12, 23);
            lblKpiCobertura.Name = "lblKpiCobertura";
            lblKpiCobertura.Size = new Size(55, 21);
            lblKpiCobertura.TabIndex = 1;
            lblKpiCobertura.Text = "$ 0,00";
            // 
            // lblTituloCob
            // 
            lblTituloCob.AutoSize = true;
            lblTituloCob.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloCob.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloCob.Location = new Point(12, 7);
            lblTituloCob.Name = "lblTituloCob";
            lblTituloCob.Size = new Size(147, 12);
            lblTituloCob.TabIndex = 0;
            lblTituloCob.Text = "COBERTURA OBRAS SOCIALES";
            // 
            // pnlKpiRec
            // 
            pnlKpiRec.BackColor = Color.White;
            pnlKpiRec.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiRec.Controls.Add(lblSubKpiRecaudacion);
            pnlKpiRec.Controls.Add(lblKpiRecaudacion);
            pnlKpiRec.Controls.Add(lblTituloRec);
            pnlKpiRec.Location = new Point(250, 8);
            pnlKpiRec.Name = "pnlKpiRec";
            pnlKpiRec.Size = new Size(230, 64);
            pnlKpiRec.TabIndex = 1;
            // 
            // lblSubKpiRecaudacion
            // 
            lblSubKpiRecaudacion.AutoSize = true;
            lblSubKpiRecaudacion.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiRecaudacion.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiRecaudacion.Location = new Point(12, 45);
            lblSubKpiRecaudacion.Name = "lblSubKpiRecaudacion";
            lblSubKpiRecaudacion.Size = new Size(130, 12);
            lblSubKpiRecaudacion.TabIndex = 2;
            lblSubKpiRecaudacion.Text = "Cobrado en ventanilla / caja";
            // 
            // lblKpiRecaudacion
            // 
            lblKpiRecaudacion.AutoSize = true;
            lblKpiRecaudacion.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblKpiRecaudacion.ForeColor = Color.FromArgb(22, 101, 52);
            lblKpiRecaudacion.Location = new Point(12, 23);
            lblKpiRecaudacion.Name = "lblKpiRecaudacion";
            lblKpiRecaudacion.Size = new Size(55, 21);
            lblKpiRecaudacion.TabIndex = 1;
            lblKpiRecaudacion.Text = "$ 0,00";
            // 
            // lblTituloRec
            // 
            lblTituloRec.AutoSize = true;
            lblTituloRec.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloRec.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloRec.Location = new Point(12, 7);
            lblTituloRec.Name = "lblTituloRec";
            lblTituloRec.Size = new Size(147, 12);
            lblTituloRec.TabIndex = 0;
            lblTituloRec.Text = "RECAUDACIÓN CAJA (EFECTIVO)";
            // 
            // pnlKpiTurnos
            // 
            pnlKpiTurnos.BackColor = Color.White;
            pnlKpiTurnos.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiTurnos.Controls.Add(lblSubKpiTurnos);
            pnlKpiTurnos.Controls.Add(lblKpiTurnos);
            pnlKpiTurnos.Controls.Add(lblTituloTurnos);
            pnlKpiTurnos.Location = new Point(10, 8);
            pnlKpiTurnos.Name = "pnlKpiTurnos";
            pnlKpiTurnos.Size = new Size(230, 64);
            pnlKpiTurnos.TabIndex = 0;
            // 
            // lblSubKpiTurnos
            // 
            lblSubKpiTurnos.AutoSize = true;
            lblSubKpiTurnos.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiTurnos.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiTurnos.Location = new Point(12, 45);
            lblSubKpiTurnos.Name = "lblSubKpiTurnos";
            lblSubKpiTurnos.Size = new Size(111, 12);
            lblSubKpiTurnos.TabIndex = 2;
            lblSubKpiTurnos.Text = "Esp: 0 | Emergencia: 0";
            // 
            // lblKpiTurnos
            // 
            lblKpiTurnos.AutoSize = true;
            lblKpiTurnos.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
            lblKpiTurnos.ForeColor = Color.FromArgb(15, 118, 110);
            lblKpiTurnos.Location = new Point(12, 21);
            lblKpiTurnos.Name = "lblKpiTurnos";
            lblKpiTurnos.Size = new Size(22, 25);
            lblKpiTurnos.TabIndex = 1;
            lblKpiTurnos.Text = "0";
            // 
            // lblTituloTurnos
            // 
            lblTituloTurnos.AutoSize = true;
            lblTituloTurnos.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloTurnos.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloTurnos.Location = new Point(12, 7);
            lblTituloTurnos.Name = "lblTituloTurnos";
            lblTituloTurnos.Size = new Size(130, 12);
            lblTituloTurnos.TabIndex = 0;
            lblTituloTurnos.Text = "TOTAL TURNOS EMITIDOS";
            // 
            // pnlGrilla
            // 
            pnlGrilla.Controls.Add(dgvTurnosCaja);
            pnlGrilla.Dock = DockStyle.Fill;
            pnlGrilla.Location = new Point(0, 197);
            pnlGrilla.Name = "pnlGrilla";
            pnlGrilla.Padding = new Padding(10);
            pnlGrilla.Size = new Size(1008, 483);
            pnlGrilla.TabIndex = 3;
            // 
            // dgvTurnosCaja
            // 
            dgvTurnosCaja.BackgroundColor = Color.White;
            dgvTurnosCaja.BorderStyle = BorderStyle.None;
            dgvTurnosCaja.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTurnosCaja.Dock = DockStyle.Fill;
            dgvTurnosCaja.Location = new Point(10, 10);
            dgvTurnosCaja.Name = "dgvTurnosCaja";
            dgvTurnosCaja.Size = new Size(988, 463);
            dgvTurnosCaja.TabIndex = 0;
            // 
            // FrmCierreCaja
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(1008, 680);
            Controls.Add(pnlGrilla);
            Controls.Add(pnlKpis);
            Controls.Add(pnlFiltros);
            Controls.Add(pnlHeader);
            Name = "FrmCierreCaja";
            Text = "Cierre Diario de Caja — Recepción";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFiltros.ResumeLayout(false);
            pnlFiltros.PerformLayout();
            pnlKpis.ResumeLayout(false);
            pnlKpiFact.ResumeLayout(false);
            pnlKpiFact.PerformLayout();
            pnlKpiCob.ResumeLayout(false);
            pnlKpiCob.PerformLayout();
            pnlKpiRec.ResumeLayout(false);
            pnlKpiRec.PerformLayout();
            pnlKpiTurnos.ResumeLayout(false);
            pnlKpiTurnos.PerformLayout();
            pnlGrilla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTurnosCaja).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Panel pnlFiltros;
        private Label lblFechaCaja;
        private DateTimePicker dtpFechaCaja;
        private Button btnConsultar;
        private Button btnExportarPdf;
        private Panel pnlKpis;
        private Panel pnlKpiTurnos;
        private Label lblTituloTurnos;
        private Label lblKpiTurnos;
        private Label lblSubKpiTurnos;
        private Panel pnlKpiRec;
        private Label lblSubKpiRecaudacion;
        private Label lblKpiRecaudacion;
        private Label lblTituloRec;
        private Panel pnlKpiCob;
        private Label lblSubKpiCobertura;
        private Label lblKpiCobertura;
        private Label lblTituloCob;
        private Panel pnlKpiFact;
        private Label lblSubKpiFacturacion;
        private Label lblKpiFacturacion;
        private Label lblTituloFact;
        private Panel pnlGrilla;
        private DataGridView dgvTurnosCaja;
    }
}
