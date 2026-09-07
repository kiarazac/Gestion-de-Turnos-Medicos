using System;
using System.Collections.Generic;
using System.Text;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    // Mapea el registro histórico de atenciones de un paciente.
    public class HistoriaClinicaDTO
    {
        public int IdHistoria { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoTurno { get; set; }
        public string DiagRapido { get; set; }
        public string DescripHistoriaClinica { get; set; }
        public string RecetaMedicamentos { get; set; }
        public int IdPaciente { get; set; }
        public int IdTurno { get; set; }
        public int IdUsuario { get; set; }
        public string ApellidoMedico { get; set; }
        public string NombreMedico { get; set; }
    }
}
