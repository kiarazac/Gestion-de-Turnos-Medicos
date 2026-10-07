using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.Servicios
{
    /// <summary>
    /// Servicio centralizado de reportería y generación de planillas Microsoft Excel (.xlsx) nativas.
    /// Emplea OpenXML (ClosedXML) para confeccionar libros con diseño corporativo médico,
    /// colores semánticos, auto-ajuste de columnas, bordes y formatos numéricos tipados.
    /// </summary>
    public static class ExportadorExcel
    {
        // Paleta de colores corporativos médicos
        private static readonly XLColor ColorTealPrincipal = XLColor.FromArgb(15, 118, 110);    // #0F766E
        private static readonly XLColor ColorSlateOscuro = XLColor.FromArgb(30, 41, 59);        // #1E293B
        private static readonly XLColor ColorSlateBorde = XLColor.FromArgb(203, 213, 225);      // #CBD5E1
        private static readonly XLColor ColorZebraClaro = XLColor.FromArgb(248, 250, 252);      // #F8FAFC
        private static readonly XLColor ColorBlanco = XLColor.White;

        // Colores semánticos para Triage y Alertas
        private static readonly XLColor ColorTriageAltaFondo = XLColor.FromArgb(254, 226, 226); // #FEE2E2
        private static readonly XLColor ColorTriageAltaTexto = XLColor.FromArgb(153, 27, 27);   // #991B1B
        private static readonly XLColor ColorTriageMediaFondo = XLColor.FromArgb(254, 243, 199);// #FEF3C7
        private static readonly XLColor ColorTriageMediaTexto = XLColor.FromArgb(146, 64, 14);  // #92400E
        private static readonly XLColor ColorTriageBajaFondo = XLColor.FromArgb(220, 252, 231); // #DCFCE7
        private static readonly XLColor ColorTriageBajaTexto = XLColor.FromArgb(22, 101, 52);   // #166534

        #region 1. Reporte Operativo de Guardia (FrmReporteGuardiaAdmin)

        /// <summary>
        /// Genera el libro Excel para el Reporte Operativo de Guardia: Triage y Distribución de Urgencias.
        /// Contiene 2 hojas: "Resumen y Triage" (con KPIs y Ranking de síntomas) y "Detalle de Ingresos".
        /// </summary>
        public static void ExportarReporteGuardia(
            string rutaArchivo,
            ReporteGuardiaResumenDTO resumen,
            List<ReporteGuardiaSintomaDTO> rankingSintomas,
            List<ReporteGuardiaDetalleDTO> detalles,
            DateTime fechaDesde,
            DateTime fechaHasta,
            string prioridadFiltrada,
            UsuarioLoginResult? usuarioActual)
        {
            using (var wb = new XLWorkbook())
            {
                // ==========================================
                // HOJA 1: RESUMEN Y TRIAGE
                // ==========================================
                var wsResumen = wb.Worksheets.Add("Resumen y Triage");
                wsResumen.ShowGridLines = true;

                // 1. Banner Principal
                wsResumen.Range("A1:D1").Merge();
                var banner = wsResumen.Cell("A1");
                banner.Value = "CENTRO MÉDICO — REPORTE OPERATIVO DE GUARDIA";
                EstilarBanner(banner, ColorTealPrincipal, 13);
                wsResumen.Row(1).Height = 28;

                // 2. Ficha de Metadatos
                int filaMeta = 3;
                EscribirMetadato(wsResumen, filaMeta++, "Fecha de Emisión:", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " hs");
                EscribirMetadato(wsResumen, filaMeta++, "Período Consultado:", $"{fechaDesde:dd/MM/yyyy} al {fechaHasta:dd/MM/yyyy}");
                EscribirMetadato(wsResumen, filaMeta++, "Prioridad Filtrada:", prioridadFiltrada);
                string emisor = usuarioActual != null ? $"{usuarioActual.Nombre} {usuarioActual.Apellido} ({usuarioActual.NombreRol})" : "Administrador del Sistema";
                EscribirMetadato(wsResumen, filaMeta++, "Generado por:", emisor);

                // 3. Sección KPIs
                int filaKpi = filaMeta + 1;
                wsResumen.Range(filaKpi, 1, filaKpi, 2).Merge();
                var lblKpiTitulo = wsResumen.Cell(filaKpi, 1);
                lblKpiTitulo.Value = "INDICADORES CLAVE DE FLUJO (KPIS)";
                lblKpiTitulo.Style.Font.Bold = true;
                lblKpiTitulo.Style.Font.FontSize = 10.5;
                lblKpiTitulo.Style.Font.FontColor = ColorSlateOscuro;

                filaKpi++;
                wsResumen.Cell(filaKpi, 1).Value = "Métrica Operativa";
                wsResumen.Cell(filaKpi, 2).Value = "Valor";
                EstilarEncabezadoTabla(wsResumen.Range(filaKpi, 1, filaKpi, 2), ColorSlateOscuro);
                wsResumen.Row(filaKpi).Height = 22;

                var metricas = new List<(string Titulo, object Valor, string Formato, XLColor? Fondo, XLColor? Texto)>
                {
                    ("Total Ingresos a Guardia", resumen.TotalEmergencias, "#,##0", null, null),
                    ("Triage Alta (Rojo)", resumen.TotalAlta, "#,##0", ColorTriageAltaFondo, ColorTriageAltaTexto),
                    ("Triage Media (Amarillo)", resumen.TotalMedia, "#,##0", ColorTriageMediaFondo, ColorTriageMediaTexto),
                    ("Triage Baja (Verde)", resumen.TotalBaja, "#,##0", ColorTriageBajaFondo, ColorTriageBajaTexto),
                    ("Turnos Atendidos / En Consulta", resumen.TotalAtendidos, "#,##0", null, null),
                    ("Turnos En Espera en Sala", resumen.TotalEnEspera, "#,##0", null, null),
                    ("Turnos Cancelados / Desertados", resumen.TotalCancelados, "#,##0", null, null),
                    ("Tasa de Resolución de Guardia", (double)(resumen.TasaResolucion / 100m), "0.00%", ColorTriageBajaFondo, ColorTriageBajaTexto)
                };

                int filaInicioKpis = filaKpi + 1;
                foreach (var m in metricas)
                {
                    filaKpi++;
                    var cNombre = wsResumen.Cell(filaKpi, 1);
                    var cValor = wsResumen.Cell(filaKpi, 2);

                    cNombre.Value = m.Titulo;
                    cValor.Value = XLCellValue.FromObject(m.Valor);
                    cValor.Style.NumberFormat.Format = m.Formato;
                    cValor.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    if (m.Fondo != null && m.Texto != null)
                    {
                        cValor.Style.Fill.BackgroundColor = m.Fondo;
                        cValor.Style.Font.FontColor = m.Texto;
                        cValor.Style.Font.Bold = true;
                    }
                    else if ((filaKpi % 2) == 0)
                    {
                        wsResumen.Range(filaKpi, 1, filaKpi, 2).Style.Fill.BackgroundColor = ColorZebraClaro;
                    }
                }
                AplicarBordesTabla(wsResumen.Range(filaInicioKpis - 1, 1, filaKpi, 2));

                // 4. Sección Ranking de Síntomas
                int filaRank = filaKpi + 2;
                wsResumen.Range(filaRank, 1, filaRank, 4).Merge();
                var lblRankTitulo = wsResumen.Cell(filaRank, 1);
                lblRankTitulo.Value = "RANKING DE SÍNTOMAS Y MOTIVOS DE CONSULTA";
                lblRankTitulo.Style.Font.Bold = true;
                lblRankTitulo.Style.Font.FontSize = 10.5;
                lblRankTitulo.Style.Font.FontColor = ColorSlateOscuro;

                filaRank++;
                wsResumen.Cell(filaRank, 1).Value = "Síntoma / Motivo Manifestado";
                wsResumen.Cell(filaRank, 2).Value = "Severidad";
                wsResumen.Cell(filaRank, 3).Value = "Casos";
                wsResumen.Cell(filaRank, 4).Value = "% del Total";
                EstilarEncabezadoTabla(wsResumen.Range(filaRank, 1, filaRank, 4), ColorTealPrincipal);
                wsResumen.Row(filaRank).Height = 22;

                int filaInicioRank = filaRank + 1;
                foreach (var s in rankingSintomas)
                {
                    filaRank++;
                    wsResumen.Cell(filaRank, 1).Value = s.Sintoma;
                    
                    var cSev = wsResumen.Cell(filaRank, 2);
                    cSev.Value = s.Gravedad;
                    cSev.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    AplicarColorSeveridad(cSev, s.Gravedad);

                    var cCasos = wsResumen.Cell(filaRank, 3);
                    cCasos.Value = s.CantidadCasos;
                    cCasos.Style.NumberFormat.Format = "#,##0";
                    cCasos.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    var cPct = wsResumen.Cell(filaRank, 4);
                    cPct.Value = (double)(s.Porcentaje / 100m);
                    cPct.Style.NumberFormat.Format = "0.00%";
                    cPct.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    if ((filaRank % 2) == 0 && cSev.Style.Fill.BackgroundColor == XLColor.NoColor)
                    {
                        wsResumen.Range(filaRank, 1, filaRank, 4).Style.Fill.BackgroundColor = ColorZebraClaro;
                    }
                }
                if (rankingSintomas.Count > 0)
                {
                    AplicarBordesTabla(wsResumen.Range(filaInicioRank - 1, 1, filaRank, 4));
                }

                AjustarAnchoColumnas(wsResumen, new (int, double)[]
                {
                    (1, 35.0), // Métrica / Síntoma
                    (2, 18.0), // Valor / Severidad
                    (3, 15.0), // Casos
                    (4, 16.0)  // % del Total
                });

                // ==========================================
                // HOJA 2: DETALLE DE INGRESOS
                // ==========================================
                var wsDetalle = wb.Worksheets.Add("Detalle de Ingresos");
                wsDetalle.ShowGridLines = true;

                // Banner
                wsDetalle.Range("A1:I1").Merge();
                var bannerDetalle = wsDetalle.Cell("A1");
                bannerDetalle.Value = "REGISTRO NOMINAL DE INGRESOS A GUARDIA Y TRIAGE";
                EstilarBanner(bannerDetalle, ColorSlateOscuro, 13);
                wsDetalle.Row(1).Height = 28;

                // Encabezados
                int filaD = 3;
                string[] cabecerasDetalle = {
                    "Fecha y Hora", "N° Turno", "Paciente", "DNI", "Cobertura Médica",
                    "Triage / Prioridad", "Síntomas Manifestados", "Estado", "Consultorio / Sala"
                };

                for (int i = 0; i < cabecerasDetalle.Length; i++)
                {
                    wsDetalle.Cell(filaD, i + 1).Value = cabecerasDetalle[i];
                }
                EstilarEncabezadoTabla(wsDetalle.Range(filaD, 1, filaD, cabecerasDetalle.Length), ColorTealPrincipal);
                wsDetalle.Row(filaD).Height = 24;

                int filaInicioD = filaD + 1;
                foreach (var d in detalles)
                {
                    filaD++;
                    var cFecha = wsDetalle.Cell(filaD, 1);
                    cFecha.Value = d.Fecha;
                    cFecha.Style.NumberFormat.Format = "dd/MM/yyyy HH:mm";
                    cFecha.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var cTurno = wsDetalle.Cell(filaD, 2);
                    cTurno.Value = d.NroOrden;
                    cTurno.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cTurno.Style.Font.Bold = true;

                    wsDetalle.Cell(filaD, 3).Value = d.PacienteCompleto;

                    var cDni = wsDetalle.Cell(filaD, 4);
                    cDni.Value = d.DniPaciente;
                    cDni.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    wsDetalle.Cell(filaD, 5).Value = d.ObraSocial;

                    var cPrio = wsDetalle.Cell(filaD, 6);
                    cPrio.Value = d.Prioridad;
                    cPrio.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    AplicarColorSeveridad(cPrio, d.Prioridad);

                    var cSintomas = wsDetalle.Cell(filaD, 7);
                    cSintomas.Value = d.Sintomas;
                    cSintomas.Style.Alignment.WrapText = true;

                    var cEstado = wsDetalle.Cell(filaD, 8);
                    cEstado.Value = d.Estado;
                    cEstado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var cSala = wsDetalle.Cell(filaD, 9);
                    cSala.Value = d.NombreSala;
                    cSala.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    if ((filaD % 2) == 0 && cPrio.Style.Fill.BackgroundColor == XLColor.NoColor)
                    {
                        wsDetalle.Range(filaD, 1, filaD, 9).Style.Fill.BackgroundColor = ColorZebraClaro;
                    }
                }

                if (detalles.Count > 0)
                {
                    AplicarBordesTabla(wsDetalle.Range(filaInicioD - 1, 1, filaD, 9));
                }

                AjustarAnchoColumnas(wsDetalle, new (int, double)[]
                {
                    (1, 18.0), // Fecha y Hora
                    (2, 13.0), // N° Turno
                    (3, 26.0), // Paciente
                    (4, 14.0), // DNI
                    (5, 20.0), // Cobertura Médica
                    (6, 18.0), // Triage / Prioridad
                    (7, 40.0), // Síntomas Manifestados
                    (8, 16.0), // Estado
                    (9, 20.0)  // Consultorio / Sala
                });
                wsDetalle.Column(7).Width = 40.0; // Columna de síntomas con ancho holgado

                wb.SaveAs(rutaArchivo);
            }
        }

        #endregion



        #region 3. Reporte de Atenciones del Médico (FrmMisAtenciones)

        /// <summary>
        /// Genera el libro Excel profesional para el historial y reporte de consultas del profesional médico.
        /// </summary>
        public static void ExportarAtencionesMedico(
            string rutaArchivo,
            List<AtencionMedicoDTO> listaAtenciones,
            DateTime fechaDesde,
            DateTime fechaHasta,
            UsuarioLoginResult? usuarioActual)
        {
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Atenciones Realizadas");
                ws.ShowGridLines = true;

                // 1. Banner
                ws.Range("A1:K1").Merge();
                var banner = ws.Cell("A1");
                string nombreDr = usuarioActual != null ? $"Dr. {usuarioActual.Nombre} {usuarioActual.Apellido}" : "Personal Médico";
                banner.Value = $"CENTRO MÉDICO — CONSULTAS CLÍNICAS Y ATENCIONES REALIZADAS ({nombreDr})";
                EstilarBanner(banner, ColorTealPrincipal, 12.5);
                ws.Row(1).Height = 28;

                // 2. Metadatos
                int filaMeta = 3;
                EscribirMetadato(ws, filaMeta++, "Profesional Médico:", nombreDr);
                EscribirMetadato(ws, filaMeta++, "Fecha de Emisión:", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " hs");
                EscribirMetadato(ws, filaMeta++, "Período Consultado:", $"{fechaDesde:dd/MM/yyyy} al {fechaHasta:dd/MM/yyyy}");
                EscribirMetadato(ws, filaMeta++, "Total Atenciones:", listaAtenciones.Count.ToString());

                // 3. Encabezados de Tabla
                int filaT = filaMeta + 1;
                string[] cabeceras = {
                    "N°", "Fecha / Hora", "N° Turno", "Paciente", "DNI",
                    "Cobertura", "Modalidad", "Especialidad", "Sala / Consultorio",
                    "Diagnóstico Clínico", "Receta / Indicaciones"
                };

                for (int i = 0; i < cabeceras.Length; i++)
                {
                    ws.Cell(filaT, i + 1).Value = cabeceras[i];
                }
                EstilarEncabezadoTabla(ws.Range(filaT, 1, filaT, cabeceras.Length), ColorSlateOscuro);
                ws.Row(filaT).Height = 24;

                // 4. Filas de Datos
                int filaInicio = filaT + 1;
                int contador = 1;

                foreach (var a in listaAtenciones)
                {
                    filaT++;

                    var cNum = ws.Cell(filaT, 1);
                    cNum.Value = contador++;
                    cNum.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var cFecha = ws.Cell(filaT, 2);
                    cFecha.Value = a.Fecha;
                    cFecha.Style.NumberFormat.Format = "dd/MM/yyyy HH:mm";
                    cFecha.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var cTurno = ws.Cell(filaT, 3);
                    cTurno.Value = a.NroOrden;
                    cTurno.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cTurno.Style.Font.Bold = true;

                    ws.Cell(filaT, 4).Value = a.PacienteCompleto;

                    var cDni = ws.Cell(filaT, 5);
                    cDni.Value = a.DniPaciente;
                    cDni.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(filaT, 6).Value = a.ObraSocial;

                    var cMod = ws.Cell(filaT, 7);
                    cMod.Value = a.TipoTurno;
                    cMod.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(filaT, 8).Value = a.Especialidad;

                    var cSala = ws.Cell(filaT, 9);
                    cSala.Value = a.NombreSala;
                    cSala.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var cDiag = ws.Cell(filaT, 10);
                    cDiag.Value = a.DiagRapido;
                    cDiag.Style.Alignment.WrapText = true;

                    var cReceta = ws.Cell(filaT, 11);
                    cReceta.Value = string.IsNullOrWhiteSpace(a.RecetaMedicamentos) ? "(Sin prescripción)" : a.RecetaMedicamentos;
                    cReceta.Style.Alignment.WrapText = true;

                    if ((filaT % 2) == 0)
                    {
                        ws.Range(filaT, 1, filaT, 11).Style.Fill.BackgroundColor = ColorZebraClaro;
                    }
                }

                if (listaAtenciones.Count > 0)
                {
                    AplicarBordesTabla(ws.Range(filaInicio - 1, 1, filaT, 11));
                }

                AjustarAnchoColumnas(ws, new (int, double)[]
                {
                    (1, 6.0),   // N°
                    (2, 18.0),  // Fecha / Hora
                    (3, 13.0),  // N° Turno
                    (4, 26.0),  // Paciente
                    (5, 14.0),  // DNI
                    (6, 20.0),  // Cobertura
                    (7, 15.0),  // Modalidad
                    (8, 22.0),  // Especialidad
                    (9, 20.0),  // Sala / Consultorio
                    (10, 38.0), // Diagnóstico Clínico
                    (11, 38.0)  // Receta / Indicaciones
                });
                ws.Column(10).Width = 38.0; // Diagnóstico con ancho amplio garantizado
                ws.Column(11).Width = 38.0; // Receta con ancho amplio garantizado

                wb.SaveAs(rutaArchivo);
            }
        }

        #endregion

        #region Métodos Auxiliares de Estilizado

        private static void EstilarBanner(IXLCell celda, XLColor colorFondo, double tamañoFuente)
        {
            celda.Style.Fill.BackgroundColor = colorFondo;
            celda.Style.Font.FontColor = ColorBlanco;
            celda.Style.Font.Bold = true;
            celda.Style.Font.FontSize = tamañoFuente;
            celda.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            celda.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private static void EscribirMetadato(IXLWorksheet ws, int fila, string etiqueta, string valor)
        {
            var cEti = ws.Cell(fila, 1);
            cEti.Value = etiqueta;
            cEti.Style.Font.Bold = true;
            cEti.Style.Font.FontColor = ColorSlateOscuro;

            var cVal = ws.Cell(fila, 2);
            cVal.Value = valor;
            cVal.Style.Font.FontColor = ColorSlateOscuro;
        }

        private static void EstilarEncabezadoTabla(IXLRange rango, XLColor colorFondo)
        {
            rango.Style.Fill.BackgroundColor = colorFondo;
            rango.Style.Font.FontColor = ColorBlanco;
            rango.Style.Font.Bold = true;
            rango.Style.Font.FontSize = 9.5;
            rango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rango.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private static void AplicarBordesTabla(IXLRange rango)
        {
            rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rango.Style.Border.OutsideBorderColor = ColorSlateBorde;
            rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rango.Style.Border.InsideBorderColor = ColorSlateBorde;
        }

        private static void AplicarColorSeveridad(IXLCell celda, string severidad)
        {
            if (string.IsNullOrWhiteSpace(severidad)) return;

            string s = severidad.Trim().ToLowerInvariant();
            if (s.Contains("alta") || s.Contains("rojo") || s.Contains("grave"))
            {
                celda.Style.Fill.BackgroundColor = ColorTriageAltaFondo;
                celda.Style.Font.FontColor = ColorTriageAltaTexto;
                celda.Style.Font.Bold = true;
            }
            else if (s.Contains("media") || s.Contains("amarill") || s.Contains("moderad"))
            {
                celda.Style.Fill.BackgroundColor = ColorTriageMediaFondo;
                celda.Style.Font.FontColor = ColorTriageMediaTexto;
                celda.Style.Font.Bold = true;
            }
            else if (s.Contains("baja") || s.Contains("verde") || s.Contains("leve"))
            {
                celda.Style.Fill.BackgroundColor = ColorTriageBajaFondo;
                celda.Style.Font.FontColor = ColorTriageBajaTexto;
                celda.Style.Font.Bold = true;
            }
        }

        /// <summary>
        /// Ajusta el ancho de las columnas calculando el tamaño del contenido (desde la fila 2 para no romper
        /// el cálculo con el banner combinado de la fila 1) y aplicando márgenes y mínimos garantizados para
        /// evitar que encabezados o datos se recorten o muestren '###'.
        /// </summary>
        private static void AjustarAnchoColumnas(IXLWorksheet ws, (int Columna, double AnchoMinimo)[] anchosConfigurados)
        {
            int ultimaFila = Math.Max(ws.LastRowUsed()?.RowNumber() ?? 50, 20);

            // Ajustamos desde la fila 2 hasta la última fila utilizada para ignorar el banner combinado de A1
            // e incluir metadatos, encabezados y registros.
            ws.Columns().AdjustToContents(2, ultimaFila);

            foreach (var (col, minAncho) in anchosConfigurados)
            {
                var columna = ws.Column(col);
                // Le sumamos un margen de respiración visual (+3.0) y aseguramos el ancho mínimo configurado
                columna.Width = Math.Max(columna.Width + 3.0, minAncho);
            }
        }

        #endregion
    }
}
