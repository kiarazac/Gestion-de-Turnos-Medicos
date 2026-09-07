using Gestion_de_Turnos_Medicos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    public class PacienteDAL
    {
        // Ejecuta sp_GuardarPaciente para el Upsert Inteligente.
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