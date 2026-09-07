using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmSalasAdmin : Form
    {
        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly SalaBLL _salaBLL = new SalaBLL();
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();

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

            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "id_sala", HeaderText = "ID Sala", ReadOnly = true, Width = 60 });
            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "nombreSala", HeaderText = "Nombre de Sala", Width = 150 });
            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "estadoSala", HeaderText = "Estado", Width = 120 });
            dgvSalas.Columns.Add(new DataGridViewTextBoxColumn { Name = "personal_asignado", HeaderText = "Personal Asignado", Width = 250 });
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
        /// </summary>
        private void CargarSalasDesdeBD()
        {
            dgvSalas.Rows.Clear();

            try
            {
                // BLL delega a SalaDAL -> sp_ObtenerSalas
                var salas = _salaBLL.ObtenerSalas(null);

                if (salas != null)
                {
                    foreach (var s in salas)
                    {
                        int filaIndex = dgvSalas.Rows.Add();
                        DataGridViewRow fila = dgvSalas.Rows[filaIndex];

                        fila.Cells["id_sala"].Value = s.IdSala;
                        fila.Cells["nombreSala"].Value = s.NombreSala;
                        fila.Cells["estadoSala"].Value = s.EstadoSala;

                        string medico = !string.IsNullOrWhiteSpace(s.ApellidoMedico)
                            ? $"{s.ApellidoMedico}, {s.NombreMedico}"
                            : "Sin asignar";
                        fila.Cells["personal_asignado"].Value = medico;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron consultar las salas desde la base de datos:\n" + ex.Message,
                    "Error al cargar salas", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void LimpiarCampos()
        {
            txtNombreSala.Clear();
            cmbEstadoSala.SelectedIndex = 0;

            for (int i = 0; i < clbPersonal.Items.Count; i++)
            {
                clbPersonal.SetItemChecked(i, false);
            }

            txtNombreSala.Focus();
        }
    }
}