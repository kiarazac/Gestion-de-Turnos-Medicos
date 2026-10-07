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
        /// Obtiene y valida la distribución y porcentaje de turnos de emergencia atendidos por un médico clínico según nivel de gravedad (Alta, Media, Baja).
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario profesional médico clínico.</param>
        /// <param name="fechaDesde">Fecha inicial de filtrado (opcional).</param>
        /// <param name="fechaHasta">Fecha final de filtrado (opcional).</param>
        /// <returns>Lista de <see cref="ReporteMedicoGravedadDTO"/> con los registros consolidados.</returns>
        public List<ReporteMedicoGravedadDTO> ObtenerRankingGravedadMedico(int idUsuario, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException("Debe indicar un identificador de usuario médico válido.");
            }

            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                throw new ArgumentException("La fecha inicial ('Desde') no puede ser posterior a la fecha final ('Hasta').");
            }

            return _reporteDAL.ObtenerRankingGravedadMedico(idUsuario, fechaDesde, fechaHasta);
        }

        /// <summary>
        /// Obtiene y valida el ranking epidemiológico de síntomas manifestados por pacientes de guardia atendidos por el médico tratante.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario profesional médico clínico.</param>
        /// <param name="fechaDesde">Fecha inicial de filtrado (opcional).</param>
        /// <param name="fechaHasta">Fecha final de filtrado (opcional).</param>
        /// <returns>Lista de <see cref="ReporteMedicoSintomaDTO"/> con los síntomas clasificados.</returns>
        public List<ReporteMedicoSintomaDTO> ObtenerRankingSintomasMedico(int idUsuario, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException("Debe indicar un identificador de usuario médico válido.");
            }

            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                throw new ArgumentException("La fecha inicial ('Desde') no puede ser posterior a la fecha final ('Hasta').");
            }

            return _reporteDAL.ObtenerRankingSintomasMedico(idUsuario, fechaDesde, fechaHasta);
        }

        /// <summary>
        /// Obtiene la lista de especialidades médicas activas para abastecer los filtros de selección en los reportes.
        /// </summary>
        /// <returns>Lista de <see cref="EspecialidadDTO"/> activas en el sistema.</returns>
        public List<EspecialidadDTO> ObtenerEspecialidadesParaFiltro()
        {
            return _especialidadDAL.ListarEspecialidades(incluirInactivas: false);
        }

        /// <summary>
        /// Obtiene y valida las estadísticas de ingresos y facturación acumulada por profesional médico.
        /// </summary>
        public List<ReporteIngresoMedicoDTO> ObtenerIngresosPorMedico(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? idEspecialidad = null)
        {
            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                throw new ArgumentException("La fecha inicial ('Desde') no puede ser posterior a la fecha final ('Hasta').");
            }

            if (idEspecialidad.HasValue && idEspecialidad.Value <= 0)
            {
                idEspecialidad = null;
            }

            return _reporteDAL.ObtenerIngresosPorMedico(fechaDesde, fechaHasta, idEspecialidad);
        }

        /// <summary>
        /// Obtiene la comparativa económica de facturación de particulares frente a obras sociales y prepagas.
        /// </summary>
        public List<ReporteObraSocialVsParticularDTO> ObtenerObrasSocialesVsParticulares(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                throw new ArgumentException("La fecha inicial ('Desde') no puede ser posterior a la fecha final ('Hasta').");
            }

            return _reporteDAL.ObtenerObrasSocialesVsParticulares(fechaDesde, fechaHasta);
        }

        /// <summary>
        /// Obtiene el ranking analítico de médicos ordenados por demanda asistencial.
        /// </summary>
        public List<ReporteDemandaMedicoRankingDTO> ObtenerDemandaMedicosRanking(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                throw new ArgumentException("La fecha inicial ('Desde') no puede ser posterior a la fecha final ('Hasta').");
            }

            return _reporteDAL.ObtenerDemandaMedicosRanking(fechaDesde, fechaHasta);
        }

        /// <summary>
        /// Obtiene el ranking consolidado de diagnósticos clínicos y síntomas atendidos por el médico en su especialidad.
        /// </summary>
        public List<ReporteMedicoDiagnosticoFrecuenteDTO> ObtenerRankingDiagnosticosMedico(int idUsuario, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException("Debe indicar un identificador de usuario médico válido.");
            }

            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                throw new ArgumentException("La fecha inicial ('Desde') no puede ser posterior a la fecha final ('Hasta').");
            }

            return _reporteDAL.ObtenerRankingDiagnosticosMedico(idUsuario, fechaDesde, fechaHasta);
        }
    }
}
