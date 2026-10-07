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
    /// Formulario de administración de obras sociales y medicina prepaga.
    /// Permite dar de alta nuevas coberturas, modificar sus datos, ejecutar bajas lógicas y reactivarlas.
    /// </summary>
    public partial class FrmGestionObrasSociales : Form
    {
        // Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly ObraSocialBLL _obraSocialBLL = new ObraSocialBLL();

        // Variables de estado y caché
        private int _idObraSocialSeleccionada = 0;
        private List<ObraSocialDTO> _obrasSocialesCache = new List<ObraSocialDTO>();
        private bool _cargandoDatos = false;

        public FrmGestionObrasSociales()
        {
            InitializeComponent();
            this.Load += FrmGestionObrasSociales_Load;
        }

        private void FrmGestionObrasSociales_Load(object? sender, EventArgs e)
        {
            ConfigurarDataGrid();
            dgvObrasSociales.SelectionChanged += DgvObrasSociales_SelectionChanged;
            dgvObrasSociales.CellClick += DgvObrasSociales_CellClick;
            CargarObrasSocialesDesdeBD();
        }

        private void ConfigurarDataGrid()
        {
            dgvObrasSociales.Columns.Clear();
            dgvObrasSociales.AutoGenerateColumns = false;
            dgvObrasSociales.AllowUserToAddRows = false;
            dgvObrasSociales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvObrasSociales.MultiSelect = false;
            dgvObrasSociales.RowHeadersVisible = false;
            dgvObrasSociales.EnableHeadersVisualStyles = false;

            // Encabezados con estilo médico moderno
            dgvObrasSociales.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvObrasSociales.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvObrasSociales.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvObrasSociales.ColumnHeadersHeight = 36;

            // Celdas y alternancia
            dgvObrasSociales.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvObrasSociales.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 251, 241);
            dgvObrasSociales.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvObrasSociales.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvObrasSociales.RowTemplate.Height = 28;

            dgvObrasSociales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "IdObraSocial",
                HeaderText = "ID",
                DataPropertyName = "IdObraSocial",
                ReadOnly = true,
                Width = 70
            });

            dgvObrasSociales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nombre",
                HeaderText = "Nombre de Obra Social / Prepaga",
                DataPropertyName = "Nombre",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvObrasSociales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Sigla",
                HeaderText = "Sigla / Acrónimo",
                DataPropertyName = "Sigla",
                ReadOnly = true,
                Width = 140
            });

            dgvObrasSociales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                HeaderText = "Estado",
                ReadOnly = true,
                Width = 120
            });
        }

        /// <summary>
        /// Obtiene el catálogo de coberturas médicas desde la Capa de Negocio (BLL).
        /// Admite listar coberturas inactivas según el estado de chkMostrarInactivas.
        /// </summary>
        private void CargarObrasSocialesDesdeBD()
        {
            _cargandoDatos = true;
            dgvObrasSociales.Rows.Clear();

            try
            {
                bool incluirInactivas = chkMostrarInactivas.Checked;
                var coberturas = _obraSocialBLL.ObtenerObrasSociales(incluirInactivas);
                _obrasSocialesCache = coberturas ?? new List<ObraSocialDTO>();

                if (_obrasSocialesCache.Count > 0)
                {
                    foreach (var os in _obrasSocialesCache)
                    {
                        int filaIndex = dgvObrasSociales.Rows.Add();
                        DataGridViewRow fila = dgvObrasSociales.Rows[filaIndex];

                        fila.Cells["IdObraSocial"].Value = os.IdObraSocial;
                        fila.Cells["Nombre"].Value = os.Nombre;
                        fila.Cells["Sigla"].Value = string.IsNullOrWhiteSpace(os.Sigla) ? "-" : os.Sigla;
                        fila.Cells["Estado"].Value = os.Activo ? "🟢 Activa" : "🔴 Inactiva";

                        // Estilo visual tenue para coberturas inactivas
                        if (!os.Activo)
                        {
                            fila.DefaultCellStyle.ForeColor = Color.FromArgb(120, 113, 108);
                            fila.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las obras sociales desde la base de datos:\n" + ex.Message,
                    "Error al consultar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoDatos = false;
                LimpiarCampos();
            }
        }

        private void DgvObrasSociales_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                CargarSeleccionDesdeFila(dgvObrasSociales.Rows[e.RowIndex]);
            }
        }

        private void DgvObrasSociales_SelectionChanged(object? sender, EventArgs e)
        {
            if (_cargandoDatos)
                return;

            if (dgvObrasSociales.CurrentRow == null || dgvObrasSociales.CurrentRow.Index < 0)
                return;

            CargarSeleccionDesdeFila(dgvObrasSociales.CurrentRow);
        }

        private void CargarSeleccionDesdeFila(DataGridViewRow fila)
        {
            if (fila?.Cells["IdObraSocial"].Value == null)
                return;

            _idObraSocialSeleccionada = Convert.ToInt32(fila.Cells["IdObraSocial"].Value);
            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString() ?? string.Empty;
            string siglaVal = fila.Cells["Sigla"].Value?.ToString() ?? string.Empty;
            txtSigla.Text = siglaVal == "-" ? string.Empty : siglaVal;

            var osObj = _obrasSocialesCache.FirstOrDefault(o => o.IdObraSocial == _idObraSocialSeleccionada);
            bool esInactiva = osObj != null && !osObj.Activo;

            if (_idObraSocialSeleccionada == 1)
            {
                // Cobertura particular: no se puede dar de baja
                btnReactivar.Enabled = false;
                btnDesactivar.Enabled = false;
                btnModificar.Enabled = true;
                btnGuardar.Enabled = false;
            }
            else if (esInactiva)
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
            CargarObrasSocialesDesdeBD();
        }

        private bool ValidarCampos()
        {
            string nombre = txtNombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre de la obra social es obligatorio y no puede quedar vacío.",
                    "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (nombre.Length > 100)
            {
                MessageBox.Show("El nombre de la obra social no puede superar los 100 caracteres.",
                    "Longitud excedida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            string sigla = txtSigla.Text.Trim();
            if (sigla.Length > 20)
            {
                MessageBox.Show("La sigla o acrónimo no puede superar los 20 caracteres.",
                    "Longitud excedida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSigla.Focus();
                return false;
            }

            return true;
        }

        private void btnGuardar_Click(object? sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            string nombre = txtNombre.Text.Trim();
            string? sigla = string.IsNullOrWhiteSpace(txtSigla.Text) ? null : txtSigla.Text.Trim();

            try
            {
                _obraSocialBLL.RegistrarObraSocial(nombre, sigla);

                MessageBox.Show($"Obra social '{nombre}' registrada correctamente.",
                    "Cobertura Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarObrasSocialesDesdeBD();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al registrar la obra social:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object? sender, EventArgs e)
        {
            if (_idObraSocialSeleccionada <= 0)
            {
                MessageBox.Show("Seleccione una obra social de la tabla para modificar.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos())
                return;

            string nuevoNombre = txtNombre.Text.Trim();
            string? nuevaSigla = string.IsNullOrWhiteSpace(txtSigla.Text) ? null : txtSigla.Text.Trim();

            var confirmar = MessageBox.Show($"¿Desea guardar los cambios en la obra social seleccionada con el nombre '{nuevoNombre}'?",
                "Confirmar Modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                _obraSocialBLL.ModificarObraSocial(_idObraSocialSeleccionada, nuevoNombre, nuevaSigla);

                MessageBox.Show($"Obra social modificada correctamente a '{nuevoNombre}'.",
                    "Modificación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarObrasSocialesDesdeBD();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al modificar la obra social:\n" + ex.Message,
                    "Error al modificar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDesactivar_Click(object? sender, EventArgs e)
        {
            if (_idObraSocialSeleccionada <= 0)
            {
                MessageBox.Show("Seleccione una obra social de la tabla para desactivarla.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_idObraSocialSeleccionada == 1)
            {
                MessageBox.Show("No es posible dar de baja la cobertura Particular requerida por el sistema.",
                    "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombreObraSocial = txtNombre.Text.Trim();
            if (string.IsNullOrEmpty(nombreObraSocial) && dgvObrasSociales.CurrentRow != null)
            {
                nombreObraSocial = dgvObrasSociales.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "Obra Social";
            }

            var confirmar = MessageBox.Show($"¿Seguro que desea dar de baja lógica la cobertura médica '{nombreObraSocial}'?",
                "Confirmar Desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                _obraSocialBLL.EliminarObraSocial(_idObraSocialSeleccionada);

                MessageBox.Show($"La obra social '{nombreObraSocial}' fue desactivada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarObrasSocialesDesdeBD();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo desactivar la obra social:\n" + ex.Message,
                    "Error al desactivar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReactivar_Click(object? sender, EventArgs e)
        {
            if (_idObraSocialSeleccionada <= 0)
            {
                MessageBox.Show("Seleccione una obra social inactiva para re-dar de alta.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombreObraSocial = txtNombre.Text.Trim();
            if (string.IsNullOrEmpty(nombreObraSocial) && dgvObrasSociales.CurrentRow != null)
            {
                nombreObraSocial = dgvObrasSociales.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "Obra Social";
            }

            var confirmar = MessageBox.Show($"¿Desea reactivar y re-dar de alta la obra social '{nombreObraSocial}'?",
                "Confirmar Reactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                _obraSocialBLL.ReactivarObraSocial(_idObraSocialSeleccionada);

                MessageBox.Show($"La obra social '{nombreObraSocial}' fue reactivada correctamente y ya se encuentra disponible para emitir turnos.",
                    "Reactivación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarObrasSocialesDesdeBD();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo reactivar la obra social:\n" + ex.Message,
                    "Error al reactivar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object? sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            _idObraSocialSeleccionada = 0;
            txtNombre.Clear();
            txtSigla.Clear();

            btnGuardar.Enabled = true;
            btnModificar.Enabled = false;
            btnDesactivar.Enabled = false;
            btnReactivar.Enabled = false;

            dgvObrasSociales.ClearSelection();
        }
    }
}
