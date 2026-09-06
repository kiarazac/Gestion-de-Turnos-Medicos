using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmSalasAdmin : Form
    {
        public FrmSalasAdmin()
        {
            InitializeComponent();
            this.Load += FrmSalasAdmin_Load;
        }

        private void FrmSalasAdmin_Load(object sender, EventArgs e)
        {
            ConfigurarDataGrid();
            CargarEstados();
            CargarPersonalMedicoDesdeBD();
            CargarSalasDesdeBD();
        }

        private void ConfigurarDataGrid()
        {
            dgvSalas.Columns.Clear();
            dgvSalas.AutoGenerateColumns = false;
            dgvSalas.AllowUserToAddRows = false;
            dgvSalas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSalas.MultiSelect = false;

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

        /// <summary>
        /// Carga el personal médico activo desde SQL Server para poblar el CheckedListBox.
        /// </summary>
        private void CargarPersonalMedicoDesdeBD()
        {
            clbPersonal.Items.Clear();

            try
            {
                // Stored Procedure: sp_ListarPersonalMedico
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ListarPersonalMedico", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string medico = reader["NombreCompleto"]?.ToString() ?? string.Empty;
                                if (!string.IsNullOrWhiteSpace(medico))
                                {
                                    clbPersonal.Items.Add(medico);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Valores de fallback si la BD está en proceso de carga inicial
                clbPersonal.Items.Add("Dr. Pérez (Cardiología)");
                clbPersonal.Items.Add("Dra. Gómez (Pediatría)");
                clbPersonal.Items.Add("Enf. Martínez");
                clbPersonal.Items.Add("Dr. López (Traumatología)");
            }
        }

        /// <summary>
        /// Carga todas las salas registradas y activas en el DataGridView.
        /// </summary>
        private void CargarSalasDesdeBD()
        {
            dgvSalas.Rows.Clear();

            try
            {
                // Stored Procedure: sp_ListarSalas
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ListarSalas", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int filaIndex = dgvSalas.Rows.Add();
                                DataGridViewRow fila = dgvSalas.Rows[filaIndex];

                                fila.Cells["id_sala"].Value = reader["id_sala"];
                                fila.Cells["nombreSala"].Value = reader["nombreSala"];
                                fila.Cells["estadoSala"].Value = reader["estadoSala"];
                                fila.Cells["personal_asignado"].Value = reader["personal_asignado"] != DBNull.Value ? reader["personal_asignado"] : "";
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al cargar las salas desde la base de datos:\n" + sqlEx.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar salas:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            string nombreSala = txtNombreSala.Text.Trim();
            string estadoSala = cmbEstadoSala.SelectedItem!.ToString()!;

            List<string> listaPersonal = new List<string>();
            foreach (var item in clbPersonal.CheckedItems)
            {
                listaPersonal.Add(item.ToString()!);
            }
            string personalConcatenado = string.Join(", ", listaPersonal);

            try
            {
                // Stored Procedure: sp_GuardarSala
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GuardarSala", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreSala", SqlDbType.VarChar, 100).Value = nombreSala;
                        cmd.Parameters.Add("@EstadoSala", SqlDbType.VarChar, 50).Value = estadoSala;
                        cmd.Parameters.Add("@PersonalAsignado", SqlDbType.VarChar, 255).Value = personalConcatenado;

                        SqlParameter paramIdSala = new SqlParameter("@IdSala", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(paramIdSala);

                        con.Open();
                        cmd.ExecuteNonQuery();

                        int nuevoId = Convert.ToInt32(paramIdSala.Value);

                        MessageBox.Show($"Sala '{nombreSala}' registrada correctamente con ID #{nuevoId}.", "Sala Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                CargarSalasDesdeBD();
                LimpiarCampos();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al guardar la sala en la base de datos:\n" + sqlEx.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado:\n" + ex.Message, "Error Inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvSalas.CurrentRow == null || dgvSalas.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccioná una sala de la lista para desactivarla.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmar = MessageBox.Show("¿Seguro que querés desactivar la sala seleccionada?", "Confirmar Desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            int idSala = Convert.ToInt32(dgvSalas.CurrentRow.Cells["id_sala"].Value);

            try
            {
                // Stored Procedure: sp_DesactivarSala
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DesactivarSala", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IdSala", SqlDbType.Int).Value = idSala;

                        con.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Sala desactivada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                CargarSalasDesdeBD();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al desactivar la sala en la base de datos:\n" + sqlEx.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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