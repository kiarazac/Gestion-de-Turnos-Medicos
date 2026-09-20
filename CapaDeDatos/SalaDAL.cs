using Gestion_de_Turnos_Medicos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    // La Capa de Datos (DAL) es la ÚNICA que se comunica con SQL Server, utilizando exclusivamente dbTurnosMedicos (EF Core)[cite: 2].
    public class SalaDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento sp_ObtenerSalas o consulta directa resiliente.
        /// Retorna una lista de SalaDTO. Si se le pasa un idUsuario, trae solo las de ese médico; si es null, trae todas.
        /// Soporta el parámetro opcional incluirInactivas para listar salas dadas de baja lógica.
        /// </summary>
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
                    // Fallback SQL directo con soporte de filtrado activo e histórico:
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
        /// Reactiva o re-da de alta lógicamente una sala en el sistema (Activo = 1, FechaBaja = NULL)
        /// y restaura sus asignaciones históricas en DetallesSalas.
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
                catch (SqlException ex) when (ex.Number == 2812) // Si no existe el SP en el motor
                {
                    context.Database.ExecuteSqlRaw(
                        @"UPDATE Salas SET Activo = 1, FechaBaja = NULL, FechaModificacion = GETDATE() WHERE IdSala = @IdSala;
                          UPDATE DetallesSalas SET Activo = 1, FechaBaja = NULL WHERE IdSala = @IdSala;",
                        pIdSala);
                }
            }
        }

        // Ejecuta el procedimiento sp_AbrirSala.
        public void AbrirSala(int idSala, int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);

                // ExecuteSqlRaw se utiliza cuando hacemos operaciones (como cambios de estado, inserts o updates) que NO retornan tablas[cite: 2].
                context.Database.ExecuteSqlRaw("EXEC sp_AbrirSala @IdSala, @IdUsuario", pIdSala, pIdUsuario);
            }
        }

        // Ejecuta el procedimiento sp_CerrarSala.
        public void CerrarSala(int idSala)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                context.Database.ExecuteSqlRaw("EXEC sp_CerrarSala @IdSala", pIdSala);
    }
        }

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
        public void EliminarSala(int idSala)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                context.Database.ExecuteSqlRaw("EXEC sp_EliminarSala @IdSala", pIdSala);
            }
        }

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

        // Actualiza de manera genérica el estado operativo de una sala.
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
        /// Ejecuta el procedimiento almacenado sp_ModificarSala para actualizar el nombre de una sala existente.
        /// Respeta la firma real de 2 parámetros en dbGestionTurnos (@IdSala, @NombreSala) y actualiza el campo EstadoSala si se especifica.
        /// Utiliza parámetros tipados para proteger contra inyección SQL.
        /// </summary>
        /// <param name="idSala">Identificador único de la sala a modificar.</param>
        /// <param name="nombreSala">Nuevo nombre descriptivo para la sala.</param>
        /// <param name="estadoSala">Estado operativo de la sala ('Disponible', 'Libre', 'Ocupada', 'En Mantenimiento', 'Cerrada').</param>
        public void ModificarSala(int idSala, string nombreSala, string? estadoSala = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pNombreSala = new SqlParameter("@NombreSala", nombreSala);
                var pEstadoSala = new SqlParameter("@EstadoSala", (object?)estadoSala ?? DBNull.Value);

                try
                {
                    // 2. Ejecutamos sp_ModificarSala con los 3 parámetros (@IdSala, @NombreSala, @EstadoSala)
                    context.Database.ExecuteSqlRaw("EXEC sp_ModificarSala @IdSala, @NombreSala, @EstadoSala", pIdSala, pNombreSala, pEstadoSala);
                }
                catch (SqlException ex) when (ex.Number == 8144) // Error 8144: Si el procedimiento en una BD legacy tuviera solo 2 parámetros
                {
                    var pIdSala2 = new SqlParameter("@IdSala", idSala);
                    var pNombreSala2 = new SqlParameter("@NombreSala", nombreSala);
                    context.Database.ExecuteSqlRaw("EXEC sp_ModificarSala @IdSala, @NombreSala", pIdSala2, pNombreSala2);

                    // 3. Fallback: Si se especificó un estado de sala, actualizamos el campo EstadoSala manualmente
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
        /// Aplica una baja lógica a las asignaciones activas existentes en DetallesSalas
        /// y luego inserta las nuevas vinculaciones seleccionadas mediante el Stored Procedure existente sp_AsignarSalaMedico.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idsMedicos">Lista de identificadores de los usuarios médicos a vincular.</param>
        public void ReasignarMedicosASala(int idSala, List<int> idsMedicos)
        {
            using (var context = new dbTurnosMedicos())
            {
                // 1. Damos de baja lógica las asignaciones activas actuales de la sala
                var pIdSala = new SqlParameter("@IdSala", idSala);
                context.Database.ExecuteSqlRaw("UPDATE DetallesSalas SET Activo = 0, FechaBaja = GETDATE() WHERE IdSala = @IdSala AND Activo = 1", pIdSala);

                // 2. Insertamos las nuevas asignaciones seleccionadas por el administrador mediante sp_AsignarSalaMedico
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