using System;
using System.Drawing;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    public partial class MisSalas_PM : Form
    {
        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly SalaBLL _salaBLL = new SalaBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        public MisSalas_PM() : this(null)
        {
        }

        public MisSalas_PM(UsuarioLoginResult? usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

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
        /// Obtiene las salas asignadas al médico autenticado a través de la Capa de Negocio (BLL).
        /// </summary>
        private void CargarMisSalas()
        {
            dgvMisSalas.Rows.Clear();

            try
            {
                // BLL delega a SalaDAL y ejecuta sp_ObtenerSalas filtrando por IdUsuario
                var salas = _salaBLL.ObtenerSalas(_usuarioActual?.IdUsuario);

                if (salas != null)
                {
                    foreach (var s in salas)
                    {
                        int filaIndex = dgvMisSalas.Rows.Add();
                        DataGridViewRow fila = dgvMisSalas.Rows[filaIndex];

                        fila.Cells["ID"].Value = s.IdSala;
                        fila.Cells["NombreSala"].Value = s.NombreSala;
                        fila.Cells["descrip_atención"].Value = !string.IsNullOrWhiteSpace(s.DescripcionAtencion) ? s.DescripcionAtencion : "--";
                        fila.Cells["Estado"].Value = s.EstadoSala;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las salas asignadas:\n" + ex.Message,
                    "Error al consultar salas", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                if (estado.Equals("Disponible", StringComparison.OrdinalIgnoreCase) || estado.Equals("Libre", StringComparison.OrdinalIgnoreCase))
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
            if (dgvMisSalas.CurrentRow == null || dgvMisSalas.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccione una sala de la tabla para abrirla.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idSala = Convert.ToInt32(dgvMisSalas.CurrentRow.Cells["ID"].Value);
            string nombre = dgvMisSalas.CurrentRow.Cells["NombreSala"].Value?.ToString() ?? "Sala";
            int idUsuario = _usuarioActual?.IdUsuario ?? 0;

            try
            {
                // BLL delega a SalaDAL y ejecuta sp_AbrirSala (aplica validaciones de negocio en SQL Server)
                _salaBLL.AbrirSala(idSala, idUsuario);

                MessageBox.Show($"La sala '{nombre}' fue abierta correctamente y se encuentra disponible para atención.",
                    "Sala Abierta", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarMisSalas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir la sala:\n" + ex.Message, "Acción denegada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCerrarSala_Click(object? sender, EventArgs e)
        {
            if (dgvMisSalas.CurrentRow == null || dgvMisSalas.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccione una sala de la tabla para cerrarla.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idSala = Convert.ToInt32(dgvMisSalas.CurrentRow.Cells["ID"].Value);
            string nombre = dgvMisSalas.CurrentRow.Cells["NombreSala"].Value?.ToString() ?? "Sala";

            try
            {
                // BLL delega a SalaDAL y ejecuta sp_CerrarSala (bloquea si está en estado 'Ocupada')
                _salaBLL.CerrarSala(idSala);

                MessageBox.Show($"La sala '{nombre}' fue cerrada correctamente.",
                    "Sala Cerrada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarMisSalas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cerrar la sala:\n" + ex.Message, "Acción denegada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
