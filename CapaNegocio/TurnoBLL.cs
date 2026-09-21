using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio para la gestión integral de turnos médicos (emergencias, triage, consultas programadas y flujo de atención).
    /// </summary>
    public class TurnoBLL
    {
        private readonly TurnoDAL _turnoDAL = new TurnoDAL();

        /// <summary>
        /// Obtiene el catálogo completo de síntomas médicos para la pantalla de triage.
        /// </summary>
        /// <returns>Lista de <see cref="SintomaDTO"/> disponibles en el sistema.</returns>
        public List<SintomaDTO> ObtenerSintomas()
        {
            return _turnoDAL.ObtenerSintomas();
        }

        /// <summary>
        /// Busca un paciente por su número de DNI aplicando validación de formato inicial.
        /// </summary>
        /// <param name="dni">Número de documento a consultar.</param>
        /// <returns>Objeto <see cref="PacienteDTO"/> si fue hallado; de lo contrario, <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Se lanza si el DNI es nulo o vacío.</exception>
        public PacienteDTO BuscarPacientePorDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Debe ingresar un DNI válido para la búsqueda.");

            return _turnoDAL.BuscarPacientePorDNI(dni.Trim());
        }

        /// <summary>
        /// Determina la prioridad de triage según los síntomas seleccionados, crea el turno de emergencia
        /// y persiste las relaciones intermedias con cada síntoma manifestado.
        /// </summary>
        /// <param name="idPaciente">Identificador único del paciente admitido.</param>
        /// <param name="idsSintomas">Lista de identificadores de síntomas tildados.</param>
        /// <param name="esOtroSeleccionado">Indica si el paciente seleccionó la opción de síntoma no catalogado ("Otro").</param>
        /// <returns>Código correlativo de turno asignado (ej. 'E-001').</returns>
        /// <exception cref="ArgumentException">Se lanza si el ID del paciente es inválido o no se especificó ningún síntoma.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error al persistir el turno en base de datos.</exception>
        public string CrearTurnoEmergenciaConSintomas(int idPaciente, List<int> idsSintomas, bool esOtroSeleccionado)
        {
            if (idPaciente <= 0)
                throw new ArgumentException("El ID del paciente es inválido o no ha sido registrado/cargado.");

            int prioridadDeterminada = 3; // Prioridad por defecto: 3 (Baja)

            if (esOtroSeleccionado)
            {
                prioridadDeterminada = 3; // Baja
            }
            else
            {
                if (idsSintomas == null || idsSintomas.Count == 0)
                    throw new ArgumentException("Debe registrar al menos un síntoma o marcar la opción 'Otro'.");

                foreach (int idSintoma in idsSintomas)
                {
                    int gravedadSintoma = _turnoDAL.ObtenerGravedadDeSintoma(idSintoma);
                    // La prioridad menor en número representa mayor urgencia médica (1 = Alta, 2 = Media, 3 = Baja)
                    if (gravedadSintoma < prioridadDeterminada)
                        prioridadDeterminada = gravedadSintoma;
                }
            }

            var resultadoTurno = _turnoDAL.CrearTurnoEmergenciaCompleto(idPaciente, prioridadDeterminada);

            if (resultadoTurno.IdNuevoTurno <= 0)
                throw new Exception("Error al generar el turno en la base de datos.");

            if (!esOtroSeleccionado && idsSintomas != null)
            {
                foreach (int idSintoma in idsSintomas)
                {
                    _turnoDAL.GuardarTurnoSintoma(resultadoTurno.IdNuevoTurno, idSintoma);
                }
            }

            return resultadoTurno.NroOrden;
        }

        /// <summary>
        /// Realiza el llamado a un paciente en espera asignándole el profesional médico y consultorio físico correspondiente.
        /// </summary>
        /// <param name="idTurno">Identificador del turno a llamar.</param>
        /// <param name="nombreMedico">Nombre del médico que realiza la llamada.</param>
        /// <param name="salaAsignada">Denominación de la sala o box de atención.</param>
        /// <exception cref="ArgumentException">Se lanza si falta información requerida para el llamado.</exception>
        public void LlamarSiguientePaciente(int idTurno, string nombreMedico, string salaAsignada)
        {
            if (idTurno <= 0 || string.IsNullOrWhiteSpace(nombreMedico) || string.IsNullOrWhiteSpace(salaAsignada))
                throw new ArgumentException("Se requieren todos los datos del turno y del profesional para llamar al paciente.");

            _turnoDAL.LlamarSiguientePaciente(idTurno, nombreMedico, salaAsignada);
        }

        /// <summary>
        /// Obtiene los horarios de atención disponibles para una especialidad y fecha específica.
        /// </summary>
        /// <param name="nombreEspecialidad">Nombre de la especialidad requerida.</param>
        /// <param name="fecha">Fecha solicitada para el turno.</param>
        /// <returns>Lista de <see cref="HorarioDisponibleDTO"/> con los turnos no ocupados.</returns>
        /// <exception cref="ArgumentException">Se lanza si el nombre de la especialidad está vacío.</exception>
        public List<HorarioDisponibleDTO> ObtenerHorariosDisponibles(string nombreEspecialidad, DateTime fecha)
        {
            if (string.IsNullOrWhiteSpace(nombreEspecialidad))
                throw new ArgumentException("Debe seleccionar una especialidad.");

            return _turnoDAL.ObtenerHorariosDisponibles(nombreEspecialidad, fecha);
        }

        /// <summary>
        /// Registra un nuevo turno programado por especialidad médica validando que la fecha sea igual o posterior al día actual.
        /// </summary>
        /// <param name="idPaciente">Identificador del paciente.</param>
        /// <param name="nombreEspecialidad">Nombre de la especialidad solicitada.</param>
        /// <param name="fecha">Fecha asignada para el turno.</param>
        /// <param name="horario">Horario de atención acordado.</param>
        /// <param name="estado">Estado inicial del turno (por defecto 'En Espera').</param>
        /// <returns>Objeto <see cref="ResultadoTurnoDTO"/> con el ID de turno y código de orden generado.</returns>
        /// <exception cref="ArgumentException">Se lanza si faltan datos o la fecha es anterior al día actual.</exception>
        public ResultadoTurnoDTO CrearTurnoEspecialidad(int idPaciente, string nombreEspecialidad, DateTime fecha, string horario, string estado = "En Espera")
        {
            if (idPaciente <= 0 || string.IsNullOrWhiteSpace(nombreEspecialidad) || string.IsNullOrWhiteSpace(horario))
                throw new ArgumentException("Todos los datos del turno programado son obligatorios.");

            if (fecha.Date < DateTime.Now.Date)
                throw new ArgumentException("No se pueden registrar turnos en fechas pasadas.");

            return _turnoDAL.CrearTurnoEspecialidad(idPaciente, nombreEspecialidad, fecha, horario, estado);
        }  

        /// <summary>
        /// Obtiene el listado de todos los turnos registrados bajo la modalidad de Emergencia / Triage.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoEmergenciaDTO"/>.</returns>
        public List<TurnoEmergenciaDTO> ListarTurnosEmergencia()
        {
            return _turnoDAL.ListarTurnosEmergencia();
        }

        /// <summary>
        /// Obtiene los turnos para la visualización pública general en pantallas informativas de sala de espera.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoGeneralPantallaDTO"/>.</returns>
        public List<TurnoGeneralPantallaDTO> ObtenerTurnosPantallaGeneral()
        {
            return _turnoDAL.ObtenerTurnosPantallaGeneral();
        }

        /// <summary>
        /// Obtiene el listado de turnos asociados a una especialidad médica específica.
        /// </summary>
        /// <param name="nombreEspecialidad">Nombre de la especialidad.</param>
        /// <returns>Lista de <see cref="TurnoListadoDTO"/>.</returns>
        /// <exception cref="ArgumentException">Se lanza si el nombre de la especialidad está en blanco.</exception>
        public List<TurnoListadoDTO> ListarTurnosEspecialidad(string nombreEspecialidad)
        {
            if (string.IsNullOrWhiteSpace(nombreEspecialidad))
                throw new ArgumentException("Debe indicar la especialidad a consultar.");

            return _turnoDAL.ListarTurnosEspecialidad(nombreEspecialidad);
        }

        /// <summary>
        /// Obtiene la lista parametrizada de turnos médicos filtrando opcionalmente por especialidad y estado operativo.
        /// </summary>
        /// <param name="idEspecialidad">Identificador de especialidad (opcional).</param>
        /// <param name="estado">Estado de turno a filtrar (opcional).</param>
        /// <returns>Lista de <see cref="TurnoListadoDTO"/>.</returns>
        public List<TurnoListadoDTO> ObtenerListaTurnos(int? idEspecialidad = null, string? estado = null)
        {
            return _turnoDAL.ObtenerListaTurnos(idEspecialidad, estado);
        }

        /// <summary>
        /// Obtiene la lista de turnos actualmente llamados o en proceso de atención para el panel médico.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoAtencionDTO"/>.</returns>
        public List<TurnoAtencionDTO> ListarTurnosAtencion()
        {
            return _turnoDAL.ListarTurnosAtencion();
        }

        /// <summary>
        /// Cambia el estado del turno a 'En Atencion' y confirma la sala de consulta.
        /// </summary>
        /// <param name="idTurno">Identificador del turno.</param>
        /// <param name="salaAsignada">Nombre de la sala donde se efectúa la atención.</param>
        /// <exception cref="ArgumentException">Se lanza si el ID de turno no es válido.</exception>
        public void IniciarAtencionTurno(int idTurno, string salaAsignada)
        {
            if (idTurno <= 0)
                throw new ArgumentException("El ID del turno no es válido.");

            _turnoDAL.IniciarAtencionTurno(idTurno, salaAsignada);
        }

        /// <summary>
        /// Finaliza la atención del turno médico, registrando el diagnóstico emitido y liberando la atención.
        /// </summary>
        /// <param name="idTurno">Identificador del turno.</param>
        /// <param name="diagnostico">Diagnóstico o conclusiones médicas.</param>
        /// <param name="nombreMedico">Nombre del profesional que atendió.</param>
        /// <param name="salaAsignada">Sala en la que se realizó la atención.</param>
        /// <exception cref="ArgumentException">Se lanza si falta información requerida para el cierre de la atención.</exception>
        public void FinalizarAtencionTurno(int idTurno, string diagnostico, string nombreMedico, string salaAsignada)
        {
            if (idTurno <= 0 || string.IsNullOrWhiteSpace(diagnostico) || string.IsNullOrWhiteSpace(nombreMedico) || string.IsNullOrWhiteSpace(salaAsignada))
                throw new ArgumentException("Faltan datos obligatorios para finalizar la atención.");

            _turnoDAL.FinalizarAtencionTurno(idTurno, diagnostico, nombreMedico, salaAsignada);
        }

        /// <summary>
        /// Obtiene los turnos llamados recientemente para ser mostrados en la pantalla pública de visualización.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoPantallaDTO"/>.</returns>
        public List<TurnoPantallaDTO> ObtenerTurnosPantallaPublica()
        {
            return _turnoDAL.ObtenerTurnosPantallaPublica();
        }

        /// <summary>
        /// Obtiene los turnos actualmente en estado 'En Espera' ordenados según la cola de atención de una especialidad.
        /// </summary>
        /// <param name="idEspecialidad">Identificador único de la especialidad.</param>
        /// <returns>Lista de <see cref="TurnoEsperaDTO"/>.</returns>
        /// <exception cref="ArgumentException">Se lanza si el ID de especialidad no es válido.</exception>
        public List<TurnoEsperaDTO> ObtenerTurnosEnEspera(int idEspecialidad)
        {
            if (idEspecialidad <= 0)
                throw new ArgumentException("Debe indicar una especialidad válida para consultar la cola de espera.");

            return _turnoDAL.ObtenerTurnosEnEspera(idEspecialidad);
        }
    }
}