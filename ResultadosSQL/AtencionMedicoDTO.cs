using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que representa una consulta médica realizada
    /// por un profesional de la salud, devuelta por el procedimiento almacenado <c>sp_ObtenerAtencionesPorMedico</c>.
    /// </summary>
    public class AtencionMedicoDTO
    {
        /// <summary>Identificador del registro de historia clínica.</summary>
        public int IdHistoria { get; set; }

        /// <summary>Fecha y hora en que se efectuó la atención.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Modalidad o tipo de atención ('Emergencia' o 'Consulta').</summary>
        public string TipoTurno { get; set; } = string.Empty;

        /// <summary>Diagnóstico rápido o presuntivo determinado por el profesional.</summary>
        public string DiagRapido { get; set; } = string.Empty;

        /// <summary>Descripción detallada de la anamnesis y evolución clínica.</summary>
        public string DescripHistoriaClinica { get; set; } = string.Empty;

        /// <summary>Prescripción farmacológica e indicaciones terapéuticas.</summary>
        public string RecetaMedicamentos { get; set; } = string.Empty;

        /// <summary>Identificador del paciente atendido.</summary>
        public int IdPaciente { get; set; }

        /// <summary>Nombre del paciente atendido.</summary>
        public string NombrePaciente { get; set; } = string.Empty;

        /// <summary>Apellido del paciente atendido.</summary>
        public string ApellidoPaciente { get; set; } = string.Empty;

        /// <summary>Documento Nacional de Identidad del paciente.</summary>
        public string DniPaciente { get; set; } = string.Empty;

        /// <summary>Obra social o cobertura médica del paciente.</summary>
        public string ObraSocial { get; set; } = string.Empty;

        /// <summary>Código alfanumérico visible de orden del turno (ej. 'E-001', 'C-010').</summary>
        public string NroOrden { get; set; } = string.Empty;

        /// <summary>Nombre o denominación del consultorio o sala de atención.</summary>
        public string NombreSala { get; set; } = string.Empty;

        /// <summary>Especialidad médica bajo la que se realizó la atención.</summary>
        public string Especialidad { get; set; } = string.Empty;

        /// <summary>Nombre completo del paciente para visualización en grillas y reportes.</summary>
        public string PacienteCompleto => $"{ApellidoPaciente}, {NombrePaciente}".Trim(' ', ',');
    }
}
