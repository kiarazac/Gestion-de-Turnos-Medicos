using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta el ranking de motivos de consulta
    /// y síntomas registrados en el triage de guardia médica devuelto por <c>sp_ReporteGuardiaTriage_RankingSintomas</c>.
    /// </summary>
    public class ReporteGuardiaSintomaDTO
    {
        /// <summary>Identificador del síntoma.</summary>
        public int IdSintoma { get; set; }

        /// <summary>Denominación o descripción clínica del síntoma manifestado.</summary>
        public string Sintoma { get; set; } = string.Empty;

        /// <summary>Nivel de gravedad asociado al síntoma en catálogo ('Alta', 'Media', 'Baja').</summary>
        public string Gravedad { get; set; } = string.Empty;

        /// <summary>Número de pacientes que manifestaron este síntoma en el período analizado.</summary>
        public int CantidadCasos { get; set; }

        /// <summary>Participación porcentual del síntoma respecto al volumen total de síntomas computados.</summary>
        public decimal Porcentaje { get; set; }
    }
}
