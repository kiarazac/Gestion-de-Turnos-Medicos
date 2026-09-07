using System;
using System.Collections.Generic;
using System.Text;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    // Esta clase (DTO - Data Transfer Object) sirve exclusivamente para transportar datos desde la base de datos hacia la aplicación, evitando exponer el modelo real[cite: 2].
    // Mapea exactamente las columnas combinadas que devuelve el procedimiento almacenado sp_ObtenerSalas[cite: 2].
    public class SalaDTO
    {
        public int IdSala { get; set; }
        public string NombreSala { get; set; } = string.Empty;
        public string EstadoSala { get; set; } = string.Empty;
        public int? IdUsuario { get; set; }
        public string? NombreMedico { get; set; }
        public string? ApellidoMedico { get; set; }
        public string? DescripcionAtencion { get; set; }
    }
}