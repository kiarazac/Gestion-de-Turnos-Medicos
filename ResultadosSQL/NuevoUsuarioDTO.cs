using Microsoft.EntityFrameworkCore;

namespace ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que captura el identificador autogenerado por <c>SCOPE_IDENTITY()</c>
    /// al dar de alta un usuario mediante el procedimiento almacenado <c>sp_InsertarUsuario</c>.
    /// </summary>
    [Keyless]
    public class NuevoUsuarioIdDTO
    {
        /// <summary>Identificador autoincremental generado para el nuevo usuario en la tabla <c>Usuarios</c>.</summary>
        public int IdNuevoUsuario { get; set; }
    }
}