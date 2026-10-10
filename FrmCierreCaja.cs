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
    /// Formulario de reportería y control financiero para el perfil Recepcionista.
    /// Incorpora dos apartados:
    /// 1. Turnos del Día de la Fecha (auditoría operativa y conciliación financiera en tiempo real).
    /// 2. Búsqueda por Fecha y Especialidad (consulta histórica y discriminada por especialidad).
    /// Permite exportar informes oficiales inmutables en formato PDF con el estándar institucional.
    /// </summary>
    public partial class FrmCierreCaja : Form
    {
        private readonly CajaBLL _cajaBLL = new CajaBLL();
        private readonly EspecialidadBLL _especialidadBLL = new EspecialidadBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        private ReporteCierreCajaResumenDTO _resumenHoy = new ReporteCierreCajaResumenDTO();
        private List<ReporteCierreCajaDetalleDTO> _detallesHoy = new List<ReporteCierreCajaDetalleDTO>();

        private ReporteCierreCajaResumenDTO _resumenFiltro = new ReporteCierreCajaResumenDTO();
        private List<ReporteCierreCajaDetalleDTO> _detallesFiltro = new List<ReporteCierreCajaDetalleDTO>();

        public FrmCierreCaja() : this(null)
        {
        }

        public FrmCierreCaja(UsuarioLoginResult? usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            this.Load += FrmCierreCaja_Load;

            // Eventos Pestaña 1: Turnos de Hoy
            this.btnActualizarHoy.Click += (s, e) => CargarTurnosHoy();
            this.btnExportarPdfHoy.Click += BtnExportarPdfHoy_Click;

            // Eventos Pestaña 2: Búsqueda Histórica
            this.btnConsultarFiltro.Click += (s, e) => CargarTurnosFiltro();
            this.btnExportarPdfFiltro.Click += BtnExportarPdfFiltro_Click;
        }

        private void FrmCierreCaja_Load(object? sender, EventArgs e)
        {
            ConfigurarGrilla(dgvTurnosHoy);
            ConfigurarGrilla(dgvTurnosFiltro);

            lblHoyFechaValor.Text = DateTime.Today.ToString("dd/MM/yyyy");
            dtpFechaFiltro.Value = DateTime.Today;

            CargarComboEspecialidades();

            CargarTurnosHoy();
            CargarTurnosFiltro();
        }

        private void ConfigurarGrilla(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = true;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNroOrden",
                HeaderText = "N° Turno",
                DataPropertyName = "NroOrden",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFechaProgramada",
                HeaderText = "Fecha Turno",
                DataPropertyName = "Fecha",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "dd/MM/yyyy" }
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHorario",
                HeaderText = "Horario",
                DataPropertyName = "Horario",
                Width = 75,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = @"hh\:mm" }
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaciente",
                HeaderText = "Paciente",
                DataPropertyName = "PacienteCompleto",
                Width = 190
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDni",
                HeaderText = "DNI",
                DataPropertyName = "DniPaciente",
                Width = 90
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colObraSocial",
                HeaderText = "Cobertura / Obra Social",
                DataPropertyName = "ObraSocial",
                Width = 160
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipo",
                HeaderText = "Tipo Turno",
                DataPropertyName = "TipoTurno",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEspecialidad",
                HeaderText = "Especialidad / Sala",
                DataPropertyName = "Especialidad",
                Width = 140
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMedico",
                HeaderText = "Profesional Asignado",
                DataPropertyName = "MedicoAsignado",
                Width = 160
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
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

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstado",
                HeaderText = "Estado",
                DataPropertyName = "Estado",
                Width = 95,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
        }

        private void CargarComboEspecialidades()
        {
            cmbEspecialidadFiltro.Items.Clear();
            cmbEspecialidadFiltro.Items.Add("Todas las Especialidades");
            cmbEspecialidadFiltro.Items.Add("Emergencia");

            try
            {
                var lista = _especialidadBLL.ObtenerEspecialidades();
                if (lista != null)
                {
                    foreach (var esp in lista)
                    {
                        if (!string.IsNullOrWhiteSpace(esp.Nombre) &&
                            !esp.Nombre.StartsWith("Emergenc", StringComparison.OrdinalIgnoreCase))
                        {
                            cmbEspecialidadFiltro.Items.Add(esp.Nombre);
                        }
                    }
                }
            }
            catch
            {
                // Fallback silencioso si no se pudo acceder al catálogo de especialidades
            }

            cmbEspecialidadFiltro.SelectedIndex = 0;
        }

        #region Apartado 1: Turnos de la Jornada Actual (Hoy)

        private void CargarTurnosHoy()
        {
            DateTime fechaHoy = DateTime.Today;

            try
            {
                Cursor = Cursors.WaitCursor;

                // Carga todos los turnos emitidos en la fecha actual (independientemente del día para el que fueron programados o modalidad)
                _detallesHoy = _cajaBLL.ObtenerDetalleCierreCaja(fechaHoy, especialidad: null, soloEmitidosHoy: true);
                _resumenHoy = _cajaBLL.CalcularResumenDeDetalles(fechaHoy, _detallesHoy);

                dgvTurnosHoy.DataSource = null;
                dgvTurnosHoy.DataSource = _detallesHoy;

                ActualizarKpisHoy();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al consultar los turnos de la jornada actual:\n" + ex.Message,
                    "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ActualizarKpisHoy()
        {
            decimal totalRecaudadoCaja = _resumenHoy.TotalRecaudado;
            decimal coberturaOS = _resumenHoy.TurnosObraSocial > 0 ? Math.Round(_resumenHoy.MontoObraSocial * (70m / 30m), 2) : 0m;
            decimal facturacionTotal = _resumenHoy.MontoParticulares + (_resumenHoy.TurnosObraSocial > 0 ? (_resumenHoy.MontoObraSocial + coberturaOS) : 0m);

            lblKpiTurnosHoy.Text = _resumenHoy.TotalTurnos.ToString();
            lblSubKpiTurnosHoy.Text = $"Esp: {_resumenHoy.TurnosEspecialidad} | Urg: {_resumenHoy.TurnosEmergencia}";

            lblKpiRecaudacionHoy.Text = $"$ {totalRecaudadoCaja:N2}";
            lblSubKpiRecaudacionHoy.Text = $"Part: $ {_resumenHoy.MontoParticulares:N2} | O.S.: $ {_resumenHoy.MontoObraSocial:N2}";

            lblKpiCoberturaHoy.Text = $"$ {coberturaOS:N2}";
            lblSubKpiCoberturaHoy.Text = $"70% cubierto por {_resumenHoy.TurnosObraSocial} pacientes O.S.";

            lblKpiFacturacionHoy.Text = $"$ {facturacionTotal:N2}";
            decimal promedio = _resumenHoy.TotalTurnos > 0 ? Math.Round(totalRecaudadoCaja / _resumenHoy.TotalTurnos, 2) : 0m;
            lblSubKpiFacturacionHoy.Text = $"Promedio Caja: $ {promedio:N2} / turno";
        }

        private void BtnExportarPdfHoy_Click(object? sender, EventArgs e)
        {
            if (_detallesHoy.Count == 0 && _resumenHoy.TotalTurnos == 0)
            {
                MessageBox.Show("No hay turnos registrados emitidos en la jornada de hoy para generar el reporte.",
                    "Sin Movimientos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string fechaStr = DateTime.Today.ToString("yyyyMMdd");
                sfd.Filter = "Documento Oficial PDF (*.pdf)|*.pdf";
                sfd.FilterIndex = 1;
                sfd.FileName = $"Reporte_Turnos_Emitidos_{fechaStr}.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;

                        ExportadorPdf.ExportarCierreCaja(
                            sfd.FileName,
                            _resumenHoy,
                            _detallesHoy,
                            DateTime.Today,
                            _usuarioActual,
                            null,
                            "REPORTE OFICIAL DE TURNOS EMITIDOS EN EL DÍA");

                        var resp = MessageBox.Show(
                            $"Reporte del día exportado exitosamente en:\n{sfd.FileName}\n\n¿Desea abrir el comprobante PDF oficial?",
                            "Reporte Generado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (resp == DialogResult.Yes)
                        {
                            try
                            {
                                Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                            }
                            catch { }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar el reporte en PDF:\n" + ex.Message,
                            "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        #endregion

        #region Apartado 2: Búsqueda por Fecha y Especialidad

        private void CargarTurnosFiltro()
        {
            DateTime fechaSeleccionada = dtpFechaFiltro.Value.Date;
            string? espSeleccionada = cmbEspecialidadFiltro.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(espSeleccionada) ||
                espSeleccionada.Equals("Todas las Especialidades", StringComparison.OrdinalIgnoreCase))
            {
                espSeleccionada = null;
            }

            try
            {
                Cursor = Cursors.WaitCursor;

                _detallesFiltro = _cajaBLL.ObtenerDetalleCierreCaja(fechaSeleccionada, espSeleccionada);
                _resumenFiltro = _cajaBLL.CalcularResumenDeDetalles(fechaSeleccionada, _detallesFiltro);

                dgvTurnosFiltro.DataSource = null;
                dgvTurnosFiltro.DataSource = _detallesFiltro;

                ActualizarKpisFiltro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al consultar los turnos filtrados:\n" + ex.Message,
                    "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ActualizarKpisFiltro()
        {
            decimal totalRecaudadoCaja = _resumenFiltro.TotalRecaudado;
            decimal coberturaOS = _resumenFiltro.TurnosObraSocial > 0 ? Math.Round(_resumenFiltro.MontoObraSocial * (70m / 30m), 2) : 0m;
            decimal facturacionTotal = _resumenFiltro.MontoParticulares + (_resumenFiltro.TurnosObraSocial > 0 ? (_resumenFiltro.MontoObraSocial + coberturaOS) : 0m);

            lblKpiTurnosFiltro.Text = _resumenFiltro.TotalTurnos.ToString();
            lblSubKpiTurnosFiltro.Text = $"Esp: {_resumenFiltro.TurnosEspecialidad} | Urg: {_resumenFiltro.TurnosEmergencia}";

            lblKpiRecaudacionFiltro.Text = $"$ {totalRecaudadoCaja:N2}";
            lblSubKpiRecaudacionFiltro.Text = $"Part: $ {_resumenFiltro.MontoParticulares:N2} | O.S.: $ {_resumenFiltro.MontoObraSocial:N2}";

            lblKpiCoberturaFiltro.Text = $"$ {coberturaOS:N2}";
            lblSubKpiCoberturaFiltro.Text = $"70% cubierto por {_resumenFiltro.TurnosObraSocial} pacientes O.S.";

            lblKpiFacturacionFiltro.Text = $"$ {facturacionTotal:N2}";
            decimal promedio = _resumenFiltro.TotalTurnos > 0 ? Math.Round(totalRecaudadoCaja / _resumenFiltro.TotalTurnos, 2) : 0m;
            lblSubKpiFacturacionFiltro.Text = $"Promedio: $ {promedio:N2} / turno";
        }

        private void BtnExportarPdfFiltro_Click(object? sender, EventArgs e)
        {
            if (_detallesFiltro.Count == 0 && _resumenFiltro.TotalTurnos == 0)
            {
                MessageBox.Show("No se encontraron turnos con los filtros seleccionados para generar el reporte.",
                    "Sin Resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DateTime fecha = dtpFechaFiltro.Value.Date;
            string? esp = cmbEspecialidadFiltro.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(esp) || esp.Equals("Todas las Especialidades", StringComparison.OrdinalIgnoreCase))
            {
                esp = null;
            }

            string espSanitizada = !string.IsNullOrWhiteSpace(esp) ? esp.Replace(" ", "_") : "General";

            using (var sfd = new SaveFileDialog())
            {
                string fechaStr = fecha.ToString("yyyyMMdd");
                sfd.Filter = "Documento Oficial PDF (*.pdf)|*.pdf";
                sfd.FilterIndex = 1;
                sfd.FileName = $"Reporte_Turnos_{fechaStr}_{espSanitizada}.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;

                        string titulo = !string.IsNullOrWhiteSpace(esp)
                            ? $"REPORTE OFICIAL DE TURNOS Y FACTURACIÓN — {esp.ToUpperInvariant()}"
                            : "REPORTE OFICIAL DE TURNOS Y FACTURACIÓN POR FECHA";

                        ExportadorPdf.ExportarCierreCaja(
                            sfd.FileName,
                            _resumenFiltro,
                            _detallesFiltro,
                            fecha,
                            _usuarioActual,
                            esp,
                            titulo);

                        var resp = MessageBox.Show(
                            $"Reporte exportado exitosamente en:\n{sfd.FileName}\n\n¿Desea abrir el comprobante PDF oficial?",
                            "Reporte Generado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (resp == DialogResult.Yes)
                        {
                            try
                            {
                                Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                            }
                            catch { }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar el reporte en PDF:\n" + ex.Message,
                            "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        #endregion
    }
}
