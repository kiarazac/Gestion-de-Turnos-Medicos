using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations; // Necesario para [Keyless]

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    public class TurnoEmergenciaDTO
    {
        public int IdTurno { get; set; }
        public string? NroOrden { get; set; }
        public string? Turno { get; set; }
        public string? Prioridad { get; set; }
        public string? Hora { get; set; }
        public string? Estado { get; set; }
        public string? Sala { get; set; }
        public string? TipoTurno { get; set; }
        public DateTime? Fecha { get; set; }
        public int? IdPrioridad { get; set; }
        public int? IdPaciente { get; set; }
        public int? IdEspecialidad { get; set; }
        public string? Paciente { get; set; }
        public int? Edad { get; set; }
        public string? Triage { get; set; }
    }
}
