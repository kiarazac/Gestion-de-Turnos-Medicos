using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que representa un día de ausencia programada de un médico
    /// retornado por <c>sp_ListarAusenciasPorMedico</c>.
    /// </summary>
    public class AusenciaMedicaDTO
    {
        /// <summary>Identificador único de la ausencia.</summary>
        public int IdAusencia { get; set; }

        /// <summary>Día de ausencia.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Motivo opcional informado por el médico.</summary>
        public string? Motivo { get; set; }
    }

    /// <summary>
    /// Resultado de <c>sp_RegistrarAusencias</c>: días efectivamente registrados y turnos que quedaron sin profesional.
    /// </summary>
    public class ResultadoAusenciaDTO
    {
        /// <summary>Cantidad de días nuevos registrados (se omiten los ya cargados).</summary>
        public int DiasRegistrados { get; set; }

        /// <summary>Cantidad de turnos reservados que quedaron sin profesional disponible y deben reprogramarse.</summary>
        public int TurnosSinCobertura { get; set; }
    }
}
