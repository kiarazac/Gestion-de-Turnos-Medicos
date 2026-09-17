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

        public List<UsuarioListadoDTO> ObtenerUsuarios(bool incluirInactivos = false)
        {
            return _usuarioDAL.ListarUsuarios(incluirInactivos);
        }

        public UsuarioListadoDTO? ObtenerUsuarioPorDni(string dni, bool incluirInactivos = true)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return null;

            return _usuarioDAL.ObtenerUsuarioPorDni(dni.Trim(), incluirInactivos);
        }

        /// <summary>
        /// Da de baja lógica a un usuario del sistema previa validación de reglas de negocio fundamentales:
        /// - No se permite desactivar cuentas inválidas (ID <= 0).
        /// - La cuenta administradora principal ('admin@gmail.com') cuenta con inmunidad absoluta del sistema y no puede ser desactivada.
        /// </summary>
        /// <param name="idUsuario">ID único del usuario a desactivar.</param>
        public void EliminarUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario proporcionado no es válido.");

            // Regla de inmunidad: Bloquear desactivación de la cuenta administradora principal
            var usuarios = _usuarioDAL.ListarUsuarios(incluirInactivos: false);
            var usuario = usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
            if (usuario != null && usuario.Correo.Trim().Equals("admin@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("La cuenta administradora principal ('admin@gmail.com') posee inmunidad y no puede ser desactivada del sistema.");
            }

            _usuarioDAL.EliminarUsuario(idUsuario);
        }

        /// <summary>
        /// Reactiva o re-da de alta lógicamente a un usuario previamente dado de baja (Activo = 0).
        /// </summary>
        /// <param name="idUsuario">ID del usuario a reactivar.</param>
        public void ReactivarUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario proporcionado no es válido.");

            _usuarioDAL.ReactivarUsuario(idUsuario);
        }

        /// <summary>
        /// Reactiva lógicamente a un usuario y aplica inmediatamente la actualización de sus datos personales,
        /// rol, matrícula médica, especialidades y asignación de salas en una sola operación controlada.
        /// </summary>
        public void ReactivarUsuario(
            int idUsuario,
            string nombre,
            string apellido,
            string correo,
            string dni,
            string telefono,
            string? matricula,
            int? idRol,
            List<int>? salasIds,
            string? descripcionAtencion,
            List<int>? especialidadesIds,
            string? nuevaContrasena = null)
        {
            // 1. Primero reactivamos lógicamente al usuario en la base de datos
            ReactivarUsuario(idUsuario);

            // 2. Con el usuario ya activo, aplicamos las modificaciones solicitadas
            ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, matricula, idRol, salasIds, descripcionAtencion, especialidadesIds, nuevaContrasena);
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
        /// Soporta la actualización de matrícula médica, salas y especialidades asignadas, así como cambio opcional de contraseña.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario a modificar.</param>
        /// <param name="nombre">Nombre actualizado del usuario.</param>
        /// <param name="apellido">Apellido actualizado del usuario.</param>
        /// <param name="correo">Correo electrónico actualizado del usuario.</param>
        /// <param name="dni">DNI actualizado del usuario.</param>
        /// <param name="telefono">Teléfono actualizado de contacto.</param>
        /// <param name="matricula">Matrícula médica actualizada (aplica a personal médico).</param>
        /// <param name="idRol">ID del rol asignado (opcional si es nulo o menor o igual a 0).</param>
        /// <param name="salasIds">Lista de IDs de salas asignadas (null = no modificar, vacía = desasignar todas, con elementos = reasignar salas).</param>
        /// <param name="descripcionAtencion">Notas u observaciones sobre la atención en la sala.</param>
        /// <param name="especialidadesIds">Lista de IDs de especialidades médicas (null = no modificar, vacía = desasignar todas, con elementos = reasignar especialidades).</param>
        /// <param name="nuevaContrasena">Nueva contraseña en texto plano para ser hasheada de forma segura (opcional).</param>
        public void ModificarUsuario(
            int idUsuario,
            string nombre,
            string apellido,
            string correo,
            string dni,
            string telefono,
            string? matricula,
            int? idRol,
            List<int>? salasIds,
            string? descripcionAtencion,
            List<int>? especialidadesIds,
            string? nuevaContrasena = null)
        {
            // 1. Validaciones de reglas de negocio fundamentales
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario proporcionado no es válido.");

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
                throw new ArgumentException("El nombre y el apellido son campos obligatorios.");

            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("El correo electrónico es un campo obligatorio.");

            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("El DNI es un campo obligatorio.");

            // 2. Hasheo seguro de la nueva clave si fue provista por el administrador
            string? passHash = null;
            if (!string.IsNullOrWhiteSpace(nuevaContrasena))
            {
                passHash = Seguridad.HashearContrasenia(nuevaContrasena);
            }

            // 3. Delegamos la persistencia atómica a la Capa de Datos (DAL)
            _usuarioDAL.ModificarUsuario(
                idUsuario,
                nombre.Trim(),
                apellido.Trim(),
                correo.Trim(),
                dni.Trim(),
                telefono?.Trim() ?? string.Empty,
                matricula?.Trim(),
                idRol,
                salasIds,
                descripcionAtencion?.Trim(),
                especialidadesIds,
                passHash);
        }

        /// <summary>
        /// Sobrecarga básica para modificar datos personales y rol del usuario (8 parámetros).
        /// Compatible con llamadas directas desde grillas o formularios legados.
        /// </summary>
        public void ModificarUsuario(int idUsuario, string nombre, string apellido, string correo, string dni, string telefono, string? matricula, int? idRol)
        {
            ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, matricula, idRol, (List<int>?)null, null, null, null);
        }

        /// <summary>
        /// Sobrecarga de compatibilidad para modificar usuario enviando salas sin especialidades ni contraseña.
        /// </summary>
        public void ModificarUsuario(int idUsuario, string nombre, string apellido, string correo, string dni, string telefono, string? matricula, int? idRol, List<int>? salasIds, string? descripcionAtencion = null)
        {
            ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, matricula, idRol, salasIds, descripcionAtencion, null, null);
        }

        /// <summary>
        /// Sobrecarga de compatibilidad para modificar usuario enviando un único ID de sala.
        /// </summary>
        public void ModificarUsuario(int idUsuario, string nombre, string apellido, string correo, string dni, string telefono, string? matricula, int? idRol, int? idSala, string? descripcionAtencion = null)
        {
            List<int>? salasIds = null;
            if (idSala.HasValue)
            {
                salasIds = new List<int>();
                if (idSala.Value > 0)
                    salasIds.Add(idSala.Value);
            }

            ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, matricula, idRol, salasIds, descripcionAtencion, null, null);
        }
    }
}