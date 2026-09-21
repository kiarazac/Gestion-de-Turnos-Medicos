namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta el contexto del usuario autenticado
    /// devuelto por el procedimiento almacenado <c>sp_ValidarLogin</c>.
    /// </summary>
    public class UsuarioLoginResult
    {
        /// <summary>Identificador único del usuario.</summary>
        public int IdUsuario { get; set; }

        /// <summary>Nombre(s) del usuario.</summary>
        public string Nombre { get; set; }

        /// <summary>Apellido(s) del usuario.</summary>
        public string Apellido { get; set; }

        /// <summary>Correo electrónico de acceso.</summary>
        public string Correo { get; set; }

        /// <summary>Identificador del rol funcional asignado.</summary>
        public int IdRol { get; set; }

        /// <summary>Nombre o descripción del rol asignado ('Administrador', 'Personal Médico', 'Recepcionista').</summary>
        public string NombreRol { get; set; }
    }
}
