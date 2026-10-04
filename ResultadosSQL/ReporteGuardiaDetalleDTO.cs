using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta el detalle individual de cada ingreso
    /// y atención de guardia médica devuelto por el procedimiento almacenado <c>sp_ReporteGuardiaTriage_Detalle</c>.
    /// </summary>
    public class ReporteGuardiaDetalleDTO
    {
        /// <summary>Identificador único del turno de guardia.</summary>
        public int IdTurno { get; set; }

        /// <summary>Código alfanumérico de orden de atención (ej. 'E-015').</summary>
        public string NroOrden { get; set; } = string.Empty;

        /// <summary>Fecha y hora de registro o ingreso a la guardia.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Nombre del paciente atendido.</summary>
        public string NombrePaciente { get; set; } = string.Empty;

        /// <summary>Apellido del paciente atendido.</summary>
        public string ApellidoPaciente { get; set; } = string.Empty;

        /// <summary>Número de Documento Nacional de Identidad del paciente.</summary>
        public string DniPaciente { get; set; } = string.Empty;

        /// <summary>Obra social o cobertura médica ('OSDE', 'Swiss Medical', 'PAMI', 'Particular', etc.).</summary>
        public string ObraSocial { get; set; } = string.Empty;

        /// <summary>Nivel de prioridad o triage asignado ('Alta', 'Media', 'Baja').</summary>
        public string Prioridad { get; set; } = string.Empty;

        /// <summary>Identificador numérico de prioridad (1 = Alta, 2 = Media, 3 = Baja).</summary>
        public int IdPrioridad { get; set; }

        /// <summary>Listado concatenado de síntomas manifestados por el paciente en el triage de guardia.</summary>
        public string Sintomas { get; set; } = string.Empty;

        /// <summary>Estado de la atención ('En Espera', 'Llamado', 'En Consulta', 'Atendido', 'Cancelado').</summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>Nombre del consultorio, box o sala asignada para la atención.</summary>
        public string NombreSala { get; set; } = string.Empty;

        /// <summary>Propiedad calculada para visualización del nombre completo del paciente.</summary>
        public string PacienteCompleto => $"{ApellidoPaciente}, {NombrePaciente}".Trim(' ', ',');
    }
}
