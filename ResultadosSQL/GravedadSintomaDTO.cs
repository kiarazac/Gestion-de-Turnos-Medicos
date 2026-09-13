using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    public class GravedadSintomaDTO
    {
        // Asumiendo que tenés la Gravedad como un entero (ej: 1=Alta, 2=Media, 3=Baja). 
        // Si en tu BD es un string (NVARCHAR), cambialo a public string Gravedad { get; set; }
        public string Gravedad { get; set; }
    }
}
