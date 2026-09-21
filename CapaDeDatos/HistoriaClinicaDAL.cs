using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) para la persistencia y consulta de historias clínicas y evoluciones médicas mediante Stored Procedures.
    /// </summary>
    public class HistoriaClinicaDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_InsertarHistoriaClinica</c> para registrar la evolución, diagnóstico y receta médica de una consulta finalizada.
        /// </summary>
        /// <param name="tipoTurno">Tipo de atención médica ('Emergencia' o 'Especialidad').</param>
        /// <param name="diagRapido">Diagnóstico presuntivo o rápido.</param>
        /// <param name="descripHistoriaClinica">Descripción de la anamnesis y evolución clínica.</param>
        /// <param name="recetaMedicamentos">Detalle de fármacos prescritos e indicaciones.</param>
        /// <param name="idPaciente">Identificador único del paciente.</param>
        /// <param name="idTurno">Identificador único del turno atendido.</param>
        /// <param name="idUsuario">Identificador único del profesional médico firmante.</param>
        public void InsertarHistoria(string tipoTurno, string diagRapido, string descripHistoriaClinica, string recetaMedicamentos, int idPaciente, int idTurno, int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pTipoTurno = new SqlParameter("@TipoTurno", tipoTurno);
                var pDiagRapido = new SqlParameter("@DiagRapido", (object)diagRapido ?? DBNull.Value);
                var pDescrip = new SqlParameter("@DescripHistoriaClinica", (object)descripHistoriaClinica ?? DBNull.Value);
                var pReceta = new SqlParameter("@RecetaMedicamentos", (object)recetaMedicamentos ?? DBNull.Value);
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);

                context.Database.ExecuteSqlRaw("EXEC sp_InsertarHistoriaClinica @TipoTurno, @DiagRapido, @DescripHistoriaClinica, @RecetaMedicamentos, @IdPaciente, @IdTurno, @IdUsuario",
                    pTipoTurno, pDiagRapido, pDescrip, pReceta, pIdPaciente, pIdTurno, pIdUsuario);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerHistoriaClinicaPaciente</c> para recuperar el expediente de antecedentes médicos de un paciente.
        /// </summary>
        /// <param name="idPaciente">Identificador único del paciente.</param>
        /// <returns>Lista de <see cref="HistoriaClinicaDTO"/> ordenada cronológicamente.</returns>
        public List<HistoriaClinicaDTO> ObtenerHistoriaClinicaPaciente(int idPaciente)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);

                return context.Database
                    .SqlQueryRaw<HistoriaClinicaDTO>("EXEC sp_ObtenerHistoriaClinicaPaciente @IdPaciente", pIdPaciente)
                    .ToList();
            }
        }
    }
}