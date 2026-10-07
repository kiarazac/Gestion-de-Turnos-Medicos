using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) para la administración de copias de seguridad (Backup)
    /// y restauración de la base de datos SQL Server dbGestionTurnos.
    /// </summary>
    public class BackupDAL
    {
        private const string BaseDatosNombre = "dbGestionTurnos";
        private const string CadenaConexionDefault = "Server=localhost\\SQLEXPRESS;Database=dbGestionTurnos;Integrated Security=True;TrustServerCertificate=True;";

        /// <summary>
        /// Obtiene la cadena de conexión configurada en el contexto de base de datos.
        /// </summary>
        private string ObtenerCadenaConexion()
        {
            try
            {
                using (var context = new dbTurnosMedicos())
                {
                    string connStr = context.Database.GetDbConnection().ConnectionString;
                    if (!string.IsNullOrWhiteSpace(connStr))
                        return connStr;
                }
            }
            catch
            {
                // Fallback seguro a la instancia local por defecto
            }

            return CadenaConexionDefault;
        }

        /// <summary>
        /// Obtiene el directorio predeterminado de copias de seguridad del servidor SQL Server.
        /// </summary>
        /// <returns>Ruta absoluta en el servidor o string vacío si no se encuentra.</returns>
        public string ObtenerRutaBackupDefaultServidor()
        {
            string cadena = ObtenerCadenaConexion();
            try
            {
                using (var conn = new SqlConnection(cadena))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS NVARCHAR(500));", conn))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return result.ToString() ?? string.Empty;
                        }
                    }
                }
            }
            catch
            {
                // Ignorar y retornar vacío
            }

            return string.Empty;
        }

        /// <summary>
        /// Obtiene la lista de usuarios activos con rol de Gerente para la doble autorización.
        /// </summary>
        public List<UsuarioLoginResult> ListarGerentesActivos()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<UsuarioLoginResult>("EXEC sp_ListarGerentes").ToList();
            }
        }

        /// <summary>
        /// Ejecuta la copia de seguridad física completa (Full Backup) de la base de datos dbGestionTurnos.
        /// Implementa un mecanismo de staging transparente para evitar el error de permisos 'Operating system error 5 (Acceso denegado)'
        /// cuando el Administrador elige rutas en su perfil de usuario (Escritorio, Documentos, Descargas).
        /// </summary>
        /// <param name="rutaDestinoFinal">Ruta física completa donde debe residir el archivo .bak generado.</param>
        public void GenerarBackup(string rutaDestinoFinal)
        {
            if (string.IsNullOrWhiteSpace(rutaDestinoFinal))
                throw new ArgumentException("La ruta de destino del archivo de copia no puede ser nula o vacía.");

            string? directorioDestino = Path.GetDirectoryName(rutaDestinoFinal);
            if (!string.IsNullOrEmpty(directorioDestino) && !Directory.Exists(directorioDestino))
            {
                Directory.CreateDirectory(directorioDestino);
            }

            string cadena = ObtenerCadenaConexion();

            try
            {
                // Intentar backup directo a la ruta seleccionada
                using (var context = new dbTurnosMedicos())
                {
                    var paramRuta = new SqlParameter("@RutaArchivo", rutaDestinoFinal);
                    context.Database.ExecuteSqlRaw("EXEC sp_RealizarBackupBaseDatos @RutaArchivo", paramRuta);
                }
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 3201 || sqlEx.Message.Contains("Operating system error 5") || sqlEx.Message.Contains("Acceso denegado"))
            {
                // Staging: SQL Server no tiene permiso de escritura en la carpeta del usuario.
                // Generamos en el directorio nativo de SQL Server y copiamos mediante C#
                string defaultBackupDir = ObtenerRutaBackupDefaultServidor();
                if (string.IsNullOrWhiteSpace(defaultBackupDir) || !Directory.Exists(defaultBackupDir))
                {
                    defaultBackupDir = Path.GetTempPath();
                }

                string nombreArchivo = Path.GetFileName(rutaDestinoFinal);
                string rutaStaging = Path.Combine(defaultBackupDir, $"staging_{Guid.NewGuid():N}_{nombreArchivo}");

                using (var context = new dbTurnosMedicos())
                {
                    var paramStaging = new SqlParameter("@RutaArchivo", rutaStaging);
                    context.Database.ExecuteSqlRaw("EXEC sp_RealizarBackupBaseDatos @RutaArchivo", paramStaging);
                }

                // C# tiene permisos plenos del usuario para mover/copiar al destino seleccionado
                if (File.Exists(rutaStaging))
                {
                    File.Copy(rutaStaging, rutaDestinoFinal, overwrite: true);
                    try
                    {
                        File.Delete(rutaStaging);
                    }
                    catch
                    {
                        // Staging temporal no bloquea el éxito
                    }
                }
            }
        }

        /// <summary>
        /// Restaura la base de datos dbGestionTurnos a partir del archivo .bak indicado.
        /// Conecta al catálogo neutral 'master', desconecta sesiones activas mediante SINGLE_USER,
        /// ejecuta la restauración con REPLACE y restablece MULTI_USER.
        /// </summary>
        /// <param name="rutaArchivoBak">Ruta del archivo de copia de seguridad (.bak).</param>
        public void RestaurarBaseDatos(string rutaArchivoBak)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivoBak))
                throw new ArgumentException("Debe especificar un archivo de copia de seguridad válido.");

            if (!File.Exists(rutaArchivoBak))
                throw new FileNotFoundException("El archivo de copia de seguridad no existe en la ruta especificada.", rutaArchivoBak);

            string rutaParaSql = rutaArchivoBak;
            string? stagingCreado = null;

            // Para evitar que SQL Server falle al leer carpetas privadas del usuario durante el restore,
            // si la ruta no está en el directorio de SQL Server, preparamos una copia de lectura en el directorio nativo
            string defaultBackupDir = ObtenerRutaBackupDefaultServidor();
            if (!string.IsNullOrWhiteSpace(defaultBackupDir) && Directory.Exists(defaultBackupDir))
            {
                try
                {
                    string nombreBak = Path.GetFileName(rutaArchivoBak);
                    stagingCreado = Path.Combine(defaultBackupDir, $"restore_staging_{Guid.NewGuid():N}_{nombreBak}");
                    File.Copy(rutaArchivoBak, stagingCreado, overwrite: true);
                    rutaParaSql = stagingCreado;
                }
                catch
                {
                    // Si no se puede copiar por permisos de Windows sobre Program Files, se intenta directo con la ruta original
                    rutaParaSql = rutaArchivoBak;
                    stagingCreado = null;
                }
            }

            string cadenaOriginal = ObtenerCadenaConexion();
            var builder = new SqlConnectionStringBuilder(cadenaOriginal)
            {
                InitialCatalog = "master"
            };

            // Liberar conexiones previas retenidas en pools de ADO.NET
            SqlConnection.ClearAllPools();

            try
            {
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();

                    // 1. Aislar dbGestionTurnos y cortar conexiones activas
                    using (var cmdSingle = new SqlCommand($"ALTER DATABASE [{BaseDatosNombre}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", conn))
                    {
                        cmdSingle.CommandTimeout = 120;
                        cmdSingle.ExecuteNonQuery();
                    }

                    try
                    {
                        // 2. Ejecutar restauración con reemplazo
                        string restoreSql = $"RESTORE DATABASE [{BaseDatosNombre}] FROM DISK = @RutaBackup WITH REPLACE, STATS = 10;";
                        using (var cmdRestore = new SqlCommand(restoreSql, conn))
                        {
                            cmdRestore.Parameters.Add(new SqlParameter("@RutaBackup", rutaParaSql));
                            cmdRestore.CommandTimeout = 300;
                            cmdRestore.ExecuteNonQuery();
                        }
                    }
                    finally
                    {
                        // 3. Restablecer acceso multi-usuario en dbGestionTurnos
                        using (var cmdMulti = new SqlCommand($"ALTER DATABASE [{BaseDatosNombre}] SET MULTI_USER;", conn))
                        {
                            cmdMulti.CommandTimeout = 120;
                            cmdMulti.ExecuteNonQuery();
                        }
                    }
                }
            }
            finally
            {
                // Limpiar staging temporal si fue creado
                if (!string.IsNullOrEmpty(stagingCreado) && File.Exists(stagingCreado))
                {
                    try
                    {
                        File.Delete(stagingCreado);
                    }
                    catch
                    {
                        // Ignorar limpieza de archivo de staging
                    }
                }
            }
        }
    }
}
