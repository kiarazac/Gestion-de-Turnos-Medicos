using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    public class TurnoBLL
    {
        private readonly TurnoDAL _turnoDAL = new TurnoDAL();

        public List<SintomaDTO> ObtenerSintomas()
        {
            return _turnoDAL.ObtenerSintomas();
        }



        public PacienteDTO BuscarPacientePorDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Debe ingresar un DNI válido para la búsqueda.");

            return _turnoDAL.BuscarPacientePorDNI(dni.Trim());
        }

        public string CrearTurnoEmergenciaConSintomas(int idPaciente, List<int> idsSintomas, bool esOtroSeleccionado)
        {
            if (idPaciente <= 0)
                throw new ArgumentException("El ID del paciente es inválido o no ha sido registrado/cargado.");

            int prioridadDeterminada = 3; // Por defecto o si selecciona "Otro"

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

        // Actualizado para usar la información descriptiva en lugar del IdSala[cite: 3].
        public void LlamarSiguientePaciente(int idTurno, string nombreMedico, string salaAsignada)
        {
            if (idTurno <= 0 || string.IsNullOrWhiteSpace(nombreMedico) || string.IsNullOrWhiteSpace(salaAsignada))
                throw new ArgumentException("Se requieren todos los datos del turno y del profesional para llamar al paciente.");

            _turnoDAL.LlamarSiguientePaciente(idTurno, nombreMedico, salaAsignada);
        }

        public List<HorarioDisponibleDTO> ObtenerHorariosDisponibles(string nombreEspecialidad, DateTime fecha)
        {
            if (string.IsNullOrWhiteSpace(nombreEspecialidad))
                throw new ArgumentException("Debe seleccionar una especialidad.");

            return _turnoDAL.ObtenerHorariosDisponibles(nombreEspecialidad, fecha);
        }

        public ResultadoTurnoDTO CrearTurnoEspecialidad(int idPaciente, string nombreEspecialidad, DateTime fecha, string horario, string estado = "En Espera")
        {
            if (idPaciente <= 0 || string.IsNullOrWhiteSpace(nombreEspecialidad) || string.IsNullOrWhiteSpace(horario))
                throw new ArgumentException("Todos los datos del turno programado son obligatorios.");

            if (fecha.Date < DateTime.Now.Date)
                throw new ArgumentException("No se pueden registrar turnos en fechas pasadas.");

            return _turnoDAL.CrearTurnoEspecialidad(idPaciente, nombreEspecialidad, fecha, horario, estado);
        }  
        public List<TurnoEmergenciaDTO> ListarTurnosEmergencia()
        {
            return _turnoDAL.ListarTurnosEmergencia();
        }

        public List<TurnoGeneralPantallaDTO> ObtenerTurnosPantallaGeneral()
        {
            return _turnoDAL.ObtenerTurnosPantallaGeneral();
        }

        public List<TurnoListadoDTO> ListarTurnosEspecialidad(string nombreEspecialidad)
        {
            if (string.IsNullOrWhiteSpace(nombreEspecialidad))
                throw new ArgumentException("Debe indicar la especialidad a consultar.");

            return _turnoDAL.ListarTurnosEspecialidad(nombreEspecialidad);
        }

        public List<TurnoListadoDTO> ObtenerListaTurnos(int? idEspecialidad = null, string? estado = null)
        {
            return _turnoDAL.ObtenerListaTurnos(idEspecialidad, estado);
        }

        public List<TurnoAtencionDTO> ListarTurnosAtencion()
        {
            return _turnoDAL.ListarTurnosAtencion();
        }


        public void IniciarAtencionTurno(int idTurno, string salaAsignada)
        {
            if (idTurno <= 0)
                throw new ArgumentException("El ID del turno no es válido.");

            _turnoDAL.IniciarAtencionTurno(idTurno, salaAsignada);
        }

        public void FinalizarAtencionTurno(int idTurno, string diagnostico, string nombreMedico, string salaAsignada)
        {
            if (idTurno <= 0 || string.IsNullOrWhiteSpace(diagnostico) || string.IsNullOrWhiteSpace(nombreMedico) || string.IsNullOrWhiteSpace(salaAsignada))
                throw new ArgumentException("Faltan datos obligatorios para finalizar la atención.");

            _turnoDAL.FinalizarAtencionTurno(idTurno, diagnostico, nombreMedico, salaAsignada);
        }

        public List<TurnoPantallaDTO> ObtenerTurnosPantallaPublica()
        {
            return _turnoDAL.ObtenerTurnosPantallaPublica();
        }

        public List<TurnoEsperaDTO> ObtenerTurnosEnEspera(int idEspecialidad)
        {
            if (idEspecialidad <= 0)
                throw new ArgumentException("Debe indicar una especialidad válida para consultar la cola de espera.");

            return _turnoDAL.ObtenerTurnosEnEspera(idEspecialidad);
        }
    }
}