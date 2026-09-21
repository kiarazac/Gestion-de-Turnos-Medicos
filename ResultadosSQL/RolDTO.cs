namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que representa un perfil o rol de acceso al sistema
    /// retornado por <c>sp_ListarRoles</c> para poblar selectores desplegables.
    /// </summary>
    public class RolDTO
    {
        /// <summary>Identificador del rol.</summary>
        public int IdRol { get; set; }

        /// <summary>Descripción o nombre del rol (ej. 'Administrador', 'Personal Médico', 'Recepcionista').</summary>
        public string Descripcion { get; set; }
    }
}
