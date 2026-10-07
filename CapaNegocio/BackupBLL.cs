using System;
using System.Collections.Generic;
using System.IO;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio para la gestión de copias de seguridad (Backup a demanda)
    /// y restauración con doble autorización obligatoria (Administrador y Gerente).
    /// </summary>
    public class BackupBLL
    {
        private readonly BackupDAL _backupDAL = new BackupDAL();
        private readonly UsuarioDAL _usuarioDAL = new UsuarioDAL();

        /// <summary>
        /// Genera el nombre estandarizado exigido para el archivo de backup,
        /// incluyendo nombre de la base de datos, fecha y hora exacta.
        /// </summary>
        /// <returns>Nombre del archivo con formato dbGestionTurnos_Backup_yyyyMMdd_HHmmss.bak</returns>
        public string GenerarNombreSugeridoBackup()
        {
            return $"dbGestionTurnos_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
        }

        /// <summary>
        /// Obtiene el catálogo de usuarios activos con rol de Gerente habilitados para autorizar la restauración.
        /// </summary>
        public List<UsuarioLoginResult> ObtenerGerentesActivos()
        {
            return _backupDAL.ListarGerentesActivos();
        }

        /// <summary>
        /// Obtiene la ruta por defecto del servidor SQL Server si estuviese disponible.
        /// </summary>
        public string ObtenerRutaServidorDefault()
        {
            return _backupDAL.ObtenerRutaBackupDefaultServidor();
        }

        /// <summary>
        /// Ejecuta la creación a demanda de la copia de seguridad en el directorio seleccionado por el Administrador.
        /// </summary>
        /// <param name="directorioDestino">Directorio o carpeta elegida por el usuario.</param>
        /// <param name="rutaArchivoGenerado">Ruta física absoluta final del archivo generado.</param>
        public void CrearBackup(string directorioDestino, out string rutaArchivoGenerado)
        {
            if (string.IsNullOrWhiteSpace(directorioDestino))
            {
                throw new ArgumentException("Debe seleccionar un directorio de destino para guardar la copia de seguridad.");
            }

            if (!Directory.Exists(directorioDestino))
            {
                Directory.CreateDirectory(directorioDestino);
            }

            string nombreArchivo = GenerarNombreSugeridoBackup();
            rutaArchivoGenerado = Path.Combine(directorioDestino, nombreArchivo);

            _backupDAL.GenerarBackup(rutaArchivoGenerado);
        }

        /// <summary>
        /// Valida la doble autorización obligatoria exigida para restaurar la base de datos:
        /// comprueba credenciales activas del Administrador y del Gerente.
        /// </summary>
        /// <param name="correoAdmin">Correo del Administrador autenticado.</param>
        /// <param name="contrasenaAdmin">Contraseña ingresada por el Administrador.</param>
        /// <param name="correoGerente">Correo del Gerente seleccionado.</param>
        /// <param name="contrasenaGerente">Contraseña ingresada por el Gerente.</param>
        /// <exception cref="ArgumentException">Se lanza si algún campo requerido está vacío.</exception>
        /// <exception cref="UnauthorizedAccessException">Se lanza si alguna de las dos credenciales es inválida.</exception>
        public void ValidarDobleAutorizacion(string correoAdmin, string contrasenaAdmin, string correoGerente, string contrasenaGerente)
        {
            if (string.IsNullOrWhiteSpace(correoAdmin))
                throw new ArgumentException("El correo del Administrador no puede ser nulo o vacío.");

            if (string.IsNullOrWhiteSpace(contrasenaAdmin))
                throw new ArgumentException("Debe ingresar la contraseña del Administrador.");

            if (string.IsNullOrWhiteSpace(correoGerente))
                throw new ArgumentException("Debe seleccionar o ingresar el correo del Gerente autorizante.");

            if (string.IsNullOrWhiteSpace(contrasenaGerente))
                throw new ArgumentException("Debe ingresar la contraseña del Gerente autorizante.");

            // 1. Validar identidad del Administrador
            UsuarioLoginResult? adminLogin = null;
            try
            {
                adminLogin = _usuarioDAL.ValidarLogin(correoAdmin.Trim(), contrasenaAdmin);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException($"Error al autenticar al Administrador: {ex.Message}", ex);
            }

            if (adminLogin == null)
            {
                throw new UnauthorizedAccessException("La contraseña del Administrador es incorrecta o el usuario no está activo.");
            }

            string rolAdmin = adminLogin.NombreRol?.Trim().ToLowerInvariant() ?? string.Empty;
            if (!rolAdmin.Contains("admin") && adminLogin.IdRol != 3)
            {
                throw new UnauthorizedAccessException("El usuario especificado como Administrador no cuenta con privilegios de Administrador.");
            }

            // 2. Validar identidad del Gerente
            UsuarioLoginResult? gerenteLogin = null;
            try
            {
                gerenteLogin = _usuarioDAL.ValidarLogin(correoGerente.Trim(), contrasenaGerente);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException($"Error al autenticar al Gerente: {ex.Message}", ex);
            }

            if (gerenteLogin == null)
            {
                throw new UnauthorizedAccessException("La contraseña del Gerente es incorrecta o el usuario no está activo.");
            }

            string rolGerente = gerenteLogin.NombreRol?.Trim().ToLowerInvariant() ?? string.Empty;
            if (!rolGerente.Contains("geren") && gerenteLogin.IdRol != 5 && gerenteLogin.IdRol != 21)
            {
                throw new UnauthorizedAccessException("El usuario seleccionado como Gerente no cuenta con perfil de Gerente.");
            }
        }

        /// <summary>
        /// Ejecuta el proceso integral de restauración de la base de datos tras verificar la doble autorización.
        /// </summary>
        public void RestaurarBaseDatos(string rutaArchivoBak, string correoAdmin, string contrasenaAdmin, string correoGerente, string contrasenaGerente)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivoBak))
                throw new ArgumentException("Debe seleccionar un archivo de copia de seguridad (.bak).");

            if (!File.Exists(rutaArchivoBak))
                throw new FileNotFoundException("El archivo de copia de seguridad especificado no existe.", rutaArchivoBak);

            // Validar ambas autorizaciones antes de proceder
            ValidarDobleAutorizacion(correoAdmin, contrasenaAdmin, correoGerente, contrasenaGerente);

            // Proceder a la restauración física
            _backupDAL.RestaurarBaseDatos(rutaArchivoBak);
        }
    }
}
