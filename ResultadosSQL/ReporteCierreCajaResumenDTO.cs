using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO que encapsula los indicadores consolidados del cierre diario de caja devueltos por <c>sp_ReporteCierreCajaDiario</c>.
    /// </summary>
    public class ReporteCierreCajaResumenDTO
    {
        public DateTime FechaCaja { get; set; }
        public int TotalTurnos { get; set; }

        /// <summary>Dinero que ingresó a la caja por los turnos emitidos (sin restar devoluciones).</summary>
        public decimal TotalCobrado { get; set; }

        /// <summary>Dinero devuelto ese día (en positivo); puede corresponder a turnos cobrados otro día.</summary>
        public decimal TotalDevuelto { get; set; }
        public int CantidadDevoluciones { get; set; }

        /// <summary>Saldo neto de la caja: cobrado menos devuelto. Puede ser negativo.</summary>
        public decimal TotalRecaudado { get; set; }

        public int TurnosParticulares { get; set; }
        public decimal MontoParticulares { get; set; }

        public int TurnosObraSocial { get; set; }
        public decimal MontoObraSocial { get; set; }

        public int TurnosEmergencia { get; set; }
        public decimal MontoEmergencia { get; set; }

        public int TurnosEspecialidad { get; set; }
        public decimal MontoEspecialidad { get; set; }
    }
}
