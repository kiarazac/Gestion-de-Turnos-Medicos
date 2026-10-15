using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Gestion_de_Turnos_Medicos.Servicios;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de panel de control y auditoría estratégica exclusiva para el perfil Gerente / Dueño de Negocio.
    /// Centraliza exactamente dos reportes fundamentales:
    /// 1. Recaudación del día / Auditoría de recepcionistas (por fecha de emisión, tipo de turno y coberturas).
    /// 2. Ranking oficial de médicos por atenciones y consultas efectivamente realizadas.
    /// </summary>
    public partial class FrmReportesGerente : Form
    {
        private readonly ReporteBLL _reporteBLL = new ReporteBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        private ReporteRecaudacionAuditoriaResumenDTO _resumen = new ReporteRecaudacionAuditoriaResumenDTO();
        private List<ReporteRecaudacionAuditoriaDetalleDTO> _detalles = new List<ReporteRecaudacionAuditoriaDetalleDTO>();
        private List<ReporteRankingMedicoDTO> _ranking = new List<ReporteRankingMedicoDTO>();

        public FrmReportesGerente() : this(null)
        {
        }

        public FrmReportesGerente(UsuarioLoginResult? usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            this.Load += FrmReportesGerente_Load;
            this.cmbPeriodo.SelectedIndexChanged += CmbPeriodo_SelectedIndexChanged;
            this.btnFiltrar.Click += BtnFiltrar_Click;
            this.btnExportar.Click += BtnExportar_Click;
        }

        private void FrmReportesGerente_Load(object? sender, EventArgs e)
        {
            ConfigurarGrillas();
            cmbPeriodo.SelectedIndex = 0; // "Hoy" por defecto para auditar la recaudación del día
            CargarDatos();
        }

        private void ConfigurarGrillas()
        {
            // 1. Grilla de Recaudación y Auditoría de Recepción (Emisión en Caja)
            dgvRecaudacionAuditoria.AutoGenerateColumns = false;
            dgvRecaudacionAuditoria.Columns.Clear();
            dgvRecaudacionAuditoria.RowHeadersVisible = false;
            dgvRecaudacionAuditoria.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecaudacionAuditoria.ReadOnly = true;
            dgvRecaudacionAuditoria.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNroOrden",
                HeaderText = "N° Orden",
                DataPropertyName = "NroOrden",
                Width = 75,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F, FontStyle.Bold) }
            });
            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFechaEmision",
                HeaderText = "Fecha / Hora Emisión",
                DataPropertyName = "FechaEmision",
                Width = 145,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm", Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaciente",
                HeaderText = "Paciente",
                DataPropertyName = "Paciente",
                Width = 160
            });
            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDni",
                HeaderText = "DNI",
                DataPropertyName = "DniPaciente",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipoTurno",
                HeaderText = "Tipo Turno",
                DataPropertyName = "TipoTurno",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEspecialidad",
                HeaderText = "Especialidad",
                DataPropertyName = "Especialidad",
                Width = 130
            });
            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colObraSocial",
                HeaderText = "Obra Social / Prepaga",
                DataPropertyName = "ObraSocial",
                Width = 150
            });
            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipoCobertura",
                HeaderText = "Cobertura",
                DataPropertyName = "TipoCobertura",
                Width = 105,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMonto",
                HeaderText = "Monto Cobrado",
                DataPropertyName = "MontoCobrado",
                Width = 115,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(22, 101, 52) }
            });
            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstado",
                HeaderText = "Estado Turno",
                DataPropertyName = "EstadoTurno",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRecaudacionAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMedicoAsignado",
                HeaderText = "Médico Tratante",
                DataPropertyName = "MedicoAsignado",
                Width = 150
            });

            // 2. Grilla de Ranking de Médicos por Atenciones Realizadas
            dgvRankingMedicos.AutoGenerateColumns = false;
            dgvRankingMedicos.Columns.Clear();
            dgvRankingMedicos.RowHeadersVisible = false;
            dgvRankingMedicos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRankingMedicos.ReadOnly = true;
            dgvRankingMedicos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvRankingMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPosicion",
                HeaderText = "# Posición",
                DataPropertyName = "Posicion",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(15, 118, 110) }
            });
            dgvRankingMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombreMedico",
                HeaderText = "Profesional Médico",
                DataPropertyName = "NombreMedico",
                Width = 220,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9F, FontStyle.Bold) }
            });
            dgvRankingMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEspecialidadRanking",
                HeaderText = "Especialidades Asignadas",
                DataPropertyName = "Especialidad",
                Width = 200
            });
            dgvRankingMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMatricula",
                HeaderText = "Matrícula",
                DataPropertyName = "Matricula",
                Width = 95,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRankingMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAtenciones",
                HeaderText = "Atenciones Realizadas",
                DataPropertyName = "AtencionesRealizadas",
                Width = 145,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(15, 118, 110) }
            });
            dgvRankingMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTotalRecaudado",
                HeaderText = "Facturación Total",
                DataPropertyName = "TotalRecaudado",
                Width = 135,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvRankingMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPorcentaje",
                HeaderText = "% Participación",
                DataPropertyName = "PorcentajeParticipacion",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N1", Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9F, FontStyle.Bold) }
            });
        }

        private void CmbPeriodo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            DateTime hoy = DateTime.Today;

            switch (cmbPeriodo.SelectedIndex)
            {
                case 0: // Hoy
                    dtpDesde.Value = hoy;
                    dtpHasta.Value = hoy;
                    break;
                case 1: // Últimos 7 días
                    dtpDesde.Value = hoy.AddDays(-6);
                    dtpHasta.Value = hoy;
                    break;
                case 2: // Mes actual
                    dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
                    dtpHasta.Value = hoy;
                    break;
                case 3: // Mes anterior
                    var mesAnt = hoy.AddMonths(-1);
                    dtpDesde.Value = new DateTime(mesAnt.Year, mesAnt.Month, 1);
                    dtpHasta.Value = new DateTime(mesAnt.Year, mesAnt.Month, DateTime.DaysInMonth(mesAnt.Year, mesAnt.Month));
                    break;
                case 4: // Histórico / Todo
                    dtpDesde.Value = new DateTime(2020, 1, 1);
                    dtpHasta.Value = hoy;
                    break;
            }
        }

        private void BtnFiltrar_Click(object? sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date;

            if (desde > hasta)
            {
                MessageBox.Show("La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.",
                    "Rango Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;

                // 1. Auditoría de Recaudación en base al día de emisión del turno (Caja / Recepcionistas)
                _resumen = _reporteBLL.ObtenerAuditoriaRecaudacionResumen(desde, hasta);
                _detalles = _reporteBLL.ObtenerAuditoriaRecaudacionDetalle(desde, hasta);

                // 2. Ranking de Médicos en base a las atenciones efectivamente realizadas
                _ranking = _reporteBLL.ObtenerRankingMedicos(desde, hasta);

                // Enlazar grillas
                dgvRecaudacionAuditoria.DataSource = null;
                dgvRecaudacionAuditoria.DataSource = _detalles;

                dgvRankingMedicos.DataSource = null;
                dgvRankingMedicos.DataSource = _ranking;

                // Actualizar tarjetas superiores de KPI
                ActualizarKpis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al cargar la información de auditoría:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ActualizarKpis()
        {
            lblKpiConsultas.Text = $"{_resumen.TotalTurnos} tickets";
            lblKpiFacturacion.Text = $"$ {_resumen.TotalRecaudado:N2}";
            lblKpiObrasSociales.Text = $"OS: ${_resumen.MontoObraSocial:N0} | Part: ${_resumen.MontoParticulares:N0}";
            lblKpiParticulares.Text = $"Emg: ${_resumen.MontoEmergencia:N0} | Esp: ${_resumen.MontoEspecialidad:N0}";
        }

        private void BtnExportar_Click(object? sender, EventArgs e)
        {
            if (_detalles.Count == 0 && _ranking.Count == 0)
            {
                MessageBox.Show("No hay datos disponibles para exportar en el rango de fechas seleccionado.",
                    "Exportar Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
                sfd.Filter = "Documento Oficial PDF (*.pdf)|*.pdf";
                sfd.FilterIndex = 1;
                sfd.FileName = $"Reporte_Gerencial_Recaudacion_y_Ranking_{timestamp}.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;

                        ExportadorPdf.ExportarReporteGerencial(
                            sfd.FileName,
                            _resumen,
                            _detalles,
                            _ranking,
                            dtpDesde.Value.Date,
                            dtpHasta.Value.Date,
                            _usuarioActual);

                        var resp = MessageBox.Show(
                            $"Reporte oficial gerencial generado con éxito en:\n{sfd.FileName}\n\n¿Desea abrir el documento PDF ahora?",
                            "Reporte Gerencial Emitido",
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
                                // Si el visor predeterminado no pudo abrirse automáticamente
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al generar el reporte PDF:\n" + ex.Message,
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
