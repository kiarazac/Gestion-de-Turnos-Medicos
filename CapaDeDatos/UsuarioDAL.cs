using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    public class UsuarioDAL
    {
        public UsuarioLoginResult ValidarLogin(string correo, string contrasena)
        {
           
            using (var context = new dbTurnosMedicos())
            {
                // 1. Preparamos los parámetros para evitar Inyección SQL
                var paramCorreo = new SqlParameter("@Correo", correo);
                var paramContrasena = new SqlParameter("@Contrasena", contrasena);

                // 2. Ejecutamos el Procedimiento Almacenado
                var usuarioLogueado = context.Database.SqlQueryRaw<UsuarioLoginResult>(
                    "EXEC sp_ValidarLogin @Correo, @Contrasena",
                    paramCorreo,
                    paramContrasena
                ).AsEnumerable().FirstOrDefault(); // Toma el primer registro encontrado o devuelve null

                // 3. Retornamos la credencial a la Capa de Negocio
                return usuarioLogueado;
            }
        }

        public List<RolDTO> ListarRoles()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<RolDTO>("EXEC sp_ListarRoles").ToList();
            }
        }

        public List<MedicoDTO> ListarPersonalMedico()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<MedicoDTO>("EXEC sp_ListarPersonalMedico").ToList();
            }
        }

        public int InsertarUsuario(string nombre, string apellido, string correo, string contrasena, string dni, string telefono, string nroMatricula, int idRol)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombre = new SqlParameter("@Nombre", nombre);
                var pApellido = new SqlParameter("@Apellido", apellido);
                var pCorreo = new SqlParameter("@Correo", correo);
                var pContrasena = new SqlParameter("@Contrasena", contrasena);
                var pDni = new SqlParameter("@Dni", dni);
                var pTelefono = new SqlParameter("@Telefono", (object)telefono ?? DBNull.Value);
                var pMatricula = new SqlParameter("@NroMatricula", (object)nroMatricula ?? DBNull.Value);
                var pIdRol = new SqlParameter("@IdRol", idRol);

                // Ahora mapeamos el resultado contra el DTO que tiene el nombre de columna correcto
                var resultado = context.Database
                    .SqlQueryRaw<NuevoUsuarioIdDTO>("EXEC sp_InsertarUsuario @Nombre, @Apellido, @Correo, @Contrasena, @Dni, @Telefono, @NroMatricula, @IdRol",
                        pNombre, pApellido, pCorreo, pContrasena, pDni, pTelefono, pMatricula, pIdRol)
                    .AsEnumerable()
                    .FirstOrDefault();

                // Extraemos el ID real generado, o devolvemos 0 si algo falló
                return resultado != null ? resultado.IdNuevoUsuario : 0;
            }
        }

        public void AsignarEspecialidadMedico(int idUsuario, int idEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", idEspecialidad);

                context.Database.ExecuteSqlRaw("EXEC sp_AsignarEspecialidadMedico @IdUsuario, @IdEspecialidad", pIdUsuario, pIdEspecialidad);
            }
        }

        public void AsignarSalaMedico(int idSala, int idUsuario, string descripcionAtencion)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pDesc = new SqlParameter("@DescripcionAtencion", (object)descripcionAtencion ?? DBNull.Value);

                // El orden de las variables al final (pIdSala, pIdUsuario) debe ser idéntico al de los @parámetros en el texto
                context.Database.ExecuteSqlRaw("EXEC sp_AsignarSalaMedico @IdSala, @IdUsuario, @DescripcionAtencion",
                    pIdSala, pIdUsuario, pDesc);
            }
        }
        public List<UsuarioListadoDTO> ListarUsuarios()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database
                    .SqlQueryRaw<UsuarioListadoDTO>("EXEC sp_ListarUsuarios")
                    .ToList();
            }
        }

        public void EliminarUsuario(int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                context.Database.ExecuteSqlRaw("EXEC sp_EliminarUsuario @IdUsuario", pIdUsuario);
            }
        }

        /// <summary>
        /// Modifica los datos de un usuario existente en la base de datos a través de sp_ModificarUsuario.
        /// Permite actualizar datos personales, documento, matrícula profesional médica y una o múltiples salas asignadas en DetallesSalas.
        /// Aplica parámetros parametrizados para proteger contra Inyección SQL y transacciones atómicas.
        /// </summary>
        /// <param name="idUsuario">ID del usuario a modificar.</param>
        /// <param name="nombre">Nombre actualizado.</param>
        /// <param name="apellido">Apellido actualizado.</param>
        /// <param name="correo">Correo electrónico actualizado.</param>
        /// <param name="dni">DNI actualizado.</param>
        /// <param name="telefono">Teléfono actualizado.</param>
        /// <param name="nroMatricula">Matrícula médica actualizada (si es médico).</param>
        /// <param name="idRol">Rol asignado.</param>
        /// <param name="salasIds">Lista de IDs de salas asignadas (NULL = no tocar salas, vacía = desasignar todas, con IDs = asignar múltiples salas).</param>
        /// <param name="descripcionAtencion">Notas u observaciones sobre la atención en la sala.</param>
        public void ModificarUsuario(int idUsuario, string nombre, string apellido, string correo, string dni, string telefono, string? nroMatricula, int? idRol, List<int>? salasIds, string? descripcionAtencion = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                // 1. Preparación de parámetros con protección contra Inyección SQL
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pNombre = new SqlParameter("@Nombre", nombre);
                var pApellido = new SqlParameter("@Apellido", apellido);
                var pCorreo = new SqlParameter("@Correo", correo);
                var pTelefono = new SqlParameter("@Telefono", (object?)telefono ?? DBNull.Value);
                var pDni = new SqlParameter("@Dni", (object?)dni ?? DBNull.Value);
                var pMatricula = new SqlParameter("@NroMatricula", (object?)nroMatricula ?? DBNull.Value);
                var pIdRol = new SqlParameter("@IdRol", idRol.HasValue && idRol.Value > 0 ? (object)idRol.Value : DBNull.Value);
                var pIdSala = new SqlParameter("@IdSala", DBNull.Value);
                var pDescripcion = new SqlParameter("@DescripcionAtencion", (object?)descripcionAtencion ?? DBNull.Value);

                // Convertimos la lista de salas a formato CSV para enviarla a STRING_SPLIT en SQL Server
                string? salasCsv = salasIds != null ? string.Join(",", salasIds) : null;
                var pSalasIds = new SqlParameter("@SalasIds", (object?)salasCsv ?? DBNull.Value);

                try
                {
                    // 2. Ejecutamos sp_ModificarUsuario con soporte de múltiples salas (@SalasIds)
                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_ModificarUsuario @IdUsuario, @Nombre, @Apellido, @Correo, @Telefono, @Dni, @NroMatricula, @IdRol, @IdSala, @DescripcionAtencion, @SalasIds",
                        pIdUsuario, pNombre, pApellido, pCorreo, pTelefono, pDni, pMatricula, pIdRol, pIdSala, pDescripcion, pSalasIds);
                }
                catch (SqlException ex) when (ex.Number == 8144) // Error 8144: Si el SP tuviera una versión anterior sin @SalasIds
                {
                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_ModificarUsuario @IdUsuario, @Nombre, @Apellido, @Correo, @Telefono, @Dni, @NroMatricula, @IdRol, @IdSala, @DescripcionAtencion",
                        pIdUsuario, pNombre, pApellido, pCorreo, pTelefono, pDni, pMatricula, pIdRol, pIdSala, pDescripcion);
                }
            }
        }

        /// <summary>
        /// Sobrecarga de compatibilidad para modificar un usuario enviando un único ID de sala.
        /// </summary>
        public void ModificarUsuario(int idUsuario, string nombre, string apellido, string correo, string dni, string telefono, string? nroMatricula, int? idRol, int? idSala = null, string? descripcionAtencion = null)
        {
            List<int>? salasIds = null;
            if (idSala.HasValue)
            {
                salasIds = new List<int>();
                if (idSala.Value > 0)
                    salasIds.Add(idSala.Value);
            }

            ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, nroMatricula, idRol, salasIds, descripcionAtencion);
        }
    }
}