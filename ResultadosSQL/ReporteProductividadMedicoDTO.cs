using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que consolida métricas gerenciales de productividad
    /// profesional médica, devuelto por el procedimiento almacenado <c>sp_ReporteProductividadMedicos</c>.
    /// </summary>
    public class ReporteProductividadMedicoDTO
    {
        /// <summary>
        /// Identificador único del usuario médico en el sistema.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Nombres del profesional de la salud.
        /// </summary>
        public string NombreMedico { get; set; } = string.Empty;

        /// <summary>
        /// Apellidos del profesional de la salud.
        /// </summary>
        public string ApellidoMedico { get; set; } = string.Empty;

        /// <summary>
        /// Matrícula habilitante del médico.
        /// </summary>
        public string Matricula { get; set; } = string.Empty;

        /// <summary>
        /// Denominación de la especialidad asociada al profesional.
        /// </summary>
        public string Especialidad { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad total de consultas clínicas efectivamente atendidas por el profesional en el período.
        /// </summary>
        public int ConsultasAtendidas { get; set; }

        /// <summary>
        /// Cantidad de pacientes distintos o únicos que recibieron atención médica por parte del profesional.
        /// </summary>
        public int PacientesUnicos { get; set; }

        /// <summary>
        /// Nombre completo formateado del médico para representación en grillas y encabezados de reporte.
        /// </summary>
        public string MedicoCompleto => $"{ApellidoMedico}, {NombreMedico}".Trim(' ', ',');
    }
}
