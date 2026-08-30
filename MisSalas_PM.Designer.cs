namespace Gestion_de_Turnos_Medicos
{
    partial class MisSalas_PM
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
            panel1 = new Panel();
            label1 = new Label();
            groupBox1 = new GroupBox();
            dgvMisSalas = new DataGridView();
            ID = new DataGridViewTextBoxColumn();
            NombreSala = new DataGridViewTextBoxColumn();
            descrip_atención = new DataGridViewTextBoxColumn();
            Estado = new DataGridViewTextBoxColumn();
            btnAbrirSala = new Button();
            btnCerrarSala = new Button();
            label2 = new Label();
            LestadoSala = new Label();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMisSalas).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Highlight;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(label1);
            panel1.Location = new Point(-157, -2);
            panel1.Name = "panel1";
            panel1.Size = new Size(1115, 65);
            panel1.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Britannic Bold", 24.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(217, 9);
            label1.Name = "label1";
            label1.Size = new Size(345, 37);
            label1.TabIndex = 0;
            label1.Text = "Gestión de Mi/s Sala/s";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(LestadoSala);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btnCerrarSala);
            groupBox1.Controls.Add(btnAbrirSala);
            groupBox1.Controls.Add(dgvMisSalas);
            groupBox1.Location = new Point(39, 92);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(750, 450);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Mis Salas Asignadas";
            // 
            // dgvMisSalas
            // 
            dgvMisSalas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMisSalas.Columns.AddRange(new DataGridViewColumn[] { ID, NombreSala, descrip_atención, Estado });
            dgvMisSalas.Location = new Point(25, 34);
            dgvMisSalas.Name = "dgvMisSalas";
            dgvMisSalas.Size = new Size(443, 390);
            dgvMisSalas.TabIndex = 0;
            // 
            // ID
            // 
            ID.HeaderText = "ID";
            ID.Name = "ID";
            // 
            // NombreSala
            // 
            NombreSala.HeaderText = "Nombre de Sala";
            NombreSala.Name = "NombreSala";
            // 
            // descrip_atención
            // 
            descrip_atención.HeaderText = "Descripción";
            descrip_atención.Name = "descrip_atención";
            // 
            // Estado
            // 
            Estado.HeaderText = "Estado";
            Estado.Name = "Estado";
            // 
            // btnAbrirSala
            // 
            btnAbrirSala.BackColor = Color.LightGreen;
            btnAbrirSala.Enabled = false;
            btnAbrirSala.Location = new Point(525, 132);
            btnAbrirSala.Name = "btnAbrirSala";
            btnAbrirSala.Size = new Size(176, 64);
            btnAbrirSala.TabIndex = 1;
            btnAbrirSala.Text = "ABRIR SALA";
            btnAbrirSala.UseVisualStyleBackColor = false;
            // 
            // btnCerrarSala
            // 
            btnCerrarSala.BackColor = Color.Tomato;
            btnCerrarSala.Enabled = false;
            btnCerrarSala.Location = new Point(525, 221);
            btnCerrarSala.Name = "btnCerrarSala";
            btnCerrarSala.Size = new Size(176, 64);
            btnCerrarSala.TabIndex = 2;
            btnCerrarSala.Text = "CERRAR SALA";
            btnCerrarSala.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label2.Location = new Point(525, 34);
            label2.Name = "label2";
            label2.Size = new Size(165, 25);
            label2.TabIndex = 3;
            label2.Text = "ESTADO DE SALA";
            // 
            // LestadoSala
            // 
            LestadoSala.AutoSize = true;
            LestadoSala.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            LestadoSala.ForeColor = SystemColors.ButtonShadow;
            LestadoSala.Location = new Point(578, 77);
            LestadoSala.Name = "LestadoSala";
            LestadoSala.Size = new Size(52, 25);
            LestadoSala.TabIndex = 4;
            LestadoSala.Text = "-----";
            // 
            // MisSalas_PM
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(949, 574);
            Controls.Add(groupBox1);
            Controls.Add(panel1);
            Name = "MisSalas_PM";
            Text = "MisSalas_PM";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMisSalas).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private GroupBox groupBox1;
        private DataGridView dgvMisSalas;
        private DataGridViewTextBoxColumn ID;
        private DataGridViewTextBoxColumn NombreSala;
        private DataGridViewTextBoxColumn descrip_atención;
        private DataGridViewTextBoxColumn Estado;
        private Button btnCerrarSala;
        private Button btnAbrirSala;
        private Label LestadoSala;
        private Label label2;
    }
}