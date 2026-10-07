using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) para representar la frecuencia y ranking
    /// de síntomas manifestados por pacientes de urgencia atendidos por el profesional clínico.
    /// Obtenido mediante el procedimiento almacenado <c>sp_ReporteMedico_RankingSintomasAtendidos</c>.
    /// </summary>
    public class ReporteMedicoSintomaDTO
    {
        /// <summary>Identificador del síntoma.</summary>
        public int IdSintoma { get; set; }

        /// <summary>Descripción clínica del síntoma manifestado.</summary>
        public string Sintoma { get; set; } = string.Empty;

        /// <summary>Nivel de gravedad asignado al síntoma ('Alta', 'Media', 'Baja').</summary>
        public string Gravedad { get; set; } = string.Empty;

        /// <summary>Cantidad de pacientes atendidos por el médico que presentaron este síntoma.</summary>
        public int CantidadCasos { get; set; }

        /// <summary>Porcentaje de frecuencia sobre el total de síntomas atendidos en guardia.</summary>
        public decimal Porcentaje { get; set; }
    }
}
