using Gestion_de_Turnos_Medicos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gestion_de_Turnos_Medicos 
{
    public class Validaciones
    {
        // Método para validar correos de Gmail, Hotmail u Outlook
        public static bool EsEmailValido(string email)
        {
            string patronEmail = @"^[a-z0-9]+(\.[a-z0-9_]+)*@(gmail|hotmail|outlook)\.com$";
            return Regex.IsMatch(email, patronEmail, RegexOptions.IgnoreCase);
        }

        // Método para validar que el nombre solo tenga letras (sin números ni símbolos)
        public static bool EsNombreValido(string nombre)
        {
            string patronNombre = @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$";
            return Regex.IsMatch(nombre, patronNombre);
        }
    }
}

