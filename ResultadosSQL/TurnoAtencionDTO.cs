using System;
using Microsoft.EntityFrameworkCore;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que representa un turno llamado o en proceso de atención
    /// devuelto por el procedimiento almacenado <c>sp_ListarTurnosAtencion</c>.
    /// </summary>
    [Keyless]
    public class TurnoAtencionDTO
    {
        /// <summary>Identificador del turno.</summary>
        public int IdTurno { get; set; }

        /// <summary>Código alfanumérico visible de orden (ej. 'E-001').</summary>
        public string NroOrden { get; set; }

        /// <summary>Fecha asignada al turno.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Estado operativo actual del turno.</summary>
        public string Estado { get; set; }

        /// <summary>Nombre de la especialidad médica.</summary>
        public string Especialidad { get; set; }

        /// <summary>Clasificación de triage o prioridad de atención.</summary>
        public string Triage { get; set; }

        /// <summary>Nombre de pila del paciente.</summary>
        public string NombrePaciente { get; set; }

        /// <summary>Apellido del paciente.</summary>
        public string ApellidoPaciente { get; set; }

        /// <summary>DNI del paciente.</summary>
        public string DniPaciente { get; set; }

        /// <summary>Cobertura médica u obra social del paciente.</summary>
        public string ObraSocial { get; set; }

        /// <summary>Nombre del consultorio o sala asignada para la consulta.</summary>
        public string NombreSala { get; set; } = string.Empty;
    }
}
