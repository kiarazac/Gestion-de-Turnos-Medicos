namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta el identificador y nombre completo de un médico
    /// retornado por <c>sp_ListarPersonalMedico</c> para poblar selectores y asignaciones.
    /// </summary>
    public class MedicoDTO
    {
        /// <summary>Identificador del usuario con rol de Personal Médico.</summary>
        public int IdUsuario { get; set; }

        /// <summary>Nombre completo concatenado del médico (ej. 'Dr. Pérez, Juan').</summary>
        public string NombreCompleto { get; set; }
    }
}
