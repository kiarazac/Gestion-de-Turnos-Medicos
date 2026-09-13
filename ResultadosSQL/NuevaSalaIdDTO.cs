using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    [Keyless]
    public class NuevaSalaIdDTO
    {
        public int IdNuevaSala { get; set; }
    }
}
