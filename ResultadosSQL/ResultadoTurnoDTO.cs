using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO que representa el identificador y el número de orden generado para un nuevo turno.
    /// Utilizado por FrmTurnoEmergencia y FrmTurnoEspecialidad.
    /// </summary>
    public class ResultadoTurnoDTO
    {
        public int IdNuevoTurno { get; set; }
        public string NroOrden { get; set; } = string.Empty;
    }
}
