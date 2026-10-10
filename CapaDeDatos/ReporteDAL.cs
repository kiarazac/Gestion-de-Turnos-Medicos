using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) responsable de ejecutar consultas y procedimientos almacenados
    /// para reportes gerenciales, estadísticos y de toma de decisiones del centro médico.
    /// </summary>
    public class ReporteDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteMedico_RankingGravedadTriage</c> para obtener
        /// la distribución y porcentaje de turnos de emergencia atendidos por un médico clínico según nivel de gravedad (Alta, Media, Baja).
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario profesional médico clínico.</param>
        /// <param name="fechaDesde">Fecha inicial de filtrado (opcional).</param>
        /// <param name="fechaHasta">Fecha final de filtrado (opcional).</param>
        /// <returns>Lista de objetos <see cref="ReporteMedicoGravedadDTO"/> con las cantidades y porcentajes.</returns>
        public List<ReporteMedicoGravedadDTO> ObtenerRankingGravedadMedico(int idUsuario, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);

                try
                {
                    return context.Database
                        .SqlQueryRaw<ReporteMedicoGravedadDTO>(
                            "EXEC sp_ReporteMedico_RankingGravedadTriage @IdUsuario, @FechaDesde, @FechaHasta",
                            pIdUsuario, pFechaDesde, pFechaHasta)
                        .ToList();
                }
                catch (Exception)
                {
                    var pIdUsuario2 = new SqlParameter("@IdUsuario", idUsuario);
                    var pFechaDesde2 = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                    var pFechaHasta2 = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);

                    string sql = @"
                        DECLARE @TotalEmergencias INT;

                        SELECT @TotalEmergencias = COUNT(DISTINCT t.IdTurno)
                        FROM HistoriasClinicas hc
                        INNER JOIN Turnos t ON hc.IdTurno = t.IdTurno
                        WHERE hc.IdUsuario = @IdUsuario
                          AND hc.Activo = 1
                          AND t.TipoTurno = 'Emergencia'
                          AND (@FechaDesde IS NULL OR CAST(hc.Fecha AS DATE) >= @FechaDesde)
                          AND (@FechaHasta IS NULL OR CAST(hc.Fecha AS DATE) <= @FechaHasta);

                        SELECT 
                            p.IdPrioridad,
                            p.Descripcion AS Gravedad,
                            COUNT(DISTINCT t.IdTurno) AS CantidadTurnos,
                            CAST(
                                CASE WHEN @TotalEmergencias > 0 
                                     THEN (COUNT(DISTINCT t.IdTurno) * 100.0) / @TotalEmergencias 
                                     ELSE 0.0 
                                END AS DECIMAL(5,2)
                            ) AS Porcentaje
                        FROM Prioridades p
                        LEFT JOIN Turnos t ON p.IdPrioridad = t.IdPrioridad 
                                           AND t.TipoTurno = 'Emergencia'
                                           AND EXISTS (
                                               SELECT 1 FROM HistoriasClinicas hc 
                                               WHERE hc.IdTurno = t.IdTurno 
                                                 AND hc.IdUsuario = @IdUsuario 
                                                 AND hc.Activo = 1
                                                 AND (@FechaDesde IS NULL OR CAST(hc.Fecha AS DATE) >= @FechaDesde)
                                                 AND (@FechaHasta IS NULL OR CAST(hc.Fecha AS DATE) <= @FechaHasta)
                                           )
                        WHERE p.Activo = 1
                        GROUP BY p.IdPrioridad, p.Descripcion
                        ORDER BY p.IdPrioridad ASC;";

                    return context.Database
                        .SqlQueryRaw<ReporteMedicoGravedadDTO>(sql, pIdUsuario2, pFechaDesde2, pFechaHasta2)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteMedico_RankingSintomasAtendidos</c> para obtener
        /// los síntomas clínicos más prevalentes presentados por los pacientes de guardia atendidos por el médico tratante.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario profesional médico clínico.</param>
        /// <param name="fechaDesde">Fecha inicial de filtrado (opcional).</param>
        /// <param name="fechaHasta">Fecha final de filtrado (opcional).</param>
        /// <returns>Lista de objetos <see cref="ReporteMedicoSintomaDTO"/> con los casos y porcentajes epidemiológicos.</returns>
        public List<ReporteMedicoSintomaDTO> ObtenerRankingSintomasMedico(int idUsuario, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);

                try
                {
                    return context.Database
                        .SqlQueryRaw<ReporteMedicoSintomaDTO>(
                            "EXEC sp_ReporteMedico_RankingSintomasAtendidos @IdUsuario, @FechaDesde, @FechaHasta",
                            pIdUsuario, pFechaDesde, pFechaHasta)
                        .ToList();
                }
                catch (Exception)
                {
                    var pIdUsuario2 = new SqlParameter("@IdUsuario", idUsuario);
                    var pFechaDesde2 = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                    var pFechaHasta2 = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);

                    string sql = @"
                        DECLARE @TotalCasosSintomas INT;

                        SELECT @TotalCasosSintomas = COUNT(ts.IdTurnoSintoma)
                        FROM TurnoSintomas ts
                        INNER JOIN Turnos t ON ts.IdTurno = t.IdTurno
                        INNER JOIN HistoriasClinicas hc ON t.IdTurno = hc.IdTurno
                        WHERE hc.IdUsuario = @IdUsuario
                          AND hc.Activo = 1
                          AND t.TipoTurno = 'Emergencia'
                          AND ts.Activo = 1
                          AND (@FechaDesde IS NULL OR CAST(hc.Fecha AS DATE) >= @FechaDesde)
                          AND (@FechaHasta IS NULL OR CAST(hc.Fecha AS DATE) <= @FechaHasta);

                        SELECT 
                            s.IdSintoma,
                            s.Descripcion AS Sintoma,
                            s.Gravedad,
                            COUNT(ts.IdTurnoSintoma) AS CantidadCasos,
                            CAST(
                                CASE WHEN @TotalCasosSintomas > 0 
                                     THEN (COUNT(ts.IdTurnoSintoma) * 100.0) / @TotalCasosSintomas 
                                     ELSE 0.0 
                                END AS DECIMAL(5,2)
                            ) AS Porcentaje
                        FROM Sintomas s
                        INNER JOIN TurnoSintomas ts ON s.IdSintoma = ts.IdSintoma AND ts.Activo = 1
                        INNER JOIN Turnos t ON ts.IdTurno = t.IdTurno AND t.TipoTurno = 'Emergencia'
                        INNER JOIN HistoriasClinicas hc ON t.IdTurno = hc.IdTurno AND hc.Activo = 1
                        WHERE hc.IdUsuario = @IdUsuario
                          AND (@FechaDesde IS NULL OR CAST(hc.Fecha AS DATE) >= @FechaDesde)
                          AND (@FechaHasta IS NULL OR CAST(hc.Fecha AS DATE) <= @FechaHasta)
                        GROUP BY s.IdSintoma, s.Descripcion, s.Gravedad
                        ORDER BY CantidadCasos DESC, s.Descripcion ASC;";

                    return context.Database
                        .SqlQueryRaw<ReporteMedicoSintomaDTO>(sql, pIdUsuario2, pFechaDesde2, pFechaHasta2)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGerencialIngresosPorMedico</c> para obtener
        /// la facturación acumulada e ingresos monetarios producidos por cada profesional médico.
        /// </summary>
        public List<ReporteIngresoMedicoDTO> ObtenerIngresosPorMedico(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idEspecialidad = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", (object?)idEspecialidad ?? DBNull.Value);

                return context.Database
                    .SqlQueryRaw<ReporteIngresoMedicoDTO>(
                        "EXEC sp_ReporteGerencialIngresosPorMedico @FechaDesde, @FechaHasta, @IdEspecialidad",
                        pFechaDesde, pFechaHasta, pIdEspecialidad)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGerencialObrasSocialesVsParticulares</c>
        /// para contrastar los montos recaudados de particulares frente a obras sociales y prepagas.
        /// </summary>
        public List<ReporteObraSocialVsParticularDTO> ObtenerObrasSocialesVsParticulares(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);

                return context.Database
                    .SqlQueryRaw<ReporteObraSocialVsParticularDTO>(
                        "EXEC sp_ReporteGerencialObrasSocialesVsParticulares @FechaDesde, @FechaHasta",
                        pFechaDesde, pFechaHasta)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGerencialDemandaMedicos</c> para obtener
        /// el ranking clasificado de médicos con mayor y menor demanda asistencial.
        /// </summary>
        public List<ReporteDemandaMedicoRankingDTO> ObtenerDemandaMedicosRanking(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);

                return context.Database
                    .SqlQueryRaw<ReporteDemandaMedicoRankingDTO>(
                        "EXEC sp_ReporteGerencialDemandaMedicos @FechaDesde, @FechaHasta",
                        pFechaDesde, pFechaHasta)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteMedico_RankingDiagnosticosYSintomas</c>
        /// para obtener los diagnósticos y síntomas más frecuentes atendidos por el médico en su especialidad.
        /// </summary>
        public List<ReporteMedicoDiagnosticoFrecuenteDTO> ObtenerRankingDiagnosticosMedico(int idUsuario, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);

                return context.Database
                    .SqlQueryRaw<ReporteMedicoDiagnosticoFrecuenteDTO>(
                        "EXEC sp_ReporteMedico_RankingDiagnosticosYSintomas @IdUsuario, @FechaDesde, @FechaHasta",
                        pIdUsuario, pFechaDesde, pFechaHasta)
                    .ToList();
            }
        }
    }
}
