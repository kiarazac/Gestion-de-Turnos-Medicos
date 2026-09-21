using Microsoft.EntityFrameworkCore;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que captura el identificador autogenerado de una sala recién creada
    /// devuelto por el procedimiento almacenado <c>sp_InsertarSala</c>.
    /// </summary>
    [Keyless]
    public class NuevaSalaIdDTO
    {
        /// <summary>Identificador autoincremental generado para la nueva sala.</summary>
        public int IdNuevaSala { get; set; }
    }
}
