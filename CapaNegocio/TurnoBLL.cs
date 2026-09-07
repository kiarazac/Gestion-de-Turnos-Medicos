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

        // Adaptado para enviar el NroOrden y separar el síntoma principal del resto[cite: 3].
        public void CrearTurnoEmergenciaConSintomas(string nroOrden, int idPaciente, List<int> idsSintomas, string estadoActual)
        {
            if (idPaciente <= 0 || string.IsNullOrWhiteSpace(nroOrden))
                throw new ArgumentException("Datos del paciente o número de orden inválidos.");

            if (idsSintomas == null || idsSintomas.Count == 0)
                throw new ArgumentException("Debe registrar al menos un síntoma para que el sistema determine la prioridad.");

            // 1. El motor de urgencias de SQL pide un único síntoma base para calcular la prioridad[cite: 3].
            int sintomaPrincipal = idsSintomas[0];
            int idTurnoNuevo = _turnoDAL.RegistrarTurnoEmergencia(nroOrden, idPaciente, sintomaPrincipal, estadoActual);

            // 2. Si el paciente indicó más de un síntoma, los agregamos como secundarios[cite: 3].
            for (int i = 1; i < idsSintomas.Count; i++)
            {
                _turnoDAL.RegistrarTurnoSintoma(idTurnoNuevo, idsSintomas[i], estadoActual);
            }
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

        public void CrearTurnoProgramado(int idPaciente, string nombreEspecialidad, DateTime fecha, string horario)
        {
            if (idPaciente <= 0 || string.IsNullOrWhiteSpace(nombreEspecialidad) || string.IsNullOrWhiteSpace(horario))
                throw new ArgumentException("Todos los datos del turno programado son obligatorios.");

            if (fecha.Date < DateTime.Now.Date)
                throw new ArgumentException("No se pueden registrar turnos en fechas pasadas.");

            _turnoDAL.CrearTurnoEspecialidad(idPaciente, nombreEspecialidad, fecha, horario);
        }

        
        public List<TurnoEmergenciaDTO> ListarTurnosEmergencia()
        {
            return _turnoDAL.ListarTurnosEmergencia();
        }

        public List<TurnoListadoDTO> ListarTurnosEspecialidad(string nombreEspecialidad)
        {
            if (string.IsNullOrWhiteSpace(nombreEspecialidad))
                throw new ArgumentException("Debe indicar la especialidad a consultar.");

            return _turnoDAL.ListarTurnosEspecialidad(nombreEspecialidad);
        }

        public List<TurnoListadoDTO> ObtenerListaTurnos(int? idEspecialidad = null, string estado = null)
        {
            return _turnoDAL.ObtenerListaTurnos(idEspecialidad, estado);
        }

        public List<TurnoAtencionDTO> ListarTurnosAtencion()
        {
            return _turnoDAL.ListarTurnosAtencion();
        }

        public void IniciarAtencionTurno(int idTurno)
        {
            if (idTurno <= 0)
                throw new ArgumentException("El ID del turno no es válido.");

            _turnoDAL.IniciarAtencionTurno(idTurno);
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