using System;
using System.Collections.Generic;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.CapaNegocio
{
    /// <summary>
    /// Capa de lógica de negocio (BLL) encargada del cierre diario de caja y conciliación de turnos.
    /// Valida parámetros temporales y coordina con <see cref="CajaDAL"/>.
    /// </summary>
    public class CajaBLL
    {
        private readonly CajaDAL _cajaDAL = new CajaDAL();

        /// <summary>
        /// Obtiene las métricas consolidadas del cierre de caja para la fecha dada.
        /// </summary>
        /// <param name="fecha">Fecha a auditar.</param>
        /// <returns><see cref="ReporteCierreCajaResumenDTO"/> con los totales de caja.</returns>
        public ReporteCierreCajaResumenDTO ObtenerResumenCierreCaja(DateTime fecha)
        {
            return _cajaDAL.ObtenerResumenCierreCaja(fecha.Date);
        }

        /// <summary>
        /// Obtiene el detalle nominal de todos los turnos emitidos/atendidos durante la jornada de caja.
        /// </summary>
        /// <param name="fecha">Fecha a auditar.</param>
        /// <returns>Lista de <see cref="ReporteCierreCajaDetalleDTO"/> con los registros de cobro.</returns>
        public List<ReporteCierreCajaDetalleDTO> ObtenerDetalleCierreCaja(DateTime fecha)
        {
            return _cajaDAL.ObtenerDetalleCierreCaja(fecha.Date);
        }
    }
}
