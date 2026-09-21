namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta el listado completo de usuarios
    /// devuelto por el procedimiento almacenado <c>sp_ListarUsuarios</c> para poblar la grilla de administración.
    /// </summary>
    public class UsuarioListadoDTO
    {
        /// <summary>Identificador único del usuario.</summary>
        public int IdUsuario { get; set; }

        /// <summary>Nombre(s) del usuario.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Apellido(s) del usuario.</summary>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>Correo electrónico de acceso.</summary>
        public string Correo { get; set; } = string.Empty;

        /// <summary>Documento Nacional de Identidad.</summary>
        public string Dni { get; set; } = string.Empty;

        /// <summary>Teléfono de contacto.</summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>Descripción del rol de acceso asignado.</summary>
        public string Rol { get; set; } = string.Empty;

        /// <summary>Matrícula médica profesional (si aplica).</summary>
        public string? NroMatricula { get; set; }

        /// <summary>Cadena concatenada con las especialidades médicas asignadas (ej. 'Pediatría, Traumatología').</summary>
        public string? Especialidades { get; set; }

        /// <summary>Cadena concatenada con las salas o consultorios asignados (ej. 'Consultorio 1, Box 2').</summary>
        public string? Salas { get; set; }

        /// <summary>Estado de actividad lógica del usuario (<c>true</c> si activo, <c>false</c> si dado de baja lógica).</summary>
        public bool Activo { get; set; } = true;
    }
}
