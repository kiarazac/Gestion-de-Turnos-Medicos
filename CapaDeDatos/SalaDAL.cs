using Gestion_de_Turnos_Medicos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapaDeDatos
{
    // La Capa de Datos (DAL) es la ÚNICA que se comunica con SQL Server, utilizando exclusivamente dbTurnosMedicos (EF Core)[cite: 2].
    public class SalaDAL
    {
        // Ejecuta el procedimiento sp_ObtenerSalas.
        // Retorna una lista de SalaDTO. Si se le pasa un idUsuario, trae solo las de ese médico; si es null, trae todas (ideal para el administrador)[cite: 2].
        public List<SalaDTO> ObtenerSalas(int? idUsuario = null)
        {
            // Instanciamos tu contexto personalizado de Entity Framework Core[cite: 2].
            using (var context = new dbTurnosMedicos())
            {
                // Preparamos el parámetro SQL de forma segura y tipada para evitar ataques de inyección SQL[cite: 2].
                var paramUsuario = new SqlParameter("@IdUsuario", (object)idUsuario ?? DBNull.Value);

                // SqlQueryRaw se usa cuando esperamos que la base de datos nos devuelva registros (filas y columnas) que debemos mapear a una clase[cite: 2].
                return context.Database
                    .SqlQueryRaw<SalaDTO>("EXEC sp_ObtenerSalas @IdUsuario", paramUsuario)
                    .ToList(); // Materializamos el resultado a una lista genérica de C#.
            }
        }

        // Ejecuta el procedimiento sp_AbrirSala.
        public void AbrirSala(int idSala, int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);

                // ExecuteSqlRaw se utiliza cuando hacemos operaciones (como cambios de estado, inserts o updates) que NO retornan tablas[cite: 2].
                context.Database.ExecuteSqlRaw("EXEC sp_AbrirSala @IdSala, @IdUsuario", pIdSala, pIdUsuario);
            }
        }

        // Ejecuta el procedimiento sp_CerrarSala.
        public void CerrarSala(int idSala)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                context.Database.ExecuteSqlRaw("EXEC sp_CerrarSala @IdSala", pIdSala);
    }
        }

        public int InsertarSala(string nombreSala, string estadoSala)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombreSala = new SqlParameter("@NombreSala", nombreSala);
                var pEstadoSala = new SqlParameter("@EstadoSala", estadoSala);

                return context.Database
                    .SqlQueryRaw<int>("EXEC sp_InsertarSala @NombreSala, @EstadoSala", pNombreSala, pEstadoSala)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
        }

        public void EliminarSala(int idSala)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                context.Database.ExecuteSqlRaw("EXEC sp_EliminarSala @IdSala", pIdSala);
            }
        }

        public void AsignarSalaMedico(int idSala, int idUsuario, string descripcionAtencion)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pDesc = new SqlParameter("@DescripcionAtencion", (object)descripcionAtencion ?? DBNull.Value);

                context.Database.ExecuteSqlRaw("EXEC sp_AsignarSalaMedico @IdSala, @IdUsuario, @DescripcionAtencion", pIdSala, pIdUsuario, pDesc);
            }
        }

        // Actualiza de manera genérica el estado operativo de una sala.
        public void ActualizarEstadoSala(int idSala, string nuevoEstado)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pNuevoEstado = new SqlParameter("@NuevoEstado", nuevoEstado);

                context.Database.ExecuteSqlRaw("EXEC sp_ActualizarEstadoSala @IdSala, @NuevoEstado", pIdSala, pNuevoEstado);
            }
        }
    }
}