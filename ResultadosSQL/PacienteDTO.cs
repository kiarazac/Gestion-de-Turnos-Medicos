namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que representa la información filiatoria y de cobertura de un paciente
    /// retornado por <c>sp_BuscarPacientePorDNI</c>.
    /// </summary>
    public class PacienteDTO
    {
        /// <summary>Identificador único del paciente.</summary>
        public int IdPaciente { get; set; }

        /// <summary>Nombre(s) del paciente.</summary>
        public string Nombre { get; set; }

        /// <summary>Apellido(s) del paciente.</summary>
        public string Apellido { get; set; }

        /// <summary>Documento Nacional de Identidad.</summary>
        public string DNI { get; set; }

        /// <summary>Obra social o cobertura médica prepaga.</summary>
        public string ObraSocial { get; set; }
    }
}
