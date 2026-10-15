using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                        decimal montoCoberturaOS = resumen.TurnosObraSocial > 0 ? Math.Round(resumen.MontoObraSocial * (70m / 30m), 2) : 0m;
                        decimal facturacionTotalBruta = resumen.MontoParticulares + (resumen.TurnosObraSocial > 0 ? (resumen.MontoObraSocial + montoCoberturaOS) : 0m);
                        decimal promedioPorTurno = resumen.TotalTurnos > 0 ? Math.Round(resumen.TotalRecaudado / resumen.TotalTurnos, 2) : 0m;

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
                                c.Item().Text($"$ {resumen.TotalRecaudado:N2}").FontSize(14).Bold().FontColor(ColorExito);
                                c.Item().Text("Abonado en ventanilla").FontSize(7);
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("COBERTURA OBRAS SOCIALES (70%)").FontSize(8).Bold().FontColor(ColorGrisTexto);
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

                        // Fila de Total Final
                        col.Item().PaddingTop(6).AlignRight().Text($"TOTAL RECAUDADO EN CAJA: $ {resumen.TotalRecaudado:N2}")
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

        #region 2. Reporte Gerencial de Recaudación y Ranking Médico (Gerente)

        /// <summary>
        /// Genera el reporte gerencial oficial de auditoría de recaudación de recepcionistas y ranking de médicos en PDF inmutable.
        /// </summary>
        public static void ExportarReporteGerencial(
            string rutaArchivo,
            ReporteRecaudacionAuditoriaResumenDTO resumen,
            List<ReporteRecaudacionAuditoriaDetalleDTO> detalles,
            List<ReporteRankingMedicoDTO> ranking,
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
                                c.Item().Text("INFORME GERENCIAL: RECAUDACIÓN Y RANKING MÉDICO")
                                    .FontSize(11).Bold().FontColor(ColorSecundario);
                                c.Item().Text($"Período Auditado: {fechaDesde:dd/MM/yyyy} al {fechaHasta:dd/MM/yyyy}")
                                    .FontSize(9).SemiBold();
                            });

                            row.ConstantItem(180).AlignRight().Column(c =>
                            {
                                c.Item().Text($"Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8);
                                c.Item().Text($"Gerencia: {usuarioActual?.Nombre} {usuarioActual?.Apellido}").FontSize(8);
                                c.Item().Text("AUDITORÍA DE EMISIÓN DE CAJA").FontSize(7).Bold().FontColor(ColorAlerta);
                                c.Item().Text("DOCUMENTO OFICIAL INMUTABLE").FontSize(7).Bold().FontColor(ColorPrimario);
                            });
                        });

                        col.Item().PaddingTop(5).LineHorizontal(1.5f).LineColor(ColorPrimario);
                    });

                    // Contenido
                    page.Content().PaddingTop(10).Column(col =>
                    {
                        // Tarjetas KPI Macro de Auditoría
                        col.Item().Border(1).BorderColor(ColorBorde).Padding(8).Background(ColorKpiFondo).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("TOTAL TICKETS EMITIDOS").FontSize(7).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"{resumen.TotalTurnos} tickets").FontSize(12).Bold().FontColor(ColorPrimario);
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("RECAUDACIÓN TOTAL AUDITADA").FontSize(7).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"$ {resumen.TotalRecaudado:N2}").FontSize(12).Bold().FontColor(ColorExito);
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("OBRAS SOCIALES VS PART.").FontSize(7).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"OS: ${resumen.MontoObraSocial:N0} | Part: ${resumen.MontoParticulares:N0}").FontSize(9).Bold().FontColor(ColorAcento);
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("EMERGENCIA VS ESPECIALIDAD").FontSize(7).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"Emg: ${resumen.MontoEmergencia:N0} | Esp: ${resumen.MontoEspecialidad:N0}").FontSize(9).Bold().FontColor(ColorSecundario);
                            });
                        });

                        // SECCIÓN 1: Auditoría de Recaudación y Emisión de Recepcionistas
                        col.Item().PaddingTop(12).Text("1. AUDITORÍA DE RECAUDACIÓN Y EMISIÓN (RECEPCIÓN Y CAJA)")
                            .FontSize(10).Bold().FontColor(ColorPrimario);

                        col.Item().PaddingTop(4).Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(45);   // Nro Orden
                                c.ConstantColumn(85);   // Emisión
                                c.RelativeColumn(2.5f); // Paciente
                                c.ConstantColumn(55);   // DNI
                                c.ConstantColumn(65);   // Tipo
                                c.RelativeColumn(2.2f); // Especialidad
                                c.RelativeColumn(2.5f); // Obra Social
                                c.ConstantColumn(65);   // Monto
                                c.ConstantColumn(55);   // Estado
                            });

                            t.Header(h =>
                            {
                                h.Cell().Background(ColorPrimario).Padding(3).AlignCenter().Text("N° Ord").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorPrimario).Padding(3).Text("Emisión").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorPrimario).Padding(3).Text("Paciente").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorPrimario).Padding(3).Text("DNI").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorPrimario).Padding(3).Text("Modalidad").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorPrimario).Padding(3).Text("Especialidad").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorPrimario).Padding(3).Text("Obra Social").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorPrimario).Padding(3).AlignRight().Text("Monto").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorPrimario).Padding(3).Text("Estado").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                            });

                            int maxFilasDetalle = Math.Min(detalles.Count, 40);
                            for (int i = 0; i < maxFilasDetalle; i++)
                            {
                                var det = detalles[i];
                                var fondo = (i % 2 == 1) ? ColorFondoTabla : ColorBlanco;

                                t.Cell().Background(fondo).Padding(2.5f).AlignCenter().Text(det.NroOrden).FontSize(7).SemiBold();
                                t.Cell().Background(fondo).Padding(2.5f).Text($"{det.FechaEmision:dd/MM/yyyy HH:mm}").FontSize(7);
                                t.Cell().Background(fondo).Padding(2.5f).Text(det.Paciente).FontSize(7).SemiBold();
                                t.Cell().Background(fondo).Padding(2.5f).Text(det.DniPaciente).FontSize(7);
                                t.Cell().Background(fondo).Padding(2.5f).Text(det.TipoTurno).FontSize(7);
                                t.Cell().Background(fondo).Padding(2.5f).Text(det.Especialidad).FontSize(7);
                                t.Cell().Background(fondo).Padding(2.5f).Text(det.ObraSocial).FontSize(7);
                                t.Cell().Background(fondo).Padding(2.5f).AlignRight().Text($"$ {det.MontoCobrado:N2}").FontSize(7).Bold();
                                t.Cell().Background(fondo).Padding(2.5f).Text(det.EstadoTurno).FontSize(7);
                            }
                        });

                        if (detalles.Count > 40)
                        {
                            col.Item().PaddingTop(2).Text($"* Mostrando las primeras 40 emisiones de {detalles.Count} registradas en el período auditado.")
                                .FontSize(7).Italic().FontColor(ColorGrisTexto);
                        }

                        // SECCIÓN 2: Ranking de Médicos por Atenciones Realizadas
                        col.Item().PaddingTop(14).Text("2. RANKING DE MÉDICOS POR ATENCIONES EFECTIVAMENTE REALIZADAS")
                            .FontSize(10).Bold().FontColor(ColorPrimario);

                        col.Item().PaddingTop(4).Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(25);   // #
                                c.RelativeColumn(3.5f); // Profesional Médico
                                c.RelativeColumn(3.5f); // Especialidad
                                c.ConstantColumn(65);   // Matrícula
                                c.ConstantColumn(65);   // Atenciones
                                c.ConstantColumn(85);   // Facturación
                                c.ConstantColumn(55);   // % Part.
                            });

                            t.Header(h =>
                            {
                                h.Cell().Background(ColorSecundario).Padding(3).AlignCenter().Text("#").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Profesional Médico").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Especialidades Asignadas").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Matrícula").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).AlignCenter().Text("Atenciones").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).AlignRight().Text("Facturación").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).AlignRight().Text("% Part.").FontSize(7.5f).Bold().FontColor(ColorBlanco);
                            });

                            for (int i = 0; i < ranking.Count; i++)
                            {
                                var med = ranking[i];
                                var fondo = (i % 2 == 1) ? ColorFondoTabla : ColorBlanco;

                                t.Cell().Background(fondo).Padding(3).AlignCenter().Text($"{med.Posicion}").FontSize(7.5f).Bold();
                                t.Cell().Background(fondo).Padding(3).Text(med.NombreMedico).FontSize(7.5f).SemiBold();
                                t.Cell().Background(fondo).Padding(3).Text(med.Especialidad).FontSize(7.5f);
                                t.Cell().Background(fondo).Padding(3).Text(med.Matricula).FontSize(7.5f);
                                t.Cell().Background(fondo).Padding(3).AlignCenter().Text($"{med.AtencionesRealizadas}").FontSize(7.5f).Bold();
                                t.Cell().Background(fondo).Padding(3).AlignRight().Text($"$ {med.TotalRecaudado:N2}").FontSize(7.5f);
                                t.Cell().Background(fondo).Padding(3).AlignRight().Text($"{med.PorcentajeParticipacion:N1}%").FontSize(7.5f).Bold();
                            }
                        });
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
        /// Sobrecarga de compatibilidad hacia atrás para pruebas unitarias y de integración existentes.
        /// </summary>
        public static void ExportarReporteGerencial(
            string rutaArchivo,
            List<ReporteIngresoMedicoDTO> ingresos,
            List<ReporteObraSocialVsParticularDTO> coberturas,
            List<ReporteDemandaMedicoRankingDTO> demanda,
            DateTime fechaDesde,
            DateTime fechaHasta,
            UsuarioLoginResult? usuarioActual)
        {
            var resumen = new ReporteRecaudacionAuditoriaResumenDTO
            {
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                TotalTurnos = coberturas.Sum(c => c.CantidadTurnos),
                TotalRecaudado = ingresos.Sum(i => i.IngresosTotales),
                MontoParticulares = coberturas.Where(c => c.TipoCobertura.Equals("Particular", StringComparison.OrdinalIgnoreCase)).Sum(c => c.TotalRecaudado),
                MontoObraSocial = coberturas.Where(c => !c.TipoCobertura.Equals("Particular", StringComparison.OrdinalIgnoreCase)).Sum(c => c.TotalRecaudado)
            };

            var ranking = demanda.Select((d, idx) => new ReporteRankingMedicoDTO
            {
                Posicion = idx + 1,
                NombreMedico = d.NombreMedico,
                Especialidad = d.Especialidad,
                Matricula = d.Matricula,
                AtencionesRealizadas = d.TurnosAtendidos,
                TotalRecaudado = 0,
                PorcentajeParticipacion = d.TasaResolucion
            }).ToList();

            ExportarReporteGerencial(rutaArchivo, resumen, new List<ReporteRecaudacionAuditoriaDetalleDTO>(), ranking, fechaDesde, fechaHasta, usuarioActual);
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
                            int consultasCount = atenciones.Count(a => a.TipoTurno.Equals("Consulta", StringComparison.OrdinalIgnoreCase));

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

        #region 4. Reporte Operativo de Guardia: Triage y Distribución de Urgencias

        /// <summary>
        /// Genera el reporte oficial de guardia y triage en formato PDF inmutable.
        /// </summary>
        public static void ExportarReporteGuardia(
            string rutaArchivo,
            ReporteGuardiaResumenDTO resumen,
            List<ReporteGuardiaSintomaDTO> rankingSintomas,
            List<ReporteGuardiaDetalleDTO> detalles,
            DateTime fechaDesde,
            DateTime fechaHasta,
            string filtroPrioridad,
            UsuarioLoginResult? usuarioActual)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.2f, Unit.Centimetre);
                    page.PageColor(ColorBlanco);
                    page.DefaultTextStyle(x => x.FontSize(8).FontColor(ColorSecundario).FontFamily(Fonts.Lato));

                    // Encabezado
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("CENTRO MÉDICO DE ESPECIALIDADES")
                                    .FontSize(14).Bold().FontColor(ColorPrimario);
                                c.Item().Text("REPORTE OPERATIVO DE GUARDIA: TRIAGE Y DISTRIBUCIÓN DE URGENCIAS")
                                    .FontSize(11).Bold().FontColor(ColorSecundario);
                                c.Item().Text($"Período: {fechaDesde:dd/MM/yyyy} al {fechaHasta:dd/MM/yyyy}  |  Filtro Prioridad: {filtroPrioridad}")
                                    .FontSize(9).SemiBold();
                            });

                            row.ConstantItem(220).AlignRight().Column(c =>
                            {
                                c.Item().Text($"Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8);
                                c.Item().Text($"Auditor: {usuarioActual?.Nombre} {usuarioActual?.Apellido} ({usuarioActual?.NombreRol})").FontSize(8);
                                c.Item().Text("MONITOREO DE TRIAGE AUDITADO").FontSize(7).Bold().FontColor(ColorAlerta);
                                c.Item().Text("DOCUMENTO OFICIAL INMUTABLE").FontSize(7).Bold().FontColor(ColorPrimario);
                            });
                        });

                        col.Item().PaddingTop(4).LineHorizontal(1.5f).LineColor(ColorPrimario);
                    });

                    // Contenido
                    page.Content().PaddingTop(8).Column(col =>
                    {
                        // Tarjetas de KPIs
                        col.Item().Border(1).BorderColor(ColorBorde).Padding(6).Background(ColorKpiFondo).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("TOTAL INGRESOS").FontSize(7).Bold().FontColor(ColorGrisTexto);
                                c.Item().Text($"{resumen.TotalEmergencias}").FontSize(13).Bold().FontColor(ColorSecundario);
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("TRIAGE ALTA (ROJO)").FontSize(7).Bold().FontColor(ColorAlerta);
                                c.Item().Text($"{resumen.TotalAlta}").FontSize(13).Bold().FontColor(ColorAlerta);
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("TRIAGE MEDIA (AMARILLO)").FontSize(7).Bold().FontColor(Color.FromHex("#B45309"));
                                c.Item().Text($"{resumen.TotalMedia}").FontSize(13).Bold().FontColor(Color.FromHex("#B45309"));
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("TRIAGE BAJA (VERDE)").FontSize(7).Bold().FontColor(ColorExito);
                                c.Item().Text($"{resumen.TotalBaja}").FontSize(13).Bold().FontColor(ColorExito);
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("TASA RESOLUCIÓN").FontSize(7).Bold().FontColor(ColorAcento);
                                c.Item().Text($"{resumen.TasaResolucion:F1}%").FontSize(13).Bold().FontColor(ColorAcento);
                            });
                        });

                        // Ranking de Síntomas
                        if (rankingSintomas.Count > 0)
                        {
                            col.Item().PaddingTop(8).Text("RANKING DE SÍNTOMAS PREDOMINANTES").FontSize(9).Bold().FontColor(ColorPrimario);
                            col.Item().PaddingTop(3).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3); // Síntoma
                                    cd.RelativeColumn(1.5f); // Gravedad
                                    cd.RelativeColumn(1.5f); // Casos
                                    cd.RelativeColumn(1.5f); // %
                                });

                                t.Header(h =>
                                {
                                    h.Cell().Background(ColorPrimario).Padding(3).Text("Síntoma Manifestado").FontSize(7).Bold().FontColor(ColorBlanco);
                                    h.Cell().Background(ColorPrimario).Padding(3).Text("Gravedad Triage").FontSize(7).Bold().FontColor(ColorBlanco);
                                    h.Cell().Background(ColorPrimario).Padding(3).Text("Casos").FontSize(7).Bold().FontColor(ColorBlanco);
                                    h.Cell().Background(ColorPrimario).Padding(3).Text("% Participación").FontSize(7).Bold().FontColor(ColorBlanco);
                                });

                                for (int i = 0; i < Math.Min(rankingSintomas.Count, 5); i++)
                                {
                                    var s = rankingSintomas[i];
                                    var fondo = i % 2 == 0 ? ColorFondoTabla : ColorBlanco;

                                    t.Cell().Background(fondo).Padding(3).Text(s.Sintoma).FontSize(7);
                                    t.Cell().Background(fondo).Padding(3).Text(s.Gravedad).FontSize(7).SemiBold();
                                    t.Cell().Background(fondo).Padding(3).Text(s.CantidadCasos.ToString()).FontSize(7);
                                    t.Cell().Background(fondo).Padding(3).Text($"{s.Porcentaje:F1}%").FontSize(7);
                                }
                            });
                        }

                        // Detalle de Ingresos a Guardia
                        col.Item().PaddingTop(8).Text($"DETALLE DE INGRESOS A GUARDIA ({detalles.Count} Registros)").FontSize(9).Bold().FontColor(ColorPrimario);
                        col.Item().PaddingTop(3).Table(t =>
                        {
                            t.ColumnsDefinition(cd =>
                            {
                                cd.ConstantColumn(40);  // Nro
                                cd.ConstantColumn(65);  // Fecha
                                cd.RelativeColumn(2);   // Paciente
                                cd.ConstantColumn(55);  // DNI
                                cd.RelativeColumn(1.5f);// Cobertura
                                cd.ConstantColumn(50);  // Triage
                                cd.RelativeColumn(2.5f);// Síntomas
                                cd.ConstantColumn(60);  // Estado
                                cd.ConstantColumn(55);  // Box
                            });

                            t.Header(h =>
                            {
                                h.Cell().Background(ColorSecundario).Padding(3).Text("N°").FontSize(7).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Fecha").FontSize(7).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Paciente").FontSize(7).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("DNI").FontSize(7).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Cobertura").FontSize(7).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Triage").FontSize(7).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Síntomas").FontSize(7).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Estado").FontSize(7).Bold().FontColor(ColorBlanco);
                                h.Cell().Background(ColorSecundario).Padding(3).Text("Consultorio").FontSize(7).Bold().FontColor(ColorBlanco);
                            });

                            for (int i = 0; i < detalles.Count; i++)
                            {
                                var d = detalles[i];
                                var fondo = i % 2 == 0 ? ColorFondoTabla : ColorBlanco;

                                t.Cell().Background(fondo).Padding(3).Text(d.NroOrden ?? "-").FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(d.Fecha.ToString("dd/MM HH:mm")).FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(d.PacienteCompleto).FontSize(7).SemiBold();
                                t.Cell().Background(fondo).Padding(3).Text(d.DniPaciente ?? "-").FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(d.ObraSocial ?? "-").FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(d.Prioridad ?? "-").FontSize(7).Bold();
                                t.Cell().Background(fondo).Padding(3).Text(d.Sintomas ?? "-").FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(d.Estado ?? "-").FontSize(7);
                                t.Cell().Background(fondo).Padding(3).Text(d.NombreSala ?? "-").FontSize(7);
                            }
                        });
                    });

                    // Pie de Página
                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(ColorBorde);
                        col.Item().PaddingTop(3).Row(row =>
                        {
                            row.RelativeItem().Text("Reporte Oficial de Guardia Médica — Formato Inmutable PDF")
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
