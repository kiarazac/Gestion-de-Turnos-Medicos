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
    /// Formulario de panel de control y reportería estratégica para el perfil Gerente / Dueño de Negocio.
    /// Centraliza la auditoría financiera: facturación por médico, obras sociales vs particulares,
    /// ranking de demanda y exportación exclusiva a formato PDF oficial inmutable.
    /// </summary>
    public partial class FrmReportesGerente : Form
    {
        private readonly ReporteBLL _reporteBLL = new ReporteBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        private List<ReporteIngresoMedicoDTO> _ingresos = new List<ReporteIngresoMedicoDTO>();
        private List<ReporteObraSocialVsParticularDTO> _coberturas = new List<ReporteObraSocialVsParticularDTO>();
        private List<ReporteDemandaMedicoRankingDTO> _demanda = new List<ReporteDemandaMedicoRankingDTO>();

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
            cmbPeriodo.SelectedIndex = 2; // "Mes actual"
            CargarDatos();
        }

        private void ConfigurarGrillas()
        {
            // 1. Grilla de Ingresos por Médico
            dgvIngresos.AutoGenerateColumns = false;
            dgvIngresos.Columns.Clear();
            dgvIngresos.RowHeadersVisible = false;
            dgvIngresos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvIngresos.ReadOnly = true;
            dgvIngresos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvIngresos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMedico",
                HeaderText = "Profesional Médico",
                DataPropertyName = "NombreMedico",
                Width = 190
            });
            dgvIngresos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEspecialidad",
                HeaderText = "Especialidad",
                DataPropertyName = "Especialidad",
                Width = 140
            });
            dgvIngresos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMatricula",
                HeaderText = "Matrícula",
                DataPropertyName = "Matricula",
                Width = 90
            });
            dgvIngresos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colConsultas",
                HeaderText = "Consultas",
                DataPropertyName = "ConsultasAtendidas",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvIngresos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFacturacion",
                HeaderText = "Facturación Total",
                DataPropertyName = "IngresosTotales",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvIngresos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTicket",
                HeaderText = "Ticket Promedio",
                DataPropertyName = "TicketPromedio",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvIngresos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAporte",
                HeaderText = "% Aporte",
                DataPropertyName = "PorcentajeAporte",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N1", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // 2. Grilla de Obras Sociales vs Particulares
            dgvCoberturas.AutoGenerateColumns = false;
            dgvCoberturas.Columns.Clear();
            dgvCoberturas.RowHeadersVisible = false;
            dgvCoberturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCoberturas.ReadOnly = true;
            dgvCoberturas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvCoberturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCobertura",
                HeaderText = "Obra Social / Cobertura",
                DataPropertyName = "NombreCobertura",
                Width = 220
            });
            dgvCoberturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipoCobertura",
                HeaderText = "Tipo Cobertura",
                DataPropertyName = "TipoCobertura",
                Width = 140
            });
            dgvCoberturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCantTurnos",
                HeaderText = "Turnos",
                DataPropertyName = "CantidadTurnos",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvCoberturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTotalRecaudado",
                HeaderText = "Total Recaudado",
                DataPropertyName = "TotalRecaudado",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCoberturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTicketCob",
                HeaderText = "Ticket Promedio",
                DataPropertyName = "TicketPromedio",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCoberturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPorcFact",
                HeaderText = "% Facturación",
                DataPropertyName = "PorcentajeFacturacion",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N1", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // 3. Grilla de Demanda de Médicos
            dgvDemanda.AutoGenerateColumns = false;
            dgvDemanda.Columns.Clear();
            dgvDemanda.RowHeadersVisible = false;
            dgvDemanda.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDemanda.ReadOnly = true;
            dgvDemanda.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRank",
                HeaderText = "#",
                Width = 45,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDemandaMedico",
                HeaderText = "Profesional Médico",
                DataPropertyName = "NombreMedico",
                Width = 190
            });
            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDemandaEsp",
                HeaderText = "Especialidad",
                DataPropertyName = "Especialidad",
                Width = 140
            });
            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAsignados",
                HeaderText = "Asignados",
                DataPropertyName = "TotalTurnosAsignados",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAtendidos",
                HeaderText = "Atendidos",
                DataPropertyName = "TurnosAtendidos",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEspera",
                HeaderText = "En Espera",
                DataPropertyName = "TurnosEnEspera",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colResolucion",
                HeaderText = "% Resolutividad",
                DataPropertyName = "TasaResolucion",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N1", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCategoria",
                HeaderText = "Categoría Demanda",
                DataPropertyName = "CategoriaDemanda",
                Width = 140
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

                _ingresos = _reporteBLL.ObtenerIngresosPorMedico(desde, hasta);
                _coberturas = _reporteBLL.ObtenerObrasSocialesVsParticulares(desde, hasta);
                _demanda = _reporteBLL.ObtenerDemandaMedicosRanking(desde, hasta);

                // Enlazar grillas
                dgvIngresos.DataSource = null;
                dgvIngresos.DataSource = _ingresos;

                dgvCoberturas.DataSource = null;
                dgvCoberturas.DataSource = _coberturas;

                dgvDemanda.DataSource = null;
                dgvDemanda.DataSource = _demanda;

                // Asignar números de orden correlativos al ranking de demanda
                for (int i = 0; i < dgvDemanda.Rows.Count; i++)
                {
                    dgvDemanda.Rows[i].Cells["colRank"].Value = (i + 1).ToString();
                }

                // Actualizar KPIs
                ActualizarKpis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al cargar la información gerencial:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ActualizarKpis()
        {
            int totalConsultas = _ingresos.Sum(i => i.ConsultasAtendidas);
            decimal totalFacturado = _ingresos.Sum(i => i.IngresosTotales);
            decimal totalParticulares = _coberturas.Where(c => c.TipoCobertura.Equals("Particular", StringComparison.OrdinalIgnoreCase)).Sum(c => c.TotalRecaudado);
            decimal totalObrasSociales = _coberturas.Where(c => !c.TipoCobertura.Equals("Particular", StringComparison.OrdinalIgnoreCase)).Sum(c => c.TotalRecaudado);

            lblKpiConsultas.Text = totalConsultas.ToString("N0");
            lblKpiFacturacion.Text = $"$ {totalFacturado:N2}";
            lblKpiParticulares.Text = $"$ {totalParticulares:N2}";
            lblKpiObrasSociales.Text = $"$ {totalObrasSociales:N2}";
        }

        private void BtnExportar_Click(object? sender, EventArgs e)
        {
            if (_ingresos.Count == 0 && _coberturas.Count == 0 && _demanda.Count == 0)
            {
                MessageBox.Show("No hay datos disponibles para exportar en el rango seleccionado.",
                    "Exportar Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
                sfd.Filter = "Documento Oficial PDF (*.pdf)|*.pdf";
                sfd.FilterIndex = 1;
                sfd.FileName = $"Reporte_Gerencial_Facturacion_{timestamp}.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;

                        ExportadorPdf.ExportarReporteGerencial(
                            sfd.FileName,
                            _ingresos,
                            _coberturas,
                            _demanda,
                            dtpDesde.Value.Date,
                            dtpHasta.Value.Date,
                            _usuarioActual);

                        var resp = MessageBox.Show(
                            $"Reporte oficial generado con éxito en:\n{sfd.FileName}\n\n¿Desea abrir el documento PDF ahora?",
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
                                // Si el visor predeterminado no pudo abrirse directamente
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
