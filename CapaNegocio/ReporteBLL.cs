using System;
using System.Collections.Generic;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio (BLL) encargada de centralizar la generación de reportes gerenciales,
    /// consolidación de estadísticas de turnos y métricas de productividad clínica para la toma de decisiones.
    /// </summary>
    public class ReporteBLL
    {
        private readonly ReporteDAL _reporteDAL = new ReporteDAL();
        private readonly EspecialidadDAL _especialidadDAL = new EspecialidadDAL();

        /// <summary>
        /// Obtiene y valida las estadísticas consolidadas de demanda por especialidad médica
        /// en un rango temporal determinado.
        /// </summary>
        /// <param name="fechaDesde">Fecha inicial de filtrado opcional.</param>
        /// <param name="fechaHasta">Fecha final de filtrado opcional.</param>
        /// <param name="idEspecialidad">Identificador de especialidad médica específica o null para todas.</param>
        /// <returns>Lista de <see cref="ReporteDemandaEspecialidadDTO"/> con las métricas computadas.</returns>
        /// <exception cref="ArgumentException">Se lanza si la fecha 'Desde' es posterior a la fecha 'Hasta'.</exception>
        public List<ReporteDemandaEspecialidadDTO> ObtenerDemandaEspecialidades(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idEspecialidad = null)
        {
            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                throw new ArgumentException("La fecha inicial ('Desde') no puede ser posterior a la fecha final ('Hasta').");
            }

            if (idEspecialidad.HasValue && idEspecialidad.Value <= 0)
            {
                idEspecialidad = null;
            }

            return _reporteDAL.ObtenerDemandaEspecialidades(fechaDesde, fechaHasta, idEspecialidad);
        }

        /// <summary>
        /// Obtiene y valida las métricas de productividad y volumen de atención de los profesionales médicos.
        /// </summary>
        /// <param name="fechaDesde">Fecha inicial de filtrado opcional.</param>
        /// <param name="fechaHasta">Fecha final de filtrado opcional.</param>
        /// <param name="idEspecialidad">Identificador de especialidad médica para filtrar o null para todas.</param>
        /// <returns>Lista de <see cref="ReporteProductividadMedicoDTO"/> con los registros de consultas y pacientes atendidos.</returns>
        /// <exception cref="ArgumentException">Se lanza si la fecha 'Desde' es posterior a la fecha 'Hasta'.</exception>
        public List<ReporteProductividadMedicoDTO> ObtenerProductividadMedicos(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idEspecialidad = null)
        {
            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                throw new ArgumentException("La fecha inicial ('Desde') no puede ser posterior a la fecha final ('Hasta').");
            }

            if (idEspecialidad.HasValue && idEspecialidad.Value <= 0)
            {
                idEspecialidad = null;
            }

            return _reporteDAL.ObtenerProductividadMedicos(fechaDesde, fechaHasta, idEspecialidad);
        }

        /// <summary>
        /// Obtiene la lista de especialidades médicas activas para abastecer los filtros de selección en los reportes.
        /// </summary>
        /// <returns>Lista de <see cref="EspecialidadDTO"/> activas en el sistema.</returns>
        public List<EspecialidadDTO> ObtenerEspecialidadesParaFiltro()
        {
            return _especialidadDAL.ListarEspecialidades(incluirInactivas: false);
        }
    }
}
