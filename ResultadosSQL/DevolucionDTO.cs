using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Devolución de dinero de un turno programado cancelado sin reprogramar, retornada por <c>sp_ListarDevoluciones</c>.
    /// </summary>
    public class DevolucionDTO
    {
        public int IdDevolucion { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string NroOrden { get; set; } = string.Empty;
        public DateTime FechaTurno { get; set; }
        public string Especialidad { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string ObraSocial { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public bool Revisada { get; set; }
        public DateTime? FechaRevision { get; set; }
        public string RevisadaPor { get; set; } = string.Empty;
    }
}
