namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta la información de especialidades médicas
    /// retornadas por procedimientos como <c>sp_ListarEspecialidades</c> y <c>sp_ObtenerEspecialidadesPorMedico</c>.
    /// </summary>
    public class EspecialidadDTO
    {
        /// <summary>Identificador único de la especialidad.</summary>
        public int IdEspecialidad { get; set; }

        /// <summary>Nombre descriptivo de la especialidad.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Estado de actividad lógica de la especialidad.</summary>
        public bool Activo { get; set; } = true;
    }
}