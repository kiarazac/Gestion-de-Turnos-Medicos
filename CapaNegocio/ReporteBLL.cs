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
        /// Obtiene los totales de recaudación del período (cobrado en caja, a cobrar a obras sociales, facturado, ticket promedio y ausentismo).
        /// </summary>
        public ReporteFinanzasResumenDTO ObtenerFinanzasResumen(DateTime fechaDesde, DateTime fechaHasta)
        {
            ValidarRango(fechaDesde, fechaHasta);
            return _reporteDAL.ObtenerFinanzasResumen(fechaDesde, fechaHasta, ArancelEspecialidad, ArancelEmergencia);
        }

        /// <summary>
        /// Obtiene la evolución de la recaudación por día (o por mes si el rango supera los 62 días).
        /// </summary>
        public List<ReporteRecaudacionDiariaDTO> ObtenerRecaudacionDiaria(DateTime fechaDesde, DateTime fechaHasta)
        {
            ValidarRango(fechaDesde, fechaHasta);
            return _reporteDAL.ObtenerRecaudacionDiaria(fechaDesde, fechaHasta, ArancelEspecialidad, ArancelEmergencia);
        }

        /// <summary>
        /// Obtiene el ranking de médicos por facturación de sus atenciones realizadas, opcionalmente filtrado por especialidad del turno.
        /// </summary>
        public List<ReporteRankingFacturacionDTO> ObtenerRankingMedicos(DateTime fechaDesde, DateTime fechaHasta, int? idEspecialidad = null)
        {
            ValidarRango(fechaDesde, fechaHasta);
            if (idEspecialidad.HasValue && idEspecialidad.Value <= 0)
            {
                idEspecialidad = null;
            }

            return _reporteDAL.ObtenerRankingMedicos(fechaDesde, fechaHasta, ArancelEspecialidad, ArancelEmergencia, idEspecialidad);
        }

        /// <summary>
        /// Obtiene el ranking de especialidades por facturación.
        /// </summary>
        public List<ReporteRankingFacturacionDTO> ObtenerRankingEspecialidades(DateTime fechaDesde, DateTime fechaHasta)
        {
            ValidarRango(fechaDesde, fechaHasta);
            return _reporteDAL.ObtenerRankingEspecialidades(fechaDesde, fechaHasta, ArancelEspecialidad, ArancelEmergencia);
        }

        /// <summary>
        /// Obtiene el ranking de obras sociales por facturación (copagos, monto a cobrar y cobertura vigente vs. aplicada).
        /// </summary>
        public List<ReporteRankingFacturacionDTO> ObtenerRankingObrasSociales(DateTime fechaDesde, DateTime fechaHasta)
        {
            ValidarRango(fechaDesde, fechaHasta);
            return _reporteDAL.ObtenerRankingObrasSociales(fechaDesde, fechaHasta, ArancelEspecialidad, ArancelEmergencia);
        }

        /// <summary>
        /// Obtiene la facturación por modalidad (Emergencia / Especialidad) y por cobertura (Obra Social / Particular).
        /// </summary>
        public List<ReporteRankingFacturacionDTO> ObtenerFacturacionPorSegmento(DateTime fechaDesde, DateTime fechaHasta)
        {
            ValidarRango(fechaDesde, fechaHasta);
            return _reporteDAL.ObtenerFacturacionPorSegmento(fechaDesde, fechaHasta, ArancelEspecialidad, ArancelEmergencia);
        }

        /// <summary>
        /// Devuelve el período inmediatamente anterior de igual duración (ej. 01/11–07/11 → 25/10–31/10), para comparar KPIs.
        /// </summary>
        public static (DateTime Desde, DateTime Hasta) ObtenerPeriodoAnterior(DateTime fechaDesde, DateTime fechaHasta)
        {
            int dias = (fechaHasta.Date - fechaDesde.Date).Days + 1;
            DateTime hastaAnterior = fechaDesde.Date.AddDays(-1);
            return (hastaAnterior.AddDays(1 - dias), hastaAnterior);
        }

        /// <summary>
        /// Variación porcentual entre dos valores; null si el período anterior no tiene movimiento.
        /// </summary>
        public static decimal? CalcularVariacion(decimal actual, decimal anterior)
        {
            return anterior == 0 ? null : Math.Round((actual - anterior) * 100m / anterior, 1);
        }

        // Los aranceles base viven en TurnoBLL; se pasan a los SPs para no duplicarlos en SQL
        private static decimal ArancelEspecialidad => TurnoBLL.ObtenerArancelBase("Especialidad");
        private static decimal ArancelEmergencia => TurnoBLL.ObtenerArancelBase("Emergencia");

        private static void ValidarRango(DateTime fechaDesde, DateTime fechaHasta)
        {
            if (fechaDesde.Date > fechaHasta.Date)
            {
                throw new ArgumentException("La fecha inicial ('Desde') no puede ser posterior a la fecha final ('Hasta').");
            }
        }
    }
}
