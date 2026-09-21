using System.Security.Cryptography;
using System.Text;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Utilidades criptográficas y funciones de seguridad transversales de la aplicación.
    /// </summary>
    public static class Seguridad
    {
        /// <summary>
        /// Genera el valor hash criptográfico SHA-256 de una contraseña en texto plano.
        /// </summary>
        /// <param name="contraseniaPlano">Contraseña en texto plano a transformar.</param>
        /// <returns>Cadena hexadecimal de 64 caracteres en minúsculas representativa del hash SHA-256.</returns>
        /// <remarks>
        /// Implementa una función de resumen unidireccional estándar para evitar el almacenamiento o transporte de contraseñas en texto claro.
        /// No requiere paquetes NuGet externos. Para entornos de producción de alta seguridad, se aconseja migrar a algoritmos con salt y factor de costo adaptativo (ej. BCrypt o Argon2).
        /// </remarks>
        public static string HashearContrasenia(string contraseniaPlano)
        {
            if (string.IsNullOrEmpty(contraseniaPlano))
                return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(contraseniaPlano);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}
