using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmTurnoEspecialidad : Form
    {
        public FrmTurnoEspecialidad()
        {
            InitializeComponent();

            // Cableado manual de eventos en el constructor
            this.button1.Click += BtnGenerarTurno_Click;
        }

        private void FrmTurnoEspecialidad_Load(object sender, EventArgs e)
        {
            // 1. Configurar calendario
            calFechaTurno.MinDate = DateTime.Today;

            // 2. Cargar las especialidades desde la base de datos
            CargarEspecialidades();

            // 3. Inicializar panel visual de turno generado
            Lid_turno.Text = "# --";
            Ldescrip_turno_especialidad.Text = "Especialidad";
        }

        /// <summary>
        /// Carga las especialidades disponibles desde SQL Server mediante Stored Procedure.
        /// </summary>
        private void CargarEspecialidades()
        {
            cmbEspecialidad.Items.Clear();
            cmbEspecialidad.Items.Add("Seleccione especialidad...");

            try
            {
                // Stored Procedure: sp_ObtenerEspecialidades
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
                                string nombreEsp = reader["Nombre"]?.ToString() ?? string.Empty;
                                if (!string.IsNullOrWhiteSpace(nombreEsp))
                                {
                                    cmbEspecialidad.Items.Add(nombreEsp);
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException)
            {
                // Si la BD aún no cuenta con datos cargados, proveer las opciones iniciales
                if (cmbEspecialidad.Items.Count <= 1)
                {
                    cmbEspecialidad.Items.Add("Cardiología");
                    cmbEspecialidad.Items.Add("Pediatría");
                    cmbEspecialidad.Items.Add("Traumatología");
                }
            }
            catch (Exception)
            {
                if (cmbEspecialidad.Items.Count <= 1)
                {
                    cmbEspecialidad.Items.Add("Cardiología");
                    cmbEspecialidad.Items.Add("Pediatría");
                    cmbEspecialidad.Items.Add("Traumatología");
                }
            }

            cmbEspecialidad.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            calFechaTurno.RemoveAllBoldedDates();
            cmbHorarios.Items.Clear();

            if (cmbEspecialidad.SelectedIndex <= 0)
                return;

            DateTime hoy = DateTime.Today;
            // Días predeterminados de atención
            calFechaTurno.BoldedDates = new DateTime[] { hoy.AddDays(1), hoy.AddDays(2), hoy.AddDays(3), hoy.AddDays(4), hoy.AddDays(5) };
            calFechaTurno.UpdateBoldedDates();
        }

        private void calFechaTurno_DateChanged(object sender, DateRangeEventArgs e)
        {
            cmbHorarios.Items.Clear();

            if (cmbEspecialidad.SelectedIndex <= 0)
            {
                MessageBox.Show("Por favor, seleccione una especialidad antes de elegir la fecha.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string especialidad = cmbEspecialidad.SelectedItem?.ToString() ?? string.Empty;
            DateTime fechaElegida = e.Start.Date;

            // Consultar horarios disponibles en la base de datos
            try
            {
                // Stored Procedure: sp_ObtenerHorariosDisponibles
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ObtenerHorariosDisponibles", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreEspecialidad", SqlDbType.VarChar, 100).Value = especialidad;
                        cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fechaElegida;

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string hora = reader["Horario"]?.ToString() ?? string.Empty;
                                if (!string.IsNullOrWhiteSpace(hora))
                                    cmbHorarios.Items.Add(hora);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Opciones de contingencia si el SP aún no está creado en la base
                cmbHorarios.Items.AddRange(new object[] { "08:30", "09:00", "09:30", "10:00", "10:30", "11:00", "14:00", "14:30", "15:00", "16:00" });
            }

            if (cmbHorarios.Items.Count > 0)
            {
                cmbHorarios.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No hay turnos disponibles para esta fecha y especialidad.", "Sin disponibilidad", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Evento del botón GENERAR TURNO (button1).
        /// Registra el paciente (reutilizando sp_GuardarPaciente) y crea el turno de especialidad con sp_CrearTurnoEspecialidad.
        /// </summary>
        private void BtnGenerarTurno_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos())
                return;

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string dni = txtDNI.Text.Trim();
            string obraSocial = txtObraSocial.Text.Trim();
            string especialidad = cmbEspecialidad.SelectedItem!.ToString()!;
            DateTime fecha = calFechaTurno.SelectionStart.Date;
            string horario = cmbHorarios.SelectedItem!.ToString()!;

            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    con.Open();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            // -------------------------------------------------------------
                            // Stored Procedure: sp_GuardarPaciente (Reutilizado)
                            // -------------------------------------------------------------
                            int idPaciente;
                            using (SqlCommand cmdPaciente = new SqlCommand("sp_GuardarPaciente", con, tran))
                            {
                                cmdPaciente.CommandType = CommandType.StoredProcedure;
                                cmdPaciente.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = nombre;
                                cmdPaciente.Parameters.Add("@Apellido", SqlDbType.VarChar, 100).Value = apellido;
                                cmdPaciente.Parameters.Add("@Dni", SqlDbType.VarChar, 20).Value = dni;
                                cmdPaciente.Parameters.Add("@ObraSocial", SqlDbType.VarChar, 100).Value = obraSocial;

                                SqlParameter paramIdPaciente = new SqlParameter("@IdPaciente", SqlDbType.Int)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                cmdPaciente.Parameters.Add(paramIdPaciente);

                                cmdPaciente.ExecuteNonQuery();
                                idPaciente = Convert.ToInt32(paramIdPaciente.Value);
                            }

                            // -------------------------------------------------------------
                            // Stored Procedure: sp_CrearTurnoEspecialidad
                            // -------------------------------------------------------------
                            int idTurno;
                            string nroOrden;
                            using (SqlCommand cmdTurno = new SqlCommand("sp_CrearTurnoEspecialidad", con, tran))
                            {
                                cmdTurno.CommandType = CommandType.StoredProcedure;
                                cmdTurno.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = idPaciente;
                                cmdTurno.Parameters.Add("@NombreEspecialidad", SqlDbType.VarChar, 100).Value = especialidad;
                                cmdTurno.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha;
                                cmdTurno.Parameters.Add("@Horario", SqlDbType.VarChar, 10).Value = horario;
                                cmdTurno.Parameters.Add("@Estado", SqlDbType.VarChar, 50).Value = "En Espera";

                                SqlParameter paramIdTurno = new SqlParameter("@IdTurno", SqlDbType.Int)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                SqlParameter paramNroOrden = new SqlParameter("@NroOrden", SqlDbType.VarChar, 20)
                                {
                                    Direction = ParameterDirection.Output
                                };

                                cmdTurno.Parameters.Add(paramIdTurno);
                                cmdTurno.Parameters.Add(paramNroOrden);

                                cmdTurno.ExecuteNonQuery();

                                idTurno = Convert.ToInt32(paramIdTurno.Value);
                                nroOrden = paramNroOrden.Value?.ToString() ?? $"T-{idTurno:D3}";
                            }

                            tran.Commit();

                            // Actualizar UI
                            Lid_turno.Text = $"# {nroOrden}";
                            Ldescrip_turno_especialidad.Text = especialidad;

                            MessageBox.Show(
                                $"¡Turno programado con éxito!\n\n" +
                                $"Paciente: {apellido}, {nombre}\n" +
                                $"Especialidad: {especialidad}\n" +
                                $"Fecha: {fecha:dd/MM/yyyy} a las {horario} hs\n" +
                                $"N° Turno: {nroOrden}",
                                "Turno Generado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            LimpiarCampos();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Error al conectar o registrar turno en la base de datos:\n{sqlEx.Message}", "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error inesperado al generar el turno:\n{ex.Message}", "Error Inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtDNI.Text) ||
                string.IsNullOrWhiteSpace(txtObraSocial.Text))
            {
                MessageBox.Show("Por favor, complete todos los datos del paciente.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Validaciones.EsNombreValido(txtNombre.Text.Trim()))
            {
                MessageBox.Show("El Nombre contiene caracteres inválidos. Solo se admiten letras.", "Nombre inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (!Validaciones.EsNombreValido(txtApellido.Text.Trim()))
            {
                MessageBox.Show("El Apellido contiene caracteres inválidos. Solo se admiten letras.", "Apellido inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtDNI.Text.Trim(), @"^\d{7,8}$"))
            {
                MessageBox.Show("El DNI debe tener entre 7 y 8 números.", "DNI inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDNI.Focus();
                return false;
            }

            if (cmbEspecialidad.SelectedIndex <= 0)
            {
                MessageBox.Show("Por favor, seleccione una especialidad.", "Falta especialidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbEspecialidad.Focus();
                return false;
            }

            if (cmbHorarios.SelectedItem == null || string.IsNullOrWhiteSpace(cmbHorarios.SelectedItem.ToString()))
            {
                MessageBox.Show("Por favor, seleccione un horario para el turno.", "Falta horario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbHorarios.Focus();
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtDNI.Clear();
            txtObraSocial.Clear();
            cmbEspecialidad.SelectedIndex = 0;
            cmbHorarios.Items.Clear();
            txtNombre.Focus();
        }
    }
}
