using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) para las ausencias programadas de los profesionales médicos mediante Stored Procedures.
    /// </summary>
    public class AusenciaDAL
    {
        /// <summary>
        /// Ejecuta <c>sp_RegistrarAusencias</c>: registra un día de ausencia por cada fecha del rango.
        /// </summary>
        /// <param name="idUsuario">Médico que se ausenta.</param>
        /// <param name="desde">Primer día de ausencia.</param>
        /// <param name="hasta">Último día de ausencia.</param>
        /// <param name="motivo">Motivo opcional.</param>
        /// <returns>Días registrados y turnos que quedaron sin profesional.</returns>
        public ResultadoAusenciaDTO RegistrarAusencias(int idUsuario, DateTime desde, DateTime hasta, string? motivo)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pDesde = new SqlParameter("@FechaDesde", desde.Date);
                var pHasta = new SqlParameter("@FechaHasta", hasta.Date);
                var pMotivo = new SqlParameter("@Motivo", (object?)motivo ?? DBNull.Value);

                return context.Database
                    .SqlQueryRaw<ResultadoAusenciaDTO>("EXEC sp_RegistrarAusencias @IdUsuario, @FechaDesde, @FechaHasta, @Motivo",
                        pIdUsuario, pDesde, pHasta, pMotivo)
                    .AsEnumerable()
                    .First();
            }
        }

        /// <summary>
        /// Ejecuta <c>sp_ListarAusenciasPorMedico</c>: ausencias vigentes (desde hoy) del médico.
        /// </summary>
        /// <param name="idUsuario">Médico consultado.</param>
        public List<AusenciaMedicaDTO> ListarAusenciasPorMedico(int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                return context.Database
                    .SqlQueryRaw<AusenciaMedicaDTO>("EXEC sp_ListarAusenciasPorMedico @IdUsuario", pIdUsuario)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta <c>sp_AnularAusencia</c>: baja lógica de una ausencia del propio médico.
        /// </summary>
        /// <param name="idAusencia">Ausencia a anular.</param>
        /// <param name="idUsuario">Médico titular de la ausencia.</param>
        public void AnularAusencia(int idAusencia, int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdAusencia = new SqlParameter("@IdAusencia", idAusencia);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                context.Database.ExecuteSqlRaw("EXEC sp_AnularAusencia @IdAusencia, @IdUsuario", pIdAusencia, pIdUsuario);
            }
        }
    }
}
