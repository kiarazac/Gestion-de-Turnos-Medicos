using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmTurnoEmergencia : Form
    {
        // 1. Invocación exclusiva de la Capa de Negocio (BLL)
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly PacienteBLL _pacienteBLL = new PacienteBLL();

        // Estructura interna para relacionar el síntoma con su ID
        private sealed class ItemSintoma
        {
            public int IdSintoma { get; set; }
            public string Descripcion { get; set; } = string.Empty;
            public string Gravedad { get; set; } = string.Empty;
            public override string ToString() => Descripcion;
        }

        public FrmTurnoEmergencia()
        {
            InitializeComponent();

            this.Load += FrmTurnoEmergencia_Load;
            this.button1.Click += BtnGenerarTurno_Click;
        }

        private void FrmTurnoEmergencia_Load(object? sender, EventArgs e)
        {
            Lid_turno.Text = "# --";
            Ldescrip_turno_especialidad.Text = "Emergencia";

            CargarCatalogoSintomas();
        }

        /// <summary>
        /// Obtiene el catálogo de síntomas activos desde la Capa de Negocio (BLL) para el triage dinámico.
        /// </summary>
        private void CargarCatalogoSintomas()
        {
            try
            {
                // BLL delega a TurnoDAL -> sp_ObtenerSintomas
                var catalogo = _turnoBLL.ObtenerSintomas();

                if (catalogo != null && catalogo.Count > 0)
                {
                    checkedListBox1.Items.Clear();
                    checkedListBox2.Items.Clear();
                    checkedListBox3.Items.Clear();

                    foreach (var s in catalogo)
                    {
                        var item = new ItemSintoma
                        {
                            IdSintoma = s.IdSintoma,
                            Descripcion = s.Descripcion,
                            Gravedad = s.Gravedad
                        };

                        if (s.Gravedad.Equals("Alta", StringComparison.OrdinalIgnoreCase))
                            checkedListBox1.Items.Add(item);
                        else if (s.Gravedad.Equals("Media", StringComparison.OrdinalIgnoreCase))
                            checkedListBox2.Items.Add(item);
                        else
                            checkedListBox3.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar el catálogo dinámico de síntomas:\n" + ex.Message,
                    "Aviso de Triage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnGenerarTurno_Click(object? sender, EventArgs e)
        {
            if (!ValidarDatos())
                return;

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string dni = txtDNI.Text.Trim();
            string obraSocial = txtObraSocial.Text.Trim();

            // 1. Extraemos directamente una lista única con todos los IDs de síntomas seleccionados
            var seleccionados = ObtenerSintomasSeleccionados();
            List<int> idsSintomas = new List<int>();
            foreach (var item in seleccionados)
            {
                idsSintomas.Add(item.IdSintoma);
            }

            try
            {
                // 2. Registramos o recuperamos al paciente
                int idPaciente = _pacienteBLL.GuardarPaciente(nombre, apellido, dni, obraSocial);

                // 3. Recibimos el número de orden real devuelto por la BLL
                string nroOrdenFinal = _turnoBLL.CrearTurnoEmergenciaConSintomas(idPaciente, idsSintomas);

                string prioridad = CalcularPrioridadTexto();

                // 4. Actualizamos la interfaz gráfica con el código real (ej: E-008)
                MostrarTurnoGenerado(nroOrdenFinal, prioridad);

                MessageBox.Show(
                    $"¡Turno de urgencia generado con éxito!\n\n" +
                    $"Paciente: {apellido}, {nombre}\n" +
                    $"Prioridad Triage: {prioridad}",
                    "Turno Generado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LimpiarCampos();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al generar el turno de emergencia:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtDNI.Text) ||
                string.IsNullOrWhiteSpace(txtObraSocial.Text))
            {
                MessageBox.Show("Por favor, complete todos los datos del paciente (Nombre, Apellido, DNI y Obra Social).",
                    "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Validaciones.EsNombreValido(txtNombre.Text.Trim()))
            {
                MessageBox.Show("El Nombre contiene caracteres inválidos. Solo se admiten letras.",
                    "Nombre inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (!Validaciones.EsNombreValido(txtApellido.Text.Trim()))
            {
                MessageBox.Show("El Apellido contiene caracteres inválidos. Solo se admiten letras.",
                    "Apellido inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtDNI.Text.Trim(), @"^\d{7,8}$"))
            {
                MessageBox.Show("El DNI debe tener entre 7 y 8 números (sin puntos ni letras).",
                    "DNI inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDNI.Focus();
                return false;
            }

            bool tieneSintomasAlta = checkedListBox1.CheckedItems.Count > 0;
            bool tieneSintomasMedia = checkedListBox2.CheckedItems.Count > 0;
            bool tieneCondicionEspecial = checkedListBox3.CheckedItems.Count > 0;
            bool tieneOtro = checkBox1.Checked;

            if (!tieneSintomasAlta && !tieneSintomasMedia && !tieneCondicionEspecial && !tieneOtro)
            {
                MessageBox.Show("Debe seleccionar al menos un síntoma o condición del paciente para clasificar el turno.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private string CalcularPrioridadTexto()
        {
            if (checkedListBox1.CheckedItems.Count > 0)
                return "ALTA";
            if (checkedListBox2.CheckedItems.Count > 0 || checkedListBox3.CheckedItems.Count > 0)
                return "MEDIA";

            return "BAJA";
        }

        private List<ItemSintoma> ObtenerSintomasSeleccionados()
        {
            List<ItemSintoma> lista = new List<ItemSintoma>();

            foreach (var item in checkedListBox1.CheckedItems)
            {
                if (item is ItemSintoma s)
                    lista.Add(s);
            }

            foreach (var item in checkedListBox2.CheckedItems)
            {
                if (item is ItemSintoma s)
                    lista.Add(s);
            }

            foreach (var item in checkedListBox3.CheckedItems)
            {
                if (item is ItemSintoma s)
                    lista.Add(s);
            }

            return lista;
        }

        private void MostrarTurnoGenerado(string nroOrden, string prioridad)
        {
            Lid_turno.Text = $"# {nroOrden}";
            Ldescrip_turno_especialidad.Text = $"Prioridad ({prioridad})";

            switch (prioridad)
            {
                case "ALTA":
                    Ldescrip_turno_especialidad.ForeColor = Color.FromArgb(214, 39, 40);
                    break;
                case "MEDIA":
                    Ldescrip_turno_especialidad.ForeColor = Color.FromArgb(204, 102, 0);
                    break;
                case "BAJA":
                    Ldescrip_turno_especialidad.ForeColor = Color.FromArgb(46, 139, 87);
                    break;
                default:
                    Ldescrip_turno_especialidad.ForeColor = SystemColors.Highlight;
                    break;
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtDNI.Clear();
            txtObraSocial.Clear();

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, false);

            for (int i = 0; i < checkedListBox2.Items.Count; i++)
                checkedListBox2.SetItemChecked(i, false);

            for (int i = 0; i < checkedListBox3.Items.Count; i++)
                checkedListBox3.SetItemChecked(i, false);

            checkBox1.Checked = false;
            txtNombre.Focus();
        }
    }
}
