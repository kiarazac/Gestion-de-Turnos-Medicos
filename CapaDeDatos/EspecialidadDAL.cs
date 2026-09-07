using Gestion_de_Turnos_Medicos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    // Clase exclusiva para interactuar con dbTurnosMedicos y ejecutar los Stored Procedures de especialidades[cite: 2].
    public class EspecialidadDAL
    {
        // Ejecuta el procedimiento sp_ListarEspecialidades y devuelve el resultado mapeado a EspecialidadDTO[cite: 2].
        public List<EspecialidadDTO> ListarEspecialidades()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database
                    .SqlQueryRaw<EspecialidadDTO>("EXEC sp_ListarEspecialidades")
                    .ToList();
            }
        }

        // Ejecuta sp_InsertarEspecialidad para dar de alta una nueva especialidad en el sistema[cite: 2].
        public void InsertarEspecialidad(string nombre)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombre = new SqlParameter("@Nombre", nombre);
                // ExecuteSqlRaw se usa porque es una inserción y no esperamos una tabla de retorno[cite: 2].
                context.Database.ExecuteSqlRaw("EXEC sp_InsertarEspecialidad @Nombre", pNombre);
            }
        }

        // Ejecuta sp_EliminarEspecialidad para realizar una baja lógica, conservando el registro histórico[cite: 1, 2].
        public void EliminarEspecialidad(int idEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", idEspecialidad);
                context.Database.ExecuteSqlRaw("EXEC sp_EliminarEspecialidad @IdEspecialidad", pIdEspecialidad);
            }
        }
    }
}