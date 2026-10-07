using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO que representa la comparativa de recaudación entre particulares y obras sociales devuelto por <c>sp_ReporteGerencialObrasSocialesVsParticulares</c>.
    /// </summary>
    public class ReporteObraSocialVsParticularDTO
    {
        public long IdFila { get; set; }
        public string NombreCobertura { get; set; } = string.Empty;
        public string TipoCobertura { get; set; } = string.Empty;
        public int CantidadTurnos { get; set; }
        public decimal TotalRecaudado { get; set; }
        public decimal PorcentajeFacturacion { get; set; }
        public decimal TicketPromedio { get; set; }
    }
}
