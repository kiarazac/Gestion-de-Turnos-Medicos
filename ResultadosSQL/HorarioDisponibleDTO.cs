using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations; // Necesario para [Keyless]

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    // Mapea la lista de horarios libres devueltos por la base de datos.
    public class HorarioDisponibleDTO
    {
        public string? Horario { get; set; }

        public static implicit operator string?(HorarioDisponibleDTO? dto) => dto?.Horario;
        public override string ToString() => Horario ?? string.Empty;
    }
}

