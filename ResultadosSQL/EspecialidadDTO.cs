using System;
using System.Collections.Generic;
using System.Text;

namespace ResultadosSQL
{
    // DTO utilizado para transportar los datos de la especialidad desde la base de datos hacia las grillas o ComboBoxes de la UI[cite: 2].
    public class EspecialidadDTO
    {
        public int IdEspecialidad { get; set; }
        public string Nombre { get; set; }
    }
}