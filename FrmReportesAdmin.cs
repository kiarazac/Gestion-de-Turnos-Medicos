using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de panel de control administrativo para la visualización, análisis y exportación
    /// de reportes gerenciales de demanda médica, resolutividad de turnos y productividad profesional.
    /// </summary>
    public partial class FrmReportesAdmin : Form
    {
        private readonly ReporteBLL _reporteBLL = new ReporteBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        private List<ReporteDemandaEspecialidadDTO> _datosDemanda = new List<ReporteDemandaEspecialidadDTO>();
        private List<ReporteProductividadMedicoDTO> _datosProductividad = new List<ReporteProductividadMedicoDTO>();
        private bool _cargandoFiltros = false;

        /// <summary>
        /// Constructor predeterminado sin contexto de usuario (útil para el diseñador).
        /// </summary>
        public FrmReportesAdmin() : this(null)
        {
        }

        /// <summary>
        /// Inicializa el panel de reportes gerenciales inyectando las credenciales de administración.
        /// </summary>
        /// <param name="usuario">Contexto del usuario administrador autenticado (<see cref="UsuarioLoginResult"/>).</param>
        public FrmReportesAdmin(UsuarioLoginResult? usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            this.Load += FrmReportesAdmin_Load;
            this.cmbPeriodo.SelectedIndexChanged += CmbPeriodo_SelectedIndexChanged;
            this.btnFiltrar.Click += BtnFiltrar_Click;
            this.btnExportar.Click += BtnExportar_Click;
            this.dgvDemanda.CellFormatting += DgvDemanda_CellFormatting;
            this.dgvProductividad.CellFormatting += DgvProductividad_CellFormatting;
        }

        private void FrmReportesAdmin_Load(object? sender, EventArgs e)
        {
            ConfigurarGrillas();
            CargarCombosFiltro();
            CargarReportes();
        }

        /// <summary>
        /// Configura el diseño visual, alineaciones, fuentes y columnas de las grillas de demanda y productividad.
        /// </summary>
        private void ConfigurarGrillas()
        {
            // === Grilla 1: Demanda por Especialidad ===
            dgvDemanda.AutoGenerateColumns = false;
            dgvDemanda.Columns.Clear();
            dgvDemanda.RowHeadersVisible = false;
            dgvDemanda.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDemanda.MultiSelect = false;
            dgvDemanda.ReadOnly = true;
            dgvDemanda.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvDemanda.DefaultCellStyle.Font = new Font("Segoe UI", 9.25F);
            dgvDemanda.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvDemanda.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(226, 232, 240);
            dgvDemanda.EnableHeadersVisualStyles = false;

            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colIdEspecialidad",
                HeaderText = "ID",
                DataPropertyName = "IdEspecialidad",
                Visible = false
            });

            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEspecialidad",
                HeaderText = "Especialidad Médica",
                DataPropertyName = "Especialidad",
                FillWeight = 38
            });

            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTotalTurnos",
                HeaderText = "Total Turnos",
                DataPropertyName = "TotalTurnos",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" },
                FillWeight = 16
            });

            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTurnosAtendidos",
                HeaderText = "Atendidos",
                DataPropertyName = "TurnosAtendidos",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" },
                FillWeight = 15
            });

            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTurnosEnEspera",
                HeaderText = "En Espera / Sala",
                DataPropertyName = "TurnosEnEspera",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" },
                FillWeight = 15
            });

            dgvDemanda.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPorcentajeAtencion",
                HeaderText = "% Resolutividad",
                DataPropertyName = "PorcentajeAtencion",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N2" },
                FillWeight = 16
            });

            // === Grilla 2: Productividad por Médico ===
            dgvProductividad.AutoGenerateColumns = false;
            dgvProductividad.Columns.Clear();
            dgvProductividad.RowHeadersVisible = false;
            dgvProductividad.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductividad.MultiSelect = false;
            dgvProductividad.ReadOnly = true;
            dgvProductividad.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvProductividad.DefaultCellStyle.Font = new Font("Segoe UI", 9.25F);
            dgvProductividad.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvProductividad.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(226, 232, 240);
            dgvProductividad.EnableHeadersVisualStyles = false;

            dgvProductividad.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colIdUsuario",
                HeaderText = "ID",
                DataPropertyName = "IdUsuario",
                Visible = false
            });

            dgvProductividad.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMedico",
                HeaderText = "Profesional Médico",
                DataPropertyName = "MedicoCompleto",
                FillWeight = 35
            });

            dgvProductividad.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMatricula",
                HeaderText = "Matrícula",
                DataPropertyName = "Matricula",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                FillWeight = 15
            });

            dgvProductividad.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEspecialidadMed",
                HeaderText = "Especialidad",
                DataPropertyName = "Especialidad",
                FillWeight = 25
            });

            dgvProductividad.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colConsultasAtendidas",
                HeaderText = "Consultas Atendidas",
                DataPropertyName = "ConsultasAtendidas",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" },
                FillWeight = 15
            });

            dgvProductividad.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPacientesUnicos",
                HeaderText = "Pacientes Únicos",
                DataPropertyName = "PacientesUnicos",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" },
                FillWeight = 15
            });
        }

        /// <summary>
        /// Inicializa los elementos de los selectores de período y especialidades.
        /// </summary>
        private void CargarCombosFiltro()
        {
            _cargandoFiltros = true;

            // Períodos preconfigurados
            cmbPeriodo.Items.Clear();
            cmbPeriodo.Items.Add("Hoy");
            cmbPeriodo.Items.Add("Últimos 7 días");
            cmbPeriodo.Items.Add("Este mes");
            cmbPeriodo.Items.Add("Mes anterior");
            cmbPeriodo.Items.Add("Año en curso");
            cmbPeriodo.Items.Add("Todo el historial");
            cmbPeriodo.Items.Add("Personalizado");
            cmbPeriodo.SelectedIndex = 2; // "Este mes" por defecto

            // Especialidades médicas
            cmbEspecialidad.Items.Clear();
            cmbEspecialidad.Items.Add(new ElementoComboItem(0, "Todas las especialidades"));

            try
            {
                var especialidades = _reporteBLL.ObtenerEspecialidadesParaFiltro();
                foreach (var esp in especialidades)
                {
                    cmbEspecialidad.Items.Add(new ElementoComboItem(esp.IdEspecialidad, esp.Nombre));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el catálogo de especialidades: {ex.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            cmbEspecialidad.SelectedIndex = 0;
            _cargandoFiltros = false;

            AplicarRangoFechasSegunPeriodo();
        }

        private void CmbPeriodo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_cargandoFiltros) return;
            AplicarRangoFechasSegunPeriodo();
        }

        /// <summary>
        /// Ajusta las fechas de los controles <c>dtpDesde</c> y <c>dtpHasta</c> según la opción elegida en el selector de períodos.
        /// </summary>
        private void AplicarRangoFechasSegunPeriodo()
        {
            string seleccionado = cmbPeriodo.SelectedItem?.ToString() ?? "Este mes";
            DateTime hoy = DateTime.Today;

            switch (seleccionado)
            {
                case "Hoy":
                    dtpDesde.Value = hoy;
                    dtpHasta.Value = hoy;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case "Últimos 7 días":
                    dtpDesde.Value = hoy.AddDays(-7);
                    dtpHasta.Value = hoy;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case "Este mes":
                    dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
                    dtpHasta.Value = hoy;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case "Mes anterior":
                    DateTime primerDiaMesActual = new DateTime(hoy.Year, hoy.Month, 1);
                    DateTime ultimoDiaMesAnterior = primerDiaMesActual.AddDays(-1);
                    DateTime primerDiaMesAnterior = new DateTime(ultimoDiaMesAnterior.Year, ultimoDiaMesAnterior.Month, 1);
                    dtpDesde.Value = primerDiaMesAnterior;
                    dtpHasta.Value = ultimoDiaMesAnterior;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case "Año en curso":
                    dtpDesde.Value = new DateTime(hoy.Year, 1, 1);
                    dtpHasta.Value = hoy;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case "Todo el historial":
                    dtpDesde.Value = new DateTime(2020, 1, 1);
                    dtpHasta.Value = hoy;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case "Personalizado":
                default:
                    dtpDesde.Enabled = true;
                    dtpHasta.Enabled = true;
                    break;
            }
        }

        private void BtnFiltrar_Click(object? sender, EventArgs e)
        {
            CargarReportes();
        }

        /// <summary>
        /// Consulta la Capa de Negocio para refrescar la demanda por especialidad y la productividad médica.
        /// Actualiza tanto las grillas como las tarjetas de KPI ejecutivas.
        /// </summary>
        private void CargarReportes()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                lblEstado.Text = "Consultando estadísticas en la base de datos...";

                string periodo = cmbPeriodo.SelectedItem?.ToString() ?? "Personalizado";
                DateTime? fechaDesde = (periodo == "Todo el historial") ? null : dtpDesde.Value.Date;
                DateTime? fechaHasta = (periodo == "Todo el historial") ? null : dtpHasta.Value.Date;

                int? idEspecialidad = null;
                if (cmbEspecialidad.SelectedItem is ElementoComboItem item && item.Id > 0)
                {
                    idEspecialidad = item.Id;
                }

                // Obtener datos desde BLL
                _datosDemanda = _reporteBLL.ObtenerDemandaEspecialidades(fechaDesde, fechaHasta, idEspecialidad);
                _datosProductividad = _reporteBLL.ObtenerProductividadMedicos(fechaDesde, fechaHasta, idEspecialidad);

                // Enlazar DataGridViews
                dgvDemanda.DataSource = null;
                dgvDemanda.DataSource = _datosDemanda;

                dgvProductividad.DataSource = null;
                dgvProductividad.DataSource = _datosProductividad;

                // Actualizar Tarjetas KPI ejecutivas
                ActualizarKpis();

                lblEstado.Text = $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | {_datosDemanda.Count} especialidades evaluadas | {_datosProductividad.Count} médicos listados.";
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error al generar reportes.";
                MessageBox.Show($"Ocurrió un error al procesar el reporte: {ex.Message}", "Error de Reportes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Consolida los totales de los datos procesados y actualiza los indicadores visuales en las tarjetas superiores.
        /// </summary>
        private void ActualizarKpis()
        {
            int totalTurnos = _datosDemanda.Sum(x => x.TotalTurnos);
            int turnosAtendidos = _datosDemanda.Sum(x => x.TurnosAtendidos);
            int turnosEnEspera = _datosDemanda.Sum(x => x.TurnosEnEspera);

            decimal pctAtendidos = totalTurnos > 0 ? Math.Round((decimal)turnosAtendidos * 100m / totalTurnos, 1) : 0m;
            decimal pctEnEspera = totalTurnos > 0 ? Math.Round((decimal)turnosEnEspera * 100m / totalTurnos, 1) : 0m;

            lblKpiTotalNum.Text = totalTurnos.ToString("N0");
            lblKpiAtendidosNum.Text = $"{turnosAtendidos:N0} ({pctAtendidos}%)";
            lblKpiEnEsperaNum.Text = $"{turnosEnEspera:N0} ({pctEnEspera}%)";

            var topEspecialidad = _datosDemanda
                .Where(x => x.TotalTurnos > 0)
                .OrderByDescending(x => x.TotalTurnos)
                .FirstOrDefault();

            if (topEspecialidad != null)
            {
                lblKpiTopNombre.Text = $"{topEspecialidad.Especialidad} ({topEspecialidad.TotalTurnos})";
            }
            else
            {
                lblKpiTopNombre.Text = "Sin turnos registrados";
            }
        }

        private void DgvDemanda_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null) return;

            // Formato con sufijo % para Porcentaje de Atención
            if (dgvDemanda.Columns[e.ColumnIndex].Name == "colPorcentajeAtencion" && e.Value is decimal pct)
            {
                e.Value = $"{pct:N2} %";
                e.FormattingApplied = true;

                if (pct >= 80m)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(16, 120, 80);
                    e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }
                else if (pct >= 50m)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(180, 100, 20);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(185, 28, 28);
                }
            }
        }

        private void DgvProductividad_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null) return;

            // Resaltar médicos con atenciones
            if (dgvProductividad.Columns[e.ColumnIndex].Name == "colConsultasAtendidas" && e.Value is int atenciones)
            {
                if (atenciones > 0)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(15, 118, 110);
                    e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }
            }
        }

        /// <summary>
        /// Gestiona la exportación de las estadísticas consolidadas a archivo CSV delimitado o informe ejecutivo en texto plano.
        /// </summary>
        private void BtnExportar_Click(object? sender, EventArgs e)
        {
            if (_datosDemanda.Count == 0 && _datosProductividad.Count == 0)
            {
                MessageBox.Show("No existen datos generados para exportar en el período seleccionado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string fechaNom = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                sfd.Title = "Guardar Reporte Gerencial";
                sfd.Filter = "Documento de Valores Separados por Comas (*.csv)|*.csv|Informe Ejecutivo de Texto (*.txt)|*.txt";
                sfd.FileName = $"Reporte_Gerencial_CentroMedico_{fechaNom}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (sfd.FilterIndex == 1) // CSV
                        {
                            ExportarACSV(sfd.FileName);
                        }
                        else // TXT
                        {
                            ExportarATextoEjecutivo(sfd.FileName);
                        }

                        MessageBox.Show($"Reporte gerencial exportado con éxito en:\n{sfd.FileName}", "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ocurrió un error al escribir el archivo: {ex.Message}", "Error al Exportar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Genera un archivo CSV con las dos secciones del reporte separadas claramente.
        /// </summary>
        private void ExportarACSV(string rutaArchivo)
        {
            var sb = new StringBuilder();

            sb.AppendLine("sep=;");
            sb.AppendLine($"REPORTE GERENCIAL Y ESTADÍSTICO - CENTRO MÉDICO");
            sb.AppendLine($"Fecha de Emisión;{DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Generado por;{(_usuarioActual != null ? $"{_usuarioActual.Nombre} {_usuarioActual.Apellido} (Admin)" : "Administrador")}");
            sb.AppendLine($"Rango Consultado;Desde {dtpDesde.Value:dd/MM/yyyy} hasta {dtpHasta.Value:dd/MM/yyyy} ({cmbPeriodo.SelectedItem})");
            sb.AppendLine();

            sb.AppendLine("=== SECCIÓN 1: DEMANDA Y RESOLUTIVIDAD POR ESPECIALIDAD ===");
            sb.AppendLine("Especialidad;Total Turnos;Atendidos;En Espera / Consulta;% Resolutividad");
            foreach (var d in _datosDemanda)
            {
                sb.AppendLine($"\"{d.Especialidad}\";{d.TotalTurnos};{d.TurnosAtendidos};{d.TurnosEnEspera};{d.PorcentajeAtencion:N2}%");
            }

            sb.AppendLine();
            sb.AppendLine("=== SECCIÓN 2: PRODUCTIVIDAD DEL CUERPO MÉDICO ===");
            sb.AppendLine("Profesional Médico;Matrícula;Especialidad;Consultas Atendidas;Pacientes Únicos");
            foreach (var p in _datosProductividad)
            {
                sb.AppendLine($"\"{p.MedicoCompleto}\";\"{p.Matricula}\";\"{p.Especialidad}\";{p.ConsultasAtendidas};{p.PacientesUnicos}");
            }

            File.WriteAllText(rutaArchivo, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Genera un informe ejecutivo formateado en columnas de texto plano, ideal para lectura directa o impresión.
        /// </summary>
        private void ExportarATextoEjecutivo(string rutaArchivo)
        {
            var sb = new StringBuilder();
            string separador = new string('=', 85);
            string lineaFina = new string('-', 85);

            sb.AppendLine(separador);
            sb.AppendLine("                INFORME GERENCIAL Y ESTADÍSTICO DE ATENCIÓN MÉDICA");
            sb.AppendLine(separador);
            sb.AppendLine($"Fecha de generación : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Auditor / Emisor    : {(_usuarioActual != null ? $"{_usuarioActual.Nombre} {_usuarioActual.Apellido} ({_usuarioActual.NombreRol})" : "Administrador")}");
            sb.AppendLine($"Período analizado   : {cmbPeriodo.SelectedItem} ({dtpDesde.Value:dd/MM/yyyy} - {dtpHasta.Value:dd/MM/yyyy})");
            sb.AppendLine(separador);
            sb.AppendLine();

            sb.AppendLine("RESUMEN GENERAL (KPIs EJECUTIVOS):");
            sb.AppendLine(lineaFina);
            sb.AppendLine($"Total de Turnos Emitidos        : {lblKpiTotalNum.Text}");
            sb.AppendLine($"Turnos Efectivamente Atendidos  : {lblKpiAtendidosNum.Text}");
            sb.AppendLine($"Turnos en Espera / Sala         : {lblKpiEnEsperaNum.Text}");
            sb.AppendLine($"Especialidad con Mayor Demanda  : {lblKpiTopNombre.Text}");
            sb.AppendLine();

            sb.AppendLine("1. DETALLE DE DEMANDA POR ESPECIALIDAD MÉDICA:");
            sb.AppendLine(lineaFina);
            sb.AppendLine(string.Format("{0,-30} | {1,7} | {2,10} | {3,12} | {4,10}",
                "Especialidad", "Total", "Atendidos", "En Espera", "% Atenc."));
            sb.AppendLine(lineaFina);

            foreach (var d in _datosDemanda)
            {
                string esp = d.Especialidad.Length > 30 ? d.Especialidad.Substring(0, 27) + "..." : d.Especialidad;
                sb.AppendLine(string.Format("{0,-30} | {1,7} | {2,10} | {3,12} | {4,9:N2}%",
                    esp, d.TotalTurnos, d.TurnosAtendidos, d.TurnosEnEspera, d.PorcentajeAtencion));
            }

            sb.AppendLine();
            sb.AppendLine("2. PRODUCTIVIDAD DEL CUERPO MÉDICO:");
            sb.AppendLine(lineaFina);
            sb.AppendLine(string.Format("{0,-30} | {1,-12} | {2,-20} | {3,10} | {4,10}",
                "Profesional Médico", "Matrícula", "Especialidad", "Consultas", "Pac. Únicos"));
            sb.AppendLine(lineaFina);

            foreach (var p in _datosProductividad)
            {
                string med = p.MedicoCompleto.Length > 30 ? p.MedicoCompleto.Substring(0, 27) + "..." : p.MedicoCompleto;
                string esp = p.Especialidad.Length > 20 ? p.Especialidad.Substring(0, 17) + "..." : p.Especialidad;
                sb.AppendLine(string.Format("{0,-30} | {1,-12} | {2,-20} | {3,10} | {4,10}",
                    med, p.Matricula, esp, p.ConsultasAtendidas, p.PacientesUnicos));
            }

            sb.AppendLine();
            sb.AppendLine(separador);
            sb.AppendLine("Fin del Reporte Oficial - Sistema de Gestión de Turnos Médicos");
            sb.AppendLine(separador);

            File.WriteAllText(rutaArchivo, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Elemento auxiliar para enlazar valores numéricos con etiquetas de texto en el ComboBox de especialidades.
        /// </summary>
        private class ElementoComboItem
        {
            public int Id { get; }
            public string Nombre { get; }

            public ElementoComboItem(int id, string nombre)
            {
                Id = id;
                Nombre = nombre;
            }

            public override string ToString() => Nombre;
        }

        private void FrmReportesAdmin_Load_1(object sender, EventArgs e)
        {

        }
    }
}
