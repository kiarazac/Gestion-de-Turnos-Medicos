using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO que encapsula la facturación acumulada e ingresos por profesional médico devuelto por <c>sp_ReporteGerencialIngresosPorMedico</c>.
    /// </summary>
    public class ReporteIngresoMedicoDTO
    {
        public int IdUsuario { get; set; }
        public string NombreMedico { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public int ConsultasAtendidas { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal TicketPromedio { get; set; }
        public decimal PorcentajeAporte { get; set; }
    }
}
