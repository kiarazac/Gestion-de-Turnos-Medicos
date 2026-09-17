using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmSalasAdmin : Form
    {
        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly SalaBLL _salaBLL = new SalaBLL();
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();

        // 2. Variables de estado para edición y cache de datos
        private int _idSalaSeleccionada = 0;
        private List<SalaDTO> _salasCache = new List<SalaDTO>();
        private bool _cargandoDatos = false;

        // Estructura interna para relacionar nombre del médico con su IdUsuario
        private sealed class ItemMedico
        {
            public int IdUsuario { get; set; }
            public string NombreCompleto { get; set; } = string.Empty;
            public override string ToString() => NombreCompleto;
        }

        public FrmSalasAdmin()
        {
            InitializeComponent();
            this.Load += FrmSalasAdmin_Load;
            this.dgvSalas.SelectionChanged += DgvSalas_SelectionChanged;
        }

        private void FrmSalasAdmin_Load(object? sender, EventArgs e)
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
            dgvSalas.RowHeadersVisible = false;
            dgvSalas.EnableHeadersVisualStyles = false;

            // Encabezados con estilo médico moderno
            dgvSalas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvSalas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSalas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvSalas.ColumnHeadersHeight = 36;

            // Celdas y alternancia
            dgvSalas.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvSalas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 251, 241);
            dgvSalas.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvSalas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvSalas.RowTemplate.Height = 28;

            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "id_sala", HeaderText = "ID Sala", ReadOnly = true, Width = 80 });
            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "nombreSala", HeaderText = "Nombre de Sala", Width = 180 });
            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "estadoSala", HeaderText = "Estado", Width = 140 });
            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "personal_asignado", HeaderText = "Personal Asignado", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
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
        /// Obtiene el personal médico activo a través de UsuarioBLL (eliminando datos simulados).
        /// </summary>
        private void CargarPersonalMedicoDesdeBD()
        {
            clbPersonal.Items.Clear();

            try
            {
                // BLL delega a UsuarioDAL -> sp_ListarPersonalMedico
                var medicos = _usuarioBLL.ObtenerMedicos();

                if (medicos != null)
                {
                    foreach (var m in medicos)
                    {
                        clbPersonal.Items.Add(new ItemMedico
                        {
                            IdUsuario = m.IdUsuario,
                            NombreCompleto = m.NombreCompleto
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar la lista de profesionales médicos:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Carga todas las salas registradas y activas desde la Capa de Negocio (BLL).
        /// Agrupa las asignaciones de profesionales para mostrar una única fila por sala física en la grilla.
        /// </summary>
        private void CargarSalasDesdeBD()
        {
            _cargandoDatos = true;
            dgvSalas.Rows.Clear();

            try
            {
                // 1. Invocamos a la Capa de Negocio (SalaBLL) -> SalaDAL -> sp_ObtenerSalas
                var salas = _salaBLL.ObtenerSalas(null);
                _salasCache = salas ?? new List<SalaDTO>();

                if (_salasCache.Count > 0)
                {
                    // 2. Agrupamos por IdSala para que las salas con múltiples médicos no aparezcan duplicadas
                    var salasAgrupadas = _salasCache.GroupBy(s => s.IdSala);

                    foreach (var grupo in salasAgrupadas)
                    {
                        var primera = grupo.First();
                        int filaIndex = dgvSalas.Rows.Add();
                        DataGridViewRow fila = dgvSalas.Rows[filaIndex];

                        fila.Cells["id_sala"].Value = primera.IdSala;
                        fila.Cells["nombreSala"].Value = primera.NombreSala;
                        fila.Cells["estadoSala"].Value = primera.EstadoSala;

                        // 3. Concatenamos los nombres de los profesionales médicos asignados
                        var medicos = grupo
                            .Where(g => !string.IsNullOrWhiteSpace(g.ApellidoMedico))
                            .Select(g => $"{g.ApellidoMedico}, {g.NombreMedico}")
                            .Distinct()
                            .ToList();

                        fila.Cells["personal_asignado"].Value = medicos.Count > 0
                            ? string.Join("; ", medicos)
                            : "Sin asignar";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron consultar las salas desde la base de datos:\n" + ex.Message,
                    "Error al cargar salas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoDatos = false;
                dgvSalas.ClearSelection();
            }
        }

        /// <summary>
        /// Evento disparado cuando el usuario hace clic o navega en la grilla de salas.
        /// Carga automáticamente los datos de la sala seleccionada en los campos de edición superiores
        /// para permitir su modificación inmediata.
        /// </summary>
        private void DgvSalas_SelectionChanged(object? sender, EventArgs e)
        {
            // Evitamos responder al evento mientras la grilla se está rellenando
            if (_cargandoDatos)
                return;

            if (dgvSalas.CurrentRow == null || dgvSalas.CurrentRow.Index < 0)
            {
                _idSalaSeleccionada = 0;
                return;
            }

            var fila = dgvSalas.CurrentRow;
            if (fila.Cells["id_sala"].Value == null)
                return;

            // 1. Obtenemos el identificador de la sala seleccionada
            _idSalaSeleccionada = Convert.ToInt32(fila.Cells["id_sala"].Value);

            // 2. Poblamos las cajas de texto y el combo de estado
            txtNombreSala.Text = fila.Cells["nombreSala"].Value?.ToString() ?? string.Empty;

            string estado = fila.Cells["estadoSala"].Value?.ToString() ?? "Disponible";
            int indiceEstado = cmbEstadoSala.FindStringExact(estado);
            cmbEstadoSala.SelectedIndex = indiceEstado >= 0 ? indiceEstado : 0;

            // 3. Obtenemos los IDs de los médicos actualmente vinculados a esta sala según la cache
            var medicosAsignadosIds = _salasCache
                .Where(s => s.IdSala == _idSalaSeleccionada && s.IdUsuario.HasValue)
                .Select(s => s.IdUsuario!.Value)
                .ToHashSet();

            // 4. Marcamos en el CheckedListBox los profesionales que atienden en esta sala
            for (int i = 0; i < clbPersonal.Items.Count; i++)
            {
                if (clbPersonal.Items[i] is ItemMedico med)
                {
                    clbPersonal.SetItemChecked(i, medicosAsignadosIds.Contains(med.IdUsuario));
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            string nombreSala = txtNombreSala.Text.Trim();
            string estadoSala = cmbEstadoSala.SelectedItem!.ToString()!;

            List<int> idsMedicosSeleccionados = new List<int>();
            foreach (var item in clbPersonal.CheckedItems)
            {
                if (item is ItemMedico med)
                {
                    idsMedicosSeleccionados.Add(med.IdUsuario);
                }
            }

            try
            {
                // BLL delega a SalaDAL -> sp_InsertarSala y sp_AsignarSalaMedico
                _salaBLL.RegistrarSala(nombreSala, estadoSala, idsMedicosSeleccionados);

                MessageBox.Show($"Sala '{nombreSala}' registrada correctamente.",
                    "Sala Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarSalasDesdeBD();
                LimpiarCampos();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al guardar la sala:\n" + ex.Message,
                    "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento disparado al presionar el botón 'Modificar'.
        /// Valida los datos editados y delega en SalaBLL -> SalaDAL -> sp_ModificarSala
        /// para actualizar el nombre, estado y reasignar los médicos vinculados.
        /// </summary>
        private void btnModificar_Click(object? sender, EventArgs e)
        {
            // 1. Verificamos que previamente se haya seleccionado una sala en la tabla inferior
            if (_idSalaSeleccionada <= 0)
            {
                MessageBox.Show("Por favor, seleccioná una sala de la lista inferior para modificar sus datos.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Validamos que los campos obligatorios contengan información válida
            if (!ValidarCampos())
                return;

            string nombreSala = txtNombreSala.Text.Trim();
            string estadoSala = cmbEstadoSala.SelectedItem!.ToString()!;

            // 3. Recopilamos los médicos tildados para la reasignación
            List<int> idsMedicosSeleccionados = new List<int>();
            foreach (var item in clbPersonal.CheckedItems)
            {
                if (item is ItemMedico med)
                {
                    idsMedicosSeleccionados.Add(med.IdUsuario);
                }
            }

            try
            {
                // 4. Invocamos a la Capa BLL que ejecutará sp_ModificarSala y actualizará DetallesSalas
                _salaBLL.ModificarSala(_idSalaSeleccionada, nombreSala, estadoSala, idsMedicosSeleccionados);

                MessageBox.Show($"La sala '{nombreSala}' fue modificada correctamente.",
                    "Modificación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 5. Refrescamos la grilla y dejamos el formulario en estado inicial
                CargarSalasDesdeBD();
                LimpiarCampos();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al modificar la sala:\n" + ex.Message,
                    "Error al modificar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvSalas.CurrentRow == null || dgvSalas.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccioná una sala de la lista para desactivarla.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idSala = Convert.ToInt32(dgvSalas.CurrentRow.Cells["id_sala"].Value);
            string nombreSala = dgvSalas.CurrentRow.Cells["nombreSala"].Value?.ToString() ?? "Sala";

            var confirmar = MessageBox.Show($"¿Seguro que querés desactivar la sala '{nombreSala}'?",
                "Confirmar Desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                // BLL delega a SalaDAL -> sp_EliminarSala (baja lógica)
                _salaBLL.EliminarSala(idSala);

                MessageBox.Show($"La sala '{nombreSala}' fue desactivada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarSalasDesdeBD();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al desactivar la sala:\n" + ex.Message,
                    "Error al desactivar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento del botón 'Limpiar' para cancelar la edición actual y resetear el formulario.
        /// </summary>
        private void btnLimpiar_Click(object? sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombreSala.Text))
            {
                MessageBox.Show("Ingresá el nombre de la sala.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreSala.Focus();
                return false;
            }

            if (cmbEstadoSala.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccioná el estado inicial de la sala.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbEstadoSala.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Restablece los campos de texto, combos y selecciones a sus valores por defecto.
        /// </summary>
        private void LimpiarCampos()
        {
            _idSalaSeleccionada = 0;
            txtNombreSala.Clear();
            cmbEstadoSala.SelectedIndex = 0;

            for (int i = 0; i < clbPersonal.Items.Count; i++)
            {
                clbPersonal.SetItemChecked(i, false);
            }

            dgvSalas.ClearSelection();
            txtNombreSala.Focus();
        }
    }
}