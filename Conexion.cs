using Microsoft.Data.SqlClient;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Proveedor centralizado de conexión a la base de datos SQL Server.
    /// Permite configurar la cadena de conexión en un solo lugar para todos los formularios.
    /// </summary>
    public static class Conexion
    {
        // =========================================================================
        // CADENA DE CONEXIÓN A SQL SERVER (DOCKER / LOCAL)
        // NOTA: Modificá el valor de "TU_PASSWORD" y las credenciales según tu contenedor Docker.
        // =========================================================================
        public static string CadenaConexion = "Server=localhost,1433;Database=GestionTurnosMedicos;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True;";

        /// <summary>
        /// Crea y devuelve una nueva instancia de SqlConnection usando la cadena centralizada.
        /// </summary>
        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(CadenaConexion);
        }
    }
}
