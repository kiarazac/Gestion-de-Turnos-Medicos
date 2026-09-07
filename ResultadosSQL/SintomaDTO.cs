using System;
using System.Collections.Generic;
using System.Text;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    // Retorno del procedimiento sp_ObtenerSintomas. Sirve para cargar el triage dinámico en el formulario de recepción de emergencias (FrmTurnoEmergencia)[cite: 2].
    public class SintomaDTO
    {
        public int IdSintoma { get; set; }
        public string Descripcion { get; set; }
        // La gravedad determina la prioridad automática en la cola FIFO del sistema[cite: 1].
        public string Gravedad { get; set; }
    }
}
