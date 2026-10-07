using System;
using System.Collections.Generic;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio para la gestión y consulta de obras sociales y medicinas prepagas.
    /// </summary>
    public class ObraSocialBLL
    {
        private readonly ObraSocialDAL _obraSocialDAL = new ObraSocialDAL();

        /// <summary>
        /// Obtiene el listado de obras sociales y coberturas médicas disponibles para la asignación a pacientes.
        /// </summary>
        /// <param name="incluirInactivas">Indica si se deben incluir coberturas dadas de baja lógica.</param>
        /// <returns>Lista de <see cref="ObraSocialDTO"/> ordenadas para su presentación.</returns>
        public List<ObraSocialDTO> ObtenerObrasSociales(bool incluirInactivas = false)
        {
            return _obraSocialDAL.ListarObrasSociales(incluirInactivas);
        }

        /// <summary>
        /// Valida las reglas de dominio y registra una nueva obra social o medicina prepaga en el sistema.
        /// </summary>
        /// <param name="nombre">Nombre oficial de la obra social (máximo 100 caracteres).</param>
        /// <param name="sigla">Sigla o acrónimo opcional (máximo 20 caracteres).</param>
        /// <exception cref="ArgumentException">Se lanza si las validaciones de longitud o datos requeridos fallan.</exception>
        public void RegistrarObraSocial(string nombre, string? sigla)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la obra social es obligatorio y no puede estar vacío.");

            string nombreLimpio = nombre.Trim();
            if (nombreLimpio.Length > 100)
                throw new ArgumentException("El nombre de la obra social no puede exceder los 100 caracteres.");

            string? siglaLimpia = string.IsNullOrWhiteSpace(sigla) ? null : sigla.Trim().ToUpperInvariant();
            if (siglaLimpia != null && siglaLimpia.Length > 20)
                throw new ArgumentException("La sigla no puede exceder los 20 caracteres.");

            _obraSocialDAL.InsertarObraSocial(nombreLimpio, siglaLimpia);
        }

        /// <summary>
        /// Aplica reglas de negocio y delega en DAL la modificación de los datos de una obra social existente.
        /// </summary>
        /// <param name="idObraSocial">Identificador único de la obra social.</param>
        /// <param name="nombre">Nuevo nombre asignado.</param>
        /// <param name="sigla">Nueva sigla o acrónimo.</param>
        /// <exception cref="ArgumentException">Se lanza si el ID es inválido o no cumple las restricciones.</exception>
        public void ModificarObraSocial(int idObraSocial, string nombre, string? sigla)
        {
            if (idObraSocial <= 0)
                throw new ArgumentException("El ID de la obra social no es válido.");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la obra social no puede estar vacío.");

            string nombreLimpio = nombre.Trim();
            if (nombreLimpio.Length > 100)
                throw new ArgumentException("El nombre de la obra social no puede superar los 100 caracteres.");

            if (idObraSocial == 1 && !nombreLimpio.ToUpperInvariant().Contains("PARTICULAR"))
                throw new ArgumentException("No se puede modificar la denominación base de la cobertura Particular requerida por el sistema.");

            string? siglaLimpia = string.IsNullOrWhiteSpace(sigla) ? null : sigla.Trim().ToUpperInvariant();
            if (siglaLimpia != null && siglaLimpia.Length > 20)
                throw new ArgumentException("La sigla no puede superar los 20 caracteres.");

            _obraSocialDAL.ModificarObraSocial(idObraSocial, nombreLimpio, siglaLimpia);
        }

        /// <summary>
        /// Aplica la baja lógica a una obra social en el catálogo del sistema.
        /// Protege la cobertura base Particular contra borrado.
        /// </summary>
        /// <param name="idObraSocial">Identificador único de la obra social a desactivar.</param>
        /// <exception cref="ArgumentException">Se lanza si se intenta dar de baja la cobertura Particular o el ID es inválido.</exception>
        public void EliminarObraSocial(int idObraSocial)
        {
            if (idObraSocial <= 0)
                throw new ArgumentException("El ID de la obra social proporcionado no es válido.");

            if (idObraSocial == 1)
                throw new ArgumentException("No es posible dar de baja la cobertura Particular / Sin Obra Social requerida por el sistema.");

            _obraSocialDAL.EliminarObraSocial(idObraSocial);
        }

        /// <summary>
        /// Valida el identificador y delega en DAL la reactivación lógica de una cobertura médica inactiva.
        /// </summary>
        /// <param name="idObraSocial">Identificador de la obra social a reactivar.</param>
        /// <exception cref="ArgumentException">Se lanza si el ID es menor o igual a cero.</exception>
        public void ReactivarObraSocial(int idObraSocial)
        {
            if (idObraSocial <= 0)
                throw new ArgumentException("El ID de la obra social no es válido para reactivación.");

            _obraSocialDAL.ReactivarObraSocial(idObraSocial);
        }
    }
}
