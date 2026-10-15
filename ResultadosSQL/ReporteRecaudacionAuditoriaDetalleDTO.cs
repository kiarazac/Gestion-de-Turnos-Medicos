using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO que representa una fila del detalle auditado de turnos emitidos y cobrados por recepción devuelto por <c>sp_ReporteGerencialRecaudacionAuditoriaDetalle</c>.
    /// </summary>
    public class ReporteRecaudacionAuditoriaDetalleDTO
    {
        public int IdTurno { get; set; }
        public string NroOrden { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public string Paciente { get; set; } = string.Empty;
        public string DniPaciente { get; set; } = string.Empty;
        public string TipoTurno { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string ObraSocial { get; set; } = string.Empty;
        public string TipoCobertura { get; set; } = string.Empty;
        public decimal MontoCobrado { get; set; }
        public string EstadoTurno { get; set; } = string.Empty;
        public string MedicoAsignado { get; set; } = string.Empty;
    }
}
