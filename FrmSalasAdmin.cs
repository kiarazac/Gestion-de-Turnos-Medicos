using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    // El nombre debe coincidir exactamente con el del Designer
    public partial class FrmSalasAdmin : Form
    {
        private int contadorIdSala = 1;

        public FrmSalasAdmin()
        {
            InitializeComponent();
            this.Load += FrmSalasAdmin_Load;
        }

        private void FrmSalasAdmin_Load(object sender, EventArgs e)
        {
            ConfigurarDataGrid();
            CargarEstados();
            CargarPersonalPrueba();
        }

        private void ConfigurarDataGrid()
        {
            dgvSalas.Columns.Clear();
            dgvSalas.AutoGenerateColumns = false;
            dgvSalas.AllowUserToAddRows = false;

            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "id_sala", HeaderText = "ID Sala", ReadOnly = true, Width = 60 });
            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "nombreSala", HeaderText = "Nombre de Sala", Width = 150 });
            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "estadoSala", HeaderText = "Estado", Width = 120 });

            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "personal_asignado", HeaderText = "Personal Asignado", Width = 250 });
        }

        private void CargarEstados()
        {
            cmbEstadoSala.Items.Clear();
            cmbEstadoSala.Items.Add("Disponible");
            cmbEstadoSala.Items.Add("Ocupada");
            cmbEstadoSala.Items.Add("En Mantenimiento");
            cmbEstadoSala.SelectedIndex = 0;
        }

        private void CargarPersonalPrueba()
        {
            clbPersonal.Items.Clear();
            clbPersonal.Items.Add("Dr. Pérez (Cardiología)");
            clbPersonal.Items.Add("Dra. Gómez (Pediatría)");
            clbPersonal.Items.Add("Enf. Martínez");
            clbPersonal.Items.Add("Dr. López (Traumatología)");
        }

        // Aquí están los métodos que Visual Studio no encontraba
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            List<string> listaPersonal = new List<string>();
            foreach (var item in clbPersonal.CheckedItems)
            {
                listaPersonal.Add(item.ToString());
            }
            string personalConcatenado = string.Join(", ", listaPersonal);

            int filaIndex = dgvSalas.Rows.Add();
            DataGridViewRow fila = dgvSalas.Rows[filaIndex];

            fila.Cells["id_sala"].Value = contadorIdSala;
            fila.Cells["nombreSala"].Value = txtNombreSala.Text.Trim();
            fila.Cells["estadoSala"].Value = cmbEstadoSala.SelectedItem.ToString();
            fila.Cells["personal_asignado"].Value = personalConcatenado;

            contadorIdSala++;

            LimpiarCampos();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvSalas.CurrentRow == null || dgvSalas.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccioná una sala de la lista para eliminarla.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmar = MessageBox.Show("¿Seguro que querés eliminar la sala seleccionada?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar == DialogResult.Yes)
            {
                dgvSalas.Rows.RemoveAt(dgvSalas.CurrentRow.Index);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombreSala.Text))
            {
                MessageBox.Show("Ingresá el nombre de la sala.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbEstadoSala.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccioná el estado de la sala.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (clbPersonal.CheckedItems.Count == 0)
            {
                MessageBox.Show("Tenés que asignar al menos a una persona a la sala.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            txtNombreSala.Clear();
            cmbEstadoSala.SelectedIndex = 0;

            for (int i = 0; i < clbPersonal.Items.Count; i++)
            {
                clbPersonal.SetItemChecked(i, false);
            }

            txtNombreSala.Focus();
        }
    }
}