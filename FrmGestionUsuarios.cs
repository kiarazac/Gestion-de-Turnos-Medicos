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
    public partial class FrmGestionUsuarios : Form
    {
        private const string ROL_PERSONAL_MEDICO = "Personal Médico";

        // Contador simple para el id_usuario mientras no haya base de datos conectada.
        // Si esto ya lo trae la BD (autoincremental), sacá este contador y usá el id que devuelva el INSERT.
        private int contadorId = 1;

        public FrmGestionUsuarios()
        {
            InitializeComponent();
            this.Load += FrmPersonal_Load;
        }

        private void FrmPersonal_Load(object sender, EventArgs e)
        {
            ConfigurarDataGrid();

            // Arranca en "Inactivo": dispara cmbRol_SelectedIndexChanged,
            // que oculta la sección médica y muestra el cartel informativo.
            cmbRol.SelectedIndex = 0;
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
                Name = "telefono",
                HeaderText = "telefono"
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "sexo",
                HeaderText = "sexo"
            });

            // *** Rol elegido por el administrador ***
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "rol",
                HeaderText = "Rol",
                Width = 110
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "nro_matricula",
                HeaderText = "Nro Matricula"
            });

            // *** Columnas exclusivas de Personal Médico (quedan vacías si el rol no aplica) ***
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "especialidades",
                HeaderText = "Especialidades",
                Width = 150
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "sala",
                HeaderText = "Sala",
                Width = 90
            });
        }

        // Se dispara al elegir un rol distinto y también una vez al cargar el form
        // (porque FrmPersonal_Load fuerza cmbRol.SelectedIndex = 0).
        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarSeccionMedica();
        }

        // Muestra/oculta el panel "Datos Médicos" según el rol elegido y,
        // cuando el rol deja de ser Personal Médico, limpia esos campos
        // para que no quede información médica "pegada" a otro rol.
        private void ActualizarSeccionMedica()
        {
            bool esPersonalMedico = EsPersonalMedico();

            pnlDatosMedicos.Visible = esPersonalMedico;
            lblInfoMedico.Visible = !esPersonalMedico;

            if (!esPersonalMedico)
            {
                for (int i = 0; i < clbEspecialidades.Items.Count; i++)
                    clbEspecialidades.SetItemChecked(i, false);

                for (int i = 0; i < clbSala.Items.Count; i++)
                    clbSala.SetItemChecked(i, false);

                txtMatricula.Clear();
            }
        }

        private bool EsPersonalMedico()
        {
            return cmbRol.SelectedItem != null &&
                   cmbRol.SelectedItem.ToString() == ROL_PERSONAL_MEDICO;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            string sexo = rbHombre.Checked ? "Hombre" : "Mujer";
            string rol = cmbRol.SelectedItem.ToString();
            bool esPersonalMedico = EsPersonalMedico();

            // Los atributos médicos solo se completan si el rol es Personal Médico;
            // en cualquier otro caso quedan vacíos (equivalente al "null" que pedías
            // cuando esto se conecte a una base de datos: ahí esas columnas irían
            // como DBNull.Value en vez de string.Empty).
            string textoEspecialidades = string.Empty;
            string textoSala = string.Empty;
            string matricula = string.Empty;

            if (esPersonalMedico)
            {
                List<string> listaEspecialidades = new List<string>();
                foreach (var item in clbEspecialidades.CheckedItems)
                    listaEspecialidades.Add(item.ToString());
                textoEspecialidades = string.Join(", ", listaEspecialidades);

                List<string> listaSala = new List<string>();
                foreach (var item in clbSala.CheckedItems)
                    listaSala.Add(item.ToString());
                textoSala = string.Join(", ", listaSala);

                matricula = txtMatricula.Text.Trim();
            }

            int filaIndex = dgvPersonal.Rows.Add();
            DataGridViewRow fila = dgvPersonal.Rows[filaIndex];

            fila.Cells["id_usuario"].Value = contadorId;
            fila.Cells["nombre"].Value = txtNombre.Text.Trim();
            fila.Cells["apellido"].Value = txtApellido.Text.Trim();
            fila.Cells["usuario"].Value = txtUsuario.Text.Trim();
            fila.Cells["contrasenia"].Value = txtContrasenia.Text; // ver nota de seguridad al final
            fila.Cells["dni"].Value = txtDNI.Text.Trim();
            fila.Cells["email"].Value = txtEmail.Text.Trim();
            fila.Cells["telefono"].Value = txtTelefono.Text.Trim();
            fila.Cells["sexo"].Value = sexo;
            fila.Cells["rol"].Value = rol;
            fila.Cells["nro_matricula"].Value = matricula;
            fila.Cells["especialidades"].Value = textoEspecialidades;
            fila.Cells["sala"].Value = textoSala;

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

            var confirmar = MessageBox.Show("¿Seguro que querés eliminar el usuario seleccionado?",
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

            // El combo arranca en "Inactivo" (índice 0): no es un rol válido para
            // dar de alta a un usuario, así que obligamos a elegir uno real.
            if (cmbRol.SelectedIndex <= 0)
            {
                MessageBox.Show("Seleccioná un rol para el usuario (Recepcionista, Personal Médico o Administrador).",
                    "Falta un dato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // La especialidad solo es obligatoria cuando el rol es Personal Médico.
            if (EsPersonalMedico() && clbEspecialidades.CheckedItems.Count == 0)
            {
                MessageBox.Show("Seleccioná al menos una especialidad.",
                    "Falta un dato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // Vuelve a "Inactivo": dispara cmbRol_SelectedIndexChanged, que ya se
            // encarga de ocultar el panel médico y limpiar sus campos.
            cmbRol.SelectedIndex = 0;

            txtNombre.Focus();
        }
    }
}