using System;
using System.Text;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que encapsula la información requerida
    /// para la emisión y descarga de comprobantes de turno en formato texto plano (.txt).
    /// </summary>
    public class DatosComprobanteTurno
    {
        /// <summary>Código correlativo de turno asignado (ej. 'E-001', 'C-010').</summary>
        public string NroOrden { get; set; } = string.Empty;

        /// <summary>Nivel de prioridad o triage asignado ('ALTA', 'MEDIA', 'BAJA', 'NORMAL').</summary>
        public string Prioridad { get; set; } = string.Empty;

        /// <summary>Sección médica o especialidad (ej. 'Emergencia', 'Cardiología', 'Pediatría').</summary>
        public string Seccion { get; set; } = string.Empty;

        /// <summary>Fecha y hora exacta en la que se generó el ticket de atención.</summary>
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        /// <summary>Apellido y nombre del paciente atendido.</summary>
        public string NombrePaciente { get; set; } = string.Empty;

        /// <summary>Documento Nacional de Identidad del paciente.</summary>
        public string DniPaciente { get; set; } = string.Empty;

        /// <summary>Cobertura médica u obra social registrada.</summary>
        public string ObraSocial { get; set; } = string.Empty;

        /// <summary>Fecha programada de la consulta (aplicable en turnos por especialidad).</summary>
        public string? FechaTurnoProgramado { get; set; }

        /// <summary>Franja horaria programada (aplicable en turnos por especialidad).</summary>
        public string? HorarioTurnoProgramado { get; set; }

        /// <summary>Palabra clave alfanumérica de confirmación de doble factor (2FA) para cancelación del turno.</summary>
        public string? CodigoCancelacion { get; set; }

        /// <summary>Arancel monetario cobrado o a abonar en caja.</summary>
        public decimal? MontoCobrado { get; set; }

        /// <summary>Detalle de la condición arancelaria (ej. 'Particular (100% Arancel Completo)' o 'Copago 30% (Obra Social)').</summary>
        public string? DetalleArancel { get; set; }

        /// <summary>
        /// Genera el contenido formateado del comprobante médico en texto plano.
        /// </summary>
        /// <returns>Cadena de texto con el formato oficial del comprobante para exportar a archivo .txt.</returns>
        public string GenerarContenidoTxt()
        {
            var sb = new StringBuilder();
            sb.AppendLine("============================================================");
            sb.AppendLine("              CENTRO MÉDICO - COMPROBANTE DE TURNO          ");
            sb.AppendLine("============================================================");
            sb.AppendLine($"NÚMERO DE ORDEN   : {NroOrden}");
            sb.AppendLine($"PRIORIDAD / TRIAGE: {Prioridad}");
            sb.AppendLine($"SECCIÓN / SERVICIO: {Seccion}");
            sb.AppendLine($"FECHA DE EMISIÓN  : {FechaEmision:dd/MM/yyyy}");
            sb.AppendLine($"HORA DE EMISIÓN   : {FechaEmision:HH:mm:ss} hs");
            sb.AppendLine("------------------------------------------------------------");
            sb.AppendLine($"PACIENTE          : {NombrePaciente}");
            if (!string.IsNullOrWhiteSpace(DniPaciente))
                sb.AppendLine($"DNI               : {DniPaciente}");
            if (!string.IsNullOrWhiteSpace(ObraSocial))
                sb.AppendLine($"OBRA SOCIAL       : {ObraSocial}");

            if (!string.IsNullOrWhiteSpace(FechaTurnoProgramado))
            {
                sb.AppendLine("------------------------------------------------------------");
                sb.AppendLine($"FECHA PROGRAMADA  : {FechaTurnoProgramado}");
                if (!string.IsNullOrWhiteSpace(HorarioTurnoProgramado))
                    sb.AppendLine($"HORARIO ASIGNADO  : {HorarioTurnoProgramado} hs");
            }

            if (MontoCobrado.HasValue)
            {
                sb.AppendLine("------------------------------------------------------------");
                sb.AppendLine($"ARANCEL EN CAJA   : $ {MontoCobrado.Value:N2}");
                if (!string.IsNullOrWhiteSpace(DetalleArancel))
                    sb.AppendLine($"CONDICIÓN DE PAGO : {DetalleArancel}");
            }

            if (!string.IsNullOrWhiteSpace(CodigoCancelacion))
            {
                sb.AppendLine("------------------------------------------------------------");
                sb.AppendLine($"CLAVE DE CANCELACIÓN (2FA): {CodigoCancelacion}");
                sb.AppendLine("* Conserve esta clave personal para gestionar o cancelar su turno.");
            }

            sb.AppendLine("============================================================");
            sb.AppendLine("      Por favor, conserve este comprobante hasta su llamado ");
            sb.AppendLine("============================================================");
            return sb.ToString();
        }
    }
}
