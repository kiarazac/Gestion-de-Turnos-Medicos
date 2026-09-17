using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de Gestión de Personal y Usuarios (Versión 2.0).
    /// Implementa un flujo guiado y obligatorio:
    ///   1. Solicita en primer lugar el DNI del usuario a gestionar.
    ///   2. Evalúa en la base de datos si ya existe:
    ///      - Si existe: Entra en 'Modo Modificación', precarga todos sus datos previos,
    ///        habilita modificar/desactivar y bloquea el botón de nuevo registro.
    ///      - Si no existe: Entra en 'Modo Alta', deja el DNI fijado y habilita los campos
    ///        en blanco junto con el botón de registrar nuevo usuario.
    ///   3. Posee un rediseño moderno por tarjetas (cards) y paleta clínica profesional.
    /// </summary>
    public partial class FrmGestionUsuarios2 : Form
    {
        private const string ROL_PERSONAL_MEDICO = "Personal Médico";

        // Invocación a las capas de negocio
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();
        private readonly EspecialidadBLL _especialidadBLL = new EspecialidadBLL();
        private readonly SalaBLL _salaBLL = new SalaBLL();

        // Identificador del usuario que se está editando (0 para nuevo registro)
        private int _idUsuarioSeleccionado = 0;

        // Clase auxiliar para vincular identificadores de BD con el texto de presentación
        private sealed class ItemConId
        {
            public int Id { get; set; }
            public string Texto { get; set; } = string.Empty;
            public override string ToString() => Texto;
        }

        public FrmGestionUsuarios2()
        {
            InitializeComponent();
            this.Load += FrmGestionUsuarios2_Load;
        }

        private void FrmGestionUsuarios2_Load(object? sender, EventArgs e)
        {
            ConfigurarEstilosDataGrid();
            CargarRolesDesdeBD();
            CargarEspecialidadesDesdeBD();
            CargarSalasDesdeBD();
            CargarUsuariosDesdeBD();

            // Al inicio, el formulario de datos permanece deshabilitado hasta verificar el DNI
            BloquearFormularioEnPaso1();
        }

        // ---------------------------------------------------------------------
        // PASO 1: Control de flujo y verificación obligatoria por DNI
        // ---------------------------------------------------------------------

        /// <summary>
        /// Restablece el estado inicial: bloquea el formulario inferior y enfoca la caja de DNI.
        /// </summary>
        private void BloquearFormularioEnPaso1()
        {
            _idUsuarioSeleccionado = 0;

            // Bloqueamos el contenedor del formulario hasta que se valide el DNI
            pnlFormularioDatos.Enabled = false;

            // Limpiamos los campos
            txtNombre.Clear();
            txtApellido.Clear();
            txtDniVerificado.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            txtContrasena.Clear();
            txtMatricula.Clear();
            txtNotaSala.Clear();

            if (cmbRol.Items.Count > 0)
                cmbRol.SelectedIndex = 0;

            DesmarcarChecklists();

            // Configuramos el distintivo de estado inicial
            lblEstadoDni.Text = "Esperando ingreso de DNI...";
            lblEstadoDni.BackColor = Color.FromArgb(241, 245, 249);
            lblEstadoDni.ForeColor = Color.FromArgb(100, 116, 139);

            btnGuardar.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;

            txtDniBusqueda.Clear();
            txtDniBusqueda.Focus();
        }

        /// <summary>
        /// Evento disparado al presionar el botón 'Verificar DNI'.
        /// </summary>
        private void btnVerificarDni_Click(object sender, EventArgs e)
        {
            EjecutarVerificacionDni();
        }

        /// <summary>
        /// Permite presionar la tecla Enter en el cuadro de búsqueda para verificar directamente.
        /// </summary>
        private void txtDniBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evita el beep de Windows
                EjecutarVerificacionDni();
            }
        }

        /// <summary>
        /// Realiza la consulta a la base de datos para determinar si el DNI pertenece a un usuario existente o nuevo.
        /// </summary>
        private void EjecutarVerificacionDni()
        {
            string dniIngresado = txtDniBusqueda.Text.Trim();

            // 1. Validaciones básicas de formato del DNI
            if (string.IsNullOrWhiteSpace(dniIngresado))
            {
                MessageBox.Show("Por favor, ingrese un número de DNI para continuar.",
                    "DNI Obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDniBusqueda.Focus();
                return;
            }

            if (!Regex.IsMatch(dniIngresado, @"^\d{7,10}$"))
            {
                MessageBox.Show("El DNI debe contener entre 7 y 10 dígitos numéricos.",
                    "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDniBusqueda.Focus();
                return;
            }

            try
            {
                // 2. Consultamos los usuarios activos en la base de datos
                var usuarios = _usuarioBLL.ObtenerUsuarios();
                var usuarioExistente = usuarios?.FirstOrDefault(u => u.Dni.Trim() == dniIngresado);

                // Habilitamos el panel de carga de datos
                pnlFormularioDatos.Enabled = true;

                if (usuarioExistente != null)
                {
                    // -------------------------------------------------------------
                    // CASO A: EL USUARIO YA EXISTE -> MODO MODIFICACIÓN
                    // -------------------------------------------------------------
                    _idUsuarioSeleccionado = usuarioExistente.IdUsuario;

                    lblEstadoDni.Text = $"✓ Usuario Registrado: {usuarioExistente.Apellido}, {usuarioExistente.Nombre} (Modo Modificación)";
                    lblEstadoDni.BackColor = Color.FromArgb(209, 250, 229); // Verde suave
                    lblEstadoDni.ForeColor = Color.FromArgb(6, 95, 70);    // Verde oscuro

                    // Precargamos los datos personales existentes
                    txtDniVerificado.Text = usuarioExistente.Dni;
                    txtNombre.Text = usuarioExistente.Nombre;
                    txtApellido.Text = usuarioExistente.Apellido;
                    txtTelefono.Text = usuarioExistente.Telefono;
                    txtCorreo.Text = usuarioExistente.Correo;
                    txtContrasena.Clear(); // La contraseña se deja en blanco si no se desea modificar

                    // Seleccionamos el rol del usuario
                    for (int i = 0; i < cmbRol.Items.Count; i++)
                    {
                        if (cmbRol.Items[i] is ItemConId item && item.Texto.Equals(usuarioExistente.Rol, StringComparison.OrdinalIgnoreCase))
                        {
                            cmbRol.SelectedIndex = i;
                            break;
                        }
                    }

                    // Sincronizamos sección médica si aplica
                    ActualizarSeccionMedica();

                    if (EsPersonalMedico())
                    {
                        txtMatricula.Text = usuarioExistente.NroMatricula ?? string.Empty;

                        // Marcamos las especialidades asignadas
                        string espTexto = usuarioExistente.Especialidades ?? string.Empty;
                        var espArray = espTexto.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < clbEspecialidades.Items.Count; i++)
                        {
                            if (clbEspecialidades.Items[i] is ItemConId espItem)
                            {
                                bool marcada = espArray.Any(e => e.Equals(espItem.Texto, StringComparison.OrdinalIgnoreCase));
                                clbEspecialidades.SetItemChecked(i, marcada);
                            }
                        }

                        // Marcamos las salas asignadas
                        string salasTexto = usuarioExistente.Salas ?? string.Empty;
                        var salasArray = salasTexto.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < clbSala.Items.Count; i++)
                        {
                            if (clbSala.Items[i] is ItemConId salaItem)
                            {
                                bool marcada = salasArray.Any(s => s.Equals(salaItem.Texto, StringComparison.OrdinalIgnoreCase));
                                clbSala.SetItemChecked(i, marcada);
                            }
                        }
                    }

                    // Botones de acción para modo modificación
                    btnGuardar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;

                    txtNombre.Focus();
                }
                else
                {
                    // -------------------------------------------------------------
                    // CASO B: EL USUARIO NO EXISTE -> MODO ALTA / REGISTRO
                    // -------------------------------------------------------------
                    _idUsuarioSeleccionado = 0;

                    lblEstadoDni.Text = "＋ Nuevo Usuario: Ingrese los datos para el alta (Modo Registro)";
                    lblEstadoDni.BackColor = Color.FromArgb(224, 242, 254); // Azul suave
                    lblEstadoDni.ForeColor = Color.FromArgb(7, 89, 133);    // Azul oscuro

                    // Fijamos el DNI verificado y limpiamos el resto de campos
                    txtDniVerificado.Text = dniIngresado;
                    txtNombre.Clear();
                    txtApellido.Clear();
                    txtTelefono.Clear();
                    txtCorreo.Clear();
                    txtContrasena.Clear();
                    txtMatricula.Clear();
                    txtNotaSala.Clear();

                    if (cmbRol.Items.Count > 0)
                        cmbRol.SelectedIndex = 0;

                    DesmarcarChecklists();
                    ActualizarSeccionMedica();

                    // Botones de acción para modo alta
                    btnGuardar.Enabled = true;
                    btnModificar.Enabled = false;
                    btnEliminar.Enabled = false;

                    txtNombre.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al verificar el DNI en la base de datos:\n" + ex.Message,
                    "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReiniciarDni_Click(object sender, EventArgs e)
        {
            BloquearFormularioEnPaso1();
        }

        // ---------------------------------------------------------------------
        // Configuración y carga de catálogos desde la Base de Datos
        // ---------------------------------------------------------------------

        private void CargarRolesDesdeBD()
        {
            cmbRol.Items.Clear();
            cmbRol.Items.Add(new ItemConId { Id = 0, Texto = "Seleccione un rol..." });

            try
            {
                var roles = _usuarioBLL.ObtenerRoles();
                if (roles != null)
                {
                    foreach (var r in roles)
                    {
                        cmbRol.Items.Add(new ItemConId { Id = r.IdRol, Texto = r.Descripcion });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los roles:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEspecialidadesDesdeBD()
        {
            clbEspecialidades.Items.Clear();

            try
            {
                var especialidades = _especialidadBLL.ObtenerEspecialidades();
                if (especialidades != null)
                {
                    foreach (var esp in especialidades)
                    {
                        clbEspecialidades.Items.Add(new ItemConId { Id = esp.IdEspecialidad, Texto = esp.Nombre });
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
                var salas = _salaBLL.ObtenerSalas(null);
                if (salas != null)
                {
                    // Agrupamos por IdSala para que cada sala física aparezca una única vez
                    var salasUnicas = salas
                        .GroupBy(s => s.IdSala)
                        .Select(g => g.First())
                        .ToList();

                    foreach (var s in salasUnicas)
                    {
                        clbSala.Items.Add(new ItemConId { Id = s.IdSala, Texto = s.NombreSala });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las salas:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarUsuariosDesdeBD()
        {
            dgvPersonal.Rows.Clear();

            try
            {
                var usuarios = _usuarioBLL.ObtenerUsuarios();
                if (usuarios != null)
                {
                    foreach (var u in usuarios)
                    {
                        int index = dgvPersonal.Rows.Add();
                        DataGridViewRow fila = dgvPersonal.Rows[index];

                        fila.Cells["IdUsuario"].Value = u.IdUsuario;
                        fila.Cells["Dni"].Value = u.Dni;
                        fila.Cells["Nombre"].Value = u.Nombre;
                        fila.Cells["Apellido"].Value = u.Apellido;
                        fila.Cells["Rol"].Value = u.Rol;
                        fila.Cells["Correo"].Value = u.Correo;
                        fila.Cells["Telefono"].Value = u.Telefono;
                        fila.Cells["NroMatricula"].Value = !string.IsNullOrWhiteSpace(u.NroMatricula) ? u.NroMatricula : "--";
                        fila.Cells["Especialidades"].Value = !string.IsNullOrWhiteSpace(u.Especialidades) ? u.Especialidades : "--";
                        fila.Cells["Salas"].Value = !string.IsNullOrWhiteSpace(u.Salas) ? u.Salas : "--";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar el listado de personal:\n" + ex.Message,
                    "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------------------------------
        // Estilos visuales del DataGridView
        // ---------------------------------------------------------------------

        private void ConfigurarEstilosDataGrid()
        {
            dgvPersonal.Columns.Clear();
            dgvPersonal.AutoGenerateColumns = false;
            dgvPersonal.AllowUserToAddRows = false;
            dgvPersonal.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPersonal.MultiSelect = false;
            dgvPersonal.RowHeadersVisible = false;
            dgvPersonal.EnableHeadersVisualStyles = false;

            // Encabezados con estilo médico moderno
            dgvPersonal.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59); // Slate oscuro
            dgvPersonal.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPersonal.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvPersonal.ColumnHeadersHeight = 36;

            // Celdas y alternancia
            dgvPersonal.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvPersonal.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 251, 241); // Soft teal
            dgvPersonal.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvPersonal.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvPersonal.RowTemplate.Height = 28;

            // Columnas ordenadas lógicamente
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdUsuario", HeaderText = "ID", Width = 50, ReadOnly = true });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Dni", HeaderText = "DNI", Width = 95, ReadOnly = true });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", Width = 110, ReadOnly = true });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Apellido", HeaderText = "Apellido", Width = 110, ReadOnly = true });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Rol", HeaderText = "Rol", Width = 120, ReadOnly = true });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Correo", HeaderText = "Correo Electrónico", Width = 160, ReadOnly = true });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", Width = 100, ReadOnly = true });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "NroMatricula", HeaderText = "Matrícula", Width = 90, ReadOnly = true });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Especialidades", HeaderText = "Especialidades", Width = 160, ReadOnly = true });
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Salas", HeaderText = "Salas Asignadas", Width = 130, ReadOnly = true });
        }

        // ---------------------------------------------------------------------
        // Dinámica de sección médica según el rol seleccionado
        // ---------------------------------------------------------------------

        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarSeccionMedica();
        }

        private void ActualizarSeccionMedica()
        {
            bool esMedico = EsPersonalMedico();
            pnlDatosMedicos.Visible = esMedico;

            if (!esMedico)
            {
                DesmarcarChecklists();
                txtMatricula.Clear();
                txtNotaSala.Clear();
            }
        }

        private bool EsPersonalMedico()
        {
            return cmbRol.SelectedItem is ItemConId item &&
                   item.Texto.Equals(ROL_PERSONAL_MEDICO, StringComparison.OrdinalIgnoreCase);
        }

        private void DesmarcarChecklists()
        {
            for (int i = 0; i < clbEspecialidades.Items.Count; i++)
                clbEspecialidades.SetItemChecked(i, false);

            for (int i = 0; i < clbSala.Items.Count; i++)
                clbSala.SetItemChecked(i, false);
        }

        // ---------------------------------------------------------------------
        // Operaciones CRUD: Guardar (Alta), Modificar y Desactivar
        // ---------------------------------------------------------------------

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos(esAlta: true))
                return;

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string contrasena = txtContrasena.Text;
            string dni = txtDniVerificado.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            var rolSeleccionado = (ItemConId)cmbRol.SelectedItem!;
            bool esMedico = EsPersonalMedico();

            string matricula = esMedico ? txtMatricula.Text.Trim() : string.Empty;
            string notaSala = txtNotaSala.Text.Trim();

            List<int> especialidadesIds = new List<int>();
            List<int> salasIds = new List<int>();

            if (esMedico)
            {
                foreach (var item in clbEspecialidades.CheckedItems)
                    if (item is ItemConId esp) especialidadesIds.Add(esp.Id);

                foreach (var item in clbSala.CheckedItems)
                    if (item is ItemConId sala) salasIds.Add(sala.Id);
            }

            try
            {
                _usuarioBLL.RegistrarUsuario(nombre, apellido, correo, contrasena, dni, telefono,
                    rolSeleccionado.Id, matricula, especialidadesIds, salasIds, notaSala);

                MessageBox.Show($"Usuario '{nombre} {apellido}' registrado exitosamente en el sistema.",
                    "Registro Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarUsuariosDesdeBD();

                // Simulamos una nueva verificación con el mismo DNI para dejarlo abierto en modo modificación
                txtDniBusqueda.Text = dni;
                EjecutarVerificacionDni();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo registrar el usuario:\n" + ex.Message,
                    "Error de Registro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_idUsuarioSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar o verificar un usuario existente para modificar.",
                    "Usuario no identificado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos(esAlta: false))
                return;

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string dni = txtDniVerificado.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            var rolSeleccionado = (ItemConId)cmbRol.SelectedItem!;
            bool esMedico = EsPersonalMedico();

            string? matricula = esMedico ? txtMatricula.Text.Trim() : null;
            string notaSala = txtNotaSala.Text.Trim();

            List<int>? salasIds = null;
            if (esMedico)
            {
                salasIds = new List<int>();
                foreach (var item in clbSala.CheckedItems)
                    if (item is ItemConId sala) salasIds.Add(sala.Id);
            }

            var confirmacion = MessageBox.Show($"¿Desea guardar los cambios para '{nombre} {apellido}'?",
                "Confirmar Modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                _usuarioBLL.ModificarUsuario(_idUsuarioSeleccionado, nombre, apellido, correo, dni, telefono,
                    matricula, rolSeleccionado.Id, salasIds, notaSala);

                MessageBox.Show($"Usuario '{nombre} {apellido}' actualizado correctamente.",
                    "Modificación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarUsuariosDesdeBD();

                // Recargamos el estado actualizado
                txtDniBusqueda.Text = dni;
                EjecutarVerificacionDni();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron guardar las modificaciones:\n" + ex.Message,
                    "Error al Modificar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idUsuarioSeleccionado <= 0)
            {
                MessageBox.Show("Seleccione un usuario activo para desactivar.",
                    "Sin Selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombreCompleto = $"{txtNombre.Text.Trim()} {txtApellido.Text.Trim()}";
            var resultado = MessageBox.Show($"¿Está seguro de que desea desactivar al usuario '{nombreCompleto}'?",
                "Confirmar Desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (resultado != DialogResult.Yes)
                return;

            try
            {
                _usuarioBLL.EliminarUsuario(_idUsuarioSeleccionado);
                MessageBox.Show($"Usuario '{nombreCompleto}' desactivado correctamente.",
                    "Usuario Desactivado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarUsuariosDesdeBD();
                BloquearFormularioEnPaso1();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desactivar el usuario:\n" + ex.Message,
                    "Error de Desactivación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BloquearFormularioEnPaso1();
        }

        // ---------------------------------------------------------------------
        // Selección directa en la grilla para verificar DNI de forma ágil
        // ---------------------------------------------------------------------

        private void dgvPersonal_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvPersonal.Rows.Count)
                return;

            DataGridViewRow fila = dgvPersonal.Rows[e.RowIndex];
            string dni = fila.Cells["Dni"].Value?.ToString() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(dni))
            {
                txtDniBusqueda.Text = dni;
                EjecutarVerificacionDni();
            }
        }

        // ---------------------------------------------------------------------
        // Validaciones de entrada
        // ---------------------------------------------------------------------

        private bool ValidarCampos(bool esAlta)
        {
            if (string.IsNullOrWhiteSpace(txtDniVerificado.Text))
            {
                MessageBox.Show("Debe verificar un DNI antes de continuar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text) || txtNombre.Text.Trim().Length < 2)
            {
                MessageBox.Show("El nombre debe tener al menos 2 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text) || txtApellido.Text.Trim().Length < 2)
            {
                MessageBox.Show("El apellido debe tener al menos 2 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCorreo.Text) || !Validaciones.EsEmailValido(txtCorreo.Text.Trim()))
            {
                MessageBox.Show("Ingrese un correo electrónico válido (@gmail.com, @hotmail.com, etc.).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCorreo.Focus();
                return false;
            }

            if (esAlta && string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MessageBox.Show("Debe ingresar una contraseña inicial para el nuevo usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContrasena.Focus();
                return false;
            }

            if (cmbRol.SelectedItem is not ItemConId rol || rol.Id <= 0)
            {
                MessageBox.Show("Debe seleccionar un rol para el usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRol.Focus();
                return false;
            }

            if (EsPersonalMedico() && string.IsNullOrWhiteSpace(txtMatricula.Text))
            {
                MessageBox.Show("Para el rol 'Personal Médico' es obligatorio indicar la matrícula profesional.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatricula.Focus();
                return false;
            }

            return true;
        }
    }
}
