using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmGestionUsuarios : Form
    {
        private const string ROL_PERSONAL_MEDICO = "Personal Médico";

        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();
        private readonly EspecialidadBLL _especialidadBLL = new EspecialidadBLL();
        private readonly SalaBLL _salaBLL = new SalaBLL();

        // Wrapper para mostrar texto y conservar el Id real en ComboBox/CheckedListBox
        private sealed class ItemConId
        {
            public int Id { get; set; }
            public string Texto { get; set; } = string.Empty;
            public override string ToString() => Texto;
        }

        public FrmGestionUsuarios()
        {
            InitializeComponent();
            this.Load += FrmPersonal_Load;
        }

        private void FrmPersonal_Load(object sender, EventArgs e)
        {
            ConfigurarDataGrid();

            CargarRolesDesdeBD();
            CargarEspecialidadesDesdeBD();
            CargarSalasDesdeBD();

            if (cmbRol.Items.Count > 0)
                cmbRol.SelectedIndex = 0;

            CargarUsuariosDesdeBD();
        }

        private void ConfigurarDataGrid()
        {
            dgvPersonal.Columns.Clear();
            dgvPersonal.AutoGenerateColumns = false;
            dgvPersonal.AllowUserToAddRows = false;
            dgvPersonal.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPersonal.MultiSelect = false;

            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdUsuario", HeaderText = "ID", ReadOnly = true, Width = 60 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", Width = 100 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Apellido", HeaderText = "Apellido", Width = 100 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Correo", HeaderText = "Correo", Width = 150 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Dni", HeaderText = "DNI", Width = 90 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", Width = 100 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Rol", HeaderText = "Rol", Width = 120 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "NroMatricula", HeaderText = "Matrícula", Width = 90 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Especialidades", HeaderText = "Especialidades", Width = 150 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Salas", HeaderText = "Salas", Width = 120 });
        }

        // ---------------------------------------------------------------
        // Carga de catálogos mediante la Capa de Negocio (BLL)
        // ---------------------------------------------------------------

        private void CargarRolesDesdeBD()
        {
            cmbRol.Items.Clear();
            cmbRol.Items.Add(new ItemConId { Id = 0, Texto = "Seleccione un rol..." });

            try
            {
                // BLL delega en UsuarioDAL -> sp_ListarRoles
                var roles = _usuarioBLL.ObtenerRoles();

                if (roles != null)
                {
                    foreach (var r in roles)
                    {
                        cmbRol.Items.Add(new ItemConId
                        {
                            Id = r.IdRol,
                            Texto = r.Descripcion
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los roles de usuario:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEspecialidadesDesdeBD()
        {
            clbEspecialidades.Items.Clear();

            try
            {
                // BLL delega en EspecialidadDAL -> sp_ListarEspecialidades
                var especialidades = _especialidadBLL.ObtenerEspecialidades();

                if (especialidades != null)
                {
                    foreach (var esp in especialidades)
                    {
                        clbEspecialidades.Items.Add(new ItemConId
                        {
                            Id = esp.IdEspecialidad,
                            Texto = esp.Nombre
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las especialidades:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarSalasDesdeBD()
        {
            clbSala.Items.Clear();

            try
            {
                // BLL delega en SalaDAL -> sp_ObtenerSalas
                var salas = _salaBLL.ObtenerSalas(null);

                if (salas != null)
                {
                    foreach (var s in salas)
                    {
                        clbSala.Items.Add(new ItemConId
                        {
                            Id = s.IdSala,
                            Texto = s.NombreSala
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las salas:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Obtiene los usuarios activos a través de UsuarioBLL (delegando en sp_ListarUsuarios).
        /// </summary>
        private void CargarUsuariosDesdeBD()
        {
            dgvPersonal.Rows.Clear();

            try
            {
                // BLL delega en UsuarioDAL -> sp_ListarUsuarios
                var usuarios = _usuarioBLL.ObtenerUsuarios();

                if (usuarios != null)
                {
                    foreach (var u in usuarios)
                    {
                        int filaIndex = dgvPersonal.Rows.Add();
                        DataGridViewRow fila = dgvPersonal.Rows[filaIndex];

                        fila.Cells["IdUsuario"].Value = u.IdUsuario;
                        fila.Cells["Nombre"].Value = u.Nombre;
                        fila.Cells["Apellido"].Value = u.Apellido;
                        fila.Cells["Correo"].Value = u.Correo;
                        fila.Cells["Dni"].Value = u.Dni;
                        fila.Cells["Telefono"].Value = u.Telefono;
                        fila.Cells["Rol"].Value = u.Rol;
                        fila.Cells["NroMatricula"].Value = !string.IsNullOrWhiteSpace(u.NroMatricula) ? u.NroMatricula : "";
                        fila.Cells["Especialidades"].Value = !string.IsNullOrWhiteSpace(u.Especialidades) ? u.Especialidades : "";
                        fila.Cells["Salas"].Value = !string.IsNullOrWhiteSpace(u.Salas) ? u.Salas : "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar la lista de usuarios desde la base de datos:\n" + ex.Message,
                    "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------------------------
        // Dinámica visual de sección médica según rol
        // ---------------------------------------------------------------

        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarSeccionMedica();
        }

        private void ActualizarSeccionMedica()
        {
            bool esPersonalMedico = EsPersonalMedico();

            pnlDatosMedicos.Visible = esPersonalMedico;
            lblInfoMedico.Visible = !esPersonalMedico;

            if (!esPersonalMedico)
            {
                for (int i = 0; i < clbEspecialidades.Items.Count; i++)
                    clbEspecialidades.SetItemChecked(i, false);

                for (int i = 0; i < clbSala.Items.Count; i++)
                    clbSala.SetItemChecked(i, false);

                txtMatricula.Clear();
                txtNotaSala.Clear();
            }
        }

        private bool EsPersonalMedico()
        {
            return cmbRol.SelectedItem is ItemConId item &&
                   item.Texto.Equals(ROL_PERSONAL_MEDICO, StringComparison.OrdinalIgnoreCase);
        }

        // ---------------------------------------------------------------
        // Guardar y Desactivar usuarios mediante Capa BLL
        // ---------------------------------------------------------------

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string contrasena = txtContrasena.Text;
            string dni = txtDNI.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            var rolSeleccionado = (ItemConId)cmbRol.SelectedItem!;
            bool esPersonalMedico = EsPersonalMedico();

            string matricula = esPersonalMedico ? txtMatricula.Text.Trim() : string.Empty;
            string notaSala = txtNotaSala.Text.Trim();

            List<int> especialidadesIds = new List<int>();
            List<int> salasIds = new List<int>();

            if (esPersonalMedico)
            {
                foreach (var item in clbEspecialidades.CheckedItems)
                {
                    if (item is ItemConId esp)
                        especialidadesIds.Add(esp.Id);
                }

                foreach (var item in clbSala.CheckedItems)
                {
                    if (item is ItemConId sala)
                        salasIds.Add(sala.Id);
                }
            }

            try
            {
                // BLL orquesta el guardado con UsuarioDAL -> sp_InsertarUsuario, sp_AsignarEspecialidadMedico, sp_AsignarSalaMedico
                _usuarioBLL.RegistrarUsuario(nombre, apellido, correo, contrasena, dni, telefono,
                    rolSeleccionado.Id, matricula, especialidadesIds, salasIds, notaSala);

                MessageBox.Show($"Usuario '{nombre} {apellido}' registrado correctamente.",
                    "Usuario Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarUsuariosDesdeBD();
                LimpiarCampos();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al registrar el usuario:\n" + ex.Message,
                    "Error Inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvPersonal.CurrentRow == null || dgvPersonal.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccioná una fila para desactivar.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idUsuario = Convert.ToInt32(dgvPersonal.CurrentRow.Cells["IdUsuario"].Value);
            string usuarioNombre = $"{dgvPersonal.CurrentRow.Cells["Nombre"].Value} {dgvPersonal.CurrentRow.Cells["Apellido"].Value}";

            var confirmar = MessageBox.Show($"¿Seguro que querés desactivar al usuario '{usuarioNombre}'?",
                "Confirmar Desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                // BLL delega a UsuarioDAL -> sp_EliminarUsuario (baja lógica)
                _usuarioBLL.EliminarUsuario(idUsuario);

                MessageBox.Show($"El usuario '{usuarioNombre}' fue desactivado correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarUsuariosDesdeBD();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al desactivar el usuario:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                string.IsNullOrWhiteSpace(txtContrasena.Text) ||
                string.IsNullOrWhiteSpace(txtDNI.Text))
            {
                MessageBox.Show("Completá al menos Nombre, Apellido, Correo, Contraseña y DNI.",
                    "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Regex.IsMatch(txtDNI.Text.Trim(), @"^\d{7,8}$"))
            {
                MessageBox.Show("El DNI tiene que tener entre 7 y 8 números, sin puntos.",
                    "DNI inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDNI.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtCorreo.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("El correo no tiene un formato válido.",
                    "Correo inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCorreo.Focus();
                return false;
            }

            if (cmbRol.SelectedItem is not ItemConId rolSeleccionado || rolSeleccionado.Id == 0)
            {
                MessageBox.Show("Seleccioná un rol válido para el usuario.",
                    "Falta un dato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRol.Focus();
                return false;
            }

            if (EsPersonalMedico() && clbEspecialidades.CheckedItems.Count == 0)
            {
                MessageBox.Show("El personal médico debe tener al menos una especialidad seleccionada.",
                    "Falta un dato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtCorreo.Clear();
            txtContrasena.Clear();
            txtDNI.Clear();
            txtTelefono.Clear();
            txtNotaSala.Clear();

            if (cmbRol.Items.Count > 0)
                cmbRol.SelectedIndex = 0;

            txtNombre.Focus();
        }
    }
}