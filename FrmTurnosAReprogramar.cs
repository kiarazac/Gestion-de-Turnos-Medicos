using System;
using System.Linq;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Lista para recepción de los turnos de especialidad que quedaron sin profesional disponible (ausencias, bajas
    /// o especialidad sin médicos). Permite copiar un mensaje para avisar al paciente, cancelar con la clave 2FA
    /// del comprobante y volver a <see cref="FrmTurnoEspecialidad"/> con los datos cargados para reprogramar.
    /// </summary>
    public partial class FrmTurnosAReprogramar : Form
    {
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();

        /// <summary>Turno cancelado que debe reprogramarse (disponible cuando el diálogo cierra con <see cref="DialogResult.OK"/>).</summary>
        public TurnoAReprogramarDTO? TurnoAReprogramar { get; private set; }

        public FrmTurnosAReprogramar()
        {
            InitializeComponent();

            this.Load += (s, e) => CargarTurnos();
            btnCopiarMensaje.Click += BtnCopiarMensaje_Click;
            btnCancelarYReprogramar.Click += (s, e) => CancelarSeleccionado(reprogramar: true);
            btnSoloCancelar.Click += (s, e) => CancelarSeleccionado(reprogramar: false);
        }

        private void CargarTurnos()
        {
            try
            {
                var turnos = _turnoBLL.ListarTurnosAReprogramar();
                dgvTurnos.DataSource = turnos;

                dgvTurnos.Columns[nameof(TurnoAReprogramarDTO.IdTurno)].Visible = false;
                dgvTurnos.Columns[nameof(TurnoAReprogramarDTO.NroOrden)].HeaderText = "Nº";
                dgvTurnos.Columns[nameof(TurnoAReprogramarDTO.Fecha)].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvTurnos.Columns[nameof(TurnoAReprogramarDTO.Horario)].HeaderText = "Hora";
                dgvTurnos.Columns[nameof(TurnoAReprogramarDTO.Dni)].HeaderText = "DNI";
                dgvTurnos.Columns[nameof(TurnoAReprogramarDTO.Telefono)].HeaderText = "Teléfono";
                dgvTurnos.Columns[nameof(TurnoAReprogramarDTO.ObraSocial)].HeaderText = "Obra social";
                dgvTurnos.Columns[nameof(TurnoAReprogramarDTO.ProximoHorarioLibre)].HeaderText = "Próximo horario libre";

                lblTitulo.Text = $"⚠ Turnos sin profesional disponible ({turnos.Count})";
                bool hayTurnos = turnos.Count > 0;
                btnCopiarMensaje.Enabled = hayTurnos;
                btnCancelarYReprogramar.Enabled = hayTurnos;
                btnSoloCancelar.Enabled = hayTurnos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudieron cargar los turnos a reprogramar:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TurnoAReprogramarDTO? TurnoSeleccionado()
        {
            if (dgvTurnos.CurrentRow?.DataBoundItem is TurnoAReprogramarDTO turno)
                return turno;

            MessageBox.Show("Seleccione un turno de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        /// <summary>
        /// Copia al portapapeles un mensaje listo para enviar al paciente (WhatsApp, SMS) o leer por teléfono.
        /// </summary>
        private void BtnCopiarMensaje_Click(object? sender, EventArgs e)
        {
            if (TurnoSeleccionado() is not { } t)
                return;

            string nombre = t.Paciente?.Split(", ").Last() ?? string.Empty;
            string oferta = string.IsNullOrWhiteSpace(t.ProximoHorarioLibre)
                ? "Nos comunicaremos para ofrecerle una nueva fecha."
                : $"Le ofrecemos el {t.ProximoHorarioLibre}.";

            string mensaje =
                $"Hola {nombre}, le informamos que su turno de {t.Especialidad} del {t.Fecha:dd/MM/yyyy} a las {t.Horario} hs " +
                $"debe reprogramarse por ausencia del profesional. {oferta} " +
                "Para confirmar necesitaremos el código de cancelación que figura en su comprobante.";

            Clipboard.SetText(mensaje);
            MessageBox.Show($"Mensaje copiado al portapapeles.\n\nTeléfono del paciente: {t.Telefono ?? "no registrado"}\n\n{mensaje}",
                "Mensaje Copiado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Cancela el turno seleccionado con la clave 2FA (validada por el servidor). Si <paramref name="reprogramar"/> es
        /// <c>true</c>, cierra el diálogo devolviendo el turno para precargar el formulario de turnos de especialidad.
        /// </summary>
        private void CancelarSeleccionado(bool reprogramar)
        {
            if (TurnoSeleccionado() is not { } t)
                return;

            string clave = txtClave.Text.Trim();
            if (string.IsNullOrWhiteSpace(clave))
            {
                MessageBox.Show("Ingrese la clave de cancelación (2FA) que figura en el comprobante del paciente.", "Clave 2FA Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClave.Focus();
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Confirma la cancelación del turno #{t.NroOrden} de {t.Paciente} ({t.Especialidad}, {t.Fecha:dd/MM/yyyy} {t.Horario} hs)?" +
                (reprogramar ? "\n\nLuego se abrirá la carga del nuevo turno con los datos del paciente." : string.Empty),
                "Confirmar Cancelación (2FA)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                _turnoBLL.CancelarTurnoEspecialidad(t.IdTurno, clave);
                txtClave.Clear();

                if (reprogramar)
                {
                    TurnoAReprogramar = t;
                    DialogResult = DialogResult.OK;
                    return;
                }

                MessageBox.Show($"El turno #{t.NroOrden} fue cancelado.", "Cancelación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarTurnos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo cancelar el turno:\n{ex.Message}", "Error de Cancelación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClave.Focus();
                txtClave.SelectAll();
            }
        }
    }
}
