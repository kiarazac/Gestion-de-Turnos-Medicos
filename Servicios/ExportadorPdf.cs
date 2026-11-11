using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gestion_de_Turnos_Medicos.CapaNegocio;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Color = QuestPDF.Infrastructure.Color;

namespace Gestion_de_Turnos_Medicos.Servicios
{
    /// <summary>
    /// Servicio centralizado de reportería oficial en formato PDF inmutable y de alta fidelidad visual.
    /// Emplea el motor tipográfico vectorial QuestPDF para garantizar la seguridad, trazabilidad
    /// e inmutabilidad de los reportes gerenciales, cierres de caja y estadísticas clínicas.
    /// </summary>
    public static class ExportadorPdf
    {
        static ExportadorPdf()
        {
            // Licencia comunitaria de QuestPDF para aplicaciones internas y de código abierto
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.UseSystemFonts = true;
        }

        // Paleta de colores corporativos médicos
        private static readonly Color ColorPrimario = Color.FromHex("#0F766E");       // Teal institucional médico
        private static readonly Color ColorSecundario = Color.FromHex("#1E293B");     // Slate oscuro para textos
        private static readonly Color ColorFondoTabla = Color.FromHex("#F8FAFC");     // Zebra row
        private static readonly Color ColorBorde = Color.FromHex("#CBD5E1");          // Slate claro para divisores
        private static readonly Color ColorExito = Color.FromHex("#166534");          // Verde para totales
        private static readonly Color ColorAcento = Color.FromHex("#0369A1");         // Azul corporativo
        private static readonly Color ColorKpiFondo = Color.FromHex("#F1F5F9");       // Gris suave para KPIs
        private static readonly Color ColorAlerta = Color.FromHex("#DC2626");         // Rojo para confidencial
        private static readonly Color ColorGrisTexto = Color.FromHex("#64748B");      // Gris para metadatos
        private static readonly Color ColorBlanco = Colors.White;

        #region 1. Reporte de Cierre Diario de Caja (Recepcionista)

        /// <summary>
        /// Genera el reporte oficial de cierre de caja y turnos emitidos en PDF inmutable.
        /// </summary>
        public static void ExportarCierreCaja(
            string rutaArchivo,
            ReporteCierreCajaResumenDTO resumen,
            List<ReporteCierreCajaDetalleDTO> detalles,
            DateTime fechaCierre,
            UsuarioLoginResult? usuarioActual,
            string? filtroEspecialidad = null,
            string? tituloPersonalizado = null)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(ColorBlanco);
                    page.DefaultTextStyle(x => x.FontSize(9).FontColor(ColorSecundario).FontFamily(Fonts.Lato));

                    // Encabezado
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("CENTRO MÉDICO DE ESPECIALIDADES")
                                    .FontSize(14).Bold().FontColor(ColorPrimario);

                                string titulo = !string.IsNullOrWhiteSpace(tituloPersonalizado)
                                    ? tituloPersonalizado
                                    : (!string.IsNullOrWhiteSpace(filtroEspecialidad)
                                        ? "REPORTE OFICIAL DE TURNOS Y FACTURACIÓN POR ESPECIALIDAD"
                                        : "REPORTE OFICIAL DE CIERRE DIARIO DE CAJA Y TURNOS");

                                c.Item().Text(titulo)
                                    .FontSize(11).Bold().FontColor(ColorSecundario);

                                string subtituloFecha = $"Fecha Auditada: {fechaCierre:dd/MM/yyyy}";
                                if (!string.IsNullOrWhiteSpace(filtroEspecialidad) &&
                                    !filtroEspecialidad.Equals("Todas", StringComparison.OrdinalIgnoreCase) &&
                                    !filtroEspecialidad.Equals("Todas las Especialidades", StringComparison.OrdinalIgnoreCase))
                                {
                                    subtituloFecha += $"  |  Especialidad: {filtroEspecialidad}";
                                }
                                c.Item().Text(subtituloFecha)
                                    .FontSize(10).SemiBold();
                            });

                            row.ConstantItem(180).AlignRight().Column(c =>
                            {
                                c.Item().Text($"Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8);
                                c.Item().Text($"Operador: {usuarioActual?.Nombre} {usuarioActual?.Apellido}").FontSize(8);
                                c.Item().Text($"Rol: {usuarioActual?.NombreRol ?? "Recepcionista"}").FontSize(8);
                                c.Item().Text("DOCUMENTO OFICIAL AUDITADO").FontSize(7).Bold().FontColor(ColorPrimario);
                            });
                        });

                        col.Item().PaddingTop(5).LineHorizontal(1.5f).LineColor(ColorPrimario);
                    });

                    // Contenido
                    page.Content().PaddingTop(10).Column(col =>
                    {
                        // Tarjetas de Resumen Financiero (KPIs)
                        decimal montoCoberturaOS = CajaBLL.CalcularCoberturaObraSocial(detalles);
                        decimal facturacionTotalBruta = resumen.MontoParticulares + (resumen.TurnosObraSocial > 0 ? (resumen.MontoObraSocial + montoCoberturaOS) : 0m);
                        decimal promedioPorTurno = resumen.TotalTurnos > 0 ? Math.Round(resumen.TotalCobrado / resumen.TotalTurnos, 2) : 0m;

                        col.Item().Border(1).BorderColor(ColorBorde).Padding(8).Background(ColorKpiFondo).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("TOTAL TURNOS").FontSize(8).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"{resumen.TotalTurnos}").FontSize(14).Bold().FontColor(ColorPrimario);
                                c.Item().Text($"Esp: {resumen.TurnosEspecialidad} | Urg: {resumen.TurnosEmergencia}").FontSize(7);
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("RECAUDACIÓN CAJA (PACIENTES)").FontSize(8).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"$ {resumen.TotalCobrado:N2}").FontSize(14).Bold().FontColor(ColorExito);
                                c.Item().Text(resumen.CantidadDevoluciones > 0
                                    ? $"Devuelto: $ {resumen.TotalDevuelto:N2} | Neto: $ {resumen.TotalRecaudado:N2}"
                                    : "Abonado en ventanilla").FontSize(7);
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("COBERTURA OBRAS SOCIALES").FontSize(8).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"$ {montoCoberturaOS:N2}").FontSize(14).Bold().FontColor(ColorAcento);
                                c.Item().Text($"OS: {resumen.TurnosObraSocial} | Part: {resumen.TurnosParticulares}").FontSize(7);
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("FACTURACIÓN TOTAL BRUTA").FontSize(8).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"$ {facturacionTotalBruta:N2}").FontSize(14).Bold().FontColor(ColorSecundario);
                                c.Item().Text($"Promedio: $ {promedioPorTurno:N2}").FontSize(7);
                            });
                        });

                        string tituloTabla = !string.IsNullOrWhiteSpace(filtroEspecialidad) &&
                                             !filtroEspecialidad.Equals("Todas", StringComparison.OrdinalIgnoreCase) &&
                                             !filtroEspecialidad.Equals("Todas las Especialidades", StringComparison.OrdinalIgnoreCase)
                            ? $"DETALLE DE TURNOS Y FACTURACIÓN ({filtroEspecialidad.ToUpperInvariant()})"
                            : "DETALLE DE TURNOS Y FACTURACIÓN DE LA JORNADA";

                        col.Item().PaddingTop(12).Text(tituloTabla)
                            .FontSize(10).Bold().FontColor(ColorPrimario);

                        // Tabla de Detalle
                        col.Item().PaddingTop(5).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);  // N° Orden
                                columns.ConstantColumn(48);  // Fecha Turno
                                columns.ConstantColumn(35);  // Hora
                                columns.RelativeColumn(2.5f);// Paciente
                                columns.ConstantColumn(55);  // DNI
                                columns.RelativeColumn(2f);  // Obra Social
                                columns.ConstantColumn(58);  // Tipo
                                columns.RelativeColumn(2f);  // Profesional / Sala
                                columns.ConstantColumn(60);  // Monto Cobrado
                            });

                            // Cabecera de la tabla
                            tabla.Header(header =>
                            {
                                header.Cell().Background(ColorPrimario).Padding(4).Text("Turno").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                header.Cell().Background(ColorPrimario).Padding(4).Text("Fecha").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                header.Cell().Background(ColorPrimario).Padding(4).Text("Hora").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                header.Cell().Background(ColorPrimario).Padding(4).Text("Paciente").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                header.Cell().Background(ColorPrimario).Padding(4).Text("DNI").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                header.Cell().Background(ColorPrimario).Padding(4).Text("Cobertura").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                header.Cell().Background(ColorPrimario).Padding(4).Text("Tipo").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                header.Cell().Background(ColorPrimario).Padding(4).Text("Médico / Sala").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                header.Cell().Background(ColorPrimario).Padding(4).AlignRight().Text("Monto Caja").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                            });

                            // Filas de datos
                            for (int i = 0; i < detalles.Count; i++)
                            {
                                var d = detalles[i];
                                var fondoFila = (i % 2 == 1) ? ColorFondoTabla : ColorBlanco;

                                tabla.Cell().Background(fondoFila).Padding(4).Text(d.NroOrden ?? "-").FontSize(7.5f);
                                tabla.Cell().Background(fondoFila).Padding(4).Text(d.Fecha.ToString("dd/MM/yyyy")).FontSize(7.5f);
                                tabla.Cell().Background(fondoFila).Padding(4).Text(d.Horario.ToString(@"hh\:mm")).FontSize(7.5f);
                                tabla.Cell().Background(fondoFila).Padding(4).Text(d.PacienteCompleto).FontSize(7.5f).SemiBold();
                                tabla.Cell().Background(fondoFila).Padding(4).Text(d.DniPaciente).FontSize(7.5f);
                                tabla.Cell().Background(fondoFila).Padding(4).Text(d.ObraSocial).FontSize(7.5f);
                                tabla.Cell().Background(fondoFila).Padding(4).Text(d.TipoTurno).FontSize(7.5f);
                                tabla.Cell().Background(fondoFila).Padding(4).Text(!string.IsNullOrWhiteSpace(d.MedicoAsignado) ? d.MedicoAsignado : d.Especialidad).FontSize(7.5f);
                                tabla.Cell().Background(fondoFila).Padding(4).AlignRight().Text($"$ {d.MontoCobrado:N2}").FontSize(7.5f).Bold();
                            }
                        });

                        // Totales finales: lo cobrado y lo devuelto por separado, y el neto que queda en la caja
                        if (resumen.CantidadDevoluciones > 0)
                        {
                            col.Item().PaddingTop(6).AlignRight().Text($"COBRADO: $ {resumen.TotalCobrado:N2}").FontSize(9).Bold();
                            col.Item().AlignRight().Text($"DEVUELTO ({resumen.CantidadDevoluciones}): $ {resumen.TotalDevuelto:N2}").FontSize(9).Bold();
                        }

                        col.Item().PaddingTop(resumen.CantidadDevoluciones > 0 ? 2 : 6).AlignRight().Text($"NETO EN CAJA: $ {resumen.TotalRecaudado:N2}")
                            .FontSize(11).Bold().FontColor(ColorExito);
                    });

                    // Pie de Página
                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(ColorBorde);
                        col.Item().PaddingTop(3).Row(row =>
                        {
                            row.RelativeItem().Text("Documento oficial inmutable — Sistema de Gestión de Turnos Médicos")
                                .FontSize(7).Italic().FontColor(ColorGrisTexto);
                            row.RelativeItem().AlignRight().Text(x =>
                            {
                                x.Span("Página ");
                                x.CurrentPageNumber();
                                x.Span(" de ");
                                x.TotalPages();
                            });
                        });
                    });
                });
            });

            doc.GeneratePdf(rutaArchivo);
        }

        #endregion

        #region 2. Reporte Gerencial de Recaudación y Control del Flujo de Dinero (Gerente)

        private static readonly Color ColorBarraCaja = Color.FromHex("#14B8A6");      // Teal claro: cobrado en caja
        private static readonly Color ColorBarraObraSocial = Color.FromHex("#38BDF8"); // Celeste: a cobrar a obras sociales
        private static readonly Color ColorAdvertencia = Color.FromHex("#C2410C");    // Naranja: cobertura con diferencia
        private static readonly Color[] ColoresPodio = { Color.FromHex("#FEF3C7"), Color.FromHex("#E2E8F0"), Color.FromHex("#FFEDD5") };

        /// <summary>
        /// Genera el reporte gerencial oficial de recaudación (KPIs con variación, evolución, rankings de médicos,
        /// especialidades y obras sociales, y facturación por modalidad / cobertura) en PDF inmutable.
        /// </summary>
        public static void ExportarReporteGerencial(
            string rutaArchivo,
            ReporteFinanzasResumenDTO resumen,
            ReporteFinanzasResumenDTO resumenAnterior,
            List<ReporteRecaudacionDiariaDTO> evolucion,
            List<ReporteRankingFacturacionDTO> rankingMedicos,
            List<ReporteRankingFacturacionDTO> rankingEspecialidades,
            List<ReporteRankingFacturacionDTO> rankingObrasSociales,
            List<ReporteRankingFacturacionDTO> segmentos,
            string filtroEspecialidadMedicos,
            DateTime fechaDesde,
            DateTime fechaHasta,
            UsuarioLoginResult? usuarioActual)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(ColorBlanco);
                    page.DefaultTextStyle(x => x.FontSize(9).FontColor(ColorSecundario).FontFamily(Fonts.Lato));

                    // Encabezado
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("CENTRO MÉDICO DE ESPECIALIDADES")
                                    .FontSize(14).Bold().FontColor(ColorPrimario);
                                c.Item().Text("INFORME GERENCIAL: RECAUDACIÓN Y FLUJO DE DINERO")
                                    .FontSize(11).Bold().FontColor(ColorSecundario);
                                c.Item().Text($"Período Auditado: {fechaDesde:dd/MM/yyyy} al {fechaHasta:dd/MM/yyyy}")
                                    .FontSize(9).SemiBold();
                            });

                            row.ConstantItem(180).AlignRight().Column(c =>
                            {
                                c.Item().Text($"Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8);
                                c.Item().Text($"Gerencia: {usuarioActual?.Nombre} {usuarioActual?.Apellido}").FontSize(8);
                                c.Item().Text("CONTROL DE RECAUDACIÓN").FontSize(7).Bold().FontColor(ColorAlerta);
                                c.Item().Text("DOCUMENTO OFICIAL INMUTABLE").FontSize(7).Bold().FontColor(ColorPrimario);
                            });
                        });

                        col.Item().PaddingTop(5).LineHorizontal(1.5f).LineColor(ColorPrimario);
                    });

                    // Contenido
                    page.Content().PaddingTop(10).Column(col =>
                    {
                        // Tarjetas KPI con variación contra el período anterior de igual duración
                        var anterior = TextoPeriodoAnterior(fechaDesde, fechaHasta);
                        decimal facturado = resumen.TotalFacturado;

                        col.Item().Border(1).BorderColor(ColorBorde).Background(ColorKpiFondo).Padding(8).Column(kpis =>
                        {
                            kpis.Item().Row(row =>
                            {
                                KpiPdf(row, "TOTAL FACTURADO", $"$ {resumen.TotalFacturado:N2}", resumen.TotalFacturado, resumenAnterior.TotalFacturado, "Caja + obras sociales", ColorExito);
                                KpiPdf(row, "COBRADO EN CAJA", $"$ {resumen.CobradoCaja:N2}", resumen.CobradoCaja, resumenAnterior.CobradoCaja, $"{Proporcion(resumen.CobradoCaja, facturado):P0} del facturado", ColorPrimario);
                                KpiPdf(row, "A COBRAR A OBRAS SOCIALES", $"$ {resumen.ACobrarObraSocial:N2}", resumen.ACobrarObraSocial, resumenAnterior.ACobrarObraSocial, $"{Proporcion(resumen.ACobrarObraSocial, facturado):P0} del facturado", ColorAcento);
                            });
                            kpis.Item().PaddingTop(6).Row(row =>
                            {
                                KpiPdf(row, "TICKET PROMEDIO", $"$ {resumen.TicketPromedio:N2}", resumen.TicketPromedio, resumenAnterior.TicketPromedio, "Por turno facturado", ColorSecundario);
                                KpiPdf(row, "TURNOS FACTURADOS", $"{resumen.TotalTurnos} turnos", resumen.TotalTurnos, resumenAnterior.TotalTurnos, $"Atendidos {resumen.TurnosAtendidos} · Pendientes {resumen.TurnosPendientes}", ColorSecundario);
                                KpiPdf(row, "AUSENTISMO", $"{resumen.TurnosAusentes} ({Proporcion(resumen.TurnosAusentes, resumen.TotalTurnos):P1})", resumen.TurnosAusentes, resumenAnterior.TurnosAusentes, $"$ {resumen.FacturadoAusentes:N0} sin atención", ColorAdvertencia, invertirColores: true);
                            });
                            kpis.Item().PaddingTop(4).Text($"Variaciones calculadas contra el período anterior: {anterior}.")
                                .FontSize(6.5f).Italic().FontColor(ColorGrisTexto);
                        });

                        // SECCIÓN 1: Evolución de la recaudación
                        bool porMes = evolucion.FirstOrDefault()?.AgrupadoPorMes == true;
                        decimal maximoDia = evolucion.Select(d => d.TotalFacturado).DefaultIfEmpty(0m).Max();

                        col.Item().PaddingTop(12).Text(porMes ? "1. EVOLUCIÓN MENSUAL DE LA RECAUDACIÓN" : "1. EVOLUCIÓN DIARIA DE LA RECAUDACIÓN")
                            .FontSize(10).Bold().FontColor(ColorPrimario);
                        col.Item().Text("Barra: cobrado en caja (verde) + a cobrar a obras sociales (celeste), relativa al período de mayor facturación.")
                            .FontSize(6.5f).Italic().FontColor(ColorGrisTexto);

                        col.Item().PaddingTop(4).Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(80);   // Fecha
                                c.ConstantColumn(40);   // Turnos
                                c.ConstantColumn(70);   // Caja
                                c.ConstantColumn(70);   // OS
                                c.ConstantColumn(75);   // Total
                                c.RelativeColumn();     // Barra
                            });

                            EncabezadosPdf(t, ColorPrimario, "Fecha", "Turnos", "Caja", "Obras Soc.", "Total", "Composición");

                            for (int i = 0; i < evolucion.Count; i++)
                            {
                                var dia = evolucion[i];
                                var fondo = (i % 2 == 1) ? ColorFondoTabla : ColorBlanco;

                                CeldaPdf(t, fondo).Text(porMes ? $"{dia.Fecha:MMMM yyyy}" : $"{dia.Fecha:ddd dd/MM/yyyy}").FontSize(7.5f).SemiBold();
                                CeldaPdf(t, fondo).AlignCenter().Text($"{dia.CantidadTurnos}").FontSize(7.5f);
                                CeldaPdf(t, fondo).AlignRight().Text($"$ {dia.CobradoCaja:N2}").FontSize(7.5f);
                                CeldaPdf(t, fondo).AlignRight().Text($"$ {dia.ACobrarObraSocial:N2}").FontSize(7.5f);
                                CeldaPdf(t, fondo).AlignRight().Text($"$ {dia.TotalFacturado:N2}").FontSize(7.5f).Bold().FontColor(ColorExito);
                                BarraPdf(CeldaPdf(t, fondo), maximoDia, null, (dia.CobradoCaja, ColorBarraCaja), (dia.ACobrarObraSocial, ColorBarraObraSocial));
                            }
                        });

                        // SECCIÓN 2: Ranking de médicos
                        col.Item().PaddingTop(14).Text("2. RANKING DE MÉDICOS POR FACTURACIÓN DE ATENCIONES REALIZADAS")
                            .FontSize(10).Bold().FontColor(ColorPrimario);
                        col.Item().Text($"Especialidad de los turnos: {filtroEspecialidadMedicos}")
                            .FontSize(6.5f).Italic().FontColor(ColorGrisTexto);
                        TablaFacturacionPdf(col, rankingMedicos, "Profesional Médico", "Especialidades", "Atenc.", mostrarPosicion: true);

                        // SECCIÓN 3: Ranking de especialidades
                        col.Item().PaddingTop(14).Text("3. RANKING DE ESPECIALIDADES POR FACTURACIÓN")
                            .FontSize(10).Bold().FontColor(ColorPrimario);
                        TablaFacturacionPdf(col, rankingEspecialidades, "Especialidad", "Modalidad", "Turnos", mostrarPosicion: true);

                        // SECCIÓN 4: Ranking de obras sociales
                        col.Item().PaddingTop(14).Text("4. RANKING DE OBRAS SOCIALES: COPAGOS Y MONTOS A COBRAR")
                            .FontSize(10).Bold().FontColor(ColorPrimario);

                        col.Item().PaddingTop(4).Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(22);   // #
                                c.RelativeColumn(3);    // Obra Social
                                c.ConstantColumn(36);   // Turnos
                                c.ConstantColumn(62);   // Copagos
                                c.ConstantColumn(66);   // A cobrar
                                c.ConstantColumn(66);   // Total
                                c.ConstantColumn(42);   // % Vigente
                                c.ConstantColumn(46);   // % Aplicada
                                c.RelativeColumn(2);    // Barra
                            });

                            EncabezadosPdf(t, ColorSecundario, "#", "Obra Social / Prepaga", "Turnos", "Copagos", "A Cobrar OS", "Total", "% Vig.", "% Aplic.", "% Particip.");

                            for (int i = 0; i < rankingObrasSociales.Count; i++)
                            {
                                var os = rankingObrasSociales[i];
                                var fondo = FondoFilaRanking(os, i);
                                var colorAplicada = os.CoberturaConDiferencia ? ColorAdvertencia : ColorSecundario;

                                CeldaPdf(t, fondo).AlignCenter().Text($"{os.Posicion}").FontSize(7.5f).Bold();
                                CeldaPdf(t, fondo).Text(os.Nombre).FontSize(7.5f).SemiBold();
                                CeldaPdf(t, fondo).AlignCenter().Text($"{os.CantidadTurnos}").FontSize(7.5f);
                                CeldaPdf(t, fondo).AlignRight().Text($"$ {os.CobradoCaja:N2}").FontSize(7.5f);
                                CeldaPdf(t, fondo).AlignRight().Text($"$ {os.ACobrarObraSocial:N2}").FontSize(7.5f).Bold().FontColor(ColorAcento);
                                CeldaPdf(t, fondo).AlignRight().Text($"$ {os.TotalFacturado:N2}").FontSize(7.5f).Bold().FontColor(ColorExito);
                                CeldaPdf(t, fondo).AlignRight().Text($"{os.PorcentajeCoberturaVigente:0.##} %").FontSize(7.5f);
                                CeldaPdf(t, fondo).AlignRight().Text($"{(os.CoberturaConDiferencia ? "(!) " : "")}{os.PorcentajeCoberturaAplicada:0.##} %").FontSize(7.5f).Bold().FontColor(colorAplicada);
                                BarraPdf(CeldaPdf(t, fondo), 100m, $"{os.PorcentajeParticipacion:N1}%", (os.PorcentajeParticipacion, ColorBarraCaja));
                            }
                        });

                        if (rankingObrasSociales.Any(o => o.CoberturaConDiferencia))
                        {
                            col.Item().PaddingTop(2).Text("(!) La cobertura aplicada según lo cobrado en caja difiere de la vigente: el % de la obra social se modificó en el período o se cobró un monto distinto al configurado.")
                                .FontSize(6.5f).Italic().FontColor(ColorAdvertencia);
                        }

                        // SECCIÓN 5: Modalidad y cobertura
                        col.Item().PaddingTop(14).Text("5. FACTURACIÓN POR MODALIDAD Y COBERTURA")
                            .FontSize(10).Bold().FontColor(ColorPrimario);
                        TablaFacturacionPdf(col, segmentos, "Segmento", "Agrupación", "Turnos", mostrarPosicion: false);
                    });

                    // Pie de Página
                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(ColorBorde);
                        col.Item().PaddingTop(3).Row(row =>
                        {
                            row.RelativeItem().Text("Informe Gerencial Oficial Confidencial — Sistema de Gestión de Turnos Médicos")
                                .FontSize(7).Italic().FontColor(ColorGrisTexto);
                            row.RelativeItem().AlignRight().Text(x =>
                            {
                                x.Span("Página ");
                                x.CurrentPageNumber();
                                x.Span(" de ");
                                x.TotalPages();
                            });
                        });
                    });
                });
            });

            doc.GeneratePdf(rutaArchivo);
        }

        /// <summary>
        /// Tabla de facturación compartida por los rankings de médicos y especialidades y la tabla de modalidad / cobertura.
        /// </summary>
        private static void TablaFacturacionPdf(ColumnDescriptor col, List<ReporteRankingFacturacionDTO> datos,
            string tituloNombre, string tituloDetalle, string tituloCantidad, bool mostrarPosicion)
        {
            col.Item().PaddingTop(4).Table(t =>
            {
                t.ColumnsDefinition(c =>
                {
                    if (mostrarPosicion) c.ConstantColumn(22); // #
                    c.RelativeColumn(3);    // Nombre
                    c.RelativeColumn(3);    // Detalle
                    c.ConstantColumn(36);   // Cantidad
                    c.ConstantColumn(62);   // Caja
                    c.ConstantColumn(62);   // OS
                    c.ConstantColumn(66);   // Total
                    c.RelativeColumn(2);    // Barra
                });

                var titulos = new List<string> { tituloNombre, tituloDetalle, tituloCantidad, "Caja", "Obras Soc.", "Total", "% Particip." };
                if (mostrarPosicion) titulos.Insert(0, "#");
                EncabezadosPdf(t, ColorSecundario, titulos.ToArray());

                for (int i = 0; i < datos.Count; i++)
                {
                    var item = datos[i];
                    var fondo = mostrarPosicion ? FondoFilaRanking(item, i) : ((i % 2 == 1) ? ColorFondoTabla : ColorBlanco);
                    var colorTexto = item.TotalFacturado == 0 ? ColorGrisTexto : ColorSecundario;

                    if (mostrarPosicion) CeldaPdf(t, fondo).AlignCenter().Text($"{item.Posicion}").FontSize(7.5f).Bold().FontColor(colorTexto);
                    CeldaPdf(t, fondo).Text(item.Nombre).FontSize(7.5f).SemiBold().FontColor(colorTexto);
                    CeldaPdf(t, fondo).Text(item.Detalle ?? "—").FontSize(7).FontColor(colorTexto);
                    CeldaPdf(t, fondo).AlignCenter().Text($"{item.CantidadTurnos}").FontSize(7.5f).Bold().FontColor(colorTexto);
                    CeldaPdf(t, fondo).AlignRight().Text($"$ {item.CobradoCaja:N2}").FontSize(7.5f).FontColor(colorTexto);
                    CeldaPdf(t, fondo).AlignRight().Text($"$ {item.ACobrarObraSocial:N2}").FontSize(7.5f).FontColor(colorTexto);
                    CeldaPdf(t, fondo).AlignRight().Text($"$ {item.TotalFacturado:N2}").FontSize(7.5f).Bold().FontColor(item.TotalFacturado == 0 ? ColorGrisTexto : ColorExito);
                    BarraPdf(CeldaPdf(t, fondo), 100m, $"{item.PorcentajeParticipacion:N1}%", (item.PorcentajeParticipacion, ColorBarraCaja));
                }
            });
        }

        private static void KpiPdf(RowDescriptor row, string titulo, string valor, decimal actual, decimal anterior, string complemento, Color color, bool invertirColores = false)
        {
            decimal? variacion = ReporteBLL.CalcularVariacion(actual, anterior);
            bool sube = variacion >= 0;

            row.RelativeItem().Column(c =>
            {
                c.Item().Text(titulo).FontSize(7).Bold().FontColor(ColorGrisTexto);
                c.Item().Text(valor).FontSize(11).Bold().FontColor(color);
                c.Item().Text(variacion == null
                        ? "— sin movimiento anterior"
                        : $"{(sube ? "▲" : "▼")} {Math.Abs(variacion.Value):N1}% vs. período anterior")
                    .FontSize(7).SemiBold().FontColor(variacion == null ? ColorGrisTexto : ((sube ^ invertirColores) ? ColorExito : ColorAlerta));
                c.Item().Text(complemento).FontSize(6.5f).FontColor(ColorGrisTexto);
            });
        }

        private static void EncabezadosPdf(TableDescriptor t, Color fondo, params string[] titulos)
        {
            t.Header(h =>
            {
                foreach (var titulo in titulos)
                {
                    h.Cell().Background(fondo).Padding(3).Text(titulo).FontSize(7.5f).Bold().FontColor(ColorBlanco);
                }
            });
        }

        private static IContainer CeldaPdf(TableDescriptor t, Color fondo) => t.Cell().Background(fondo).Padding(3).AlignMiddle();

        private static Color FondoFilaRanking(ReporteRankingFacturacionDTO item, int indice)
        {
            if (item.TotalFacturado > 0 && item.Posicion >= 1 && item.Posicion <= ColoresPodio.Length)
            {
                return ColoresPodio[item.Posicion - 1];
            }
            return (indice % 2 == 1) ? ColorFondoTabla : ColorBlanco;
        }

        /// <summary>
        /// Dibuja una barra horizontal con uno o más tramos proporcionales a <paramref name="escala"/> y, opcionalmente, un texto a la derecha.
        /// </summary>
        private static void BarraPdf(IContainer celda, decimal escala, string? texto, params (decimal Valor, Color Color)[] tramos)
        {
            celda.Row(row =>
            {
                row.RelativeItem().AlignMiddle().Height(7).Background(ColorKpiFondo).Row(barra =>
                {
                    decimal resto = 1m;
                    foreach (var (valor, color) in tramos)
                    {
                        decimal fraccion = escala > 0 ? Math.Min(Math.Max(valor, 0m) / escala, resto) : 0m;
                        if (fraccion <= 0) continue;
                        barra.RelativeItem((float)fraccion).Background(color);
                        resto -= fraccion;
                    }
                    if (resto > 0) barra.RelativeItem((float)resto);
                });

                if (texto != null)
                {
                    row.ConstantItem(32).AlignRight().Text(texto).FontSize(7).Bold();
                }
            });
        }

        private static decimal Proporcion(decimal parte, decimal total) => total == 0 ? 0 : parte / total;

        private static string TextoPeriodoAnterior(DateTime fechaDesde, DateTime fechaHasta)
        {
            var anterior = ReporteBLL.ObtenerPeriodoAnterior(fechaDesde, fechaHasta);
            return $"{anterior.Desde:dd/MM/yyyy} al {anterior.Hasta:dd/MM/yyyy}";
        }

        #endregion

        #region 3. Reporte Clínico de Atenciones y Diagnósticos Frecuentes (Médico)

        /// <summary>
        /// Genera el reporte clínico oficial del médico con el historial de pacientes atendidos,
        /// detalle de diagnóstico por turno, y opcionalmente el anexo de gravedad y síntomas de triage si es clínico.
        /// </summary>
        public static void ExportarReporteMedico(
            string rutaArchivo,
            List<AtencionMedicoDTO> atenciones,
            List<ReporteMedicoGravedadDTO>? rankingGravedad,
            List<ReporteMedicoSintomaDTO>? rankingSintomas,
            DateTime fechaDesde,
            DateTime fechaHasta,
            UsuarioLoginResult? usuarioActual)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(ColorBlanco);
                    page.DefaultTextStyle(x => x.FontSize(9).FontColor(ColorSecundario).FontFamily(Fonts.Lato));

                    // Encabezado
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("CENTRO MÉDICO DE ESPECIALIDADES")
                                    .FontSize(14).Bold().FontColor(ColorPrimario);
                                c.Item().Text("INFORME CLÍNICO OFICIAL DE ATENCIONES MÉDICAS")
                                    .FontSize(11).Bold().FontColor(ColorSecundario);
                                c.Item().Text($"Profesional: Dr./Dra. {usuarioActual?.Nombre} {usuarioActual?.Apellido}")
                                    .FontSize(9).SemiBold();
                                c.Item().Text($"Período: {fechaDesde:dd/MM/yyyy} al {fechaHasta:dd/MM/yyyy}").FontSize(8);
                            });

                            row.ConstantItem(180).AlignRight().Column(c =>
                            {
                                c.Item().Text($"Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8);
                                c.Item().Text($"Matrícula/ID: MED-{usuarioActual?.IdUsuario:D4}").FontSize(8);
                                c.Item().Text("HISTORIAL CLÍNICO AUDITADO").FontSize(7).Bold().FontColor(ColorPrimario);
                                c.Item().Text("DOCUMENTO OFICIAL INMUTABLE").FontSize(7).Bold().FontColor(ColorAcento);
                            });
                        });

                        col.Item().PaddingTop(5).LineHorizontal(1.5f).LineColor(ColorPrimario);
                    });

                    // Contenido
                    page.Content().PaddingTop(10).Column(col =>
                    {
                        // Resumen
                        col.Item().Border(1).BorderColor(ColorBorde).Padding(8).Background(ColorKpiFondo).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("TOTAL ATENCIONES REALIZADAS").FontSize(8).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"{atenciones.Count}").FontSize(14).Bold().FontColor(ColorPrimario);
                            });

                            int emergenciasCount = atenciones.Count(a => a.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase));
                            int consultasCount = atenciones.Count(a => a.TipoTurno.Equals("Especialidad", StringComparison.OrdinalIgnoreCase));

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("CONSULTAS PROGRAMADAS").FontSize(8).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"{consultasCount}").FontSize(14).Bold().FontColor(ColorAcento);
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("URGENCIAS DE GUARDIA").FontSize(8).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"{emergenciasCount}").FontSize(14).Bold().FontColor(ColorAlerta);
                            });
                        });

                        // SECCIÓN 1: Anexo de Triage y Urgencias de Guardia (si aplica a médico clínico)
                        if (rankingGravedad != null && rankingGravedad.Count > 0 && rankingGravedad.Any(g => g.CantidadTurnos > 0))
                        {
                            col.Item().PaddingTop(12).Text("1. ANEXO CLÍNICO DE GUARDIA: DISTRIBUCIÓN POR GRAVEDAD Y SÍNTOMAS")
                                .FontSize(10).Bold().FontColor(ColorPrimario);

                            col.Item().PaddingTop(4).Row(rowTriage =>
                            {
                                rowTriage.RelativeItem(1f).Column(colG =>
                                {
                                    colG.Item().Text("Distribución por Gravedad de Triage").FontSize(8.5f).Bold().FontColor(ColorSecundario);
                                    colG.Item().PaddingTop(2).Table(t =>
                                    {
                                        t.ColumnsDefinition(c =>
                                        {
                                            c.RelativeColumn(2f);
                                            c.ConstantColumn(50);
                                            c.ConstantColumn(50);
                                        });

                                        t.Header(h =>
                                        {
                                            h.Cell().Background(ColorPrimario).Padding(3).Text("Gravedad").FontSize(8).Bold().FontColor(ColorBlanco);
                                            h.Cell().Background(ColorPrimario).Padding(3).AlignCenter().Text("Turnos").FontSize(8).Bold().FontColor(ColorBlanco);
                                            h.Cell().Background(ColorPrimario).Padding(3).AlignRight().Text("%").FontSize(8).Bold().FontColor(ColorBlanco);
                                        });

                                        foreach (var g in rankingGravedad)
                                        {
                                            t.Cell().Padding(3).Text(g.Gravedad).FontSize(8).SemiBold();
                                            t.Cell().Padding(3).AlignCenter().Text($"{g.CantidadTurnos}").FontSize(8);
                                            t.Cell().Padding(3).AlignRight().Text($"{g.Porcentaje:N1}%").FontSize(8).Bold();
                                        }
                                    });
                                });

                                rowTriage.ConstantItem(15);

                                if (rankingSintomas != null && rankingSintomas.Count > 0)
                                {
                                    rowTriage.RelativeItem(1.3f).Column(colS =>
                                    {
                                        colS.Item().Text("Prevalencia de Síntomas Atendidos").FontSize(8.5f).Bold().FontColor(ColorSecundario);
                                        colS.Item().PaddingTop(2).Table(t =>
                                        {
                                            t.ColumnsDefinition(c =>
                                            {
                                                c.RelativeColumn(2.5f);
                                                c.ConstantColumn(55);
                                                c.ConstantColumn(40);
                                                c.ConstantColumn(45);
                                            });

                                            t.Header(h =>
                                            {
                                                h.Cell().Background(ColorSecundario).Padding(3).Text("Síntoma").FontSize(8).Bold().FontColor(ColorBlanco);
                                                h.Cell().Background(ColorSecundario).Padding(3).Text("Severidad").FontSize(8).Bold().FontColor(ColorBlanco);
                                                h.Cell().Background(ColorSecundario).Padding(3).AlignCenter().Text("Casos").FontSize(8).Bold().FontColor(ColorBlanco);
                                                h.Cell().Background(ColorSecundario).Padding(3).AlignRight().Text("%").FontSize(8).Bold().FontColor(ColorBlanco);
                                            });

                                            foreach (var s in rankingSintomas)
                                            {
                                                t.Cell().Padding(3).Text(s.Sintoma).FontSize(7.5f);
                                                t.Cell().Padding(3).Text(s.Gravedad).FontSize(7.5f);
                                                t.Cell().Padding(3).AlignCenter().Text($"{s.CantidadCasos}").FontSize(7.5f);
                                                t.Cell().Padding(3).AlignRight().Text($"{s.Porcentaje:N1}%").FontSize(7.5f).Bold();
                                            }
                                        });
                                    });
                                }
                            });
                        }

                        // SECCIÓN 2: Historial de consultas y pacientes atendidos (con detalle de diagnósticos)
                        string tituloHistorial = (rankingGravedad != null && rankingGravedad.Any(g => g.CantidadTurnos > 0))
                            ? "2. DETALLE DE CONSULTAS Y PACIENTES ATENDIDOS"
                            : "1. HISTORIAL DE CONSULTAS Y PACIENTES ATENDIDOS";

                        col.Item().PaddingTop(14).Text(tituloHistorial)
                            .FontSize(10).Bold().FontColor(ColorPrimario);

                        col.Item().PaddingTop(4).Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(40);  // Turno
                                c.ConstantColumn(65);  // Fecha/Hora
                                c.RelativeColumn(2.5f);// Paciente
                                c.ConstantColumn(55);  // DNI
                                c.RelativeColumn(2f);  // Obra Social
                                c.RelativeColumn(3f);  // Diagnóstico / Evolución
                                c.RelativeColumn(2.5f);// Tratamiento
                            });

                            t.Header(h =>
                            {
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Turno").FontSize(8).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Fecha").FontSize(8).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Paciente").FontSize(8).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("DNI").FontSize(8).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Cobertura").FontSize(8).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Diagnóstico").FontSize(8).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Prescripción").FontSize(8).Bold().FontColor(ColorBlanco);
                            });

                            for (int i = 0; i < atenciones.Count; i++)
                            {
                                var a = atenciones[i];
                                var fondo = (i % 2 == 1) ? ColorFondoTabla : ColorBlanco;
                                string diag = !string.IsNullOrWhiteSpace(a.DiagRapido) ? a.DiagRapido : (a.DescripHistoriaClinica ?? "-");
                                string receta = !string.IsNullOrWhiteSpace(a.RecetaMedicamentos) ? a.RecetaMedicamentos : "-";

                                t.Cell().Background(fondo).Padding(3).Text(a.NroOrden ?? "-").FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(a.Fecha.ToString("dd/MM HH:mm")).FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(a.PacienteCompleto).FontSize(7).SemiBold();
                                t.Cell().Background(fondo).Padding(3).Text(a.DniPaciente).FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(a.ObraSocial).FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(diag).FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(receta).FontSize(7);
                            }
                        });
                    });

                    // Pie de Página
                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(ColorBorde);
                        col.Item().PaddingTop(3).Row(row =>
                        {
                            row.RelativeItem().Text("Reporte Clínico Oficial — Sistema de Gestión de Turnos Médicos")
                                .FontSize(7).Italic().FontColor(ColorGrisTexto);
                            row.RelativeItem().AlignRight().Text(x =>
                            {
                                x.Span("Página ");
                                x.CurrentPageNumber();
                                x.Span(" de ");
                                x.TotalPages();
                            });
                        });
                    });
                });
            });

            doc.GeneratePdf(rutaArchivo);
        }

        #endregion
    }
}
