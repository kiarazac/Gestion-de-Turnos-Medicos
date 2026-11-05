using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) de un turno de especialidad que quedó sin profesional disponible
    /// y debe ser cancelado/reprogramado, retornado por <c>sp_ListarTurnosAReprogramar</c>.
    /// </summary>
    public class TurnoAReprogramarDTO
    {
        /// <summary>Identificador único del turno.</summary>
        public int IdTurno { get; set; }

        /// <summary>Número de orden del turno (ej. 'C-001').</summary>
        public string? NroOrden { get; set; }

        /// <summary>Fecha programada del turno.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Horario programado (ej. '10:30').</summary>
        public string? Horario { get; set; }

        /// <summary>Nombre de la especialidad.</summary>
        public string? Especialidad { get; set; }

        /// <summary>Paciente en formato 'Apellido, Nombre'.</summary>
        public string? Paciente { get; set; }

        /// <summary>DNI del paciente.</summary>
        public string? Dni { get; set; }

        /// <summary>Teléfono de contacto del paciente.</summary>
        public string? Telefono { get; set; }

        /// <summary>Obra social del paciente.</summary>
        public string? ObraSocial { get; set; }

        /// <summary>Próximo horario libre sugerido para reprogramar (ej. '10/11/2026 09:30 hs'), o null si no hay en 30 días.</summary>
        public string? ProximoHorarioLibre { get; set; }
    }
}
