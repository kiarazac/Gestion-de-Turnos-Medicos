using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    [Keyless]
    public class NuevoTurnoIdDTO
    {
        public int IdNuevoTurno { get; set; }
    }
}
