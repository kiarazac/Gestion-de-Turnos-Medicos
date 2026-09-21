namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta una franja horaria vacante para consultas
    /// retornada por el procedimiento almacenado <c>sp_ObtenerHorariosDisponibles</c>.
    /// </summary>
    public class HorarioDisponibleDTO
    {
        /// <summary>Horario de consulta disponible en formato string (ej. '08:30', '14:00').</summary>
        public string? Horario { get; set; }

        /// <summary>Conversión implícita de HorarioDisponibleDTO a string.</summary>
        public static implicit operator string?(HorarioDisponibleDTO? dto) => dto?.Horario;

        /// <summary>Devuelve la representación textual del horario disponible.</summary>
        public override string ToString() => Horario ?? string.Empty;
    }
}
