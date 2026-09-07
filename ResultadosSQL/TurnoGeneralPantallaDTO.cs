using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// DTO para la grilla general de la pantalla pública de sala de espera (FrmUsuarioVentana).
    /// Mapea el resultado de sp_ListarTurnosGeneralesPantalla.
    /// </summary>
    public class TurnoGeneralPantallaDTO
    {
        public string Turno { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
    }
}
