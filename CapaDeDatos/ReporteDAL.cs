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
        /// las métricas de turnos solicitados, atendidos, cancelados y pendientes clasificados por especialidad médica.
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

                return context.Database
                    .SqlQueryRaw<ReporteDemandaEspecialidadDTO>(
                        "EXEC sp_ReporteDemandaEspecialidades @FechaDesde, @FechaHasta, @IdEspecialidad",
                        pFechaDesde, pFechaHasta, pIdEspecialidad)
                    .ToList();
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

                return context.Database
                    .SqlQueryRaw<ReporteProductividadMedicoDTO>(
                        "EXEC sp_ReporteProductividadMedicos @FechaDesde, @FechaHasta, @IdEspecialidad",
                        pFechaDesde, pFechaHasta, pIdEspecialidad)
                    .ToList();
            }
        }
    }
}
