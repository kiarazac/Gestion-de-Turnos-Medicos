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
            tabReportes = new TabControl();
            tabJornadaHoy = new TabPage();
            pnlGrillaHoy = new Panel();
            dgvTurnosHoy = new DataGridView();
            pnlKpisHoy = new Panel();
            pnlKpiFactHoy = new Panel();
            lblSubKpiFacturacionHoy = new Label();
            lblKpiFacturacionHoy = new Label();
            lblTituloFactHoy = new Label();
            pnlKpiCobHoy = new Panel();
            lblSubKpiCoberturaHoy = new Label();
            lblKpiCoberturaHoy = new Label();
            lblTituloCobHoy = new Label();
            pnlKpiRecHoy = new Panel();
            lblSubKpiRecaudacionHoy = new Label();
            lblKpiRecaudacionHoy = new Label();
            lblTituloRecHoy = new Label();
            pnlKpiTurnosHoy = new Panel();
            lblSubKpiTurnosHoy = new Label();
            lblKpiTurnosHoy = new Label();
            lblTituloTurnosHoy = new Label();
            pnlFiltrosHoy = new Panel();
            btnExportarPdfHoy = new Button();
            btnActualizarHoy = new Button();
            lblHoyFechaValor = new Label();
            lblHoyFecha = new Label();
            tabConsultaHistorica = new TabPage();
            pnlGrillaFiltro = new Panel();
            dgvTurnosFiltro = new DataGridView();
            pnlKpisFiltro = new Panel();
            pnlKpiFactFiltro = new Panel();
            lblSubKpiFacturacionFiltro = new Label();
            lblKpiFacturacionFiltro = new Label();
            lblTituloFactFiltro = new Label();
            pnlKpiCobFiltro = new Panel();
            lblSubKpiCoberturaFiltro = new Label();
            lblKpiCoberturaFiltro = new Label();
            lblTituloCobFiltro = new Label();
            pnlKpiRecFiltro = new Panel();
            lblSubKpiRecaudacionFiltro = new Label();
            lblKpiRecaudacionFiltro = new Label();
            lblTituloRecFiltro = new Label();
            pnlKpiTurnosFiltro = new Panel();
            lblSubKpiTurnosFiltro = new Label();
            lblKpiTurnosFiltro = new Label();
            lblTituloTurnosFiltro = new Label();
            pnlFiltrosHistorico = new Panel();
            btnExportarPdfFiltro = new Button();
            btnConsultarFiltro = new Button();
            cmbEspecialidadFiltro = new ComboBox();
            lblEspecialidadFiltro = new Label();
            dtpFechaFiltro = new DateTimePicker();
            lblFechaFiltro = new Label();
            pnlHeader.SuspendLayout();
            tabReportes.SuspendLayout();
            tabJornadaHoy.SuspendLayout();
            pnlGrillaHoy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTurnosHoy).BeginInit();
            pnlKpisHoy.SuspendLayout();
            pnlKpiFactHoy.SuspendLayout();
            pnlKpiCobHoy.SuspendLayout();
            pnlKpiRecHoy.SuspendLayout();
            pnlKpiTurnosHoy.SuspendLayout();
            pnlFiltrosHoy.SuspendLayout();
            tabConsultaHistorica.SuspendLayout();
            pnlGrillaFiltro.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTurnosFiltro).BeginInit();
            pnlKpisFiltro.SuspendLayout();
            pnlKpiFactFiltro.SuspendLayout();
            pnlKpiCobFiltro.SuspendLayout();
            pnlKpiRecFiltro.SuspendLayout();
            pnlKpiTurnosFiltro.SuspendLayout();
            pnlFiltrosHistorico.SuspendLayout();
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
            lblSubtitulo.Size = new Size(530, 17);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Listado y detalle de turnos emitidos, facturación correspondiente y conciliación de caja";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(18, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(395, 25);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Reportes y Control de Caja — Recepción";
            // 
            // tabReportes
            // 
            tabReportes.Controls.Add(tabJornadaHoy);
            tabReportes.Controls.Add(tabConsultaHistorica);
            tabReportes.Dock = DockStyle.Fill;
            tabReportes.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            tabReportes.ItemSize = new Size(260, 30);
            tabReportes.Location = new Point(0, 65);
            tabReportes.Name = "tabReportes";
            tabReportes.Padding = new Point(18, 4);
            tabReportes.SelectedIndex = 0;
            tabReportes.Size = new Size(1008, 615);
            tabReportes.TabIndex = 1;
            // 
            // tabJornadaHoy
            // 
            tabJornadaHoy.BackColor = Color.FromArgb(248, 250, 252);
            tabJornadaHoy.Controls.Add(pnlGrillaHoy);
            tabJornadaHoy.Controls.Add(pnlKpisHoy);
            tabJornadaHoy.Controls.Add(pnlFiltrosHoy);
            tabJornadaHoy.Location = new Point(4, 34);
            tabJornadaHoy.Name = "tabJornadaHoy";
            tabJornadaHoy.Padding = new Padding(3);
            tabJornadaHoy.Size = new Size(1000, 577);
            tabJornadaHoy.TabIndex = 0;
            tabJornadaHoy.Text = "📅  Turnos Emitidos en el Día (Fecha Actual)";
            // 
            // pnlGrillaHoy
            // 
            pnlGrillaHoy.Controls.Add(dgvTurnosHoy);
            pnlGrillaHoy.Dock = DockStyle.Fill;
            pnlGrillaHoy.Location = new Point(3, 131);
            pnlGrillaHoy.Name = "pnlGrillaHoy";
            pnlGrillaHoy.Padding = new Padding(6);
            pnlGrillaHoy.Size = new Size(994, 443);
            pnlGrillaHoy.TabIndex = 2;
            // 
            // dgvTurnosHoy
            // 
            dgvTurnosHoy.BackgroundColor = Color.White;
            dgvTurnosHoy.BorderStyle = BorderStyle.None;
            dgvTurnosHoy.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTurnosHoy.Dock = DockStyle.Fill;
            dgvTurnosHoy.Location = new Point(6, 6);
            dgvTurnosHoy.Name = "dgvTurnosHoy";
            dgvTurnosHoy.Size = new Size(982, 431);
            dgvTurnosHoy.TabIndex = 0;
            // 
            // pnlKpisHoy
            // 
            pnlKpisHoy.BackColor = Color.FromArgb(241, 245, 249);
            pnlKpisHoy.Controls.Add(pnlKpiFactHoy);
            pnlKpisHoy.Controls.Add(pnlKpiCobHoy);
            pnlKpisHoy.Controls.Add(pnlKpiRecHoy);
            pnlKpisHoy.Controls.Add(pnlKpiTurnosHoy);
            pnlKpisHoy.Dock = DockStyle.Top;
            pnlKpisHoy.Location = new Point(3, 53);
            pnlKpisHoy.Name = "pnlKpisHoy";
            pnlKpisHoy.Padding = new Padding(6);
            pnlKpisHoy.Size = new Size(994, 78);
            pnlKpisHoy.TabIndex = 1;
            // 
            // pnlKpiFactHoy
            // 
            pnlKpiFactHoy.BackColor = Color.White;
            pnlKpiFactHoy.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiFactHoy.Controls.Add(lblSubKpiFacturacionHoy);
            pnlKpiFactHoy.Controls.Add(lblKpiFacturacionHoy);
            pnlKpiFactHoy.Controls.Add(lblTituloFactHoy);
            pnlKpiFactHoy.Location = new Point(730, 7);
            pnlKpiFactHoy.Name = "pnlKpiFactHoy";
            pnlKpiFactHoy.Size = new Size(230, 64);
            pnlKpiFactHoy.TabIndex = 3;
            // 
            // lblSubKpiFacturacionHoy
            // 
            lblSubKpiFacturacionHoy.AutoSize = true;
            lblSubKpiFacturacionHoy.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiFacturacionHoy.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiFacturacionHoy.Location = new Point(12, 45);
            lblSubKpiFacturacionHoy.Name = "lblSubKpiFacturacionHoy";
            lblSubKpiFacturacionHoy.Size = new Size(142, 12);
            lblSubKpiFacturacionHoy.TabIndex = 2;
            lblSubKpiFacturacionHoy.Text = "Promedio Caja: $ 0,00 / turno";
            // 
            // lblKpiFacturacionHoy
            // 
            lblKpiFacturacionHoy.AutoSize = true;
            lblKpiFacturacionHoy.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblKpiFacturacionHoy.ForeColor = Color.FromArgb(30, 41, 59);
            lblKpiFacturacionHoy.Location = new Point(12, 23);
            lblKpiFacturacionHoy.Name = "lblKpiFacturacionHoy";
            lblKpiFacturacionHoy.Size = new Size(55, 21);
            lblKpiFacturacionHoy.TabIndex = 1;
            lblKpiFacturacionHoy.Text = "$ 0,00";
            // 
            // lblTituloFactHoy
            // 
            lblTituloFactHoy.AutoSize = true;
            lblTituloFactHoy.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloFactHoy.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloFactHoy.Location = new Point(12, 7);
            lblTituloFactHoy.Name = "lblTituloFactHoy";
            lblTituloFactHoy.Size = new Size(142, 12);
            lblTituloFactHoy.TabIndex = 0;
            lblTituloFactHoy.Text = "FACTURACIÓN BRUTA TOTAL";
            // 
            // pnlKpiCobHoy
            // 
            pnlKpiCobHoy.BackColor = Color.White;
            pnlKpiCobHoy.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiCobHoy.Controls.Add(lblSubKpiCoberturaHoy);
            pnlKpiCobHoy.Controls.Add(lblKpiCoberturaHoy);
            pnlKpiCobHoy.Controls.Add(lblTituloCobHoy);
            pnlKpiCobHoy.Location = new Point(490, 7);
            pnlKpiCobHoy.Name = "pnlKpiCobHoy";
            pnlKpiCobHoy.Size = new Size(230, 64);
            pnlKpiCobHoy.TabIndex = 2;
            // 
            // lblSubKpiCoberturaHoy
            // 
            lblSubKpiCoberturaHoy.AutoSize = true;
            lblSubKpiCoberturaHoy.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiCoberturaHoy.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiCoberturaHoy.Location = new Point(12, 45);
            lblSubKpiCoberturaHoy.Name = "lblSubKpiCoberturaHoy";
            lblSubKpiCoberturaHoy.Size = new Size(153, 12);
            lblSubKpiCoberturaHoy.TabIndex = 2;
            lblSubKpiCoberturaHoy.Text = "70% aportado por Obras Sociales";
            // 
            // lblKpiCoberturaHoy
            // 
            lblKpiCoberturaHoy.AutoSize = true;
            lblKpiCoberturaHoy.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblKpiCoberturaHoy.ForeColor = Color.FromArgb(3, 105, 161);
            lblKpiCoberturaHoy.Location = new Point(12, 23);
            lblKpiCoberturaHoy.Name = "lblKpiCoberturaHoy";
            lblKpiCoberturaHoy.Size = new Size(55, 21);
            lblKpiCoberturaHoy.TabIndex = 1;
            lblKpiCoberturaHoy.Text = "$ 0,00";
            // 
            // lblTituloCobHoy
            // 
            lblTituloCobHoy.AutoSize = true;
            lblTituloCobHoy.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloCobHoy.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloCobHoy.Location = new Point(12, 7);
            lblTituloCobHoy.Name = "lblTituloCobHoy";
            lblTituloCobHoy.Size = new Size(147, 12);
            lblTituloCobHoy.TabIndex = 0;
            lblTituloCobHoy.Text = "COBERTURA OBRAS SOCIALES";
            // 
            // pnlKpiRecHoy
            // 
            pnlKpiRecHoy.BackColor = Color.White;
            pnlKpiRecHoy.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiRecHoy.Controls.Add(lblSubKpiRecaudacionHoy);
            pnlKpiRecHoy.Controls.Add(lblKpiRecaudacionHoy);
            pnlKpiRecHoy.Controls.Add(lblTituloRecHoy);
            pnlKpiRecHoy.Location = new Point(250, 7);
            pnlKpiRecHoy.Name = "pnlKpiRecHoy";
            pnlKpiRecHoy.Size = new Size(230, 64);
            pnlKpiRecHoy.TabIndex = 1;
            // 
            // lblSubKpiRecaudacionHoy
            // 
            lblSubKpiRecaudacionHoy.AutoSize = true;
            lblSubKpiRecaudacionHoy.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiRecaudacionHoy.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiRecaudacionHoy.Location = new Point(12, 45);
            lblSubKpiRecaudacionHoy.Name = "lblSubKpiRecaudacionHoy";
            lblSubKpiRecaudacionHoy.Size = new Size(130, 12);
            lblSubKpiRecaudacionHoy.TabIndex = 2;
            lblSubKpiRecaudacionHoy.Text = "Cobrado en ventanilla / caja";
            // 
            // lblKpiRecaudacionHoy
            // 
            lblKpiRecaudacionHoy.AutoSize = true;
            lblKpiRecaudacionHoy.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblKpiRecaudacionHoy.ForeColor = Color.FromArgb(22, 101, 52);
            lblKpiRecaudacionHoy.Location = new Point(12, 23);
            lblKpiRecaudacionHoy.Name = "lblKpiRecaudacionHoy";
            lblKpiRecaudacionHoy.Size = new Size(55, 21);
            lblKpiRecaudacionHoy.TabIndex = 1;
            lblKpiRecaudacionHoy.Text = "$ 0,00";
            // 
            // lblTituloRecHoy
            // 
            lblTituloRecHoy.AutoSize = true;
            lblTituloRecHoy.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloRecHoy.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloRecHoy.Location = new Point(12, 7);
            lblTituloRecHoy.Name = "lblTituloRecHoy";
            lblTituloRecHoy.Size = new Size(147, 12);
            lblTituloRecHoy.TabIndex = 0;
            lblTituloRecHoy.Text = "RECAUDACIÓN CAJA (EFECTIVO)";
            // 
            // pnlKpiTurnosHoy
            // 
            pnlKpiTurnosHoy.BackColor = Color.White;
            pnlKpiTurnosHoy.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiTurnosHoy.Controls.Add(lblSubKpiTurnosHoy);
            pnlKpiTurnosHoy.Controls.Add(lblKpiTurnosHoy);
            pnlKpiTurnosHoy.Controls.Add(lblTituloTurnosHoy);
            pnlKpiTurnosHoy.Location = new Point(10, 7);
            pnlKpiTurnosHoy.Name = "pnlKpiTurnosHoy";
            pnlKpiTurnosHoy.Size = new Size(230, 64);
            pnlKpiTurnosHoy.TabIndex = 0;
            // 
            // lblSubKpiTurnosHoy
            // 
            lblSubKpiTurnosHoy.AutoSize = true;
            lblSubKpiTurnosHoy.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiTurnosHoy.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiTurnosHoy.Location = new Point(12, 45);
            lblSubKpiTurnosHoy.Name = "lblSubKpiTurnosHoy";
            lblSubKpiTurnosHoy.Size = new Size(111, 12);
            lblSubKpiTurnosHoy.TabIndex = 2;
            lblSubKpiTurnosHoy.Text = "Esp: 0 | Emergencia: 0";
            // 
            // lblKpiTurnosHoy
            // 
            lblKpiTurnosHoy.AutoSize = true;
            lblKpiTurnosHoy.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
            lblKpiTurnosHoy.ForeColor = Color.FromArgb(15, 118, 110);
            lblKpiTurnosHoy.Location = new Point(12, 21);
            lblKpiTurnosHoy.Name = "lblKpiTurnosHoy";
            lblKpiTurnosHoy.Size = new Size(22, 25);
            lblKpiTurnosHoy.TabIndex = 1;
            lblKpiTurnosHoy.Text = "0";
            // 
            // lblTituloTurnosHoy
            // 
            lblTituloTurnosHoy.AutoSize = true;
            lblTituloTurnosHoy.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloTurnosHoy.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloTurnosHoy.Location = new Point(12, 7);
            lblTituloTurnosHoy.Name = "lblTituloTurnosHoy";
            lblTituloTurnosHoy.Size = new Size(130, 12);
            lblTituloTurnosHoy.TabIndex = 0;
            lblTituloTurnosHoy.Text = "TOTAL TURNOS EMITIDOS";
            // 
            // pnlFiltrosHoy
            // 
            pnlFiltrosHoy.BackColor = Color.White;
            pnlFiltrosHoy.Controls.Add(btnExportarPdfHoy);
            pnlFiltrosHoy.Controls.Add(btnActualizarHoy);
            pnlFiltrosHoy.Controls.Add(lblHoyFechaValor);
            pnlFiltrosHoy.Controls.Add(lblHoyFecha);
            pnlFiltrosHoy.Dock = DockStyle.Top;
            pnlFiltrosHoy.Location = new Point(3, 3);
            pnlFiltrosHoy.Name = "pnlFiltrosHoy";
            pnlFiltrosHoy.Size = new Size(994, 50);
            pnlFiltrosHoy.TabIndex = 0;
            // 
            // btnExportarPdfHoy
            // 
            btnExportarPdfHoy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportarPdfHoy.BackColor = Color.FromArgb(15, 118, 110);
            btnExportarPdfHoy.Cursor = Cursors.Hand;
            btnExportarPdfHoy.FlatStyle = FlatStyle.Flat;
            btnExportarPdfHoy.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportarPdfHoy.ForeColor = Color.White;
            btnExportarPdfHoy.Location = new Point(744, 10);
            btnExportarPdfHoy.Name = "btnExportarPdfHoy";
            btnExportarPdfHoy.Size = new Size(240, 30);
            btnExportarPdfHoy.TabIndex = 3;
            btnExportarPdfHoy.Text = "📄 Exportar Turnos Emitidos a PDF";
            btnExportarPdfHoy.UseVisualStyleBackColor = false;
            // 
            // btnActualizarHoy
            // 
            btnActualizarHoy.BackColor = Color.SteelBlue;
            btnActualizarHoy.Cursor = Cursors.Hand;
            btnActualizarHoy.FlatStyle = FlatStyle.Flat;
            btnActualizarHoy.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnActualizarHoy.ForeColor = Color.White;
            btnActualizarHoy.Location = new Point(310, 10);
            btnActualizarHoy.Name = "btnActualizarHoy";
            btnActualizarHoy.Size = new Size(160, 30);
            btnActualizarHoy.TabIndex = 2;
            btnActualizarHoy.Text = "🔄 Actualizar Jornada";
            btnActualizarHoy.UseVisualStyleBackColor = false;
            // 
            // lblHoyFechaValor
            // 
            lblHoyFechaValor.AutoSize = true;
            lblHoyFechaValor.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblHoyFechaValor.ForeColor = Color.SteelBlue;
            lblHoyFechaValor.Location = new Point(190, 15);
            lblHoyFechaValor.Name = "lblHoyFechaValor";
            lblHoyFechaValor.Size = new Size(89, 19);
            lblHoyFechaValor.TabIndex = 1;
            lblHoyFechaValor.Text = "--/--/----";
            // 
            // lblHoyFecha
            // 
            lblHoyFecha.AutoSize = true;
            lblHoyFecha.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblHoyFecha.Location = new Point(12, 16);
            lblHoyFecha.Name = "lblHoyFecha";
            lblHoyFecha.Size = new Size(172, 17);
            lblHoyFecha.TabIndex = 0;
            lblHoyFecha.Text = "Fecha de emisión:";
            // 
            // tabConsultaHistorica
            // 
            tabConsultaHistorica.BackColor = Color.FromArgb(248, 250, 252);
            tabConsultaHistorica.Controls.Add(pnlGrillaFiltro);
            tabConsultaHistorica.Controls.Add(pnlKpisFiltro);
            tabConsultaHistorica.Controls.Add(pnlFiltrosHistorico);
            tabConsultaHistorica.Location = new Point(4, 34);
            tabConsultaHistorica.Name = "tabConsultaHistorica";
            tabConsultaHistorica.Padding = new Padding(3);
            tabConsultaHistorica.Size = new Size(1000, 577);
            tabConsultaHistorica.TabIndex = 1;
            tabConsultaHistorica.Text = "🔍  Búsqueda por Fecha y Especialidad";
            // 
            // pnlGrillaFiltro
            // 
            pnlGrillaFiltro.Controls.Add(dgvTurnosFiltro);
            pnlGrillaFiltro.Dock = DockStyle.Fill;
            pnlGrillaFiltro.Location = new Point(3, 131);
            pnlGrillaFiltro.Name = "pnlGrillaFiltro";
            pnlGrillaFiltro.Padding = new Padding(6);
            pnlGrillaFiltro.Size = new Size(994, 443);
            pnlGrillaFiltro.TabIndex = 2;
            // 
            // dgvTurnosFiltro
            // 
            dgvTurnosFiltro.BackgroundColor = Color.White;
            dgvTurnosFiltro.BorderStyle = BorderStyle.None;
            dgvTurnosFiltro.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTurnosFiltro.Dock = DockStyle.Fill;
            dgvTurnosFiltro.Location = new Point(6, 6);
            dgvTurnosFiltro.Name = "dgvTurnosFiltro";
            dgvTurnosFiltro.Size = new Size(982, 431);
            dgvTurnosFiltro.TabIndex = 0;
            // 
            // pnlKpisFiltro
            // 
            pnlKpisFiltro.BackColor = Color.FromArgb(241, 245, 249);
            pnlKpisFiltro.Controls.Add(pnlKpiFactFiltro);
            pnlKpisFiltro.Controls.Add(pnlKpiCobFiltro);
            pnlKpisFiltro.Controls.Add(pnlKpiRecFiltro);
            pnlKpisFiltro.Controls.Add(pnlKpiTurnosFiltro);
            pnlKpisFiltro.Dock = DockStyle.Top;
            pnlKpisFiltro.Location = new Point(3, 53);
            pnlKpisFiltro.Name = "pnlKpisFiltro";
            pnlKpisFiltro.Padding = new Padding(6);
            pnlKpisFiltro.Size = new Size(994, 78);
            pnlKpisFiltro.TabIndex = 1;
            // 
            // pnlKpiFactFiltro
            // 
            pnlKpiFactFiltro.BackColor = Color.White;
            pnlKpiFactFiltro.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiFactFiltro.Controls.Add(lblSubKpiFacturacionFiltro);
            pnlKpiFactFiltro.Controls.Add(lblKpiFacturacionFiltro);
            pnlKpiFactFiltro.Controls.Add(lblTituloFactFiltro);
            pnlKpiFactFiltro.Location = new Point(730, 7);
            pnlKpiFactFiltro.Name = "pnlKpiFactFiltro";
            pnlKpiFactFiltro.Size = new Size(230, 64);
            pnlKpiFactFiltro.TabIndex = 3;
            // 
            // lblSubKpiFacturacionFiltro
            // 
            lblSubKpiFacturacionFiltro.AutoSize = true;
            lblSubKpiFacturacionFiltro.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiFacturacionFiltro.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiFacturacionFiltro.Location = new Point(12, 45);
            lblSubKpiFacturacionFiltro.Name = "lblSubKpiFacturacionFiltro";
            lblSubKpiFacturacionFiltro.Size = new Size(142, 12);
            lblSubKpiFacturacionFiltro.TabIndex = 2;
            lblSubKpiFacturacionFiltro.Text = "Promedio Caja: $ 0,00 / turno";
            // 
            // lblKpiFacturacionFiltro
            // 
            lblKpiFacturacionFiltro.AutoSize = true;
            lblKpiFacturacionFiltro.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblKpiFacturacionFiltro.ForeColor = Color.FromArgb(30, 41, 59);
            lblKpiFacturacionFiltro.Location = new Point(12, 23);
            lblKpiFacturacionFiltro.Name = "lblKpiFacturacionFiltro";
            lblKpiFacturacionFiltro.Size = new Size(55, 21);
            lblKpiFacturacionFiltro.TabIndex = 1;
            lblKpiFacturacionFiltro.Text = "$ 0,00";
            // 
            // lblTituloFactFiltro
            // 
            lblTituloFactFiltro.AutoSize = true;
            lblTituloFactFiltro.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloFactFiltro.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloFactFiltro.Location = new Point(12, 7);
            lblTituloFactFiltro.Name = "lblTituloFactFiltro";
            lblTituloFactFiltro.Size = new Size(142, 12);
            lblTituloFactFiltro.TabIndex = 0;
            lblTituloFactFiltro.Text = "FACTURACIÓN BRUTA TOTAL";
            // 
            // pnlKpiCobFiltro
            // 
            pnlKpiCobFiltro.BackColor = Color.White;
            pnlKpiCobFiltro.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiCobFiltro.Controls.Add(lblSubKpiCoberturaFiltro);
            pnlKpiCobFiltro.Controls.Add(lblKpiCoberturaFiltro);
            pnlKpiCobFiltro.Controls.Add(lblTituloCobFiltro);
            pnlKpiCobFiltro.Location = new Point(490, 7);
            pnlKpiCobFiltro.Name = "pnlKpiCobFiltro";
            pnlKpiCobFiltro.Size = new Size(230, 64);
            pnlKpiCobFiltro.TabIndex = 2;
            // 
            // lblSubKpiCoberturaFiltro
            // 
            lblSubKpiCoberturaFiltro.AutoSize = true;
            lblSubKpiCoberturaFiltro.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiCoberturaFiltro.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiCoberturaFiltro.Location = new Point(12, 45);
            lblSubKpiCoberturaFiltro.Name = "lblSubKpiCoberturaFiltro";
            lblSubKpiCoberturaFiltro.Size = new Size(153, 12);
            lblSubKpiCoberturaFiltro.TabIndex = 2;
            lblSubKpiCoberturaFiltro.Text = "70% aportado por Obras Sociales";
            // 
            // lblKpiCoberturaFiltro
            // 
            lblKpiCoberturaFiltro.AutoSize = true;
            lblKpiCoberturaFiltro.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblKpiCoberturaFiltro.ForeColor = Color.FromArgb(3, 105, 161);
            lblKpiCoberturaFiltro.Location = new Point(12, 23);
            lblKpiCoberturaFiltro.Name = "lblKpiCoberturaFiltro";
            lblKpiCoberturaFiltro.Size = new Size(55, 21);
            lblKpiCoberturaFiltro.TabIndex = 1;
            lblKpiCoberturaFiltro.Text = "$ 0,00";
            // 
            // lblTituloCobFiltro
            // 
            lblTituloCobFiltro.AutoSize = true;
            lblTituloCobFiltro.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloCobFiltro.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloCobFiltro.Location = new Point(12, 7);
            lblTituloCobFiltro.Name = "lblTituloCobFiltro";
            lblTituloCobFiltro.Size = new Size(147, 12);
            lblTituloCobFiltro.TabIndex = 0;
            lblTituloCobFiltro.Text = "COBERTURA OBRAS SOCIALES";
            // 
            // pnlKpiRecFiltro
            // 
            pnlKpiRecFiltro.BackColor = Color.White;
            pnlKpiRecFiltro.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiRecFiltro.Controls.Add(lblSubKpiRecaudacionFiltro);
            pnlKpiRecFiltro.Controls.Add(lblKpiRecaudacionFiltro);
            pnlKpiRecFiltro.Controls.Add(lblTituloRecFiltro);
            pnlKpiRecFiltro.Location = new Point(250, 7);
            pnlKpiRecFiltro.Name = "pnlKpiRecFiltro";
            pnlKpiRecFiltro.Size = new Size(230, 64);
            pnlKpiRecFiltro.TabIndex = 1;
            // 
            // lblSubKpiRecaudacionFiltro
            // 
            lblSubKpiRecaudacionFiltro.AutoSize = true;
            lblSubKpiRecaudacionFiltro.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiRecaudacionFiltro.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiRecaudacionFiltro.Location = new Point(12, 45);
            lblSubKpiRecaudacionFiltro.Name = "lblSubKpiRecaudacionFiltro";
            lblSubKpiRecaudacionFiltro.Size = new Size(130, 12);
            lblSubKpiRecaudacionFiltro.TabIndex = 2;
            lblSubKpiRecaudacionFiltro.Text = "Cobrado en ventanilla / caja";
            // 
            // lblKpiRecaudacionFiltro
            // 
            lblKpiRecaudacionFiltro.AutoSize = true;
            lblKpiRecaudacionFiltro.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblKpiRecaudacionFiltro.ForeColor = Color.FromArgb(22, 101, 52);
            lblKpiRecaudacionFiltro.Location = new Point(12, 23);
            lblKpiRecaudacionFiltro.Name = "lblKpiRecaudacionFiltro";
            lblKpiRecaudacionFiltro.Size = new Size(55, 21);
            lblKpiRecaudacionFiltro.TabIndex = 1;
            lblKpiRecaudacionFiltro.Text = "$ 0,00";
            // 
            // lblTituloRecFiltro
            // 
            lblTituloRecFiltro.AutoSize = true;
            lblTituloRecFiltro.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloRecFiltro.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloRecFiltro.Location = new Point(12, 7);
            lblTituloRecFiltro.Name = "lblTituloRecFiltro";
            lblTituloRecFiltro.Size = new Size(147, 12);
            lblTituloRecFiltro.TabIndex = 0;
            lblTituloRecFiltro.Text = "RECAUDACIÓN CAJA (EFECTIVO)";
            // 
            // pnlKpiTurnosFiltro
            // 
            pnlKpiTurnosFiltro.BackColor = Color.White;
            pnlKpiTurnosFiltro.BorderStyle = BorderStyle.FixedSingle;
            pnlKpiTurnosFiltro.Controls.Add(lblSubKpiTurnosFiltro);
            pnlKpiTurnosFiltro.Controls.Add(lblKpiTurnosFiltro);
            pnlKpiTurnosFiltro.Controls.Add(lblTituloTurnosFiltro);
            pnlKpiTurnosFiltro.Location = new Point(10, 7);
            pnlKpiTurnosFiltro.Name = "pnlKpiTurnosFiltro";
            pnlKpiTurnosFiltro.Size = new Size(230, 64);
            pnlKpiTurnosFiltro.TabIndex = 0;
            // 
            // lblSubKpiTurnosFiltro
            // 
            lblSubKpiTurnosFiltro.AutoSize = true;
            lblSubKpiTurnosFiltro.Font = new Font("Segoe UI", 7.5F);
            lblSubKpiTurnosFiltro.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubKpiTurnosFiltro.Location = new Point(12, 45);
            lblSubKpiTurnosFiltro.Name = "lblSubKpiTurnosFiltro";
            lblSubKpiTurnosFiltro.Size = new Size(111, 12);
            lblSubKpiTurnosFiltro.TabIndex = 2;
            lblSubKpiTurnosFiltro.Text = "Esp: 0 | Emergencia: 0";
            // 
            // lblKpiTurnosFiltro
            // 
            lblKpiTurnosFiltro.AutoSize = true;
            lblKpiTurnosFiltro.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
            lblKpiTurnosFiltro.ForeColor = Color.FromArgb(15, 118, 110);
            lblKpiTurnosFiltro.Location = new Point(12, 21);
            lblKpiTurnosFiltro.Name = "lblKpiTurnosFiltro";
            lblKpiTurnosFiltro.Size = new Size(22, 25);
            lblKpiTurnosFiltro.TabIndex = 1;
            lblKpiTurnosFiltro.Text = "0";
            // 
            // lblTituloTurnosFiltro
            // 
            lblTituloTurnosFiltro.AutoSize = true;
            lblTituloTurnosFiltro.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblTituloTurnosFiltro.ForeColor = Color.FromArgb(100, 116, 139);
            lblTituloTurnosFiltro.Location = new Point(12, 7);
            lblTituloTurnosFiltro.Name = "lblTituloTurnosFiltro";
            lblTituloTurnosFiltro.Size = new Size(130, 12);
            lblTituloTurnosFiltro.TabIndex = 0;
            lblTituloTurnosFiltro.Text = "TOTAL TURNOS EMITIDOS";
            // 
            // pnlFiltrosHistorico
            // 
            pnlFiltrosHistorico.BackColor = Color.White;
            pnlFiltrosHistorico.Controls.Add(btnExportarPdfFiltro);
            pnlFiltrosHistorico.Controls.Add(btnConsultarFiltro);
            pnlFiltrosHistorico.Controls.Add(cmbEspecialidadFiltro);
            pnlFiltrosHistorico.Controls.Add(lblEspecialidadFiltro);
            pnlFiltrosHistorico.Controls.Add(dtpFechaFiltro);
            pnlFiltrosHistorico.Controls.Add(lblFechaFiltro);
            pnlFiltrosHistorico.Dock = DockStyle.Top;
            pnlFiltrosHistorico.Location = new Point(3, 3);
            pnlFiltrosHistorico.Name = "pnlFiltrosHistorico";
            pnlFiltrosHistorico.Size = new Size(994, 50);
            pnlFiltrosHistorico.TabIndex = 0;
            // 
            // btnExportarPdfFiltro
            // 
            btnExportarPdfFiltro.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportarPdfFiltro.BackColor = Color.FromArgb(15, 118, 110);
            btnExportarPdfFiltro.Cursor = Cursors.Hand;
            btnExportarPdfFiltro.FlatStyle = FlatStyle.Flat;
            btnExportarPdfFiltro.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportarPdfFiltro.ForeColor = Color.White;
            btnExportarPdfFiltro.Location = new Point(764, 10);
            btnExportarPdfFiltro.Name = "btnExportarPdfFiltro";
            btnExportarPdfFiltro.Size = new Size(220, 30);
            btnExportarPdfFiltro.TabIndex = 5;
            btnExportarPdfFiltro.Text = "📄 Exportar Consulta a PDF";
            btnExportarPdfFiltro.UseVisualStyleBackColor = false;
            // 
            // btnConsultarFiltro
            // 
            btnConsultarFiltro.BackColor = Color.SteelBlue;
            btnConsultarFiltro.Cursor = Cursors.Hand;
            btnConsultarFiltro.FlatStyle = FlatStyle.Flat;
            btnConsultarFiltro.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnConsultarFiltro.ForeColor = Color.White;
            btnConsultarFiltro.Location = new Point(560, 10);
            btnConsultarFiltro.Name = "btnConsultarFiltro";
            btnConsultarFiltro.Size = new Size(130, 30);
            btnConsultarFiltro.TabIndex = 4;
            btnConsultarFiltro.Text = "🔍 Consultar";
            btnConsultarFiltro.UseVisualStyleBackColor = false;
            // 
            // cmbEspecialidadFiltro
            // 
            cmbEspecialidadFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEspecialidadFiltro.Font = new Font("Segoe UI", 9F);
            cmbEspecialidadFiltro.FormattingEnabled = true;
            cmbEspecialidadFiltro.Location = new Point(310, 14);
            cmbEspecialidadFiltro.Name = "cmbEspecialidadFiltro";
            cmbEspecialidadFiltro.Size = new Size(230, 23);
            cmbEspecialidadFiltro.TabIndex = 3;
            // 
            // lblEspecialidadFiltro
            // 
            lblEspecialidadFiltro.AutoSize = true;
            lblEspecialidadFiltro.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblEspecialidadFiltro.Location = new Point(220, 17);
            lblEspecialidadFiltro.Name = "lblEspecialidadFiltro";
            lblEspecialidadFiltro.Size = new Size(88, 17);
            lblEspecialidadFiltro.TabIndex = 2;
            lblEspecialidadFiltro.Text = "Especialidad:";
            // 
            // dtpFechaFiltro
            // 
            dtpFechaFiltro.Font = new Font("Segoe UI", 9F);
            dtpFechaFiltro.Format = DateTimePickerFormat.Short;
            dtpFechaFiltro.Location = new Point(70, 14);
            dtpFechaFiltro.Name = "dtpFechaFiltro";
            dtpFechaFiltro.Size = new Size(125, 23);
            dtpFechaFiltro.TabIndex = 1;
            // 
            // lblFechaFiltro
            // 
            lblFechaFiltro.AutoSize = true;
            lblFechaFiltro.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblFechaFiltro.Location = new Point(15, 17);
            lblFechaFiltro.Name = "lblFechaFiltro";
            lblFechaFiltro.Size = new Size(47, 17);
            lblFechaFiltro.TabIndex = 0;
            lblFechaFiltro.Text = "Fecha:";
            // 
            // FrmCierreCaja
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(1008, 680);
            Controls.Add(tabReportes);
            Controls.Add(pnlHeader);
            Name = "FrmCierreCaja";
            Text = "Reportes y Control de Caja — Recepción";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            tabReportes.ResumeLayout(false);
            tabJornadaHoy.ResumeLayout(false);
            pnlGrillaHoy.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTurnosHoy).EndInit();
            pnlKpisHoy.ResumeLayout(false);
            pnlKpiFactHoy.ResumeLayout(false);
            pnlKpiFactHoy.PerformLayout();
            pnlKpiCobHoy.ResumeLayout(false);
            pnlKpiCobHoy.PerformLayout();
            pnlKpiRecHoy.ResumeLayout(false);
            pnlKpiRecHoy.PerformLayout();
            pnlKpiTurnosHoy.ResumeLayout(false);
            pnlKpiTurnosHoy.PerformLayout();
            pnlFiltrosHoy.ResumeLayout(false);
            pnlFiltrosHoy.PerformLayout();
            tabConsultaHistorica.ResumeLayout(false);
            pnlGrillaFiltro.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTurnosFiltro).EndInit();
            pnlKpisFiltro.ResumeLayout(false);
            pnlKpiFactFiltro.ResumeLayout(false);
            pnlKpiFactFiltro.PerformLayout();
            pnlKpiCobFiltro.ResumeLayout(false);
            pnlKpiCobFiltro.PerformLayout();
            pnlKpiRecFiltro.ResumeLayout(false);
            pnlKpiRecFiltro.PerformLayout();
            pnlKpiTurnosFiltro.ResumeLayout(false);
            pnlKpiTurnosFiltro.PerformLayout();
            pnlFiltrosHistorico.ResumeLayout(false);
            pnlFiltrosHistorico.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private TabControl tabReportes;
        private TabPage tabJornadaHoy;
        private Panel pnlFiltrosHoy;
        private Label lblHoyFecha;
        private Label lblHoyFechaValor;
        private Button btnActualizarHoy;
        private Button btnExportarPdfHoy;
        private Panel pnlKpisHoy;
        private Panel pnlKpiTurnosHoy;
        private Label lblTituloTurnosHoy;
        private Label lblKpiTurnosHoy;
        private Label lblSubKpiTurnosHoy;
        private Panel pnlKpiRecHoy;
        private Label lblTituloRecHoy;
        private Label lblKpiRecaudacionHoy;
        private Label lblSubKpiRecaudacionHoy;
        private Panel pnlKpiCobHoy;
        private Label lblTituloCobHoy;
        private Label lblKpiCoberturaHoy;
        private Label lblSubKpiCoberturaHoy;
        private Panel pnlKpiFactHoy;
        private Label lblTituloFactHoy;
        private Label lblKpiFacturacionHoy;
        private Label lblSubKpiFacturacionHoy;
        private Panel pnlGrillaHoy;
        private DataGridView dgvTurnosHoy;
        private TabPage tabConsultaHistorica;
        private Panel pnlFiltrosHistorico;
        private Label lblFechaFiltro;
        private DateTimePicker dtpFechaFiltro;
        private Label lblEspecialidadFiltro;
        private ComboBox cmbEspecialidadFiltro;
        private Button btnConsultarFiltro;
        private Button btnExportarPdfFiltro;
        private Panel pnlKpisFiltro;
        private Panel pnlKpiTurnosFiltro;
        private Label lblTituloTurnosFiltro;
        private Label lblKpiTurnosFiltro;
        private Label lblSubKpiTurnosFiltro;
        private Panel pnlKpiRecFiltro;
        private Label lblTituloRecFiltro;
        private Label lblKpiRecaudacionFiltro;
        private Label lblSubKpiRecaudacionFiltro;
        private Panel pnlKpiCobFiltro;
        private Label lblTituloCobFiltro;
        private Label lblKpiCoberturaFiltro;
        private Label lblSubKpiCoberturaFiltro;
        private Panel pnlKpiFactFiltro;
        private Label lblTituloFactFiltro;
        private Label lblKpiFacturacionFiltro;
        private Label lblSubKpiFacturacionFiltro;
        private Panel pnlGrillaFiltro;
        private DataGridView dgvTurnosFiltro;
    }
}
