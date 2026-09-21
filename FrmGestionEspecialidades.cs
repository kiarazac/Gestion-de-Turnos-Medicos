using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de administración de especialidades médicas.
    /// Permite dar de alta nuevas especialidades, editar sus nombres, efectuar bajas lógicas y reactivarlas.
    /// </summary>
    public partial class FrmGestionEspecialidades : Form
    {
        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly EspecialidadBLL _especialidadBLL = new EspecialidadBLL();

        // 2. Variables de estado y caché
        private int _idEspecialidadSeleccionada = 0;
        private List<EspecialidadDTO> _especialidadesCache = new List<EspecialidadDTO>();
        private bool _cargandoDatos = false;

        public FrmGestionEspecialidades()
        {
            InitializeComponent();
            this.Load += FrmGestionEspecialidades_Load;
        }

        private void FrmGestionEspecialidades_Load(object? sender, EventArgs e)
        {
            ConfigurarDataGrid();
            dgvEspecialidades.SelectionChanged += DgvEspecialidades_SelectionChanged;
            dgvEspecialidades.CellClick += DgvEspecialidades_CellClick;
            CargarEspecialidadesDesdeBD();
        }

        private void ConfigurarDataGrid()
        {
            dgvEspecialidades.Columns.Clear();
            dgvEspecialidades.AutoGenerateColumns = false;
            dgvEspecialidades.AllowUserToAddRows = false;
            dgvEspecialidades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEspecialidades.MultiSelect = false;
            dgvEspecialidades.RowHeadersVisible = false;
            dgvEspecialidades.EnableHeadersVisualStyles = false;

            // Encabezados con estilo médico moderno
            dgvEspecialidades.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvEspecialidades.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEspecialidades.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvEspecialidades.ColumnHeadersHeight = 36;

            // Celdas y alternancia
            dgvEspecialidades.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvEspecialidades.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 251, 241);
            dgvEspecialidades.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvEspecialidades.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvEspecialidades.RowTemplate.Height = 28;

            dgvEspecialidades.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "IdEspecialidad",
                HeaderText = "ID",
                DataPropertyName = "IdEspecialidad",
                ReadOnly = true,
                Width = 80
            });

            dgvEspecialidades.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nombre",
                HeaderText = "Nombre de Especialidad",
                DataPropertyName = "Nombre",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvEspecialidades.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                HeaderText = "Estado",
                ReadOnly = true,
                Width = 120
            });
        }

        /// <summary>
        /// Obtiene el catálogo de especialidades desde la Capa de Negocio (BLL).
        /// Admite listar inactivas según el estado de chkMostrarInactivas.
        /// </summary>
        private void CargarEspecialidadesDesdeBD()
        {
            _cargandoDatos = true;
            dgvEspecialidades.Rows.Clear();

            try
            {
                bool incluirInactivas = chkMostrarInactivas.Checked;
                // BLL delega en EspecialidadDAL -> sp_ListarEspecialidades
                var especialidades = _especialidadBLL.ObtenerEspecialidades(incluirInactivas);
                _especialidadesCache = especialidades ?? new List<EspecialidadDTO>();

                if (_especialidadesCache.Count > 0)
                {
                    foreach (var esp in _especialidadesCache)
                    {
                        int filaIndex = dgvEspecialidades.Rows.Add();
                        DataGridViewRow fila = dgvEspecialidades.Rows[filaIndex];

                        fila.Cells["IdEspecialidad"].Value = esp.IdEspecialidad;
                        fila.Cells["Nombre"].Value = esp.Nombre;
                        fila.Cells["Estado"].Value = esp.Activo ? "🟢 Activa" : "🔴 Inactiva";

                        // Estilo visual tenue para especialidades inactivas
                        if (!esp.Activo)
                        {
                            fila.DefaultCellStyle.ForeColor = Color.FromArgb(120, 113, 108);
                            fila.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las especialidades desde la base de datos:\n" + ex.Message,
                    "Error al consultar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoDatos = false;
                LimpiarCampos();
            }
        }

        private void DgvEspecialidades_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                CargarSeleccionDesdeFila(dgvEspecialidades.Rows[e.RowIndex]);
            }
        }

        private void DgvEspecialidades_SelectionChanged(object? sender, EventArgs e)
        {
            if (_cargandoDatos)
                return;

            if (dgvEspecialidades.CurrentRow == null || dgvEspecialidades.CurrentRow.Index < 0)
            {
                return;
            }

            CargarSeleccionDesdeFila(dgvEspecialidades.CurrentRow);
        }

        private void CargarSeleccionDesdeFila(DataGridViewRow fila)
        {
            if (fila?.Cells["IdEspecialidad"].Value == null)
                return;

            _idEspecialidadSeleccionada = Convert.ToInt32(fila.Cells["IdEspecialidad"].Value);
            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString() ?? string.Empty;

            var espObj = _especialidadesCache.FirstOrDefault(e => e.IdEspecialidad == _idEspecialidadSeleccionada);
            bool esInactiva = espObj != null && !espObj.Activo;

            if (esInactiva)
            {
                btnReactivar.Enabled = true;
                btnDesactivar.Enabled = false;
                btnModificar.Enabled = true;
                btnGuardar.Enabled = false;
            }
            else
            {
                btnReactivar.Enabled = false;
                btnDesactivar.Enabled = true;
                btnModificar.Enabled = true;
                btnGuardar.Enabled = false;
            }
        }

        private void chkMostrarInactivas_CheckedChanged(object? sender, EventArgs e)
        {
            CargarEspecialidadesDesdeBD();
        }

        private void btnGuardar_Click(object? sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            string nombre = txtNombre.Text.Trim();

            try
            {
                // BLL delega en EspecialidadDAL -> sp_InsertarEspecialidad
                _especialidadBLL.RegistrarEspecialidad(nombre);

                MessageBox.Show($"Especialidad '{nombre}' registrada correctamente.",
                    "Especialidad Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarEspecialidadesDesdeBD();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al registrar la especialidad:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object? sender, EventArgs e)
        {
            if (_idEspecialidadSeleccionada <= 0)
            {
                MessageBox.Show("Seleccioná una especialidad de la tabla para modificar.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos())
                return;

            string nuevoNombre = txtNombre.Text.Trim();
            var confirmar = MessageBox.Show($"¿Desea guardar los cambios en la especialidad seleccionada con el nombre '{nuevoNombre}'?",
                "Confirmar Modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                // BLL delega en EspecialidadDAL -> sp_ModificarEspecialidad
                _especialidadBLL.ModificarEspecialidad(_idEspecialidadSeleccionada, nuevoNombre);

                MessageBox.Show($"Especialidad modificada correctamente a '{nuevoNombre}'.",
                    "Modificación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarEspecialidadesDesdeBD();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al modificar la especialidad:\n" + ex.Message,
                    "Error al modificar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDesactivar_Click(object? sender, EventArgs e)
        {
            if (_idEspecialidadSeleccionada <= 0)
            {
                MessageBox.Show("Seleccioná una especialidad de la tabla para desactivarla.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombreEspecialidad = txtNombre.Text.Trim();
            if (string.IsNullOrEmpty(nombreEspecialidad) && dgvEspecialidades.CurrentRow != null)
            {
                nombreEspecialidad = dgvEspecialidades.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "Especialidad";
            }

            var confirmar = MessageBox.Show($"¿Seguro que querés desactivar la especialidad '{nombreEspecialidad}'?",
                "Confirmar Desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                // BLL delega en EspecialidadDAL -> sp_EliminarEspecialidad (baja lógica)
                _especialidadBLL.EliminarEspecialidad(_idEspecialidadSeleccionada);

                MessageBox.Show($"La especialidad '{nombreEspecialidad}' fue desactivada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarEspecialidadesDesdeBD();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo desactivar la especialidad:\n" + ex.Message,
                    "Error al desactivar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReactivar_Click(object? sender, EventArgs e)
        {
            if (_idEspecialidadSeleccionada <= 0)
            {
                MessageBox.Show("Seleccioná una especialidad inactiva para re-dar de alta.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombreEspecialidad = txtNombre.Text.Trim();
            if (string.IsNullOrEmpty(nombreEspecialidad) && dgvEspecialidades.CurrentRow != null)
            {
                nombreEspecialidad = dgvEspecialidades.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "Especialidad";
            }

            var confirmar = MessageBox.Show($"¿Desea re-dar de alta y reactivar la especialidad '{nombreEspecialidad}' en el sistema?",
                "Confirmar Reactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                // BLL delega en EspecialidadDAL -> sp_ReactivarEspecialidad
                _especialidadBLL.ReactivarEspecialidad(_idEspecialidadSeleccionada);

                MessageBox.Show($"La especialidad '{nombreEspecialidad}' fue reactivada exitosamente.",
                    "Reactivación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarEspecialidadesDesdeBD();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo reactivar la especialidad:\n" + ex.Message,
                    "Error al reactivar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object? sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private bool ValidarCampos()
        {
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingresá el nombre de la especialidad.",
                    "Falta dato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (nombre.Length > 100)
            {
                MessageBox.Show("El nombre de la especialidad no puede superar los 100 caracteres.",
                    "Nombre demasiado largo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (!Validaciones.EsNombreValido(nombre))
            {
                MessageBox.Show("El nombre de la especialidad solo debe contener letras y espacios.",
                    "Nombre inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            _idEspecialidadSeleccionada = 0;
            txtNombre.Clear();
            if (dgvEspecialidades.Rows.Count > 0)
                dgvEspecialidades.ClearSelection();

            btnGuardar.Enabled = true;
            btnModificar.Enabled = false;
            btnDesactivar.Enabled = false;
            btnReactivar.Enabled = false;
            txtNombre.Focus();
        }
    }
}
