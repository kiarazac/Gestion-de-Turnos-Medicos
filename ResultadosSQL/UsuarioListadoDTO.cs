using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO para transportar los datos del listado de usuarios activos devueltos por sp_ListarUsuarios.
    /// </summary>
    public class UsuarioListadoDTO
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string? NroMatricula { get; set; }
        public string? Especialidades { get; set; }
        public string? Salas { get; set; }
        public bool Activo { get; set; } = true;
    }
}
