using Microsoft.EntityFrameworkCore;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que captura el identificador autogenerado de un turno creado
    /// devuelto por los procedimientos de emisión de turnos.
    /// </summary>
    [Keyless]
    public class NuevoTurnoIdDTO
    {
        /// <summary>Identificador autoincremental generado para el nuevo turno.</summary>
        public int IdNuevoTurno { get; set; }
    }
}
