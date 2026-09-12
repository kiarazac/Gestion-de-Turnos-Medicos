using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ResultadosSQL
{
    // Atrapa el resultado exacto de SCOPE_IDENTITY() devolviendo el IdNuevoUsuario.
    [Keyless]
    public class NuevoUsuarioIdDTO
    {
        public int IdNuevoUsuario { get; set; }
    }
}