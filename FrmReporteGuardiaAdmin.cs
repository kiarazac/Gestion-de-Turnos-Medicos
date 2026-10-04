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
    /// Formulario independiente para el perfil Administrador:
    /// Reporte Operativo de Guardia: Triage y Distribución de Urgencias médicas.
    /// Provee indicadores clave de flujo, severidad de triage, ranking de síntomas,
    /// grilla interactiva y exportación en formatos CSV y TXT.
    /// </summary>
    public partial class FrmReporteGuardiaAdmin : Form
    {
        private readonly ReporteGuardiaBLL _reporteBLL = new ReporteGuardiaBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        private ReporteGuardiaResumenDTO _resumenActual = new ReporteGuardiaResumenDTO();
        private List<ReporteGuardiaSintomaDTO> _rankingSintomas = new List<ReporteGuardiaSintomaDTO>();
        private List<ReporteGuardiaDetalleDTO> _todosLosDetalles = new List<ReporteGuardiaDetalleDTO>();
        private List<ReporteGuardiaDetalleDTO> _detallesFiltrados = new List<ReporteGuardiaDetalleDTO>();

        private const string PLACEHOLDER_BUSCAR = "🔍 Buscar por paciente, DNI, turno o síntoma...";

        public FrmReporteGuardiaAdmin() : this(null)
        {
        }

        public FrmReporteGuardiaAdmin(UsuarioLoginResult? usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            this.Load += FrmReporteGuardiaAdmin_Load;
            this.cmbPeriodo.SelectedIndexChanged += CmbPeriodo_SelectedIndexChanged;
            this.cmbPrioridad.SelectedIndexChanged += CmbPrioridad_SelectedIndexChanged;
            this.btnFiltrar.Click += BtnFiltrar_Click;
            this.btnExportar.Click += BtnExportar_Click;

            this.txtBuscar.Enter += TxtBuscar_Enter;
            this.txtBuscar.Leave += TxtBuscar_Leave;
            this.txtBuscar.TextChanged += TxtBuscar_TextChanged;

            this.dgvDetalleGuardia.CellFormatting += DgvDetalleGuardia_CellFormatting;
            this.dgvRankingSintomas.CellFormatting += DgvRankingSintomas_CellFormatting;
        }

        private void FrmReporteGuardiaAdmin_Load(object? sender, EventArgs e)
        {
            ConfigurarGrillas();

            if (cmbPeriodo.SelectedIndex < 0)
                cmbPeriodo.SelectedIndex = 1; // "Últimos 7 días" por defecto

            if (cmbPrioridad.SelectedIndex < 0)
                cmbPrioridad.SelectedIndex = 0; // "Todas las prioridades"

            CargarReportes();
        }

        /// <summary>
        /// Configura el diseño visual, anchos, alineaciones y estilos de las grillas.
        /// </summary>
        private void ConfigurarGrillas()
        {
            // === Grilla 1: Detalle de Ingresos a Guardia ===
            dgvDetalleGuardia.AutoGenerateColumns = false;
            dgvDetalleGuardia.Columns.Clear();
            dgvDetalleGuardia.RowHeadersVisible = false;
            dgvDetalleGuardia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDetalleGuardia.MultiSelect = false;
            dgvDetalleGuardia.ReadOnly = true;
            dgvDetalleGuardia.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvDetalleGuardia.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvDetalleGuardia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvDetalleGuardia.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(226, 232, 240);
            dgvDetalleGuardia.EnableHeadersVisualStyles = false;

            dgvDetalleGuardia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFecha",
                HeaderText = "Fecha / Hora",
                DataPropertyName = "Fecha",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" },
                FillWeight = 14
            });

            dgvDetalleGuardia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTurno",
                HeaderText = "N° Turno",
                DataPropertyName = "NroOrden",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
                FillWeight = 9
            });

            dgvDetalleGuardia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaciente",
                HeaderText = "Paciente",
                DataPropertyName = "PacienteCompleto",
                FillWeight = 19
            });

            dgvDetalleGuardia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDni",
                HeaderText = "DNI",
                DataPropertyName = "DniPaciente",
                FillWeight = 10
            });

            dgvDetalleGuardia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colObraSocial",
                HeaderText = "Cobertura",
                DataPropertyName = "ObraSocial",
                FillWeight = 13
            });

            dgvDetalleGuardia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrioridad",
                HeaderText = "Triage / Prioridad",
                DataPropertyName = "Prioridad",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                FillWeight = 13
            });

            dgvDetalleGuardia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSintomas",
                HeaderText = "Síntomas Manifestados",
                DataPropertyName = "Sintomas",
                FillWeight = 24
            });

            dgvDetalleGuardia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstado",
                HeaderText = "Estado",
                DataPropertyName = "Estado",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                FillWeight = 11
            });

            dgvDetalleGuardia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSala",
                HeaderText = "Consultorio / Box",
                DataPropertyName = "NombreSala",
                FillWeight = 12
            });

            // === Grilla 2: Ranking de Síntomas Predominantes ===
            dgvRankingSintomas.AutoGenerateColumns = false;
            dgvRankingSintomas.Columns.Clear();
            dgvRankingSintomas.RowHeadersVisible = false;
            dgvRankingSintomas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRankingSintomas.MultiSelect = false;
            dgvRankingSintomas.ReadOnly = true;
            dgvRankingSintomas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvRankingSintomas.DefaultCellStyle.Font = new Font("Segoe UI", 9.25F);
            dgvRankingSintomas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvRankingSintomas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(226, 232, 240);
            dgvRankingSintomas.EnableHeadersVisualStyles = false;

            dgvRankingSintomas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSintomaNombre",
                HeaderText = "Síntoma / Motivo de Consulta",
                DataPropertyName = "Sintoma",
                FillWeight = 45
            });

            dgvRankingSintomas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSintomaGravedad",
                HeaderText = "Nivel de Severidad",
                DataPropertyName = "Gravedad",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                FillWeight = 20
            });

            dgvRankingSintomas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSintomaCasos",
                HeaderText = "Cantidad de Casos",
                DataPropertyName = "CantidadCasos",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" },
                FillWeight = 17
            });

            dgvRankingSintomas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSintomaPorcentaje",
                HeaderText = "% Participación",
                DataPropertyName = "Porcentaje",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N2" },
                FillWeight = 18
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
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case 1: // Últimos 7 días
                    dtpDesde.Value = hoy.AddDays(-7);
                    dtpHasta.Value = hoy;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case 2: // Este mes
                    dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
                    dtpHasta.Value = hoy;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case 3: // Todos los registros
                    dtpDesde.Value = new DateTime(2020, 1, 1);
                    dtpHasta.Value = hoy.AddDays(1);
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case 4: // Rango personalizado
                    dtpDesde.Enabled = true;
                    dtpHasta.Enabled = true;
                    break;
            }

            if (cmbPeriodo.SelectedIndex != 4)
            {
                CargarReportes();
            }
        }

        private void CmbPrioridad_SelectedIndexChanged(object? sender, EventArgs e)
        {
            CargarReportes();
        }

        private void BtnFiltrar_Click(object? sender, EventArgs e)
        {
            CargarReportes();
        }

        /// <summary>
        /// Ejecuta las consultas a la capa BLL y actualiza tarjetas, grillas y estados.
        /// </summary>
        private void CargarReportes()
        {
            DateTime fechaDesde = dtpDesde.Value.Date;
            DateTime fechaHasta = dtpHasta.Value.Date;

            int? idPrioridad = cmbPrioridad.SelectedIndex switch
            {
                1 => 1, // Alta
                2 => 2, // Media
                3 => 3, // Baja
                _ => null
            };

            try
            {
                // Invocaciones desacopladas a la Capa de Lógica de Negocio (BLL)
                _resumenActual = _reporteBLL.ObtenerResumenGuardia(fechaDesde, fechaHasta, idPrioridad);
                _rankingSintomas = _reporteBLL.ObtenerRankingSintomas(fechaDesde, fechaHasta, idPrioridad);
                _todosLosDetalles = _reporteBLL.ObtenerDetalleGuardia(fechaDesde, fechaHasta, idPrioridad);

                ActualizarTarjetasKPI();
                AplicarFiltroEnMemoria();

                dgvRankingSintomas.DataSource = null;
                dgvRankingSintomas.DataSource = _rankingSintomas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos del reporte de guardia:\n" + ex.Message,
                    "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Actualiza los valores y leyendas de las 5 tarjetas superiores (KPIs).
        /// </summary>
        private void ActualizarTarjetasKPI()
        {
            int total = _resumenActual.TotalEmergencias;
            lblTotalValor.Text = total.ToString("N0");

            // Triage Alta (Rojo)
            lblAltaValor.Text = _resumenActual.TotalAlta.ToString("N0");
            double pctAlta = total > 0 ? ((double)_resumenActual.TotalAlta / total) * 100.0 : 0.0;
            lblAltaDesc.Text = $"{pctAlta:F1}% de los ingresos";

            // Triage Media (Amarillo)
            lblMediaValor.Text = _resumenActual.TotalMedia.ToString("N0");
            double pctMedia = total > 0 ? ((double)_resumenActual.TotalMedia / total) * 100.0 : 0.0;
            lblMediaDesc.Text = $"{pctMedia:F1}% de los ingresos";

            // Triage Baja (Verde)
            lblBajaValor.Text = _resumenActual.TotalBaja.ToString("N0");
            double pctBaja = total > 0 ? ((double)_resumenActual.TotalBaja / total) * 100.0 : 0.0;
            lblBajaDesc.Text = $"{pctBaja:F1}% de los ingresos";

            // Tasa de Resolución
            lblResolucionValor.Text = $"{_resumenActual.TasaResolucion:F1}%";
            lblResolucionDesc.Text = $"{_resumenActual.TotalAtendidos} atendidos / {_resumenActual.TotalEnEspera} en espera";
        }

        private void TxtBuscar_Enter(object? sender, EventArgs e)
        {
            if (txtBuscar.Text == PLACEHOLDER_BUSCAR)
            {
                txtBuscar.Text = string.Empty;
                txtBuscar.ForeColor = Color.Black;
            }
        }

        private void TxtBuscar_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = PLACEHOLDER_BUSCAR;
                txtBuscar.ForeColor = Color.Gray;
            }
        }

        private void TxtBuscar_TextChanged(object? sender, EventArgs e)
        {
            AplicarFiltroEnMemoria();
        }

        private void AplicarFiltroEnMemoria()
        {
            string filtro = txtBuscar.Text.Trim();

            if (string.IsNullOrWhiteSpace(filtro) || filtro == PLACEHOLDER_BUSCAR)
            {
                _detallesFiltrados = new List<ReporteGuardiaDetalleDTO>(_todosLosDetalles);
            }
            else
            {
                string filtroMin = filtro.ToLowerInvariant();
                _detallesFiltrados = _todosLosDetalles
                    .Where(d => (d.PacienteCompleto != null && d.PacienteCompleto.ToLowerInvariant().Contains(filtroMin))
                             || (d.DniPaciente != null && d.DniPaciente.Contains(filtro))
                             || (d.NroOrden != null && d.NroOrden.ToLowerInvariant().Contains(filtroMin))
                             || (d.Sintomas != null && d.Sintomas.ToLowerInvariant().Contains(filtroMin))
                             || (d.ObraSocial != null && d.ObraSocial.ToLowerInvariant().Contains(filtroMin))
                             || (d.Estado != null && d.Estado.ToLowerInvariant().Contains(filtroMin)))
                    .ToList();
            }

            dgvDetalleGuardia.DataSource = null;
            dgvDetalleGuardia.DataSource = _detallesFiltrados;

            lblTotalRegistros.Text = $"Total registros mostrados: {_detallesFiltrados.Count} (de {_todosLosDetalles.Count} ingresos totales)";
        }

        /// <summary>
        /// Formato condicional de celdas para Triage (Rojo/Amarillo/Verde) y Estado de turnos.
        /// </summary>
        private void DgvDetalleGuardia_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvDetalleGuardia.Columns[e.ColumnIndex].Name;

            if (colName == "colPrioridad" && e.Value != null)
            {
                string prioridad = e.Value.ToString()?.Trim().ToUpperInvariant() ?? string.Empty;

                switch (prioridad)
                {
                    case "ALTA":
                        e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
                        e.CellStyle.ForeColor = Color.FromArgb(185, 28, 28);
                        e.CellStyle.Font = new Font(dgvDetalleGuardia.Font, FontStyle.Bold);
                        break;

                    case "MEDIA":
                        e.CellStyle.BackColor = Color.FromArgb(254, 243, 199);
                        e.CellStyle.ForeColor = Color.FromArgb(180, 83, 9);
                        e.CellStyle.Font = new Font(dgvDetalleGuardia.Font, FontStyle.Bold);
                        break;

                    case "BAJA":
                        e.CellStyle.BackColor = Color.FromArgb(209, 250, 229);
                        e.CellStyle.ForeColor = Color.FromArgb(4, 120, 87);
                        e.CellStyle.Font = new Font(dgvDetalleGuardia.Font, FontStyle.Bold);
                        break;
                }
            }
            else if (colName == "colEstado" && e.Value != null)
            {
                string estado = e.Value.ToString()?.Trim().ToUpperInvariant() ?? string.Empty;

                if (estado == "ATENDIDO" || estado == "FINALIZADO")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(5, 150, 105);
                    e.CellStyle.Font = new Font(dgvDetalleGuardia.Font, FontStyle.Bold);
                }
                else if (estado == "EN CONSULTA")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(37, 99, 235);
                    e.CellStyle.Font = new Font(dgvDetalleGuardia.Font, FontStyle.Bold);
                }
                else if (estado == "CANCELADO" || estado == "BAJA")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                }
            }
        }

        private void DgvRankingSintomas_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvRankingSintomas.Columns[e.ColumnIndex].Name;

            if (colName == "colSintomaGravedad" && e.Value != null)
            {
                string gravedad = e.Value.ToString()?.Trim().ToUpperInvariant() ?? string.Empty;

                switch (gravedad)
                {
                    case "ALTA":
                        e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
                        e.CellStyle.ForeColor = Color.FromArgb(185, 28, 28);
                        e.CellStyle.Font = new Font(dgvRankingSintomas.Font, FontStyle.Bold);
                        break;

                    case "MEDIA":
                        e.CellStyle.BackColor = Color.FromArgb(254, 243, 199);
                        e.CellStyle.ForeColor = Color.FromArgb(180, 83, 9);
                        e.CellStyle.Font = new Font(dgvRankingSintomas.Font, FontStyle.Bold);
                        break;

                    case "BAJA":
                        e.CellStyle.BackColor = Color.FromArgb(209, 250, 229);
                        e.CellStyle.ForeColor = Color.FromArgb(4, 120, 87);
                        e.CellStyle.Font = new Font(dgvRankingSintomas.Font, FontStyle.Bold);
                        break;
                }
            }
            else if (colName == "colSintomaPorcentaje" && e.Value != null)
            {
                e.Value = $"{e.Value} %";
                e.FormattingApplied = true;
            }
        }

        /// <summary>
        /// Gestiona la exportación de datos a formatos CSV o TXT según la selección del usuario.
        /// </summary>
        private void BtnExportar_Click(object? sender, EventArgs e)
        {
            if (_todosLosDetalles.Count == 0 && _rankingSintomas.Count == 0)
            {
                MessageBox.Show("No hay datos cargados para exportar en el período seleccionado.",
                    "Exportación Vacía", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Exportar Reporte Operativo de Guardia";
                sfd.Filter = "Archivo CSV (*.csv)|*.csv|Archivo de Texto Plano (*.txt)|*.txt";
                sfd.FilterIndex = 1;
                sfd.FileName = $"Reporte_Guardia_Triage_{DateTime.Now:yyyyMMdd_HHmm}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string extension = Path.GetExtension(sfd.FileName).ToLowerInvariant();

                        if (extension == ".csv")
                        {
                            ExportarACSV(sfd.FileName);
                        }
                        else
                        {
                            ExportarATXT(sfd.FileName);
                        }

                        MessageBox.Show($"Reporte exportado exitosamente en:\n{sfd.FileName}",
                            "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar el archivo:\n" + ex.Message,
                            "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Genera un archivo CSV con formato estandarizado y delimitado por comas.
        /// </summary>
        private void ExportarACSV(string rutaArchivo)
        {
            var sb = new StringBuilder();

            // Metadatos
            sb.AppendLine("# REPORTE OPERATIVO DE GUARDIA: TRIAGE Y DISTRIBUCION DE URGENCIAS");
            sb.AppendLine($"# Fecha de emision,{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"# Periodo analizado,{dtpDesde.Value:yyyy-MM-dd} a {dtpHasta.Value:yyyy-MM-dd}");
            sb.AppendLine($"# Prioridad filtrada,{cmbPrioridad.SelectedItem}");
            sb.AppendLine();

            // Sección 1: Indicadores Clave
            sb.AppendLine("## INDICADORES CLAVE (KPIS)");
            sb.AppendLine("Metrica,Valor");
            sb.AppendLine($"Total Ingresos Guardia,{_resumenActual.TotalEmergencias}");
            sb.AppendLine($"Triage Alta (Rojo),{_resumenActual.TotalAlta}");
            sb.AppendLine($"Triage Media (Amarillo),{_resumenActual.TotalMedia}");
            sb.AppendLine($"Triage Baja (Verde),{_resumenActual.TotalBaja}");
            sb.AppendLine($"Atendidos,{_resumenActual.TotalAtendidos}");
            sb.AppendLine($"En Espera,{_resumenActual.TotalEnEspera}");
            sb.AppendLine($"Cancelados,{_resumenActual.TotalCancelados}");
            sb.AppendLine($"Tasa de Resolucion (%),{_resumenActual.TasaResolucion:F2}");
            sb.AppendLine();

            // Sección 2: Ranking de Síntomas
            sb.AppendLine("## RANKING DE SINTOMAS Y MOTIVOS DE CONSULTA");
            sb.AppendLine("Sintoma,Severidad,Cantidad de Casos,Porcentaje (%)");
            foreach (var s in _rankingSintomas)
            {
                sb.AppendLine($"\"{s.Sintoma}\",\"{s.Gravedad}\",{s.CantidadCasos},{s.Porcentaje:F2}");
            }
            sb.AppendLine();

            // Sección 3: Detalle de Ingresos
            sb.AppendLine("## DETALLE DE INGRESOS A GUARDIA");
            sb.AppendLine("Fecha y Hora,Turno,Paciente,DNI,Cobertura,Triage,Sintomas,Estado,Consultorio");
            foreach (var d in _detallesFiltrados)
            {
                string sintomasEscapados = d.Sintomas.Replace("\"", "\"\"");
                sb.AppendLine($"\"{d.Fecha:yyyy-MM-dd HH:mm}\",\"{d.NroOrden}\",\"{d.PacienteCompleto}\",\"{d.DniPaciente}\",\"{d.ObraSocial}\",\"{d.Prioridad}\",\"{sintomasEscapados}\",\"{d.Estado}\",\"{d.NombreSala}\"");
            }

            File.WriteAllText(rutaArchivo, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Genera un informe en texto plano estructurado (.txt) con formato tabulado legible.
        /// </summary>
        private void ExportarATXT(string rutaArchivo)
        {
            var sb = new StringBuilder();

            sb.AppendLine("==========================================================================================");
            sb.AppendLine("            REPORTE OPERATIVO DE GUARDIA: TRIAGE Y DISTRIBUCIÓN DE URGENCIAS              ");
            sb.AppendLine("==========================================================================================");
            sb.AppendLine($"Fecha de emisión   : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Período analizado  : {dtpDesde.Value:dd/MM/yyyy} al {dtpHasta.Value:dd/MM/yyyy}");
            sb.AppendLine($"Filtro Prioridad   : {cmbPrioridad.SelectedItem}");
            sb.AppendLine($"Emitido por        : {(_usuarioActual != null ? $"{_usuarioActual.Nombre} {_usuarioActual.Apellido}" : "Administrador")}");
            sb.AppendLine("------------------------------------------------------------------------------------------");
            sb.AppendLine();

            sb.AppendLine("1. RESUMEN EJECUTIVO E INDICADORES CLAVE (KPIS)");
            sb.AppendLine("------------------------------------------------------------------------------------------");
            sb.AppendLine($"  • Total Ingresos de Guardia   : {_resumenActual.TotalEmergencias}");
            sb.AppendLine($"  • Triage Alta (Código Rojo)   : {_resumenActual.TotalAlta,4} ({(_resumenActual.TotalEmergencias > 0 ? (double)_resumenActual.TotalAlta / _resumenActual.TotalEmergencias * 100.0 : 0):F1}%)");
            sb.AppendLine($"  • Triage Media (Código Amar.) : {_resumenActual.TotalMedia,4} ({(_resumenActual.TotalEmergencias > 0 ? (double)_resumenActual.TotalMedia / _resumenActual.TotalEmergencias * 100.0 : 0):F1}%)");
            sb.AppendLine($"  • Triage Baja (Código Verde)  : {_resumenActual.TotalBaja,4} ({(_resumenActual.TotalEmergencias > 0 ? (double)_resumenActual.TotalBaja / _resumenActual.TotalEmergencias * 100.0 : 0):F1}%)");
            sb.AppendLine($"  • Consultas Atendidas         : {_resumenActual.TotalAtendidos}");
            sb.AppendLine($"  • Pacientes en Espera         : {_resumenActual.TotalEnEspera}");
            sb.AppendLine($"  • Deserciones / Cancelados    : {_resumenActual.TotalCancelados}");
            sb.AppendLine($"  • Tasa de Resolución de Guardia: {_resumenActual.TasaResolucion:F1} %");
            sb.AppendLine();

            sb.AppendLine("2. RANKING DE SÍNTOMAS PREDOMINANTES");
            sb.AppendLine("------------------------------------------------------------------------------------------");
            sb.AppendLine(string.Format("{0,-35} | {1,-10} | {2,8} | {3,12}", "SÍNTOMA / MOTIVO", "SEVERIDAD", "CASOS", "% PARTICIP."));
            sb.AppendLine(new string('-', 75));
            foreach (var s in _rankingSintomas)
            {
                sb.AppendLine(string.Format("{0,-35} | {1,-10} | {2,8} | {3,10:F2} %",
                    s.Sintoma.Length > 35 ? s.Sintoma.Substring(0, 32) + "..." : s.Sintoma,
                    s.Gravedad,
                    s.CantidadCasos,
                    s.Porcentaje));
            }
            sb.AppendLine();

            sb.AppendLine("3. DETALLE DE INGRESOS REGISTRADOS");
            sb.AppendLine("------------------------------------------------------------------------------------------");
            sb.AppendLine(string.Format("{0,-17} | {1,-7} | {2,-25} | {3,-9} | {4,-7} | {5,-12} | {6,-12}",
                "FECHA/HORA", "TURNO", "PACIENTE", "DNI", "TRIAGE", "ESTADO", "SALA"));
            sb.AppendLine(new string('-', 95));
            foreach (var d in _detallesFiltrados)
            {
                sb.AppendLine(string.Format("{0,-17:dd/MM/yyyy HH:mm} | {1,-7} | {2,-25} | {3,-9} | {4,-7} | {5,-12} | {6,-12}",
                    d.Fecha,
                    d.NroOrden,
                    d.PacienteCompleto.Length > 25 ? d.PacienteCompleto.Substring(0, 22) + "..." : d.PacienteCompleto,
                    d.DniPaciente,
                    d.Prioridad,
                    d.Estado,
                    d.NombreSala));
                if (!string.IsNullOrWhiteSpace(d.Sintomas) && d.Sintomas != "Sin síntomas registrados")
                {
                    sb.AppendLine($"   └─ Síntomas: {d.Sintomas}");
                }
            }
            sb.AppendLine("==========================================================================================");

            File.WriteAllText(rutaArchivo, sb.ToString(), Encoding.UTF8);
        }
    }
}
