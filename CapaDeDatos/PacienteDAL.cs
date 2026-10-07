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
        /// si el paciente con ese DNI ya existe, actualiza sus datos y cobertura; si no existe, lo inserta en la tabla <c>Pacientes</c>.
        /// </summary>
        /// <param name="nombre">Nombre(s) del paciente.</param>
        /// <param name="apellido">Apellido(s) del paciente.</param>
        /// <param name="dni">DNI del paciente.</param>
        /// <param name="idObraSocial">Identificador único de la obra social asociada.</param>
        /// <returns>Identificador único del paciente (<c>IdPaciente</c>).</returns>
        public int GuardarPaciente(string nombre, string apellido, string dni, int idObraSocial)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombre = new SqlParameter("@Nombre", nombre);
                var pApellido = new SqlParameter("@Apellido", apellido);
                var pDni = new SqlParameter("@Dni", dni);
                var pIdObraSocial = new SqlParameter("@IdObraSocial", idObraSocial <= 0 ? 1 : idObraSocial);

                return context.Database
                    .SqlQueryRaw<int>("EXEC sp_GuardarPaciente @Nombre, @Apellido, @Dni, @IdObraSocial",
                        pNombre, pApellido, pDni, pIdObraSocial)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
        }
    }
}