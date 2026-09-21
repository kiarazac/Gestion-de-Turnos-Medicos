namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) para la grilla general de visualización pública
    /// en la pantalla de sala de espera (<c>FrmUsuarioVentana</c>), obtenido mediante <c>sp_ListarTurnosGeneralesPantalla</c>.
    /// </summary>
    public class TurnoGeneralPantallaDTO
    {
        /// <summary>Código identificador del turno.</summary>
        public string Turno { get; set; } = string.Empty;

        /// <summary>Hora asignada para la atención.</summary>
        public string Hora { get; set; } = string.Empty;

        /// <summary>Fecha de la consulta.</summary>
        public string Fecha { get; set; } = string.Empty;

        /// <summary>Especialidad médica de atención.</summary>
        public string Especialidad { get; set; } = string.Empty;

        /// <summary>Estado actual del turno ('En Espera', 'En Atencion').</summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>Sala o box de atención designado.</summary>
        public string Sala { get; set; } = string.Empty;
    }
}
