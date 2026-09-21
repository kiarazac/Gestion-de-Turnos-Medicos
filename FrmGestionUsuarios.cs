using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de administración de usuarios y personal médico (Versión 1.0 heredada).
    /// Permite el alta, modificación y baja lógica de usuarios con asignación de roles, consultorios y especialidades.
    /// </summary>
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

        // Variable para almacenar el ID del usuario seleccionado actualmente para modificar (0 si no hay selección)
        private int _idUsuarioSeleccionado = 0;

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

            // 1. Columnas no editables directamente por texto libre (claves primarias o relaciones foráneas complejas):
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdUsuario", HeaderText = "ID", ReadOnly = true, Width = 60 });

            // 2. Columnas editables directamente en la celda (inline editing con CellEndEdit):
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", Width = 100 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Apellido", HeaderText = "Apellido", Width = 100 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Correo", HeaderText = "Correo", Width = 150 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Dni", HeaderText = "DNI", Width = 90 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", Width = 100 });

            // 3. Columnas relacionales protegidas (se modifican seleccionando la fila y usando los selectores del formulario):
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Rol", HeaderText = "Rol", ReadOnly = true, Width = 120 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "NroMatricula", HeaderText = "Matrícula", Width = 90 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Especialidades", HeaderText = "Especialidades", ReadOnly = true, Width = 150 });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Salas", HeaderText = "Salas", ReadOnly = true, Width = 120 });
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
                    // Agrupamos por IdSala para que cada sala física aparezca una única vez en el checklist,
                    // ya que sp_ObtenerSalas devuelve una fila por cada médico asignado a dicha sala.
                    var salasUnicas = salas
                        .GroupBy(s => s.IdSala)
                        .Select(g => g.First())
                        .ToList();

                    foreach (var s in salasUnicas)
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
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al desactivar el usuario:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento disparado al hacer clic en el botón 'Modificar'.
        /// Lee los valores corregidos en las cajas de texto y combos, valida los datos y los envía a la Capa de Negocio (BLL).
        /// Permite actualizar datos personales, matrícula médica y la reasignación de sala.
        /// </summary>
        private void btnModificar_Click(object? sender, EventArgs e)
        {
            // 1. Verificamos que previamente se haya seleccionado una fila de la grilla
            if (_idUsuarioSeleccionado <= 0)
            {
                MessageBox.Show("Por favor, hacé clic sobre el usuario que deseás modificar en la tabla inferior.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Validamos los campos ingresados (indicando esModificacion: true para no exigir contraseña obligatoria)
            if (!ValidarCampos(esModificacion: true))
                return;

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string dni = txtDNI.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            var rolSeleccionado = (ItemConId)cmbRol.SelectedItem!;
            bool esPersonalMedico = EsPersonalMedico();
            string matricula = esPersonalMedico ? txtMatricula.Text.Trim() : string.Empty;
            string notaSala = txtNotaSala.Text.Trim();

            // 3. Determinamos las salas a enviar al procedimiento almacenado sp_ModificarUsuario
            // Si es personal médico, recopilamos todas las salas tildadas en clbSala (soporta múltiples salas simultáneas).
            // Si el usuario destildó todas las salas, enviamos una lista vacía para desasignarlas en DetallesSalas.
            // Si no es personal médico, enviamos null para no modificar las asignaciones de salas.
            List<int>? salasIdsParaModificar = null;
            if (esPersonalMedico)
            {
                salasIdsParaModificar = new List<int>();
                foreach (var item in clbSala.CheckedItems)
                {
                    if (item is ItemConId sala)
                        salasIdsParaModificar.Add(sala.Id);
                }
            }

            // 4. Confirmación del usuario antes de persistir los cambios
            var confirmacion = MessageBox.Show($"¿Deseás guardar las modificaciones para el usuario '{nombre} {apellido}'?",
                "Confirmar Modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                // 5. Invocamos a la Capa BLL -> DAL -> sp_ModificarUsuario (incluye matrícula, múltiples salas y descripción)
                _usuarioBLL.ModificarUsuario(_idUsuarioSeleccionado, nombre, apellido, correo, dni, telefono, matricula, rolSeleccionado.Id, salasIdsParaModificar, notaSala);

                MessageBox.Show($"El usuario '{nombre} {apellido}' fue modificado correctamente.",
                    "Usuario Modificado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 6. Recargamos la grilla y limpiamos los campos del formulario
                CargarUsuariosDesdeBD();
                LimpiarCampos();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al modificar el usuario:\n" + ex.Message,
                    "Error Inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento disparado al hacer clic en el botón 'Limpiar'.
        /// Deselecciona cualquier usuario activo y restablece el formulario al modo de alta.
        /// </summary>
        private void btnLimpiar_Click(object? sender, EventArgs e)
        {
            LimpiarCampos();
        }

        /// <summary>
        /// Evento disparado al hacer clic en una fila del DataGridView.
        /// Vuelca la información del usuario seleccionado en los campos del formulario para facilitar su edición,
        /// incluyendo matrícula médica y sincronización de checkboxes de salas y especialidades.
        /// </summary>
        private void dgvPersonal_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Validamos que el índice sea una fila de datos y no el encabezado
            if (e.RowIndex < 0 || e.RowIndex >= dgvPersonal.Rows.Count)
                return;

            DataGridViewRow fila = dgvPersonal.Rows[e.RowIndex];
            if (fila.Cells["IdUsuario"].Value == null || !int.TryParse(fila.Cells["IdUsuario"].Value?.ToString(), out int idUsuario))
                return;

            // Guardamos el ID del usuario seleccionado
            _idUsuarioSeleccionado = idUsuario;

            // Poblamos los campos de texto
            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString() ?? string.Empty;
            txtApellido.Text = fila.Cells["Apellido"].Value?.ToString() ?? string.Empty;
            txtCorreo.Text = fila.Cells["Correo"].Value?.ToString() ?? string.Empty;
            txtDNI.Text = fila.Cells["Dni"].Value?.ToString() ?? string.Empty;
            txtTelefono.Text = fila.Cells["Telefono"].Value?.ToString() ?? string.Empty;

            // La contraseña no se muestra por seguridad; se deja limpia a menos que el usuario desee cambiarla
            txtContrasena.Clear();

            // Sincronizamos el ComboBox de roles según el texto del rol del usuario
            string rolNombre = fila.Cells["Rol"].Value?.ToString() ?? string.Empty;
            for (int i = 0; i < cmbRol.Items.Count; i++)
            {
                if (cmbRol.Items[i] is ItemConId item && item.Texto.Equals(rolNombre, StringComparison.OrdinalIgnoreCase))
                {
                    cmbRol.SelectedIndex = i;
                    break;
                }
            }

            // Si es personal médico, cargamos la matrícula y sincronizamos las salas y especialidades asignadas
            txtMatricula.Text = fila.Cells["NroMatricula"].Value?.ToString() ?? string.Empty;

            if (EsPersonalMedico())
            {
                // Sincronizamos la selección de salas en clbSala según los datos de la fila seleccionada
                string salasTexto = fila.Cells["Salas"].Value?.ToString() ?? string.Empty;
                var salasAsignadas = salasTexto.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < clbSala.Items.Count; i++)
                {
                    if (clbSala.Items[i] is ItemConId salaItem)
                    {
                        bool estaAsignada = salasAsignadas.Any(s => s.Equals(salaItem.Texto, StringComparison.OrdinalIgnoreCase));
                        clbSala.SetItemChecked(i, estaAsignada);
                    }
                }

                // Sincronizamos la selección de especialidades en clbEspecialidades según la fila
                string espTexto = fila.Cells["Especialidades"].Value?.ToString() ?? string.Empty;
                var espAsignadas = espTexto.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < clbEspecialidades.Items.Count; i++)
                {
                    if (clbEspecialidades.Items[i] is ItemConId espItem)
                    {
                        bool estaAsignada = espAsignadas.Any(e => e.Equals(espItem.Texto, StringComparison.OrdinalIgnoreCase));
                        clbEspecialidades.SetItemChecked(i, estaAsignada);
                    }
                }
            }
        }

        /// <summary>
        /// Evento disparado al finalizar la edición de una celda directamente en la grilla (inline editing).
        /// Valida el dato cambiado y lo persiste de inmediato en la base de datos a través de la Capa BLL.
        /// </summary>
        private void dgvPersonal_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            // Validamos que sea una fila válida
            if (e.RowIndex < 0 || e.RowIndex >= dgvPersonal.Rows.Count)
                return;

            DataGridViewRow fila = dgvPersonal.Rows[e.RowIndex];
            if (fila.Cells["IdUsuario"].Value == null || !int.TryParse(fila.Cells["IdUsuario"].Value?.ToString(), out int idUsuario))
                return;

            // Leemos los valores actuales de la fila editada
            string nombre = fila.Cells["Nombre"].Value?.ToString()?.Trim() ?? string.Empty;
            string apellido = fila.Cells["Apellido"].Value?.ToString()?.Trim() ?? string.Empty;
            string correo = fila.Cells["Correo"].Value?.ToString()?.Trim() ?? string.Empty;
            string dni = fila.Cells["Dni"].Value?.ToString()?.Trim() ?? string.Empty;
            string telefono = fila.Cells["Telefono"].Value?.ToString()?.Trim() ?? string.Empty;
            string matricula = fila.Cells["NroMatricula"].Value?.ToString()?.Trim() ?? string.Empty;

            // Validaciones básicas de campos obligatorios
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(dni))
            {
                MessageBox.Show("Los campos Nombre, Apellido, Correo y DNI no pueden quedar vacíos.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CargarUsuariosDesdeBD(); // Revertimos la grilla a su estado anterior
                return;
            }

            // Validación de DNI (7 a 8 dígitos)
            if (!Regex.IsMatch(dni, @"^\d{7,8}$"))
            {
                MessageBox.Show("El DNI debe tener entre 7 y 8 números sin puntos.",
                    "DNI Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CargarUsuariosDesdeBD();
                return;
            }

            // Validación de formato de correo
            if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("El correo no tiene un formato válido.",
                    "Correo Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CargarUsuariosDesdeBD();
                return;
            }

            try
            {
                // Persistimos los cambios a través de la Capa BLL (idRol = null conserva el rol actual en SQL)
                _usuarioBLL.ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, matricula, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al guardar la edición de la celda:\n" + ex.Message,
                    "Error al Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CargarUsuariosDesdeBD(); // Revertimos ante cualquier excepción
            }
        }

        /// <summary>
        /// Valida los campos obligatorios del formulario y sus formatos requeridos.
        /// </summary>
        /// <param name="esModificacion">Indica si se trata de una modificación (en cuyo caso la contraseña no es obligatoria).</param>
        private bool ValidarCampos(bool esModificacion = false)
        {
            bool camposBasicosIncompletos = string.IsNullOrWhiteSpace(txtNombre.Text) ||
                                            string.IsNullOrWhiteSpace(txtApellido.Text) ||
                                            string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                                            string.IsNullOrWhiteSpace(txtDNI.Text);

            // Al crear un nuevo usuario, la contraseña es estrictamente obligatoria
            if (!esModificacion && string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                camposBasicosIncompletos = true;
            }

            if (camposBasicosIncompletos)
            {
                string mensaje = esModificacion
                    ? "Completá al menos Nombre, Apellido, Correo y DNI."
                    : "Completá al menos Nombre, Apellido, Correo, Contraseña y DNI.";

                MessageBox.Show(mensaje, "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            if (EsPersonalMedico() && clbEspecialidades.CheckedItems.Count == 0 && !esModificacion)
            {
                MessageBox.Show("El personal médico debe tener al menos una especialidad seleccionada.",
                    "Falta un dato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Limpia los campos del formulario y deselecciona al usuario activo para permitir un nuevo registro.
        /// </summary>
        private void LimpiarCampos()
        {
            _idUsuarioSeleccionado = 0; // Deseleccionamos el usuario en modificación

            txtNombre.Clear();
            txtApellido.Clear();
            txtCorreo.Clear();
            txtContrasena.Clear();
            txtDNI.Clear();
            txtTelefono.Clear();
            txtMatricula.Clear();
            txtNotaSala.Clear();

            if (cmbRol.Items.Count > 0)
                cmbRol.SelectedIndex = 0;

            for (int i = 0; i < clbEspecialidades.Items.Count; i++)
                clbEspecialidades.SetItemChecked(i, false);

            for (int i = 0; i < clbSala.Items.Count; i++)
                clbSala.SetItemChecked(i, false);

            dgvPersonal.ClearSelection();
            txtNombre.Focus();
        }
    }
}