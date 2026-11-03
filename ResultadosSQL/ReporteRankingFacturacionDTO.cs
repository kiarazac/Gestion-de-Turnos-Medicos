using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO compartido por los rankings de facturación del Gerente: médicos (<c>sp_ReporteGerencialRankingMedicos</c>),
    /// especialidades (<c>sp_ReporteGerencialRankingEspecialidades</c>), obras sociales (<c>sp_ReporteGerencialRankingObrasSociales</c>)
    /// y modalidad / cobertura (<c>sp_ReporteGerencialFacturacionPorSegmento</c>). Las columnas que no aplican vienen en null.
    /// </summary>
    public class ReporteRankingFacturacionDTO
    {
        public long Posicion { get; set; }
        public int? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Detalle { get; set; }
        public string? Matricula { get; set; }
        public int CantidadTurnos { get; set; }
        public decimal CobradoCaja { get; set; }
        public decimal ACobrarObraSocial { get; set; }
        public decimal TotalFacturado { get; set; }
        public decimal TicketPromedio { get; set; }
        public decimal PorcentajeParticipacion { get; set; }
        public decimal? PorcentajeCoberturaVigente { get; set; }
        public decimal? PorcentajeCoberturaAplicada { get; set; }

        /// <summary>
        /// Indica que la cobertura aplicada en caja difiere de la vigente (el % se editó en el período o se cobró distinto a lo configurado).
        /// </summary>
        public bool CoberturaConDiferencia =>
            PorcentajeCoberturaVigente.HasValue && PorcentajeCoberturaAplicada.HasValue &&
            Math.Abs(PorcentajeCoberturaVigente.Value - PorcentajeCoberturaAplicada.Value) >= 0.5m;
    }
}
