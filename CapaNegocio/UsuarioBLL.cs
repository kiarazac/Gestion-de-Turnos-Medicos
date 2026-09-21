using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio para la gestión de usuarios, autenticación, control de perfiles y roles.
    /// </summary>
    public class UsuarioBLL
    {
        private readonly UsuarioDAL _usuarioDAL = new UsuarioDAL();

        /// <summary>
        /// Valida las credenciales de acceso de un usuario comprobando reglas de negocio iniciales y delegando en DAL.
        /// </summary>
        /// <param name="correo">Correo electrónico ingresado por el usuario.</param>
        /// <param name="contrasena">Contraseña ingresada en texto plano.</param>
        /// <returns>Objeto <see cref="UsuarioLoginResult"/> con la información del usuario autenticado si es válido y está activo; de lo contrario, <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Se lanza si el correo o la contraseña están vacíos o contienen únicamente espacios en blanco.</exception>
        public UsuarioLoginResult Login(string correo, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                throw new ArgumentException("El correo y la contraseña son campos obligatorios.");
            }

            return _usuarioDAL.ValidarLogin(correo, contrasena);
        }

        /// <summary>
        /// Obtiene el catálogo completo de roles activos disponibles en el sistema.
        /// </summary>
        /// <returns>Lista de objetos <see cref="RolDTO"/> con los identificadores y nombres de roles.</returns>
        public List<RolDTO> ObtenerRoles()
        {
            return _usuarioDAL.ListarRoles();
        }

        /// <summary>
        /// Obtiene la nómina de usuarios con rol de Personal Médico activos en el sistema.
        /// </summary>
        /// <returns>Lista de objetos <see cref="MedicoDTO"/> para poblar combos y asignaciones de atención.</returns>
        public List<MedicoDTO> ObtenerPersonalMedico()
        {
            return _usuarioDAL.ListarPersonalMedico();
        }

        /// <summary>
        /// Método de conveniencia que reenvía la consulta a <see cref="ObtenerPersonalMedico"/>.
        /// </summary>
        /// <returns>Lista de profesionales médicos registrados.</returns>
        public List<MedicoDTO> ObtenerMedicos()
        {
            return ObtenerPersonalMedico();
        }

        /// <summary>
        /// Obtiene la lista de usuarios del sistema con opción de incluir aquellos dados de baja lógica.
        /// </summary>
        /// <param name="incluirInactivos">Indica si se deben incorporar usuarios con estado inactivo (<c>Activo = 0</c>).</param>
        /// <returns>Lista de objetos <see cref="UsuarioListadoDTO"/> con los datos completos de los usuarios.</returns>
        public List<UsuarioListadoDTO> ObtenerUsuarios(bool incluirInactivos = false)
        {
            return _usuarioDAL.ListarUsuarios(incluirInactivos);
        }

        /// <summary>
        /// Busca y retorna la ficha de un usuario a partir de su Documento Nacional de Identidad (DNI).
        /// </summary>
        /// <param name="dni">Número de documento a consultar.</param>
        /// <param name="incluirInactivos">Indica si se deben buscar también usuarios inactivos.</param>
        /// <returns>Instancia de <see cref="UsuarioListadoDTO"/> si se encuentra coincidencia; de lo contrario, <c>null</c>.</returns>
        public UsuarioListadoDTO? ObtenerUsuarioPorDni(string dni, bool incluirInactivos = true)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return null;

            return _usuarioDAL.ObtenerUsuarioPorDni(dni.Trim(), incluirInactivos);
        }

        /// <summary>
        /// Aplica la baja lógica a un usuario del sistema previa comprobación de reglas de negocio:
        /// el identificador debe ser válido y la cuenta administradora raíz ('admin@gmail.com') cuenta con inmunidad absoluta.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario a desactivar.</param>
        /// <exception cref="ArgumentException">Se lanza si el ID es menor o igual a cero.</exception>
        /// <exception cref="InvalidOperationException">Se lanza si se intenta dar de baja la cuenta del administrador raíz.</exception>
        public void EliminarUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario proporcionado no es válido.");

            var usuarios = _usuarioDAL.ListarUsuarios(incluirInactivos: false);
            var usuario = usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
            if (usuario != null && usuario.Correo.Trim().Equals("admin@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("La cuenta administradora principal ('admin@gmail.com') posee inmunidad y no puede ser desactivada del sistema.");
            }

            _usuarioDAL.EliminarUsuario(idUsuario);
        }

        /// <summary>
        /// Reactiva lógicamente a un usuario que se encontraba dado de baja (restablece <c>Activo = 1</c>).
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario a reactivar.</param>
        /// <exception cref="ArgumentException">Se lanza si el ID es menor o igual a cero.</exception>
        public void ReactivarUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario proporcionado no es válido.");

            _usuarioDAL.ReactivarUsuario(idUsuario);
        }

        /// <summary>
        /// Reactiva lógicamente a un usuario previamente inactivo y actualiza inmediatamente sus datos personales,
        /// rol, matrícula médica, especialidades y salas asignadas en una única operación atómica.
        /// </summary>
        /// <param name="idUsuario">ID único del usuario.</param>
        /// <param name="nombre">Nombre actualizado.</param>
        /// <param name="apellido">Apellido actualizado.</param>
        /// <param name="correo">Correo electrónico actualizado.</param>
        /// <param name="dni">DNI actualizado.</param>
        /// <param name="telefono">Teléfono actualizado.</param>
        /// <param name="matricula">Matrícula médica actualizada.</param>
        /// <param name="idRol">ID del rol asignado.</param>
        /// <param name="salasIds">Lista de salas asignadas.</param>
        /// <param name="descripcionAtencion">Notas de atención de sala.</param>
        /// <param name="especialidadesIds">Lista de especialidades asignadas.</param>
        /// <param name="nuevaContrasena">Nueva contraseña (opcional).</param>
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
            ReactivarUsuario(idUsuario);
            ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, matricula, idRol, salasIds, descripcionAtencion, especialidadesIds, nuevaContrasena);
        }

        /// <summary>
        /// Sobrecarga simplificada para registrar un nuevo usuario con sus especialidades médicas asociadas.
        /// </summary>
        public void RegistrarUsuario(string nombre, string apellido, string correo, string contrasena, string dni, string telefono, string nroMatricula, int idRol, List<int> especialidadesIds)
        {
            RegistrarUsuario(nombre, apellido, correo, contrasena, dni, telefono, idRol, nroMatricula, especialidadesIds, new List<int>(), string.Empty);
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema con validación de obligatoriedad de campos, asignación de rol,
        /// especialidades médicas y consultorios designados.
        /// </summary>
        /// <param name="nombre">Nombre de pila.</param>
        /// <param name="apellido">Apellido.</param>
        /// <param name="correo">Correo electrónico único.</param>
        /// <param name="contrasena">Contraseña de acceso.</param>
        /// <param name="dni">Número de DNI.</param>
        /// <param name="telefono">Teléfono de contacto.</param>
        /// <param name="idRol">Identificador de rol seleccionado.</param>
        /// <param name="matricula">Número de matrícula si es médico.</param>
        /// <param name="especialidadesIds">Listado de identificadores de especialidades a vincular.</param>
        /// <param name="salasIds">Listado de identificadores de salas a asignar.</param>
        /// <param name="notaSala">Observación sobre el consultorio asignado.</param>
        /// <exception cref="ArgumentException">Se lanza si alguno de los campos requeridos está vacío o si el rol no es válido.</exception>
        public void RegistrarUsuario(string nombre, string apellido, string correo, string contrasena, string dni, string telefono, int idRol, string matricula, List<int> especialidadesIds, List<int> salasIds, string notaSala)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Los campos nombre, apellido, correo y DNI son obligatorios.");

            if (idRol <= 0)
                throw new ArgumentException("Debe seleccionar un rol válido.");

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
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario proporcionado no es válido.");

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
                throw new ArgumentException("El nombre y el apellido son campos obligatorios.");

            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("El correo electrónico es un campo obligatorio.");

            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("El DNI es un campo obligatorio.");

            string? passHash = null;
            if (!string.IsNullOrWhiteSpace(nuevaContrasena))
            {
                passHash = Seguridad.HashearContrasenia(nuevaContrasena);
            }

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
        /// Sobrecarga básica para modificar datos personales y rol del usuario.
        /// Compatible con llamadas directas desde grillas o formularios de gestión.
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