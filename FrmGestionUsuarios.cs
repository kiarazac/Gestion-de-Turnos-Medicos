using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmGestionUsuarios : Form
    {
        private const string ROL_PERSONAL_MEDICO = "Personal Médico";

        public FrmGestionUsuarios()
        {
            InitializeComponent();
            this.Load += FrmPersonal_Load;
        }

        private void FrmPersonal_Load(object sender, EventArgs e)
        {
            ConfigurarDataGrid();

            // Cargar usuarios existentes desde la base de datos
            CargarUsuariosDesdeBD();

            // Arranca en "Inactivo": dispara cmbRol_SelectedIndexChanged,
            // que oculta la sección médica y muestra el cartel informativo.
            cmbRol.SelectedIndex = 0;
        }

        private void ConfigurarDataGrid()
        {
            dgvPersonal.Columns.Clear();
            dgvPersonal.AutoGenerateColumns = false;
            dgvPersonal.AllowUserToAddRows = false;
            dgvPersonal.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPersonal.MultiSelect = false;

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "id_usuario",
                HeaderText = "ID",
                DataPropertyName = "id_usuario",
                ReadOnly = true,
                Width = 60
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "nombre",
                HeaderText = "Nombre",
                DataPropertyName = "nombre",
                Width = 100
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "apellido",
                HeaderText = "Apellido",
                DataPropertyName = "apellido",
                Width = 100
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "usuario",
                HeaderText = "Usuario",
                DataPropertyName = "usuario",
                Width = 100
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "contrasenia",
                HeaderText = "Contraseña",
                DataPropertyName = "contrasenia",
                Visible = false // Por seguridad no mostrar contraseña en texto plano
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "dni",
                HeaderText = "DNI",
                DataPropertyName = "dni",
                Width = 90
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "email",
                HeaderText = "Email",
                DataPropertyName = "email",
                Width = 150
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "telefono",
                HeaderText = "Teléfono",
                DataPropertyName = "telefono",
                Width = 100
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "sexo",
                HeaderText = "Sexo",
                DataPropertyName = "sexo",
                Width = 80
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "rol",
                HeaderText = "Rol",
                DataPropertyName = "rol",
                Width = 120
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "nro_matricula",
                HeaderText = "Matrícula",
                DataPropertyName = "nro_matricula",
                Width = 90
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "especialidades",
                HeaderText = "Especialidades",
                DataPropertyName = "especialidades",
                Width = 150
            });

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "sala",
                HeaderText = "Sala",
                DataPropertyName = "sala",
                Width = 90
            });
        }

        /// <summary>
        /// Obtiene la lista de usuarios activos desde la base de datos SQL Server.
        /// </summary>
        private void CargarUsuariosDesdeBD()
        {
            dgvPersonal.Rows.Clear();

            try
            {
                // Stored Procedure: sp_ListarUsuarios
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ListarUsuarios", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int filaIndex = dgvPersonal.Rows.Add();
                                DataGridViewRow fila = dgvPersonal.Rows[filaIndex];

                                fila.Cells["id_usuario"].Value = reader["id_usuario"];
                                fila.Cells["nombre"].Value = reader["nombre"];
                                fila.Cells["apellido"].Value = reader["apellido"];
                                fila.Cells["usuario"].Value = reader["usuario"];
                                fila.Cells["contrasenia"].Value = reader["contrasenia"];
                                fila.Cells["dni"].Value = reader["dni"];
                                fila.Cells["email"].Value = reader["email"];
                                fila.Cells["telefono"].Value = reader["telefono"];
                                fila.Cells["sexo"].Value = reader["sexo"];
                                fila.Cells["rol"].Value = reader["rol"];
                                fila.Cells["nro_matricula"].Value = reader["nro_matricula"] != DBNull.Value ? reader["nro_matricula"] : "";
                                fila.Cells["especialidades"].Value = reader["especialidades"] != DBNull.Value ? reader["especialidades"] : "";
                                fila.Cells["sala"].Value = reader["sala"] != DBNull.Value ? reader["sala"] : "";
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al cargar la lista de usuarios desde la base de datos:\n" + sqlEx.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar usuarios:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarSeccionMedica();
        }

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

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string usuario = txtUsuario.Text.Trim();
            string contrasenia = txtContrasenia.Text;
            string dni = txtDNI.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string sexo = rbHombre.Checked ? "Hombre" : "Mujer";
            string rol = cmbRol.SelectedItem!.ToString()!;
            bool esPersonalMedico = EsPersonalMedico();

            string textoEspecialidades = string.Empty;
            string textoSala = string.Empty;
            string matricula = string.Empty;

            if (esPersonalMedico)
            {
                List<string> listaEspecialidades = new List<string>();
                foreach (var item in clbEspecialidades.CheckedItems)
                    listaEspecialidades.Add(item.ToString()!);
                textoEspecialidades = string.Join(", ", listaEspecialidades);

                List<string> listaSala = new List<string>();
                foreach (var item in clbSala.CheckedItems)
                    listaSala.Add(item.ToString()!);
                textoSala = string.Join(", ", listaSala);

                matricula = txtMatricula.Text.Trim();
            }

            try
            {
                // Stored Procedure: sp_GuardarUsuario
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GuardarUsuario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = nombre;
                        cmd.Parameters.Add("@Apellido", SqlDbType.VarChar, 100).Value = apellido;
                        cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 100).Value = usuario;
                        cmd.Parameters.Add("@Contrasenia", SqlDbType.VarChar, 100).Value = contrasenia;
                        cmd.Parameters.Add("@Dni", SqlDbType.VarChar, 20).Value = dni;
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = string.IsNullOrEmpty(email) ? (object)DBNull.Value : email;
                        cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 50).Value = string.IsNullOrEmpty(telefono) ? (object)DBNull.Value : telefono;
                        cmd.Parameters.Add("@Sexo", SqlDbType.VarChar, 20).Value = sexo;
                        cmd.Parameters.Add("@Rol", SqlDbType.VarChar, 50).Value = rol;
                        cmd.Parameters.Add("@NroMatricula", SqlDbType.VarChar, 50).Value = string.IsNullOrEmpty(matricula) ? (object)DBNull.Value : matricula;
                        cmd.Parameters.Add("@Especialidades", SqlDbType.VarChar, 255).Value = string.IsNullOrEmpty(textoEspecialidades) ? (object)DBNull.Value : textoEspecialidades;
                        cmd.Parameters.Add("@Sala", SqlDbType.VarChar, 100).Value = string.IsNullOrEmpty(textoSala) ? (object)DBNull.Value : textoSala;

                        SqlParameter paramIdUsuario = new SqlParameter("@IdUsuario", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(paramIdUsuario);

                        con.Open();
                        cmd.ExecuteNonQuery();

                        int nuevoId = Convert.ToInt32(paramIdUsuario.Value);

                        MessageBox.Show($"Usuario registrado correctamente con ID #{nuevoId}.", "Usuario Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                // Refrescar listado desde la base de datos
                CargarUsuariosDesdeBD();
                LimpiarCampos();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al guardar el usuario en la base de datos:\n" + sqlEx.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado:\n" + ex.Message, "Error Inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvPersonal.CurrentRow == null || dgvPersonal.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccioná una fila para desactivar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmar = MessageBox.Show("¿Seguro que querés desactivar el usuario seleccionado?", "Confirmar Desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            int idUsuario = Convert.ToInt32(dgvPersonal.CurrentRow.Cells["id_usuario"].Value);

            try
            {
                // Stored Procedure: sp_DesactivarUsuario
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DesactivarUsuario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;

                        con.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Usuario desactivado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                CargarUsuariosDesdeBD();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al desactivar usuario en la base de datos:\n" + sqlEx.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Completá al menos Nombre, Apellido, Usuario, Contraseña y DNI.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Regex.IsMatch(txtDNI.Text.Trim(), @"^\d{7,8}$"))
            {
                MessageBox.Show("El DNI tiene que tener entre 7 y 8 números, sin puntos.", "DNI inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!rbHombre.Checked && !rbMujer.Checked)
            {
                MessageBox.Show("Seleccioná el sexo (Hombre o Mujer).", "Falta un dato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) &&
                !Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("El email no tiene un formato válido.", "Email inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbRol.SelectedIndex <= 0)
            {
                MessageBox.Show("Seleccioná un rol para el usuario (Recepcionista, Personal Médico o Administrador).", "Falta un dato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (EsPersonalMedico() && clbEspecialidades.CheckedItems.Count == 0)
            {
                MessageBox.Show("Seleccioná al menos una especialidad.", "Falta un dato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            cmbRol.SelectedIndex = 0;
            txtNombre.Focus();
        }
    }
}