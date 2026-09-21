using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) para la persistencia y consulta de pacientes mediante Stored Procedures.
    /// </summary>
    public class PacienteDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_GuardarPaciente</c> realizando un Upsert inteligente:
        /// si el paciente con ese DNI ya existe, actualiza sus datos; si no existe, lo inserta en la tabla <c>Pacientes</c>.
        /// </summary>
        /// <param name="nombre">Nombre(s) del paciente.</param>
        /// <param name="apellido">Apellido(s) del paciente.</param>
        /// <param name="dni">DNI del paciente.</param>
        /// <param name="obraSocial">Nombre de la cobertura médica u obra social.</param>
        /// <returns>Identificador único del paciente (<c>IdPaciente</c>).</returns>
        public int GuardarPaciente(string nombre, string apellido, string dni, string obraSocial)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombre = new SqlParameter("@Nombre", nombre);
                var pApellido = new SqlParameter("@Apellido", apellido);
                var pDni = new SqlParameter("@Dni", dni);
                var pObraSocial = new SqlParameter("@ObraSocial", string.IsNullOrWhiteSpace(obraSocial) ? DBNull.Value : (object)obraSocial);

                return context.Database
                    .SqlQueryRaw<int>("EXEC sp_GuardarPaciente @Nombre, @Apellido, @Dni, @ObraSocial",
                        pNombre, pApellido, pDni, pObraSocial)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
        }
    }
}