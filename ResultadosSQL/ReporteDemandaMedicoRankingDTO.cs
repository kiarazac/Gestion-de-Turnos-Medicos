using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO que representa el ranking de demanda de los profesionales médicos devuelto por <c>sp_ReporteGerencialDemandaMedicos</c>.
    /// </summary>
    public class ReporteDemandaMedicoRankingDTO
    {
        public int IdUsuario { get; set; }
        public string NombreMedico { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public int TotalTurnosAsignados { get; set; }
        public int TurnosAtendidos { get; set; }
        public int TurnosEnEspera { get; set; }
        public decimal TasaResolucion { get; set; }
        public string CategoriaDemanda { get; set; } = string.Empty;
    }
}
