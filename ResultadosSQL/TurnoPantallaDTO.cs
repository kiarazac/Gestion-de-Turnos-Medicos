using System;
using System.Collections.Generic;
using System.Text;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    // Mapea los datos públicos para el monitor de la sala de espera.
    public class TurnoPantallaDTO
    {
        public string NroOrden { get; set; }
        public string NombreSala { get; set; }
        public string MedicoApellido { get; set; }
    }
}
