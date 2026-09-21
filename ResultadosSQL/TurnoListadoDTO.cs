using System;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) que transporta el listado de turnos
    /// devuelto por los procedimientos <c>sp_ObtenerListaTurnos</c> y <c>sp_ListarTurnosEspecialidad</c>.
    /// </summary>
    public class TurnoListadoDTO
    {
        /// <summary>Identificador único del turno.</summary>
        public int IdTurno { get; set; }

        /// <summary>Código de orden visible (ej. 'T-002').</summary>
        public string NroOrden { get; set; }

        /// <summary>Estado operativo del turno ('En Espera', 'En Atencion', 'Finalizado').</summary>
        public string Estado { get; set; }

        /// <summary>Modalidad del turno ('Emergencia' o 'Especialidad').</summary>
        public string TipoTurno { get; set; }

        /// <summary>Fecha asignada al turno.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Nombre de la especialidad médica.</summary>
        public string NombreEspecialidad { get; set; }

        /// <summary>Descripción textual de la prioridad asignada.</summary>
        public string PrioridadTexto { get; set; }

        /// <summary>Identificador foráneo de prioridad.</summary>
        public int? IdPrioridad { get; set; }
    }
}
