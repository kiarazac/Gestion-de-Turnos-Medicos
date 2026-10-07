using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) para la administración y consulta de obras sociales y medicinas prepagas,
    /// ejecutando el procedimiento almacenado <c>sp_ListarObrasSociales</c> o consultas SQL de contingencia.
    /// </summary>
    public class ObraSocialDAL
    {
        /// <summary>
        /// Obtiene el catálogo de obras sociales y empresas prepagas disponibles en el sistema.
        /// </summary>
        /// <param name="incluirInactivas">Si es <c>true</c>, incluye también coberturas dadas de baja lógica.</param>
        /// <returns>Lista de <see cref="ObraSocialDTO"/> ordenadas alfabéticamente (con Particular primero).</returns>
        public List<ObraSocialDTO> ListarObrasSociales(bool incluirInactivas = false)
        {
            using (var context = new dbTurnosMedicos())
            {
                var paramInactivas = new SqlParameter("@IncluirInactivas", incluirInactivas);
                try
                {
                    return context.Database
                        .SqlQueryRaw<ObraSocialDTO>("EXEC sp_ListarObrasSociales @IncluirInactivas", paramInactivas)
                        .ToList();
                }
                catch (SqlException)
                {
                    string sql = @"
                        SELECT 
                            IdObraSocial,
                            Nombre,
                            ISNULL(Sigla, '') AS Sigla,
                            Activo
                        FROM ObrasSociales
                        WHERE (@IncluirInactivas = 1 OR Activo = 1)
                        ORDER BY 
                            CASE WHEN IdObraSocial = 1 THEN 0 ELSE 1 END,
                            Nombre ASC;";

                    return context.Database
                        .SqlQueryRaw<ObraSocialDTO>(sql, paramInactivas)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_InsertarObraSocial</c> para dar de alta una nueva cobertura médica.
        /// Cuenta con mecanismo fallback ante entornos donde no se haya desplegado el SP.
        /// </summary>
        /// <param name="nombre">Nombre oficial o denominación de la obra social.</param>
        /// <param name="sigla">Sigla o acrónimo identificatorio opcional.</param>
        public void InsertarObraSocial(string nombre, string? sigla)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombre = new SqlParameter("@Nombre", (object)nombre ?? DBNull.Value);
                var pSigla = new SqlParameter("@Sigla", (object?)sigla ?? DBNull.Value);

                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_InsertarObraSocial @Nombre, @Sigla", pNombre, pSigla);
                }
                catch (SqlException ex) when (ex.Number == 2812)
                {
                    string sql = @"
                        IF EXISTS (SELECT 1 FROM ObrasSociales WHERE UPPER(LTRIM(RTRIM(Nombre))) = UPPER(@Nombre) AND Activo = 0)
                        BEGIN
                            UPDATE ObrasSociales 
                            SET Activo = 1, FechaBaja = NULL, FechaModificacion = GETDATE(), Sigla = COALESCE(@Sigla, Sigla)
                            WHERE UPPER(LTRIM(RTRIM(Nombre))) = UPPER(@Nombre);
                        END
                        ELSE
                        BEGIN
                            INSERT INTO ObrasSociales (Nombre, Sigla, Activo, FechaCreacion)
                            VALUES (@Nombre, @Sigla, 1, GETDATE());
                        END";
                    context.Database.ExecuteSqlRaw(sql, pNombre, pSigla);
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ModificarObraSocial</c> para actualizar los datos de una obra social existente.
        /// </summary>
        /// <param name="idObraSocial">Identificador único de la obra social.</param>
        /// <param name="nombre">Nueva denominación.</param>
        /// <param name="sigla">Nueva sigla opcional.</param>
        public void ModificarObraSocial(int idObraSocial, string nombre, string? sigla)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pId = new SqlParameter("@IdObraSocial", idObraSocial);
                var pNombre = new SqlParameter("@Nombre", (object)nombre ?? DBNull.Value);
                var pSigla = new SqlParameter("@Sigla", (object?)sigla ?? DBNull.Value);

                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_ModificarObraSocial @IdObraSocial, @Nombre, @Sigla", pId, pNombre, pSigla);
                }
                catch (SqlException ex) when (ex.Number == 2812)
                {
                    string sql = "UPDATE ObrasSociales SET Nombre = @Nombre, Sigla = @Sigla, FechaModificacion = GETDATE() WHERE IdObraSocial = @IdObraSocial";
                    context.Database.ExecuteSqlRaw(sql, pNombre, pSigla, pId);
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_EliminarObraSocial</c> para aplicar una baja lógica a la cobertura médica.
        /// </summary>
        /// <param name="idObraSocial">Identificador de la obra social a desactivar.</param>
        public void EliminarObraSocial(int idObraSocial)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pId = new SqlParameter("@IdObraSocial", idObraSocial);

                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_EliminarObraSocial @IdObraSocial", pId);
                }
                catch (SqlException ex) when (ex.Number == 2812)
                {
                    string sql = "UPDATE ObrasSociales SET Activo = 0, FechaBaja = GETDATE(), FechaModificacion = GETDATE() WHERE IdObraSocial = @IdObraSocial";
                    context.Database.ExecuteSqlRaw(sql, pId);
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReactivarObraSocial</c> para restituir lógicamente una cobertura médica inactiva.
        /// </summary>
        /// <param name="idObraSocial">Identificador de la obra social a reactivar.</param>
        public void ReactivarObraSocial(int idObraSocial)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pId = new SqlParameter("@IdObraSocial", idObraSocial);

                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_ReactivarObraSocial @IdObraSocial", pId);
                }
                catch (SqlException ex) when (ex.Number == 2812)
                {
                    string sql = "UPDATE ObrasSociales SET Activo = 1, FechaBaja = NULL, FechaModificacion = GETDATE() WHERE IdObraSocial = @IdObraSocial";
                    context.Database.ExecuteSqlRaw(sql, pId);
                }
            }
        }
    }
}
