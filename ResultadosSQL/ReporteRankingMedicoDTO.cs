using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO que representa una posición en el ranking de médicos según consultas efectivamente atendidas devuelto por <c>sp_ReporteGerencialRankingMedicos</c>.
    /// </summary>
    public class ReporteRankingMedicoDTO
    {
        public long Posicion { get; set; }
        public int IdUsuario { get; set; }
        public string NombreMedico { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public int AtencionesRealizadas { get; set; }
        public decimal TotalRecaudado { get; set; }
        public decimal PorcentajeParticipacion { get; set; }
    }
}
