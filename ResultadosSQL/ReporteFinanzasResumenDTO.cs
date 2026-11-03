using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO con los totales de recaudación del período (KPIs del Gerente) devuelto por <c>sp_ReporteGerencialFinanzasResumen</c>.
    /// </summary>
    public class ReporteFinanzasResumenDTO
    {
        public int TotalTurnos { get; set; }
        public decimal CobradoCaja { get; set; }
        public decimal ACobrarObraSocial { get; set; }
        public decimal TotalFacturado { get; set; }
        public decimal TicketPromedio { get; set; }
        public int TurnosAtendidos { get; set; }
        public int TurnosPendientes { get; set; }
        public int TurnosAusentes { get; set; }
        public decimal FacturadoAusentes { get; set; }
    }
}
