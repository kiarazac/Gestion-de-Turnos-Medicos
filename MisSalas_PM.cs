using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Gestion_de_Turnos_Medicos
{
    public partial class MisSalas_PM : Form
    {
        public MisSalas_PM()
        {
            InitializeComponent();

            // Cableado manual de eventos en el constructor
            this.Load += MisSalas_PM_Load;
            this.dgvMisSalas.SelectionChanged += DgvMisSalas_SelectionChanged;
            this.btnAbrirSala.Click += BtnAbrirSala_Click;
            this.btnCerrarSala.Click += BtnCerrarSala_Click;
        }

        private void MisSalas_PM_Load(object? sender, EventArgs e)
        {
            dgvMisSalas.AutoGenerateColumns = false;
            dgvMisSalas.AllowUserToAddRows = false;
            dgvMisSalas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMisSalas.MultiSelect = false;

            CargarMisSalas();
        }

        /// <summary>
        /// Obtiene de SQL Server las salas asignadas al personal médico.
        /// </summary>
        private void CargarMisSalas()
        {
            dgvMisSalas.Rows.Clear();

            try
            {
                // Stored Procedure: sp_ListarMisSalas
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ListarMisSalas", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int filaIndex = dgvMisSalas.Rows.Add();
                                DataGridViewRow fila = dgvMisSalas.Rows[filaIndex];

                                fila.Cells["ID"].Value = reader["id_sala"];
                                fila.Cells["NombreSala"].Value = reader["nombreSala"];
                                fila.Cells["descrip_atención"].Value = reader["descripcion_atencion"] != DBNull.Value ? reader["descripcion_atencion"] : "--";
                                fila.Cells["Estado"].Value = reader["estadoSala"];
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al consultar salas asignadas:\n" + sqlEx.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al cargar salas:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ActualizarEstadoVisual();
        }

        private void DgvMisSalas_SelectionChanged(object? sender, EventArgs e)
        {
            ActualizarEstadoVisual();
        }

        private void ActualizarEstadoVisual()
        {
            if (dgvMisSalas.CurrentRow != null && dgvMisSalas.CurrentRow.Index >= 0)
            {
                string estado = dgvMisSalas.CurrentRow.Cells["Estado"].Value?.ToString() ?? "-----";
                LestadoSala.Text = estado;

                if (estado.Equals("Disponible", StringComparison.OrdinalIgnoreCase))
                {
                    LestadoSala.ForeColor = Color.ForestGreen;
                    btnAbrirSala.Enabled = false;
                    btnCerrarSala.Enabled = true;
                }
                else if (estado.Equals("Ocupada", StringComparison.OrdinalIgnoreCase))
                {
                    LestadoSala.ForeColor = Color.IndianRed;
                    btnAbrirSala.Enabled = false;
                    btnCerrarSala.Enabled = true;
                }
                else
                {
                    LestadoSala.ForeColor = Color.DarkOrange;
                    btnAbrirSala.Enabled = true;
                    btnCerrarSala.Enabled = false;
                }
            }
            else
            {
                LestadoSala.Text = "-----";
                LestadoSala.ForeColor = SystemColors.ButtonShadow;
                btnAbrirSala.Enabled = false;
                btnCerrarSala.Enabled = false;
            }
        }

        private void BtnAbrirSala_Click(object? sender, EventArgs e)
        {
            CambiarEstadoSala("Disponible");
        }

        private void BtnCerrarSala_Click(object? sender, EventArgs e)
        {
            CambiarEstadoSala("En Mantenimiento");
        }

        /// <summary>
        /// Modifica el estado de la sala seleccionada mediante Stored Procedure en SQL Server.
        /// </summary>
        private void CambiarEstadoSala(string nuevoEstado)
        {
            if (dgvMisSalas.CurrentRow == null || dgvMisSalas.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccione una sala de la tabla.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idSala = Convert.ToInt32(dgvMisSalas.CurrentRow.Cells["ID"].Value);
            string nombre = dgvMisSalas.CurrentRow.Cells["NombreSala"].Value?.ToString() ?? "Sala";

            try
            {
                // Stored Procedure: sp_ActualizarEstadoSala
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarEstadoSala", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IdSala", SqlDbType.Int).Value = idSala;
                        cmd.Parameters.Add("@NuevoEstado", SqlDbType.VarChar, 50).Value = nuevoEstado;

                        con.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show($"La sala '{nombre}' ahora se encuentra '{nuevoEstado}'.", "Estado Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                CargarMisSalas();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al actualizar estado en base de datos:\n" + sqlEx.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
