using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) para la administración y persistencia de especialidades médicas,
    /// ejecución de altas, bajas lógicas, modificaciones y consultas mediante Stored Procedures.
    /// </summary>
    public class EspecialidadDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarEspecialidades</c> o consulta SQL resiliente.
        /// Devuelve el conjunto de especialidades mapeadas a <see cref="EspecialidadDTO"/>, permitiendo incluir especialidades dadas de baja lógica.
        /// </summary>
        /// <param name="incluirInactivas">Si es <c>true</c>, incluye especialidades inactivas (<c>Activo = 0</c>).</param>
        /// <returns>Lista de <see cref="EspecialidadDTO"/>.</returns>
        public List<EspecialidadDTO> ListarEspecialidades(bool incluirInactivas = false)
        {
            using (var context = new dbTurnosMedicos())
            {
                var paramInactivas = new SqlParameter("@IncluirInactivas", incluirInactivas);
                try
                {
                    return context.Database
                        .SqlQueryRaw<EspecialidadDTO>("EXEC sp_ListarEspecialidades @IncluirInactivas", paramInactivas)
                        .ToList();
                }
                catch (SqlException)
                {
                    string sql = @"
                        SELECT 
                            IdEspecialidad,
                            Nombre,
                            Activo
                        FROM Especialidades
                        WHERE (@IncluirInactivas = 1 OR Activo = 1)
                        ORDER BY Activo DESC, Nombre ASC;";

                    return context.Database
                        .SqlQueryRaw<EspecialidadDTO>(sql, paramInactivas)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_InsertarEspecialidad</c> para dar de alta una nueva especialidad médica.
        /// </summary>
        /// <param name="nombre">Nombre de la nueva especialidad.</param>
        public void InsertarEspecialidad(string nombre)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombre = new SqlParameter("@Nombre", nombre);
                context.Database.ExecuteSqlRaw("EXEC sp_InsertarEspecialidad @Nombre", pNombre);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ModificarEspecialidad</c> para actualizar la denominación de una especialidad.
        /// Cuenta con fallback SQL directo para resiliencia ante motores de base de datos sin el SP instalado.
        /// </summary>
        /// <param name="idEspecialidad">Identificador único de la especialidad.</param>
        /// <param name="nombre">Nombre actualizado de la especialidad.</param>
        public void ModificarEspecialidad(int idEspecialidad, string nombre)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pId = new SqlParameter("@IdEspecialidad", idEspecialidad);
                var pNombre = new SqlParameter("@Nombre", nombre);

                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_ModificarEspecialidad @IdEspecialidad, @Nombre", pId, pNombre);
                }
                catch (SqlException ex) when (ex.Number == 2812)
                {
                    context.Database.ExecuteSqlRaw(
                        "UPDATE Especialidades SET Nombre = @Nombre, FechaModificacion = GETDATE() WHERE IdEspecialidad = @IdEspecialidad",
                        pNombre, pId);
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_EliminarEspecialidad</c> para aplicar una baja lógica, conservando el registro histórico.
        /// </summary>
        /// <param name="idEspecialidad">Identificador único de la especialidad a dar de baja.</param>
        public void EliminarEspecialidad(int idEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", idEspecialidad);
                context.Database.ExecuteSqlRaw("EXEC sp_EliminarEspecialidad @IdEspecialidad", pIdEspecialidad);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReactivarEspecialidad</c> para restituir lógicamente una especialidad inactiva (<c>Activo = 1, FechaBaja = NULL</c>)
        /// y reactivar sus vínculos con médicos en <c>MedicosEspecialidades</c>.
        /// </summary>
        /// <param name="idEspecialidad">Identificador único de la especialidad a reactivar.</param>
        public void ReactivarEspecialidad(int idEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", idEspecialidad);
                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_ReactivarEspecialidad @IdEspecialidad", pIdEspecialidad);
                }
                catch (SqlException ex) when (ex.Number == 2812)
                {
                    context.Database.ExecuteSqlRaw(
                        @"UPDATE Especialidades SET Activo = 1, FechaBaja = NULL, FechaModificacion = GETDATE() WHERE IdEspecialidad = @IdEspecialidad;
                          UPDATE MedicosEspecialidades SET Activo = 1, FechaBaja = NULL WHERE IdEspecialidad = @IdEspecialidad;",
                        pIdEspecialidad);
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerEspecialidadesPorMedico</c> para listar las ramas médicas que atiende un profesional.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario médico.</param>
        /// <returns>Lista de <see cref="EspecialidadDTO"/>.</returns>
        public List<EspecialidadDTO> ObtenerEspecialidadesPorMedico(int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var parametro = new Microsoft.Data.SqlClient.SqlParameter("@IdUsuario", idUsuario);
                return context.Database
                    .SqlQueryRaw<EspecialidadDTO>("EXEC sp_ObtenerEspecialidadesPorMedico @IdUsuario", parametro)
                    .ToList();
            }
        }
    }
}