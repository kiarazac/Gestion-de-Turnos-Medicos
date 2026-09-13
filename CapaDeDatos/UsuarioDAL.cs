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
        /// Aplica parámetros parametrizados para proteger contra Inyección SQL y soporta fallback de compatibilidad.
        /// </summary>
        public void ModificarUsuario(int idUsuario, string nombre, string apellido, string correo, string dni, string telefono, string? nroMatricula, int? idRol)
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

                try
                {
                    // 2. Intentamos ejecutar la versión completa de sp_ModificarUsuario (con DNI, Matrícula y Rol)
                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_ModificarUsuario @IdUsuario, @Nombre, @Apellido, @Correo, @Telefono, @Dni, @NroMatricula, @IdRol",
                        pIdUsuario, pNombre, pApellido, pCorreo, pTelefono, pDni, pMatricula, pIdRol);
                }
                catch (SqlException ex) when (ex.Number == 8144) // Error 8144: Demasiados argumentos si el SP sólo tenía 5 parámetros
                {
                    // 3. Fallback de compatibilidad: Si la base de datos tiene la versión previa de 5 parámetros
                    var pIdUsuarioOld = new SqlParameter("@IdUsuario", idUsuario);
                    var pNombreOld = new SqlParameter("@Nombre", nombre);
                    var pApellidoOld = new SqlParameter("@Apellido", apellido);
                    var pCorreoOld = new SqlParameter("@Correo", correo);
                    var pTelefonoOld = new SqlParameter("@Telefono", (object?)telefono ?? DBNull.Value);

                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_ModificarUsuario @IdUsuario, @Nombre, @Apellido, @Correo, @Telefono",
                        pIdUsuarioOld, pNombreOld, pApellidoOld, pCorreoOld, pTelefonoOld);
                }
            }
        }
    }
}