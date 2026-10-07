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
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteDemandaEspecialidades</c> para obtener
        /// las métricas de turnos solicitados, atendidos y en espera clasificados por especialidad médica.
        /// </summary>
        /// <param name="fechaDesde">Fecha inicial del intervalo de análisis (opcional).</param>
        /// <param name="fechaHasta">Fecha final del intervalo de análisis (opcional).</param>
        /// <param name="idEspecialidad">Identificador de especialidad médica para filtrar una sola, o null para todas.</param>
        /// <returns>Lista de objetos <see cref="ReporteDemandaEspecialidadDTO"/> con las métricas consolidadas.</returns>
        public List<ReporteDemandaEspecialidadDTO> ObtenerDemandaEspecialidades(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idEspecialidad = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", (object?)idEspecialidad ?? DBNull.Value);

                try
                {
                    return context.Database
                        .SqlQueryRaw<ReporteDemandaEspecialidadDTO>(
                            "EXEC sp_ReporteDemandaEspecialidades @FechaDesde, @FechaHasta, @IdEspecialidad",
                            pFechaDesde, pFechaHasta, pIdEspecialidad)
                        .ToList();
                }
                catch (SqlException)
                {
                    string sql = @"
                        SELECT 
                            e.IdEspecialidad,
                            e.Nombre AS Especialidad,
                            COUNT(t.IdTurno) AS TotalTurnos,
                            SUM(CASE WHEN t.Estado IN ('Atendido', 'Finalizado') THEN 1 ELSE 0 END) AS TurnosAtendidos,
                            SUM(CASE WHEN t.Estado IN ('En Espera', 'Llamado', 'En Consulta') THEN 1 ELSE 0 END) AS TurnosEnEspera,
                            CAST(
                                CASE 
                                    WHEN COUNT(t.IdTurno) > 0 
                                    THEN (CAST(SUM(CASE WHEN t.Estado IN ('Atendido', 'Finalizado') THEN 1 ELSE 0 END) AS DECIMAL(10,2)) / COUNT(t.IdTurno)) * 100.0
                                    ELSE 0.0 
                                END AS DECIMAL(5,2)
                            ) AS PorcentajeAtencion
                        FROM Especialidades e
                        LEFT JOIN Turnos t ON e.IdEspecialidad = t.IdEspecialidad 
                                           AND (@FechaDesde IS NULL OR CAST(t.Fecha AS DATE) >= @FechaDesde)
                                           AND (@FechaHasta IS NULL OR CAST(t.Fecha AS DATE) <= @FechaHasta)
                                           AND t.Activo = 1
                        WHERE (@IdEspecialidad IS NULL OR e.IdEspecialidad = @IdEspecialidad)
                          AND e.Activo = 1
                        GROUP BY e.IdEspecialidad, e.Nombre
                        ORDER BY TotalTurnos DESC, e.Nombre ASC;";

                    return context.Database
                        .SqlQueryRaw<ReporteDemandaEspecialidadDTO>(sql, pFechaDesde, pFechaHasta, pIdEspecialidad)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteProductividadMedicos</c> para obtener
        /// las estadísticas de consultas atendidas y pacientes únicos por profesional de la salud.
        /// </summary>
        /// <param name="fechaDesde">Fecha inicial del intervalo de análisis (opcional).</param>
        /// <param name="fechaHasta">Fecha final del intervalo de análisis (opcional).</param>
        /// <param name="idEspecialidad">Identificador de especialidad médica para filtrar médicos de un área, o null para todas.</param>
        /// <returns>Lista de objetos <see cref="ReporteProductividadMedicoDTO"/> con los datos de productividad.</returns>
        public List<ReporteProductividadMedicoDTO> ObtenerProductividadMedicos(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idEspecialidad = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", (object?)idEspecialidad ?? DBNull.Value);

                try
                {
                    return context.Database
                        .SqlQueryRaw<ReporteProductividadMedicoDTO>(
                            "EXEC sp_ReporteProductividadMedicos @FechaDesde, @FechaHasta, @IdEspecialidad",
                            pFechaDesde, pFechaHasta, pIdEspecialidad)
                        .ToList();
                }
                catch (SqlException)
                {
                    string sql = @"
                        SELECT 
                            u.IdUsuario,
                            u.Nombre AS NombreMedico,
                            u.Apellido AS ApellidoMedico,
                            ISNULL(u.NroMatricula, '--') AS Matricula,
                            ISNULL(e.Nombre, 'General') AS Especialidad,
                            COUNT(hc.IdHistoria) AS ConsultasAtendidas,
                            COUNT(DISTINCT hc.IdPaciente) AS PacientesUnicos
                        FROM Usuarios u
                        INNER JOIN Roles r ON u.IdRol = r.IdRol
                        LEFT JOIN HistoriasClinicas hc ON u.IdUsuario = hc.IdUsuario 
                                                       AND (@FechaDesde IS NULL OR CAST(hc.Fecha AS DATE) >= @FechaDesde)
                                                       AND (@FechaHasta IS NULL OR CAST(hc.Fecha AS DATE) <= @FechaHasta)
                                                       AND hc.Activo = 1
                        LEFT JOIN MedicosEspecialidades me ON u.IdUsuario = me.IdUsuario AND me.Activo = 1
                        LEFT JOIN Especialidades e ON me.IdEspecialidad = e.IdEspecialidad
                        WHERE (u.IdRol = 1 OR r.Descripcion LIKE '%Médic%' OR r.Descripcion LIKE '%Medic%')
                          AND u.Activo = 1
                          AND (@IdEspecialidad IS NULL OR e.IdEspecialidad = @IdEspecialidad)
                        GROUP BY u.IdUsuario, u.Nombre, u.Apellido, u.NroMatricula, e.Nombre
                        ORDER BY ConsultasAtendidas DESC, u.Apellido ASC;";

                    return context.Database
                        .SqlQueryRaw<ReporteProductividadMedicoDTO>(sql, pFechaDesde, pFechaHasta, pIdEspecialidad)
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
