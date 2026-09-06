using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System;
using System.Collections.Generic;
using System.Text;


namespace Gestion_de_Turnos_Medicos.Negocio
{
    public class UsuarioBLL
    {
        private UsuarioDAL _usuarioDAL = new UsuarioDAL();

        public UsuarioLoginResult Login(string correo, string contrasena)
        {
            // Regla de negocio básica: evitar que viajen a la BD datos vacíos
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                throw new ArgumentException("El correo y la contraseña son campos obligatorios.");
            }

            // Si todo está bien, le pasamos la pelota a la Capa de Datos
            return _usuarioDAL.ValidarLogin(correo, contrasena);
        }
    }
}