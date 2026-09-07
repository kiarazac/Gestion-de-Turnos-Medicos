using System;
using System.Collections.Generic;
using System.Text;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    // Mapea los resultados de sp_ObtenerListaTurnos y sp_ListarTurnosEspecialidad.
    public class TurnoListadoDTO
    {
        public int IdTurno { get; set; }
        public string NroOrden { get; set; }
        public string Estado { get; set; }
        public string TipoTurno { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreEspecialidad { get; set; }
        public string PrioridadTexto { get; set; }
        public int? IdPrioridad { get; set; }
    }
}
