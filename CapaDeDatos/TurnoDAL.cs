using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) para la gestión, emisión y ciclo de vida de los turnos médicos,
    /// ejecución de triage, control de llamadas a consultorio y pantallas públicas mediante Stored Procedures.
    /// </summary>
    public class TurnoDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerSintomas</c> para recuperar el catálogo de síntomas y gravedades.
        /// </summary>
        /// <returns>Lista de <see cref="SintomaDTO"/>.</returns>
        public List<SintomaDTO> ObtenerSintomas()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<SintomaDTO>("EXEC sp_ObtenerSintomas").ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_CrearTurnoEmergencia</c> para registrar un turno de urgencia en base de datos.
        /// </summary>
        /// <param name="idPaciente">Identificador del paciente.</param>
        /// <param name="idPrioridad">Nivel de prioridad calculado (1=Alta, 2=Media, 3=Baja).</param>
        /// <param name="monto">Arancel cobrado en caja (opcional).</param>
        /// <param name="codigoCancelacion">Palabra clave alfanumérica de seguridad 2FA para cancelación (opcional).</param>
        /// <returns>Objeto <see cref="ResultadoTurnoDTO"/> con el ID del turno creado y el número de orden asignado.</returns>
        public ResultadoTurnoDTO CrearTurnoEmergenciaCompleto(int idPaciente, int idPrioridad, decimal? monto = null, string? codigoCancelacion = null)
        {
            // Instanciamos el contexto de base de datos para ejecutar el procedimiento
            using (var context = new dbTurnosMedicos())
            {
                // Parámetros SQL para la ejecución del procedimiento almacenado
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);
                var pIdPrioridad = new SqlParameter("@IdPrioridad", idPrioridad);
                var pMonto = new SqlParameter("@Monto", (object?)monto ?? DBNull.Value);
                var pCodigo = new SqlParameter("@CodigoCancelacion", (object?)codigoCancelacion ?? DBNull.Value);

                // Ejecutamos sp_CrearTurnoEmergencia pasando los parámetros requeridos
                var resultado = context.Database
                    .SqlQueryRaw<ResultadoTurnoDTO>("EXEC sp_CrearTurnoEmergencia @IdPaciente, @IdPrioridad, @Monto, @CodigoCancelacion", pIdPaciente, pIdPrioridad, pMonto, pCodigo)
                    .AsEnumerable()
                    .FirstOrDefault();

                // Aseguramos que el código 2FA quede asignado en el DTO de retorno
                if (resultado != null && !string.IsNullOrWhiteSpace(codigoCancelacion))
                {
                    resultado.CodigoCancelacion = codigoCancelacion;
                }

                return resultado ?? new ResultadoTurnoDTO { CodigoCancelacion = codigoCancelacion };
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_GuardarTurnoSintoma</c> para vincular un síntoma reportado con el turno generado.
        /// </summary>
        /// <param name="idTurno">Identificador del turno.</param>
        /// <param name="idSintoma">Identificador del síntoma.</param>
        public void GuardarTurnoSintoma(int idTurno, int idSintoma)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pIdSintoma = new SqlParameter("@IdSintoma", idSintoma);

                context.Database.ExecuteSqlRaw("EXEC sp_GuardarTurnoSintoma @IdTurno, @IdSintoma", pIdTurno, pIdSintoma);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerGravedadSintoma</c> y devuelve el valor numérico de prioridad correspondiente.
        /// </summary>
        /// <param name="idSintoma">Identificador del síntoma.</param>
        /// <returns>Nivel numérico de prioridad: 1 para Alta, 2 para Media, 3 para Baja.</returns>
        public int ObtenerGravedadDeSintoma(int idSintoma)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSintoma = new SqlParameter("@IdSintoma", idSintoma);

                var resultado = context.Database
                    .SqlQueryRaw<GravedadSintomaDTO>("EXEC sp_ObtenerGravedadSintoma @IdSintoma", pIdSintoma)
                    .AsEnumerable()
                    .FirstOrDefault();

                string gravedadTexto = resultado != null ? resultado.Gravedad : "Baja";

                if (gravedadTexto.Equals("Alta", StringComparison.OrdinalIgnoreCase))
                    return 1;

                if (gravedadTexto.Equals("Media", StringComparison.OrdinalIgnoreCase))
                    return 2;

                return 3;
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_LlamarSiguienteTurno</c> para convocar al paciente a una sala con su médico asignado.
        /// Valida en base de datos que solo profesionales con especialidad Clínico atiendan turnos de emergencia.
        /// </summary>
        /// <param name="idTurno">Identificador del turno.</param>
        /// <param name="nombreMedico">Nombre del médico que realiza la llamada.</param>
        /// <param name="salaAsignada">Consultorio o sala asignada para la atención.</param>
        /// <param name="idUsuario">Identificador único del médico (opcional para validación de especialidad y trazabilidad).</param>
        public void LlamarSiguientePaciente(int idTurno, string nombreMedico, string salaAsignada, int? idUsuario = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pNombreMedico = new SqlParameter("@NombreMedico", nombreMedico);
                var pSalaAsignada = new SqlParameter("@SalaAsignada", salaAsignada);
                var pIdUsuario = new SqlParameter("@IdUsuario", (object?)idUsuario ?? DBNull.Value);

                context.Database.ExecuteSqlRaw("EXEC sp_LlamarSiguienteTurno @IdTurno, @NombreMedico, @SalaAsignada, @IdUsuario",
                    pIdTurno, pNombreMedico, pSalaAsignada, pIdUsuario);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerHorariosConEstado</c> para una especialidad y fecha determinadas.
        /// Retorna la matriz completa de franjas horarias con su indicador de disponibilidad y datos de turnos ocupados.
        /// </summary>
        /// <param name="nombreEspecialidad">Nombre de la especialidad.</param>
        /// <param name="fecha">Fecha requerida.</param>
        /// <returns>Lista de <see cref="HorarioDisponibleDTO"/> con horarios disponibles y ocupados con su respectivo detalle.</returns>
        public List<HorarioDisponibleDTO> ObtenerHorariosDisponibles(string nombreEspecialidad, DateTime fecha)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pEspecialidad = new SqlParameter("@NombreEspecialidad", nombreEspecialidad);
                var pFecha = new SqlParameter("@Fecha", fecha.Date);

                try
                {
                    return context.Database
                        .SqlQueryRaw<HorarioDisponibleDTO>("EXEC sp_ObtenerHorariosConEstado @NombreEspecialidad, @Fecha", pEspecialidad, pFecha)
                        .ToList();
                }
                catch (SqlException)
                {
                    string sql = @"
                        DECLARE @Horarios TABLE (Horario VARCHAR(10));
                        INSERT INTO @Horarios VALUES ('08:30'), ('09:00'), ('09:30'), ('10:00'), ('10:30'), ('11:00'), ('14:00'), ('14:30'), ('15:00'), ('16:00');

                        SELECT 
                            h.Horario,
                            CAST(CASE WHEN t.IdTurno IS NULL THEN 1 ELSE 0 END AS BIT) AS EstaDisponible,
                            t.IdTurno,
                            t.NroOrden,
                            CASE WHEN t.IdTurno IS NOT NULL THEN CONCAT(pac.Apellido, ', ', pac.Nombre) ELSE NULL END AS Paciente,
                            pac.Dni,
                            ISNULL(os.Nombre, 'Particular / Sin Obra Social') AS ObraSocial,
                            t.Estado,
                            t.CodigoCancelacion
                        FROM @Horarios h
                        LEFT JOIN (
                            SELECT 
                                t_sub.IdTurno,
                                CONVERT(VARCHAR(5), t_sub.Horario, 108) AS HorarioTexto,
                                t_sub.NroOrden,
                                t_sub.IdPaciente,
                                t_sub.Estado,
                                t_sub.CodigoCancelacion
                            FROM Turnos t_sub
                            INNER JOIN Especialidades e ON t_sub.IdEspecialidad = e.IdEspecialidad
                            WHERE e.Nombre = @NombreEspecialidad
                              AND CAST(t_sub.Fecha AS DATE) = @Fecha
                              AND t_sub.Activo = 1
                              AND t_sub.Estado <> 'Cancelado'
                        ) t ON h.Horario = t.HorarioTexto
                        LEFT JOIN Pacientes pac ON t.IdPaciente = pac.IdPaciente
                        LEFT JOIN ObrasSociales os ON pac.IdObraSocial = os.IdObraSocial
                        ORDER BY h.Horario ASC;";

                    return context.Database
                        .SqlQueryRaw<HorarioDisponibleDTO>(sql, pEspecialidad, pFecha)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Comprueba si ya existe un turno activo registrado con la misma fecha, horario y especialidad médica.
        /// Si alguno de estos tres atributos difiere, la consulta devuelve <c>false</c>.
        /// Ejecuta el procedimiento almacenado <c>sp_ExisteTurnoEspecialidad</c> con fallback a consulta SQL directa.
        /// </summary>
        /// <param name="nombreEspecialidad">Nombre de la especialidad médica solicitada.</param>
        /// <param name="fecha">Fecha requerida para el turno.</param>
        /// <param name="horario">Horario asignado (ej. '10:00').</param>
        /// <returns><c>true</c> si ya existe un turno con la misma combinación de fecha, horario y especialidad; <c>false</c> en caso contrario.</returns>
        public bool ExisteTurnoEspecialidad(string nombreEspecialidad, DateTime fecha, string horario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pEspecialidad = new SqlParameter("@NombreEspecialidad", nombreEspecialidad);
                var pFecha = new SqlParameter("@Fecha", fecha.Date);
                var pHorario = new SqlParameter("@Horario", horario);

                try
                {
                    var res = context.Database
                        .SqlQueryRaw<int>("EXEC sp_ExisteTurnoEspecialidad @NombreEspecialidad, @Fecha, @Horario", pEspecialidad, pFecha, pHorario)
                        .AsEnumerable()
                        .FirstOrDefault();

                    return res > 0;
                }
                catch (SqlException)
                {
                    string sql = @"
                        SELECT CAST(COUNT(1) AS INT)
                        FROM Turnos t
                        INNER JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
                        WHERE e.Nombre = @NombreEspecialidad
                          AND CAST(t.Fecha AS DATE) = @Fecha
                          AND CAST(t.Horario AS TIME) = CAST(@Horario AS TIME)
                          AND t.Activo = 1;";

                    var count = context.Database
                        .SqlQueryRaw<int>(sql, pEspecialidad, pFecha, pHorario)
                        .AsEnumerable()
                        .FirstOrDefault();

                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_CrearTurnoEspecialidad</c> para registrar un turno programado.
        /// </summary>
        /// <param name="idPaciente">Identificador del paciente.</param>
        /// <param name="nombreEspecialidad">Nombre de la especialidad solicitada.</param>
        /// <param name="fecha">Fecha acordada.</param>
        /// <param name="horario">Horario asignado.</param>
        /// <param name="estado">Estado inicial del turno.</param>
        /// <param name="codigoCancelacion">Palabra clave alfanumérica de 2FA para cancelación.</param>
        /// <returns>Objeto <see cref="ResultadoTurnoDTO"/> con el ID del nuevo turno y número de orden.</returns>
        public ResultadoTurnoDTO CrearTurnoEspecialidad(int idPaciente, string nombreEspecialidad, DateTime fecha, string horario, string estado = "En Espera", string? codigoCancelacion = null, decimal? monto = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);
                var pEspecialidad = new SqlParameter("@NombreEspecialidad", nombreEspecialidad);
                var pFecha = new SqlParameter("@Fecha", fecha.Date);
                var pHorario = new SqlParameter("@Horario", horario);
                var pEstado = new SqlParameter("@Estado", estado);
                var pCodigo = new SqlParameter("@CodigoCancelacion", (object?)codigoCancelacion ?? DBNull.Value);
                var pMonto = new SqlParameter("@Monto", (object?)monto ?? DBNull.Value);

                var res = context.Database
                    .SqlQueryRaw<ResultadoTurnoDTO>("EXEC sp_CrearTurnoEspecialidad @IdPaciente, @NombreEspecialidad, @Fecha, @Horario, @Estado, @CodigoCancelacion, @Monto",
                        pIdPaciente, pEspecialidad, pFecha, pHorario, pEstado, pCodigo, pMonto)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (res != null)
                {
                    res.CodigoCancelacion = codigoCancelacion;
                }

                return res ?? new ResultadoTurnoDTO { CodigoCancelacion = codigoCancelacion };
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_CancelarTurnoEspecialidad</c> para cancelar un turno de especialidad
        /// mediante validación estricta de doble factor (2FA) con la palabra clave alfanumérica.
        /// </summary>
        /// <param name="idTurno">Identificador del turno a cancelar.</param>
        /// <param name="codigoCancelacion">Palabra clave alfanumérica de seguridad.</param>
        public void CancelarTurnoEspecialidad(int idTurno, string codigoCancelacion)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pCodigo = new SqlParameter("@CodigoCancelacion", codigoCancelacion.Trim());

                context.Database.ExecuteSqlRaw("EXEC sp_CancelarTurnoEspecialidad @IdTurno, @CodigoCancelacion", pIdTurno, pCodigo);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_BuscarTurnoActivoEmergencia</c> para localizar un turno activo
        /// de guardia/emergencia por su número de orden (ej: 'E-001') o por el DNI del paciente.
        /// </summary>
        /// <param name="termino">Número de orden o DNI del paciente a buscar.</param>
        /// <returns>Objeto <see cref="TurnoEmergenciaCancelacionDTO"/> con la información del turno o <c>null</c> si no existe.</returns>
        public TurnoEmergenciaCancelacionDTO? BuscarTurnoActivoEmergencia(string termino)
        {
            // Instanciamos el contexto de base de datos
            using (var context = new dbTurnosMedicos())
            {
                // Parámetro con el término de búsqueda limpio
                var pTermino = new SqlParameter("@Termino", termino.Trim());

                // Invocamos el procedimiento sp_BuscarTurnoActivoEmergencia y mapeamos al DTO especializado
                return context.Database
                    .SqlQueryRaw<TurnoEmergenciaCancelacionDTO>("EXEC sp_BuscarTurnoActivoEmergencia @Termino", pTermino)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtiene el turno de emergencia activo (sin atender) de un paciente si existe.
        /// Se consideran turnos sin atender aquellos con TipoTurno 'Emergencia' y Estado distinto de 'Atendido', 'Ausente' y 'Cancelado'.
        /// </summary>
        /// <param name="idPaciente">Identificador del paciente.</param>
        /// <returns>Objeto <see cref="TurnoEmergenciaCancelacionDTO"/> con la información del turno activo o <c>null</c> si no tiene.</returns>
        public TurnoEmergenciaCancelacionDTO? ObtenerTurnoEmergenciaActivoPorPaciente(int idPaciente)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);
                string sql = @"
                    SELECT TOP 1
                        t.IdTurno,
                        t.NroOrden,
                        pac.Apellido,
                        pac.Nombre,
                        pac.Dni,
                        ISNULL(pr.Descripcion, 'MEDIA') AS Prioridad,
                        LEFT(CAST(t.Horario AS VARCHAR(10)), 5) AS Hora,
                        t.Estado,
                        ISNULL(t.CodigoCancelacion, '') AS CodigoCancelacion
                    FROM Turnos t
                    INNER JOIN Pacientes pac ON t.IdPaciente = pac.IdPaciente
                    LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
                    WHERE t.TipoTurno = 'Emergencia'
                      AND t.Activo = 1
                      AND t.IdPaciente = @IdPaciente
                      AND t.Estado NOT IN ('Atendido', 'Ausente', 'Cancelado')
                    ORDER BY t.IdTurno DESC;";

                return context.Database
                    .SqlQueryRaw<TurnoEmergenciaCancelacionDTO>(sql, pIdPaciente)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtiene el turno actualmente en atención médica ('En Consulta', 'En Atención', 'En Atencion') asignado a un profesional médico.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario médico.</param>
        /// <returns>Objeto <see cref="TurnoEmergenciaCancelacionDTO"/> con la información del turno en consulta o <c>null</c> si no tiene.</returns>
        public TurnoEmergenciaCancelacionDTO? ObtenerTurnoEnConsultaPorMedico(int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                string sql = @"
                    SELECT TOP 1
                        t.IdTurno,
                        t.NroOrden,
                        pac.Apellido,
                        pac.Nombre,
                        pac.Dni,
                        ISNULL(pr.Descripcion, 'MEDIA') AS Prioridad,
                        LEFT(CAST(t.Horario AS VARCHAR(10)), 5) AS Hora,
                        t.Estado,
                        ISNULL(t.CodigoCancelacion, '') AS CodigoCancelacion
                    FROM Turnos t
                    INNER JOIN Pacientes pac ON t.IdPaciente = pac.IdPaciente
                    LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
                    WHERE t.IdUsuario = @IdUsuario
                      AND t.Activo = 1
                      AND t.Estado IN ('En Consulta', 'En Atención', 'En Atencion')
                    ORDER BY t.IdTurno DESC;";

                return context.Database
                    .SqlQueryRaw<TurnoEmergenciaCancelacionDTO>(sql, pIdUsuario)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtiene el turno que se encuentra actualmente en estado 'Llamado' asignado a un profesional médico.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario médico.</param>
        /// <returns>Objeto <see cref="TurnoEmergenciaCancelacionDTO"/> con la información del turno llamado o <c>null</c> si no tiene.</returns>
        public TurnoEmergenciaCancelacionDTO? ObtenerTurnoLlamadoPorMedico(int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                string sql = @"
                    SELECT TOP 1
                        t.IdTurno,
                        t.NroOrden,
                        pac.Apellido,
                        pac.Nombre,
                        pac.Dni,
                        ISNULL(pr.Descripcion, 'MEDIA') AS Prioridad,
                        LEFT(CAST(t.Horario AS VARCHAR(10)), 5) AS Hora,
                        t.Estado,
                        ISNULL(t.CodigoCancelacion, '') AS CodigoCancelacion
                    FROM Turnos t
                    INNER JOIN Pacientes pac ON t.IdPaciente = pac.IdPaciente
                    LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
                    WHERE t.IdUsuario = @IdUsuario
                      AND t.Activo = 1
                      AND t.Estado = 'Llamado'
                    ORDER BY t.IdTurno DESC;";

                return context.Database
                    .SqlQueryRaw<TurnoEmergenciaCancelacionDTO>(sql, pIdUsuario)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_CancelarTurnoEmergencia</c> para cancelar un turno de guardia
        /// mediante validación estricta de doble factor (2FA) con la palabra clave alfanumérica del ticket.
        /// </summary>
        /// <param name="idTurno">Identificador del turno de emergencia a cancelar.</param>
        /// <param name="codigoCancelacion">Palabra clave alfanumérica de seguridad 2FA.</param>
        public void CancelarTurnoEmergencia(int idTurno, string codigoCancelacion)
        {
            // Instanciamos el contexto de base de datos
            using (var context = new dbTurnosMedicos())
            {
                // Parámetros SQL requeridos para validar 2FA y cancelar el turno
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pCodigo = new SqlParameter("@CodigoCancelacion", codigoCancelacion.Trim());

                // Ejecutamos sp_CancelarTurnoEmergencia en SQL Server
                context.Database.ExecuteSqlRaw("EXEC sp_CancelarTurnoEmergencia @IdTurno, @CodigoCancelacion", pIdTurno, pCodigo);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarTurnosEmergencia</c> para obtener la lista de urgencias registradas.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoEmergenciaDTO"/>.</returns>
        public List<TurnoEmergenciaDTO> ListarTurnosEmergencia()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<TurnoEmergenciaDTO>("EXEC sp_ListarTurnosEmergencia").ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarTurnosEspecialidad</c> para listar turnos filtrados por nombre de especialidad.
        /// </summary>
        /// <param name="nombreEspecialidad">Nombre de la especialidad.</param>
        /// <returns>Lista de <see cref="TurnoListadoDTO"/>.</returns>
        public List<TurnoListadoDTO> ListarTurnosEspecialidad(string nombreEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombreEspecialidad = new SqlParameter("@NombreEspecialidad", (object)nombreEspecialidad ?? DBNull.Value);
                try
                {
                    return context.Database
                        .SqlQueryRaw<TurnoListadoDTO>("EXEC sp_ListarTurnosEspecialidad @NombreEspecialidad", pNombreEspecialidad)
                        .ToList();
                }
                catch (SqlException)
                {
                    string sql = @"
                        SELECT 
                            t.IdTurno,
                            t.NroOrden,
                            t.Estado,
                            t.TipoTurno,
                            CAST(CAST(t.Fecha AS DATE) AS DATETIME) + CAST(ISNULL(t.Horario, '00:00') AS DATETIME) AS Fecha,
                            e.Nombre AS NombreEspecialidad,
                            ISNULL(p.Descripcion, 'Normal') AS PrioridadTexto,
                            t.IdPrioridad,
                            ISNULL(s.NombreSala, '--') AS NombreSala
                        FROM Turnos t
                        INNER JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
                        LEFT JOIN Prioridades p ON t.IdPrioridad = p.IdPrioridad
                        LEFT JOIN Salas s ON t.IdSala = s.IdSala
                        WHERE t.Activo = 1 
                          AND t.TipoTurno = 'Consulta'
                          AND (e.Nombre = @NombreEspecialidad OR @NombreEspecialidad IS NULL OR @NombreEspecialidad = '')
                          AND t.Estado IN ('En Espera', 'Llamado', 'En Consulta', 'En Atención', 'En Atencion')
                          AND t.Estado NOT IN ('Atendido', 'Ausente', 'Cancelado')
                        ORDER BY t.Fecha ASC, t.Horario ASC;";

                    return context.Database
                        .SqlQueryRaw<TurnoListadoDTO>(sql, pNombreEspecialidad)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerListaTurnos</c> con filtros opcionales de especialidad y estado.
        /// </summary>
        /// <param name="idEspecialidad">ID de especialidad (opcional).</param>
        /// <param name="estado">Estado del turno (opcional).</param>
        /// <returns>Lista de <see cref="TurnoListadoDTO"/>.</returns>
        public List<TurnoListadoDTO> ObtenerListaTurnos(int? idEspecialidad = null, string? estado = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", (object)idEspecialidad ?? DBNull.Value);
                var pEstado = new SqlParameter("@Estado", (object?)estado ?? DBNull.Value);

                try
                {
                    return context.Database
                        .SqlQueryRaw<TurnoListadoDTO>("EXEC sp_ObtenerListaTurnos @IdEspecialidad, @Estado", pIdEspecialidad, pEstado)
                        .ToList();
                }
                catch (SqlException)
                {
                    string sql = @"
                        SELECT 
                            t.IdTurno,
                            t.NroOrden,
                            t.Estado,
                            t.TipoTurno,
                            t.Fecha,
                            e.Nombre AS NombreEspecialidad,
                            pr.Descripcion AS PrioridadTexto,
                            t.IdPrioridad,
                            ISNULL(s.NombreSala, '--') AS NombreSala
                        FROM Turnos t
                        LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
                        LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
                        LEFT JOIN Salas s ON t.IdSala = s.IdSala
                        WHERE t.Activo = 1
                          AND (@IdEspecialidad IS NULL OR t.IdEspecialidad = @IdEspecialidad)
                          AND (@Estado IS NULL OR t.Estado = @Estado)
                        ORDER BY t.Fecha ASC, t.FechaCreacion ASC;";

                    return context.Database
                        .SqlQueryRaw<TurnoListadoDTO>(sql, pIdEspecialidad, pEstado)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Ejecuta <c>sp_ObtenerListaTurnos</c> filtrando por <c>@IdTurno</c> para recuperar los datos del comprobante de un turno.
        /// </summary>
        /// <param name="idTurno">ID del turno.</param>
        /// <returns>Datos del comprobante, o <c>null</c> si el turno no existe o está inactivo.</returns>
        public DatosComprobanteTurno? ObtenerDatosComprobante(int idTurno)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", DBNull.Value);
                var pEstado = new SqlParameter("@Estado", DBNull.Value);
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);

                return context.Database
                    .SqlQueryRaw<DatosComprobanteTurno>("EXEC sp_ObtenerListaTurnos @IdEspecialidad, @Estado, @IdTurno", pIdEspecialidad, pEstado, pIdTurno)
                    .ToList()
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarTurnosAtencion</c> para obtener la cola de turnos en proceso de atención.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoAtencionDTO"/>.</returns>
        public List<TurnoAtencionDTO> ListarTurnosAtencion()
        {
            using (var context = new dbTurnosMedicos())
            {
                try
                {
                    return context.Database.SqlQueryRaw<TurnoAtencionDTO>("EXEC sp_ListarTurnosAtencion").ToList();
                }
                catch (SqlException)
                {
                    string sql = @"
                        SELECT 
                            t.IdTurno,
                            t.NroOrden,
                            CAST(CAST(t.Fecha AS DATE) AS DATETIME) + CAST(ISNULL(t.Horario, ISNULL(CAST(t.FechaCreacion AS TIME), '00:00')) AS DATETIME) AS Fecha,
                            t.Estado,
                            ISNULL(e.Nombre, 'Emergencias / Guardia') AS Especialidad,
                            ISNULL(pr.Descripcion, 'MEDIA') AS Triage,
                            p.Nombre AS NombrePaciente,
                            p.Apellido AS ApellidoPaciente,
                            p.Dni AS DniPaciente,
                            ISNULL(os.Nombre, 'Particular / Sin Obra Social') AS ObraSocial,
                            ISNULL(s.NombreSala, '') AS NombreSala,
                            ISNULL(
                                (
                                    SELECT STRING_AGG(s2.Descripcion, ', ')
                                    FROM TurnoSintomas ts2
                                    INNER JOIN Sintomas s2 ON ts2.IdSintoma = s2.IdSintoma
                                    WHERE ts2.IdTurno = t.IdTurno AND ts2.Activo = 1 AND s2.Activo = 1
                                ),
                                ''
                            ) AS Sintomas,
                            t.IdUsuario,
                            t.IdSala
                        FROM Turnos t
                        INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
                        LEFT JOIN ObrasSociales os ON p.IdObraSocial = os.IdObraSocial
                        LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
                        LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
                        LEFT JOIN Salas s ON t.IdSala = s.IdSala
                        WHERE t.Activo = 1 
                          AND t.Estado IN ('En Espera', 'Llamado', 'En Consulta', 'En Atención', 'En Atencion')
                          AND t.Estado NOT IN ('Atendido', 'Ausente', 'Cancelado')
                        ORDER BY 
                            CASE WHEN t.Estado IN ('Llamado', 'En Consulta', 'En Atención', 'En Atencion') THEN 0 ELSE 1 END ASC,
                            t.IdPrioridad ASC, 
                            t.FechaCreacion ASC;";

                    return context.Database.SqlQueryRaw<TurnoAtencionDTO>(sql).ToList();
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_IniciarAtencionTurno</c> marcando el ingreso del paciente al consultorio.
        /// </summary>
        /// <param name="idTurno">ID del turno.</param>
        /// <param name="salaAsignada">Nombre del consultorio o sala.</param>
        public void IniciarAtencionTurno(int idTurno, string salaAsignada)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pSalaAsignada = new SqlParameter("@SalaAsignada", System.Data.SqlDbType.NVarChar, 100)
                {
                    Value = (object?)salaAsignada ?? DBNull.Value
                };

                context.Database.ExecuteSqlRaw("EXEC sp_IniciarAtencionTurno @IdTurno, @SalaAsignada", pIdTurno, pSalaAsignada);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_FinalizarAtencionTurno</c> para registrar el diagnóstico y concluir la consulta.
        /// </summary>
        /// <param name="idTurno">ID del turno finalizado.</param>
        /// <param name="diagnostico">Conclusiones diagnósticas emitidas por el médico.</param>
        /// <param name="nombreMedico">Nombre del médico tratante.</param>
        /// <param name="salaAsignada">Consultorio donde se prestó la atención.</param>
        /// <param name="idUsuario">ID del médico actuante autenticado (opcional).</param>
        public void FinalizarAtencionTurno(int idTurno, string diagnostico, string nombreMedico, string salaAsignada, int? idUsuario = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pDiagnostico = new SqlParameter("@Diagnostico", diagnostico);
                var pNombreMedico = new SqlParameter("@NombreMedico", nombreMedico);
                var pSalaAsignada = new SqlParameter("@SalaAsignada", salaAsignada);
                var pIdUsuario = new SqlParameter("@IdUsuario", (object?)idUsuario ?? DBNull.Value);

                context.Database.ExecuteSqlRaw("EXEC sp_FinalizarAtencionTurno @IdTurno, @Diagnostico, @NombreMedico, @SalaAsignada, @IdUsuario",
                    pIdTurno, pDiagnostico, pNombreMedico, pSalaAsignada, pIdUsuario);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_MarcarTurnoAusente</c> para registrar la no comparecencia del paciente y liberar la sala de atención.
        /// </summary>
        /// <param name="idTurno">ID del turno a marcar como ausente.</param>
        /// <param name="salaAsignada">Nombre de la sala asociada a liberar (opcional).</param>
        public void MarcarTurnoAusente(int idTurno, string? salaAsignada = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pSala = new SqlParameter("@SalaAsignada", (object?)salaAsignada ?? DBNull.Value);

                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_MarcarTurnoAusente @IdTurno, @SalaAsignada", pIdTurno, pSala);
                }
                catch (SqlException ex) when (ex.Number == 8144 || ex.Number == 8146) // Too many arguments
                {
                    // Fallback para SP anterior con un solo parámetro
                    context.Database.ExecuteSqlRaw("EXEC sp_MarcarTurnoAusente @IdTurno", pIdTurno);

                    // Liberación defensiva de la sala en fallback
                    if (!string.IsNullOrWhiteSpace(salaAsignada) && !salaAsignada.Contains("Sin sala"))
                    {
                        var pSalaDirecta = new SqlParameter("@Sala", salaAsignada);
                        context.Database.ExecuteSqlRaw(
                            "UPDATE Salas SET EstadoSala = 'Disponible', FechaModificacion = GETDATE() WHERE (NombreSala = @Sala OR NombreSala LIKE '%' + @Sala + '%') AND Activo = 1",
                            pSalaDirecta);
                    }
                    else
                    {
                        context.Database.ExecuteSqlRaw(
                            @"UPDATE s SET s.EstadoSala = 'Disponible', s.FechaModificacion = GETDATE()
                              FROM Salas s
                              INNER JOIN Turnos t ON t.IdSala = s.IdSala
                              WHERE t.IdTurno = @IdTurno AND s.Activo = 1",
                            pIdTurno);
                    }
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerTurnosPantallaPublica</c> para poblar la pantalla de llamados en sala de espera.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoPantallaDTO"/>.</returns>
        public List<TurnoPantallaDTO> ObtenerTurnosPantallaPublica()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<TurnoPantallaDTO>("EXEC sp_ObtenerTurnosPantallaPublica").ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarTurnosGeneralesPantalla</c> para alimentar el tablero general de visualización.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoGeneralPantallaDTO"/>.</returns>
        public List<TurnoGeneralPantallaDTO> ObtenerTurnosPantallaGeneral()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database
                    .SqlQueryRaw<TurnoGeneralPantallaDTO>("EXEC sp_ListarTurnosGeneralesPantalla")
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerTurnosEnEspera</c> para obtener la cola ordenada de una especialidad.
        /// </summary>
        /// <param name="idEspecialidad">Identificador único de la especialidad.</param>
        /// <returns>Lista de <see cref="TurnoEsperaDTO"/>.</returns>
        public List<TurnoEsperaDTO> ObtenerTurnosEnEspera(int idEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", idEspecialidad);

                return context.Database
                    .SqlQueryRaw<TurnoEsperaDTO>("EXEC sp_ObtenerTurnosEnEspera @IdEspecialidad", pIdEspecialidad)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_BuscarPacientePorDNI</c> para obtener la ficha de un paciente.
        /// </summary>
        /// <param name="dni">Número de DNI del paciente a buscar.</param>
        /// <returns>Objeto <see cref="PacienteDTO"/> si existe; de lo contrario, <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Se lanza si el DNI es nulo o vacío.</exception>
        public PacienteDTO BuscarPacientePorDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Debe indicar un DNI válido para la búsqueda.");

            string query = "EXEC sp_BuscarPacientePorDNI @DNI";
            var parametro = new Microsoft.Data.SqlClient.SqlParameter("@DNI", dni);

            using (var context = new dbTurnosMedicos())
            {
                var resultado = context.Database.SqlQueryRaw<PacienteDTO>(query, parametro).AsEnumerable().FirstOrDefault();
                return resultado;
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_PuedeAtenderEmergencias</c> para comprobar si el médico
        /// posee la especialidad Clínico (o afín a Clínica Médica) activa para atender turnos de guardia/emergencia.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario médico.</param>
        /// <returns><c>true</c> si el médico tiene especialidad clínico activa; de lo contrario, <c>false</c>.</returns>
        public bool PuedeAtenderEmergencias(int idUsuario)
        {
            if (idUsuario <= 0) return false;

            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);

                try
                {
                    return context.Database
                        .SqlQueryRaw<bool>("EXEC sp_PuedeAtenderEmergencias @IdUsuario", pIdUsuario)
                        .AsEnumerable()
                        .FirstOrDefault();
                }
                catch (SqlException)
                {
                    // Fallback defensivo a consulta directa si el SP no estuviera disponible
                    string sql = @"
                        SELECT CAST(CASE WHEN EXISTS (
                            SELECT 1 
                            FROM MedicosEspecialidades me
                            INNER JOIN Especialidades e ON me.IdEspecialidad = e.IdEspecialidad
                            INNER JOIN Usuarios u ON me.IdUsuario = u.IdUsuario
                            WHERE me.IdUsuario = @IdUsuario 
                              AND me.Activo = 1 
                              AND e.Activo = 1 
                              AND u.Activo = 1
                              AND (e.Nombre = 'Clinico' OR e.Nombre LIKE '%clinic%')
                        ) THEN 1 ELSE 0 END AS BIT);";

                    return context.Database
                        .SqlQueryRaw<bool>(sql, pIdUsuario)
                        .AsEnumerable()
                        .FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Comprueba si un turno médico corresponde a la modalidad de Emergencia o Guardia.
        /// </summary>
        /// <param name="idTurno">Identificador del turno.</param>
        /// <returns><c>true</c> si el turno es de emergencia; de lo contrario, <c>false</c>.</returns>
        public bool EsTurnoEmergencia(int idTurno)
        {
            if (idTurno <= 0) return false;

            using (var context = new dbTurnosMedicos())
            {
                return context.Turnos
                    .Where(t => t.IdTurno == idTurno && t.Activo)
                    .Select(t => t.TipoTurno == "Emergencia" || (t.Especialidad != null && t.Especialidad.Nombre == "Emergencia") || (t.NroOrden != null && t.NroOrden.StartsWith("E-")))
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Recupera la lista de síntomas manifestados en el triage para un turno de emergencia específico mediante <c>sp_ObtenerSintomasPorTurno</c>.
        /// </summary>
        /// <param name="idTurno">Identificador del turno médico.</param>
        /// <returns>Lista de <see cref="SintomaDTO"/> asociados al turno.</returns>
        public List<SintomaDTO> ObtenerSintomasPorTurno(int idTurno)
        {
            if (idTurno <= 0) return new List<SintomaDTO>();

            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                return context.Database
                    .SqlQueryRaw<SintomaDTO>("EXEC sp_ObtenerSintomasPorTurno @IdTurno", pIdTurno)
                    .ToList();
            }
        }

        /// <summary>
        /// Comprueba si ya existe un turno activo y no atendido con el mismo número de orden.
        /// </summary>
        /// <param name="nroOrden">Número de orden a verificar (ej. 'E-001', 'C-002').</param>
        /// <param name="idTurnoExcluir">ID opcional del turno a excluir de la búsqueda (para modificaciones).</param>
        /// <returns><c>true</c> si ya existe otro turno no atendido con ese número; de lo contrario, <c>false</c>.</returns>
        public bool ExisteNroOrdenNoAtendido(string nroOrden, int? idTurnoExcluir = null)
        {
            if (string.IsNullOrWhiteSpace(nroOrden)) return false;

            using (var context = new dbTurnosMedicos())
            {
                return context.Turnos
                    .Any(t => t.Activo 
                           && t.NroOrden == nroOrden 
                           && (!idTurnoExcluir.HasValue || t.IdTurno != idTurnoExcluir.Value)
                           && t.Estado != "Atendido" 
                           && t.Estado != "Cancelado");
            }
        }

        /// <summary>
        /// Inserta un turno ejecutando el procedimiento almacenado <c>sp_InsertarTurno</c>.
        /// </summary>
        public void InsertarTurno(string nroOrden, string tipoTurno, int idPrioridad, int idPaciente, int idEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNroOrden = new SqlParameter("@NroOrden", nroOrden);
                var pTipoTurno = new SqlParameter("@TipoTurno", tipoTurno);
                var pIdPrioridad = new SqlParameter("@IdPrioridad", idPrioridad);
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", idEspecialidad);

                context.Database.ExecuteSqlRaw(
                    "EXEC sp_InsertarTurno @NroOrden, @TipoTurno, @IdPrioridad, @IdPaciente, @IdEspecialidad",
                    pNroOrden, pTipoTurno, pIdPrioridad, pIdPaciente, pIdEspecialidad);
            }
        }
    }
}