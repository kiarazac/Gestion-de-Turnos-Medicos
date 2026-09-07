using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations; // Necesario para [Keyless]

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    public class TurnoAtencionDTO
    {
        public int IdTurno { get; set; }
        public string NroOrden { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public string Especialidad { get; set; }
        public string Triage { get; set; }
        public string NombrePaciente { get; set; }
        public string ApellidoPaciente { get; set; }
        public string DniPaciente { get; set; }
        public string ObraSocial { get; set; }
    }
}
