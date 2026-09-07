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

        public List<RolDTO> ObtenerRoles()
        {
            return _usuarioDAL.ListarRoles();
        }

        public List<MedicoDTO> ObtenerPersonalMedico()
        {
            return _usuarioDAL.ListarPersonalMedico();
        }

        public void RegistrarUsuario(string nombre, string apellido, string correo, string contrasena, string dni, string telefono, string nroMatricula, int idRol, List<int> especialidadesIds)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Los campos nombre, apellido, correo y DNI son obligatorios.");

            if (idRol <= 0)
                throw new ArgumentException("Debe seleccionar un rol válido.");

            // Se asume que la contraseña ya viene hasheada desde la UI o se aplica el método de seguridad aquí
            int nuevoUsuarioId = _usuarioDAL.InsertarUsuario(nombre, apellido, correo, contrasena, dni, telefono, nroMatricula, idRol);

            if (especialidadesIds != null && especialidadesIds.Count > 0)
            {
                foreach (int idEspecialidad in especialidadesIds)
                {
                    _usuarioDAL.AsignarEspecialidadMedico(nuevoUsuarioId, idEspecialidad);
                }
            }
        }
    }
}