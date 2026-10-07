using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.CapaNegocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Gestion_de_Turnos_Medicos.Servicios;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de cierre diario de caja y control financiero para el perfil Recepcionista.
    /// Permite auditar en tiempo real la cantidad de turnos emitidos (emergencias y especialidades),
    /// la recaudación efectiva en ventanilla y exportar el acta oficial inmutable en formato PDF.
    /// </summary>
    public partial class FrmCierreCaja : Form
    {
        private readonly CajaBLL _cajaBLL = new CajaBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        private ReporteCierreCajaResumenDTO _resumen = new ReporteCierreCajaResumenDTO();
        private List<ReporteCierreCajaDetalleDTO> _detalles = new List<ReporteCierreCajaDetalleDTO>();

        public FrmCierreCaja() : this(null)
        {
        }

        public FrmCierreCaja(UsuarioLoginResult? usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            this.Load += FrmCierreCaja_Load;
            this.btnConsultar.Click += BtnConsultar_Click;
            this.btnExportarPdf.Click += BtnExportarPdf_Click;
        }

        private void FrmCierreCaja_Load(object? sender, EventArgs e)
        {
            ConfigurarGrilla();
            dtpFechaCaja.Value = DateTime.Today;
            CargarCierre();
        }

        private void ConfigurarGrilla()
        {
            dgvTurnosCaja.AutoGenerateColumns = false;
            dgvTurnosCaja.Columns.Clear();
            dgvTurnosCaja.RowHeadersVisible = false;
            dgvTurnosCaja.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTurnosCaja.ReadOnly = true;
            dgvTurnosCaja.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvTurnosCaja.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNroOrden",
                HeaderText = "N° Turno",
                DataPropertyName = "NroOrden",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvTurnosCaja.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHorario",
                HeaderText = "Horario",
                DataPropertyName = "Horario",
                Width = 75,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = @"hh\:mm" }
            });

            dgvTurnosCaja.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaciente",
                HeaderText = "Paciente",
                DataPropertyName = "PacienteCompleto",
                Width = 190
            });

            dgvTurnosCaja.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDni",
                HeaderText = "DNI",
                DataPropertyName = "DniPaciente",
                Width = 90
            });

            dgvTurnosCaja.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colObraSocial",
                HeaderText = "Cobertura / Obra Social",
                DataPropertyName = "ObraSocial",
                Width = 160
            });

            dgvTurnosCaja.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipo",
                HeaderText = "Tipo Turno",
                DataPropertyName = "TipoTurno",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvTurnosCaja.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEspecialidad",
                HeaderText = "Especialidad / Sala",
                DataPropertyName = "Especialidad",
                Width = 140
            });

            dgvTurnosCaja.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMedico",
                HeaderText = "Profesional Asignado",
                DataPropertyName = "MedicoAsignado",
                Width = 160
            });

            dgvTurnosCaja.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMonto",
                HeaderText = "Monto Abonado",
                DataPropertyName = "MontoCobrado",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(22, 101, 52)
                }
            });

            dgvTurnosCaja.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstado",
                HeaderText = "Estado",
                DataPropertyName = "Estado",
                Width = 95,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
        }

        private void BtnConsultar_Click(object? sender, EventArgs e)
        {
            CargarCierre();
        }

        private void CargarCierre()
        {
            DateTime fechaSeleccionada = dtpFechaCaja.Value.Date;

            try
            {
                Cursor = Cursors.WaitCursor;

                _resumen = _cajaBLL.ObtenerResumenCierreCaja(fechaSeleccionada);
                _detalles = _cajaBLL.ObtenerDetalleCierreCaja(fechaSeleccionada);

                dgvTurnosCaja.DataSource = null;
                dgvTurnosCaja.DataSource = _detalles;

                ActualizarKpis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al consultar el cierre de caja:\n" + ex.Message,
                    "Error de Cierre", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ActualizarKpis()
        {
            decimal totalRecaudadoCaja = _resumen.TotalRecaudado;
            decimal coberturaOS = _resumen.TurnosObraSocial > 0 ? Math.Round(_resumen.MontoObraSocial * (70m / 30m), 2) : 0m;
            decimal facturacionTotal = _resumen.MontoParticulares + (_resumen.TurnosObraSocial > 0 ? (_resumen.MontoObraSocial + coberturaOS) : 0m);

            lblKpiTurnos.Text = _resumen.TotalTurnos.ToString();
            lblSubKpiTurnos.Text = $"Esp: {_resumen.TurnosEspecialidad} | Urg: {_resumen.TurnosEmergencia}";

            lblKpiRecaudacion.Text = $"$ {totalRecaudadoCaja:N2}";
            lblSubKpiRecaudacion.Text = $"Part: $ {_resumen.MontoParticulares:N2} | O.S.: $ {_resumen.MontoObraSocial:N2}";

            lblKpiCobertura.Text = $"$ {coberturaOS:N2}";
            lblSubKpiCobertura.Text = $"70% cubierto por { _resumen.TurnosObraSocial} pacientes O.S.";

            lblKpiFacturacion.Text = $"$ {facturacionTotal:N2}";
            decimal promedio = _resumen.TotalTurnos > 0 ? Math.Round(totalRecaudadoCaja / _resumen.TotalTurnos, 2) : 0m;
            lblSubKpiFacturacion.Text = $"Promedio Caja: $ {promedio:N2} / turno";
        }

        private void BtnExportarPdf_Click(object? sender, EventArgs e)
        {
            if (_detalles.Count == 0 && _resumen.TotalTurnos == 0)
            {
                MessageBox.Show("No hay movimientos registrados en la fecha seleccionada para generar el cierre de caja.",
                    "Sin Movimientos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string fechaStr = dtpFechaCaja.Value.ToString("yyyyMMdd");
                sfd.Filter = "Documento Oficial PDF (*.pdf)|*.pdf";
                sfd.FilterIndex = 1;
                sfd.FileName = $"Cierre_Caja_Oficial_{fechaStr}.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;

                        ExportadorPdf.ExportarCierreCaja(
                            sfd.FileName,
                            _resumen,
                            _detalles,
                            dtpFechaCaja.Value.Date,
                            _usuarioActual);

                        var resp = MessageBox.Show(
                            $"Cierre diario de caja exportado exitosamente en:\n{sfd.FileName}\n\n¿Desea abrir el comprobante PDF oficial?",
                            "Cierre de Caja Generado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (resp == DialogResult.Yes)
                        {
                            try
                            {
                                Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                            }
                            catch
                            {
                                // Silencioso si no se pudo disparar el visor predeterminado del sistema operativo
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar el cierre de caja en PDF:\n" + ex.Message,
                            "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
}
