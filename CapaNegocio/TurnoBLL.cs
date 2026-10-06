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
        /// Si se seleccionan varios síntomas con diferentes niveles de gravedad, la prioridad asignada
        /// corresponderá a la del síntoma con mayor urgencia/gravedad (1 = Alta, 2 = Media, 3 = Baja).
        /// </summary>
        /// <param name="idPaciente">Identificador único del paciente admitido.</param>
        /// <param name="idsSintomas">Lista de identificadores de síntomas tildados.</param>
        /// <param name="esOtroSeleccionado">Indica si el paciente seleccionó la opción de síntoma no catalogado ("Otro").</param>
        /// <returns>Código correlativo de turno asignado (ej. 'E-001').</returns>
        /// <exception cref="ArgumentException">Se lanza si el ID del paciente es inválido o no se especificó ningún síntoma ni la opción 'Otro'.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error al persistir el turno en base de datos.</exception>
        public string CrearTurnoEmergenciaConSintomas(int idPaciente, List<int> idsSintomas, bool esOtroSeleccionado)
        {
            // Invoca la sobrecarga principal descartando la salida de texto de la prioridad
            return CrearTurnoEmergenciaConSintomas(idPaciente, idsSintomas, esOtroSeleccionado, out _);
        }

        /// <summary>
        /// Determina la prioridad de triage según los síntomas seleccionados, crea el turno de emergencia,
        /// persiste los síntomas asociados y retorna tanto el código de turno como la descripción textual de la prioridad calculada.
        /// Garantiza que el síntoma con la gravedad más severa determine la prioridad final del turno médico.
        /// </summary>
        /// <param name="idPaciente">Identificador único del paciente admitido.</param>
        /// <param name="idsSintomas">Lista de identificadores de síntomas tildados.</param>
        /// <param name="esOtroSeleccionado">Indica si el paciente seleccionó la opción de síntoma no catalogado ("Otro").</param>
        /// <param name="prioridadTexto">Parámetro de salida que entrega el nivel de urgencia asignado ('ALTA', 'MEDIA', 'BAJA').</param>
        /// <returns>Código correlativo de turno asignado (ej. 'E-001').</returns>
        /// <exception cref="ArgumentException">Se lanza si los datos son inválidos o no hay ningún síntoma seleccionado.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error al persistir el turno en base de datos.</exception>
        public string CrearTurnoEmergenciaConSintomas(int idPaciente, List<int> idsSintomas, bool esOtroSeleccionado, out string prioridadTexto)
        {
            if (idPaciente <= 0)
                throw new ArgumentException("El ID del paciente es inválido o no ha sido registrado/cargado.");

            bool tieneSintomas = idsSintomas != null && idsSintomas.Count > 0;

            // Validación: Debe existir al menos un síntoma seleccionado o haberse marcado la casilla "Otro"
            if (!tieneSintomas && !esOtroSeleccionado)
                throw new ArgumentException("Debe seleccionar al menos un síntoma principal o marcar la opción 'Otro'.");

            // La escala de prioridades en base de datos y modelo es:
            // 1 = Alta (mayor urgencia vital)
            // 2 = Media (urgencia moderada)
            // 3 = Baja (guardia regular / sin gravedad inmediata)
            // Por ende, el menor valor numérico representa la mayor gravedad médica.
            int prioridadDeterminada = 3; // Inicializamos con la menor gravedad posible (Baja / 3)

            // Si hay síntomas seleccionados en el catálogo, evaluamos la gravedad de cada uno
            if (tieneSintomas)
            {
                foreach (int idSintoma in idsSintomas!)
                {
                    int gravedadSintoma = _turnoDAL.ObtenerGravedadDeSintoma(idSintoma);

                    // Si el síntoma actual tiene mayor severidad (es decir, un número menor)
                    // que la prioridad acumulada, se actualiza la prioridad asignada.
                    // Esto asegura que si hay al menos UN síntoma de gravedad Alta (1),
                    // la prioridad final será Alta aunque también haya síntomas de gravedad Media (2) o "Otro".
                    if (gravedadSintoma < prioridadDeterminada)
                    {
                        prioridadDeterminada = gravedadSintoma;
                    }
                }
            }

            // Asignamos la etiqueta textual descriptiva de acuerdo al resultado de triage obtenido
            prioridadTexto = prioridadDeterminada switch
            {
                1 => "ALTA",
                2 => "MEDIA",
                _ => "BAJA"
            };

            // Invocamos al procedimiento almacenado sp_CrearTurnoEmergencia con la prioridad calculada
            var resultadoTurno = _turnoDAL.CrearTurnoEmergenciaCompleto(idPaciente, prioridadDeterminada);

            if (resultadoTurno.IdNuevoTurno <= 0)
                throw new Exception("Error al generar el turno en la base de datos.");

            // Guardamos cada uno de los síntomas manifestados en la tabla asociativa TurnoSintomas
            if (tieneSintomas)
            {
                foreach (int idSintoma in idsSintomas!)
                {
                    _turnoDAL.GuardarTurnoSintoma(resultadoTurno.IdNuevoTurno, idSintoma);
                }
            }

            return resultadoTurno.NroOrden;
        }

        private readonly SalaDAL _salaDAL = new SalaDAL();

        /// <summary>
        /// Realiza el llamado a un paciente en espera asignándole el profesional médico y consultorio físico correspondiente.
        /// Valida en la Capa de Negocio que la sala de atención no se encuentre ocupada por otra consulta en curso
        /// y que solo los profesionales con especialidad Clínico puedan atender turnos de guardia/emergencia.
        /// </summary>
        /// <param name="idTurno">Identificador del turno a llamar.</param>
        /// <param name="nombreMedico">Nombre del médico que realiza la llamada.</param>
        /// <param name="salaAsignada">Denominación de la sala o box de atención.</param>
        /// <param name="idUsuario">Identificador único del profesional médico (opcional para control de especialidad).</param>
        /// <exception cref="ArgumentException">Se lanza si falta información requerida para el llamado.</exception>
        /// <exception cref="InvalidOperationException">Se lanza si la sala se encuentra ocupada o si un médico no clínico intenta atender emergencias.</exception>
        public void LlamarSiguientePaciente(int idTurno, string nombreMedico, string salaAsignada, int? idUsuario = null)
        {
            if (idTurno <= 0 || string.IsNullOrWhiteSpace(nombreMedico) || string.IsNullOrWhiteSpace(salaAsignada))
                throw new ArgumentException("Se requieren todos los datos del turno y del profesional para llamar al paciente.");

            if (!salaAsignada.Equals("(Sin sala abierta)", StringComparison.OrdinalIgnoreCase) &&
                !salaAsignada.Equals("--", StringComparison.OrdinalIgnoreCase))
            {
                var sala = _salaDAL.ObtenerSalaPorNombre(salaAsignada);
                if (sala != null && sala.EstadoSala.Equals("Ocupada", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"No se puede llamar a un paciente porque la sala '{salaAsignada}' se encuentra actualmente ocupada por otra atención médica en curso.");
                }
            }

            // Regla de Negocio: Solo usuarios con especialidad Clínico pueden atender turnos de emergencia
            if (idUsuario.HasValue && idUsuario.Value > 0)
            {
                if (_turnoDAL.EsTurnoEmergencia(idTurno) && !_turnoDAL.PuedeAtenderEmergencias(idUsuario.Value))
                {
                    throw new InvalidOperationException("Solo los usuarios con especialidad Clínico pueden atender turnos de emergencia.");
                }
            }

            _turnoDAL.LlamarSiguientePaciente(idTurno, nombreMedico, salaAsignada, idUsuario);
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
        /// Comprueba si ya existe un turno activo reservado para la misma fecha, horario y especialidad médica.
        /// Si cualquiera de estos tres atributos difiere, la asignación es permitida.
        /// </summary>
        /// <param name="nombreEspecialidad">Nombre de la especialidad médica solicitada.</param>
        /// <param name="fecha">Fecha requerida para el turno.</param>
        /// <param name="horario">Horario solicitado (ej. '10:00').</param>
        /// <returns><c>true</c> si el turno ya está reservado; <c>false</c> si se encuentra disponible.</returns>
        public bool ExisteTurnoEspecialidad(string nombreEspecialidad, DateTime fecha, string horario)
        {
            if (string.IsNullOrWhiteSpace(nombreEspecialidad) || string.IsNullOrWhiteSpace(horario))
                return false;

            return _turnoDAL.ExisteTurnoEspecialidad(nombreEspecialidad.Trim(), fecha.Date, horario.Trim());
        }

        /// <summary>
        /// Registra un nuevo turno programado por especialidad médica validando que la fecha sea igual o posterior al día actual
        /// y comprobando que no exista previamente otro turno activo con la misma fecha, horario y especialidad médica.
        /// </summary>
        /// <summary>
        /// Genera una palabra clave o código alfanumérico seguro para la confirmación de doble factor (2FA) en cancelaciones.
        /// Formato: 'CAN-XXXX' con caracteres alfanuméricos legibles.
        /// </summary>
        /// <returns>Cadena con el código alfanumérico generado.</returns>
        public string GenerarCodigoCancelacion()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = Random.Shared;
            var token = new char[4];
            for (int i = 0; i < token.Length; i++)
            {
                token[i] = chars[random.Next(chars.Length)];
            }
            return $"CAN-{new string(token)}";
        }

        /// <summary>
        /// Registra un nuevo turno programado de consultorio externo para una especialidad médica,
        /// generando automáticamente la palabra clave alfanumérica de seguridad (2FA) para cancelación.
        /// </summary>
        /// <param name="idPaciente">Identificador del paciente.</param>
        /// <param name="nombreEspecialidad">Nombre de la especialidad solicitada.</param>
        /// <param name="fecha">Fecha asignada para el turno.</param>
        /// <param name="horario">Horario de atención acordado.</param>
        /// <param name="estado">Estado inicial del turno (por defecto 'En Espera').</param>
        /// <param name="codigoCancelacion">Palabra clave alfanumérica personalizada (si es nula, se autogenera).</param>
        /// <returns>Objeto <see cref="ResultadoTurnoDTO"/> con el ID de turno, código de orden generado y clave 2FA.</returns>
        /// <exception cref="ArgumentException">Se lanza si faltan datos o la fecha es anterior al día actual.</exception>
        /// <exception cref="InvalidOperationException">Se lanza si ya existe un turno asignado para la misma fecha, horario y especialidad.</exception>
        public ResultadoTurnoDTO CrearTurnoEspecialidad(int idPaciente, string nombreEspecialidad, DateTime fecha, string horario, string estado = "En Espera", string? codigoCancelacion = null)
        {
            if (idPaciente <= 0 || string.IsNullOrWhiteSpace(nombreEspecialidad) || string.IsNullOrWhiteSpace(horario))
                throw new ArgumentException("Todos los datos del turno programado son obligatorios.");

            if (fecha.Date < DateTime.Now.Date)
                throw new ArgumentException("No se pueden registrar turnos en fechas pasadas.");

            // Control de duplicidad: no se puede sacar más de un turno con la misma fecha, horario y especialidad
            if (ExisteTurnoEspecialidad(nombreEspecialidad, fecha, horario))
            {
                throw new InvalidOperationException($"El horario de las {horario} hs para la especialidad '{nombreEspecialidad}' en la fecha {fecha:dd/MM/yyyy} ya se encuentra reservado. Por favor, seleccione otro horario disponible.");
            }

            if (string.IsNullOrWhiteSpace(codigoCancelacion))
            {
                codigoCancelacion = GenerarCodigoCancelacion();
            }

            return _turnoDAL.CrearTurnoEspecialidad(idPaciente, nombreEspecialidad, fecha, horario, estado, codigoCancelacion);
        }

        /// <summary>
        /// Cancela un turno programado de especialidad previa validación de la palabra clave / código 2FA.
        /// </summary>
        /// <param name="idTurno">Identificador del turno a cancelar.</param>
        /// <param name="codigoCancelacion">Palabra clave alfanumérica proporcionada por el usuario.</param>
        /// <exception cref="ArgumentException">Se lanza si el código de cancelación está vacío o el ID es inválido.</exception>
        public void CancelarTurnoEspecialidad(int idTurno, string codigoCancelacion)
        {
            if (idTurno <= 0)
                throw new ArgumentException("El ID del turno es inválido.");

            if (string.IsNullOrWhiteSpace(codigoCancelacion))
                throw new ArgumentException("Debe ingresar la palabra clave / código 2FA para cancelar el turno.");

            _turnoDAL.CancelarTurnoEspecialidad(idTurno, codigoCancelacion.Trim());
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

        /// <summary>
        /// Comprueba si el usuario médico cuenta con la especialidad Clínico (o afín a Clínica Médica) activa
        /// para habilitar la atención de turnos de guardia/emergencias.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario médico.</param>
        /// <returns><c>true</c> si el médico tiene especialidad clínico activa; de lo contrario, <c>false</c>.</returns>
        public bool PuedeAtenderEmergencias(int idUsuario)
        {
            if (idUsuario <= 0) return false;
            return _turnoDAL.PuedeAtenderEmergencias(idUsuario);
        }
    }
}