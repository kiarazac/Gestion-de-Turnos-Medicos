using System;
using System.Text.RegularExpressions;

namespace Gestion_de_Turnos_Medicos 
{
    /// <summary>
    /// Provee métodos estáticos de validación de formato para entradas de datos de usuario (expresiones regulares).
    /// </summary>
    public static class Validaciones
    {
        /// <summary>
        /// Valida si una cadena de texto representa una dirección de correo electrónico válida perteneciente a dominios permitidos (@gmail.com, @hotmail.com, @outlook.com).
        /// </summary>
        /// <param name="email">Cadena que representa el correo electrónico a evaluar.</param>
        /// <returns><c>true</c> si el correo cumple con el formato y los dominios autorizados; de lo contrario, <c>false</c>.</returns>
        public static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string patronEmail = @"^[a-z0-9]+(\.[a-z0-9_]+)*@(gmail|hotmail|outlook)\.com$";
            return Regex.IsMatch(email, patronEmail, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Valida si una cadena de texto contiene únicamente letras del alfabeto español (incluyendo vocales acentuadas, diéresis y la letra eñe) y espacios en blanco.
        /// </summary>
        /// <param name="nombre">Texto correspondiente a nombres o apellidos.</param>
        /// <returns><c>true</c> si el texto contiene caracteres alfabéticos válidos y espacios; <c>false</c> si contiene números, símbolos o caracteres especiales.</returns>
        public static bool EsNombreValido(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return false;

            string patronNombre = @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$";
            return Regex.IsMatch(nombre, patronNombre);
        }
    }
}
