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

        // Usuario autenticado en el sistema (para verificar inmunidad y prevenir auto-eliminación)
        private readonly UsuarioLoginResult? _usuarioActual;

        // Clase auxiliar para vincular identificadores de BD con el texto de presentación
        private sealed class ItemConId
        {
            public int Id { get; set; }
            public string Texto { get; set; } = string.Empty;
            public override string ToString() => Texto;
        }

        public FrmGestionUsuarios2() : this(null)
        {
        }

        public FrmGestionUsuarios2(UsuarioLoginResult? usuarioActual)
        {
            InitializeComponent();
            _usuarioActual = usuarioActual;
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
            lblTituloDatosPersonales.Text = "👤 Datos Personales y de Cuenta";

            btnGuardar.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            btnReactivar.Enabled = false;

            txtBuscarUsuario.Clear();
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
                // 2. Consultamos el usuario en la base de datos (incluyendo registros con baja lógica)
                var usuarioExistente = _usuarioBLL.ObtenerUsuarioPorDni(dniIngresado, incluirInactivos: true);

                // Habilitamos el panel de carga de datos
                pnlFormularioDatos.Enabled = true;

                if (usuarioExistente != null)
                {
                    if (usuarioExistente.Activo)
                    {
                        // -------------------------------------------------------------
                        // CASO A: EL USUARIO ESTÁ ACTIVO -> MODO MODIFICACIÓN
                        // -------------------------------------------------------------
                        lblTituloDatosPersonales.Text = $"👤 Datos Personales: {usuarioExistente.Apellido}, {usuarioExistente.Nombre} (Modo Modificación)";

                        PrecargarDatosUsuario(usuarioExistente);

                        btnGuardar.Enabled = false;
                        btnModificar.Enabled = true;
                        btnEliminar.Enabled = true;
                        btnReactivar.Enabled = false;
                    }
                    else
                    {
                        // -------------------------------------------------------------
                        // CASO B: EL USUARIO ESTÁ INACTIVO -> MODO REACTIVACIÓN / RE-DAR DE ALTA
                        // -------------------------------------------------------------
                        lblTituloDatosPersonales.Text = $"⚠️ Usuario Inactivo: {usuarioExistente.Apellido}, {usuarioExistente.Nombre} (Modo Reactivación)";

                        PrecargarDatosUsuario(usuarioExistente);

                        // En modo reactivación, se habilita el botón simple de Re-dar de Alta
                        btnGuardar.Enabled = false;
                        btnModificar.Enabled = false;
                        btnEliminar.Enabled = false;
                        btnReactivar.Enabled = true;
                    }

                    txtNombre.Focus();
                }
                else
                {
                    // -------------------------------------------------------------
                    // CASO C: EL USUARIO NO EXISTE -> MODO ALTA / REGISTRO
                    // -------------------------------------------------------------
                    _idUsuarioSeleccionado = 0;

                    lblTituloDatosPersonales.Text = "👤 Nuevo Usuario: Ingrese los datos para el alta (Modo Registro)";

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
                    btnReactivar.Enabled = false;

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

        /// <summary>
        /// Precarga los datos personales, de contacto, credenciales, rol, matrícula y asignaciones
        /// de un usuario existente (sea activo o para reactivar) en los controles del formulario.
        /// </summary>
        /// <param name="usuario">Objeto DTO con los datos del usuario recuperado de la BD.</param>
        private void PrecargarDatosUsuario(UsuarioListadoDTO usuario)
        {
            _idUsuarioSeleccionado = usuario.IdUsuario;

            txtDniVerificado.Text = usuario.Dni;
            txtNombre.Text = usuario.Nombre;
            txtApellido.Text = usuario.Apellido;
            txtTelefono.Text = usuario.Telefono;
            txtCorreo.Text = usuario.Correo;
            txtContrasena.Clear(); // La contraseña se deja en blanco si no se desea modificar

            // Seleccionamos el rol del usuario
            for (int i = 0; i < cmbRol.Items.Count; i++)
            {
                if (cmbRol.Items[i] is ItemConId item)
                {
                    if (item.Texto.Equals(usuario.Rol, StringComparison.OrdinalIgnoreCase) ||
                        (!string.IsNullOrWhiteSpace(usuario.Rol) &&
                         (item.Texto.Contains(usuario.Rol, StringComparison.OrdinalIgnoreCase) ||
                          usuario.Rol.Contains(item.Texto, StringComparison.OrdinalIgnoreCase))))
                    {
                        cmbRol.SelectedIndex = i;
                        break;
                    }
                }
            }

            // Sincronizamos sección médica si aplica
            ActualizarSeccionMedica();

            if (EsPersonalMedico())
            {
                txtMatricula.Text = usuario.NroMatricula ?? string.Empty;

                // Marcamos las especialidades asignadas
                string espTexto = usuario.Especialidades ?? string.Empty;
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
                string salasTexto = usuario.Salas ?? string.Empty;
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
                bool mostrarInactivos = chkMostrarInactivos.Checked;
                var usuarios = _usuarioBLL.ObtenerUsuarios(incluirInactivos: mostrarInactivos);
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
                        fila.Cells["Estado"].Value = u.Activo ? "Activo" : "Inactivo";

                        // Estilo visual diferenciado para registros dados de baja
                        if (!u.Activo)
                        {
                            fila.DefaultCellStyle.ForeColor = Color.FromArgb(120, 113, 108); // Gris atenuado
                            fila.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242); // Fondo tenue rojizo
                        }
                    }

                    // Si existía un criterio de búsqueda previo en la caja de búsqueda, reaplicamos el filtro
                    if (txtBuscarUsuario != null && !string.IsNullOrWhiteSpace(txtBuscarUsuario.Text))
                    {
                        txtBuscarUsuario_TextChanged(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar el listado de personal:\n" + ex.Message,
                    "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkMostrarInactivos_CheckedChanged(object? sender, EventArgs e)
        {
            CargarUsuariosDesdeBD();
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
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", Width = 80, ReadOnly = true });
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
            if (cmbRol.SelectedItem is not ItemConId item || string.IsNullOrWhiteSpace(item.Texto))
                return false;

            return item.Texto.Contains("médic", StringComparison.OrdinalIgnoreCase) ||
                   item.Texto.Contains("medic", StringComparison.OrdinalIgnoreCase);
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

        /// <summary>
        /// Evento disparado al presionar el botón 'Modificar Datos'.
        /// Valida las entradas, recopila las salas y especialidades seleccionadas, y delega en UsuarioBLL -> UsuarioDAL
        /// para actualizar el usuario sin alterar la lógica de la base de datos SQL Server.
        /// </summary>
        private void btnModificar_Click(object sender, EventArgs e)
        {
            // 1. Verificamos que se haya cargado un usuario previamente verificado por DNI
            if (_idUsuarioSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar o verificar un usuario existente para modificar.",
                    "Usuario no identificado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Validamos los campos de entrada (esAlta = false permite conservar contraseña previa si está en blanco)
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

            // Clave opcional: si el campo de contraseña se llenó, se enviará para ser hasheada y actualizada
            string? nuevaContrasena = !string.IsNullOrWhiteSpace(txtContrasena.Text) ? txtContrasena.Text : null;

            // 3. Recopilamos las salas y especialidades médicas tildadas
            List<int>? salasIds = null;
            List<int>? especialidadesIds = null;

            if (esMedico)
            {
                salasIds = new List<int>();
                foreach (var item in clbSala.CheckedItems)
                {
                    if (item is ItemConId sala)
                        salasIds.Add(sala.Id);
                }

                especialidadesIds = new List<int>();
                foreach (var item in clbEspecialidades.CheckedItems)
                {
                    if (item is ItemConId esp)
                        especialidadesIds.Add(esp.Id);
                }
            }
            else
            {
                // Si el rol ya no es médico (ej. Administrador o Recepcionista), se limpian las asignaciones médicas previas
                salasIds = new List<int>();
                especialidadesIds = new List<int>();
            }

            // 4. Confirmación previa antes de persistir cambios
            var confirmacion = MessageBox.Show($"¿Desea guardar los cambios para '{nombre} {apellido}'?",
                "Confirmar Modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                // 5. Invocamos a la Capa de Negocio (UsuarioBLL) respetando la arquitectura en capas
                _usuarioBLL.ModificarUsuario(
                    _idUsuarioSeleccionado,
                    nombre,
                    apellido,
                    correo,
                    dni,
                    telefono,
                    matricula,
                    rolSeleccionado.Id,
                    salasIds,
                    notaSala,
                    especialidadesIds,
                    nuevaContrasena);

                MessageBox.Show($"Usuario '{nombre} {apellido}' actualizado correctamente.",
                    "Modificación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 6. Recargamos la grilla y refrescamos el formulario con los nuevos datos
                CargarUsuariosDesdeBD();

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

            // 1. Inmunidad absoluta de la cuenta administradora principal
            if (txtCorreo.Text.Trim().Equals("admin@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("La cuenta administradora principal ('admin@gmail.com') posee inmunidad total y no puede ser desactivada ni eliminada del sistema.",
                    "Operación Denegada", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // 2. Prevención de auto-eliminación: un usuario no puede desactivar su propia cuenta en sesión activa
            if (_usuarioActual != null && _idUsuarioSeleccionado == _usuarioActual.IdUsuario)
            {
                MessageBox.Show("No puede desactivar su propia cuenta de usuario con la que tiene la sesión iniciada en este momento.",
                    "Operación Denegada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombreCompleto = $"{txtNombre.Text.Trim()} {txtApellido.Text.Trim()}";
            var resultado = MessageBox.Show($"¿Está seguro de que desea desactivar al usuario '{nombreCompleto}'?\nSus asignaciones de salas y especialidades se conservarán para futuras reactivaciones.",
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

        /// <summary>
        /// Evento disparado al presionar el botón 'Re-dar de Alta' (btnReactivar).
        /// Reactiva al usuario seleccionado en la base de datos (Activo = 1, FechaBaja = NULL)
        /// y actualiza los datos y asignaciones modificados en el formulario.
        /// </summary>
        private void btnReactivar_Click(object? sender, EventArgs e)
        {
            if (_idUsuarioSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar o verificar un usuario inactivo para reactivar.",
                    "Sin Selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validamos los campos (esAlta = false permite mantener la contraseña previa si no se tipea una nueva)
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
            string? nuevaContrasena = !string.IsNullOrWhiteSpace(txtContrasena.Text) ? txtContrasena.Text : null;

            List<int>? salasIds = esMedico ? new List<int>() : new List<int>();
            List<int>? especialidadesIds = esMedico ? new List<int>() : new List<int>();

            if (esMedico)
            {
                foreach (var item in clbSala.CheckedItems)
                    if (item is ItemConId sala) salasIds.Add(sala.Id);

                foreach (var item in clbEspecialidades.CheckedItems)
                    if (item is ItemConId esp) especialidadesIds.Add(esp.Id);
            }

            string nombreCompleto = $"{nombre} {apellido}";
            var confirmacion = MessageBox.Show(
                $"¿Está seguro de que desea reactivar y dar de alta nuevamente al usuario '{nombreCompleto}'?",
                "Confirmar Reactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                // Invocamos la Capa de Negocio para reactivar al usuario y persistir sus datos
                _usuarioBLL.ReactivarUsuario(
                    _idUsuarioSeleccionado,
                    nombre,
                    apellido,
                    correo,
                    dni,
                    telefono,
                    matricula,
                    rolSeleccionado.Id,
                    salasIds,
                    notaSala,
                    especialidadesIds,
                    nuevaContrasena);

                MessageBox.Show($"Usuario '{nombreCompleto}' reactivado exitosamente en el sistema.",
                    "Reactivación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refrescamos la grilla
                CargarUsuariosDesdeBD();

                // Re-verificamos con el DNI para posicionar la vista directamente en Modo Modificación activo
                txtDniBusqueda.Text = dni;
                EjecutarVerificacionDni();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo reactivar el usuario:\n" + ex.Message,
                    "Error al Reactivar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BloquearFormularioEnPaso1();
        }

        // ---------------------------------------------------------------------
        // Buscador de usuarios en tiempo real en la grilla
        // ---------------------------------------------------------------------

        /// <summary>
        /// Filtra en tiempo real los registros visibles del personal en el DataGridView
        /// según el texto ingresado (coincidencias en DNI, Nombre, Apellido, Rol, Correo, etc.).
        /// </summary>
        private void txtBuscarUsuario_TextChanged(object? sender, EventArgs e)
        {
            string filtro = txtBuscarUsuario.Text.Trim();

            // Quitamos la celda actual seleccionada para evitar que Windows Forms lance excepciones al ocultar la fila
            dgvPersonal.CurrentCell = null;

            foreach (DataGridViewRow fila in dgvPersonal.Rows)
            {
                if (fila.IsNewRow) continue;

                if (string.IsNullOrWhiteSpace(filtro))
                {
                    fila.Visible = true;
                    continue;
                }

                string dni = fila.Cells["Dni"].Value?.ToString() ?? "";
                string nombre = fila.Cells["Nombre"].Value?.ToString() ?? "";
                string apellido = fila.Cells["Apellido"].Value?.ToString() ?? "";
                string rol = fila.Cells["Rol"].Value?.ToString() ?? "";
                string correo = fila.Cells["Correo"].Value?.ToString() ?? "";
                string matricula = fila.Cells["NroMatricula"].Value?.ToString() ?? "";
                string especialidades = fila.Cells["Especialidades"].Value?.ToString() ?? "";
                string salas = fila.Cells["Salas"].Value?.ToString() ?? "";
                string estado = fila.Cells["Estado"].Value?.ToString() ?? "";

                bool coincide = dni.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                                nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                                apellido.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                                $"{nombre} {apellido}".Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                                rol.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                                correo.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                                matricula.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                                especialidades.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                                salas.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                                estado.Contains(filtro, StringComparison.OrdinalIgnoreCase);

                fila.Visible = coincide;
            }
        }

        /// <summary>
        /// Al presionar Enter dentro del buscador, selecciona y carga en el formulario al primer usuario visible que coincida.
        /// </summary>
        private void txtBuscarUsuario_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Suprime el sonido beep de Windows

                foreach (DataGridViewRow fila in dgvPersonal.Rows)
                {
                    if (fila.Visible)
                    {
                        fila.Selected = true;
                        string dni = fila.Cells["Dni"].Value?.ToString() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(dni))
                        {
                            txtDniBusqueda.Text = dni;
                            EjecutarVerificacionDni();
                        }
                        break;
                    }
                }
            }
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

            if (EsPersonalMedico() && clbEspecialidades.CheckedItems.Count == 0)
            {
                MessageBox.Show("Para el rol 'Personal Médico' debe asignar al menos una especialidad médica.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                clbEspecialidades.Focus();
                return false;
            }

            return true;
        }
    }
}
