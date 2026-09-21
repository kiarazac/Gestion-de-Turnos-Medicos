using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) para la administración y persistencia de salas de atención médica,
    /// estados operativos, apertura/cierre de consultorios y asignación de profesionales mediante Stored Procedures.
    /// </summary>
    public class SalaDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerSalas</c> o consulta SQL resiliente.
        /// Retorna una lista de <see cref="SalaDTO"/>. Permite filtrar por profesional médico e incluir salas inactivas.
        /// </summary>
        /// <param name="idUsuario">ID del médico asignado (opcional).</param>
        /// <param name="incluirInactivas">Si es <c>true</c>, incluye salas dadas de baja lógica.</param>
        /// <returns>Lista de objetos <see cref="SalaDTO"/>.</returns>
        public List<SalaDTO> ObtenerSalas(int? idUsuario = null, bool incluirInactivas = false)
        {
            using (var context = new dbTurnosMedicos())
            {
                var paramUsuario = new SqlParameter("@IdUsuario", (object)idUsuario ?? DBNull.Value);
                var paramInactivas = new SqlParameter("@IncluirInactivas", incluirInactivas);

                try
                {
                    return context.Database
                        .SqlQueryRaw<SalaDTO>("EXEC sp_ObtenerSalas @IdUsuario, @IncluirInactivas", paramUsuario, paramInactivas)
                        .ToList();
                }
                catch (SqlException)
                {
                    string sql = @"
                        SELECT 
                            s.IdSala,
                            s.NombreSala,
                            s.EstadoSala,
                            ds.IdUsuario,
                            u.Nombre AS NombreMedico,
                            u.Apellido AS ApellidoMedico,
                            ISNULL(ds.DescripcionAtencion, '') AS DescripcionAtencion,
                            s.Activo
                        FROM Salas s
                        LEFT JOIN DetallesSalas ds ON s.IdSala = ds.IdSala AND (ds.Activo = 1 OR s.Activo = 0)
                        LEFT JOIN Usuarios u ON ds.IdUsuario = u.IdUsuario AND u.Activo = 1
                        WHERE (@IncluirInactivas = 1 OR s.Activo = 1)
                          AND (@IdUsuario IS NULL OR ds.IdUsuario = @IdUsuario)
                        ORDER BY s.Activo DESC, s.NombreSala ASC;";

                    return context.Database
                        .SqlQueryRaw<SalaDTO>(sql, paramInactivas, paramUsuario)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Reactiva una sala médica con baja lógica previa (<c>Activo = 1, FechaBaja = NULL</c>)
        /// mediante <c>sp_ReactivarSala</c> con fallback SQL directo.
        /// </summary>
        /// <param name="idSala">Identificador único de la sala a reactivar.</param>
        public void ReactivarSala(int idSala)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_ReactivarSala @IdSala", pIdSala);
                }
                catch (SqlException ex) when (ex.Number == 2812)
                {
                    context.Database.ExecuteSqlRaw(
                        @"UPDATE Salas SET Activo = 1, FechaBaja = NULL, FechaModificacion = GETDATE() WHERE IdSala = @IdSala;
                          UPDATE DetallesSalas SET Activo = 1, FechaBaja = NULL WHERE IdSala = @IdSala;",
                        pIdSala);
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_AbrirSala</c> para habilitar el consultorio y vincular al médico presente.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idUsuario">Identificador del médico que abre la sala.</param>
        public void AbrirSala(int idSala, int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);

                context.Database.ExecuteSqlRaw("EXEC sp_AbrirSala @IdSala, @IdUsuario", pIdSala, pIdUsuario);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_CerrarSala</c> al finalizar la atención médica o jornada de trabajo.
        /// </summary>
        /// <param name="idSala">Identificador de la sala a cerrar.</param>
        public void CerrarSala(int idSala)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                context.Database.ExecuteSqlRaw("EXEC sp_CerrarSala @IdSala", pIdSala);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_InsertarSala</c> para dar de alta una nueva sala en el establecimiento.
        /// </summary>
        /// <param name="nombreSala">Nombre o número del consultorio.</param>
        /// <param name="estadoSala">Estado operativo inicial ('Disponible', 'Ocupada').</param>
        /// <returns>ID autogenerado de la nueva sala (<c>IdNuevaSala</c>).</returns>
        public int InsertarSala(string nombreSala, string estadoSala)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombreSala = new SqlParameter("@NombreSala", nombreSala);
                var pEstadoSala = new SqlParameter("@EstadoSala", estadoSala);

                var resultado = context.Database
                    .SqlQueryRaw<NuevaSalaIdDTO>("EXEC sp_InsertarSala @NombreSala, @EstadoSala", pNombreSala, pEstadoSala)
                    .AsEnumerable()
                    .FirstOrDefault();

                return resultado != null ? resultado.IdNuevaSala : 0;
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_EliminarSala</c> para aplicar la baja lógica a una sala médica.
        /// </summary>
        /// <param name="idSala">Identificador de la sala a dar de baja.</param>
        public void EliminarSala(int idSala)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                context.Database.ExecuteSqlRaw("EXEC sp_EliminarSala @IdSala", pIdSala);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_AsignarSalaMedico</c> para registrar la asignación de un profesional a una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idUsuario">Identificador del médico.</param>
        /// <param name="descripcionAtencion">Notas u observaciones sobre la atención prestada.</param>
        public void AsignarSalaMedico(int idSala, int idUsuario, string? descripcionAtencion = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pDesc = new SqlParameter("@DescripcionAtencion", (object?)descripcionAtencion ?? DBNull.Value);

                context.Database.ExecuteSqlRaw("EXEC sp_AsignarSalaMedico @IdSala, @IdUsuario, @DescripcionAtencion", pIdSala, pIdUsuario, pDesc);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ActualizarEstadoSala</c> para modificar el estado de disponibilidad del consultorio.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nuevoEstado">Nuevo estado ('Disponible', 'Ocupada', 'En Mantenimiento').</param>
        public void ActualizarEstadoSala(int idSala, string nuevoEstado)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pNuevoEstado = new SqlParameter("@NuevoEstado", nuevoEstado);

                context.Database.ExecuteSqlRaw("EXEC sp_ActualizarEstadoSala @IdSala, @NuevoEstado", pIdSala, pNuevoEstado);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ModificarSala</c> para actualizar la denominación y estado operativo de un consultorio.
        /// Cuenta con mecanismo de fallback si el procedimiento en la base de datos solo admite 2 parámetros.
        /// </summary>
        /// <param name="idSala">Identificador único de la sala a modificar.</param>
        /// <param name="nombreSala">Nuevo nombre para la sala.</param>
        /// <param name="estadoSala">Nuevo estado operativo de la sala (opcional).</param>
        public void ModificarSala(int idSala, string nombreSala, string? estadoSala = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pNombreSala = new SqlParameter("@NombreSala", nombreSala);
                var pEstadoSala = new SqlParameter("@EstadoSala", (object?)estadoSala ?? DBNull.Value);

                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_ModificarSala @IdSala, @NombreSala, @EstadoSala", pIdSala, pNombreSala, pEstadoSala);
                }
                catch (SqlException ex) when (ex.Number == 8144)
                {
                    var pIdSala2 = new SqlParameter("@IdSala", idSala);
                    var pNombreSala2 = new SqlParameter("@NombreSala", nombreSala);
                    context.Database.ExecuteSqlRaw("EXEC sp_ModificarSala @IdSala, @NombreSala", pIdSala2, pNombreSala2);

                    if (!string.IsNullOrWhiteSpace(estadoSala))
                    {
                        var pEstado = new SqlParameter("@EstadoSala", estadoSala);
                        var pId = new SqlParameter("@IdSala", idSala);
                        context.Database.ExecuteSqlRaw(
                            "UPDATE Salas SET EstadoSala = @EstadoSala, FechaModificacion = GETDATE() WHERE IdSala = @IdSala AND Activo = 1",
                            pEstado, pId);
                    }
                }
            }
        }

        /// <summary>
        /// Reasigna los profesionales médicos asignados a una sala de atención.
        /// Aplica una baja lógica a las vinculaciones activas existentes en <c>DetallesSalas</c>
        /// e inserta las nuevas vinculaciones mediante el procedimiento <c>sp_AsignarSalaMedico</c>.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idsMedicos">Lista de identificadores de los usuarios médicos a vincular.</param>
        public void ReasignarMedicosASala(int idSala, List<int> idsMedicos)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                context.Database.ExecuteSqlRaw("UPDATE DetallesSalas SET Activo = 0, FechaBaja = GETDATE() WHERE IdSala = @IdSala AND Activo = 1", pIdSala);

                if (idsMedicos != null && idsMedicos.Count > 0)
                {
                    foreach (int idUsuario in idsMedicos)
                    {
                        var pSala = new SqlParameter("@IdSala", idSala);
                        var pUsuario = new SqlParameter("@IdUsuario", idUsuario);
                        var pDesc = new SqlParameter("@DescripcionAtencion", string.Empty);
                        context.Database.ExecuteSqlRaw("EXEC sp_AsignarSalaMedico @IdSala, @IdUsuario, @DescripcionAtencion", pSala, pUsuario, pDesc);
                    }
                }
            }
        }
    }
}