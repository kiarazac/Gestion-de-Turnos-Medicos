namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que representa una entidad de obra social o cobertura médica
    /// retornada por procedimientos almacenados como <c>sp_ListarObrasSociales</c>.
    /// </summary>
    public class ObraSocialDTO
    {
        /// <summary>Identificador único de la obra social.</summary>
        public int IdObraSocial { get; set; }

        /// <summary>Nombre descriptivo o denominación oficial de la obra social.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Sigla o acrónimo identificatorio (ej. IOSCOR, PAMI, OSDE).</summary>
        public string Sigla { get; set; } = string.Empty;

        /// <summary>Estado de actividad lógica de la cobertura médica.</summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Representación textual de la obra social para su visualización en listas y controles desplegables.
        /// </summary>
        public override string ToString() => Nombre;
    }
}
