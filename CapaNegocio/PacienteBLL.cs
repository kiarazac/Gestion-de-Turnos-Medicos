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
        /// Valida los datos filiatorios básicos y registra o actualiza un paciente en el sistema vinculando su obra social.
        /// </summary>
        /// <param name="nombre">Nombre(s) del paciente.</param>
        /// <param name="apellido">Apellido(s) del paciente.</param>
        /// <param name="dni">Documento Nacional de Identidad.</param>
        /// <param name="idObraSocial">Identificador único de la obra social asociada.</param>
        /// <param name="telefono">Teléfono de contacto (opcional). Si se informa, debe tener entre 7 y 15 dígitos.</param>
        /// <returns>Identificador único (<c>IdPaciente</c>) generado o existente en la base de datos.</returns>
        /// <exception cref="ArgumentException">Se lanza si el nombre, apellido o DNI están vacíos, o si el teléfono tiene un formato inválido.</exception>
        public int GuardarPaciente(string nombre, string apellido, string dni, int idObraSocial, string? telefono)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Nombre, apellido y DNI son campos obligatorios para registrar al paciente.");

            telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim();
            if (telefono != null && !Validaciones.EsTelefono(telefono))
                throw new ArgumentException("El teléfono ingresado no es válido. Debe contener solo números (entre 7 y 15 dígitos).");

            return _pacienteDAL.GuardarPaciente(nombre.Trim(), apellido.Trim(), dni.Trim(), idObraSocial <= 0 ? 1 : idObraSocial, telefono);
        }
    }
}