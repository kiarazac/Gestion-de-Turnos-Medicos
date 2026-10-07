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
    }
}
