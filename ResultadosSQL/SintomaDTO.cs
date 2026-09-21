namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que representa un síntoma clínico y su severidad de triage
    /// retornado por el procedimiento almacenado <c>sp_ObtenerSintomas</c>.
    /// </summary>
    public class SintomaDTO
    {
        /// <summary>Identificador único del síntoma.</summary>
        public int IdSintoma { get; set; }

        /// <summary>Descripción clínica o manifestación del síntoma.</summary>
        public string Descripcion { get; set; }

        /// <summary>Gravedad asociada al síntoma ('Alta', 'Media', 'Baja') determinante de la prioridad de atención.</summary>
        public string Gravedad { get; set; }
    }
}
