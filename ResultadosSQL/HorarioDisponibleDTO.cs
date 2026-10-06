namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta una franja horaria para consultas
    /// retornada por el procedimiento almacenado <c>sp_ObtenerHorariosConEstado</c>, indicando si se encuentra
    /// disponible u ocupada y los detalles del turno asignado.
    /// </summary>
    public class HorarioDisponibleDTO
    {
        /// <summary>Horario de consulta en formato string (ej. '08:30', '14:00').</summary>
        public string? Horario { get; set; }

        /// <summary>Indica si el horario está disponible para reserva (true) u ocupado (false).</summary>
        public bool EstaDisponible { get; set; } = true;

        /// <summary>Identificador del turno asignado si se encuentra ocupado.</summary>
        public int? IdTurno { get; set; }

        /// <summary>Número correlativo de orden del turno (ej. 'C-001').</summary>
        public string? NroOrden { get; set; }

        /// <summary>Nombre completo del paciente (Apellido, Nombre).</summary>
        public string? Paciente { get; set; }

        /// <summary>Documento Nacional de Identidad del paciente asignado.</summary>
        public string? Dni { get; set; }

        /// <summary>Obra social o cobertura médica del paciente asignado.</summary>
        public string? ObraSocial { get; set; }

        /// <summary>Estado operativo del turno (ej. 'En Espera').</summary>
        public string? Estado { get; set; }

        /// <summary>Palabra clave alfanumérica de 2FA para cancelación del turno.</summary>
        public string? CodigoCancelacion { get; set; }

        /// <summary>Texto descriptivo para mostrar en controles de interfaz de usuario.</summary>
        public string TextoDisplay => EstaDisponible
            ? $"{Horario}  (Disponible)"
            : $"{Horario}  [OCUPADO - {NroOrden}]";

        /// <summary>Conversión implícita de HorarioDisponibleDTO a string (retorna la hora limpia).</summary>
        public static implicit operator string?(HorarioDisponibleDTO? dto) => dto?.Horario;

        /// <summary>Devuelve la representación textual del horario para visualización en ComboBox.</summary>
        public override string ToString() => TextoDisplay;
    }
}
