namespace Gestion_de_Turnos_Medicos
{
    partial class FrmMisAusencias
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
            pnlHeader = new Panel();
            lblSubtituloHeader = new Label();
            lblTituloHeader = new Label();
            pnlCarga = new Panel();
            lblAyuda = new Label();
            btnRegistrar = new Button();
            txtMotivo = new TextBox();
            lblMotivo = new Label();
            dtpHasta = new DateTimePicker();
            lblHasta = new Label();
            dtpDesde = new DateTimePicker();
            lblDesde = new Label();
            dgvAusencias = new DataGridView();
            btnAnular = new Button();
            pnlHeader.SuspendLayout();
            pnlCarga.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAusencias).BeginInit();
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
            pnlHeader.Size = new Size(875, 60);
            pnlHeader.TabIndex = 0;
            //
            // lblSubtituloHeader
            //
            lblSubtituloHeader.AutoSize = true;
            lblSubtituloHeader.Font = new Font("Segoe UI", 9F);
            lblSubtituloHeader.ForeColor = Color.FromArgb(204, 251, 241);
            lblSubtituloHeader.Location = new Point(18, 34);
            lblSubtituloHeader.Name = "lblSubtituloHeader";
            lblSubtituloHeader.Size = new Size(470, 15);
            lblSubtituloHeader.TabIndex = 1;
            lblSubtituloHeader.Text = "Registre los días en que no atenderá para bloquear nuevos turnos y avisar a recepción";
            //
            // lblTituloHeader
            //
            lblTituloHeader.AutoSize = true;
            lblTituloHeader.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            lblTituloHeader.ForeColor = Color.White;
            lblTituloHeader.Location = new Point(16, 8);
            lblTituloHeader.Name = "lblTituloHeader";
            lblTituloHeader.Size = new Size(147, 25);
            lblTituloHeader.TabIndex = 0;
            lblTituloHeader.Text = "Mis Ausencias";
            //
            // pnlCarga
            //
            pnlCarga.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlCarga.BackColor = Color.White;
            pnlCarga.Controls.Add(lblAyuda);
            pnlCarga.Controls.Add(btnRegistrar);
            pnlCarga.Controls.Add(txtMotivo);
            pnlCarga.Controls.Add(lblMotivo);
            pnlCarga.Controls.Add(dtpHasta);
            pnlCarga.Controls.Add(lblHasta);
            pnlCarga.Controls.Add(dtpDesde);
            pnlCarga.Controls.Add(lblDesde);
            pnlCarga.Location = new Point(12, 72);
            pnlCarga.Name = "pnlCarga";
            pnlCarga.Size = new Size(851, 100);
            pnlCarga.TabIndex = 1;
            //
            // lblAyuda
            //
            lblAyuda.AutoSize = true;
            lblAyuda.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            lblAyuda.ForeColor = Color.FromArgb(100, 116, 139);
            lblAyuda.Location = new Point(16, 72);
            lblAyuda.Name = "lblAyuda";
            lblAyuda.Size = new Size(520, 13);
            lblAyuda.TabIndex = 7;
            lblAyuda.Text = "Las ausencias se cargan por día completo y con al menos un día de anticipación, para que recepción pueda avisar a los pacientes.";
            //
            // btnRegistrar
            //
            btnRegistrar.BackColor = Color.FromArgb(15, 118, 110);
            btnRegistrar.Cursor = Cursors.Hand;
            btnRegistrar.FlatStyle = FlatStyle.Flat;
            btnRegistrar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRegistrar.ForeColor = Color.White;
            btnRegistrar.Location = new Point(680, 32);
            btnRegistrar.Name = "btnRegistrar";
            btnRegistrar.Size = new Size(155, 30);
            btnRegistrar.TabIndex = 6;
            btnRegistrar.Text = "Registrar ausencia";
            btnRegistrar.UseVisualStyleBackColor = false;
            //
            // txtMotivo
            //
            txtMotivo.Location = new Point(356, 36);
            txtMotivo.MaxLength = 200;
            txtMotivo.Name = "txtMotivo";
            txtMotivo.PlaceholderText = "Opcional (ej. congreso, licencia)";
            txtMotivo.Size = new Size(305, 23);
            txtMotivo.TabIndex = 5;
            //
            // lblMotivo
            //
            lblMotivo.AutoSize = true;
            lblMotivo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMotivo.Location = new Point(356, 14);
            lblMotivo.Name = "lblMotivo";
            lblMotivo.Size = new Size(48, 15);
            lblMotivo.TabIndex = 4;
            lblMotivo.Text = "Motivo";
            //
            // dtpHasta
            //
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(186, 36);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(150, 23);
            dtpHasta.TabIndex = 3;
            //
            // lblHasta
            //
            lblHasta.AutoSize = true;
            lblHasta.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHasta.Location = new Point(186, 14);
            lblHasta.Name = "lblHasta";
            lblHasta.Size = new Size(39, 15);
            lblHasta.TabIndex = 2;
            lblHasta.Text = "Hasta";
            //
            // dtpDesde
            //
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(16, 36);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(150, 23);
            dtpDesde.TabIndex = 1;
            //
            // lblDesde
            //
            lblDesde.AutoSize = true;
            lblDesde.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDesde.Location = new Point(16, 14);
            lblDesde.Name = "lblDesde";
            lblDesde.Size = new Size(42, 15);
            lblDesde.TabIndex = 0;
            lblDesde.Text = "Desde";
            //
            // dgvAusencias
            //
            dgvAusencias.AllowUserToAddRows = false;
            dgvAusencias.AllowUserToDeleteRows = false;
            dgvAusencias.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvAusencias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAusencias.BackgroundColor = Color.FromArgb(248, 250, 252);
            dgvAusencias.BorderStyle = BorderStyle.None;
            dgvAusencias.Location = new Point(12, 184);
            dgvAusencias.MultiSelect = false;
            dgvAusencias.Name = "dgvAusencias";
            dgvAusencias.ReadOnly = true;
            dgvAusencias.RowHeadersVisible = false;
            dgvAusencias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAusencias.Size = new Size(851, 345);
            dgvAusencias.TabIndex = 2;
            //
            // btnAnular
            //
            btnAnular.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnAnular.BackColor = Color.FromArgb(127, 29, 29);
            btnAnular.Cursor = Cursors.Hand;
            btnAnular.FlatStyle = FlatStyle.Flat;
            btnAnular.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnular.ForeColor = Color.White;
            btnAnular.Location = new Point(683, 541);
            btnAnular.Name = "btnAnular";
            btnAnular.Size = new Size(180, 32);
            btnAnular.TabIndex = 3;
            btnAnular.Text = "Anular ausencia seleccionada";
            btnAnular.UseVisualStyleBackColor = false;
            //
            // FrmMisAusencias
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(241, 245, 249);
            ClientSize = new Size(875, 587);
            Controls.Add(btnAnular);
            Controls.Add(dgvAusencias);
            Controls.Add(pnlCarga);
            Controls.Add(pnlHeader);
            Name = "FrmMisAusencias";
            Text = "Mis Ausencias";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlCarga.ResumeLayout(false);
            pnlCarga.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAusencias).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblSubtituloHeader;
        private Label lblTituloHeader;
        private Panel pnlCarga;
        private Label lblAyuda;
        private Button btnRegistrar;
        private TextBox txtMotivo;
        private Label lblMotivo;
        private DateTimePicker dtpHasta;
        private Label lblHasta;
        private DateTimePicker dtpDesde;
        private Label lblDesde;
        private DataGridView dgvAusencias;
        private Button btnAnular;
    }
}
