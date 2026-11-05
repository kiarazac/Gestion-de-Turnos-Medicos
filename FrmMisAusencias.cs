using System;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Módulo del profesional médico para registrar y anular sus ausencias programadas (por día completo).
    /// Si ningún otro médico cubre la especialidad, la fecha queda bloqueada y recepción ve los turnos a reprogramar.
    /// </summary>
    public partial class FrmMisAusencias : Form
    {
        private readonly AusenciaBLL _ausenciaBLL = new AusenciaBLL();
        private readonly UsuarioLoginResult _usuarioActual;

        /// <summary>
        /// Inicializa el formulario para el médico autenticado.
        /// </summary>
        /// <param name="usuario">Contexto de sesión del médico.</param>
        public FrmMisAusencias(UsuarioLoginResult usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            this.Load += FrmMisAusencias_Load;
            btnRegistrar.Click += BtnRegistrar_Click;
            btnAnular.Click += BtnAnular_Click;
            dtpDesde.ValueChanged += (s, e) => { if (dtpHasta.Value.Date < dtpDesde.Value.Date) dtpHasta.Value = dtpDesde.Value; };
        }

        private void FrmMisAusencias_Load(object? sender, EventArgs e)
        {
            DateTime manana = DateTime.Today.AddDays(1);
            dtpDesde.MinDate = manana;
            dtpHasta.MinDate = manana;
            dtpDesde.Value = manana;
            dtpHasta.Value = manana;

            CargarAusencias();
        }

        private void CargarAusencias()
        {
            try
            {
                dgvAusencias.DataSource = _ausenciaBLL.ListarAusenciasPorMedico(_usuarioActual.IdUsuario);

                if (dgvAusencias.Columns.Contains(nameof(AusenciaMedicaDTO.IdAusencia)))
                    dgvAusencias.Columns[nameof(AusenciaMedicaDTO.IdAusencia)].Visible = false;

                if (dgvAusencias.Columns.Contains(nameof(AusenciaMedicaDTO.Fecha)))
                {
                    var colFecha = dgvAusencias.Columns[nameof(AusenciaMedicaDTO.Fecha)];
                    colFecha.HeaderText = "Día de ausencia";
                    colFecha.DefaultCellStyle.Format = "dddd dd/MM/yyyy";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudieron cargar sus ausencias:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date;
            int dias = (hasta - desde).Days + 1;

            var confirmacion = MessageBox.Show(
                $"¿Registrar {dias} día(s) de ausencia, del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy}?\n\n" +
                "Si ningún otro profesional cubre su especialidad, esas fechas quedarán bloqueadas para nuevos turnos.",
                "Confirmar Ausencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                var resultado = _ausenciaBLL.RegistrarAusencias(_usuarioActual.IdUsuario, desde, hasta, txtMotivo.Text);

                string mensaje = $"Se registraron {resultado.DiasRegistrados} día(s) de ausencia.";
                if (resultado.DiasRegistrados < dias)
                    mensaje += $"\n({dias - resultado.DiasRegistrados} día(s) ya estaban registrados.)";
                if (resultado.TurnosSinCobertura > 0)
                    mensaje += $"\n\n{resultado.TurnosSinCobertura} turno(s) reservado(s) quedaron sin profesional. " +
                               "Recepción los verá en 'Turnos a reprogramar' para contactar a los pacientes.";

                MessageBox.Show(mensaje, "Ausencia Registrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMotivo.Clear();
                CargarAusencias();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo registrar la ausencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAnular_Click(object? sender, EventArgs e)
        {
            if (dgvAusencias.CurrentRow?.DataBoundItem is not AusenciaMedicaDTO ausencia)
            {
                MessageBox.Show("Seleccione una ausencia de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Anular la ausencia del {ausencia.Fecha:dddd dd/MM/yyyy}?\n\nEsa fecha volverá a estar disponible para turnos.",
                "Anular Ausencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                _ausenciaBLL.AnularAusencia(ausencia.IdAusencia, _usuarioActual.IdUsuario);
                CargarAusencias();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo anular la ausencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
