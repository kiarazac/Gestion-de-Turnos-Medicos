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
    public partial class FrmPersonalAdmin : Form
    {
        // Contador simple para el id_usuario mientras no haya base de datos conectada.
        // Si esto ya lo trae la BD (autoincremental), sacá este contador y usá el id que devuelva el INSERT.
        private int contadorId = 1;

        public FrmPersonalAdmin()
        {
            InitializeComponent();
            this.Load += FrmPersonal_Load;
        }

        private void FrmPersonal_Load(object sender, EventArgs e)
        {
            ConfigurarDataGrid();
        }

        private void ConfigurarDataGrid()
        {
            dgvPersonal.Columns.Clear();
            dgvPersonal.AutoGenerateColumns = false;
            dgvPersonal.AllowUserToAddRows = false;

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "id_usuario",
                HeaderText = "id_usuario",
                ReadOnly = true,
                Width = 70
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "nombre",
                HeaderText = "nombre"
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "apellido",
                HeaderText = "apellido"
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "usuario",
                HeaderText = "usuario"
            });

            // La contraseña normalmente no se muestra en texto plano en un grid real,
            // pero si la necesitás visible por ahora, se agrega igual que las demás.
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "contrasenia",
                HeaderText = "contraseña"
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "dni",
                HeaderText = "DNI"
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "email",
                HeaderText = "Email"
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "direccion",
                HeaderText = "Dirección"
            });

            // *** Columna FECHA ***
            // La columna sigue siendo de tipo texto en el grid, pero el VALOR que le
            // cargás en cada fila es un DateTime real (no un string armado a mano).
            // Así se puede ordenar cronológicamente y no depende del formato regional.
            DataGridViewTextBoxColumn colFecha = new DataGridViewTextBoxColumn
            {
                Name = "fecha_nacimiento",
                HeaderText = "fecha nacimiento",
                ValueType = typeof(DateTime)
            };
            colFecha.DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPersonal.Columns.Add(colFecha);

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "telefono",
                HeaderText = "telefono"
            });

            // *** Columna SEXO ***
            // Se guarda como texto ("Hombre" / "Mujer"), no como bool ni como índice.
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "sexo",
                HeaderText = "sexo"
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "nro_matricula",
                HeaderText = "Nro Matricula"
            });
        }

        private void btnGuardar_Click(object sender, EventArgs e)
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
            fila.Cells["contrasenia"].Value = txtContrasenia.Text; // ver nota de seguridad más abajo
            fila.Cells["dni"].Value = txtDNI.Text.Trim();
            fila.Cells["email"].Value = txtEmail.Text.Trim();
            fila.Cells["direccion"].Value = txtDireccion.Text.Trim();
            fila.Cells["fecha_nacimiento"].Value = dtpFecha.Value.Date; // DateTime real, no string
            fila.Cells["telefono"].Value = txtTelefono.Text.Trim();
            fila.Cells["sexo"].Value = sexo;
            fila.Cells["nro_matricula"].Value = txtMatricula.Text.Trim();

            contadorId++;

            LimpiarCampos();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
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
            txtDireccion.Clear();
            txtTelefono.Clear();
            txtMatricula.Clear();
            dtpFecha.Value = DateTime.Now;
            rbHombre.Checked = false;
            rbMujer.Checked = false;
            txtNombre.Focus();
        }
    }
}