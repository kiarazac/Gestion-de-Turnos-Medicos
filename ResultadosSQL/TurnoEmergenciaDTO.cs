using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta el listado de turnos de guardia
    /// retornado por el procedimiento almacenado <c>sp_ListarTurnosEmergencia</c>.
    /// </summary>
    public class TurnoEmergenciaDTO
    {
        /// <summary>Identificador del turno.</summary>
        public int IdTurno { get; set; }

        /// <summary>Número de orden visible.</summary>
        public string? NroOrden { get; set; }

        /// <summary>Código descriptivo de turno.</summary>
        public string? Turno { get; set; }

        /// <summary>Prioridad de atención textual ('Alta', 'Media', 'Baja').</summary>
        public string? Prioridad { get; set; }

        /// <summary>Hora programada o de admisión.</summary>
        public string? Hora { get; set; }

        /// <summary>Estado operativo del turno.</summary>
        public string? Estado { get; set; }

        /// <summary>Denominación de la sala asignada.</summary>
        public string? Sala { get; set; }

        /// <summary>Tipo de turno ('Emergencia').</summary>
        public string? TipoTurno { get; set; }

        /// <summary>Fecha del turno.</summary>
        public DateTime? Fecha { get; set; }

        /// <summary>Identificador foráneo de prioridad.</summary>
        public int? IdPrioridad { get; set; }

        /// <summary>Identificador foráneo de paciente.</summary>
        public int? IdPaciente { get; set; }

        /// <summary>Identificador foráneo de especialidad.</summary>
        public int? IdEspecialidad { get; set; }

        /// <summary>Nombre y apellido del paciente.</summary>
        public string? Paciente { get; set; }

        /// <summary>Edad del paciente (si aplica).</summary>
        public int? Edad { get; set; }

        /// <summary>Clasificación de triage.</summary>
        public string? Triage { get; set; }
    }
}
