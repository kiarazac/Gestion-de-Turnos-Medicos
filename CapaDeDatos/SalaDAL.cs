using Gestion_de_Turnos_Medicos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
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

                var resultado = context.Database
                    .SqlQueryRaw<NuevaSalaIdDTO>("EXEC sp_InsertarSala @NombreSala, @EstadoSala", pNombreSala, pEstadoSala)
                    .AsEnumerable()
                    .FirstOrDefault();

                return resultado != null ? resultado.IdNuevaSala : 0;
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

        public void AsignarSalaMedico(int idSala, int idUsuario, string? descripcionAtencion = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pDesc = new SqlParameter("@DescripcionAtencion", (object?)descripcionAtencion ?? DBNull.Value);

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

        /// <summary>
        /// Ejecuta el procedimiento almacenado sp_ModificarSala para actualizar el nombre y el estado de una sala existente.
        /// Utiliza parámetros tipados para evitar inyección SQL.
        /// </summary>
        /// <param name="idSala">Identificador único de la sala a modificar.</param>
        /// <param name="nombreSala">Nuevo nombre descriptivo para la sala.</param>
        /// <param name="estadoSala">Estado operativo de la sala ('Disponible', 'Ocupada', 'En Mantenimiento').</param>
        public void ModificarSala(int idSala, string nombreSala, string? estadoSala = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                // 1. Configuramos los parámetros SQL requeridos por el procedimiento
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pNombreSala = new SqlParameter("@NombreSala", nombreSala);
                var pEstadoSala = new SqlParameter("@EstadoSala", (object?)estadoSala ?? DBNull.Value);

                // 2. Ejecutamos sp_ModificarSala en la base de datos
                context.Database.ExecuteSqlRaw("EXEC sp_ModificarSala @IdSala, @NombreSala, @EstadoSala", pIdSala, pNombreSala, pEstadoSala);
            }
        }

        /// <summary>
        /// Reasigna los profesionales médicos asignados a una sala de atención.
        /// Aplica una baja lógica a las asignaciones activas existentes en DetallesSalas
        /// y luego inserta las nuevas vinculaciones seleccionadas mediante sp_AsignarSalaMedico.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idsMedicos">Lista de identificadores de los usuarios médicos a vincular.</param>
        public void ReasignarMedicosASala(int idSala, List<int> idsMedicos)
        {
            using (var context = new dbTurnosMedicos())
            {
                // 1. Damos de baja lógica las asignaciones activas actuales de la sala
                var pIdSala = new SqlParameter("@IdSala", idSala);
                context.Database.ExecuteSqlRaw("UPDATE DetallesSalas SET Activo = 0, FechaBaja = GETDATE() WHERE IdSala = @IdSala AND Activo = 1", pIdSala);

                // 2. Insertamos las nuevas asignaciones seleccionadas por el administrador
                if (idsMedicos != null && idsMedicos.Count > 0)
                {
                    foreach (int idUsuario in idsMedicos)
                    {
                        var pSala = new SqlParameter("@IdSala", idSala);
                        var pUsuario = new SqlParameter("@IdUsuario", idUsuario);
                        var pDesc = new SqlParameter("@DescripcionAtencion", string.Empty);
                        context.Database.ExecuteSqlRaw("EXEC sp_AsignarSalaMedico @IdSala, @IdUsuario, @DescripcionAtencion", pSala, pUsuario, pDesc);
                    }
                }
            }
        }
    }
}