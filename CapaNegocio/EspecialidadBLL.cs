using System;
using System.Collections.Generic;
using CapaDeDatos;
using ResultadosSQL;

namespace CapaNegocio
{
    // Esta capa procesa las validaciones de negocio antes de permitir que la información llegue a la base de datos[cite: 2].
    public class EspecialidadBLL
    {
        private readonly EspecialidadDAL _especialidadDAL = new EspecialidadDAL();

        // Método invocado por la UI para llenar listas desplegables o tablas[cite: 2].
        public List<EspecialidadDTO> ObtenerEspecialidades()
        {
            return _especialidadDAL.ListarEspecialidades();
        }

        // Método invocado al querer crear una especialidad.
        public void RegistrarEspecialidad(string nombre)
        {
            // Validación de dominio: evitamos que se intente guardar una especialidad con nombre en blanco[cite: 2].
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la especialidad es obligatorio y no puede estar vacío.");

            _especialidadDAL.InsertarEspecialidad(nombre.Trim());
        }

        // Método invocado para la baja de una especialidad.
        public void EliminarEspecialidad(int idEspecialidad)
        {
            // Verificamos que el ID sea coherente antes de ejecutar comandos en la base de datos[cite: 2].
            if (idEspecialidad <= 0)
                throw new ArgumentException("El ID de la especialidad proporcionado no es válido.");

            _especialidadDAL.EliminarEspecialidad(idEspecialidad);
        }
    }
}