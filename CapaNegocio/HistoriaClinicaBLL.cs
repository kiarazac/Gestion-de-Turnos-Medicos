using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    public class HistoriaClinicaBLL
    {
        private readonly HistoriaClinicaDAL _historiaDAL = new HistoriaClinicaDAL();

        public void RegistrarHistoria(string tipoTurno, string diagRapido, string descripHistoriaClinica, string recetaMedicamentos, int idPaciente, int idTurno, int idUsuario)
        {
            if (idPaciente <= 0 || idTurno <= 0 || idUsuario <= 0)
                throw new ArgumentException("Los identificadores de paciente, turno y usuario son obligatorios para el registro clínico.");

            if (string.IsNullOrWhiteSpace(descripHistoriaClinica))
                throw new ArgumentException("La evolución médica no puede estar vacía.");

            _historiaDAL.InsertarHistoria(tipoTurno, diagRapido, descripHistoriaClinica, recetaMedicamentos, idPaciente, idTurno, idUsuario);
        }

        public List<HistoriaClinicaDTO> ObtenerHistoriaClinicaPaciente(int idPaciente)
        {
            if (idPaciente <= 0)
                throw new ArgumentException("El ID del paciente no es válido.");

            return _historiaDAL.ObtenerHistoriaClinicaPaciente(idPaciente);
        }
    }
}