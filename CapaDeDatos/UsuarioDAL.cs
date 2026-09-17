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
        /// <summary>
        /// Obtiene el listado de usuarios del sistema a través del procedimiento sp_ListarUsuarios.
        /// Permite incluir usuarios con baja lógica mediante el parámetro opcional incluirInactivos.
        /// Cuenta con mecanismo de contingencia para ejecutar consulta SQL parametrizada directa si el SP local no está actualizado.
        /// </summary>
        /// <param name="incluirInactivos">Si es true, retorna tanto usuarios activos como inactivos; si es false, solo activos.</param>
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
                    // Fallback directo con consulta SQL para garantizar disponibilidad inmediata
                    // si la base de datos local aún no actualizó el procedimiento sp_ListarUsuarios o no proyecta Activo.
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
                            ISNULL(STRING_AGG(e.Nombre, ', '), '') AS Especialidades,
                            ISNULL(STRING_AGG(s.NombreSala, ', '), '') AS Salas,
                            u.Activo
                        FROM Usuarios u
                        INNER JOIN Roles r ON u.IdRol = r.IdRol
                        LEFT JOIN MedicosEspecialidades me ON u.IdUsuario = me.IdUsuario AND me.Activo = 1
                        LEFT JOIN Especialidades e ON me.IdEspecialidad = e.IdEspecialidad AND e.Activo = 1
                        LEFT JOIN DetallesSalas ds ON u.IdUsuario = ds.IdUsuario AND ds.Activo = 1
                        LEFT JOIN Salas s ON ds.IdSala = s.IdSala AND s.Activo = 1
                        WHERE (@IncluirInactivos = 1 OR u.Activo = 1)
                        GROUP BY u.IdUsuario, u.Nombre, u.Apellido, u.Correo, u.Dni, u.Telefono, r.Descripcion, u.NroMatricula, u.Activo
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
        /// <param name="incluirInactivos">Si es true, busca tanto en usuarios activos como inactivos.</param>
        public UsuarioListadoDTO? ObtenerUsuarioPorDni(string dni, bool incluirInactivos = true)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pDni = new SqlParameter("@Dni", dni.Trim());
                var pIncluir = new SqlParameter("@IncluirInactivos", incluirInactivos);

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
                        ISNULL(STRING_AGG(e.Nombre, ', '), '') AS Especialidades,
                        ISNULL(STRING_AGG(s.NombreSala, ', '), '') AS Salas,
                        u.Activo
                    FROM Usuarios u
                    INNER JOIN Roles r ON u.IdRol = r.IdRol
                    LEFT JOIN MedicosEspecialidades me ON u.IdUsuario = me.IdUsuario AND me.Activo = 1
                    LEFT JOIN Especialidades e ON me.IdEspecialidad = e.IdEspecialidad AND e.Activo = 1
                    LEFT JOIN DetallesSalas ds ON u.IdUsuario = ds.IdUsuario AND ds.Activo = 1
                    LEFT JOIN Salas s ON ds.IdSala = s.IdSala AND s.Activo = 1
                    WHERE u.Dni = @Dni AND (@IncluirInactivos = 1 OR u.Activo = 1)
                    GROUP BY u.IdUsuario, u.Nombre, u.Apellido, u.Correo, u.Dni, u.Telefono, r.Descripcion, u.NroMatricula, u.Activo;";

                return context.Database
                    .SqlQueryRaw<UsuarioListadoDTO>(sql, pDni, pIncluir)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Da de baja lógica a un usuario en el sistema a través de sp_EliminarUsuario.
        /// </summary>
        /// <param name="idUsuario">ID único del usuario a desactivar.</param>
        public void EliminarUsuario(int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
                context.Database.ExecuteSqlRaw("EXEC sp_EliminarUsuario @IdUsuario", pIdUsuario);
            }
        }

        /// <summary>
        /// Reactiva o re-da de alta a un usuario con baja lógica previa (Activo = 0),
        /// restaurando Activo = 1 y limpiando la fecha de baja.
        /// Ejecuta sp_ReactivarUsuario con contingencia SQL directa en caso de no estar creado en el motor SQL Server.
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
                catch (SqlException ex) when (ex.Number == 2812) // Error 2812: No se encontró el SP sp_ReactivarUsuario
                {
                    // Contingencia directa para compatibilidad inmediata si la BD aún no tiene el procedimiento almacenado creado
                    context.Database.ExecuteSqlRaw(
                        "UPDATE Usuarios SET Activo = 1, FechaBaja = NULL, FechaModificacion = GETDATE() WHERE IdUsuario = @IdUsuario",
                        pIdUsuario);
                }
            }
        }

        /// <summary>
        /// Modifica los datos de un usuario existente en la base de datos a través de sp_ModificarUsuario.
        /// Respeta estrictamente la firma del procedimiento almacenado en dbGestionTurnos (8 parámetros: @IdUsuario, @Nombre, @Apellido, @Correo, @Telefono, @Dni, @NroMatricula, @IdRol).
        /// Adicionalmente, orquesta de forma atómica la reasignación de salas en DetallesSalas y especialidades en MedicosEspecialidades
        /// mediante borrado lógico e inserción con los procedimientos sp_AsignarSalaMedico y sp_AsignarEspecialidadMedico.
        /// </summary>
        /// <param name="idUsuario">ID único del usuario a modificar.</param>
        /// <param name="nombre">Nombre actualizado.</param>
        /// <param name="apellido">Apellido actualizado.</param>
        /// <param name="correo">Correo electrónico actualizado.</param>
        /// <param name="dni">DNI actualizado.</param>
        /// <param name="telefono">Teléfono actualizado de contacto.</param>
        /// <param name="nroMatricula">Matrícula médica profesional (si aplica).</param>
        /// <param name="idRol">ID del rol asignado.</param>
        /// <param name="salasIds">Lista opcional de IDs de salas asignadas (null = no alterar salas; vacía = desasignar todas; con IDs = reasignar).</param>
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
                // 1. Preparación de parámetros con tipos explícitos para proteger contra Inyección SQL
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
                    // 2. Invocamos sp_ModificarUsuario con los 8 parámetros que soporta la base de datos real dbGestionTurnos
                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_ModificarUsuario @IdUsuario, @Nombre, @Apellido, @Correo, @Telefono, @Dni, @NroMatricula, @IdRol",
                        pIdUsuario, pNombre, pApellido, pCorreo, pTelefono, pDni, pMatricula, pIdRol);
                }
                catch (SqlException ex) when (ex.Number == 8144) // Error 8144: Si el SP en un entorno legacy tuviera solo 5 parámetros
                {
                    // Fallback para bases de datos con la versión histórica de 5 parámetros de sp_ModificarUsuario
                    var pIdUsuarioOld = new SqlParameter("@IdUsuario", idUsuario);
                    var pNombreOld = new SqlParameter("@Nombre", nombre);
                    var pApellidoOld = new SqlParameter("@Apellido", apellido);
                    var pCorreoOld = new SqlParameter("@Correo", correo);
                    var pTelefonoOld = new SqlParameter("@Telefono", (object?)telefono ?? DBNull.Value);

                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_ModificarUsuario @IdUsuario, @Nombre, @Apellido, @Correo, @Telefono",
                        pIdUsuarioOld, pNombreOld, pApellidoOld, pCorreoOld, pTelefonoOld);

                    // Actualizamos los campos adicionales (Dni, NroMatricula, IdRol) de forma directa en Usuarios
                    var pDniExtra = new SqlParameter("@Dni", (object?)dni ?? DBNull.Value);
                    var pMatriculaExtra = new SqlParameter("@NroMatricula", (object?)nroMatricula ?? DBNull.Value);
                    var pIdRolExtra = new SqlParameter("@IdRol", idRol.HasValue && idRol.Value > 0 ? (object)idRol.Value : DBNull.Value);
                    var pIdUserExtra = new SqlParameter("@IdUsuario", idUsuario);
                    context.Database.ExecuteSqlRaw(
                        "UPDATE Usuarios SET Dni = ISNULL(@Dni, Dni), NroMatricula = @NroMatricula, IdRol = ISNULL(@IdRol, IdRol), FechaModificacion = GETDATE() WHERE IdUsuario = @IdUsuario",
                        pDniExtra, pMatriculaExtra, pIdRolExtra, pIdUserExtra);
                }

                // 3. Si se proporcionó una nueva contraseña hasheada, actualizamos las credenciales del usuario
                if (!string.IsNullOrWhiteSpace(nuevaContrasenaHash))
                {
                    var pPass = new SqlParameter("@Contrasena", nuevaContrasenaHash);
                    var pUserPass = new SqlParameter("@IdUsuario", idUsuario);
                    context.Database.ExecuteSqlRaw(
                        "UPDATE Usuarios SET Contrasena = @Contrasena, FechaModificacion = GETDATE() WHERE IdUsuario = @IdUsuario",
                        pPass, pUserPass);
                }

                // 4. Reasignación de salas en DetallesSalas si se especificó la lista (soporta desasignación si la lista viene vacía)
                if (salasIds != null)
                {
                    ReasignarSalasUsuario(context, idUsuario, salasIds, descripcionAtencion);
                }

                // 5. Reasignación de especialidades médicas en MedicosEspecialidades si se especificó la lista
                if (especialidadesIds != null)
                {
                    ReasignarEspecialidadesUsuario(context, idUsuario, especialidadesIds);
                }
            }
        }

        /// <summary>
        /// Aplica una baja lógica a las vinculaciones activas de salas de un usuario y registra las nuevas asignaciones
        /// mediante el Stored Procedure existente sp_AsignarSalaMedico.
        /// </summary>
        public void ReasignarSalasUsuario(dbTurnosMedicos context, int idUsuario, List<int> salasIds, string? descripcionAtencion = null)
        {
            // 1. Damos de baja lógica las asignaciones previas activas en DetallesSalas
            var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
            context.Database.ExecuteSqlRaw(
                "UPDATE DetallesSalas SET Activo = 0, FechaBaja = GETDATE() WHERE IdUsuario = @IdUsuario AND Activo = 1",
                pIdUsuario);

            // 2. Insertamos las nuevas salas seleccionadas utilizando el Stored Procedure existente sp_AsignarSalaMedico
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
        /// Aplica una baja lógica a las especialidades activas asignadas a un médico y registra las nuevas vinculaciones
        /// mediante el Stored Procedure existente sp_AsignarEspecialidadMedico.
        /// </summary>
        public void ReasignarEspecialidadesUsuario(dbTurnosMedicos context, int idUsuario, List<int> especialidadesIds)
        {
            // 1. Damos de baja lógica las especialidades previas activas en MedicosEspecialidades
            var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);
            context.Database.ExecuteSqlRaw(
                "UPDATE MedicosEspecialidades SET Activo = 0, FechaBaja = GETDATE() WHERE IdUsuario = @IdUsuario AND Activo = 1",
                pIdUsuario);

            // 2. Insertamos las nuevas especialidades seleccionadas utilizando el Stored Procedure existente sp_AsignarEspecialidadMedico
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