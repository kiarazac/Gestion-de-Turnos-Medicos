using System.Security.Cryptography;
using System.Text;

namespace Gestion_de_Turnos_Medicos
{
    public static class Seguridad
    {
        // Hash básico (sin salt) para no mandar la contraseña en texto plano a la base.
        // No usa paquetes externos, así que compila sin instalar nada.
        // Para producción real, lo ideal es migrar a BCrypt/Argon2 (NuGet: BCrypt.Net-Next),
        // que agregan salt y son intencionalmente lentos contra ataques de fuerza bruta.
        public static string HashearContrasenia(string contraseniaPlano)
        {
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
