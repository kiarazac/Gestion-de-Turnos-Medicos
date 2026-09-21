namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta el identificador autogenerado
    /// y el número de orden visible (ej. 'E-001', 'T-015') emitido para un nuevo turno médico.
    /// Utilizado en formularios de emisión como <c>FrmTurnoEmergencia</c> y <c>FrmTurnoEspecialidad</c>.
    /// </summary>
    public class ResultadoTurnoDTO
    {
        /// <summary>Identificador único autoincremental del turno insertado.</summary>
        public int IdNuevoTurno { get; set; }

        /// <summary>Código alfanumérico visible de orden para llamada en pantallas.</summary>
        public string NroOrden { get; set; } = string.Empty;
    }
}
