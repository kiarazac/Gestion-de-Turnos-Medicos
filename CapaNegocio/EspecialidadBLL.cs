using System;
using System.Collections.Generic;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio para la gestión y validación de especialidades médicas.
    /// Aplica restricciones de formato, longitud máxima de denominaciones y control de borrado/reactivación lógica.
    /// </summary>
    public class EspecialidadBLL
    {
        private readonly EspecialidadDAL _especialidadDAL = new EspecialidadDAL();

        /// <summary>
        /// Obtiene el catálogo de especialidades médicas registradas, con soporte opcional para incluir especialidades inactivas.
        /// </summary>
        /// <param name="incluirInactivas">Indica si se deben listar especialidades dadas de baja lógica (<c>Activo = 0</c>).</param>
        /// <returns>Lista de <see cref="EspecialidadDTO"/>.</returns>
        public List<EspecialidadDTO> ObtenerEspecialidades(bool incluirInactivas = false)
        {
            return _especialidadDAL.ListarEspecialidades(incluirInactivas);
        }

        /// <summary>
        /// Valida las reglas de dominio médico y registra una nueva especialidad en el sistema.
        /// </summary>
        /// <param name="nombre">Nombre de la especialidad (máximo 100 caracteres, solo letras y espacios).</param>
        /// <exception cref="ArgumentException">Se lanza si el nombre está vacío, excede la longitud o contiene caracteres no permitidos.</exception>
        public void RegistrarEspecialidad(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la especialidad es obligatorio y no puede estar vacío.");

            if (nombre.Trim().Length > 100)
                throw new ArgumentException("El nombre de la especialidad no puede exceder los 100 caracteres.");

            if (!Validaciones.EsNombreValido(nombre.Trim()))
                throw new ArgumentException("El nombre de la especialidad solo debe contener letras y espacios.");

            _especialidadDAL.InsertarEspecialidad(nombre.Trim());
        }

        /// <summary>
        /// Aplica reglas de negocio y delega en DAL la actualización del nombre de una especialidad existente.
        /// </summary>
        /// <param name="idEspecialidad">Identificador único de la especialidad a modificar.</param>
        /// <param name="nombre">Nuevo nombre de la especialidad.</param>
        /// <exception cref="ArgumentException">Se lanza si el ID es inválido o el nombre no cumple las reglas de formato.</exception>
        public void ModificarEspecialidad(int idEspecialidad, string nombre)
        {
            if (idEspecialidad <= 0)
                throw new ArgumentException("El ID de la especialidad no es válido.");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la especialidad no puede estar vacío.");

            if (nombre.Trim().Length > 100)
                throw new ArgumentException("El nombre de la especialidad no puede superar los 100 caracteres.");

            if (!Validaciones.EsNombreValido(nombre.Trim()))
                throw new ArgumentException("El nombre de la especialidad solo debe contener letras y espacios.");

            _especialidadDAL.ModificarEspecialidad(idEspecialidad, nombre.Trim());
        }

        /// <summary>
        /// Aplica la baja lógica a una especialidad médica del sistema.
        /// </summary>
        /// <param name="idEspecialidad">Identificador único de la especialidad a dar de baja.</param>
        /// <exception cref="ArgumentException">Se lanza si el ID de especialidad es menor o igual a cero.</exception>
        public void EliminarEspecialidad(int idEspecialidad)
        {
            if (idEspecialidad <= 0)
                throw new ArgumentException("El ID de la especialidad proporcionado no es válido.");

            _especialidadDAL.EliminarEspecialidad(idEspecialidad);
        }

        /// <summary>
        /// Valida el identificador y delega en DAL la reactivación lógica de la especialidad y sus asignaciones médicas.
        /// </summary>
        /// <param name="idEspecialidad">Identificador de la especialidad a reactivar.</param>
        /// <exception cref="ArgumentException">Se lanza si el ID de la especialidad es inválido.</exception>
        public void ReactivarEspecialidad(int idEspecialidad)
        {
            if (idEspecialidad <= 0)
                throw new ArgumentException("El ID de la especialidad no es válido para reactivación.");

            _especialidadDAL.ReactivarEspecialidad(idEspecialidad);
        }

        /// <summary>
        /// Obtiene la lista de especialidades asociadas y habilitadas para un profesional médico específico.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario médico.</param>
        /// <returns>Lista de <see cref="EspecialidadDTO"/> asociadas al médico.</returns>
        /// <exception cref="ArgumentException">Se lanza si el ID del médico es inválido.</exception>
        public List<EspecialidadDTO> ObtenerEspecialidadesPorMedico(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID del usuario médico no es válido.");

            return _especialidadDAL.ObtenerEspecialidadesPorMedico(idUsuario);
        }
    }
}