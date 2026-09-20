using Gestion_de_Turnos_Medicos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    // Clase exclusiva para interactuar con dbTurnosMedicos y ejecutar los Stored Procedures de especialidades[cite: 2].
    public class EspecialidadDAL
    {
        // Ejecuta el procedimiento sp_ListarEspecialidades o consulta SQL resiliente.
        // Devuelve el resultado mapeado a EspecialidadDTO, con soporte opcional para incluir especialidades inactivas.
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
                    // Fallback SQL directo si el SP legacy no admite parámetros o no devuelve Activo
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

        // Ejecuta sp_InsertarEspecialidad para dar de alta una nueva especialidad en el sistema[cite: 2].
        public void InsertarEspecialidad(string nombre)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombre = new SqlParameter("@Nombre", nombre);
                // ExecuteSqlRaw se usa porque es una inserción y no esperamos una tabla de retorno[cite: 2].
                context.Database.ExecuteSqlRaw("EXEC sp_InsertarEspecialidad @Nombre", pNombre);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento sp_ModificarEspecialidad para actualizar la denominación de una especialidad médica.
        /// Cuenta con fallback SQL directo para resiliencia ante bases de datos heredadas.
        /// </summary>
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
                catch (SqlException ex) when (ex.Number == 2812) // Si no existe el SP en el motor
                {
                    context.Database.ExecuteSqlRaw(
                        "UPDATE Especialidades SET Nombre = @Nombre, FechaModificacion = GETDATE() WHERE IdEspecialidad = @IdEspecialidad",
                        pNombre, pId);
                }
            }
        }

        // Ejecuta sp_EliminarEspecialidad para realizar una baja lógica, conservando el registro histórico[cite: 1, 2].
        public void EliminarEspecialidad(int idEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", idEspecialidad);
                context.Database.ExecuteSqlRaw("EXEC sp_EliminarEspecialidad @IdEspecialidad", pIdEspecialidad);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento sp_ReactivarEspecialidad para restituir lógicamente una especialidad inactiva (Activo = 1, FechaBaja = NULL)
        /// y reactivar sus vínculos con médicos en MedicosEspecialidades.
        /// </summary>
        public void ReactivarEspecialidad(int idEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", idEspecialidad);
                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_ReactivarEspecialidad @IdEspecialidad", pIdEspecialidad);
                }
                catch (SqlException ex) when (ex.Number == 2812) // Si no existe el SP en el motor
                {
                    context.Database.ExecuteSqlRaw(
                        @"UPDATE Especialidades SET Activo = 1, FechaBaja = NULL, FechaModificacion = GETDATE() WHERE IdEspecialidad = @IdEspecialidad;
                          UPDATE MedicosEspecialidades SET Activo = 1, FechaBaja = NULL WHERE IdEspecialidad = @IdEspecialidad;",
                        pIdEspecialidad);
                }
            }
        }

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