using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO que representa una fila del detalle de turnos y montos cobrados en caja devuelto por <c>sp_ReporteCierreCajaDetalle</c>.
    /// </summary>
    public class ReporteCierreCajaDetalleDTO
    {
        public int IdTurno { get; set; }
        public string NroOrden { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan Horario { get; set; }
        public string PacienteCompleto { get; set; } = string.Empty;
        public string DniPaciente { get; set; } = string.Empty;
        public string ObraSocial { get; set; } = string.Empty;
        public bool EsParticular { get; set; }
        public string TipoTurno { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public decimal MontoCobrado { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string MedicoAsignado { get; set; } = string.Empty;
    }
}
