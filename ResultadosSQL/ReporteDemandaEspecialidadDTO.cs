using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que consolida métricas gerenciales de demanda y resolución
    /// de turnos médicos por especialidad, obtenido mediante el procedimiento almacenado <c>sp_ReporteDemandaEspecialidades</c>.
    /// </summary>
    public class ReporteDemandaEspecialidadDTO
    {
        /// <summary>
        /// Identificador único de la especialidad médica.
        /// </summary>
        public int IdEspecialidad { get; set; }

        /// <summary>
        /// Nombre de la especialidad médica.
        /// </summary>
        public string Especialidad { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad total de turnos registrados en el período evaluado para la especialidad.
        /// </summary>
        public int TotalTurnos { get; set; }

        /// <summary>
        /// Cantidad de turnos finalizados efectivamente en estado 'Atendido'.
        /// </summary>
        public int TurnosAtendidos { get; set; }

        /// <summary>
        /// Cantidad de turnos que fueron cancelados ('Cancelado').
        /// </summary>
        public int TurnosCancelados { get; set; }

        /// <summary>
        /// Cantidad de turnos actualmente pendientes o en proceso ('En Espera', 'Llamado', 'En Consulta').
        /// </summary>
        public int TurnosEnEspera { get; set; }

        /// <summary>
        /// Porcentaje de cumplimiento o atención efectiva (TurnosAtendidos / TotalTurnos * 100).
        /// </summary>
        public decimal PorcentajeAtencion { get; set; }
    }
}
