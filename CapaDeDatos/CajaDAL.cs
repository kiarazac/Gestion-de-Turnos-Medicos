using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) responsable del módulo de tesorería y cierre de caja diario.
    /// Ejecuta los procedimientos almacenados de conciliación de turnos y recaudación monetaria.
    /// </summary>
    public class CajaDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteCierreCajaDiario</c> para obtener
        /// las métricas cuantitativas y montos consolidados recaudados en la fecha especificada.
        /// </summary>
        /// <param name="fecha">Fecha de la jornada a liquidar.</param>
        /// <returns>Objeto <see cref="ReporteCierreCajaResumenDTO"/> con los totales de caja.</returns>
        public ReporteCierreCajaResumenDTO ObtenerResumenCierreCaja(DateTime fecha)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFecha = new SqlParameter("@Fecha", fecha.Date);

                return context.Database
                    .SqlQueryRaw<ReporteCierreCajaResumenDTO>("EXEC sp_ReporteCierreCajaDiario @Fecha", pFecha)
                    .AsEnumerable()
                    .FirstOrDefault() ?? new ReporteCierreCajaResumenDTO { FechaCaja = fecha.Date };
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteCierreCajaDetalle</c> para recuperar
        /// la nómina completa y discriminada de turnos y montos cobrados durante la fecha indicada.
        /// </summary>
        /// <param name="fecha">Fecha de la jornada a auditar.</param>
        /// <returns>Lista de <see cref="ReporteCierreCajaDetalleDTO"/> con cada turno emitido/cobrado.</returns>
        public List<ReporteCierreCajaDetalleDTO> ObtenerDetalleCierreCaja(DateTime fecha)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFecha = new SqlParameter("@Fecha", fecha.Date);

                return context.Database
                    .SqlQueryRaw<ReporteCierreCajaDetalleDTO>("EXEC sp_ReporteCierreCajaDetalle @Fecha", pFecha)
                    .ToList();
            }
        }
    }
}
