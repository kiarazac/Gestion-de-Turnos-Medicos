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
    /// Panel de control financiero exclusivo para el perfil Gerente / Dueño de Negocio.
    /// Centraliza el control del flujo de dinero del establecimiento:
    /// 1. KPIs del período (facturado, caja, obras sociales, ticket, turnos y ausentismo) con variación contra el período anterior.
    /// 2. Evolución de la recaudación por día (o por mes en rangos largos).
    /// 3. Rankings de facturación por médico, especialidad y obra social.
    /// 4. Facturación por modalidad (Emergencia / Especialidad) y cobertura (Obra Social / Particular).
    /// </summary>
    public partial class FrmReportesGerente : Form
    {
        private static readonly Color ColorTeal = Color.FromArgb(15, 118, 110);
        private static readonly Color ColorBarraCaja = Color.FromArgb(20, 184, 166);
        private static readonly Color ColorBarraObraSocial = Color.FromArgb(56, 189, 248);
        private static readonly Color ColorVerde = Color.FromArgb(22, 101, 52);
        private static readonly Color ColorRojo = Color.FromArgb(220, 38, 38);
        private static readonly Color ColorAzul = Color.FromArgb(3, 105, 161);
        private static readonly Color ColorGris = Color.FromArgb(100, 116, 139);
        private static readonly Color ColorAdvertencia = Color.FromArgb(194, 65, 12);
        private static readonly Font FuenteBarra = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        private static readonly Color[] ColoresPodio =
        {
            Color.FromArgb(254, 243, 199), // Oro
            Color.FromArgb(226, 232, 240), // Plata
            Color.FromArgb(255, 237, 213)  // Bronce
        };

        private readonly ReporteBLL _reporteBLL = new ReporteBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        private DateTime _desde = DateTime.Today;
        private DateTime _hasta = DateTime.Today;
        private ReporteFinanzasResumenDTO _resumen = new ReporteFinanzasResumenDTO();
        private ReporteFinanzasResumenDTO _resumenAnterior = new ReporteFinanzasResumenDTO();
        private List<ReporteRecaudacionDiariaDTO> _evolucion = new List<ReporteRecaudacionDiariaDTO>();
        private List<ReporteRankingFacturacionDTO> _rankingMedicos = new List<ReporteRankingFacturacionDTO>();
        private List<ReporteRankingFacturacionDTO> _rankingEspecialidades = new List<ReporteRankingFacturacionDTO>();
        private List<ReporteRankingFacturacionDTO> _rankingObrasSociales = new List<ReporteRankingFacturacionDTO>();
        private List<ReporteRankingFacturacionDTO> _segmentos = new List<ReporteRankingFacturacionDTO>();

        private (Label Valor, Label Detalle) _kpiFacturado, _kpiCaja, _kpiObraSocial, _kpiTicket, _kpiTurnos, _kpiAusentismo;

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
            CrearTarjetasKpi();
            ConfigurarGrillas();
            CargarEspecialidades();
            cmbPeriodo.SelectedIndex = 0; // "Hoy" por defecto para controlar la recaudación del día
            CargarDatos();
            cmbEspecialidad.SelectedIndexChanged += CmbEspecialidad_SelectedIndexChanged;
        }

        #region Tarjetas KPI

        private void CrearTarjetasKpi()
        {
            _kpiFacturado = CrearTarjetaKpi(0, "TOTAL FACTURADO", ColorVerde);
            _kpiCaja = CrearTarjetaKpi(1, "COBRADO EN CAJA", ColorTeal);
            _kpiObraSocial = CrearTarjetaKpi(2, "A COBRAR A OBRAS SOCIALES", ColorAzul);
            _kpiTicket = CrearTarjetaKpi(3, "TICKET PROMEDIO", Color.FromArgb(30, 41, 59));
            _kpiTurnos = CrearTarjetaKpi(4, "TURNOS FACTURADOS", Color.FromArgb(109, 40, 217));
            _kpiAusentismo = CrearTarjetaKpi(5, "AUSENTISMO", ColorAdvertencia);
        }

        private (Label Valor, Label Detalle) CrearTarjetaKpi(int columna, string titulo, Color color)
        {
            var tarjeta = new Panel { BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Dock = DockStyle.Fill, Margin = new Padding(4) };
            var franja = new Panel { BackColor = color, Dock = DockStyle.Left, Width = 4 };
            var lblTitulo = new Label { Text = titulo, AutoSize = true, Location = new Point(10, 6), Font = new Font("Segoe UI", 7.5F, FontStyle.Bold), ForeColor = ColorGris };
            var lblValor = new Label { Text = "$ 0,00", AutoSize = true, Location = new Point(9, 21), Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = color };
            var lblDetalle = new Label { Text = string.Empty, AutoSize = true, Location = new Point(10, 47), Font = new Font("Segoe UI", 7.5F), ForeColor = ColorGris };

            tarjeta.Controls.AddRange(new Control[] { lblDetalle, lblValor, lblTitulo, franja });
            tlpKpis.Controls.Add(tarjeta, columna, 0);
            return (lblValor, lblDetalle);
        }

        private void ActualizarKpis()
        {
            var r = _resumen;
            var a = _resumenAnterior;

            _kpiFacturado.Valor.Text = $"$ {r.TotalFacturado:N2}";
            MostrarVariacion(_kpiFacturado.Detalle, r.TotalFacturado, a.TotalFacturado, "Caja + obras sociales");

            _kpiCaja.Valor.Text = $"$ {r.CobradoCaja:N2}";
            MostrarVariacion(_kpiCaja.Detalle, r.CobradoCaja, a.CobradoCaja, $"{Proporcion(r.CobradoCaja, r.TotalFacturado):P0} del facturado");

            _kpiObraSocial.Valor.Text = $"$ {r.ACobrarObraSocial:N2}";
            MostrarVariacion(_kpiObraSocial.Detalle, r.ACobrarObraSocial, a.ACobrarObraSocial, $"{Proporcion(r.ACobrarObraSocial, r.TotalFacturado):P0} del facturado");

            _kpiTicket.Valor.Text = $"$ {r.TicketPromedio:N2}";
            MostrarVariacion(_kpiTicket.Detalle, r.TicketPromedio, a.TicketPromedio, "Por turno facturado");

            _kpiTurnos.Valor.Text = $"{r.TotalTurnos} turnos";
            MostrarVariacion(_kpiTurnos.Detalle, r.TotalTurnos, a.TotalTurnos, $"Atendidos {r.TurnosAtendidos} · Pendientes {r.TurnosPendientes}");

            _kpiAusentismo.Valor.Text = $"{r.TurnosAusentes} ({Proporcion(r.TurnosAusentes, r.TotalTurnos):P1})";
            MostrarVariacion(_kpiAusentismo.Detalle, r.TurnosAusentes, a.TurnosAusentes, $"$ {r.FacturadoAusentes:N0} sin atención", invertirColores: true);
        }

        /// <summary>
        /// Muestra la variación contra el período anterior (▲ verde / ▼ rojo; invertido para indicadores donde subir es malo).
        /// </summary>
        private static void MostrarVariacion(Label lbl, decimal actual, decimal anterior, string complemento, bool invertirColores = false)
        {
            decimal? variacion = ReporteBLL.CalcularVariacion(actual, anterior);

            if (variacion == null)
            {
                lbl.Text = $"— sin movimiento anterior\n{complemento}";
                lbl.ForeColor = ColorGris;
                return;
            }

            bool sube = variacion.Value >= 0;
            lbl.Text = $"{(sube ? "▲" : "▼")} {Math.Abs(variacion.Value):N1}% vs. período anterior\n{complemento}";
            lbl.ForeColor = (sube ^ invertirColores) ? ColorVerde : ColorRojo;
        }

        private static decimal Proporcion(decimal parte, decimal total) => total == 0 ? 0 : parte / total;

        #endregion

        #region Grillas

        private void ConfigurarGrillas()
        {
            // 1. Evolución de la recaudación
            ConfigurarGrillaBase(dgvEvolucion);
            AgregarColumna(dgvEvolucion, "colFecha", "Fecha", nameof(ReporteRecaudacionDiariaDTO.Fecha), 150, "dddd dd/MM/yyyy", negrita: true);
            AgregarColumna(dgvEvolucion, "colTurnos", "Turnos", nameof(ReporteRecaudacionDiariaDTO.CantidadTurnos), 70, alineacion: DataGridViewContentAlignment.MiddleCenter);
            AgregarColumnaMonto(dgvEvolucion, "colCaja", "Cobrado en Caja", nameof(ReporteRecaudacionDiariaDTO.CobradoCaja));
            AgregarColumnaMonto(dgvEvolucion, "colObraSocial", "A Cobrar a OS", nameof(ReporteRecaudacionDiariaDTO.ACobrarObraSocial));
            AgregarColumnaMonto(dgvEvolucion, "colTotal", "Total Facturado", nameof(ReporteRecaudacionDiariaDTO.TotalFacturado), ColorVerde);
            AgregarColumnaBarra(dgvEvolucion, "Composición (verde: caja · celeste: obra social)", nameof(ReporteRecaudacionDiariaDTO.TotalFacturado), "C0");

            // 2. Ranking de médicos
            ConfigurarGrillaRanking(dgvRankingMedicos);
            AgregarColumna(dgvRankingMedicos, "colNombre", "Profesional Médico", nameof(ReporteRankingFacturacionDTO.Nombre), 190, negrita: true);
            AgregarColumna(dgvRankingMedicos, "colDetalle", "Especialidades Asignadas", nameof(ReporteRankingFacturacionDTO.Detalle), 170);
            AgregarColumna(dgvRankingMedicos, "colMatricula", "Matrícula", nameof(ReporteRankingFacturacionDTO.Matricula), 85, alineacion: DataGridViewContentAlignment.MiddleCenter);
            AgregarColumnasFacturacion(dgvRankingMedicos, "Atenciones");

            // 3. Ranking de especialidades
            ConfigurarGrillaRanking(dgvRankingEspecialidades);
            AgregarColumna(dgvRankingEspecialidades, "colNombre", "Especialidad", nameof(ReporteRankingFacturacionDTO.Nombre), 190, negrita: true);
            AgregarColumna(dgvRankingEspecialidades, "colDetalle", "Modalidad", nameof(ReporteRankingFacturacionDTO.Detalle), 140);
            AgregarColumnasFacturacion(dgvRankingEspecialidades, "Turnos");

            // 4. Ranking de obras sociales
            ConfigurarGrillaRanking(dgvRankingObrasSociales);
            AgregarColumna(dgvRankingObrasSociales, "colNombre", "Obra Social / Prepaga", nameof(ReporteRankingFacturacionDTO.Nombre), 190, negrita: true);
            AgregarColumna(dgvRankingObrasSociales, "colTurnos", "Turnos", nameof(ReporteRankingFacturacionDTO.CantidadTurnos), 65, alineacion: DataGridViewContentAlignment.MiddleCenter);
            AgregarColumnaMonto(dgvRankingObrasSociales, "colCaja", "Copagos en Caja", nameof(ReporteRankingFacturacionDTO.CobradoCaja));
            AgregarColumnaMonto(dgvRankingObrasSociales, "colObraSocial", "A Cobrar a la OS", nameof(ReporteRankingFacturacionDTO.ACobrarObraSocial), ColorAzul);
            AgregarColumnaMonto(dgvRankingObrasSociales, "colTotal", "Total Facturado", nameof(ReporteRankingFacturacionDTO.TotalFacturado), ColorVerde);
            AgregarColumna(dgvRankingObrasSociales, "colVigente", "% Cobertura Vigente", nameof(ReporteRankingFacturacionDTO.PorcentajeCoberturaVigente), 135, "0.00' %'", DataGridViewContentAlignment.MiddleRight);
            AgregarColumna(dgvRankingObrasSociales, "colAplicada", "% Cobertura Aplicada", nameof(ReporteRankingFacturacionDTO.PorcentajeCoberturaAplicada), 140, "0.00' %'", DataGridViewContentAlignment.MiddleRight);
            AgregarColumnaBarra(dgvRankingObrasSociales, "% Participación", nameof(ReporteRankingFacturacionDTO.PorcentajeParticipacion), "0.0' %'", escala: 100m);

            // 5. Modalidad y cobertura
            ConfigurarGrillaBase(dgvSegmentos);
            dgvSegmentos.DataBindingComplete += ResaltarFilas;
            AgregarColumna(dgvSegmentos, "colDetalle", "Agrupación", nameof(ReporteRankingFacturacionDTO.Detalle), 110, alineacion: DataGridViewContentAlignment.MiddleCenter);
            AgregarColumna(dgvSegmentos, "colNombre", "Segmento", nameof(ReporteRankingFacturacionDTO.Nombre), 180, negrita: true);
            AgregarColumnasFacturacion(dgvSegmentos, "Turnos");
        }

        private void ConfigurarGrillaBase(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = true;
            dgv.RowTemplate.Height = 30;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(226, 232, 240);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Padding = new Padding(4)
            };
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 251, 241);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgv.CellPainting += PintarBarra;
            // Sin selección inicial: la primera fila seleccionada tapaba el color del podio
            dgv.DataBindingComplete += (s, e) => dgv.ClearSelection();
        }

        private void ConfigurarGrillaRanking(DataGridView dgv)
        {
            ConfigurarGrillaBase(dgv);
            dgv.DataBindingComplete += ResaltarFilas;
            AgregarColumna(dgv, "colPosicion", "#", nameof(ReporteRankingFacturacionDTO.Posicion), 50, "0'°'", DataGridViewContentAlignment.MiddleCenter, negrita: true, color: ColorTeal);
        }

        private static void AgregarColumnasFacturacion(DataGridView dgv, string tituloCantidad)
        {
            AgregarColumna(dgv, "colTurnos", tituloCantidad, nameof(ReporteRankingFacturacionDTO.CantidadTurnos), 95, alineacion: DataGridViewContentAlignment.MiddleCenter, negrita: true);
            AgregarColumnaMonto(dgv, "colCaja", "Cobrado en Caja", nameof(ReporteRankingFacturacionDTO.CobradoCaja));
            AgregarColumnaMonto(dgv, "colObraSocial", "A Cobrar a OS", nameof(ReporteRankingFacturacionDTO.ACobrarObraSocial), ColorAzul);
            AgregarColumnaMonto(dgv, "colTotal", "Total Facturado", nameof(ReporteRankingFacturacionDTO.TotalFacturado), ColorVerde);
            AgregarColumnaMonto(dgv, "colTicket", "Ticket Prom.", nameof(ReporteRankingFacturacionDTO.TicketPromedio));
            AgregarColumnaBarra(dgv, "% Participación", nameof(ReporteRankingFacturacionDTO.PorcentajeParticipacion), "0.0' %'", escala: 100m);
        }

        private static void AgregarColumna(DataGridView dgv, string nombre, string titulo, string propiedad, int ancho, string? formato = null,
            DataGridViewContentAlignment alineacion = DataGridViewContentAlignment.MiddleLeft, bool negrita = false, Color? color = null)
        {
            var estilo = new DataGridViewCellStyle { Alignment = alineacion, NullValue = "—" };
            if (formato != null) estilo.Format = formato;
            if (negrita) estilo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            if (color.HasValue) estilo.ForeColor = color.Value;

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = nombre,
                HeaderText = titulo,
                DataPropertyName = propiedad,
                Width = ancho,
                DefaultCellStyle = estilo
            });
        }

        private static void AgregarColumnaMonto(DataGridView dgv, string nombre, string titulo, string propiedad, Color? color = null)
        {
            AgregarColumna(dgv, nombre, titulo, propiedad, 115, "C2", DataGridViewContentAlignment.MiddleRight, negrita: color.HasValue, color: color);
        }

        /// <summary>
        /// Columna que se dibuja como barra horizontal proporcional (ver <see cref="PintarBarra"/>). El Tag guarda el valor que ocupa la barra completa.
        /// </summary>
        private static void AgregarColumnaBarra(DataGridView dgv, string titulo, string propiedad, string formato, decimal escala = 1m)
        {
            AgregarColumna(dgv, "colBarra", titulo, propiedad, 220, formato);
            var columna = dgv.Columns["colBarra"]!;
            columna.Tag = escala;
            columna.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            columna.MinimumWidth = 160;
        }

        /// <summary>
        /// Dibuja las celdas de columnas barra: una barra proporcional al valor (en la evolución diaria, dividida en caja y obra social) con el valor encima.
        /// </summary>
        private void PintarBarra(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv || e.RowIndex < 0 || e.ColumnIndex < 0 || e.Graphics == null) return;
            if (dgv.Columns[e.ColumnIndex].Tag is not decimal escala || escala <= 0) return;

            e.PaintBackground(e.CellBounds, (e.State & DataGridViewElementStates.Selected) != 0);

            var area = new Rectangle(e.CellBounds.X + 6, e.CellBounds.Y + 7, e.CellBounds.Width - 12, e.CellBounds.Height - 14);
            var tramos = dgv.Rows[e.RowIndex].DataBoundItem is ReporteRecaudacionDiariaDTO dia
                ? new[] { (dia.CobradoCaja, ColorBarraCaja), (dia.ACobrarObraSocial, ColorBarraObraSocial) }
                : new[] { (Convert.ToDecimal(e.Value ?? 0m), ColorBarraCaja) };

            float x = area.X;
            foreach (var (valor, color) in tramos)
            {
                float ancho = (float)(Math.Min(Math.Max(valor, 0m), escala) / escala) * area.Width;
                if (ancho <= 0) continue;
                using var pincel = new SolidBrush(color);
                e.Graphics.FillRectangle(pincel, x, area.Y, ancho, area.Height);
                x += ancho;
            }

            TextRenderer.DrawText(e.Graphics, e.FormattedValue?.ToString(), FuenteBarra,
                area, Color.FromArgb(30, 41, 59), TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            e.Handled = true;
        }

        /// <summary>
        /// Resalta el podio (1°, 2°, 3°), atenúa las filas sin facturación y marca las obras sociales con cobertura aplicada distinta a la vigente.
        /// </summary>
        private void ResaltarFilas(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (sender is not DataGridView dgv) return;

            foreach (DataGridViewRow fila in dgv.Rows)
            {
                if (fila.DataBoundItem is not ReporteRankingFacturacionDTO item) continue;

                if (item.TotalFacturado == 0)
                {
                    fila.DefaultCellStyle.ForeColor = Color.FromArgb(148, 163, 184);
                }
                else if (dgv != dgvSegmentos && item.Posicion <= ColoresPodio.Length)
                {
                    fila.DefaultCellStyle.BackColor = ColoresPodio[item.Posicion - 1];
                }

                if (item.CoberturaConDiferencia && dgv.Columns.Contains("colAplicada"))
                {
                    var celda = fila.Cells["colAplicada"];
                    celda.Style.Format = "'⚠ '0.00' %'";
                    celda.Style.ForeColor = ColorAdvertencia;
                    celda.Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    celda.ToolTipText = $"Vigente {item.PorcentajeCoberturaVigente:0.##} % · aplicada en caja {item.PorcentajeCoberturaAplicada:0.##} %";
                }
            }
        }

        #endregion

        #region Filtros y carga

        private void CargarEspecialidades()
        {
            var opciones = new List<EspecialidadDTO> { new EspecialidadDTO { IdEspecialidad = 0, Nombre = "Todas las especialidades" } };
            try
            {
                opciones.AddRange(_reporteBLL.ObtenerEspecialidadesParaFiltro());
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las especialidades para el filtro:\n" + ex.Message,
                    "Filtro de Especialidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            cmbEspecialidad.DisplayMember = nameof(EspecialidadDTO.Nombre);
            cmbEspecialidad.ValueMember = nameof(EspecialidadDTO.IdEspecialidad);
            cmbEspecialidad.DataSource = opciones;
        }

        private int? IdEspecialidadSeleccionada => cmbEspecialidad.SelectedValue is int id && id > 0 ? id : null;

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

        private void CmbEspecialidad_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _rankingMedicos = _reporteBLL.ObtenerRankingMedicos(_desde, _hasta, IdEspecialidadSeleccionada);
                Enlazar(dgvRankingMedicos, _rankingMedicos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al filtrar el ranking de médicos:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
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

                var anterior = ReporteBLL.ObtenerPeriodoAnterior(desde, hasta);
                _resumen = _reporteBLL.ObtenerFinanzasResumen(desde, hasta);
                _resumenAnterior = _reporteBLL.ObtenerFinanzasResumen(anterior.Desde, anterior.Hasta);
                _evolucion = _reporteBLL.ObtenerRecaudacionDiaria(desde, hasta);
                _rankingMedicos = _reporteBLL.ObtenerRankingMedicos(desde, hasta, IdEspecialidadSeleccionada);
                _rankingEspecialidades = _reporteBLL.ObtenerRankingEspecialidades(desde, hasta);
                _rankingObrasSociales = _reporteBLL.ObtenerRankingObrasSociales(desde, hasta);
                _segmentos = _reporteBLL.ObtenerFacturacionPorSegmento(desde, hasta);
                _desde = desde;
                _hasta = hasta;

                // La barra de la evolución se escala contra el día (o mes) de mayor facturación
                dgvEvolucion.Columns["colBarra"]!.Tag = _evolucion.Select(d => d.TotalFacturado).DefaultIfEmpty(0m).Max();
                dgvEvolucion.Columns["colFecha"]!.DefaultCellStyle.Format = _evolucion.FirstOrDefault()?.AgrupadoPorMes == true ? "MMMM yyyy" : "dddd dd/MM/yyyy";

                Enlazar(dgvEvolucion, _evolucion);
                Enlazar(dgvRankingMedicos, _rankingMedicos);
                Enlazar(dgvRankingEspecialidades, _rankingEspecialidades);
                Enlazar(dgvRankingObrasSociales, _rankingObrasSociales);
                Enlazar(dgvSegmentos, _segmentos);

                lblLeyendaCobertura.Visible = _rankingObrasSociales.Any(o => o.CoberturaConDiferencia);
                ActualizarKpis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al cargar la información de recaudación:\n" + ex.Message,
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private static void Enlazar<T>(DataGridView dgv, List<T> datos)
        {
            dgv.DataSource = null;
            dgv.DataSource = datos;
        }

        #endregion

        private void BtnExportar_Click(object? sender, EventArgs e)
        {
            if (_resumen.TotalTurnos == 0)
            {
                MessageBox.Show("No hay recaudación registrada en el rango de fechas seleccionado.",
                    "Exportar Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
                sfd.Filter = "Documento Oficial PDF (*.pdf)|*.pdf";
                sfd.FilterIndex = 1;
                sfd.FileName = $"Reporte_Gerencial_Recaudacion_{timestamp}.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;

                        ExportadorPdf.ExportarReporteGerencial(
                            sfd.FileName,
                            _resumen,
                            _resumenAnterior,
                            _evolucion,
                            _rankingMedicos,
                            _rankingEspecialidades,
                            _rankingObrasSociales,
                            _segmentos,
                            cmbEspecialidad.Text,
                            _desde,
                            _hasta,
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
