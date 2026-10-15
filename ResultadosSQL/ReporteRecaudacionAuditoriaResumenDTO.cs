using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO que encapsula los indicadores consolidados de la auditoría de recaudación por emisión de turnos devueltos por <c>sp_ReporteGerencialRecaudacionAuditoriaResumen</c>.
    /// </summary>
    public class ReporteRecaudacionAuditoriaResumenDTO
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int TotalTurnos { get; set; }
        public decimal TotalRecaudado { get; set; }

        public int TurnosEmergencia { get; set; }
        public decimal MontoEmergencia { get; set; }

        public int TurnosEspecialidad { get; set; }
        public decimal MontoEspecialidad { get; set; }

        public int TurnosObraSocial { get; set; }
        public decimal MontoObraSocial { get; set; }

        public int TurnosParticulares { get; set; }
        public decimal MontoParticulares { get; set; }
    }
}
