using System;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de supervisión y monitoreo de turnos emitidos (emergencias y especialidades).
    /// Muestra estadísticas de pacientes clasificados por niveles de urgencia (Alta, Media, Baja).
    /// </summary>
    public partial class FrmListaTurnos : Form
    {
        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly EspecialidadBLL _especialidadBLL = new EspecialidadBLL();

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
        /// Obtiene los turnos de guardia del día mediante TurnoBLL (delegando a sp_ListarTurnosEmergencia)
        /// y actualiza los indicadores de prioridad.
        /// </summary>
        private void CargarTurnosEmergencia()
        {
            dataGridView1.Rows.Clear();
            int cantAlta = 0;
            int cantMedia = 0;
            int cantBaja = 0;

            try
            {
                // 1. Invocamos a la Capa BLL -> TurnoDAL -> sp_ListarTurnosEmergencia
                var turnosEmergencia = _turnoBLL.ListarTurnosEmergencia();

                if (turnosEmergencia != null)
                {
                    // 2. Iteramos los turnos de guardia mapeando cada celda con fallbacks defensivos
                    foreach (var t in turnosEmergencia)
                    {
                        string turno = !string.IsNullOrWhiteSpace(t.Turno) 
                            ? t.Turno 
                            : (!string.IsNullOrWhiteSpace(t.NroOrden) ? t.NroOrden : $"E-{t.IdTurno:D3}");

                        string prioridad = !string.IsNullOrWhiteSpace(t.Prioridad) 
                            ? t.Prioridad 
                            : (!string.IsNullOrWhiteSpace(t.Triage) ? t.Triage : "MEDIA");

                        string hora = !string.IsNullOrWhiteSpace(t.Hora) ? t.Hora : "--:--";
                        string estado = !string.IsNullOrWhiteSpace(t.Estado) ? t.Estado : "En Espera";
                        string sala = !string.IsNullOrWhiteSpace(t.Sala) ? t.Sala : "--";

                        // 3. Agregamos la fila a la grilla de recepción
                        dataGridView1.Rows.Add(turno, prioridad, hora, estado, sala);

                        // 4. Acumulamos el contador según el nivel de urgencia
                        if (prioridad.Equals("ALTA", StringComparison.OrdinalIgnoreCase))
                            cantAlta++;
                        else if (prioridad.Equals("MEDIA", StringComparison.OrdinalIgnoreCase))
                            cantMedia++;
                        else if (prioridad.Equals("BAJA", StringComparison.OrdinalIgnoreCase))
                            cantBaja++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los turnos de guardia:\n" + ex.Message,
                    "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Actualizar tarjetas de conteo de prioridades
            LtotalAlta.Text = cantAlta.ToString();
            LtotalMedia.Text = cantMedia.ToString();
            LtotalBaja.Text = cantBaja.ToString();
        }

        /// <summary>
        /// Obtiene las especialidades médicas de consultorio desde la Capa de Negocio (BLL).
        /// Excluye explícitamente "Emergencia" para que las atenciones de guardia solo se muestren
        /// en el sector izquierdo y el selector de la derecha contenga únicamente especialidades clínicas.
        /// </summary>
        private void CargarEspecialidades()
        {
            cmbEspecialidades.Items.Clear();
            cmbEspecialidades.Items.Add("Seleccione una especialidad...");

            try
            {
                // 1. Invocamos a la Capa BLL -> EspecialidadDAL -> sp_ObtenerEspecialidades
                var especialidades = _especialidadBLL.ObtenerEspecialidades();

                if (especialidades != null)
                {
                    foreach (var esp in especialidades)
                    {
                        if (!string.IsNullOrWhiteSpace(esp.Nombre))
                        {
                            // 2. Filtro de exclusión: Las emergencias no son especialidades de consultorio programado.
                            // Se gestionan por triage y se visualizan exclusivamente en el sector izquierdo (Guardia/Emergencia).
                            if (esp.Nombre.StartsWith("Emergenc", StringComparison.OrdinalIgnoreCase))
                                continue;

                            cmbEspecialidades.Items.Add(esp.Nombre);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las especialidades:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                // BLL delega a TurnoDAL -> sp_ListarTurnosEspecialidad
                var turnosEsp = _turnoBLL.ListarTurnosEspecialidad(especialidadSeleccionada);

                if (turnosEsp != null)
                {
                    foreach (var t in turnosEsp)
                    {
                        string turno = !string.IsNullOrWhiteSpace(t.NroOrden) ? t.NroOrden : t.IdTurno.ToString();
                        string hora = t.Fecha.ToString("HH:mm");
                        string fecha = t.Fecha.ToString("dd/MM/yyyy");
                        string estado = !string.IsNullOrWhiteSpace(t.Estado) ? t.Estado : "--";
                        string sala = "Consultorio";

                        dgvEspecialidades.Rows.Add(turno, hora, fecha, estado, sala);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron consultar los turnos de la especialidad:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
