using System;
using System.Collections.Generic;
using System.Text;
using Gestion_de_Turnos_Medicos.CapaDeDatos;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    // Aísla la interfaz visual de la persistencia de datos y se asegura de que la información del paciente sea consistente[cite: 2].
    public class PacienteBLL
    {
        private readonly PacienteDAL _pacienteDAL = new PacienteDAL();

        // Método que invoca el recepcionista (por ejemplo en FrmTurnoEmergencia o FrmTurnoEspecialidad) para registrar al paciente en el sistema mediante sus datos filiatorios[cite: 1, 2].
        public int GuardarPaciente(string nombre, string apellido, string dni, string obraSocial)
        {
            // Validación de negocio fundamental: no se puede registrar un paciente sin sus datos básicos identificatorios.
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Nombre, apellido y DNI son campos obligatorios para registrar al paciente.");

            // Pasamos los datos limpios de espacios extra a la capa de datos.
            return _pacienteDAL.GuardarPaciente(nombre.Trim(), apellido.Trim(), dni.Trim(), obraSocial?.Trim());
        }
    }
}