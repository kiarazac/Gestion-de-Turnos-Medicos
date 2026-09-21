namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) para la pantalla de avisos y llamados en sala de espera,
    /// retornado por el procedimiento almacenado <c>sp_ObtenerTurnosPantallaPublica</c>.
    /// </summary>
    public class TurnoPantallaDTO
    {
        /// <summary>Código alfanumérico visible de turno que debe dirigirse a la sala.</summary>
        public string NroOrden { get; set; }

        /// <summary>Denominación del consultorio o sala física de atención.</summary>
        public string NombreSala { get; set; }

        /// <summary>Apellido del profesional médico que realiza el llamado.</summary>
        public string MedicoApellido { get; set; }
    }
}
