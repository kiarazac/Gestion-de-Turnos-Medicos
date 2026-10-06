using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que consolida los indicadores clave (KPIs),
    /// volumen de ingresos por severidad de triage y tasas de resolución de guardia médica
    /// devueltos por el procedimiento almacenado <c>sp_ReporteGuardiaTriage_Resumen</c>.
    /// </summary>
    public class ReporteGuardiaResumenDTO
    {
        /// <summary>Cantidad total de ingresos de emergencia en el intervalo de análisis.</summary>
        public int TotalEmergencias { get; set; } = 0;

        /// <summary>Cantidad de ingresos categorizados como Prioridad Alta (Triage 1: Riesgo vital / Código Rojo).</summary>
        public int TotalAlta { get; set; } = 0;

        /// <summary>Cantidad de ingresos categorizados como Prioridad Media (Triage 2: Urgencia moderada / Código Amarillo).</summary>
        public int TotalMedia { get; set; } = 0;

        /// <summary>Cantidad de ingresos categorizados como Prioridad Baja (Triage 3: Guardia regular / Código Verde).</summary>
        public int TotalBaja { get; set; } = 0;

        /// <summary>Total de pacientes cuya atención médica fue iniciada o completada ('Atendido', 'En Consulta', 'Finalizado').</summary>
        public int TotalAtendidos { get; set; } = 0;

        /// <summary>Total de pacientes en espera en sala de guardia o llamados ('En Espera', 'Llamado').</summary>
        public int TotalEnEspera { get; set; } = 0;

        /// <summary>Total de pacientes que cancelaron o abandonaron la guardia ('Cancelado', 'Baja').</summary>
        public int TotalCancelados { get; set; } = 0;

        /// <summary>Porcentaje de resolución efectiva sobre el total de ingresos de guardia.</summary>
        public decimal TasaResolucion { get; set; } = 0.0m;
    }
}
