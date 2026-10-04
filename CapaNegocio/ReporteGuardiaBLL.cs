using System;
using System.Collections.Generic;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio (BLL) encargada del procesamiento, validación y cómputo de métricas
    /// para el Reporte Operativo de Guardia, Triage y Distribución de Urgencias médicas.
    /// </summary>
    public class ReporteGuardiaBLL
    {
        private readonly ReporteGuardiaDAL _reporteGuardiaDAL = new ReporteGuardiaDAL();

        /// <summary>
        /// Obtiene y valida los indicadores globales y distribución por severidad de la guardia médica.
        /// </summary>
        /// <param name="fechaDesde">Fecha inicial de filtrado opcional.</param>
        /// <param name="fechaHasta">Fecha final de filtrado opcional.</param>
        /// <param name="idPrioridad">Filtro opcional por prioridad (1=Alta, 2=Media, 3=Baja).</param>
        /// <returns>Instancia de <see cref="ReporteGuardiaResumenDTO"/> con los totales y tasas computadas.</returns>
        /// <exception cref="ArgumentException">Se lanza si la fecha 'Desde' es posterior a la fecha 'Hasta'.</exception>
        public ReporteGuardiaResumenDTO ObtenerResumenGuardia(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idPrioridad = null)
        {
            ValidarRangoFechas(fechaDesde, fechaHasta);

            if (idPrioridad.HasValue && idPrioridad.Value <= 0)
                idPrioridad = null;

            return _reporteGuardiaDAL.ObtenerResumenGuardia(fechaDesde, fechaHasta, idPrioridad);
        }

        /// <summary>
        /// Obtiene y valida el ranking de síntomas más frecuentes manifestados en el triage de guardia.
        /// </summary>
        /// <param name="fechaDesde">Fecha inicial de filtrado opcional.</param>
        /// <param name="fechaHasta">Fecha final de filtrado opcional.</param>
        /// <param name="idPrioridad">Filtro opcional por prioridad.</param>
        /// <returns>Lista de <see cref="ReporteGuardiaSintomaDTO"/> ordenados de forma descendente por frecuencia.</returns>
        /// <exception cref="ArgumentException">Se lanza si las fechas son cronológicamente inconsistentes.</exception>
        public List<ReporteGuardiaSintomaDTO> ObtenerRankingSintomas(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idPrioridad = null)
        {
            ValidarRangoFechas(fechaDesde, fechaHasta);

            if (idPrioridad.HasValue && idPrioridad.Value <= 0)
                idPrioridad = null;

            return _reporteGuardiaDAL.ObtenerRankingSintomas(fechaDesde, fechaHasta, idPrioridad);
        }

        /// <summary>
        /// Obtiene y valida el listado exhaustivo de turnos de guardia atendidos o en espera.
        /// </summary>
        /// <param name="fechaDesde">Fecha inicial de filtrado opcional.</param>
        /// <param name="fechaHasta">Fecha final de filtrado opcional.</param>
        /// <param name="idPrioridad">Filtro opcional por nivel de prioridad.</param>
        /// <returns>Lista de <see cref="ReporteGuardiaDetalleDTO"/> con los registros individuales de pacientes y triage.</returns>
        /// <exception cref="ArgumentException">Se lanza si las fechas son inconsistentes.</exception>
        public List<ReporteGuardiaDetalleDTO> ObtenerDetalleGuardia(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idPrioridad = null)
        {
            ValidarRangoFechas(fechaDesde, fechaHasta);

            if (idPrioridad.HasValue && idPrioridad.Value <= 0)
                idPrioridad = null;

            return _reporteGuardiaDAL.ObtenerDetalleGuardia(fechaDesde, fechaHasta, idPrioridad);
        }

        /// <summary>
        /// Comprueba que la fecha inicial del intervalo no sea posterior a la fecha final.
        /// </summary>
        private void ValidarRangoFechas(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                throw new ArgumentException("La fecha inicial ('Desde') no puede ser posterior a la fecha final ('Hasta').");
            }
        }
    }
}
