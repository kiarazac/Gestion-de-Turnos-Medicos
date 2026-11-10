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
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGerencialFinanzasResumen</c> para obtener los totales de
        /// recaudación del período: cobrado en caja, a cobrar a obras sociales, total facturado, ticket promedio y ausentismo.
        /// </summary>
        public ReporteFinanzasResumenDTO ObtenerFinanzasResumen(DateTime fechaDesde, DateTime fechaHasta, decimal arancelEspecialidad, decimal arancelEmergencia)
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database
                    .SqlQueryRaw<ReporteFinanzasResumenDTO>(
                        "EXEC sp_ReporteGerencialFinanzasResumen @FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia",
                        ParametrosFacturacion(fechaDesde, fechaHasta, arancelEspecialidad, arancelEmergencia))
                    .AsEnumerable()
                    .FirstOrDefault() ?? new ReporteFinanzasResumenDTO();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGerencialRecaudacionDiaria</c> para obtener
        /// la evolución de la recaudación por día (o por mes en rangos largos).
        /// </summary>
        public List<ReporteRecaudacionDiariaDTO> ObtenerRecaudacionDiaria(DateTime fechaDesde, DateTime fechaHasta, decimal arancelEspecialidad, decimal arancelEmergencia)
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database
                    .SqlQueryRaw<ReporteRecaudacionDiariaDTO>(
                        "EXEC sp_ReporteGerencialRecaudacionDiaria @FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia",
                        ParametrosFacturacion(fechaDesde, fechaHasta, arancelEspecialidad, arancelEmergencia))
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGerencialRankingMedicos</c> para clasificar a los
        /// profesionales médicos según la facturación (caja + obra social) de sus atenciones realizadas.
        /// </summary>
        public List<ReporteRankingFacturacionDTO> ObtenerRankingMedicos(DateTime fechaDesde, DateTime fechaHasta, decimal arancelEspecialidad, decimal arancelEmergencia, int? idEspecialidad = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var parametros = ParametrosFacturacion(fechaDesde, fechaHasta, arancelEspecialidad, arancelEmergencia)
                    .Append(new SqlParameter("@IdEspecialidad", (object?)idEspecialidad ?? DBNull.Value))
                    .ToArray();

                return context.Database
                    .SqlQueryRaw<ReporteRankingFacturacionDTO>(
                        "EXEC sp_ReporteGerencialRankingMedicos @FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia, @IdEspecialidad",
                        parametros)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGerencialRankingEspecialidades</c> (ranking de especialidades por facturación).
        /// </summary>
        public List<ReporteRankingFacturacionDTO> ObtenerRankingEspecialidades(DateTime fechaDesde, DateTime fechaHasta, decimal arancelEspecialidad, decimal arancelEmergencia)
        {
            return EjecutarRankingFacturacion("EXEC sp_ReporteGerencialRankingEspecialidades @FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia", fechaDesde, fechaHasta, arancelEspecialidad, arancelEmergencia);
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGerencialRankingObrasSociales</c> (ranking de obras sociales por facturación,
        /// con monto a cobrar y cobertura vigente vs. aplicada).
        /// </summary>
        public List<ReporteRankingFacturacionDTO> ObtenerRankingObrasSociales(DateTime fechaDesde, DateTime fechaHasta, decimal arancelEspecialidad, decimal arancelEmergencia)
        {
            return EjecutarRankingFacturacion("EXEC sp_ReporteGerencialRankingObrasSociales @FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia", fechaDesde, fechaHasta, arancelEspecialidad, arancelEmergencia);
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGerencialFacturacionPorSegmento</c> (facturación por modalidad y por cobertura).
        /// </summary>
        public List<ReporteRankingFacturacionDTO> ObtenerFacturacionPorSegmento(DateTime fechaDesde, DateTime fechaHasta, decimal arancelEspecialidad, decimal arancelEmergencia)
        {
            return EjecutarRankingFacturacion("EXEC sp_ReporteGerencialFacturacionPorSegmento @FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia", fechaDesde, fechaHasta, arancelEspecialidad, arancelEmergencia);
        }

        private List<ReporteRankingFacturacionDTO> EjecutarRankingFacturacion(string sql, DateTime fechaDesde, DateTime fechaHasta, decimal arancelEspecialidad, decimal arancelEmergencia)
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database
                    .SqlQueryRaw<ReporteRankingFacturacionDTO>(
                        sql,
                        ParametrosFacturacion(fechaDesde, fechaHasta, arancelEspecialidad, arancelEmergencia))
                    .ToList();
            }
        }

        private static SqlParameter[] ParametrosFacturacion(DateTime fechaDesde, DateTime fechaHasta, decimal arancelEspecialidad, decimal arancelEmergencia)
        {
            return new[]
            {
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date),
                new SqlParameter("@ArancelEspecialidad", arancelEspecialidad),
                new SqlParameter("@ArancelEmergencia", arancelEmergencia)
            };
        }
    }
}
