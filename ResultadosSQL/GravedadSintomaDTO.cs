namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta el nivel de severidad de un síntoma
    /// retornado por el procedimiento almacenado <c>sp_ObtenerGravedadSintoma</c>.
    /// </summary>
    public class GravedadSintomaDTO
    {
        /// <summary>Descripción textual del nivel de gravedad del síntoma ('Alta', 'Media', 'Baja').</summary>
        public string Gravedad { get; set; }
    }
}
