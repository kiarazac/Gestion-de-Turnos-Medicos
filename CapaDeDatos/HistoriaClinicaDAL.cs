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
                var pTipoTurno = new SqlParameter("@TipoTurno", tipoTurno ?? "Consulta");
                var pDiagRapido = new SqlParameter("@DiagRapido", (object)(diagRapido ?? string.Empty));
                var pDescrip = new SqlParameter("@DescripHistoriaClinica", (object)(descripHistoriaClinica ?? string.Empty));
                var pReceta = new SqlParameter("@RecetaMedicamentos", (object)(recetaMedicamentos ?? string.Empty));
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

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerAtencionesPorMedico</c> para recuperar las consultas atendidas por un médico.
        /// </summary>
        /// <param name="idUsuario">Identificador único del profesional médico.</param>
        /// <param name="fechaDesde">Fecha inicial de filtrado opcional.</param>
        /// <param name="fechaHasta">Fecha final de filtrado opcional.</param>
        /// <returns>Lista de <see cref="AtencionMedicoDTO"/> ordenada cronológicamente de forma descendente.</returns>
        public List<AtencionMedicoDTO> ObtenerAtencionesPorMedico(int idUsuario, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pFechaDesde = new SqlParameter("@FechaDesde", (object?)fechaDesde ?? DBNull.Value);
                var pFechaHasta = new SqlParameter("@FechaHasta", (object?)fechaHasta ?? DBNull.Value);

                try
                {
                    return context.Database
                        .SqlQueryRaw<AtencionMedicoDTO>("EXEC sp_ObtenerAtencionesPorMedico @IdUsuario, @FechaDesde, @FechaHasta", pIdUsuario, pFechaDesde, pFechaHasta)
                        .ToList();
                }
                catch (Exception)
                {
                    // Fallback defensivo: garantiza que la consulta retorne exactamente las columnas requeridas por AtencionMedicoDTO
                    var pIdUsuario2 = new SqlParameter("@IdUsuario", idUsuario);
                    var pFechaDesde2 = new SqlParameter("@FechaDesde", (object?)fechaDesde ?? DBNull.Value);
                    var pFechaHasta2 = new SqlParameter("@FechaHasta", (object?)fechaHasta ?? DBNull.Value);

                    string sql = @"
                        SELECT 
                            hc.IdHistoria,
                            hc.Fecha,
                            hc.TipoTurno,
                            hc.DiagRapido,
                            hc.DescripHistoriaClinica,
                            hc.RecetaMedicamentos,
                            p.IdPaciente,
                            p.Nombre AS NombrePaciente,
                            p.Apellido AS ApellidoPaciente,
                            p.Dni AS DniPaciente,
                            ISNULL(os.Nombre, 'Particular / Sin Obra Social') AS ObraSocial,
                            ISNULL(t.NroOrden, '--') AS NroOrden,
                            ISNULL(s.NombreSala, 'Consultorio') AS NombreSala,
                            ISNULL(e.Nombre, 'General') AS Especialidad,
                            t.IdPrioridad,
                            ISNULL(pr.Descripcion, 'N/A') AS GravedadTriage,
                            ISNULL(
                                (SELECT STRING_AGG(s2.Descripcion, ', ')
                                 FROM TurnoSintomas ts2
                                 INNER JOIN Sintomas s2 ON ts2.IdSintoma = s2.IdSintoma
                                 WHERE ts2.IdTurno = t.IdTurno AND ts2.Activo = 1),
                                '--'
                            ) AS SintomasTriage
                        FROM HistoriasClinicas hc
                        INNER JOIN Pacientes p ON hc.IdPaciente = p.IdPaciente
                        LEFT JOIN ObrasSociales os ON p.IdObraSocial = os.IdObraSocial
                        LEFT JOIN Turnos t ON hc.IdTurno = t.IdTurno
                        LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
                        LEFT JOIN Salas s ON t.IdSala = s.IdSala
                        LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
                        WHERE hc.IdUsuario = @IdUsuario
                          AND hc.Activo = 1
                          AND (@FechaDesde IS NULL OR hc.Fecha >= @FechaDesde)
                          AND (@FechaHasta IS NULL OR hc.Fecha <= @FechaHasta)
                        ORDER BY hc.Fecha DESC;";

                    return context.Database
                        .SqlQueryRaw<AtencionMedicoDTO>(sql, pIdUsuario2, pFechaDesde2, pFechaHasta2)
                        .ToList();
                }
            }
        }
    }
}