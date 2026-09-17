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

        public List<MedicoDTO> ObtenerMedicos()
        {
            return ObtenerPersonalMedico();
        }

        public List<UsuarioListadoDTO> ObtenerUsuarios()
        {
            return _usuarioDAL.ListarUsuarios();
        }

        public void EliminarUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario proporcionado no es válido.");

            _usuarioDAL.EliminarUsuario(idUsuario);
        }

        public void RegistrarUsuario(string nombre, string apellido, string correo, string contrasena, string dni, string telefono, string nroMatricula, int idRol, List<int> especialidadesIds)
        {
            RegistrarUsuario(nombre, apellido, correo, contrasena, dni, telefono, idRol, nroMatricula, especialidadesIds, new List<int>(), string.Empty);
        }

        public void RegistrarUsuario(string nombre, string apellido, string correo, string contrasena, string dni, string telefono, int idRol, string matricula, List<int> especialidadesIds, List<int> salasIds, string notaSala)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Los campos nombre, apellido, correo y DNI son obligatorios.");

            if (idRol <= 0)
                throw new ArgumentException("Debe seleccionar un rol válido.");

            // Se asume que la contraseña ya viene provista desde la UI
            int nuevoUsuarioId = _usuarioDAL.InsertarUsuario(nombre, apellido, correo, contrasena, dni, telefono, matricula, idRol);

            if (especialidadesIds != null && especialidadesIds.Count > 0)
            {
                foreach (int idEspecialidad in especialidadesIds)
                {
                    _usuarioDAL.AsignarEspecialidadMedico(nuevoUsuarioId, idEspecialidad);
                }
            }

            if (salasIds != null && salasIds.Count > 0)
            {
                foreach (int idSala in salasIds)
                {
                    // Verificar que el orden de envío coincida con la firma del método en la DAL
                    _usuarioDAL.AsignarSalaMedico(idSala, nuevoUsuarioId, notaSala);
                }
            }
        }

        /// <summary>
        /// Modifica los datos de un usuario existente aplicando validaciones de dominio y delegando la persistencia en UsuarioDAL.
        /// Soporta la actualización de matrícula médica y una o múltiples salas asignadas.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario a modificar.</param>
        /// <param name="nombre">Nombre actualizado del usuario.</param>
        /// <param name="apellido">Apellido actualizado del usuario.</param>
        /// <param name="correo">Correo electrónico actualizado del usuario.</param>
        /// <param name="dni">DNI actualizado del usuario.</param>
        /// <param name="telefono">Teléfono actualizado de contacto.</param>
        /// <param name="matricula">Matrícula médica actualizada (aplica a personal médico).</param>
        /// <param name="idRol">ID del rol asignado (opcional si es nulo o menor o igual a 0).</param>
        /// <param name="salasIds">Lista de IDs de salas asignadas (null = no modificar, vacía = desasignar todas, con elementos = asignar múltiples salas).</param>
        /// <param name="descripcionAtencion">Notas u observaciones sobre la atención en la sala.</param>
        public void ModificarUsuario(int idUsuario, string nombre, string apellido, string correo, string dni, string telefono, string? matricula, int? idRol, List<int>? salasIds, string? descripcionAtencion = null)
        {
            // 1. Validaciones de negocio fundamentales
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario proporcionado no es válido.");

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
                throw new ArgumentException("El nombre y el apellido son campos obligatorios.");

            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("El correo electrónico es un campo obligatorio.");

            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("El DNI es un campo obligatorio.");

            // 2. Delegamos la persistencia a la Capa de Datos (DAL -> sp_ModificarUsuario con soporte de múltiples salas)
            _usuarioDAL.ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, matricula, idRol, salasIds, descripcionAtencion);
        }

        /// <summary>
        /// Sobrecarga de compatibilidad para modificar usuario enviando un único ID de sala.
        /// </summary>
        public void ModificarUsuario(int idUsuario, string nombre, string apellido, string correo, string dni, string telefono, string? matricula, int? idRol, int? idSala = null, string? descripcionAtencion = null)
        {
            List<int>? salasIds = null;
            if (idSala.HasValue)
            {
                salasIds = new List<int>();
                if (idSala.Value > 0)
                    salasIds.Add(idSala.Value);
            }

            ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, matricula, idRol, salasIds, descripcionAtencion);
        }
    }
}