using Microsoft.Data.SqlClient;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Proveedor centralizado para la administración y provisión de conexiones a la base de datos SQL Server.
    /// </summary>
    /// <remarks>
    /// Provee un punto único de configuración de la cadena de conexión para todo el sistema,
    /// facilitando la conmutación entre entornos locales (ej. SQLEXPRESS) y contenedores Docker.
    /// </remarks>
    public static class Conexion
    {
        /// <summary>
        /// Cadena de conexión principal hacia la base de datos SQL Server.
        /// </summary>
        /// <remarks>
        /// Configurada por defecto para SQL Server en Docker o local en el puerto estándar 1433.
        /// Modifique las credenciales según el entorno de despliegue correspondiente.
        /// </remarks>
        public static string CadenaConexion = "Server=localhost,1433;Database=GestionTurnosMedicos;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True;";

        /// <summary>
        /// Crea y devuelve una nueva instancia de <see cref="SqlConnection"/> utilizando la cadena de conexión configurada.
        /// </summary>
        /// <returns>Una instancia de <see cref="SqlConnection"/> sin abrir lista para su consumo dentro de un bloque <c>using</c>.</returns>
        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(CadenaConexion);
        }
    }
}
