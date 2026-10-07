using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) para representar la distribución y ranking
    /// de atenciones de emergencia atendidas por un profesional clínico, agrupadas por severidad de triage.
    /// Obtenido mediante el procedimiento almacenado <c>sp_ReporteMedico_RankingGravedadTriage</c>.
    /// </summary>
    public class ReporteMedicoGravedadDTO
    {
        /// <summary>Identificador de la prioridad (1: Alta, 2: Media, 3: Baja).</summary>
        public int IdPrioridad { get; set; }

        /// <summary>Nombre descriptivo de la prioridad ('Alta', 'Media', 'Baja').</summary>
        public string Gravedad { get; set; } = string.Empty;

        /// <summary>Cantidad de turnos de emergencia atendidos correspondientes a este nivel.</summary>
        public int CantidadTurnos { get; set; }

        /// <summary>Porcentaje de participación sobre el total de atenciones de guardia del médico.</summary>
        public decimal Porcentaje { get; set; }
    }
}
