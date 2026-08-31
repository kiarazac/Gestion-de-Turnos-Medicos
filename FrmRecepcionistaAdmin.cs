using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmRecepcionistaAdmin : Form
    {
        private int contadorId = 1;

        public FrmRecepcionistaAdmin()
        {
            InitializeComponent();
            this.Load += FrmRecepcionistaAdmin_Load;
        }

        private void FrmRecepcionistaAdmin_Load(object sender, EventArgs e)
        {
            ConfigurarDataGrid();
        }

        private void ConfigurarDataGrid()
        {
            // Asumiendo que tenés un DataGridView llamado dgvPersonal en tu panel gris inferior
            dgvPersonal.Columns.Clear();
            dgvPersonal.AutoGenerateColumns = false;
            dgvPersonal.AllowUserToAddRows = false;

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "id_usuario", HeaderText = "ID", ReadOnly = true, Width = 50 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "nombre", HeaderText = "Nombre" });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "apellido", HeaderText = "Apellido" });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "usuario", HeaderText = "Usuario" });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "contrasenia", HeaderText = "Contraseña" });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "dni", HeaderText = "DNI" });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "email", HeaderText = "Email", Width = 150 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "telefono", HeaderText = "Teléfono" });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "sexo", HeaderText = "Sexo" });
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            string sexo = rbHombre.Checked ? "Hombre" : "Mujer";

            int filaIndex = dgvPersonal.Rows.Add();
            DataGridViewRow fila = dgvPersonal.Rows[filaIndex];

            fila.Cells["id_usuario"].Value = contadorId;
            fila.Cells["nombre"].Value = txtNombre.Text.Trim();
            fila.Cells["apellido"].Value = txtApellido.Text.Trim();
            fila.Cells["usuario"].Value = txtUsuario.Text.Trim();
            fila.Cells["contrasenia"].Value = txtContrasenia.Text;
            fila.Cells["dni"].Value = txtDNI.Text.Trim();
            fila.Cells["email"].Value = txtEmail.Text.Trim();
            fila.Cells["telefono"].Value = txtTelefono.Text.Trim();
            fila.Cells["sexo"].Value = sexo;

            contadorId++;

            LimpiarCampos();
        }

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            if (dgvPersonal.CurrentRow == null || dgvPersonal.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccioná una fila para eliminar.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmar = MessageBox.Show("¿Seguro que querés eliminar el registro seleccionado?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar == DialogResult.Yes)
            {
                dgvPersonal.Rows.RemoveAt(dgvPersonal.CurrentRow.Index);
            }
        }



        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContrasenia.Text) ||
                string.IsNullOrWhiteSpace(txtDNI.Text))
            {
                MessageBox.Show("Completá al menos Nombre, Apellido, Usuario, Contraseña y DNI.",
                    "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Regex.IsMatch(txtDNI.Text.Trim(), @"^\d{7,8}$"))
            {
                MessageBox.Show("El DNI tiene que tener entre 7 y 8 números, sin puntos.",
                    "DNI inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!rbHombre.Checked && !rbMujer.Checked)
            {
                MessageBox.Show("Seleccioná el sexo (Hombre o Mujer).",
                    "Falta un dato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) &&
                !Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("El email no tiene un formato válido.",
                    "Email inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtUsuario.Clear();
            txtContrasenia.Clear();
            txtEmail.Clear();
            txtDNI.Clear();
            txtTelefono.Clear();
            rbHombre.Checked = false;
            rbMujer.Checked = false;

            txtNombre.Focus();
        }

       
    }
}