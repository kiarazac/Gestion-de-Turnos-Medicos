namespace Gestion_de_Turnos_Medicos
{
    partial class FrmReportesAdmin
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
            pnlKpis = new TableLayoutPanel();
            cardTotalTurnos = new Panel();
            lblKpiTotalTitulo = new Label();
            lblKpiTotalNum = new Label();
            cardAtendidos = new Panel();
            lblKpiAtendidosTitulo = new Label();
            lblKpiAtendidosNum = new Label();
            cardCancelados = new Panel();
            lblKpiCanceladosTitulo = new Label();
            lblKpiCanceladosNum = new Label();
            cardTopEspecialidad = new Panel();
            lblKpiTopTitulo = new Label();
            lblKpiTopNombre = new Label();
            pnlFiltrosWrapper = new Panel();
            pnlFiltros = new Panel();
            btnExportar = new Button();
            btnFiltrar = new Button();
            cmbEspecialidad = new ComboBox();
            lblEspecialidad = new Label();
            dtpHasta = new DateTimePicker();
            lblHasta = new Label();
            dtpDesde = new DateTimePicker();
            lblDesde = new Label();
            cmbPeriodo = new ComboBox();
            lblPeriodo = new Label();
            pnlContenido = new Panel();
            tabControlReportes = new TabControl();
            tabDemanda = new TabPage();
            dgvDemanda = new DataGridView();
            tabProductividad = new TabPage();
            dgvProductividad = new DataGridView();
            pnlFooter = new Panel();
            lblEstado = new Label();
            pnlHeader.SuspendLayout();
            pnlKpis.SuspendLayout();
            cardTotalTurnos.SuspendLayout();
            cardAtendidos.SuspendLayout();
            cardCancelados.SuspendLayout();
            cardTopEspecialidad.SuspendLayout();
            pnlFiltrosWrapper.SuspendLayout();
            pnlFiltros.SuspendLayout();
            pnlContenido.SuspendLayout();
            tabControlReportes.SuspendLayout();
            tabDemanda.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDemanda).BeginInit();
            tabProductividad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductividad).BeginInit();
            pnlFooter.SuspendLayout();
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
            pnlHeader.Size = new Size(1100, 62);
            pnlHeader.TabIndex = 0;
            // 
            // lblSubtituloHeader
            // 
            lblSubtituloHeader.AutoSize = true;
            lblSubtituloHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSubtituloHeader.ForeColor = Color.FromArgb(204, 251, 241);
            lblSubtituloHeader.Location = new Point(18, 35);
            lblSubtituloHeader.Name = "lblSubtituloHeader";
            lblSubtituloHeader.Size = new Size(540, 15);
            lblSubtituloHeader.TabIndex = 1;
            lblSubtituloHeader.Text = "Estadísticas ejecutivas de demanda, resolución de turnos y productividad del cuerpo médico";
            // 
            // lblTituloHeader
            // 
            lblTituloHeader.AutoSize = true;
            lblTituloHeader.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloHeader.ForeColor = Color.White;
            lblTituloHeader.Location = new Point(16, 9);
            lblTituloHeader.Name = "lblTituloHeader";
            lblTituloHeader.Size = new Size(410, 25);
            lblTituloHeader.TabIndex = 0;
            lblTituloHeader.Text = "Panel de Reportes Gerenciales y Estadísticas";
            // 
            // pnlKpis
            // 
            pnlKpis.ColumnCount = 4;
            pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            pnlKpis.Controls.Add(cardTotalTurnos, 0, 0);
            pnlKpis.Controls.Add(cardAtendidos, 1, 0);
            pnlKpis.Controls.Add(cardCancelados, 2, 0);
            pnlKpis.Controls.Add(cardTopEspecialidad, 3, 0);
            pnlKpis.Dock = DockStyle.Top;
            pnlKpis.Location = new Point(0, 62);
            pnlKpis.Name = "pnlKpis";
            pnlKpis.Padding = new Padding(12, 10, 12, 4);
            pnlKpis.RowCount = 1;
            pnlKpis.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlKpis.Size = new Size(1100, 88);
            pnlKpis.TabIndex = 1;
            // 
            // cardTotalTurnos
            // 
            cardTotalTurnos.BackColor = Color.White;
            cardTotalTurnos.BorderStyle = BorderStyle.FixedSingle;
            cardTotalTurnos.Controls.Add(lblKpiTotalTitulo);
            cardTotalTurnos.Controls.Add(lblKpiTotalNum);
            cardTotalTurnos.Dock = DockStyle.Fill;
            cardTotalTurnos.Location = new Point(15, 13);
            cardTotalTurnos.Margin = new Padding(3, 3, 8, 3);
            cardTotalTurnos.Name = "cardTotalTurnos";
            cardTotalTurnos.Size = new Size(258, 68);
            cardTotalTurnos.TabIndex = 0;
            // 
            // lblKpiTotalTitulo
            // 
            lblKpiTotalTitulo.AutoSize = true;
            lblKpiTotalTitulo.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblKpiTotalTitulo.ForeColor = Color.FromArgb(100, 116, 139);
            lblKpiTotalTitulo.Location = new Point(10, 10);
            lblKpiTotalTitulo.Name = "lblKpiTotalTitulo";
            lblKpiTotalTitulo.Size = new Size(160, 13);
            lblKpiTotalTitulo.TabIndex = 0;
            lblKpiTotalTitulo.Text = "TOTAL TURNOS REGISTRADOS";
            // 
            // lblKpiTotalNum
            // 
            lblKpiTotalNum.AutoSize = true;
            lblKpiTotalNum.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblKpiTotalNum.ForeColor = Color.FromArgb(15, 23, 42);
            lblKpiTotalNum.Location = new Point(8, 28);
            lblKpiTotalNum.Name = "lblKpiTotalNum";
            lblKpiTotalNum.Size = new Size(28, 32);
            lblKpiTotalNum.TabIndex = 1;
            lblKpiTotalNum.Text = "0";
            // 
            // cardAtendidos
            // 
            cardAtendidos.BackColor = Color.White;
            cardAtendidos.BorderStyle = BorderStyle.FixedSingle;
            cardAtendidos.Controls.Add(lblKpiAtendidosTitulo);
            cardAtendidos.Controls.Add(lblKpiAtendidosNum);
            cardAtendidos.Dock = DockStyle.Fill;
            cardAtendidos.Location = new Point(284, 13);
            cardAtendidos.Margin = new Padding(3, 3, 8, 3);
            cardAtendidos.Name = "cardAtendidos";
            cardAtendidos.Size = new Size(258, 68);
            cardAtendidos.TabIndex = 1;
            // 
            // lblKpiAtendidosTitulo
            // 
            lblKpiAtendidosTitulo.AutoSize = true;
            lblKpiAtendidosTitulo.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblKpiAtendidosTitulo.ForeColor = Color.FromArgb(100, 116, 139);
            lblKpiAtendidosTitulo.Location = new Point(10, 10);
            lblKpiAtendidosTitulo.Name = "lblKpiAtendidosTitulo";
            lblKpiAtendidosTitulo.Size = new Size(118, 13);
            lblKpiAtendidosTitulo.TabIndex = 0;
            lblKpiAtendidosTitulo.Text = "TURNOS ATENDIDOS";
            // 
            // lblKpiAtendidosNum
            // 
            lblKpiAtendidosNum.AutoSize = true;
            lblKpiAtendidosNum.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblKpiAtendidosNum.ForeColor = Color.FromArgb(16, 185, 129);
            lblKpiAtendidosNum.Location = new Point(8, 28);
            lblKpiAtendidosNum.Name = "lblKpiAtendidosNum";
            lblKpiAtendidosNum.Size = new Size(74, 32);
            lblKpiAtendidosNum.TabIndex = 1;
            lblKpiAtendidosNum.Text = "0 (0%)";
            // 
            // cardCancelados
            // 
            cardCancelados.BackColor = Color.White;
            cardCancelados.BorderStyle = BorderStyle.FixedSingle;
            cardCancelados.Controls.Add(lblKpiCanceladosTitulo);
            cardCancelados.Controls.Add(lblKpiCanceladosNum);
            cardCancelados.Dock = DockStyle.Fill;
            cardCancelados.Location = new Point(553, 13);
            cardCancelados.Margin = new Padding(3, 3, 8, 3);
            cardCancelados.Name = "cardCancelados";
            cardCancelados.Size = new Size(258, 68);
            cardCancelados.TabIndex = 2;
            // 
            // lblKpiCanceladosTitulo
            // 
            lblKpiCanceladosTitulo.AutoSize = true;
            lblKpiCanceladosTitulo.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblKpiCanceladosTitulo.ForeColor = Color.FromArgb(100, 116, 139);
            lblKpiCanceladosTitulo.Location = new Point(10, 10);
            lblKpiCanceladosTitulo.Name = "lblKpiCanceladosTitulo";
            lblKpiCanceladosTitulo.Size = new Size(128, 13);
            lblKpiCanceladosTitulo.TabIndex = 0;
            lblKpiCanceladosTitulo.Text = "TURNOS CANCELADOS";
            // 
            // lblKpiCanceladosNum
            // 
            lblKpiCanceladosNum.AutoSize = true;
            lblKpiCanceladosNum.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblKpiCanceladosNum.ForeColor = Color.FromArgb(239, 68, 68);
            lblKpiCanceladosNum.Location = new Point(8, 28);
            lblKpiCanceladosNum.Name = "lblKpiCanceladosNum";
            lblKpiCanceladosNum.Size = new Size(74, 32);
            lblKpiCanceladosNum.TabIndex = 1;
            lblKpiCanceladosNum.Text = "0 (0%)";
            // 
            // cardTopEspecialidad
            // 
            cardTopEspecialidad.BackColor = Color.White;
            cardTopEspecialidad.BorderStyle = BorderStyle.FixedSingle;
            cardTopEspecialidad.Controls.Add(lblKpiTopTitulo);
            cardTopEspecialidad.Controls.Add(lblKpiTopNombre);
            cardTopEspecialidad.Dock = DockStyle.Fill;
            cardTopEspecialidad.Location = new Point(822, 13);
            cardTopEspecialidad.Margin = new Padding(3, 3, 3, 3);
            cardTopEspecialidad.Name = "cardTopEspecialidad";
            cardTopEspecialidad.Size = new Size(263, 68);
            cardTopEspecialidad.TabIndex = 3;
            // 
            // lblKpiTopTitulo
            // 
            lblKpiTopTitulo.AutoSize = true;
            lblKpiTopTitulo.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblKpiTopTitulo.ForeColor = Color.FromArgb(100, 116, 139);
            lblKpiTopTitulo.Location = new Point(10, 10);
            lblKpiTopTitulo.Name = "lblKpiTopTitulo";
            lblKpiTopTitulo.Size = new Size(182, 13);
            lblKpiTopTitulo.TabIndex = 0;
            lblKpiTopTitulo.Text = "ESPECIALIDAD MAYOR DEMANDA";
            // 
            // lblKpiTopNombre
            // 
            lblKpiTopNombre.AutoSize = true;
            lblKpiTopNombre.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblKpiTopNombre.ForeColor = Color.FromArgb(30, 64, 175);
            lblKpiTopNombre.Location = new Point(8, 32);
            lblKpiTopNombre.Name = "lblKpiTopNombre";
            lblKpiTopNombre.Size = new Size(19, 25);
            lblKpiTopNombre.TabIndex = 1;
            lblKpiTopNombre.Text = "-";
            // 
            // pnlFiltrosWrapper
            // 
            pnlFiltrosWrapper.Controls.Add(pnlFiltros);
            pnlFiltrosWrapper.Dock = DockStyle.Top;
            pnlFiltrosWrapper.Location = new Point(0, 150);
            pnlFiltrosWrapper.Name = "pnlFiltrosWrapper";
            pnlFiltrosWrapper.Padding = new Padding(12, 4, 12, 6);
            pnlFiltrosWrapper.Size = new Size(1100, 72);
            pnlFiltrosWrapper.TabIndex = 2;
            // 
            // pnlFiltros
            // 
            pnlFiltros.BackColor = Color.White;
            pnlFiltros.BorderStyle = BorderStyle.FixedSingle;
            pnlFiltros.Controls.Add(btnExportar);
            pnlFiltros.Controls.Add(btnFiltrar);
            pnlFiltros.Controls.Add(cmbEspecialidad);
            pnlFiltros.Controls.Add(lblEspecialidad);
            pnlFiltros.Controls.Add(dtpHasta);
            pnlFiltros.Controls.Add(lblHasta);
            pnlFiltros.Controls.Add(dtpDesde);
            pnlFiltros.Controls.Add(lblDesde);
            pnlFiltros.Controls.Add(cmbPeriodo);
            pnlFiltros.Controls.Add(lblPeriodo);
            pnlFiltros.Dock = DockStyle.Fill;
            pnlFiltros.Location = new Point(12, 4);
            pnlFiltros.Name = "pnlFiltros";
            pnlFiltros.Size = new Size(1076, 62);
            pnlFiltros.TabIndex = 0;
            // 
            // btnExportar
            // 
            btnExportar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportar.BackColor = Color.FromArgb(51, 65, 85);
            btnExportar.Cursor = Cursors.Hand;
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.FlatStyle = FlatStyle.Flat;
            btnExportar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExportar.ForeColor = Color.White;
            btnExportar.Location = new Point(948, 14);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(115, 32);
            btnExportar.TabIndex = 9;
            btnExportar.Text = "📁 Exportar...";
            btnExportar.UseVisualStyleBackColor = false;
            // 
            // btnFiltrar
            // 
            btnFiltrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFiltrar.BackColor = Color.FromArgb(15, 118, 110);
            btnFiltrar.Cursor = Cursors.Hand;
            btnFiltrar.FlatAppearance.BorderSize = 0;
            btnFiltrar.FlatStyle = FlatStyle.Flat;
            btnFiltrar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnFiltrar.ForeColor = Color.White;
            btnFiltrar.Location = new Point(836, 14);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(105, 32);
            btnFiltrar.TabIndex = 8;
            btnFiltrar.Text = "🔍 Filtrar";
            btnFiltrar.UseVisualStyleBackColor = false;
            // 
            // cmbEspecialidad
            // 
            cmbEspecialidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEspecialidad.Font = new Font("Segoe UI", 9.25F);
            cmbEspecialidad.FormattingEnabled = true;
            cmbEspecialidad.Location = new Point(540, 20);
            cmbEspecialidad.Name = "cmbEspecialidad";
            cmbEspecialidad.Size = new Size(190, 23);
            cmbEspecialidad.TabIndex = 7;
            // 
            // lblEspecialidad
            // 
            lblEspecialidad.AutoSize = true;
            lblEspecialidad.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblEspecialidad.ForeColor = Color.FromArgb(71, 85, 105);
            lblEspecialidad.Location = new Point(538, 4);
            lblEspecialidad.Name = "lblEspecialidad";
            lblEspecialidad.Size = new Size(74, 13);
            lblEspecialidad.TabIndex = 6;
            lblEspecialidad.Text = "Especialidad:";
            // 
            // dtpHasta
            // 
            dtpHasta.Font = new Font("Segoe UI", 9.25F);
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(390, 20);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(130, 24);
            dtpHasta.TabIndex = 5;
            // 
            // lblHasta
            // 
            lblHasta.AutoSize = true;
            lblHasta.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblHasta.ForeColor = Color.FromArgb(71, 85, 105);
            lblHasta.Location = new Point(388, 4);
            lblHasta.Name = "lblHasta";
            lblHasta.Size = new Size(40, 13);
            lblHasta.TabIndex = 4;
            lblHasta.Text = "Hasta:";
            // 
            // dtpDesde
            // 
            dtpDesde.Font = new Font("Segoe UI", 9.25F);
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(245, 20);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(130, 24);
            dtpDesde.TabIndex = 3;
            // 
            // lblDesde
            // 
            lblDesde.AutoSize = true;
            lblDesde.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDesde.ForeColor = Color.FromArgb(71, 85, 105);
            lblDesde.Location = new Point(243, 4);
            lblDesde.Name = "lblDesde";
            lblDesde.Size = new Size(42, 13);
            lblDesde.TabIndex = 2;
            lblDesde.Text = "Desde:";
            // 
            // cmbPeriodo
            // 
            cmbPeriodo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPeriodo.Font = new Font("Segoe UI", 9.25F);
            cmbPeriodo.FormattingEnabled = true;
            cmbPeriodo.Location = new Point(14, 20);
            cmbPeriodo.Name = "cmbPeriodo";
            cmbPeriodo.Size = new Size(215, 23);
            cmbPeriodo.TabIndex = 1;
            // 
            // lblPeriodo
            // 
            lblPeriodo.AutoSize = true;
            lblPeriodo.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPeriodo.ForeColor = Color.FromArgb(71, 85, 105);
            lblPeriodo.Location = new Point(12, 4);
            lblPeriodo.Name = "lblPeriodo";
            lblPeriodo.Size = new Size(50, 13);
            lblPeriodo.TabIndex = 0;
            lblPeriodo.Text = "Período:";
            // 
            // pnlContenido
            // 
            pnlContenido.Controls.Add(tabControlReportes);
            pnlContenido.Dock = DockStyle.Fill;
            pnlContenido.Location = new Point(0, 222);
            pnlContenido.Name = "pnlContenido";
            pnlContenido.Padding = new Padding(12, 4, 12, 6);
            pnlContenido.Size = new Size(1100, 428);
            pnlContenido.TabIndex = 3;
            // 
            // tabControlReportes
            // 
            tabControlReportes.Controls.Add(tabDemanda);
            tabControlReportes.Controls.Add(tabProductividad);
            tabControlReportes.Dock = DockStyle.Fill;
            tabControlReportes.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tabControlReportes.Location = new Point(12, 4);
            tabControlReportes.Name = "tabControlReportes";
            tabControlReportes.SelectedIndex = 0;
            tabControlReportes.Size = new Size(1076, 418);
            tabControlReportes.TabIndex = 0;
            // 
            // tabDemanda
            // 
            tabDemanda.BackColor = Color.White;
            tabDemanda.Controls.Add(dgvDemanda);
            tabDemanda.Location = new Point(4, 26);
            tabDemanda.Name = "tabDemanda";
            tabDemanda.Padding = new Padding(8);
            tabDemanda.Size = new Size(1068, 388);
            tabDemanda.TabIndex = 0;
            tabDemanda.Text = "  📊 Demanda por Especialidad  ";
            // 
            // dgvDemanda
            // 
            dgvDemanda.AllowUserToAddRows = false;
            dgvDemanda.AllowUserToDeleteRows = false;
            dgvDemanda.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDemanda.BackgroundColor = Color.White;
            dgvDemanda.BorderStyle = BorderStyle.None;
            dgvDemanda.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDemanda.Dock = DockStyle.Fill;
            dgvDemanda.Location = new Point(8, 8);
            dgvDemanda.Name = "dgvDemanda";
            dgvDemanda.ReadOnly = true;
            dgvDemanda.RowHeadersVisible = false;
            dgvDemanda.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDemanda.Size = new Size(1052, 372);
            dgvDemanda.TabIndex = 0;
            // 
            // tabProductividad
            // 
            tabProductividad.BackColor = Color.White;
            tabProductividad.Controls.Add(dgvProductividad);
            tabProductividad.Location = new Point(4, 26);
            tabProductividad.Name = "tabProductividad";
            tabProductividad.Padding = new Padding(8);
            tabProductividad.Size = new Size(1068, 388);
            tabProductividad.TabIndex = 1;
            tabProductividad.Text = "  👨‍⚕️ Productividad por Médico  ";
            // 
            // dgvProductividad
            // 
            dgvProductividad.AllowUserToAddRows = false;
            dgvProductividad.AllowUserToDeleteRows = false;
            dgvProductividad.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductividad.BackgroundColor = Color.White;
            dgvProductividad.BorderStyle = BorderStyle.None;
            dgvProductividad.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductividad.Dock = DockStyle.Fill;
            dgvProductividad.Location = new Point(8, 8);
            dgvProductividad.Name = "dgvProductividad";
            dgvProductividad.ReadOnly = true;
            dgvProductividad.RowHeadersVisible = false;
            dgvProductividad.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductividad.Size = new Size(1052, 372);
            dgvProductividad.TabIndex = 0;
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.White;
            pnlFooter.BorderStyle = BorderStyle.FixedSingle;
            pnlFooter.Controls.Add(lblEstado);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 650);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(1100, 30);
            pnlFooter.TabIndex = 4;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblEstado.ForeColor = Color.FromArgb(71, 85, 105);
            lblEstado.Location = new Point(12, 7);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(38, 15);
            lblEstado.TabIndex = 0;
            lblEstado.Text = "Listo.";
            // 
            // FrmReportesAdmin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(241, 245, 249);
            ClientSize = new Size(1100, 680);
            Controls.Add(pnlContenido);
            Controls.Add(pnlFooter);
            Controls.Add(pnlFiltrosWrapper);
            Controls.Add(pnlKpis);
            Controls.Add(pnlHeader);
            Name = "FrmReportesAdmin";
            Text = "Reportes Gerenciales y Estadísticas";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlKpis.ResumeLayout(false);
            cardTotalTurnos.ResumeLayout(false);
            cardTotalTurnos.PerformLayout();
            cardAtendidos.ResumeLayout(false);
            cardAtendidos.PerformLayout();
            cardCancelados.ResumeLayout(false);
            cardCancelados.PerformLayout();
            cardTopEspecialidad.ResumeLayout(false);
            cardTopEspecialidad.PerformLayout();
            pnlFiltrosWrapper.ResumeLayout(false);
            pnlFiltros.ResumeLayout(false);
            pnlFiltros.PerformLayout();
            pnlContenido.ResumeLayout(false);
            tabControlReportes.ResumeLayout(false);
            tabDemanda.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvDemanda).EndInit();
            tabProductividad.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProductividad).EndInit();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblTituloHeader;
        private Label lblSubtituloHeader;
        private TableLayoutPanel pnlKpis;
        private Panel cardTotalTurnos;
        private Label lblKpiTotalTitulo;
        private Label lblKpiTotalNum;
        private Panel cardAtendidos;
        private Label lblKpiAtendidosTitulo;
        private Label lblKpiAtendidosNum;
        private Panel cardCancelados;
        private Label lblKpiCanceladosTitulo;
        private Label lblKpiCanceladosNum;
        private Panel cardTopEspecialidad;
        private Label lblKpiTopTitulo;
        private Label lblKpiTopNombre;
        private Panel pnlFiltrosWrapper;
        private Panel pnlFiltros;
        private Label lblPeriodo;
        private ComboBox cmbPeriodo;
        private Label lblDesde;
        private DateTimePicker dtpDesde;
        private Label lblHasta;
        private DateTimePicker dtpHasta;
        private Label lblEspecialidad;
        private ComboBox cmbEspecialidad;
        private Button btnFiltrar;
        private Button btnExportar;
        private Panel pnlContenido;
        private TabControl tabControlReportes;
        private TabPage tabDemanda;
        private TabPage tabProductividad;
        private DataGridView dgvDemanda;
        private DataGridView dgvProductividad;
        private Panel pnlFooter;
        private Label lblEstado;
    }
}
