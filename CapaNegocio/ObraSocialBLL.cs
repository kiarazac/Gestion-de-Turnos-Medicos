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
    }
}
