using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) encargada de ejecutar las consultas y procedimientos almacenados
    /// del Reporte Operativo de Guardia, Triage y Distribución de Urgencias médicas mediante Entity Framework Core.
    /// </summary>
    public class ReporteGuardiaDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGuardiaTriage_Resumen</c> para obtener
        /// las métricas e indicadores globales de guardia (ingresos por prioridad, atendidos, en espera y tasa de resolución).
        /// </summary>
        /// <param name="fechaDesde">Límite inferior del rango de fechas (opcional).</param>
        /// <param name="fechaHasta">Límite superior del rango de fechas (opcional).</param>
        /// <param name="idPrioridad">Identificador de prioridad específico (1=Alta, 2=Media, 3=Baja) o null para todas.</param>
        /// <returns>Objeto <see cref="ReporteGuardiaResumenDTO"/> con los indicadores calculados.</returns>
        public ReporteGuardiaResumenDTO ObtenerResumenGuardia(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idPrioridad = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);
                var pIdPrioridad = new SqlParameter("@IdPrioridad", (object?)idPrioridad ?? DBNull.Value);

                var resultado = context.Database
                    .SqlQueryRaw<ReporteGuardiaResumenDTO>(
                        "EXEC sp_ReporteGuardiaTriage_Resumen @FechaDesde, @FechaHasta, @IdPrioridad",
                        pFechaDesde, pFechaHasta, pIdPrioridad)
                    .AsEnumerable()
                    .FirstOrDefault();

                return resultado ?? new ReporteGuardiaResumenDTO();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGuardiaTriage_RankingSintomas</c> para obtener
        /// el ranking de síntomas y motivos de consulta más frecuentes en el triage de guardia.
        /// </summary>
        /// <param name="fechaDesde">Fecha inicial de filtrado opcional.</param>
        /// <param name="fechaHasta">Fecha final de filtrado opcional.</param>
        /// <param name="idPrioridad">Filtro opcional por nivel de prioridad.</param>
        /// <returns>Lista de <see cref="ReporteGuardiaSintomaDTO"/> ordenada por volumen de casos descendente.</returns>
        public List<ReporteGuardiaSintomaDTO> ObtenerRankingSintomas(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idPrioridad = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);
                var pIdPrioridad = new SqlParameter("@IdPrioridad", (object?)idPrioridad ?? DBNull.Value);

                return context.Database
                    .SqlQueryRaw<ReporteGuardiaSintomaDTO>(
                        "EXEC sp_ReporteGuardiaTriage_RankingSintomas @FechaDesde, @FechaHasta, @IdPrioridad",
                        pFechaDesde, pFechaHasta, pIdPrioridad)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteGuardiaTriage_Detalle</c> para recuperar
        /// la nómina detallada de ingresos a guardia con paciente, triage, síntomas y consultorio.
        /// </summary>
        /// <param name="fechaDesde">Fecha inicial de filtrado opcional.</param>
        /// <param name="fechaHasta">Fecha final de filtrado opcional.</param>
        /// <param name="idPrioridad">Filtro opcional por nivel de prioridad.</param>
        /// <returns>Lista de <see cref="ReporteGuardiaDetalleDTO"/> con los registros individuales de guardia.</returns>
        public List<ReporteGuardiaDetalleDTO> ObtenerDetalleGuardia(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idPrioridad = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde?.Date ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta?.Date ?? DBNull.Value);
                var pIdPrioridad = new SqlParameter("@IdPrioridad", (object?)idPrioridad ?? DBNull.Value);

                return context.Database
                    .SqlQueryRaw<ReporteGuardiaDetalleDTO>(
                        "EXEC sp_ReporteGuardiaTriage_Detalle @FechaDesde, @FechaHasta, @IdPrioridad",
                        pFechaDesde, pFechaHasta, pIdPrioridad)
                    .ToList();
            }
        }
    }
}
