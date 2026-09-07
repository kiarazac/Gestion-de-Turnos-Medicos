using System;
using System.Collections.Generic;
using System.Text;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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

                return context.Database
                    .SqlQueryRaw<int>("EXEC sp_InsertarUsuario @Nombre, @Apellido, @Correo, @Contrasena, @Dni, @Telefono, @NroMatricula, @IdRol",
                        pNombre, pApellido, pCorreo, pContrasena, pDni, pTelefono, pMatricula, pIdRol)
                    .AsEnumerable()
                    .FirstOrDefault();
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

        public void AsignarSalaMedico(int idUsuario, int idSala, string? notaSala = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pDesc = new SqlParameter("@DescripcionAtencion", (object?)notaSala ?? DBNull.Value);

                context.Database.ExecuteSqlRaw("EXEC sp_AsignarSalaMedico @IdSala, @IdUsuario, @DescripcionAtencion", pIdSala, pIdUsuario, pDesc);
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
    }
}