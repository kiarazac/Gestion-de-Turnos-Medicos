using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO que encapsula los diagnósticos y síntomas frecuentes del médico devueltos por <c>sp_ReporteMedico_RankingDiagnosticosYSintomas</c>.
    /// </summary>
    public class ReporteMedicoDiagnosticoFrecuenteDTO
    {
        public string Concepto { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int CantidadCasos { get; set; }
        public decimal Porcentaje { get; set; }
    }
}
