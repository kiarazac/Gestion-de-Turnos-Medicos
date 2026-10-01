using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio para la administración de historias clínicas, diagnósticos y recetas médicas.
    /// </summary>
    public class HistoriaClinicaBLL
    {
        private readonly HistoriaClinicaDAL _historiaDAL = new HistoriaClinicaDAL();

        /// <summary>
        /// Valida y registra una nueva entrada de evolución clínica en la historia médica del paciente.
        /// </summary>
        /// <param name="tipoTurno">Modalidad de atención ('Emergencia' o 'Especialidad').</param>
        /// <param name="diagRapido">Diagnóstico presuntivo o rápido.</param>
        /// <param name="descripHistoriaClinica">Detalle exhaustivo de la consulta y evolución.</param>
        /// <param name="recetaMedicamentos">Prescripción médica de fármacos e indicaciones.</param>
        /// <param name="idPaciente">Identificador del paciente atendido.</param>
        /// <param name="idTurno">Identificador del turno médico.</param>
        /// <param name="idUsuario">Identificador del profesional médico responsable.</param>
        /// <exception cref="ArgumentException">Se lanza si los identificadores son inválidos o la evolución clínica está vacía.</exception>
        public void RegistrarHistoria(string tipoTurno, string diagRapido, string descripHistoriaClinica, string recetaMedicamentos, int idPaciente, int idTurno, int idUsuario)
        {
            if (idPaciente <= 0 || idTurno <= 0 || idUsuario <= 0)
                throw new ArgumentException("Los identificadores de paciente, turno y usuario son obligatorios para el registro clínico.");

            if (string.IsNullOrWhiteSpace(descripHistoriaClinica))
                descripHistoriaClinica = diagRapido;

            if (string.IsNullOrWhiteSpace(diagRapido))
                diagRapido = descripHistoriaClinica;

            if (string.IsNullOrWhiteSpace(diagRapido))
                throw new ArgumentException("El diagnóstico médico no puede estar vacío.");

            _historiaDAL.InsertarHistoria(tipoTurno, diagRapido, descripHistoriaClinica, recetaMedicamentos, idPaciente, idTurno, idUsuario);
        }

        /// <summary>
        /// Obtiene el registro histórico cronológico de consultas y diagnósticos médicos de un paciente.
        /// </summary>
        /// <param name="idPaciente">Identificador único del paciente.</param>
        /// <returns>Lista de <see cref="HistoriaClinicaDTO"/> con los antecedentes del paciente.</returns>
        /// <exception cref="ArgumentException">Se lanza si el ID del paciente es menor o igual a cero.</exception>
        public List<HistoriaClinicaDTO> ObtenerHistoriaClinicaPaciente(int idPaciente)
        {
            if (idPaciente <= 0)
                throw new ArgumentException("El ID del paciente no es válido.");

            return _historiaDAL.ObtenerHistoriaClinicaPaciente(idPaciente);
        }

        /// <summary>
        /// Obtiene y valida la consulta del historial de atenciones médicas realizadas por un profesional específico.
        /// </summary>
        /// <param name="idUsuario">Identificador único del profesional médico.</param>
        /// <param name="fechaDesde">Fecha inicial de filtrado opcional.</param>
        /// <param name="fechaHasta">Fecha final de filtrado opcional.</param>
        /// <returns>Lista de <see cref="AtencionMedicoDTO"/> con el detalle de pacientes y consultas atendidas.</returns>
        /// <exception cref="ArgumentException">Se lanza si el ID del usuario es inválido o si el rango de fechas es inconsistente.</exception>
        public List<AtencionMedicoDTO> ObtenerAtencionesPorMedico(int idUsuario, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El identificador del profesional médico es obligatorio y debe ser mayor a cero.");

            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value > fechaHasta.Value)
                throw new ArgumentException("La fecha inicial de búsqueda no puede ser posterior a la fecha final.");

            return _historiaDAL.ObtenerAtencionesPorMedico(idUsuario, fechaDesde, fechaHasta);
        }
    }
}