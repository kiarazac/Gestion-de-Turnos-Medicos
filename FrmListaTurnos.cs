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
    public partial class FrmListaTurnos : Form
    {
        public FrmListaTurnos()
        {
            InitializeComponent();
        }

        private void FrmListaTurnos_Load(object sender, EventArgs e)
        {
            CargarTurnosEmergencia();
            CargarEspecialidades();
        }

        /// <summary>
        /// Obtiene de SQL Server la lista de turnos de emergencia del día y actualiza los contadores de prioridad.
        /// </summary>
        private void CargarTurnosEmergencia()
        {
            dataGridView1.Rows.Clear();
            int cantAlta = 0;
            int cantMedia = 0;
            int cantBaja = 0;

            try
            {
                // Stored Procedure: sp_ListarTurnosEmergencia
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ListarTurnosEmergencia", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string turno = reader["Turno"]?.ToString() ?? "--";
                                string prioridad = reader["Prioridad"]?.ToString() ?? "--";
                                string hora = reader["Hora"]?.ToString() ?? "--";
                                string estado = reader["Estado"]?.ToString() ?? "--";
                                string sala = reader["Sala"]?.ToString() ?? "--";

                                dataGridView1.Rows.Add(turno, prioridad, hora, estado, sala);

                                if (prioridad.Equals("ALTA", StringComparison.OrdinalIgnoreCase))
                                    cantAlta++;
                                else if (prioridad.Equals("MEDIA", StringComparison.OrdinalIgnoreCase))
                                    cantMedia++;
                                else if (prioridad.Equals("BAJA", StringComparison.OrdinalIgnoreCase))
                                    cantBaja++;
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al consultar turnos de emergencia:\n" + sqlEx.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al cargar turnos de emergencia:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Actualizar tarjetas de conteo de prioridades
            LtotalAlta.Text = cantAlta.ToString();
            LtotalMedia.Text = cantMedia.ToString();
            LtotalBaja.Text = cantBaja.ToString();
        }

        /// <summary>
        /// Carga el combo de especialidades desde SQL Server reutilizando sp_ObtenerEspecialidades.
        /// </summary>
        private void CargarEspecialidades()
        {
            cmbEspecialidades.Items.Clear();
            cmbEspecialidades.Items.Add("Seleccione una especialidad...");

            try
            {
                // Stored Procedure: sp_ObtenerEspecialidades (Reutilizado)
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ObtenerEspecialidades", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string esp = reader["Nombre"]?.ToString() ?? string.Empty;
                                if (!string.IsNullOrWhiteSpace(esp))
                                    cmbEspecialidades.Items.Add(esp);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (cmbEspecialidades.Items.Count <= 1)
                {
                    cmbEspecialidades.Items.Add("Cardiología");
                    cmbEspecialidades.Items.Add("Pediatría");
                    cmbEspecialidades.Items.Add("Traumatología");
                }
            }

            cmbEspecialidades.SelectedIndex = 0;
        }

        private void cmbEspecialidades_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvEspecialidades.Rows.Clear();

            if (cmbEspecialidades.SelectedIndex <= 0)
                return;

            string especialidadSeleccionada = cmbEspecialidades.SelectedItem?.ToString() ?? string.Empty;

            try
            {
                // Stored Procedure: sp_ListarTurnosEspecialidad
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ListarTurnosEspecialidad", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreEspecialidad", SqlDbType.VarChar, 100).Value = especialidadSeleccionada;

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string turno = reader["Turno"]?.ToString() ?? "--";
                                string hora = reader["Hora"]?.ToString() ?? "--";
                                string fecha = reader["Fecha"]?.ToString() ?? "--";
                                string estado = reader["Estado"]?.ToString() ?? "--";
                                string sala = reader["Sala"]?.ToString() ?? "--";

                                dgvEspecialidades.Rows.Add(turno, hora, fecha, estado, sala);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al consultar turnos de la especialidad:\n" + sqlEx.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
