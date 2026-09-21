using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que representa una evolución clínica de un paciente
    /// devuelta por el procedimiento almacenado <c>sp_ObtenerHistoriaClinicaPaciente</c>.
    /// </summary>
    public class HistoriaClinicaDTO
    {
        /// <summary>Identificador del registro de historia clínica.</summary>
        public int IdHistoria { get; set; }

        /// <summary>Fecha y hora de la atención médica.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Tipo de turno atendido ('Emergencia' o 'Especialidad').</summary>
        public string TipoTurno { get; set; }

        /// <summary>Diagnóstico rápido o preliminar emitido.</summary>
        public string DiagRapido { get; set; }

        /// <summary>Detalle de la anamnesis, síntomas y conclusiones de la atención.</summary>
        public string DescripHistoriaClinica { get; set; }

        /// <summary>Medicamentos y pautas terapéuticas recetadas.</summary>
        public string RecetaMedicamentos { get; set; }

        /// <summary>Identificador del paciente atendido.</summary>
        public int IdPaciente { get; set; }

        /// <summary>Identificador del turno asociado.</summary>
        public int IdTurno { get; set; }

        /// <summary>Identificador del profesional médico responsable.</summary>
        public int IdUsuario { get; set; }

        /// <summary>Apellido del médico tratante.</summary>
        public string ApellidoMedico { get; set; }

        /// <summary>Nombre del médico tratante.</summary>
        public string NombreMedico { get; set; }
    }
}
