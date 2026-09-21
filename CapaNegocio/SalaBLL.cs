using System;
using System.Collections.Generic;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio para la administración de salas y consultorios médicos,
    /// control de disponibilidad física, apertura y cierre de jornada, y asignación de profesionales de la salud.
    /// </summary>
    public class SalaBLL
    {
        private readonly SalaDAL _salaDAL = new SalaDAL();

        /// <summary>
        /// Obtiene el catálogo de salas físicas, permitiendo filtrar por profesional asignado e incluir salas dadas de baja lógica.
        /// </summary>
        /// <param name="idUsuario">Identificador opcional del médico para filtrar sus salas asignadas.</param>
        /// <param name="incluirInactivas">Indica si se deben incorporar salas desactivadas (<c>Activo = 0</c>).</param>
        /// <returns>Lista de objetos <see cref="SalaDTO"/>.</returns>
        public List<SalaDTO> ObtenerSalas(int? idUsuario = null, bool incluirInactivas = false)
        {
            return _salaDAL.ObtenerSalas(idUsuario, incluirInactivas);
        }

        /// <summary>
        /// Reactiva lógicamente una sala médica que se encontraba dada de baja en el sistema.
        /// </summary>
        /// <param name="idSala">Identificador único de la sala.</param>
        /// <exception cref="ArgumentException">Se lanza si el ID es menor o igual a cero.</exception>
        public void ReactivarSala(int idSala)
        {
            if (idSala <= 0)
                throw new ArgumentException("El identificador de la sala a reactivar no es válido.");

            _salaDAL.ReactivarSala(idSala);
        }

        /// <summary>
        /// Registra la apertura de una sala por parte de un profesional médico al iniciar su turno o jornada.
        /// </summary>
        /// <param name="idSala">Identificador de la sala a abrir.</param>
        /// <param name="idUsuario">Identificador del médico que toma posesión del consultorio.</param>
        /// <exception cref="ArgumentException">Se lanza si el identificador de sala o usuario es inválido.</exception>
        public void AbrirSala(int idSala, int idUsuario)
        {
            if (idSala <= 0 || idUsuario <= 0)
                throw new ArgumentException("Los identificadores de sala y usuario son requeridos para abrir la sala.");

            _salaDAL.AbrirSala(idSala, idUsuario);
        }

        /// <summary>
        /// Registra el cierre operativo de una sala al finalizar la atención médica o jornada de trabajo.
        /// </summary>
        /// <param name="idSala">Identificador de la sala a cerrar.</param>
        /// <exception cref="ArgumentException">Se lanza si el ID de sala es inválido.</exception>
        public void CerrarSala(int idSala)
        {
            if (idSala <= 0)
                throw new ArgumentException("El identificador de la sala es requerido.");

            _salaDAL.CerrarSala(idSala);
        }

        /// <summary>
        /// Da de alta una nueva sala en el establecimiento con asignación inmediata de los profesionales médicos vinculados.
        /// </summary>
        /// <param name="nombreSala">Nombre o número del consultorio.</param>
        /// <param name="estadoSala">Estado inicial de la sala (ej. 'Disponible', 'Ocupada').</param>
        /// <param name="idsMedicosSeleccionados">Listado opcional de identificadores de médicos a asignar.</param>
        /// <exception cref="ArgumentException">Se lanza si el nombre o el estado de la sala están vacíos.</exception>
        public void RegistrarSala(string nombreSala, string estadoSala, List<int> idsMedicosSeleccionados)
        {
            if (string.IsNullOrWhiteSpace(nombreSala))
                throw new ArgumentException("El nombre de la sala es obligatorio.");

            if (string.IsNullOrWhiteSpace(estadoSala))
                throw new ArgumentException("El estado de la sala es obligatorio.");

            int nuevaSalaId = _salaDAL.InsertarSala(nombreSala, estadoSala);

            if (idsMedicosSeleccionados != null && idsMedicosSeleccionados.Count > 0)
            {
                foreach (int idUsuario in idsMedicosSeleccionados)
                {
                    _salaDAL.AsignarSalaMedico(nuevaSalaId, idUsuario, string.Empty);
                }
            }
        }

        /// <summary>
        /// Sobrecarga para registrar una sala médica sin asignación de médicos iniciales.
        /// </summary>
        /// <param name="nombreSala">Nombre o número del consultorio.</param>
        /// <param name="estadoSala">Estado inicial de la sala.</param>
        public void RegistrarSala(string nombreSala, string estadoSala)
        {
            RegistrarSala(nombreSala, estadoSala, null);
        }

        /// <summary>
        /// Da de baja lógica a una sala médica del sistema.
        /// </summary>
        /// <param name="idSala">Identificador de la sala a dar de baja.</param>
        /// <exception cref="ArgumentException">Se lanza si el identificador es menor o igual a cero.</exception>
        public void EliminarSala(int idSala)
        {
            if (idSala <= 0)
                throw new ArgumentException("ID de sala inválido.");

            _salaDAL.EliminarSala(idSala);
        }

        /// <summary>
        /// Asocia un médico a una sala médica registrando el detalle u observaciones de la atención prestada.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idUsuario">Identificador del usuario profesional médico.</param>
        /// <param name="descripcionAtencion">Notas descriptivas de la atención o guardias.</param>
        /// <exception cref="ArgumentException">Se lanza si no se seleccionó una sala o un profesional válido.</exception>
        public void AsignarSalaMedico(int idSala, int idUsuario, string descripcionAtencion)
        {
            if (idSala <= 0 || idUsuario <= 0)
                throw new ArgumentException("Debe seleccionar una sala y un profesional médico.");

            _salaDAL.AsignarSalaMedico(idSala, idUsuario, descripcionAtencion);
        }

        /// <summary>
        /// Actualiza el estado operativo actual de una sala médica.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nuevoEstado">Nuevo estado operativo ('Disponible', 'Ocupada', 'En Mantenimiento').</param>
        /// <exception cref="ArgumentException">Se lanza si el ID es inválido o el estado está vacío.</exception>
        public void ActualizarEstadoSala(int idSala, string nuevoEstado)
        {
            if (idSala <= 0)
                throw new ArgumentException("El ID de la sala no es válido.");

            if (string.IsNullOrWhiteSpace(nuevoEstado))
                throw new ArgumentException("El nuevo estado de la sala es obligatorio.");

            _salaDAL.ActualizarEstadoSala(idSala, nuevoEstado);
        }

        /// <summary>
        /// Modifica una sala existente actualizando su denominación física, estado y reasignando los médicos seleccionados.
        /// </summary>
        /// <param name="idSala">ID único de la sala a modificar.</param>
        /// <param name="nombreSala">Nombre descriptivo actualizado de la sala.</param>
        /// <param name="estadoSala">Estado operativo actualizado.</param>
        /// <param name="idsMedicosSeleccionados">Lista opcional de IDs de médicos a reasignar.</param>
        /// <exception cref="ArgumentException">Se lanza si los datos son inválidos o faltan campos obligatorios.</exception>
        public void ModificarSala(int idSala, string nombreSala, string estadoSala, List<int>? idsMedicosSeleccionados = null)
        {
            if (idSala <= 0)
                throw new ArgumentException("El identificador de la sala no es válido.");

            if (string.IsNullOrWhiteSpace(nombreSala))
                throw new ArgumentException("El nombre de la sala es obligatorio.");

            if (string.IsNullOrWhiteSpace(estadoSala))
                throw new ArgumentException("El estado de la sala es obligatorio.");

            _salaDAL.ModificarSala(idSala, nombreSala.Trim(), estadoSala.Trim());

            if (idsMedicosSeleccionados != null)
            {
                _salaDAL.ReasignarMedicosASala(idSala, idsMedicosSeleccionados);
            }
        }
    }
}