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
        /// <param name="soloEmitidosHoy">Si es true, consolida según la fecha de emisión del turno.</param>
        /// <returns><see cref="ReporteCierreCajaResumenDTO"/> con los totales de caja.</returns>
        public ReporteCierreCajaResumenDTO ObtenerResumenCierreCaja(DateTime fecha, bool soloEmitidosHoy = false)
        {
            return _cajaDAL.ObtenerResumenCierreCaja(fecha.Date, soloEmitidosHoy);
        }

        /// <summary>
        /// Obtiene el detalle nominal de todos los turnos emitidos/atendidos durante la jornada de caja,
        /// con filtro opcional por especialidad y por fecha de emisión.
        /// </summary>
        /// <param name="fecha">Fecha a auditar.</param>
        /// <param name="especialidad">Especialidad médica o 'Emergencia' (opcional).</param>
        /// <param name="soloEmitidosHoy">Si es true, filtra por turnos emitidos/creados en la fecha dada.</param>
        /// <returns>Lista de <see cref="ReporteCierreCajaDetalleDTO"/> con los registros de cobro.</returns>
        public List<ReporteCierreCajaDetalleDTO> ObtenerDetalleCierreCaja(DateTime fecha, string? especialidad = null, bool soloEmitidosHoy = false)
        {
            return _cajaDAL.ObtenerDetalleCierreCaja(fecha.Date, especialidad, soloEmitidosHoy);
        }

        /// <summary>
        /// Consolida dinámicamente un resumen financiero a partir de una lista filtrada de turnos.
        /// Permite calcular con precisión los KPIs de caja para cualquier subconjunto o especialidad.
        /// </summary>
        /// <param name="fecha">Fecha de consulta.</param>
        /// <param name="detalles">Lista de turnos filtrados.</param>
        /// <returns><see cref="ReporteCierreCajaResumenDTO"/> con los totales calculados.</returns>
        public ReporteCierreCajaResumenDTO CalcularResumenDeDetalles(DateTime fecha, List<ReporteCierreCajaDetalleDTO> detalles)
        {
            if (detalles == null || detalles.Count == 0)
            {
                return new ReporteCierreCajaResumenDTO { FechaCaja = fecha.Date };
            }

            return new ReporteCierreCajaResumenDTO
            {
                FechaCaja = fecha.Date,
                TotalTurnos = detalles.Count,
                TotalRecaudado = detalles.Sum(d => d.MontoCobrado),
                TurnosParticulares = detalles.Count(d => d.EsParticular),
                MontoParticulares = detalles.Where(d => d.EsParticular).Sum(d => d.MontoCobrado),
                TurnosObraSocial = detalles.Count(d => !d.EsParticular),
                MontoObraSocial = detalles.Where(d => !d.EsParticular).Sum(d => d.MontoCobrado),
                TurnosEmergencia = detalles.Count(d => d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)),
                MontoEmergencia = detalles.Where(d => d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)).Sum(d => d.MontoCobrado),
                TurnosEspecialidad = detalles.Count(d => !d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)),
                MontoEspecialidad = detalles.Where(d => !d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)).Sum(d => d.MontoCobrado)
            };
        }
    }
}
