using System;
using Gestion_de_Turnos_Medicos.CapaDeDatos;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio para la gestión de pacientes y validación de datos filiatorios.
    /// </summary>
    public class PacienteBLL
    {
        private readonly PacienteDAL _pacienteDAL = new PacienteDAL();

        /// <summary>
        /// Valida los datos filiatorios básicos y registra o actualiza un paciente en el sistema.
        /// </summary>
        /// <param name="nombre">Nombre(s) del paciente.</param>
        /// <param name="apellido">Apellido(s) del paciente.</param>
        /// <param name="dni">Documento Nacional de Identidad.</param>
        /// <param name="obraSocial">Cobertura médica u obra social (opcional).</param>
        /// <returns>Identificador único (<c>IdPaciente</c>) generado o existente en la base de datos.</returns>
        /// <exception cref="ArgumentException">Se lanza si el nombre, apellido o DNI están vacíos.</exception>
        public int GuardarPaciente(string nombre, string apellido, string dni, string obraSocial)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Nombre, apellido y DNI son campos obligatorios para registrar al paciente.");

            return _pacienteDAL.GuardarPaciente(nombre.Trim(), apellido.Trim(), dni.Trim(), obraSocial?.Trim() ?? string.Empty);
        }
    }
}