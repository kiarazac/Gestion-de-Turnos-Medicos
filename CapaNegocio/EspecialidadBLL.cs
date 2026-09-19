using System;
using System.Collections.Generic;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    // Esta capa procesa las validaciones de negocio antes de permitir que la información llegue a la base de datos[cite: 2].
    public class EspecialidadBLL
    {
        private readonly EspecialidadDAL _especialidadDAL = new EspecialidadDAL();

        // Método invocado por la UI para llenar listas desplegables o tablas[cite: 2].
        // Soporta el parámetro opcional incluirInactivas para administración y auditoría de bajas.
        public List<EspecialidadDTO> ObtenerEspecialidades(bool incluirInactivas = false)
        {
            return _especialidadDAL.ListarEspecialidades(incluirInactivas);
        }

        // Método invocado al querer crear una especialidad.
        public void RegistrarEspecialidad(string nombre)
        {
            // Validación de dominio: evitamos que se intente guardar una especialidad con nombre en blanco[cite: 2].
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

        // Método invocado para la baja de una especialidad.
        public void EliminarEspecialidad(int idEspecialidad)
        {
            // Verificamos que el ID sea coherente antes de ejecutar comandos en la base de datos[cite: 2].
            if (idEspecialidad <= 0)
                throw new ArgumentException("El ID de la especialidad proporcionado no es válido.");

            _especialidadDAL.EliminarEspecialidad(idEspecialidad);
        }

        /// <summary>
        /// Valida el identificador y delega en DAL la reactivación lógica de la especialidad y sus asignaciones médicas.
        /// </summary>
        public void ReactivarEspecialidad(int idEspecialidad)
        {
            if (idEspecialidad <= 0)
                throw new ArgumentException("El ID de la especialidad no es válido para reactivación.");

            _especialidadDAL.ReactivarEspecialidad(idEspecialidad);
        }

        public List<EspecialidadDTO> ObtenerEspecialidadesPorMedico(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID del usuario médico no es válido.");

            return _especialidadDAL.ObtenerEspecialidadesPorMedico(idUsuario);
        }
    }
}