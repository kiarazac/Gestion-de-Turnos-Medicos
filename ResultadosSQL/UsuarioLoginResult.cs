using System;
using System.Collections.Generic;
using System.Text;

namespace Gestion_de_Turnos_Medicos.ResultadosSQL
{
    public class UsuarioLoginResult //Guarda los datos devueltos por la consulta de login del usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
    }
}
