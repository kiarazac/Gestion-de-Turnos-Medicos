namespace Gestion_de_Turnos_Medicos
{
    partial class FrmTurnosAReprogramar
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
            lblTitulo = new Label();
            lblInstrucciones = new Label();
            dgvTurnos = new DataGridView();
            btnCopiarMensaje = new Button();
            lblClave = new Label();
            txtClave = new TextBox();
            btnCancelarYReprogramar = new Button();
            btnSoloCancelar = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvTurnos).BeginInit();
            SuspendLayout();
            //
            // lblTitulo
            //
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(178, 34, 34);
            lblTitulo.Location = new Point(12, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(375, 25);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "⚠ Turnos sin profesional disponible";
            //
            // lblInstrucciones
            //
            lblInstrucciones.AutoSize = true;
            lblInstrucciones.Font = new Font("Segoe UI", 9F);
            lblInstrucciones.ForeColor = Color.FromArgb(71, 85, 105);
            lblInstrucciones.Location = new Point(14, 38);
            lblInstrucciones.Name = "lblInstrucciones";
            lblInstrucciones.Size = new Size(760, 15);
            lblInstrucciones.TabIndex = 1;
            lblInstrucciones.Text = "1) Copie el mensaje o llame al paciente   2) Pídale la clave de cancelación de su comprobante   3) Cancele y cargue el nuevo turno del paciente";
            //
            // dgvTurnos
            //
            dgvTurnos.AllowUserToAddRows = false;
            dgvTurnos.AllowUserToDeleteRows = false;
            dgvTurnos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvTurnos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTurnos.BackgroundColor = Color.FromArgb(248, 250, 252);
            dgvTurnos.Location = new Point(12, 64);
            dgvTurnos.MultiSelect = false;
            dgvTurnos.Name = "dgvTurnos";
            dgvTurnos.ReadOnly = true;
            dgvTurnos.RowHeadersVisible = false;
            dgvTurnos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTurnos.Size = new Size(1060, 330);
            dgvTurnos.TabIndex = 2;
            //
            // btnCopiarMensaje
            //
            btnCopiarMensaje.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCopiarMensaje.BackColor = Color.FromArgb(0, 70, 140);
            btnCopiarMensaje.Cursor = Cursors.Hand;
            btnCopiarMensaje.FlatStyle = FlatStyle.Flat;
            btnCopiarMensaje.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCopiarMensaje.ForeColor = Color.White;
            btnCopiarMensaje.Location = new Point(12, 408);
            btnCopiarMensaje.Name = "btnCopiarMensaje";
            btnCopiarMensaje.Size = new Size(190, 34);
            btnCopiarMensaje.TabIndex = 3;
            btnCopiarMensaje.Text = "📋 Copiar mensaje";
            btnCopiarMensaje.UseVisualStyleBackColor = false;
            //
            // lblClave
            //
            lblClave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblClave.AutoSize = true;
            lblClave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblClave.Location = new Point(438, 417);
            lblClave.Name = "lblClave";
            lblClave.Size = new Size(136, 15);
            lblClave.TabIndex = 4;
            lblClave.Text = "Clave de cancelación:";
            //
            // txtClave
            //
            txtClave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            txtClave.CharacterCasing = CharacterCasing.Upper;
            txtClave.Location = new Point(578, 414);
            txtClave.Name = "txtClave";
            txtClave.PlaceholderText = "CAN-XXXX";
            txtClave.Size = new Size(120, 23);
            txtClave.TabIndex = 5;
            //
            // btnCancelarYReprogramar
            //
            btnCancelarYReprogramar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancelarYReprogramar.BackColor = Color.FromArgb(15, 118, 110);
            btnCancelarYReprogramar.Cursor = Cursors.Hand;
            btnCancelarYReprogramar.FlatStyle = FlatStyle.Flat;
            btnCancelarYReprogramar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCancelarYReprogramar.ForeColor = Color.White;
            btnCancelarYReprogramar.Location = new Point(712, 408);
            btnCancelarYReprogramar.Name = "btnCancelarYReprogramar";
            btnCancelarYReprogramar.Size = new Size(210, 34);
            btnCancelarYReprogramar.TabIndex = 6;
            btnCancelarYReprogramar.Text = "Cancelar y reprogramar ➜";
            btnCancelarYReprogramar.UseVisualStyleBackColor = false;
            //
            // btnSoloCancelar
            //
            btnSoloCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSoloCancelar.BackColor = Color.FromArgb(127, 29, 29);
            btnSoloCancelar.Cursor = Cursors.Hand;
            btnSoloCancelar.FlatStyle = FlatStyle.Flat;
            btnSoloCancelar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSoloCancelar.ForeColor = Color.White;
            btnSoloCancelar.Location = new Point(932, 408);
            btnSoloCancelar.Name = "btnSoloCancelar";
            btnSoloCancelar.Size = new Size(140, 34);
            btnSoloCancelar.TabIndex = 7;
            btnSoloCancelar.Text = "Solo cancelar";
            btnSoloCancelar.UseVisualStyleBackColor = false;
            //
            // FrmTurnosAReprogramar
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1084, 454);
            Controls.Add(btnSoloCancelar);
            Controls.Add(btnCancelarYReprogramar);
            Controls.Add(txtClave);
            Controls.Add(lblClave);
            Controls.Add(btnCopiarMensaje);
            Controls.Add(dgvTurnos);
            Controls.Add(lblInstrucciones);
            Controls.Add(lblTitulo);
            MinimizeBox = false;
            Name = "FrmTurnosAReprogramar";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Turnos a reprogramar";
            ((System.ComponentModel.ISupportInitialize)dgvTurnos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitulo;
        private Label lblInstrucciones;
        private DataGridView dgvTurnos;
        private Button btnCopiarMensaje;
        private Label lblClave;
        private TextBox txtClave;
        private Button btnCancelarYReprogramar;
        private Button btnSoloCancelar;
    }
}
