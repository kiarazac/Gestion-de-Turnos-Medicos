namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta la información de salas y consultorios médicos
    /// junto con los profesionales asignados devueltos por el procedimiento almacenado <c>sp_ObtenerSalas</c>.
    /// </summary>
    public class SalaDTO
    {
        /// <summary>Identificador único de la sala.</summary>
        public int IdSala { get; set; }

        /// <summary>Nombre descriptivo o número del consultorio.</summary>
        public string NombreSala { get; set; } = string.Empty;

        /// <summary>Estado operativo actual de la sala ('Disponible', 'Ocupada', 'En Mantenimiento').</summary>
        public string EstadoSala { get; set; } = string.Empty;

        /// <summary>Identificador del médico que tiene asignada la sala (opcional).</summary>
        public int? IdUsuario { get; set; }

        /// <summary>Nombre de pila del médico asignado.</summary>
        public string? NombreMedico { get; set; }

        /// <summary>Apellido del médico asignado.</summary>
        public string? ApellidoMedico { get; set; }

        /// <summary>Notas u observaciones sobre la atención en el consultorio.</summary>
        public string? DescripcionAtencion { get; set; }

        /// <summary>Indica si la sala se encuentra activa lógicamente.</summary>
        public bool Activo { get; set; } = true;
    }
}