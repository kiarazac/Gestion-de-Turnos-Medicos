using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ResultadosSQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) para la persistencia, consulta y administración de usuarios,
    /// autenticación y asignaciones de roles, especialidades y consultorios mediante Stored Procedures.
    /// </summary>
    public class UsuarioDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ValidarLogin</c> para autenticar a un usuario mediante correo y contraseña.
        /// </summary>
        /// <param name="correo">Correo electrónico de acceso.</param>
        /// <param name="contrasena">Contraseña ingresada.</param>
        /// <returns>Instancia de <see cref="UsuarioLoginResult"/> si las credenciales son válidas y el usuario está activo; de lo contrario, <c>null</c>.</returns>
        public UsuarioLoginResult ValidarLogin(string correo, string contrasena)
        {
            using (var context = new dbTurnosMedicos())
            {
                var paramCorreo = new SqlParameter("@Correo", correo);
                var paramContrasena = new SqlParameter("@Contrasena", contrasena);

                var usuarioLogueado = context.Database.SqlQueryRaw<UsuarioLoginResult>(
                    "EXEC sp_ValidarLogin @Correo, @Contrasena",
                    paramCorreo,
                    paramContrasena
                ).AsEnumerable().FirstOrDefault();

                return usuarioLogueado;
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarRoles</c> para obtener la lista de roles activos.
        /// </summary>
        /// <returns>Lista de <see cref="RolDTO"/> con los identificadores y descripciones de roles.</returns>
        public List<RolDTO> ListarRoles()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<RolDTO>("EXEC sp_ListarRoles").ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarPersonalMedico</c> para obtener los profesionales de la salud habilitados.
        /// </summary>
        /// <returns>Lista de <see cref="MedicoDTO"/>.</returns>
        public List<MedicoDTO> ListarPersonalMedico()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<MedicoDTO>("EXEC sp_ListarPersonalMedico").ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_InsertarUsuario</c> para dar de alta a un usuario en la base de datos.
        /// </summary>
        /// <param name="nombre">Nombre del usuario.</param>
        /// <param name="apellido">Apellido del usuario.</param>
        /// <param name="correo">Correo electrónico único.</param>
        /// <param name="contrasena">Contraseña hasheada.</param>
        /// <param name="dni">DNI del usuario.</param>
        /// <param name="telefono">Teléfono de contacto.</param>
        /// <param name="nroMatricula">Matrícula médica si aplica.</param>
        /// <param name="idRol">ID del rol asignado.</param>
        /// <returns>ID único generado para el nuevo usuario (<c>IdNuevoUsuario</c>).</returns>
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

                var resultado = context.Database
                    .SqlQueryRaw<NuevoUsuarioIdDTO>("EXEC sp_InsertarUsuario @Nombre, @Apellido, @Correo, @Contrasena, @Dni, @Telefono, @NroMatricula, @IdRol",
                        pNombre, pApellido, pCorreo, pContrasena, pDni, pTelefono, pMatricula, pIdRol)
                    .AsEnumerable()
                    .FirstOrDefault();

                return resultado != null ? resultado.IdNuevoUsuario : 0;
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_AsignarEspecialidadMedico</c> para asociar una especialidad médica a un profesional.
        /// </summary>
        /// <param name="idUsuario">ID del médico.</param>
        /// <param name="idEspecialidad">ID de la especialidad.</param>
        public void AsignarEspecialidadMedico(int idUsuario, int idEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", idEspecialidad);

                context.Database.ExecuteSqlRaw("EXEC sp_AsignarEspecialidadMedico @IdUsuario, @IdEspecialidad", pIdUsuario, pIdEspecialidad);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_AsignarSalaMedico</c> para vincular a un profesional médico con una sala o consultorio.
        /// </summary>
        /// <param name="idSala">ID de la sala.</param>
        /// <param name="idUsuario">ID del médico.</param>
        /// <param name="descripcionAtencion">Observaciones del tipo de atención.</param>
        public void AsignarSalaMedico(int idSala, int idUsuario, string descripcionAtencion)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSala = new SqlParameter("@IdSala", idSala);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                var pDesc = new SqlParameter("@DescripcionAtencion", (object)descripcionAtencion ?? DBNull.Value);

                context.Database.ExecuteSqlRaw("EXEC sp_AsignarSalaMedico @IdSala, @IdUsuario, @DescripcionAtencion",
                    pIdSala, pIdUsuario, pDesc);
            }
        }

        /// <summary>
        /// Obtiene el listado de usuarios del sistema a través del procedimiento <c>sp_ListarUsuarios</c>.
        /// Permite incluir usuarios con baja lógica mediante el parámetro opcional <paramref name="incluirInactivos"/>.
        /// Cuenta con mecanismo de contingencia para ejecutar consulta SQL directa si el SP local no está actualizado.
        /// </summary>
        /// <param name="incluirInactivos">Si es <c>true</c>, retorna tanto usuarios activos como inactivos; si es <c>false</c>, solo activos.</param>
        /// <returns>Lista de <see cref="UsuarioListadoDTO"/>.</returns>
        public List<UsuarioListadoDTO> ListarUsuarios(bool incluirInactivos = false)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIncluir = new SqlParameter("@IncluirInactivos", incluirInactivos);
                try
                {
                    return context.Database
                        .SqlQueryRaw<UsuarioListadoDTO>("EXEC sp_ListarUsuarios @IncluirInactivos", pIncluir)
                        .ToList();
                }
                catch (Exception)
                {
                    // Fallback directo con subconsultas correlacionadas DISTINCT
                    string sql = @"
                        SELECT 
                            u.IdUsuario,
                            u.Nombre,
                            u.Apellido,
                            u.Correo,
                            u.Dni,
                            u.Telefono,
                            r.Descripcion AS Rol,
                            ISNULL(u.NroMatricula, '') AS NroMatricula,
                            ISNULL((
                                SELECT STRING_AGG(e.Nombre, ', ')
                                FROM (
                                    SELECT DISTINCT e2.Nombre
                                    FROM MedicosEspecialidades me2
                                    INNER JOIN Especialidades e2 ON me2.IdEspecialidad = e2.IdEspecialidad
                                    WHERE me2.IdUsuario = u.IdUsuario AND (me2.Activo = 1 OR u.Activo = 0) AND e2.Activo = 1
                                ) e
                            ), '') AS Especialidades,
                            ISNULL((
                                SELECT STRING_AGG(s.NombreSala, ', ')
                                FROM (
                                    SELECT DISTINCT s2.NombreSala
                                    FROM DetallesSalas ds2
                                    INNER JOIN Salas s2 ON ds2.IdSala = s2.IdSala
                                    WHERE ds2.IdUsuario = u.IdUsuario AND (ds2.Activo = 1 OR u.Activo = 0) AND s2.Activo = 1
                                ) s
                            ), '') AS Salas,
                            u.Activo
                        FROM Usuarios u
                        INNER JOIN Roles r ON u.IdRol = r.IdRol
                        WHERE (@IncluirInactivos = 1 OR u.Activo = 1)
                        ORDER BY u.Activo DESC, u.Apellido, u.Nombre;";

                    return context.Database
                        .SqlQueryRaw<UsuarioListadoDTO>(sql, pIncluir)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Obtiene un usuario específico buscando por su número de DNI.
        /// Permite encontrar registros con baja lógica para posibilitar su re-dar de alta / reactivación.
        /// </summary>
        /// <param name="dni">DNI a verificar.</param>
        /// <param name="incluirInactivos">Si es <c>true</c>, busca tanto en usuarios activos como inactivos.</param>
        /// <returns>Instancia de <see cref="UsuarioListadoDTO"/> si fue hallado; de lo contrario, <c>null</c>.</returns>
        public UsuarioListadoDTO? ObtenerUsuarioPorDni(string dni, bool incluirInactivos = true)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pDni = new SqlParameter("@Dni", dni.Trim());
                var pIncluir = new SqlParameter("@IncluirInactivos", incluirInactivos);

                return context.Database
                    .SqlQueryRaw<UsuarioListadoDTO>("EXEC sp_ObtenerUsuarioPorDni @Dni, @IncluirInactivos", pDni, pIncluir)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
        }


        /// <summary>
        /// Da de baja lógica a un usuario en el sistema a través de <c>sp_EliminarUsuario</c>.
        /// Solo marca <c>Activo = 0</c> al usuario sin borrar sus atributos (salas y especialidades asignadas).
        /// </summary>
        /// <param name="idUsuario">ID único del usuario a desactivar.</param>
        public void EliminarUsuario(int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_EliminarUsuario @IdUsuario", pIdUsuario);
                }
                catch (SqlException ex) when (ex.Number == 2812)
                {
                    context.Database.ExecuteSqlRaw(
                        "UPDATE Usuarios SET Activo = 0, FechaBaja = GETDATE() WHERE IdUsuario = @IdUsuario",
                        pIdUsuario);
                }
            }
        }

        /// <summary>
        /// Reactiva a un usuario con baja lógica previa (<c>Activo = 0</c>),
        /// restaurando <c>Activo = 1</c> y limpiando la fecha de baja.
        /// Ejecuta <c>sp_ReactivarUsuario</c> con contingencia SQL directa en caso de no estar creado en el motor SQL Server.
        /// </summary>
        /// <param name="idUsuario">ID único del usuario a reactivar.</param>
        public void ReactivarUsuario(int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                try
                {
                    context.Database.ExecuteSqlRaw("EXEC sp_ReactivarUsuario @IdUsuario", pIdUsuario);
                }
                catch (SqlException ex) when (ex.Number == 2812)
                {
                    context.Database.ExecuteSqlRaw(
                        @"UPDATE Usuarios SET Activo = 1, FechaBaja = NULL, FechaModificacion = GETDATE() WHERE IdUsuario = @IdUsuario;
                          UPDATE DetallesSalas SET Activo = 1, FechaBaja = NULL WHERE IdUsuario = @IdUsuario;
                          UPDATE MedicosEspecialidades SET Activo = 1, FechaBaja = NULL WHERE IdUsuario = @IdUsuario;",
                        pIdUsuario);
                }
            }
        }

        /// <summary>
        /// Modifica los datos de un usuario existente en la base de datos a través de <c>sp_ModificarUsuario</c>.
        /// Respeta la firma del procedimiento almacenado en <c>dbGestionTurnos</c> (8 parámetros).
        /// Orquesta de forma atómica la reasignación de consultorios en <c>DetallesSalas</c> y especialidades en <c>MedicosEspecialidades</c>.
        /// </summary>
        /// <param name="idUsuario">ID único del usuario a modificar.</param>
        /// <param name="nombre">Nombre actualizado.</param>
        /// <param name="apellido">Apellido actualizado.</param>
        /// <param name="correo">Correo electrónico actualizado.</param>
        /// <param name="dni">DNI actualizado.</param>
        /// <param name="telefono">Teléfono actualizado de contacto.</param>
        /// <param name="nroMatricula">Matrícula médica profesional (si aplica).</param>
        /// <param name="idRol">ID del rol asignado.</param>
        /// <param name="salasIds">Lista opcional de IDs de salas asignadas (null = no alterar; vacía = desasignar todas; con IDs = reasignar).</param>
        /// <param name="descripcionAtencion">Notas u observaciones sobre la atención en la sala.</param>
        /// <param name="especialidadesIds">Lista opcional de IDs de especialidades asignadas al médico (null = no alterar; vacía = desasignar todas; con IDs = reasignar).</param>
        /// <param name="nuevaContrasenaHash">Hash de nueva contraseña si se desea actualizar las credenciales del usuario (opcional).</param>
        public void ModificarUsuario(
            int idUsuario,
            string nombre,
            string apellido,
            string correo,
            string dni,
            string telefono,
            string? nroMatricula,
            int? idRol,
            List<int>? salasIds = null,
            string? descripcionAtencion = null,
            List<int>? especialidadesIds = null,
            string? nuevaContrasenaHash = null)
        {
            using (var context = new dbTurnosMedicos())
            {
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
                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_ModificarUsuario @IdUsuario, @Nombre, @Apellido, @Correo, @Telefono, @Dni, @NroMatricula, @IdRol",
                        pIdUsuario, pNombre, pApellido, pCorreo, pTelefono, pDni, pMatricula, pIdRol);
                }
                catch (SqlException ex) when (ex.Number == 8144)
                {
                    var pIdUsuarioOld = new SqlParameter("@IdUsuario", idUsuario);
                    var pNombreOld = new SqlParameter("@Nombre", nombre);
                    var pApellidoOld = new SqlParameter("@Apellido", apellido);
                    var pCorreoOld = new SqlParameter("@Correo", correo);
                    var pTelefonoOld = new SqlParameter("@Telefono", (object?)telefono ?? DBNull.Value);

                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_ModificarUsuario @IdUsuario, @Nombre, @Apellido, @Correo, @Telefono",
                        pIdUsuarioOld, pNombreOld, pApellidoOld, pCorreoOld, pTelefonoOld);

                    var pDniExtra = new SqlParameter("@Dni", (object?)dni ?? DBNull.Value);
                    var pMatriculaExtra = new SqlParameter("@NroMatricula", (object?)nroMatricula ?? DBNull.Value);
                    var pIdRolExtra = new SqlParameter("@IdRol", idRol.HasValue && idRol.Value > 0 ? (object)idRol.Value : DBNull.Value);
                    var pIdUserExtra = new SqlParameter("@IdUsuario", idUsuario);
                    context.Database.ExecuteSqlRaw(
                        "UPDATE Usuarios SET Dni = ISNULL(@Dni, Dni), NroMatricula = @NroMatricula, IdRol = ISNULL(@IdRol, IdRol), FechaModificacion = GETDATE() WHERE IdUsuario = @IdUsuario",
                        pDniExtra, pMatriculaExtra, pIdRolExtra, pIdUserExtra);
                }

                if (!string.IsNullOrWhiteSpace(nuevaContrasenaHash))
                {
                    var pPass = new SqlParameter("@Contrasena", nuevaContrasenaHash);
                    var pUserPass = new SqlParameter("@IdUsuario", idUsuario);
                    context.Database.ExecuteSqlRaw(
                        "UPDATE Usuarios SET Contrasena = @Contrasena, FechaModificacion = GETDATE() WHERE IdUsuario = @IdUsuario",
                        pPass, pUserPass);
                }

                if (salasIds != null)
                {
                    ReasignarSalasUsuario(context, idUsuario, salasIds, descripcionAtencion);
                }

                if (especialidadesIds != null)
                {
                    ReasignarEspecialidadesUsuario(context, idUsuario, especialidadesIds);
                }
            }
        }

        /// <summary>
        /// Aplica una baja lógica a las vinculaciones activas de salas de un usuario y registra las nuevas asignaciones
        /// mediante el Stored Procedure <c>sp_AsignarSalaMedico</c>.
        /// </summary>
        /// <param name="context">Contexto de base de datos en ejecución.</param>
        /// <param name="idUsuario">ID único del profesional médico.</param>
        /// <param name="salasIds">Lista de identificadores de salas a asignar.</param>
        /// <param name="descripcionAtencion">Notas de atención o guardia.</param>
        public void ReasignarSalasUsuario(dbTurnosMedicos context, int idUsuario, List<int> salasIds, string? descripcionAtencion = null)
        {
            var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
            context.Database.ExecuteSqlRaw(
                "UPDATE DetallesSalas SET Activo = 0, FechaBaja = GETDATE() WHERE IdUsuario = @IdUsuario AND Activo = 1",
                pIdUsuario);

            if (salasIds != null && salasIds.Count > 0)
            {
                foreach (int idSala in salasIds)
                {
                    var pSala = new SqlParameter("@IdSala", idSala);
                    var pUser = new SqlParameter("@IdUsuario", idUsuario);
                    var pDesc = new SqlParameter("@DescripcionAtencion", (object?)descripcionAtencion ?? string.Empty);
                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_AsignarSalaMedico @IdSala, @IdUsuario, @DescripcionAtencion",
                        pSala, pUser, pDesc);
                }
            }
        }

        /// <summary>
        /// Aplica una baja lógica a las especialidades activas de un médico y registra las nuevas vinculaciones
        /// mediante el Stored Procedure <c>sp_AsignarEspecialidadMedico</c>.
        /// </summary>
        /// <param name="context">Contexto de base de datos en ejecución.</param>
        /// <param name="idUsuario">ID del médico.</param>
        /// <param name="especialidadesIds">Lista de IDs de especialidades asignadas.</param>
        public void ReasignarEspecialidadesUsuario(dbTurnosMedicos context, int idUsuario, List<int> especialidadesIds)
        {
            var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
            context.Database.ExecuteSqlRaw(
                "UPDATE MedicosEspecialidades SET Activo = 0, FechaBaja = GETDATE() WHERE IdUsuario = @IdUsuario AND Activo = 1",
                pIdUsuario);

            if (especialidadesIds != null && especialidadesIds.Count > 0)
            {
                foreach (int idEspecialidad in especialidadesIds)
                {
                    var pUser = new SqlParameter("@IdUsuario", idUsuario);
                    var pEsp = new SqlParameter("@IdEspecialidad", idEspecialidad);
                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_AsignarEspecialidadMedico @IdUsuario, @IdEspecialidad",
                        pUser, pEsp);
                }
            }
        }

        /// <summary>
        /// Sobrecarga de compatibilidad para modificar un usuario enviando salasIds y descripcionAtencion sin especialidades ni clave.
        /// </summary>
        public void ModificarUsuario(int idUsuario, string nombre, string apellido, string correo, string dni, string telefono, string? nroMatricula, int? idRol, List<int>? salasIds, string? descripcionAtencion = null)
        {
            ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, nroMatricula, idRol, salasIds, descripcionAtencion, null, null);
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

            ModificarUsuario(idUsuario, nombre, apellido, correo, dni, telefono, nroMatricula, idRol, salasIds, descripcionAtencion, null, null);
        }
    }
}