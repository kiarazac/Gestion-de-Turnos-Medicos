using System;
using System.Drawing;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario para que el profesional médico visualice y controle el estado operativo (abrir/cerrar)
    /// de los consultorios o salas que tiene asignadas.
    /// </summary>
    public partial class MisSalas_PM : Form
    {
        private readonly SalaBLL _salaBLL = new SalaBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        /// <summary>
        /// Constructor sin parámetros requerido para el diseñador de Windows Forms.
        /// </summary>
        public MisSalas_PM() : this(null)
        {
        }

        /// <summary>
        /// Inicializa el formulario asociándolo a la sesión del profesional médico autenticado.
        /// </summary>
        /// <param name="usuario">Contexto del médico logueado (<see cref="UsuarioLoginResult"/>).</param>
        public MisSalas_PM(UsuarioLoginResult? usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            this.Load += MisSalas_PM_Load;
            this.dgvMisSalas.SelectionChanged += DgvMisSalas_SelectionChanged;
            this.btnAbrirSala.Click += BtnAbrirSala_Click;
            this.btnCerrarSala.Click += BtnCerrarSala_Click;
        }

        /// <summary>
        /// Configura el comportamiento visual de la grilla e inicia la carga de datos.
        /// </summary>
        private void MisSalas_PM_Load(object? sender, EventArgs e)
        {
            dgvMisSalas.AutoGenerateColumns = false;
            dgvMisSalas.AllowUserToAddRows = false;
            dgvMisSalas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMisSalas.MultiSelect = false;

            CargarMisSalas();
        }

        /// <summary>
        /// Obtiene y proyecta en la grilla las salas asignadas al médico autenticado a través de <see cref="SalaBLL.ObtenerSalas"/>.
        /// </summary>
        private void CargarMisSalas()
        {
            dgvMisSalas.Rows.Clear();

            try
            {
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

        /// <summary>
        /// Actualiza los indicadores visuales y botones al cambiar la selección en la grilla de salas.
        /// </summary>
        private void DgvMisSalas_SelectionChanged(object? sender, EventArgs e)
        {
            ActualizarEstadoVisual();
        }

        /// <summary>
        /// Actualiza las etiquetas de color y habilitación de botones (Abrir / Cerrar) según el estado de la sala seleccionada.
        /// </summary>
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

        /// <summary>
        /// Ejecuta la apertura del consultorio seleccionado mediante <see cref="SalaBLL.AbrirSala"/>.
        /// </summary>
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

        /// <summary>
        /// Ejecuta el cierre del consultorio seleccionado mediante <see cref="SalaBLL.CerrarSala"/>.
        /// </summary>
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
