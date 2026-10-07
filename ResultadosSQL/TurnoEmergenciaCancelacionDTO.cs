namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta el detalle de un turno activo
    /// de guardia/emergencia retornado por <c>sp_BuscarTurnoActivoEmergencia</c> para su cancelación ágil con 2FA.
    /// </summary>
    public class TurnoEmergenciaCancelacionDTO
    {
        /// <summary>Identificador primario del turno médico.</summary>
        public int IdTurno { get; set; }

        /// <summary>Número de orden visible del turno (ej: 'E-001').</summary>
        public string NroOrden { get; set; } = string.Empty;

        /// <summary>Apellido del paciente.</summary>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>Nombre del paciente.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Documento Nacional de Identidad del paciente.</summary>
        public string Dni { get; set; } = string.Empty;

        /// <summary>Nivel de prioridad o triage asignado ('ALTA', 'MEDIA', 'BAJA').</summary>
        public string Prioridad { get; set; } = string.Empty;

        /// <summary>Hora de emisión del turno (formato HH:mm).</summary>
        public string Hora { get; set; } = string.Empty;

        /// <summary>Estado operativo actual del turno ('En Espera', 'Llamado').</summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>Código o palabra clave alfanumérica de seguridad 2FA emitida en el ticket.</summary>
        public string CodigoCancelacion { get; set; } = string.Empty;

        /// <summary>Nombre completo concatenado para facilitar su visualización en controles de interfaz.</summary>
        public string PacienteCompleto => $"{Apellido}, {Nombre}";
    }
}
