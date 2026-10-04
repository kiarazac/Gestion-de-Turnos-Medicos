namespace Gestion_de_Turnos_Medicos
{
    partial class FrmReporteGuardiaAdmin
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblSubtituloHeader = new System.Windows.Forms.Label();
            this.lblTituloHeader = new System.Windows.Forms.Label();
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.lblPeriodo = new System.Windows.Forms.Label();
            this.cmbPeriodo = new System.Windows.Forms.ComboBox();
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblPrioridad = new System.Windows.Forms.Label();
            this.cmbPrioridad = new System.Windows.Forms.ComboBox();
            this.btnFiltrar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.pnlCards = new System.Windows.Forms.Panel();
            this.pnlCardTotal = new System.Windows.Forms.Panel();
            this.lblTotalDesc = new System.Windows.Forms.Label();
            this.lblTotalValor = new System.Windows.Forms.Label();
            this.lblTotalTitulo = new System.Windows.Forms.Label();
            this.pnlCardAlta = new System.Windows.Forms.Panel();
            this.lblAltaDesc = new System.Windows.Forms.Label();
            this.lblAltaValor = new System.Windows.Forms.Label();
            this.lblAltaTitulo = new System.Windows.Forms.Label();
            this.pnlCardMedia = new System.Windows.Forms.Panel();
            this.lblMediaDesc = new System.Windows.Forms.Label();
            this.lblMediaValor = new System.Windows.Forms.Label();
            this.lblMediaTitulo = new System.Windows.Forms.Label();
            this.pnlCardBaja = new System.Windows.Forms.Panel();
            this.lblBajaDesc = new System.Windows.Forms.Label();
            this.lblBajaValor = new System.Windows.Forms.Label();
            this.lblBajaTitulo = new System.Windows.Forms.Label();
            this.pnlCardResolucion = new System.Windows.Forms.Panel();
            this.lblResolucionDesc = new System.Windows.Forms.Label();
            this.lblResolucionValor = new System.Windows.Forms.Label();
            this.lblResolucionTitulo = new System.Windows.Forms.Label();
            this.tabControlReporte = new System.Windows.Forms.TabControl();
            this.tabDetalle = new System.Windows.Forms.TabPage();
            this.dgvDetalleGuardia = new System.Windows.Forms.DataGridView();
            this.tabRanking = new System.Windows.Forms.TabPage();
            this.dgvRankingSintomas = new System.Windows.Forms.DataGridView();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lblTotalRegistros = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlFiltros.SuspendLayout();
            this.pnlCards.SuspendLayout();
            this.pnlCardTotal.SuspendLayout();
            this.pnlCardAlta.SuspendLayout();
            this.pnlCardMedia.SuspendLayout();
            this.pnlCardBaja.SuspendLayout();
            this.pnlCardResolucion.SuspendLayout();
            this.tabControlReporte.SuspendLayout();
            this.tabDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleGuardia)).BeginInit();
            this.tabRanking.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRankingSintomas)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.pnlHeader.Controls.Add(this.lblSubtituloHeader);
            this.pnlHeader.Controls.Add(this.lblTituloHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1080, 68);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblSubtituloHeader
            // 
            this.lblSubtituloHeader.AutoSize = true;
            this.lblSubtituloHeader.Font = new System.Drawing.Font("Segoe UI", 9.25F);
            this.lblSubtituloHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblSubtituloHeader.Location = new System.Drawing.Point(18, 40);
            this.lblSubtituloHeader.Name = "lblSubtituloHeader";
            this.lblSubtituloHeader.Size = new System.Drawing.Size(534, 17);
            this.lblSubtituloHeader.TabIndex = 1;
            this.lblSubtituloHeader.Text = "Monitoreo del flujo de guardia médica, severidad de pacientes ingresados y síntomas predominantes";
            // 
            // lblTituloHeader
            // 
            this.lblTituloHeader.AutoSize = true;
            this.lblTituloHeader.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloHeader.ForeColor = System.Drawing.Color.White;
            this.lblTituloHeader.Location = new System.Drawing.Point(16, 10);
            this.lblTituloHeader.Name = "lblTituloHeader";
            this.lblTituloHeader.Size = new System.Drawing.Size(650, 30);
            this.lblTituloHeader.TabIndex = 0;
            this.lblTituloHeader.Text = "Reporte Operativo de Guardia: Triage y Distribución de Urgencias";
            // 
            // pnlFiltros
            // 
            this.pnlFiltros.BackColor = System.Drawing.Color.White;
            this.pnlFiltros.Controls.Add(this.txtBuscar);
            this.pnlFiltros.Controls.Add(this.btnExportar);
            this.pnlFiltros.Controls.Add(this.btnFiltrar);
            this.pnlFiltros.Controls.Add(this.cmbPrioridad);
            this.pnlFiltros.Controls.Add(this.lblPrioridad);
            this.pnlFiltros.Controls.Add(this.dtpHasta);
            this.pnlFiltros.Controls.Add(this.lblHasta);
            this.pnlFiltros.Controls.Add(this.dtpDesde);
            this.pnlFiltros.Controls.Add(this.lblDesde);
            this.pnlFiltros.Controls.Add(this.cmbPeriodo);
            this.pnlFiltros.Controls.Add(this.lblPeriodo);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Location = new System.Drawing.Point(0, 68);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.pnlFiltros.Size = new System.Drawing.Size(1080, 80);
            this.pnlFiltros.TabIndex = 1;
            // 
            // lblPeriodo
            // 
            this.lblPeriodo.AutoSize = true;
            this.lblPeriodo.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPeriodo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblPeriodo.Location = new System.Drawing.Point(14, 11);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(51, 13);
            this.lblPeriodo.TabIndex = 0;
            this.lblPeriodo.Text = "PERÍODO";
            // 
            // cmbPeriodo
            // 
            this.cmbPeriodo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPeriodo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbPeriodo.FormattingEnabled = true;
            this.cmbPeriodo.Items.AddRange(new object[] {
            "Hoy",
            "Últimos 7 días",
            "Este Mes",
            "Todos los registros",
            "Rango personalizado"});
            this.cmbPeriodo.Location = new System.Drawing.Point(14, 27);
            this.cmbPeriodo.Name = "cmbPeriodo";
            this.cmbPeriodo.Size = new System.Drawing.Size(140, 23);
            this.cmbPeriodo.TabIndex = 1;
            // 
            // lblDesde
            // 
            this.lblDesde.AutoSize = true;
            this.lblDesde.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDesde.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblDesde.Location = new System.Drawing.Point(165, 11);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(42, 13);
            this.lblDesde.TabIndex = 2;
            this.lblDesde.Text = "DESDE";
            // 
            // dtpDesde
            // 
            this.dtpDesde.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(165, 27);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(105, 23);
            this.dtpDesde.TabIndex = 3;
            // 
            // lblHasta
            // 
            this.lblHasta.AutoSize = true;
            this.lblHasta.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblHasta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblHasta.Location = new System.Drawing.Point(280, 11);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(43, 13);
            this.lblHasta.TabIndex = 4;
            this.lblHasta.Text = "HASTA";
            // 
            // dtpHasta
            // 
            this.dtpHasta.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(280, 27);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(105, 23);
            this.dtpHasta.TabIndex = 5;
            // 
            // lblPrioridad
            // 
            this.lblPrioridad.AutoSize = true;
            this.lblPrioridad.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPrioridad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblPrioridad.Location = new System.Drawing.Point(395, 11);
            this.lblPrioridad.Name = "lblPrioridad";
            this.lblPrioridad.Size = new System.Drawing.Size(68, 13);
            this.lblPrioridad.TabIndex = 6;
            this.lblPrioridad.Text = "PRIORIDAD";
            // 
            // cmbPrioridad
            // 
            this.cmbPrioridad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrioridad.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbPrioridad.FormattingEnabled = true;
            this.cmbPrioridad.Items.AddRange(new object[] {
            "Todas las prioridades",
            "Alta (Rojo)",
            "Media (Amarillo)",
            "Baja (Verde)"});
            this.cmbPrioridad.Location = new System.Drawing.Point(395, 27);
            this.cmbPrioridad.Name = "cmbPrioridad";
            this.cmbPrioridad.Size = new System.Drawing.Size(155, 23);
            this.cmbPrioridad.TabIndex = 7;
            // 
            // btnFiltrar
            // 
            this.btnFiltrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnFiltrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFiltrar.FlatAppearance.BorderSize = 0;
            this.btnFiltrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFiltrar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnFiltrar.ForeColor = System.Drawing.Color.White;
            this.btnFiltrar.Location = new System.Drawing.Point(560, 24);
            this.btnFiltrar.Name = "btnFiltrar";
            this.btnFiltrar.Size = new System.Drawing.Size(95, 28);
            this.btnFiltrar.TabIndex = 8;
            this.btnFiltrar.Text = "🔄 Actualizar";
            this.btnFiltrar.UseVisualStyleBackColor = false;
            // 
            // btnExportar
            // 
            this.btnExportar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnExportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportar.FlatAppearance.BorderSize = 0;
            this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnExportar.ForeColor = System.Drawing.Color.White;
            this.btnExportar.Location = new System.Drawing.Point(665, 24);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(115, 28);
            this.btnExportar.TabIndex = 9;
            this.btnExportar.Text = "📥 Exportar...";
            this.btnExportar.UseVisualStyleBackColor = false;
            // 
            // txtBuscar
            // 
            this.txtBuscar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBuscar.ForeColor = System.Drawing.Color.Gray;
            this.txtBuscar.Location = new System.Drawing.Point(790, 27);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(275, 23);
            this.txtBuscar.TabIndex = 10;
            this.txtBuscar.Text = "🔍 Buscar por paciente, DNI, turno o síntoma...";
            // 
            // pnlCards
            // 
            this.pnlCards.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlCards.Controls.Add(this.pnlCardResolucion);
            this.pnlCards.Controls.Add(this.pnlCardBaja);
            this.pnlCards.Controls.Add(this.pnlCardMedia);
            this.pnlCards.Controls.Add(this.pnlCardAlta);
            this.pnlCards.Controls.Add(this.pnlCardTotal);
            this.pnlCards.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCards.Location = new System.Drawing.Point(0, 148);
            this.pnlCards.Name = "pnlCards";
            this.pnlCards.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlCards.Size = new System.Drawing.Size(1080, 90);
            this.pnlCards.TabIndex = 2;
            // 
            // pnlCardTotal
            // 
            this.pnlCardTotal.BackColor = System.Drawing.Color.White;
            this.pnlCardTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCardTotal.Controls.Add(this.lblTotalDesc);
            this.pnlCardTotal.Controls.Add(this.lblTotalValor);
            this.pnlCardTotal.Controls.Add(this.lblTotalTitulo);
            this.pnlCardTotal.Location = new System.Drawing.Point(14, 8);
            this.pnlCardTotal.Name = "pnlCardTotal";
            this.pnlCardTotal.Padding = new System.Windows.Forms.Padding(8);
            this.pnlCardTotal.Size = new System.Drawing.Size(195, 72);
            this.pnlCardTotal.TabIndex = 0;
            // 
            // lblTotalDesc
            // 
            this.lblTotalDesc.AutoSize = true;
            this.lblTotalDesc.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblTotalDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblTotalDesc.Location = new System.Drawing.Point(8, 50);
            this.lblTotalDesc.Name = "lblTotalDesc";
            this.lblTotalDesc.Size = new System.Drawing.Size(130, 12);
            this.lblTotalDesc.TabIndex = 2;
            this.lblTotalDesc.Text = "En período seleccionado";
            // 
            // lblTotalValor
            // 
            this.lblTotalValor.AutoSize = true;
            this.lblTotalValor.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTotalValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTotalValor.Location = new System.Drawing.Point(6, 20);
            this.lblTotalValor.Name = "lblTotalValor";
            this.lblTotalValor.Size = new System.Drawing.Size(26, 30);
            this.lblTotalValor.TabIndex = 1;
            this.lblTotalValor.Text = "0";
            // 
            // lblTotalTitulo
            // 
            this.lblTotalTitulo.AutoSize = true;
            this.lblTotalTitulo.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblTotalTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblTotalTitulo.Location = new System.Drawing.Point(8, 6);
            this.lblTotalTitulo.Name = "lblTotalTitulo";
            this.lblTotalTitulo.Size = new System.Drawing.Size(117, 13);
            this.lblTotalTitulo.TabIndex = 0;
            this.lblTotalTitulo.Text = "TOTAL INGRESOS";
            // 
            // pnlCardAlta
            // 
            this.pnlCardAlta.BackColor = System.Drawing.Color.White;
            this.pnlCardAlta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCardAlta.Controls.Add(this.lblAltaDesc);
            this.pnlCardAlta.Controls.Add(this.lblAltaValor);
            this.pnlCardAlta.Controls.Add(this.lblAltaTitulo);
            this.pnlCardAlta.Location = new System.Drawing.Point(220, 8);
            this.pnlCardAlta.Name = "pnlCardAlta";
            this.pnlCardAlta.Padding = new System.Windows.Forms.Padding(8);
            this.pnlCardAlta.Size = new System.Drawing.Size(195, 72);
            this.pnlCardAlta.TabIndex = 1;
            // 
            // lblAltaDesc
            // 
            this.lblAltaDesc.AutoSize = true;
            this.lblAltaDesc.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblAltaDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.lblAltaDesc.Location = new System.Drawing.Point(8, 50);
            this.lblAltaDesc.Name = "lblAltaDesc";
            this.lblAltaDesc.Size = new System.Drawing.Size(113, 12);
            this.lblAltaDesc.TabIndex = 2;
            this.lblAltaDesc.Text = "0% de los ingresos";
            // 
            // lblAltaValor
            // 
            this.lblAltaValor.AutoSize = true;
            this.lblAltaValor.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblAltaValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.lblAltaValor.Location = new System.Drawing.Point(6, 20);
            this.lblAltaValor.Name = "lblAltaValor";
            this.lblAltaValor.Size = new System.Drawing.Size(26, 30);
            this.lblAltaValor.TabIndex = 1;
            this.lblAltaValor.Text = "0";
            // 
            // lblAltaTitulo
            // 
            this.lblAltaTitulo.AutoSize = true;
            this.lblAltaTitulo.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblAltaTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.lblAltaTitulo.Location = new System.Drawing.Point(8, 6);
            this.lblAltaTitulo.Name = "lblAltaTitulo";
            this.lblAltaTitulo.Size = new System.Drawing.Size(155, 13);
            this.lblAltaTitulo.TabIndex = 0;
            this.lblAltaTitulo.Text = "TRIAGE ALTA (ROJO)";
            // 
            // pnlCardMedia
            // 
            this.pnlCardMedia.BackColor = System.Drawing.Color.White;
            this.pnlCardMedia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCardMedia.Controls.Add(this.lblMediaDesc);
            this.pnlCardMedia.Controls.Add(this.lblMediaValor);
            this.pnlCardMedia.Controls.Add(this.lblMediaTitulo);
            this.pnlCardMedia.Location = new System.Drawing.Point(426, 8);
            this.pnlCardMedia.Name = "pnlCardMedia";
            this.pnlCardMedia.Padding = new System.Windows.Forms.Padding(8);
            this.pnlCardMedia.Size = new System.Drawing.Size(195, 72);
            this.pnlCardMedia.TabIndex = 2;
            // 
            // lblMediaDesc
            // 
            this.lblMediaDesc.AutoSize = true;
            this.lblMediaDesc.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblMediaDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(83)))), ((int)(((byte)(9)))));
            this.lblMediaDesc.Location = new System.Drawing.Point(8, 50);
            this.lblMediaDesc.Name = "lblMediaDesc";
            this.lblMediaDesc.Size = new System.Drawing.Size(113, 12);
            this.lblMediaDesc.TabIndex = 2;
            this.lblMediaDesc.Text = "0% de los ingresos";
            // 
            // lblMediaValor
            // 
            this.lblMediaValor.AutoSize = true;
            this.lblMediaValor.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblMediaValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.lblMediaValor.Location = new System.Drawing.Point(6, 20);
            this.lblMediaValor.Name = "lblMediaValor";
            this.lblMediaValor.Size = new System.Drawing.Size(26, 30);
            this.lblMediaValor.TabIndex = 1;
            this.lblMediaValor.Text = "0";
            // 
            // lblMediaTitulo
            // 
            this.lblMediaTitulo.AutoSize = true;
            this.lblMediaTitulo.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblMediaTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(83)))), ((int)(((byte)(9)))));
            this.lblMediaTitulo.Location = new System.Drawing.Point(8, 6);
            this.lblMediaTitulo.Name = "lblMediaTitulo";
            this.lblMediaTitulo.Size = new System.Drawing.Size(167, 13);
            this.lblMediaTitulo.TabIndex = 0;
            this.lblMediaTitulo.Text = "TRIAGE MEDIA (AMARILLO)";
            // 
            // pnlCardBaja
            // 
            this.pnlCardBaja.BackColor = System.Drawing.Color.White;
            this.pnlCardBaja.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCardBaja.Controls.Add(this.lblBajaDesc);
            this.pnlCardBaja.Controls.Add(this.lblBajaValor);
            this.pnlCardBaja.Controls.Add(this.lblBajaTitulo);
            this.pnlCardBaja.Location = new System.Drawing.Point(632, 8);
            this.pnlCardBaja.Name = "pnlCardBaja";
            this.pnlCardBaja.Padding = new System.Windows.Forms.Padding(8);
            this.pnlCardBaja.Size = new System.Drawing.Size(195, 72);
            this.pnlCardBaja.TabIndex = 3;
            // 
            // lblBajaDesc
            // 
            this.lblBajaDesc.AutoSize = true;
            this.lblBajaDesc.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblBajaDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(120)))), ((int)(((byte)(87)))));
            this.lblBajaDesc.Location = new System.Drawing.Point(8, 50);
            this.lblBajaDesc.Name = "lblBajaDesc";
            this.lblBajaDesc.Size = new System.Drawing.Size(113, 12);
            this.lblBajaDesc.TabIndex = 2;
            this.lblBajaDesc.Text = "0% de los ingresos";
            // 
            // lblBajaValor
            // 
            this.lblBajaValor.AutoSize = true;
            this.lblBajaValor.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblBajaValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.lblBajaValor.Location = new System.Drawing.Point(6, 20);
            this.lblBajaValor.Name = "lblBajaValor";
            this.lblBajaValor.Size = new System.Drawing.Size(26, 30);
            this.lblBajaValor.TabIndex = 1;
            this.lblBajaValor.Text = "0";
            // 
            // lblBajaTitulo
            // 
            this.lblBajaTitulo.AutoSize = true;
            this.lblBajaTitulo.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblBajaTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(120)))), ((int)(((byte)(87)))));
            this.lblBajaTitulo.Location = new System.Drawing.Point(8, 6);
            this.lblBajaTitulo.Name = "lblBajaTitulo";
            this.lblBajaTitulo.Size = new System.Drawing.Size(155, 13);
            this.lblBajaTitulo.TabIndex = 0;
            this.lblBajaTitulo.Text = "TRIAGE BAJA (VERDE)";
            // 
            // pnlCardResolucion
            // 
            this.pnlCardResolucion.BackColor = System.Drawing.Color.White;
            this.pnlCardResolucion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCardResolucion.Controls.Add(this.lblResolucionDesc);
            this.pnlCardResolucion.Controls.Add(this.lblResolucionValor);
            this.pnlCardResolucion.Controls.Add(this.lblResolucionTitulo);
            this.pnlCardResolucion.Location = new System.Drawing.Point(838, 8);
            this.pnlCardResolucion.Name = "pnlCardResolucion";
            this.pnlCardResolucion.Padding = new System.Windows.Forms.Padding(8);
            this.pnlCardResolucion.Size = new System.Drawing.Size(225, 72);
            this.pnlCardResolucion.TabIndex = 4;
            // 
            // lblResolucionDesc
            // 
            this.lblResolucionDesc.AutoSize = true;
            this.lblResolucionDesc.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblResolucionDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.lblResolucionDesc.Location = new System.Drawing.Point(8, 50);
            this.lblResolucionDesc.Name = "lblResolucionDesc";
            this.lblResolucionDesc.Size = new System.Drawing.Size(161, 12);
            this.lblResolucionDesc.TabIndex = 2;
            this.lblResolucionDesc.Text = "0 atendidos / 0 en espera";
            // 
            // lblResolucionValor
            // 
            this.lblResolucionValor.AutoSize = true;
            this.lblResolucionValor.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblResolucionValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.lblResolucionValor.Location = new System.Drawing.Point(6, 20);
            this.lblResolucionValor.Name = "lblResolucionValor";
            this.lblResolucionValor.Size = new System.Drawing.Size(65, 30);
            this.lblResolucionValor.TabIndex = 1;
            this.lblResolucionValor.Text = "0.0%";
            // 
            // lblResolucionTitulo
            // 
            this.lblResolucionTitulo.AutoSize = true;
            this.lblResolucionTitulo.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblResolucionTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.lblResolucionTitulo.Location = new System.Drawing.Point(8, 6);
            this.lblResolucionTitulo.Name = "lblResolucionTitulo";
            this.lblResolucionTitulo.Size = new System.Drawing.Size(147, 13);
            this.lblResolucionTitulo.TabIndex = 0;
            this.lblResolucionTitulo.Text = "TASA DE RESOLUCIÓN";
            // 
            // tabControlReporte
            // 
            this.tabControlReporte.Controls.Add(this.tabDetalle);
            this.tabControlReporte.Controls.Add(this.tabRanking);
            this.tabControlReporte.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlReporte.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.tabControlReporte.Location = new System.Drawing.Point(0, 238);
            this.tabControlReporte.Name = "tabControlReporte";
            this.tabControlReporte.SelectedIndex = 0;
            this.tabControlReporte.Size = new System.Drawing.Size(1080, 392);
            this.tabControlReporte.TabIndex = 3;
            // 
            // tabDetalle
            // 
            this.tabDetalle.Controls.Add(this.dgvDetalleGuardia);
            this.tabDetalle.Location = new System.Drawing.Point(4, 26);
            this.tabDetalle.Name = "tabDetalle";
            this.tabDetalle.Padding = new System.Windows.Forms.Padding(3);
            this.tabDetalle.Size = new System.Drawing.Size(1072, 362);
            this.tabDetalle.TabIndex = 0;
            this.tabDetalle.Text = "📋 Detalle de Ingresos a Guardia";
            this.tabDetalle.UseVisualStyleBackColor = true;
            // 
            // dgvDetalleGuardia
            // 
            this.dgvDetalleGuardia.AllowUserToAddRows = false;
            this.dgvDetalleGuardia.AllowUserToDeleteRows = false;
            this.dgvDetalleGuardia.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDetalleGuardia.BackgroundColor = System.Drawing.Color.White;
            this.dgvDetalleGuardia.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetalleGuardia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalleGuardia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetalleGuardia.Location = new System.Drawing.Point(3, 3);
            this.dgvDetalleGuardia.Name = "dgvDetalleGuardia";
            this.dgvDetalleGuardia.ReadOnly = true;
            this.dgvDetalleGuardia.RowHeadersVisible = false;
            this.dgvDetalleGuardia.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalleGuardia.Size = new System.Drawing.Size(1066, 356);
            this.dgvDetalleGuardia.TabIndex = 0;
            // 
            // tabRanking
            // 
            this.tabRanking.Controls.Add(this.dgvRankingSintomas);
            this.tabRanking.Location = new System.Drawing.Point(4, 26);
            this.tabRanking.Name = "tabRanking";
            this.tabRanking.Padding = new System.Windows.Forms.Padding(3);
            this.tabRanking.Size = new System.Drawing.Size(1072, 362);
            this.tabRanking.TabIndex = 1;
            this.tabRanking.Text = "📊 Ranking de Síntomas Predominantes";
            this.tabRanking.UseVisualStyleBackColor = true;
            // 
            // dgvRankingSintomas
            // 
            this.dgvRankingSintomas.AllowUserToAddRows = false;
            this.dgvRankingSintomas.AllowUserToDeleteRows = false;
            this.dgvRankingSintomas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRankingSintomas.BackgroundColor = System.Drawing.Color.White;
            this.dgvRankingSintomas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvRankingSintomas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRankingSintomas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRankingSintomas.Location = new System.Drawing.Point(3, 3);
            this.dgvRankingSintomas.Name = "dgvRankingSintomas";
            this.dgvRankingSintomas.ReadOnly = true;
            this.dgvRankingSintomas.RowHeadersVisible = false;
            this.dgvRankingSintomas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRankingSintomas.Size = new System.Drawing.Size(1066, 356);
            this.dgvRankingSintomas.TabIndex = 0;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFooter.Controls.Add(this.lblTotalRegistros);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 630);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(12, 6, 12, 6);
            this.pnlFooter.Size = new System.Drawing.Size(1080, 32);
            this.pnlFooter.TabIndex = 4;
            // 
            // lblTotalRegistros
            // 
            this.lblTotalRegistros.AutoSize = true;
            this.lblTotalRegistros.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalRegistros.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblTotalRegistros.Location = new System.Drawing.Point(12, 8);
            this.lblTotalRegistros.Name = "lblTotalRegistros";
            this.lblTotalRegistros.Size = new System.Drawing.Size(161, 15);
            this.lblTotalRegistros.TabIndex = 0;
            this.lblTotalRegistros.Text = "Total registros mostrados: 0";
            // 
            // FrmReporteGuardiaAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1080, 662);
            this.Controls.Add(this.tabControlReporte);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlCards);
            this.Controls.Add(this.pnlFiltros);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmReporteGuardiaAdmin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte Operativo de Guardia: Triage y Distribución de Urgencias";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.pnlCards.ResumeLayout(false);
            this.pnlCardTotal.ResumeLayout(false);
            this.pnlCardTotal.PerformLayout();
            this.pnlCardAlta.ResumeLayout(false);
            this.pnlCardAlta.PerformLayout();
            this.pnlCardMedia.ResumeLayout(false);
            this.pnlCardMedia.PerformLayout();
            this.pnlCardBaja.ResumeLayout(false);
            this.pnlCardBaja.PerformLayout();
            this.pnlCardResolucion.ResumeLayout(false);
            this.pnlCardResolucion.PerformLayout();
            this.tabControlReporte.ResumeLayout(false);
            this.tabDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleGuardia)).EndInit();
            this.tabRanking.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRankingSintomas)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTituloHeader;
        private System.Windows.Forms.Label lblSubtituloHeader;
        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblPeriodo;
        private System.Windows.Forms.ComboBox cmbPeriodo;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblPrioridad;
        private System.Windows.Forms.ComboBox cmbPrioridad;
        private System.Windows.Forms.Button btnFiltrar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Panel pnlCards;
        private System.Windows.Forms.Panel pnlCardTotal;
        private System.Windows.Forms.Label lblTotalTitulo;
        private System.Windows.Forms.Label lblTotalValor;
        private System.Windows.Forms.Label lblTotalDesc;
        private System.Windows.Forms.Panel pnlCardAlta;
        private System.Windows.Forms.Label lblAltaTitulo;
        private System.Windows.Forms.Label lblAltaValor;
        private System.Windows.Forms.Label lblAltaDesc;
        private System.Windows.Forms.Panel pnlCardMedia;
        private System.Windows.Forms.Label lblMediaTitulo;
        private System.Windows.Forms.Label lblMediaValor;
        private System.Windows.Forms.Label lblMediaDesc;
        private System.Windows.Forms.Panel pnlCardBaja;
        private System.Windows.Forms.Label lblBajaTitulo;
        private System.Windows.Forms.Label lblBajaValor;
        private System.Windows.Forms.Label lblBajaDesc;
        private System.Windows.Forms.Panel pnlCardResolucion;
        private System.Windows.Forms.Label lblResolucionTitulo;
        private System.Windows.Forms.Label lblResolucionValor;
        private System.Windows.Forms.Label lblResolucionDesc;
        private System.Windows.Forms.TabControl tabControlReporte;
        private System.Windows.Forms.TabPage tabDetalle;
        private System.Windows.Forms.DataGridView dgvDetalleGuardia;
        private System.Windows.Forms.TabPage tabRanking;
        private System.Windows.Forms.DataGridView dgvRankingSintomas;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblTotalRegistros;
    }
}
