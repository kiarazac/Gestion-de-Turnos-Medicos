using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO con la recaudación de un día (o de un mes si <see cref="AgrupadoPorMes"/>) devuelto por <c>sp_ReporteGerencialRecaudacionDiaria</c>.
    /// </summary>
    public class ReporteRecaudacionDiariaDTO
    {
        public DateTime Fecha { get; set; }
        public bool AgrupadoPorMes { get; set; }
        public int CantidadTurnos { get; set; }
        public decimal CobradoCaja { get; set; }
        public decimal ACobrarObraSocial { get; set; }
        public decimal TotalFacturado { get; set; }
    }
}
