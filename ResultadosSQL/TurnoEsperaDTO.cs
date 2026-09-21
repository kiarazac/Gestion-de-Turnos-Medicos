using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que modela la cola ordenada de turnos en espera
    /// devuelta por el procedimiento almacenado <c>sp_ObtenerTurnosEnEspera</c>.
    /// </summary>
    public class TurnoEsperaDTO
    {
        /// <summary>Identificador del turno.</summary>
        public int IdTurno { get; set; }

        /// <summary>Número de orden visible.</summary>
        public string NroOrden { get; set; }

        /// <summary>Estado del turno ('En Espera').</summary>
        public string Estado { get; set; }

        /// <summary>Tipo de turno ('Emergencia' o 'Especialidad').</summary>
        public string TipoTurno { get; set; }

        /// <summary>Fecha asignada al turno.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Identificador de prioridad de triage.</summary>
        public int IdPrioridad { get; set; }

        /// <summary>Identificador del paciente.</summary>
        public int IdPaciente { get; set; }

        /// <summary>Identificador de la especialidad.</summary>
        public int IdEspecialidad { get; set; }
    }
}
