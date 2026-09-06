using System;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmGestionEspecialidades : Form
    {
        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly EspecialidadBLL _especialidadBLL = new EspecialidadBLL();

        public FrmGestionEspecialidades()
        {
            InitializeComponent();
            this.Load += FrmGestionEspecialidades_Load;
        }

        private void FrmGestionEspecialidades_Load(object? sender, EventArgs e)
        {
            ConfigurarDataGrid();
            CargarEspecialidadesDesdeBD();
        }

        private void ConfigurarDataGrid()
        {
            dgvEspecialidades.Columns.Clear();
            dgvEspecialidades.AutoGenerateColumns = false;
            dgvEspecialidades.AllowUserToAddRows = false;
            dgvEspecialidades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEspecialidades.MultiSelect = false;

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
        }

        /// <summary>
        /// Obtiene el catálogo de especialidades activas a través de la Capa de Negocio (BLL).
        /// </summary>
        private void CargarEspecialidadesDesdeBD()
        {
            dgvEspecialidades.Rows.Clear();

            try
            {
                // BLL delega en EspecialidadDAL -> sp_ListarEspecialidades
                var especialidades = _especialidadBLL.ObtenerEspecialidades();

                if (especialidades != null)
                {
                    foreach (var esp in especialidades)
                    {
                        int filaIndex = dgvEspecialidades.Rows.Add();
                        DataGridViewRow fila = dgvEspecialidades.Rows[filaIndex];

                        fila.Cells["IdEspecialidad"].Value = esp.IdEspecialidad;
                        fila.Cells["Nombre"].Value = esp.Nombre;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las especialidades desde la base de datos:\n" + ex.Message,
                    "Error al consultar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                LimpiarCampos();
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

        private void btnDesactivar_Click(object? sender, EventArgs e)
        {
            if (dgvEspecialidades.CurrentRow == null || dgvEspecialidades.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccioná una especialidad de la tabla para desactivarla.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombreEspecialidad = dgvEspecialidades.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "Especialidad";
            int idEspecialidad = Convert.ToInt32(dgvEspecialidades.CurrentRow.Cells["IdEspecialidad"].Value);

            var confirmar = MessageBox.Show($"¿Seguro que querés desactivar la especialidad '{nombreEspecialidad}'?",
                "Confirmar Desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                // BLL delega en EspecialidadDAL -> sp_EliminarEspecialidad (baja lógica)
                _especialidadBLL.EliminarEspecialidad(idEspecialidad);

                MessageBox.Show($"La especialidad '{nombreEspecialidad}' fue desactivada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarEspecialidadesDesdeBD();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo desactivar la especialidad:\n" + ex.Message,
                    "Error al desactivar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            txtNombre.Clear();
            txtNombre.Focus();
        }
    }
}
