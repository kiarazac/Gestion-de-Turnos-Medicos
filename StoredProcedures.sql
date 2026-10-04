/* =========================================================================
** SISTEMA DE GESTIÓN DE TURNOS MÉDICOS
** BASE DE DATOS: dbGestionTurnos (o GestionTurnosMedicos)
** MOTOR: Microsoft SQL Server 2019+ / Azure SQL / SQL Server Docker
** DOCUMENTACIÓN OFICIAL DE STORED PROCEDURES
**
** DESCRIPCIÓN:
** Este script contiene las definiciones completas, auditables y documentadas
** de todos los Stored Procedures requeridos por la arquitectura en capas
** de la aplicación (Forms -> BLL -> DAL -> Stored Procedures).
**
** CONVENCIONES Y REGLAS DE DOMINIO:
** - Borrado Lógico: Activo = 0, FechaBaja = GETDATE()
** - Auditoría: FechaCreacion, FechaModificacion, FechaBaja
** - Resiliencia: SET NOCOUNT ON en todos los procedimientos
** - Parámetros con tipos consistentes con el modelo EF Core (Modelos.cs)
** ========================================================================= */

USE dbGestionTurnos;

GO

/* =========================================================================
** Procedimiento : sp_ValidarLogin
** Sección       : 1.1
** Propósito     : Valida las credenciales de acceso (correo y contraseña) comprobando que el usuario esté activo. Retorna la información personal y la descripción del rol.
** Entidad/Tablas: `Usuarios`, `Roles`
** Invocado por  : `FrmLogin` (Botón `button1` ("Iniciar Sesión"))
** Estado        : `EN USO`
** Retorno       : `IdUsuario`, `Nombre`, `Apellido`, `Correo`, `IdRol`, `NombreRol`
** Parámetros   :
**                `@Correo` (`NVARCHAR(150)`, IN) - Correo electrónico de acceso.
**                `@Contrasena` (`NVARCHAR(255)`, IN) - Contraseña (o hash de contraseña).
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ValidarLogin
    @Correo NVARCHAR(150),
    @Contrasena NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validación de campos vacíos
        IF LTRIM(RTRIM(ISNULL(@Correo, ''))) = '' OR LTRIM(RTRIM(ISNULL(@Contrasena, ''))) = ''
        BEGIN
            THROW 50020, 'Debe ingresar obligatoriamente el correo electrónico y la contraseña.', 1;
        END

        -- 2. Verificar si el correo existe en la base de datos (independientemente de si está activo o no)
        IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo)
        BEGIN
            THROW 50021, 'El correo electrónico ingresado no se encuentra registrado en el sistema.', 1;
        END

        -- 3. Verificar si el usuario está dado de baja (Activo = 0)
        IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo AND Activo = 0)
        BEGIN
            THROW 50022, 'Este usuario se encuentra dado de baja en el sistema. Contacte al administrador.', 1;
        END

        -- 4. Verificar si la contraseña es incorrecta para ese correo
        IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo AND Contrasena = @Contrasena AND Activo = 1)
        BEGIN
            THROW 50023, 'La contraseña ingresada es incorrecta. Verifique sus datos e intente nuevamente.', 1;
        END

        -- 5. Si todo es correcto, devolvemos los datos del usuario y su rol
        SELECT 
            u.IdUsuario, 
            u.Nombre, 
            u.Apellido, 
            u.Correo, 
            u.IdRol, 
            r.Descripcion AS NombreRol
        FROM Usuarios u
        INNER JOIN Roles r ON u.IdRol = r.IdRol
        WHERE u.Correo = @Correo 
          AND u.Contrasena = @Contrasena 
          AND u.Activo = 1;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SET @ErrorSeverity = ERROR_SEVERITY();
        SET @ErrorState = ERROR_STATE();

        IF ERROR_NUMBER() NOT BETWEEN 50020 AND 50029
        BEGIN
            SET @ErrorMessage = 'El sistema no se encuentra disponible temporalmente o hay problemas de conexión con la Base de Datos. Intente más tarde.';
            SET @ErrorSeverity = 16;
            SET @ErrorState = 1;
        END
        ELSE
        BEGIN
            SET @ErrorMessage = ERROR_MESSAGE();
        END

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ListarRoles
** Sección       : 1.2
** Propósito     : Obtiene todos los roles activos configurados en el sistema para poblar el selector desplegable de asignación de roles.
** Entidad/Tablas: `Roles`
** Invocado por  : `FrmGestionUsuarios` (Evento `Load` / `CargarRolesDesdeBD`)
** Estado        : `PENDIENTE DE IMPLEMENTACIÓN`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   : Ninguno.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ListarRoles
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT IdRol, Descripcion
    FROM Roles
    WHERE Activo = 1
    ORDER BY IdRol ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_InsertarRol
** Sección       : 1.3
** Propósito     : Da de alta un nuevo perfil de rol en el sistema.
** Entidad/Tablas: `Roles`
** Invocado por  : Ninguno actualmente (se inicializan por script de Seeds). (N/A)
** Estado        : `NO UTILIZADO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@Descripcion` (`NVARCHAR(50)`, IN) - Nombre identificatorio del rol.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_InsertarRol
    @Descripcion NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    INSERT INTO Roles (Descripcion, Activo, FechaCreacion)
    VALUES (@Descripcion, 1, GETDATE());
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ListarUsuarios
** Sección       : 1.4
** Propósito     : Obtiene el listado de usuarios del sistema para la grilla de administración, concatenando sus especialidades médicas y salas asignadas. Soporta el parámetro opcional `@IncluirInactivos` para listar tanto usuarios activos como aquellos dados de baja lógica.
** Entidad/Tablas: `Usuarios`, `Roles`, `MedicosEspecialidades`, `Especialidades`, `DetallesSalas`, `Salas`
** Invocado por  : `FrmGestionUsuarios2` (Evento `Load` / `CargarUsuariosDesdeBD` / CheckBox `chkMostrarInactivos`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IncluirInactivos` (`BIT`, IN) - Opcional (por defecto `0`). Si es `0`, lista solo usuarios con `Activo = 1`. Si es `1`, incluye también usuarios con `Activo = 0`.
** ========================================================================= */
-- =========================================================================
-- Procedimiento: sp_ListarUsuarios
-- Descripción: Obtiene el listado de usuarios con roles, salas y especialidades.
-- Soporta filtrado opcional de usuarios dados de baja (@IncluirInactivos).
-- Optimización: Resuelve el producto cartesiano de múltiples salas y especialidades
-- mediante subconsultas correlacionadas con DISTINCT, evitando duplicaciones ("Sala I, Sala I").
-- Preserva asignaciones de salas y especialidades tanto para usuarios activos como inactivos.
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_ListarUsuarios
    @IncluirInactivos BIT = 0 -- 0: solo activos (por defecto), 1: activos e inactivos
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        u.IdUsuario,
        u.Nombre,
        u.Apellido,
        u.Correo,
        u.Dni,
        u.Telefono,
        r.Descripcion AS Rol,
        ISNULL(u.NroMatricula, '') AS NroMatricula,
        -- Subconsulta correlacionada con DISTINCT para evitar productos cartesianos y duplicaciones de especialidades
        ISNULL((
            SELECT STRING_AGG(subE.Nombre, ', ')
            FROM (
                SELECT DISTINCT e2.Nombre
                FROM MedicosEspecialidades me2
                INNER JOIN Especialidades e2 ON me2.IdEspecialidad = e2.IdEspecialidad
                WHERE me2.IdUsuario = u.IdUsuario
                  AND (me2.Activo = 1 OR u.Activo = 0)
                  AND e2.Activo = 1
            ) subE
        ), '') AS Especialidades,
        -- Subconsulta correlacionada con DISTINCT para evitar duplicaciones de salas ("Sala I, Sala I")
        ISNULL((
            SELECT STRING_AGG(subS.NombreSala, ', ')
            FROM (
                SELECT DISTINCT s2.NombreSala
                FROM DetallesSalas ds2
                INNER JOIN Salas s2 ON ds2.IdSala = s2.IdSala
                WHERE ds2.IdUsuario = u.IdUsuario
                  AND (ds2.Activo = 1 OR u.Activo = 0)
                  AND s2.Activo = 1
            ) subS
        ), '') AS Salas,
        u.Activo -- Proyección del estado de auditoría para la grilla
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    -- Filtrado condicional: si @IncluirInactivos = 1 devuelve todos; si es 0, solo u.Activo = 1
    WHERE (@IncluirInactivos = 1 OR u.Activo = 1)
    ORDER BY u.Activo DESC, u.Apellido, u.Nombre;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_InsertarUsuario
** Sección       : 1.5
** Propósito     : Registra un nuevo usuario en la tabla `Usuarios` asignándole su rol correspondiente y devuelve el `IdUsuario` generado. Cuenta con validación contra colisión de DNI con pacientes y usuarios.
** Entidad/Tablas: `Usuarios`, `Pacientes`
** Invocado por  : `FrmGestionUsuarios2` (Botón `btnGuardar`)
** Estado        : `EN USO`
** Retorno       : `IdNuevoUsuario` (INT)
** Parámetros   :
**                `@Nombre` (`NVARCHAR(100)`, IN) - Nombre del usuario.
**                `@Apellido` (`NVARCHAR(100)`, IN) - Apellido del usuario.
**                `@Correo` (`NVARCHAR(150)`, IN) - Correo electrónico único.
**                `@Contrasena` (`NVARCHAR(255)`, IN) - Contraseña (hasheada en C#).
**                `@Dni` (`NVARCHAR(20)`, IN) - Documento de identidad.
**                `@Telefono` (`NVARCHAR(20)`, IN) - Teléfono de contacto.
**                `@NroMatricula` (`NVARCHAR(50)`, IN) - Número de matrícula (opcional/médicos).
**                `@IdRol` (`INT`, IN) - ID del rol asignado.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_InsertarUsuario
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Correo NVARCHAR(150),
    @Contrasena NVARCHAR(255),
    @Dni NVARCHAR(20),
    @Telefono NVARCHAR(20),
    @NroMatricula NVARCHAR(50),
    @IdRol INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar si el DNI ya existe como Paciente activo en el sistema
        IF EXISTS (SELECT 1 FROM Pacientes WHERE Dni = @Dni AND Activo = 1)
        BEGIN
            THROW 50011, 'Ya existe un paciente registrado en el sistema con este mismo número de DNI. Un usuario del sistema no puede duplicar el DNI de un paciente.', 1;
        END

        -- Validar también si ya existe en la misma tabla de Usuarios por seguridad
        IF EXISTS (SELECT 1 FROM Usuarios WHERE Dni = @Dni AND Activo = 1)
        BEGIN
            THROW 50012, 'Ya existe otro usuario registrado con este número de DNI.', 1;
        END

        INSERT INTO Usuarios (Nombre, Apellido, Correo, Contrasena, Dni, Telefono, NroMatricula, IdRol, Activo, FechaCreacion)
        VALUES (@Nombre, @Apellido, @Correo, @Contrasena, @Dni, @Telefono, @NroMatricula, @IdRol, 1, GETDATE());

        -- Se castea explícitamente el resultado a INT para que EF Core lo pueda mapear al DTO
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdNuevoUsuario;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ModificarUsuario
** Sección       : 1.6
** Propósito     : Actualiza los datos de filiación, contacto, identificación, matrícula profesional y rol de un usuario existente en la tabla `Usuarios`. Cuenta con control de transacciones y validaciones de integridad de negocio.
** Entidad/Tablas: `Usuarios`, `Roles`, `Pacientes`
** Invocado por  : `FrmGestionUsuarios2` (versión oficial activa en el sistema de turnos; `FrmGestionUsuarios` mantenido como referencia/legado) (Botón `btnModificar` en `FrmGestionUsuarios2` y edición en grilla)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdUsuario` (`INT`, IN) - ID del usuario a modificar.
**                `@Nombre` (`NVARCHAR(100)`, IN) - Nuevo nombre.
**                `@Apellido` (`NVARCHAR(100)`, IN) - Nuevo apellido.
**                `@Correo` (`NVARCHAR(150)`, IN) - Nuevo correo electrónico.
**                `@Telefono` (`NVARCHAR(20)`, IN) - Nuevo teléfono.
**                `@Dni` (`NVARCHAR(20)`, IN) - Nuevo número de DNI (opcional, conserva anterior si es NULL).
**                `@NroMatricula` (`NVARCHAR(50)`, IN) - Nueva matrícula médica (opcional/médicos).
**                `@IdRol` (`INT`, IN) - Nuevo rol asignado (opcional, conserva anterior si es NULL).
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ModificarUsuario
    @IdUsuario INT,
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Correo NVARCHAR(150),
    @Telefono NVARCHAR(20),
    @Dni NVARCHAR(20) = NULL,
    @NroMatricula NVARCHAR(50) = NULL,
    @IdRol INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el usuario a modificar exista y esté activo
        IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario AND Activo = 1)
        BEGIN
            THROW 50030, 'El usuario a modificar no existe o se encuentra inactivo en el sistema.', 1;
        END

        -- 2. Validar que los campos obligatorios no vengan vacíos
        IF @Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = '' OR @Apellido IS NULL OR LTRIM(RTRIM(@Apellido)) = ''
        BEGIN
            THROW 50037, 'El nombre y el apellido del usuario son campos obligatorios.', 1;
        END

        IF @Correo IS NULL OR LTRIM(RTRIM(@Correo)) = ''
        BEGIN
            THROW 50038, 'El correo electrónico es un campo obligatorio.', 1;
        END

        -- 3. Validar existencia del rol si se especifica
        IF @IdRol IS NOT NULL AND @IdRol > 0 AND NOT EXISTS (SELECT 1 FROM Roles WHERE IdRol = @IdRol AND Activo = 1)
        BEGIN
            THROW 50031, 'El rol asignado al usuario no existe en el catálogo de roles.', 1;
        END

        -- 4. Validar que el DNI no esté duplicado en otro usuario activo
        IF @Dni IS NOT NULL AND LTRIM(RTRIM(@Dni)) <> ''
        BEGIN
            IF EXISTS (SELECT 1 FROM Usuarios WHERE Dni = @Dni AND IdUsuario <> @IdUsuario AND Activo = 1)
            BEGIN
                THROW 50032, 'El número de DNI ingresado ya pertenece a otro usuario registrado en el sistema.', 1;
            END

            -- Validar que el DNI no esté registrado para un paciente activo
            IF EXISTS (SELECT 1 FROM Pacientes WHERE Dni = @Dni AND Activo = 1)
            BEGIN
                THROW 50033, 'El número de DNI ingresado ya se encuentra registrado para un paciente en el sistema.', 1;
            END
        END

        -- 5. Validar que el correo no esté duplicado en otro usuario activo
        IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo AND IdUsuario <> @IdUsuario AND Activo = 1)
        BEGIN
            THROW 50034, 'La dirección de correo electrónico ingresada ya se encuentra registrada por otro usuario.', 1;
        END

        -- 6. Actualización en la tabla Usuarios
        UPDATE Usuarios
        SET Nombre = @Nombre,
            Apellido = @Apellido,
            Correo = @Correo,
            Telefono = @Telefono,
            Dni = ISNULL(@Dni, Dni),
            NroMatricula = @NroMatricula,
            IdRol = CASE WHEN @IdRol IS NOT NULL AND @IdRol > 0 THEN @IdRol ELSE IdRol END,
            FechaModificacion = GETDATE()
        WHERE IdUsuario = @IdUsuario;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_EliminarUsuario
** Sección       : 1.7
** Propósito     : Realiza el borrado lógico (`Activo = 0`) del usuario registrando la fecha de baja (`FechaBaja = GETDATE()`). Protege a la cuenta principal `admin@gmail.com` con inmunidad total y **preserva íntegramente los atributos y asignaciones de salas y especialidades** sin eliminarlos ni borrarlos, permitiendo reactivaciones futuras completas.
** Entidad/Tablas: `Usuarios`, `Turnos`, `HistoriasClinicas`
** Invocado por  : `FrmGestionUsuarios2` (Botón `btnEliminar`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdUsuario` (`INT`, IN) - ID del usuario a desactivar.
** ========================================================================= */
-- =========================================================================
-- Procedimiento: sp_EliminarUsuario
-- Descripción: Baja lógica de usuario protegiendo la cuenta admin@gmail.com
-- y preservando intactas las asignaciones de salas y especialidades médicas.
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_EliminarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el usuario exista y esté activo
        IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario AND Activo = 1)
        BEGIN
            THROW 50035, 'El usuario a desactivar no existe o ya se encuentra inactivo.', 1;
        END

        -- 2. Inmunidad para la cuenta administradora principal
        IF EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario AND Correo = 'admin@gmail.com')
        BEGIN
            THROW 50037, 'La cuenta administradora principal (admin@gmail.com) posee inmunidad total y no puede ser desactivada.', 1;
        END

        -- 3. Validar que no tenga turnos activos en atención o llamado en este momento
        IF EXISTS (
            SELECT 1 
            FROM Turnos t
            INNER JOIN HistoriasClinicas hc ON t.IdTurno = hc.IdTurno
            WHERE hc.IdUsuario = @IdUsuario 
              AND t.Estado IN ('Llamado', 'En Consulta') 
              AND t.Activo = 1
        )
        BEGIN
            THROW 50036, 'No se puede desactivar el usuario porque actualmente posee turnos en consulta o llamados activos.', 1;
        END

        BEGIN TRANSACTION;

        -- 4. Baja lógica del usuario exclusivamente (se conservan intactas las asignaciones en DetallesSalas y MedicosEspecialidades)
        UPDATE Usuarios
        SET Activo = 0,
            FechaBaja = GETDATE()
        WHERE IdUsuario = @IdUsuario;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ReactivarUsuario
** Sección       : 1.8
** Propósito     : Restablece el estado activo (`Activo = 1`) de un usuario que se encontraba previamente con baja lógica (`Activo = 0`), eliminando la marca temporal de baja (`FechaBaja = NULL`) y actualizando su fecha de modificación. Asimismo, reactiva y restaura sus asignaciones históricas de salas y especialidades médicas (`DetallesSalas` y `MedicosEspecialidades`).
** Entidad/Tablas: `Usuarios`, `DetallesSalas`, `MedicosEspecialidades`
** Invocado por  : `FrmGestionUsuarios2` (Botón `btnReactivar` ("Re-dar de Alta"))
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdUsuario` (`INT`, IN) - Identificador del usuario a reactivar.
** ========================================================================= */
-- =========================================================================
-- Procedimiento: sp_ReactivarUsuario
-- Descripción: Reactiva lógicamente a un usuario en el sistema (Activo = 1),
-- limpiando la fecha de baja y registrando la fecha de modificación.
-- Restaura además sus relaciones previas de salas y especialidades médicas.
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_ReactivarUsuario
    @IdUsuario INT -- Identificador del usuario inactivo a dar de alta
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el usuario exista en la base de datos
        IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario)
        BEGIN
            THROW 50035, 'El usuario a reactivar no existe en el sistema.', 1;
        END

        -- 2. Validar que el usuario se encuentre inactivo (Activo = 0)
        IF EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario AND Activo = 1)
        BEGIN
            THROW 50039, 'El usuario ya se encuentra activo en el sistema.', 1;
        END

        BEGIN TRANSACTION;

        -- 3. Reactivación lógica: restaurar estado Activo, limpiar fecha de baja y registrar modificación
        UPDATE Usuarios
        SET Activo = 1,
            FechaBaja = NULL,
            FechaModificacion = GETDATE()
        WHERE IdUsuario = @IdUsuario;

        -- 4. Restaurar estado Activo en relaciones médicas asignadas previamente
        UPDATE DetallesSalas
        SET Activo = 1,
            FechaBaja = NULL
        WHERE IdUsuario = @IdUsuario;

        UPDATE MedicosEspecialidades
        SET Activo = 1,
            FechaBaja = NULL
        WHERE IdUsuario = @IdUsuario;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ListarPersonalMedico
** Sección       : 1.9
** Propósito     : Obtiene los usuarios activos con rol de "Personal médico" para la asignación de profesionales a consultorios y salas.
** Entidad/Tablas: `Usuarios`, `Roles`
** Invocado por  : `FrmSalasAdmin` (Evento `Load` / `CargarPersonalMedicoDesdeBD`)
** Estado        : `PENDIENTE DE IMPLEMENTACIÓN`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   : Ninguno.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ListarPersonalMedico
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        u.IdUsuario,
        CONCAT('Dr. ', u.Apellido, ', ', u.Nombre) AS NombreCompleto
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    WHERE u.Activo = 1 
      AND (r.Descripcion LIKE '%Médic%' OR r.Descripcion LIKE '%Medic%')
    ORDER BY u.Apellido, u.Nombre;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerUsuarioPorDni
** Sección       : 1.10
** Propósito     : Busca un usuario específico por su número de DNI con soporte de filtrado opcional de bajas lógicas (@IncluirInactivos), devolviendo el perfil completo con salas y especialidades para FrmGestionUsuarios2.
** Entidad/Tablas: `Usuarios`, `Roles`, `MedicosEspecialidades`, `DetallesSalas`
** Invocado por  : `FrmGestionUsuarios2` (Búsqueda por DNI en paso inicial)
** Estado        : `EN USO`
** Retorno       : 1 fila de `UsuarioListadoDTO` o conjunto vacío si no existe
** Parámetros   :
**                `@Dni` (`NVARCHAR(20)`, IN) - Documento de identidad del usuario.
**                `@IncluirInactivos` (`BIT`, IN) - Opcional (por defecto 1). Si es 1, busca tanto activos como inactivos; si es 0, solo activos.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerUsuarioPorDni
    @Dni NVARCHAR(20),
    @IncluirInactivos BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            u.IdUsuario,
            u.Nombre,
            u.Apellido,
            u.Correo,
            u.Dni,
            u.Telefono,
            r.Descripcion AS Rol,
            ISNULL(u.NroMatricula, '') AS NroMatricula,
            -- Subconsulta correlacionada con DISTINCT para evitar productos cartesianos y duplicaciones de especialidades
            ISNULL((
                SELECT STRING_AGG(subE.Nombre, ', ')
                FROM (
                    SELECT DISTINCT e2.Nombre
                    FROM MedicosEspecialidades me2
                    INNER JOIN Especialidades e2 ON me2.IdEspecialidad = e2.IdEspecialidad
                    WHERE me2.IdUsuario = u.IdUsuario
                      AND (me2.Activo = 1 OR u.Activo = 0)
                      AND e2.Activo = 1
                ) subE
            ), '') AS Especialidades,
            -- Subconsulta correlacionada con DISTINCT para evitar duplicaciones de salas ("Sala I, Sala I")
            ISNULL((
                SELECT STRING_AGG(subS.NombreSala, ', ')
                FROM (
                    SELECT DISTINCT s2.NombreSala
                    FROM DetallesSalas ds2
                    INNER JOIN Salas s2 ON ds2.IdSala = s2.IdSala
                    WHERE ds2.IdUsuario = u.IdUsuario
                      AND (ds2.Activo = 1 OR u.Activo = 0)
                      AND s2.Activo = 1
                ) subS
            ), '') AS Salas,
            u.Activo
        FROM Usuarios u
        INNER JOIN Roles r ON u.IdRol = r.IdRol
        WHERE u.Dni = @Dni
          AND (@IncluirInactivos = 1 OR u.Activo = 1);
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_InsertarSala
** Sección       : 2.1
** Propósito     : Registra un nuevo consultorio o sala física en el catálogo de `Salas`, con validación de nombre único y estado operativo válido.
** Entidad/Tablas: `Salas`
** Invocado por  : `FrmSalasAdmin` (Botón `btnGuardar`)
** Estado        : `EN USO`
** Retorno       : `IdNuevaSala` (`SCOPE_IDENTITY()`).
** Parámetros   :
**                `@NombreSala` (`NVARCHAR(100)`, IN) - Nombre de la sala (ej. 'Consultorio 1').
**                `@EstadoSala` (`NVARCHAR(50)`, IN) - Estado inicial ('Disponible', 'Libre', 'Ocupada', 'En Mantenimiento', 'Cerrada').
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_InsertarSala
    @NombreSala NVARCHAR(100),
    @EstadoSala NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el nombre no esté vacío
        IF @NombreSala IS NULL OR LTRIM(RTRIM(@NombreSala)) = ''
        BEGIN
            THROW 50040, 'El nombre de la sala es obligatorio y no puede quedar vacío.', 1;
        END

        -- 2. Validar que no exista ya una sala activa con el mismo nombre
        IF EXISTS (SELECT 1 FROM Salas WHERE NombreSala = @NombreSala AND Activo = 1)
        BEGIN
            THROW 50041, 'Ya existe una sala activa registrada con este mismo nombre.', 1;
        END

        -- 3. Validar estado operativo permitido
        IF @EstadoSala IS NULL OR @EstadoSala NOT IN ('Disponible', 'Libre', 'Ocupada', 'En Mantenimiento', 'Cerrada')
        BEGIN
            THROW 50042, 'El estado especificado para la sala no es válido. Valores permitidos: Disponible, Libre, Ocupada, En Mantenimiento, Cerrada.', 1;
        END

        -- 4. Inserción
        INSERT INTO Salas (NombreSala, EstadoSala, Activo, FechaCreacion, FechaModificacion)
        VALUES (@NombreSala, @EstadoSala, 1, GETDATE(), GETDATE());

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdNuevaSala;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ModificarSala
** Sección       : 2.2
** Propósito     : Actualiza el nombre identificatorio de una sala de atención médica en la tabla `Salas`.
** Entidad/Tablas: `Salas`
** Invocado por  : `FrmSalasAdmin` (Botón `btnModificar` y edición interactiva en grilla DataGridView)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdSala` (`INT`, IN) - ID de la sala a modificar.
**                `@NombreSala` (`NVARCHAR(100)`, IN) - Nuevo nombre descriptivo de la sala.
**                `@EstadoSala` (`NVARCHAR(50) = NULL`, IN) - Nuevo estado operativo (opcional, conserva actual si es NULL).
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ModificarSala
    @IdSala INT,
    @NombreSala NVARCHAR(100),
    @EstadoSala NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que la sala exista y esté activa
        IF NOT EXISTS (SELECT 1 FROM Salas WHERE IdSala = @IdSala AND Activo = 1)
        BEGIN
            THROW 50043, 'La sala a modificar no existe o se encuentra inactiva.', 1;
        END

        -- 2. Validar que el nombre no esté vacío
        IF @NombreSala IS NULL OR LTRIM(RTRIM(@NombreSala)) = ''
        BEGIN
            THROW 50044, 'El nombre de la sala es un campo obligatorio.', 1;
        END

        -- 3. Validar que no haya otra sala activa con el mismo nombre
        IF EXISTS (SELECT 1 FROM Salas WHERE NombreSala = @NombreSala AND IdSala <> @IdSala AND Activo = 1)
        BEGIN
            THROW 50045, 'Ya existe otra sala activa registrada con este mismo nombre.', 1;
        END

        -- 4. Validar estado operativo si viene provisto
        IF @EstadoSala IS NOT NULL AND LTRIM(RTRIM(@EstadoSala)) <> ''
        BEGIN
            IF @EstadoSala NOT IN ('Disponible', 'Libre', 'Ocupada', 'En Mantenimiento', 'Cerrada')
            BEGIN
                THROW 50042, 'El estado especificado para la sala no es válido. Valores permitidos: Disponible, Libre, Ocupada, En Mantenimiento, Cerrada.', 1;
            END
        END

        -- 5. Actualización atómica en la tabla Salas
        UPDATE Salas
        SET NombreSala = @NombreSala,
            EstadoSala = ISNULL(@EstadoSala, EstadoSala),
            FechaModificacion = GETDATE()
        WHERE IdSala = @IdSala;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_EliminarSala
** Sección       : 2.3
** Propósito     : Realiza la baja lógica de la sala (`Activo = 0`), verificando que no esté ocupada con atención médica activa y dando de baja en cascada las asignaciones de médicos en `DetallesSalas`.
** Entidad/Tablas: `Salas`, `DetallesSalas`
** Invocado por  : `FrmSalasAdmin` (Botón `btnEliminar`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdSala` (`INT`, IN) - ID de la sala a desactivar.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_EliminarSala
    @IdSala INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que la sala exista y esté activa
        IF NOT EXISTS (SELECT 1 FROM Salas WHERE IdSala = @IdSala AND Activo = 1)
        BEGIN
            THROW 50046, 'La sala a eliminar no existe o ya se encuentra inactiva.', 1;
        END

        -- 2. Validar que la sala no esté en uso actualmente
        IF EXISTS (SELECT 1 FROM Salas WHERE IdSala = @IdSala AND EstadoSala = 'Ocupada' AND Activo = 1)
        BEGIN
            THROW 50047, 'No se puede eliminar la sala porque se encuentra en estado Ocupada con una atención médica en curso.', 1;
        END

        BEGIN TRANSACTION;

        -- 3. Baja lógica de la sala
        UPDATE Salas
        SET Activo = 0,
            FechaBaja = GETDATE()
        WHERE IdSala = @IdSala;

        -- 4. Baja lógica en cascada de asignaciones a médicos en DetallesSalas
        UPDATE DetallesSalas
        SET Activo = 0,
            FechaBaja = GETDATE()
        WHERE IdSala = @IdSala AND Activo = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerSalas
** Sección       : 2.4
** Propósito     : Lista las salas de atención médica. Si `@IdUsuario` es `NULL`, devuelve todas las salas (vista administrativa); si se provee un médico, retorna exclusivamente las salas asignadas a su perfil. Admite el parámetro opcional `@IncluirInactivas` para visualizar y reactivar salas dadas de baja lógica en `FrmSalasAdmin`.
** Entidad/Tablas: `Salas`, `DetallesSalas`, `Usuarios`
** Invocado por  : `FrmSalasAdmin`, `FrmGestionUsuarios2`, `MisSalas_PM` (Carga de grilla de salas (`CargarSalasDesdeBD`, `CargarMisSalas`, `chkMostrarInactivas_CheckedChanged`))
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdUsuario` (`INT = NULL`, IN) - ID opcional del médico. Si es NULL, lista todos los consultorios.
**                `@IncluirInactivas` (`BIT = 0`, IN) - Si es 1, incluye salas dadas de baja lógica (`Activo = 0`).
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerSalas
    @IdUsuario INT = NULL,
    @IncluirInactivas BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        s.IdSala,
        s.NombreSala,
        s.EstadoSala,
        ds.IdUsuario,
        u.Nombre AS NombreMedico,
        u.Apellido AS ApellidoMedico,
        ISNULL(ds.DescripcionAtencion, '') AS DescripcionAtencion,
        s.Activo
    FROM Salas s
    LEFT JOIN DetallesSalas ds ON s.IdSala = ds.IdSala AND (ds.Activo = 1 OR s.Activo = 0)
    LEFT JOIN Usuarios u ON ds.IdUsuario = u.IdUsuario AND u.Activo = 1
    WHERE (@IncluirInactivas = 1 OR s.Activo = 1)
      AND (@IdUsuario IS NULL OR ds.IdUsuario = @IdUsuario)
    ORDER BY s.Activo DESC, s.NombreSala ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_AsignarSalaMedico
** Sección       : 2.5
** Propósito     : Vincula un médico a un consultorio en la tabla `DetallesSalas`, validando existencia y rol médico, y previniendo duplicados activos.
** Entidad/Tablas: `DetallesSalas`, `Salas`, `Usuarios`
** Invocado por  : `FrmGestionUsuarios2`, `FrmSalasAdmin` (Botón `btnGuardar` / `btnModificar`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdSala` (`INT`, IN) - ID de la sala asignada.
**                `@IdUsuario` (`INT`, IN) - ID del médico asignado.
**                `@DescripcionAtencion` (`NVARCHAR(255) = NULL`, IN) - Notas u horario de atención.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_AsignarSalaMedico
    @IdSala INT,
    @IdUsuario INT,
    @DescripcionAtencion NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que la sala exista y esté activa
        IF NOT EXISTS (SELECT 1 FROM Salas WHERE IdSala = @IdSala AND Activo = 1)
        BEGIN
            THROW 50060, 'La sala especificada para la asignación no existe o se encuentra inactiva.', 1;
        END

        -- 2. Validar que el usuario exista, esté activo y pertenezca al rol médico
        IF NOT EXISTS (
            SELECT 1 
            FROM Usuarios u
            INNER JOIN Roles r ON u.IdRol = r.IdRol
            WHERE u.IdUsuario = @IdUsuario 
              AND u.Activo = 1 
              AND (u.IdRol = 1 OR r.Descripcion LIKE '%Médic%' OR r.Descripcion LIKE '%Medic%')
        )
        BEGIN
            THROW 50061, 'El usuario asignado no existe, está inactivo o no pertenece al rol de Personal Médico.', 1;
        END

        -- 3. Evitar duplicidad: si ya está activa la asignación, actualizar la descripción
        IF EXISTS (SELECT 1 FROM DetallesSalas WHERE IdSala = @IdSala AND IdUsuario = @IdUsuario AND Activo = 1)
        BEGIN
            UPDATE DetallesSalas
            SET DescripcionAtencion = ISNULL(@DescripcionAtencion, DescripcionAtencion),
                FechaModificacion = GETDATE()
            WHERE IdSala = @IdSala AND IdUsuario = @IdUsuario AND Activo = 1;
        END
        ELSE IF EXISTS (SELECT 1 FROM DetallesSalas WHERE IdSala = @IdSala AND IdUsuario = @IdUsuario AND Activo = 0)
        BEGIN
            -- Reactivación de asignación previamente dada de baja
            UPDATE TOP(1) DetallesSalas
            SET DescripcionAtencion = ISNULL(@DescripcionAtencion, DescripcionAtencion),
                Activo = 1,
                FechaBaja = NULL,
                FechaModificacion = GETDATE()
            WHERE IdSala = @IdSala AND IdUsuario = @IdUsuario AND Activo = 0;
        END
        ELSE
        BEGIN
            INSERT INTO DetallesSalas (IdSala, IdUsuario, DescripcionAtencion, FechaCreacion, Activo)
            VALUES (@IdSala, @IdUsuario, ISNULL(@DescripcionAtencion, ''), GETDATE(), 1);
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_AbrirSala
** Sección       : 2.6
** Propósito     : Habilita el consultorio para atención (`EstadoSala = 'Libre'`). Valida que el médico no tenga otra sala abierta y que esté asignado a la sala.
** Entidad/Tablas: `Salas`, `DetallesSalas`
** Invocado por  : `MisSalas_PM` (Botón `btnAbrirSala`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdSala` (`INT`, IN) - ID de la sala a abrir.
**                `@IdUsuario` (`INT`, IN) - ID del médico autenticado.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_AbrirSala
    @IdSala INT,
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    IF EXISTS (
        SELECT 1 
        FROM DetallesSalas ds
        INNER JOIN Salas s ON ds.IdSala = s.IdSala
        WHERE ds.IdUsuario = @IdUsuario 
          AND s.EstadoSala IN ('Libre', 'Disponible', 'Ocupada') 
          AND s.Activo = 1
          AND ds.Activo = 1
          AND s.IdSala <> @IdSala
    )
    BEGIN
        THROW 50002, 'Acción denegada: Ya tienes otra sala abierta. Debes cerrarla antes de abrir una nueva.', 1;
    END

    IF NOT EXISTS (
        SELECT 1 
        FROM DetallesSalas 
        WHERE IdSala = @IdSala AND IdUsuario = @IdUsuario AND Activo = 1
    )
    BEGIN
        THROW 50003, 'Acción denegada: No tienes permisos para abrir esta sala porque no te ha sido asignada.', 1;
    END

    IF EXISTS (
        SELECT 1 
        FROM Salas 
        WHERE IdSala = @IdSala AND EstadoSala IN ('Libre', 'Disponible', 'Ocupada') AND Activo = 1
    )
    BEGIN
        THROW 50004, 'La sala seleccionada ya se encuentra abierta.', 1;
    END

    UPDATE Salas
    SET EstadoSala = 'Disponible',
        FechaModificacion = GETDATE()
    WHERE IdSala = @IdSala AND Activo = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_CerrarSala
** Sección       : 2.7
** Propósito     : Cambia el estado de la sala a `'Cerrada'` / `'En Mantenimiento'`. Bloquea si la sala está en consulta con un paciente (`'Ocupada'`).
** Entidad/Tablas: `Salas`
** Invocado por  : `MisSalas_PM` (Botón `btnCerrarSala`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdSala` (`INT`, IN) - ID de la sala a cerrar.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_CerrarSala
    @IdSala INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    IF EXISTS (
        SELECT 1 
        FROM Salas 
        WHERE IdSala = @IdSala AND EstadoSala = 'Ocupada' AND Activo = 1
    )
    BEGIN
        THROW 50001, 'No se puede cerrar la sala porque hay un paciente siendo atendido actualmente.', 1;
    END

    UPDATE Salas
    SET EstadoSala = 'En Mantenimiento',
        FechaModificacion = GETDATE()
    WHERE IdSala = @IdSala AND Activo = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ActualizarEstadoSala
** Sección       : 2.8
** Propósito     : Actualiza de forma genérica el estado operativo de un consultorio o sala física (`Disponible`, `Libre`, `Ocupada`, `En Mantenimiento`, `Cerrada`) con control de transacciones y validaciones de integridad.
** Entidad/Tablas: `Salas`
** Invocado por  : Utilizado por la capa DAL/BLL (`SalaDAL.ActualizarEstadoSala`, `SalaBLL.ActualizarEstadoSala`) (Mantenimiento de salas y soporte operativo)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdSala` (`INT`, IN) - ID de la sala a modificar.
**                `@NuevoEstado` (`NVARCHAR(50)`, IN) - Nuevo estado operativo a asignar.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ActualizarEstadoSala
    @IdSala INT,
    @NuevoEstado NVARCHAR(50) 
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Salas WHERE IdSala = @IdSala AND Activo = 1)
        BEGIN
            THROW 50048, 'La sala a actualizar no existe o se encuentra inactiva.', 1;
        END

        IF @NuevoEstado NOT IN ('Disponible', 'Libre', 'Ocupada', 'En Mantenimiento', 'Cerrada')
        BEGIN
            THROW 50049, 'El estado operativo asignado a la sala no es válido.', 1;
        END

        UPDATE Salas
        SET EstadoSala = @NuevoEstado,
            FechaModificacion = GETDATE()
        WHERE IdSala = @IdSala AND Activo = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ReactivarSala
** Sección       : 2.9
** Propósito     : Reactiva lógicamente una sala dada de baja previamente (`Activo = 1`, `FechaBaja = NULL`), restaura sus asignaciones históricas en `DetallesSalas` y valida existencia y estado actual.
** Entidad/Tablas: `Salas`, `DetallesSalas`
** Invocado por  : `FrmSalasAdmin` (Botón `btnReactivar` ("♻ Re-dar de Alta Sala"))
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdSala` (`INT`, IN) - ID de la sala a reactivar.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ReactivarSala
    @IdSala INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar existencia
        IF NOT EXISTS (SELECT 1 FROM Salas WHERE IdSala = @IdSala)
        BEGIN
            THROW 50043, 'La sala especificada no existe en el sistema.', 1;
        END

        -- 2. Validar que no esté activa ya
        IF EXISTS (SELECT 1 FROM Salas WHERE IdSala = @IdSala AND Activo = 1)
        BEGIN
            THROW 50044, 'La sala ya se encuentra activa en el sistema.', 1;
        END

        BEGIN TRANSACTION;

        -- 3. Reactivar sala
        UPDATE Salas
        SET Activo = 1,
            FechaBaja = NULL,
            FechaModificacion = GETDATE(),
            EstadoSala = CASE WHEN EstadoSala = 'En Mantenimiento' THEN 'Disponible' ELSE EstadoSala END
        WHERE IdSala = @IdSala;

        -- 4. Reactivar asignaciones en DetallesSalas
        UPDATE DetallesSalas
        SET Activo = 1,
            FechaBaja = NULL
        WHERE IdSala = @IdSala;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ListarEspecialidades
** Sección       : 3.1
** Propósito     : Obtiene la lista completa de especialidades médicas. Admite el parámetro opcional `@IncluirInactivas` para visualizar y reactivar especialidades dadas de baja lógica en `FrmGestionEspecialidades`.
** Entidad/Tablas: `Especialidades`
** Invocado por  : `FrmGestionEspecialidades`, `FrmGestionUsuarios2`, `FrmTurnoEspecialidad`, `FrmListaTurnos`, `FrmListaTurnosAtencion` (Carga de catálogos, selectores y grilla administrativa (`chkMostrarInactivas_CheckedChanged`))
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IncluirInactivas` (`BIT = 0`, IN) - Si es 1, incluye especialidades dadas de baja lógica (`Activo = 0`).
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ListarEspecialidades
    @IncluirInactivas BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        IdEspecialidad, 
        Nombre,
        Activo
    FROM Especialidades
    WHERE (@IncluirInactivas = 1 OR Activo = 1)
    ORDER BY Activo DESC, Nombre ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_InsertarEspecialidad
** Sección       : 3.2
** Propósito     : Registra una nueva especialidad médica en el catálogo con validación de nombre no vacío y no duplicado. Si existía previamente dada de baja, la reactiva automáticamente.
** Entidad/Tablas: `Especialidades`
** Invocado por  : `FrmGestionEspecialidades` (Botón `btnGuardar`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@Nombre` (`NVARCHAR(100)`, IN) - Nombre de la especialidad.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_InsertarEspecialidad
    @Nombre NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el nombre no esté vacío
        IF @Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = ''
        BEGIN
            THROW 50050, 'El nombre de la especialidad es obligatorio y no puede quedar vacío.', 1;
        END

        -- 2. Validar que no exista ya activa
        IF EXISTS (SELECT 1 FROM Especialidades WHERE Nombre = @Nombre AND Activo = 1)
        BEGIN
            THROW 50051, 'Ya existe una especialidad activa registrada con este mismo nombre.', 1;
        END

        -- 3. Si existía previamente inactiva, la reactivamos
        IF EXISTS (SELECT 1 FROM Especialidades WHERE Nombre = @Nombre AND Activo = 0)
        BEGIN
            UPDATE Especialidades
            SET Activo = 1,
                FechaBaja = NULL
            WHERE Nombre = @Nombre;
        END
        ELSE
        BEGIN
            INSERT INTO Especialidades (Nombre, Activo, FechaCreacion)
            VALUES (@Nombre, 1, GETDATE());
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ModificarEspecialidad
** Sección       : 3.3
** Propósito     : Modifica la denominación de una especialidad médica existente con validación de no duplicados y nombre no vacío.
** Entidad/Tablas: `Especialidades`
** Invocado por  : `FrmGestionEspecialidades` (Botón `btnModificar` ("💾 Modificar Especialidad"))
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdEspecialidad` (`INT`, IN) - ID de la especialidad a modificar.
**                `@Nombre` (`NVARCHAR(100)`, IN) - Nuevo nombre asignado.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ModificarEspecialidad
    @IdEspecialidad INT,
    @Nombre NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar existencia
        IF NOT EXISTS (SELECT 1 FROM Especialidades WHERE IdEspecialidad = @IdEspecialidad)
        BEGIN
            THROW 50054, 'La especialidad a modificar no existe en el sistema.', 1;
        END

        -- 2. Validar que el nombre no esté en blanco
        IF @Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = ''
        BEGIN
            THROW 50050, 'El nombre de la especialidad es obligatorio y no puede quedar vacío.', 1;
        END

        -- 3. Validar unicidad entre especialidades activas
        IF EXISTS (SELECT 1 FROM Especialidades WHERE Nombre = @Nombre AND IdEspecialidad <> @IdEspecialidad AND Activo = 1)
        BEGIN
            THROW 50051, 'Ya existe otra especialidad activa registrada con este mismo nombre.', 1;
        END

        -- 4. Actualización
        UPDATE Especialidades
        SET Nombre = @Nombre,
            FechaModificacion = GETDATE()
        WHERE IdEspecialidad = @IdEspecialidad;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_EliminarEspecialidad
** Sección       : 3.4
** Propósito     : Realiza el borrado lógico de una especialidad médica (`Activo = 0`) previa validación de que no posea turnos pendientes o en atención en curso, y desactiva en cascada sus asignaciones médicas en `MedicosEspecialidades`.
** Entidad/Tablas: `Especialidades`, `MedicosEspecialidades`, `Turnos`
** Invocado por  : `FrmGestionEspecialidades` (Botón `btnDesactivar`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdEspecialidad` (`INT`, IN) - ID de la especialidad a desactivar.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_EliminarEspecialidad
    @IdEspecialidad INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que la especialidad exista y esté activa
        IF NOT EXISTS (SELECT 1 FROM Especialidades WHERE IdEspecialidad = @IdEspecialidad AND Activo = 1)
        BEGIN
            THROW 50052, 'La especialidad a desactivar no existe o ya se encuentra inactiva.', 1;
        END

        -- 2. Validar que no posea turnos pendientes de atención
        IF EXISTS (
            SELECT 1 
            FROM Turnos 
            WHERE IdEspecialidad = @IdEspecialidad 
              AND Estado IN ('En Espera', 'Llamado', 'En Consulta') 
              AND Activo = 1
        )
        BEGIN
            THROW 50053, 'No se puede dar de baja la especialidad porque posee turnos pendientes de atención (En Espera, Llamado o En Consulta).', 1;
        END

        BEGIN TRANSACTION;

        -- 3. Baja lógica de la especialidad
        UPDATE Especialidades
        SET Activo = 0,
            FechaBaja = GETDATE()
        WHERE IdEspecialidad = @IdEspecialidad;

        -- 4. Baja lógica en cascada en MedicosEspecialidades
        UPDATE MedicosEspecialidades
        SET Activo = 0,
            FechaBaja = GETDATE()
        WHERE IdEspecialidad = @IdEspecialidad AND Activo = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_AsignarEspecialidadMedico
** Sección       : 3.5
** Propósito     : Asocia una especialidad a un profesional médico en la tabla intermedia `MedicosEspecialidades`, validando la existencia de ambos y previniendo duplicados activos.
** Entidad/Tablas: `MedicosEspecialidades`, `Usuarios`, `Especialidades`
** Invocado por  : `FrmGestionUsuarios2` (Botón `btnGuardar` / `btnModificar` en `FrmGestionUsuarios2`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdUsuario` (`INT`, IN) - ID del médico.
**                `@IdEspecialidad` (`INT`, IN) - ID de la especialidad vinculada.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_AsignarEspecialidadMedico
    @IdUsuario INT,
    @IdEspecialidad INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el usuario exista, esté activo y pertenezca al rol médico
        IF NOT EXISTS (
            SELECT 1 
            FROM Usuarios u
            INNER JOIN Roles r ON u.IdRol = r.IdRol
            WHERE u.IdUsuario = @IdUsuario 
              AND u.Activo = 1 
              AND (u.IdRol = 1 OR r.Descripcion LIKE '%Médic%' OR r.Descripcion LIKE '%Medic%')
        )
        BEGIN
            THROW 50062, 'El profesional médico a asignar no existe, está inactivo o no pertenece al rol de Personal Médico.', 1;
        END

        -- 2. Validar que la especialidad exista y esté activa
        IF NOT EXISTS (SELECT 1 FROM Especialidades WHERE IdEspecialidad = @IdEspecialidad AND Activo = 1)
        BEGIN
            THROW 50063, 'La especialidad médica a asignar no existe o se encuentra inactiva.', 1;
        END

        -- 3. Evitar duplicidad: si ya está activa, no hacer nada; si está inactiva, reactivarla
        IF NOT EXISTS (SELECT 1 FROM MedicosEspecialidades WHERE IdUsuario = @IdUsuario AND IdEspecialidad = @IdEspecialidad AND Activo = 1)
        BEGIN
            IF EXISTS (SELECT 1 FROM MedicosEspecialidades WHERE IdUsuario = @IdUsuario AND IdEspecialidad = @IdEspecialidad AND Activo = 0)
            BEGIN
                UPDATE TOP(1) MedicosEspecialidades
                SET Activo = 1,
                    FechaBaja = NULL,
                    FechaModificacion = GETDATE()
                WHERE IdUsuario = @IdUsuario AND IdEspecialidad = @IdEspecialidad AND Activo = 0;
            END
            ELSE
            BEGIN
                INSERT INTO MedicosEspecialidades (IdUsuario, IdEspecialidad, Activo, FechaCreacion)
                VALUES (@IdUsuario, @IdEspecialidad, 1, GETDATE());
            END
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerEspecialidadesPorMedico
** Sección       : 3.6
** Propósito     : Obtiene las especialidades activas asignadas a un médico en particular a través de la tabla intermedia `MedicosEspecialidades`.
** Entidad/Tablas: `Especialidades`, `MedicosEspecialidades`
** Invocado por  : `FrmGestionUsuarios2`, `FrmListaTurnosAtencion` (Carga de especialidades vinculadas al profesional (`CargarServiciosDelMedico` y precarga en `FrmGestionUsuarios2`))
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdUsuario` (`INT`, IN) - ID del médico consultado.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerEspecialidadesPorMedico
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        e.IdEspecialidad,
        e.Nombre,
        e.Activo
    FROM Especialidades e
    INNER JOIN MedicosEspecialidades me ON e.IdEspecialidad = me.IdEspecialidad
    WHERE me.IdUsuario = @IdUsuario 
      AND me.Activo = 1 
      AND e.Activo = 1
    ORDER BY e.Nombre ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ReactivarEspecialidad
** Sección       : 3.7
** Propósito     : Reactiva lógicamente una especialidad médica previamente desactivada (`Activo = 1`, `FechaBaja = NULL`), restaura sus vínculos históricos con profesionales médicos en `MedicosEspecialidades` y valida su existencia.
** Entidad/Tablas: `Especialidades`, `MedicosEspecialidades`
** Invocado por  : `FrmGestionEspecialidades` (Botón `btnReactivar` ("♻ Re-dar de Alta"))
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdEspecialidad` (`INT`, IN) - ID de la especialidad a reactivar.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ReactivarEspecialidad
    @IdEspecialidad INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar existencia
        IF NOT EXISTS (SELECT 1 FROM Especialidades WHERE IdEspecialidad = @IdEspecialidad)
        BEGIN
            THROW 50055, 'La especialidad a reactivar no existe en el sistema.', 1;
        END

        -- 2. Validar que no se encuentre activa ya
        IF EXISTS (SELECT 1 FROM Especialidades WHERE IdEspecialidad = @IdEspecialidad AND Activo = 1)
        BEGIN
            THROW 50056, 'La especialidad ya se encuentra activa en el sistema.', 1;
        END

        BEGIN TRANSACTION;

        -- 3. Reactivación de la especialidad
        UPDATE Especialidades
        SET Activo = 1,
            FechaBaja = NULL,
            FechaModificacion = GETDATE()
        WHERE IdEspecialidad = @IdEspecialidad;

        -- 4. Reactivación en cascada de vínculos médicos en MedicosEspecialidades
        UPDATE MedicosEspecialidades
        SET Activo = 1,
            FechaBaja = NULL
        WHERE IdEspecialidad = @IdEspecialidad;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_BuscarPacientePorDNI
** Sección       : 4.1
** Propósito     : Busca y recupera la información de un paciente activo a partir de su número de documento de identidad (DNI).
** Entidad/Tablas: `Pacientes`
** Invocado por  : `FrmTurnoEmergencia`, `FrmTurnoEspecialidad` (Autocompletado y verificación de antecedentes al ingresar DNI)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@DNI` (`NVARCHAR(20)`, IN) - Documento de identidad del paciente.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_BuscarPacientePorDNI
    @DNI NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT IdPaciente, Nombre, Apellido, Dni, ObraSocial 
    FROM Pacientes 
    WHERE DNI = @DNI AND Activo = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_GuardarPaciente
** Sección       : 4.2
** Propósito     : Busca al paciente por DNI. Valida que dicho DNI no pertenezca a un usuario del sistema (código `50010`). Si el paciente ya existe en el sistema, actualiza su nombre, apellido y cobertura médica y retorna su `IdPaciente`. Si no existe, lo inserta en `Pacientes` y retorna el nuevo ID autoincremental generado.
** Entidad/Tablas: `Pacientes`, `Usuarios`
** Invocado por  : `FrmTurnoEmergencia`, `FrmTurnoEspecialidad` (Botón `BtnGenerarTurno`)
** Estado        : `EN USO`
** Retorno       : 1 fila: `IdPaciente`.
** Parámetros   :
**                `@Nombre` (`NVARCHAR(100)`, IN) - Nombre del paciente.
**                `@Apellido` (`NVARCHAR(100)`, IN) - Apellido del paciente.
**                `@Dni` (`NVARCHAR(20)`, IN) - Documento de identidad.
**                `@ObraSocial` (`NVARCHAR(100)`, IN) - Cobertura médica u obra social.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_GuardarPaciente
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni NVARCHAR(20),
    @ObraSocial NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar si el DNI ya existe como Usuario activo en el sistema
        IF EXISTS (SELECT 1 FROM Usuarios WHERE Dni = @Dni AND Activo = 1)
        BEGIN
            THROW 50010, 'Ya existe un usuario registrado en el sistema con este mismo número de DNI. No se puede duplicar.', 1;
        END

        DECLARE @IdPaciente INT;

        SELECT @IdPaciente = IdPaciente 
        FROM Pacientes 
        WHERE Dni = @Dni AND Activo = 1;

        IF @IdPaciente IS NOT NULL
        BEGIN
            UPDATE Pacientes
            SET Nombre = @Nombre,
                Apellido = @Apellido,
                ObraSocial = @ObraSocial,
                FechaModificacion = GETDATE()
            WHERE IdPaciente = @IdPaciente;
        END
        ELSE
        BEGIN
            INSERT INTO Pacientes (Nombre, Apellido, Dni, ObraSocial, Activo, FechaCreacion)
            VALUES (@Nombre, @Apellido, @Dni, @ObraSocial, 1, GETDATE());

            SET @IdPaciente = SCOPE_IDENTITY();
        END

        SELECT @IdPaciente AS IdPaciente;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_InsertarPaciente
** Sección       : 4.3
** Propósito     : Inserción simple y directa de un paciente sin validación de duplicidad por DNI ni verificación cruzada con la tabla de usuarios. Fue superado operativamente por `sp_GuardarPaciente`.
** Entidad/Tablas: `Pacientes`
** Invocado por  : Ninguno actualmente (reemplazado por `sp_GuardarPaciente`). (N/A)
** Estado        : `NO UTILIZADO (SUPERADO POR sp_GuardarPaciente)`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@Nombre` (`NVARCHAR(100)`, IN) - Nombre del paciente.
**                `@Apellido` (`NVARCHAR(100)`, IN) - Apellido del paciente.
**                `@Dni` (`NVARCHAR(20)`, IN) - Documento de identidad.
**                `@ObraSocial` (`NVARCHAR(100)`, IN) - Cobertura médica.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_InsertarPaciente
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni NVARCHAR(20),
    @ObraSocial NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    INSERT INTO Pacientes (Nombre, Apellido, Dni, ObraSocial, FechaCreacion, Activo)
    VALUES (@Nombre, @Apellido, @Dni, @ObraSocial, GETDATE(), 1);

    SELECT SCOPE_IDENTITY() AS IdNuevoPaciente;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerSintomas
** Sección       : 5.1
** Propósito     : Retorna el catálogo de síntomas activos con su nivel de gravedad para el triage dinámico de guardia.
** Entidad/Tablas: `Sintomas`
** Invocado por  : `FrmTurnoEmergencia` (Evento `Load` / `CargarCatalogoSintomas`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   : Ninguno.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerSintomas
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT IdSintoma, Descripcion, Gravedad 
    FROM Sintomas 
    WHERE Activo = 1
    ORDER BY Gravedad ASC, Descripcion ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerGravedadSintoma
** Sección       : 5.2
** Propósito     : Obtiene la gravedad nativa de un síntoma específico para el cálculo de nivel de triage de emergencias.
** Entidad/Tablas: `Sintomas`
** Invocado por  : `FrmTurnoEmergencia` (Selección de síntoma en el selector de triage)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdSintoma` (`INT`, IN) - ID del síntoma seleccionado.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerGravedadSintoma
    @IdSintoma INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT Gravedad 
    FROM Sintomas 
    WHERE IdSintoma = @IdSintoma;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_GuardarTurnoSintoma
** Sección       : 5.3
** Propósito     : Vincula un síntoma al turno de atención médica en la tabla `TurnoSintomas`, validando que ambos existan y se encuentren activos, y previniendo la duplicación del mismo síntoma en el turno.
** Entidad/Tablas: `TurnoSintomas`, `Turnos`, `Sintomas`
** Invocado por  : `FrmTurnoEmergencia` (Botón `BtnGenerarTurno`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdTurno` (`INT`, IN) - ID del turno generado.
**                `@IdSintoma` (`INT`, IN) - ID del síntoma asociado.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_GuardarTurnoSintoma
    @IdTurno INT,
    @IdSintoma INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el turno exista y esté activo
        IF NOT EXISTS (SELECT 1 FROM Turnos WHERE IdTurno = @IdTurno AND Activo = 1)
        BEGIN
            THROW 50085, 'El turno especificado para vincular el síntoma no existe o se encuentra inactivo.', 1;
        END

        -- 2. Validar que el síntoma exista y esté activo
        IF NOT EXISTS (SELECT 1 FROM Sintomas WHERE IdSintoma = @IdSintoma AND Activo = 1)
        BEGIN
            THROW 50086, 'El síntoma especificado no existe o se encuentra inactivo.', 1;
        END

        -- 3. Evitar duplicar el mismo síntoma en el turno
        IF NOT EXISTS (SELECT 1 FROM TurnoSintomas WHERE IdTurno = @IdTurno AND IdSintoma = @IdSintoma AND Activo = 1)
        BEGIN
            INSERT INTO TurnoSintomas (IdTurno, IdSintoma, EstadoActual, Activo, FechaCreacion) 
            VALUES (@IdTurno, @IdSintoma, 'en espera', 1, GETDATE());
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_CrearTurnoEmergencia
** Sección       : 5.4
** Propósito     : Genera un nuevo turno de guardia de urgencias/emergencias, asociándolo al servicio de Emergencia, asignando la prioridad de triage correspondiente y generando el ticket correlativo `E-001`. Cuenta con validaciones de negocio previas.
** Entidad/Tablas: `Turnos`, `Pacientes`, `Especialidades`
** Invocado por  : `FrmTurnoEmergencia` (Botón `BtnGenerarTurno`)
** Estado        : `EN USO`
** Retorno       : `IdNuevoTurno`, `NroOrden`.
** Parámetros   :
**                `@IdPaciente` (`INT`, IN) - ID del paciente asistido.
**                `@IdPrioridad` (`INT`, IN) - Nivel de prioridad calculado (1=Alta, 2=Media, 3=Baja).
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_CrearTurnoEmergencia
    @IdPaciente INT,
    @IdPrioridad INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validación de negocio previa en SQL por si acaso
        IF NOT EXISTS (SELECT 1 FROM Pacientes WHERE IdPaciente = @IdPaciente AND Activo = 1)
        BEGIN
            THROW 50001, 'El paciente seleccionado no existe o se encuentra inactivo en el sistema.', 1;
        END

        DECLARE @IdTurno INT;
        DECLARE @NroOrden NVARCHAR(20);
        DECLARE @IdEspecialidadEmergencia INT;

        SELECT @IdEspecialidadEmergencia = IdEspecialidad 
        FROM Especialidades 
        WHERE Nombre = 'Emergencia' AND Activo = 1;

        IF @IdEspecialidadEmergencia IS NULL
        BEGIN
            THROW 50002, 'No se encontró configurada una especialidad activa para "Emergencia" en la base de datos.', 1;
        END

        INSERT INTO Turnos (NroOrden, Estado, Fecha, Horario, TipoTurno, IdPrioridad, IdPaciente, IdEspecialidad, Activo, FechaCreacion)
        VALUES ('TEMP', 'En Espera', CAST(GETDATE() AS DATE), CAST(GETDATE() AS TIME), 'Emergencia', @IdPrioridad, @IdPaciente, @IdEspecialidadEmergencia, 1, GETDATE());

        SET @IdTurno = SCOPE_IDENTITY();
        
        SET @NroOrden = CONCAT('E-', RIGHT('000' + CAST(@IdTurno AS VARCHAR(10)), 3));
        UPDATE Turnos SET NroOrden = @NroOrden WHERE IdTurno = @IdTurno;

        -- Devolvemos ambas columnas requeridas por el DTO
        SELECT CAST(@IdTurno AS INT) AS IdNuevoTurno, @NroOrden AS NroOrden;

    END TRY
    BEGIN CATCH
        -- Capturamos el error de SQL y lo propagamos limpiamente al código C#
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerHorariosDisponibles
** Sección       : 5.5
** Propósito     : Retorna las franjas horarias libres (no reservadas) para una fecha y especialidad específicas.
** Entidad/Tablas: `Turnos`, `Especialidades`
** Invocado por  : `FrmTurnoEspecialidad` (Selección de fecha en calendario (`calFechaTurno_DateChanged`))
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@NombreEspecialidad` (`NVARCHAR(100)`, IN) - Nombre de la especialidad.
**                `@Fecha` (`DATE`, IN) - Fecha consultada.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerHorariosDisponibles
    @NombreEspecialidad NVARCHAR(100),
    @Fecha DATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    -- Tabla de horarios estándar
    DECLARE @Horarios TABLE (Horario VARCHAR(10));
    INSERT INTO @Horarios VALUES ('08:30'), ('09:00'), ('09:30'), ('10:00'), ('10:30'), ('11:00'), ('14:00'), ('14:30'), ('15:00'), ('16:00');

    SELECT h.Horario
    FROM @Horarios h
    WHERE h.Horario NOT IN (
        SELECT CONVERT(VARCHAR(5), t.Horario, 108)
        FROM Turnos t
        INNER JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
        WHERE e.Nombre = @NombreEspecialidad
          AND CAST(t.Fecha AS DATE) = @Fecha
          AND t.Activo = 1
    )
    ORDER BY h.Horario ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_CrearTurnoEspecialidad
** Sección       : 5.6
** Propósito     : Registra un turno programado de consultorio externo vinculando paciente, especialidad, fecha y horario asignado. Genera el código correlativo prefijado con la inicial de la especialidad (ej. `C-001` para Cardiología, `P-001` para Pediatría) y devuelve el ID recién creado.
** Entidad/Tablas: `Turnos`, `Especialidades`, `Pacientes`
** Invocado por  : `FrmTurnoEspecialidad` (Botón `BtnGenerarTurno`)
** Estado        : `EN USO`
** Retorno       : `IdNuevoTurno`, `NroOrden`.
** Parámetros   :
**                `@IdPaciente` (`INT`, IN) - ID del paciente.
**                `@NombreEspecialidad` (`NVARCHAR(100)`, IN) - Especialidad elegida.
**                `@Fecha` (`DATE`, IN) - Fecha del turno.
**                `@Horario` (`NVARCHAR(10)`, IN) - Horario asignado (ej. '10:30').
**                `@Estado` (`NVARCHAR(50)`, IN) - Estado inicial ('En Espera').
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_CrearTurnoEspecialidad
    @IdPaciente INT,
    @NombreEspecialidad NVARCHAR(100),
    @Fecha DATE,
    @Horario NVARCHAR(10),
    @Estado NVARCHAR(50) = 'En Espera'
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validamos que la especialidad exista y esté activa
        DECLARE @IdEspecialidad INT;
        SELECT @IdEspecialidad = IdEspecialidad 
        FROM Especialidades 
        WHERE Nombre = @NombreEspecialidad AND Activo = 1;

        IF @IdEspecialidad IS NULL
        BEGIN
            THROW 50003, 'La especialidad seleccionada no existe o se encuentra inactiva.', 1;
        END

        -- Validamos que el paciente exista
        IF NOT EXISTS (SELECT 1 FROM Pacientes WHERE IdPaciente = @IdPaciente AND Activo = 1)
        BEGIN
            THROW 50001, 'El paciente especificado no se encuentra registrado o está inactivo.', 1;
        END

        -- Validamos que no exista ya un turno activo para la misma fecha, horario y especialidad
        IF EXISTS (
            SELECT 1 
            FROM Turnos t
            WHERE t.IdEspecialidad = @IdEspecialidad
              AND CAST(t.Fecha AS DATE) = @Fecha
              AND CAST(t.Horario AS TIME) = CAST(@Horario AS TIME)
              AND t.Activo = 1
        )
        BEGIN
            THROW 50004, 'Ya existe un turno reservado para la misma fecha, horario y especialidad médica.', 1;
        END

        DECLARE @IdTurno INT;
        DECLARE @NroOrden NVARCHAR(20);
        DECLARE @InicialEspecialidad CHAR(1);

        -- Obtenemos la primera letra de la especialidad en mayúscula (ej: C de Cardiología)
        SET @InicialEspecialidad = UPPER(LEFT(@NombreEspecialidad, 1));

        -- Insertamos con un valor temporal
        INSERT INTO Turnos (NroOrden, Estado, Fecha, Horario, TipoTurno, IdPrioridad, IdPaciente, IdEspecialidad, Activo, FechaCreacion)
        VALUES ('TEMP', @Estado, @Fecha, CAST(@Horario AS TIME), 'Consulta', 3, @IdPaciente, @IdEspecialidad, 1, GETDATE());

        SET @IdTurno = SCOPE_IDENTITY();
        
        -- Formateamos el número de orden usando la inicial de la especialidad (Ej: C-001, P-005)
        SET @NroOrden = CONCAT(@InicialEspecialidad, '-', RIGHT('000' + CAST(@IdTurno AS VARCHAR(10)), 3));
        
        -- Actualizamos el turno con el NroOrden definitivo
        UPDATE Turnos SET NroOrden = @NroOrden WHERE IdTurno = @IdTurno;

        -- Retornamos los datos requeridos por el DTO
        SELECT CAST(@IdTurno AS INT) AS IdNuevoTurno, @NroOrden AS NroOrden;

    END TRY
    BEGIN CATCH
        -- Capturamos el error y lo propagamos limpiamente a C#
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ExisteTurnoEspecialidad
** Sección       : 5.6.1
** Propósito     : Comprueba de forma booleana (1 o 0) si ya se encuentra reservado un turno activo para la misma fecha, horario y especialidad médica. Si alguno de estos atributos difiere, retorna 0 (disponible).
** Entidad/Tablas: `Turnos`, `Especialidades`
** Invocado por  : `FrmTurnoEspecialidad` (Capa DAL `TurnoDAL.ExisteTurnoEspecialidad` y BLL `TurnoBLL.ExisteTurnoEspecialidad`)
** Estado        : `EN USO`
** Retorno       : `Existe` (`INT`: 1 si existe turno ocupado, 0 si está disponible)
** Parámetros   :
**                `@NombreEspecialidad` (`NVARCHAR(100)`, IN) - Nombre de la especialidad consultada.
**                `@Fecha` (`DATE`, IN) - Fecha asignada a evaluar.
**                `@Horario` (`NVARCHAR(10)`, IN) - Franja horaria solicitada (ej. '10:00').
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ExisteTurnoEspecialidad
    @NombreEspecialidad NVARCHAR(100),
    @Fecha DATE,
    @Horario NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT CAST(CASE WHEN EXISTS (
        SELECT 1
        FROM Turnos t
        INNER JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
        WHERE e.Nombre = @NombreEspecialidad
          AND CAST(t.Fecha AS DATE) = @Fecha
          AND CAST(t.Horario AS TIME) = CAST(@Horario AS TIME)
          AND t.Activo = 1
    ) THEN 1 ELSE 0 END AS INT) AS Existe;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ListarTurnosEmergencia
** Sección       : 5.7
** Propósito     : Obtiene los turnos activos de guardia y del día para el monitor de recepción, triages y contadores de prioridad. Mapea todas las propiedades de `TurnoEmergenciaDTO`, incluyendo la columna `Edad` requerida por Entity Framework Core.
** Entidad/Tablas: `Turnos`, `Prioridades`, `Salas`, `Pacientes`
** Invocado por  : `FrmListaTurnos`, `FrmUsuarioVentana` (Monitor de guardia / Pantalla pública de emergencias (`dgvEmergencias`))
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   : Ninguno.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ListarTurnosEmergencia
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        t.IdTurno,
        t.NroOrden,
        t.NroOrden AS Turno,
        ISNULL(p.Descripcion, 'MEDIA') AS Prioridad,
        ISNULL(p.Descripcion, 'MEDIA') AS Triage,
        LEFT(CAST(t.Horario AS VARCHAR(10)), 5) AS Hora,
        t.Estado,
        ISNULL(s.NombreSala, '--') AS Sala,
        t.TipoTurno,
        t.Fecha,
        t.IdPrioridad,
        t.IdPaciente,
        t.IdEspecialidad,
        CONCAT(pac.Apellido, ', ', pac.Nombre) AS Paciente,
        CAST(NULL AS INT) AS Edad
    FROM Turnos t
    INNER JOIN Pacientes pac ON t.IdPaciente = pac.IdPaciente
    LEFT JOIN Prioridades p ON t.IdPrioridad = p.IdPrioridad
    LEFT JOIN Salas s ON t.IdSala = s.IdSala
    WHERE t.Activo = 1 
      AND t.TipoTurno = 'Emergencia'
      AND (t.Estado IN ('En Espera', 'Llamado', 'En Consulta') 
           OR CAST(t.FechaCreacion AS DATE) = CAST(GETDATE() AS DATE))
    ORDER BY 
        CASE 
            WHEN t.Estado = 'En Espera' THEN 1 
            WHEN t.Estado = 'Llamado' THEN 2 
            WHEN t.Estado = 'En Consulta' THEN 3 
            ELSE 4 
        END ASC,
        t.IdPrioridad ASC, 
        t.FechaCreacion ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ListarTurnosEspecialidad
** Sección       : 5.8
** Propósito     : Obtiene los turnos registrados para una especialidad médica determinada en consultorios externos (`TipoTurno = 'Consulta'`), incluyendo fecha y horario combinados, prioridad y descripción de especialidad.
** Entidad/Tablas: `Turnos`, `Especialidades`, `Prioridades`
** Invocado por  : `FrmListaTurnos` (Cambio de especialidad (`cmbEspecialidades_SelectedIndexChanged`))
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@NombreEspecialidad` (`NVARCHAR(100)`, IN) - Nombre de la especialidad consultada.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ListarTurnosEspecialidad
    @NombreEspecialidad NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        t.IdTurno,
        t.NroOrden,
        t.Estado,
        t.TipoTurno,
        CAST(CAST(t.Fecha AS DATE) AS DATETIME) + CAST(ISNULL(t.Horario, '00:00') AS DATETIME) AS Fecha,
        e.Nombre AS NombreEspecialidad,
        ISNULL(p.Descripcion, 'Normal') AS PrioridadTexto,
        t.IdPrioridad,
        ISNULL(s.NombreSala, '--') AS NombreSala
    FROM Turnos t
    INNER JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Prioridades p ON t.IdPrioridad = p.IdPrioridad
    LEFT JOIN Salas s ON t.IdSala = s.IdSala
    WHERE t.Activo = 1 
      AND t.TipoTurno = 'Consulta'
      AND (e.Nombre = @NombreEspecialidad OR @NombreEspecialidad IS NULL OR @NombreEspecialidad = '')
    ORDER BY t.Fecha ASC, t.Horario ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ListarTurnosGeneralesPantalla
** Sección       : 5.9
** Propósito     : Obtiene el listado de turnos de consultorio y especialidades generales para ser proyectados en la pantalla pública de sala de espera (`FrmUsuarioVentana`).
** Entidad/Tablas: `Turnos`, `Especialidades`, `Salas`
** Invocado por  : `FrmUsuarioVentana` (Carga inicial y auto-refresco de `dgvGeneral`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   : Ninguno.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ListarTurnosGeneralesPantalla
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        t.NroOrden AS Turno,
        LEFT(CAST(t.Horario AS VARCHAR(10)), 5) AS Hora,
        CONVERT(VARCHAR(10), t.Fecha, 103) AS Fecha,
        ISNULL(e.Nombre, 'General') AS Especialidad,
        t.Estado,
        ISNULL(s.NombreSala, '--') AS Sala
    FROM Turnos t
    LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Salas s ON t.IdSala = s.IdSala
    WHERE t.Activo = 1 
      AND t.TipoTurno = 'Consulta'
    ORDER BY t.Fecha ASC, t.Horario ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_InsertarTurno
** Sección       : 5.10
** Propósito     : Inserción simple de turno estándar sin prefijos personalizados de especialidad ni triage automático de emergencias. Superado por `sp_CrearTurnoEspecialidad` y `sp_CrearTurnoEmergencia`.
** Entidad/Tablas: `Turnos`
** Invocado por  : Ninguno actualmente (superado). (N/A)
** Estado        : `NO UTILIZADO (SUPERADO POR sp_CrearTurnoEspecialidad y sp_CrearTurnoEmergencia)`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@NroOrden` (`NVARCHAR(50)`, IN) - Código de orden.
**                `@TipoTurno` (`NVARCHAR(50)`, IN) - Tipo de turno.
**                `@IdPrioridad` (`INT`, IN) - Nivel de prioridad.
**                `@IdPaciente` (`INT`, IN) - ID del paciente.
**                `@IdEspecialidad` (`INT`, IN) - ID de la especialidad.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_InsertarTurno
    @NroOrden NVARCHAR(50),
    @TipoTurno NVARCHAR(50),
    @IdPrioridad INT,
    @IdPaciente INT,
    @IdEspecialidad INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    INSERT INTO Turnos (NroOrden, Estado, Fecha, TipoTurno, IdPrioridad, IdPaciente, IdEspecialidad, Activo, FechaCreacion)
    VALUES (@NroOrden, 'En Espera', GETDATE(), @TipoTurno, @IdPrioridad, @IdPaciente, @IdEspecialidad, 1, GETDATE());
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerTurnosEnEspera
** Sección       : 6.1
** Propósito     : Lista los turnos en espera para una especialidad específica, ordenados por triage (`IdPrioridad ASC`) y orden de llegada (`FechaCreacion ASC`).
** Entidad/Tablas: `Turnos`
** Invocado por  : `FrmListaTurnosAtencion` (`RefrescarListado`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdEspecialidad` (`INT`, IN) - ID de la especialidad.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerTurnosEnEspera
    @IdEspecialidad INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT * 
    FROM Turnos
    WHERE Activo = 1 
      AND Estado = 'En Espera'
      AND IdEspecialidad = @IdEspecialidad
    ORDER BY IdPrioridad ASC, FechaCreacion ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerListaTurnos
** Sección       : 6.2
** Propósito     : Consulta dinámica para la grilla general de turnos. Si `@IdEspecialidad` o `@Estado` se envían como `NULL`, se omite dicho filtro.
** Entidad/Tablas: `Turnos`, `Especialidades`, `Prioridades`
** Invocado por  : `FrmListaTurnos`, `FrmListaTurnosAtencion` (Carga de grillas de seguimiento)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdEspecialidad` (`INT = NULL`, IN) - Filtro opcional por especialidad.
**                `@Estado` (`NVARCHAR(50) = NULL`, IN) - Filtro opcional por estado ('En Espera', 'Llamado', 'Atendido').
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerListaTurnos
    @IdEspecialidad INT = NULL, 
    @Estado NVARCHAR(50) = NULL  
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        t.IdTurno,
        t.NroOrden,
        t.Estado,
        t.TipoTurno,
        t.Fecha,
        e.Nombre AS NombreEspecialidad,
        pr.Descripcion AS PrioridadTexto,
        t.IdPrioridad,
        ISNULL(s.NombreSala, '--') AS NombreSala
    FROM Turnos t
    LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
    LEFT JOIN Salas s ON t.IdSala = s.IdSala
    WHERE t.Activo = 1
      AND (@IdEspecialidad IS NULL OR t.IdEspecialidad = @IdEspecialidad)
      AND (@Estado IS NULL OR t.Estado = @Estado)
    ORDER BY t.IdPrioridad ASC, t.FechaCreacion ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ListarTurnosAtencion
** Sección       : 6.3
** Propósito     : Obtiene la cola activa de turnos en espera para atención médica con datos completos del paciente para la ficha clínica.
** Entidad/Tablas: `Turnos`, `Pacientes`, `Prioridades`, `Especialidades`
** Invocado por  : `FrmListaTurnosAtencion` (Evento `Load` / `CargarTurnosDesdeBD`)
** Estado        : `PENDIENTE DE IMPLEMENTACIÓN`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   : Ninguno.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ListarTurnosAtencion
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        t.IdTurno,
        t.NroOrden,
        t.Fecha,
        t.Estado,
        ISNULL(e.Nombre, 'Emergencias / Guardia') AS Especialidad,
        ISNULL(pr.Descripcion, 'MEDIA') AS Triage,
        p.Nombre AS NombrePaciente,
        p.Apellido AS ApellidoPaciente,
        p.Dni AS DniPaciente,
        p.ObraSocial,
        ISNULL(s.NombreSala, '') AS NombreSala
    FROM Turnos t
    INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
    LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
    LEFT JOIN Salas s ON t.IdSala = s.IdSala
    WHERE t.Activo = 1 
      AND t.Estado = 'En Espera'
    ORDER BY t.IdPrioridad ASC, t.FechaCreacion ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_LlamarSiguienteTurno
** Sección       : 6.4
** Propósito     : Actualiza el estado del turno a `'Llamado'`, asociándole el consultorio de atención, registrando el médico y validando que solo profesionales con especialidad Clínico puedan atender turnos de emergencia.
** Entidad/Tablas: `Turnos`, `Salas`, `MedicosEspecialidades`, `Especialidades`
** Invocado por  : `FrmListaTurnosAtencion` (Botón `btnSiguientePaciente`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdTurno` (`INT`, IN) - ID del turno llamado.
**                `@NombreMedico` (`NVARCHAR(100)`, IN) - Nombre del profesional médico.
**                `@SalaAsignada` (`NVARCHAR(100)`, IN) - Consultorio/sala donde se atiende.
**                `@IdUsuario` (`INT = NULL`, IN) - ID del médico que realiza la atención (opcional para validación de especialidad).
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_LlamarSiguienteTurno
    @IdTurno INT,
    @NombreMedico NVARCHAR(100),
    @SalaAsignada NVARCHAR(100),
    @IdUsuario INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el turno exista y esté activo
        IF NOT EXISTS (SELECT 1 FROM Turnos WHERE IdTurno = @IdTurno AND Activo = 1)
        BEGIN
            THROW 50080, 'El turno a llamar no existe o se encuentra inactivo en el sistema.', 1;
        END

        -- 2. Validar que su estado permita ser llamado (no debe estar Atendido)
        DECLARE @EstadoActual NVARCHAR(50);
        SELECT @EstadoActual = Estado FROM Turnos WHERE IdTurno = @IdTurno;

        IF @EstadoActual = 'Atendido'
        BEGIN
            THROW 50081, 'El turno no puede ser llamado porque su estado actual es incompatible (ya se encuentra Atendido).', 1;
        END

        -- 3. Validar si el turno es de Emergencia y el médico tiene especialidad Clínico
        DECLARE @TipoTurno NVARCHAR(50);
        DECLARE @IdEspecialidadTurno INT;
        SELECT @TipoTurno = TipoTurno, @IdEspecialidadTurno = IdEspecialidad FROM Turnos WHERE IdTurno = @IdTurno;

        IF (@TipoTurno = 'Emergencia' OR EXISTS (SELECT 1 FROM Especialidades WHERE IdEspecialidad = @IdEspecialidadTurno AND Nombre = 'Emergencia'))
           AND @IdUsuario IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1 
                FROM MedicosEspecialidades me
                INNER JOIN Especialidades e ON me.IdEspecialidad = e.IdEspecialidad
                WHERE me.IdUsuario = @IdUsuario 
                  AND me.Activo = 1 
                  AND e.Activo = 1 
                  AND (e.Nombre = 'Clinico' OR e.Nombre LIKE '%clinic%')
            )
            BEGIN
                THROW 50085, 'Solo los profesionales médicos con especialidad en Clínica Médica (Clínico) pueden atender turnos de emergencia.', 1;
            END
        END

        DECLARE @IdSala INT = NULL;
        IF @SalaAsignada IS NOT NULL AND LTRIM(RTRIM(@SalaAsignada)) <> '' AND @SalaAsignada <> '--' AND @SalaAsignada NOT LIKE '%Sin sala abierta%'
        BEGIN
            SELECT TOP 1 @IdSala = IdSala FROM Salas WHERE NombreSala = @SalaAsignada AND Activo = 1;
            IF @IdSala IS NULL
            BEGIN
                SELECT TOP 1 @IdSala = IdSala FROM Salas WHERE (NombreSala LIKE '%' + @SalaAsignada + '%' OR @SalaAsignada LIKE '%' + NombreSala + '%') AND Activo = 1;
            END
        END

        UPDATE Turnos
        SET Estado = 'Llamado',
            IdSala = COALESCE(@IdSala, IdSala),
            IdUsuario = COALESCE(@IdUsuario, IdUsuario),
            FechaModificacion = GETDATE()
        WHERE IdTurno = @IdTurno AND Activo = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_PuedeAtenderEmergencias
** Sección       : 6.4.1
** Propósito     : Comprueba si un usuario médico cuenta con la especialidad Clínico (o Clínica Médica) activa en MedicosEspecialidades para atender turnos de emergencia.
** Entidad/Tablas: `MedicosEspecialidades`, `Especialidades`, `Usuarios`
** Invocado por  : `FrmListaTurnosAtencion` (a través de `TurnoBLL` y `TurnoDAL`)
** Estado        : `EN USO`
** Retorno       : `PuedeAtender` (`BIT`: 1 si está habilitado, 0 si no)
** Parámetros   :
**                `@IdUsuario` (`INT`, IN) - Identificador único del usuario médico.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_PuedeAtenderEmergencias
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT CAST(CASE WHEN EXISTS (
        SELECT 1
        FROM MedicosEspecialidades me
        INNER JOIN Especialidades e ON me.IdEspecialidad = e.IdEspecialidad
        INNER JOIN Usuarios u ON me.IdUsuario = u.IdUsuario
        WHERE me.IdUsuario = @IdUsuario
          AND me.Activo = 1
          AND e.Activo = 1
          AND u.Activo = 1
          AND (e.Nombre = 'Clinico' OR e.Nombre LIKE '%clinic%')
    ) THEN 1 ELSE 0 END AS BIT) AS PuedeAtender;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_IniciarAtencionTurno
** Sección       : 6.5
** Propósito     : Registra el inicio formal de la consulta médica pasando el turno al estado `'En Consulta'` y actualiza atómicamente la sala asignada al estado `'Ocupada'`.
** Entidad/Tablas: `Turnos`, `Salas`
** Invocado por  : `FrmListaTurnosAtencion` (Botón `btnIniciarAtencion`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdTurno` (`INT`, IN) - ID del turno en atención.
**                `@SalaAsignada` (`NVARCHAR(100) = NULL`, IN) - Consultorio físico asignado (opcional).
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_IniciarAtencionTurno
    @IdTurno INT,
    @SalaAsignada NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el turno exista y esté activo
        IF NOT EXISTS (SELECT 1 FROM Turnos WHERE IdTurno = @IdTurno AND Activo = 1)
        BEGIN
            THROW 50082, 'El turno a iniciar atención no existe o se encuentra inactivo.', 1;
        END

        -- 2. Validar que no esté ya finalizado
        IF EXISTS (SELECT 1 FROM Turnos WHERE IdTurno = @IdTurno AND Estado = 'Atendido')
        BEGIN
            THROW 50083, 'No se puede iniciar la atención porque el turno ya ha sido finalizado previamente.', 1;
        END

        DECLARE @IdSala INT = NULL;

        IF @SalaAsignada IS NOT NULL AND LTRIM(RTRIM(@SalaAsignada)) <> '' AND @SalaAsignada <> '--' AND @SalaAsignada NOT LIKE '%Sin sala abierta%'
        BEGIN
            SELECT TOP 1 @IdSala = IdSala FROM Salas WHERE NombreSala = @SalaAsignada AND Activo = 1;
            
            IF @IdSala IS NULL
            BEGIN
                SELECT TOP 1 @IdSala = IdSala FROM Salas WHERE (NombreSala LIKE '%' + @SalaAsignada + '%' OR @SalaAsignada LIKE '%' + NombreSala + '%') AND Activo = 1;
            END
        END

        BEGIN TRANSACTION;

        -- 3. Actualizar el turno a 'En Consulta' y vincular la sala
        UPDATE Turnos
        SET Estado = 'En Consulta',
            IdSala = COALESCE(@IdSala, IdSala),
            FechaModificacion = GETDATE()
        WHERE IdTurno = @IdTurno AND Activo = 1;

        -- 4. Si se identificó la sala, marcarla como 'Ocupada'
        IF @IdSala IS NOT NULL
        BEGIN
            UPDATE Salas
            SET EstadoSala = 'Ocupada',
                FechaModificacion = GETDATE()
            WHERE IdSala = @IdSala AND Activo = 1;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_FinalizarAtencionTurno
** Sección       : 6.6
** Propósito     : Concluye formalmente la consulta médica: pasa el turno a estado `'Atendido'`, vincula la sala física y libera atómicamente la sala a estado `'Disponible'`.
** Entidad/Tablas: `Turnos`, `Salas`
** Invocado por  : `FrmListaTurnosAtencion` (Botón `btnTerminarAtencion`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdTurno` (`INT`, IN) - ID del turno finalizado.
**                `@Diagnostico` (`NVARCHAR(MAX)`, IN) - Diagnóstico o nota de evolución.
**                `@NombreMedico` (`NVARCHAR(100)`, IN) - Médico que finalizó la atención.
**                `@SalaAsignada` (`NVARCHAR(100)`, IN) - Consultorio donde se atendió.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_FinalizarAtencionTurno
    @IdTurno INT,
    @Diagnostico NVARCHAR(MAX),
    @NombreMedico NVARCHAR(100),
    @SalaAsignada NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el turno exista
        IF NOT EXISTS (SELECT 1 FROM Turnos WHERE IdTurno = @IdTurno)
        BEGIN
            THROW 50084, 'El turno a finalizar no existe en el sistema.', 1;
        END

        DECLARE @IdSala INT = NULL;

        IF @SalaAsignada IS NOT NULL AND LTRIM(RTRIM(@SalaAsignada)) <> '' AND @SalaAsignada <> '--' AND @SalaAsignada NOT LIKE '%Sin sala abierta%'
        BEGIN
            SELECT TOP 1 @IdSala = IdSala FROM Salas WHERE NombreSala = @SalaAsignada AND Activo = 1;
            
            IF @IdSala IS NULL
            BEGIN
                SELECT TOP 1 @IdSala = IdSala FROM Salas WHERE (NombreSala LIKE '%' + @SalaAsignada + '%' OR @SalaAsignada LIKE '%' + NombreSala + '%') AND Activo = 1;
            END
        END

        BEGIN TRANSACTION;

        -- 2. Marcar el turno como Atendido y registrar la sala
        UPDATE Turnos
        SET Estado = 'Atendido',
            IdSala = COALESCE(@IdSala, IdSala),
            FechaModificacion = GETDATE()
        WHERE IdTurno = @IdTurno;

        -- 3. Liberar la sala a Disponible
        IF @IdSala IS NOT NULL
        BEGIN
            UPDATE Salas
            SET EstadoSala = 'Disponible',
                FechaModificacion = GETDATE()
            WHERE IdSala = @IdSala AND Activo = 1;
        END
        ELSE IF @SalaAsignada IS NOT NULL AND LTRIM(RTRIM(@SalaAsignada)) <> '' AND @SalaAsignada <> '--' AND @SalaAsignada NOT LIKE '%Sin sala abierta%'
        BEGIN
            UPDATE Salas
            SET EstadoSala = 'Disponible',
                FechaModificacion = GETDATE()
            WHERE NombreSala = @SalaAsignada AND Activo = 1;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_InsertarHistoriaClinica
** Sección       : 6.7
** Propósito     : Registra las notas clínicas, diagnóstico, evolución y recetas farmacológicas emitidas por el médico tratante en la tabla `HistoriasClinicas`. Cuenta con validación de paciente, turno, médico y diagnóstico obligatorio.
** Entidad/Tablas: `HistoriasClinicas`, `Pacientes`, `Turnos`, `Usuarios`
** Invocado por  : `FrmListaTurnosAtencion` (Botón `btnTerminarAtencion`)
** Estado        : `EN USO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@TipoTurno` (`NVARCHAR(50)`, IN) - 'Consulta' o 'Emergencia'.
**                `@DiagRapido` (`NVARCHAR(255)`, IN) - Diagnóstico presuntivo obligatorio.
**                `@DescripHistoriaClinica` (`NVARCHAR(MAX)`, IN) - Evolución médica.
**                `@RecetaMedicamentos` (`NVARCHAR(MAX)`, IN) - Recetas y posología.
**                `@IdPaciente` (`INT`, IN) - ID del paciente atendido.
**                `@IdTurno` (`INT`, IN) - ID del turno atendido.
**                `@IdUsuario` (`INT`, IN) - ID del profesional médico tratante.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_InsertarHistoriaClinica
    @TipoTurno NVARCHAR(50),
    @DiagRapido NVARCHAR(255),
    @DescripHistoriaClinica NVARCHAR(MAX),
    @RecetaMedicamentos NVARCHAR(MAX),
    @IdPaciente INT,
    @IdTurno INT,
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el paciente exista
        IF NOT EXISTS (SELECT 1 FROM Pacientes WHERE IdPaciente = @IdPaciente AND Activo = 1)
        BEGIN
            THROW 50070, 'El paciente vinculado a la historia clínica no existe o se encuentra inactivo.', 1;
        END

        -- 2. Validar que el turno exista
        IF NOT EXISTS (SELECT 1 FROM Turnos WHERE IdTurno = @IdTurno)
        BEGIN
            THROW 50071, 'El turno asociado a la historia clínica no existe en el sistema.', 1;
        END

        -- 3. Validar que el profesional médico exista
        IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario AND Activo = 1)
        BEGIN
            THROW 50072, 'El profesional médico que firma la atención no existe o se encuentra inactivo.', 1;
        END

        -- 4. Validar diagnóstico rápido
        IF @DiagRapido IS NULL OR LTRIM(RTRIM(@DiagRapido)) = ''
        BEGIN
            THROW 50073, 'El diagnóstico médico es un campo obligatorio para registrar la historia clínica.', 1;
        END

        INSERT INTO HistoriasClinicas (Fecha, TipoTurno, DiagRapido, DescripHistoriaClinica, RecetaMedicamentos, IdPaciente, IdTurno, IdUsuario, Activo, FechaCreacion)
        VALUES (GETDATE(), @TipoTurno, @DiagRapido, @DescripHistoriaClinica, @RecetaMedicamentos, @IdPaciente, @IdTurno, @IdUsuario, 1, GETDATE());
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerTurnosPantallaPublica
** Sección       : 6.8
** Propósito     : Alimenta los monitores públicos de sala de espera con los turnos recién llamados (sin exponer diagnósticos privados).
** Entidad/Tablas: `Turnos`, `Salas`, `Usuarios`
** Invocado por  : Pantalla pública de sala de espera. (Consulta periódica (Timer))
** Estado        : `SIN FORM ASOCIADO / PENDIENTE`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   : Ninguno.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerTurnosPantallaPublica
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        t.NroOrden,
        s.NombreSala,
        u.Apellido AS MedicoApellido
    FROM Turnos t
    INNER JOIN Salas s ON t.IdSala = s.IdSala
    INNER JOIN Usuarios u ON t.IdUsuario = u.IdUsuario
    WHERE t.Estado = 'Llamado' 
      AND t.Activo = 1
    ORDER BY t.FechaModificacion DESC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_FinalizarAtencion
** Sección       : 6.9
** Propósito     : Versión previa para la finalización de atención de turnos y liberación de salas. Superado por sp_FinalizarAtencionTurno (que incorpora validación con THROW 50084, registro de evolución diagnóstica y control transaccional).
** Entidad/Tablas: `Turnos`, `Salas`
** Invocado por  : Ninguno actualmente (superado por `sp_FinalizarAtencionTurno`).
** Estado        : `NO UTILIZADO`
** Retorno       : No retorna conjunto de datos (DML/Update)
** Parámetros   :
**                `@IdTurno` (`INT`, IN) - ID del turno a finalizar.
**                `@IdSala` (`INT`, IN) - ID de la sala a liberar.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_FinalizarAtencion
    @IdTurno INT,
    @IdSala INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Marcar el turno como Atendido
        UPDATE Turnos
        SET Estado = 'Atendido',
            FechaModificacion = GETDATE()
        WHERE IdTurno = @IdTurno;

        -- 2. Volver a poner la sala en estado 'Libre' para recibir otro paciente
        UPDATE Salas
        SET EstadoSala = 'Libre',
            FechaModificacion = GETDATE()
        WHERE IdSala = @IdSala;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerHistoriaClinicaPaciente
** Sección       : 6.10
** Propósito     : Permite consultar los antecedentes clínicos completos del paciente ordenados cronológicamente.
** Entidad/Tablas: `HistoriasClinicas`, `Usuarios`
** Invocado por  : `FrmListaTurnosAtencion` (Panel de Atención Actual - Visor de Antecedentes Previos)
** Acción        : Botón `btnIniciarAtencion` (Carga automática de antecedentes al iniciar la atención médica)
** Estado        : `EN USO`
** Retorno       : Registros de HistoriasClinicas vinculadas con datos del médico tratante
** Parámetros   :
**                `@IdPaciente` (`INT`, IN) - ID del paciente consultado.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerHistoriaClinicaPaciente
    @IdPaciente INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT hc.*, u.Apellido AS ApellidoMedico, u.Nombre AS NombreMedico
    FROM HistoriasClinicas hc
    INNER JOIN Usuarios u ON hc.IdUsuario = u.IdUsuario
    WHERE hc.IdPaciente = @IdPaciente AND hc.Activo = 1
    ORDER BY hc.Fecha DESC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ObtenerAtencionesPorMedico
** Sección       : 6.11
** Propósito     : Consulta el historial de consultas y atenciones médicas realizadas 
**                 por un profesional de la salud específico, con soporte de filtrado 
**                 opcional por rango de fechas (FechaDesde / FechaHasta).
** Entidad/Tablas: HistoriasClinicas, Pacientes, Turnos, Salas, Especialidades
** Invocado por  : FrmMisAtenciones (Menú Lateral de Pantalla_Principal_PERSONAL_MEDICO)
** Acción        : Carga inicial y filtrado del módulo "Mis Atenciones Realizadas"
** Estado        : EN USO
** Retorno       : Registros de atenciones con datos del paciente, turno, diagnóstico, receta y sala
** Parámetros   :
**                @IdUsuario   (INT, IN)          - ID del usuario médico a consultar.
**                @FechaDesde  (DATETIME, IN, OPT) - Fecha y hora mínima de consulta.
**                @FechaHasta  (DATETIME, IN, OPT) - Fecha y hora máxima de consulta.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ObtenerAtencionesPorMedico
    @IdUsuario INT,
    @FechaDesde DATETIME = NULL,
    @FechaHasta DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    -- Validar existencia del usuario médico
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario AND Activo = 1)
    BEGIN
        THROW 50030, 'El usuario médico consultado no existe o se encuentra inactivo.', 1;
    END

    SELECT 
        hc.IdHistoria,
        hc.Fecha,
        hc.TipoTurno,
        hc.DiagRapido,
        hc.DescripHistoriaClinica,
        hc.RecetaMedicamentos,
        p.IdPaciente,
        p.Nombre AS NombrePaciente,
        p.Apellido AS ApellidoPaciente,
        p.Dni AS DniPaciente,
        ISNULL(p.ObraSocial, 'Particular') AS ObraSocial,
        ISNULL(t.NroOrden, '--') AS NroOrden,
        ISNULL(s.NombreSala, 'Consultorio') AS NombreSala,
        ISNULL(e.Nombre, 'General') AS Especialidad
    FROM HistoriasClinicas hc
    INNER JOIN Pacientes p ON hc.IdPaciente = p.IdPaciente
    LEFT JOIN Turnos t ON hc.IdTurno = t.IdTurno
    LEFT JOIN Salas s ON t.IdSala = s.IdSala
    LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    WHERE hc.IdUsuario = @IdUsuario
      AND hc.Activo = 1
      AND (@FechaDesde IS NULL OR hc.Fecha >= @FechaDesde)
      AND (@FechaHasta IS NULL OR hc.Fecha <= @FechaHasta)
    ORDER BY hc.Fecha DESC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ReporteDemandaEspecialidades
** Sección       : 6.12
** Propósito     : Genera estadísticas consolidadas de demanda, turnos emitidos,
**                 atenciones efectivas y turnos en espera por especialidad
**                 médica dentro de un rango de fechas opcional.
** Entidad/Tablas: Especialidades, Turnos
** Invocado por  : FrmReportesAdmin (Panel de Reportes Gerenciales)
** Acción        : Carga de métricas y comparativa de demanda por especialidad
** Estado        : EN USO
** Retorno       : Total de turnos, atendidos, en espera y porcentaje de atención por especialidad
** Parámetros   :
**                @FechaDesde     (DATE, IN, OPT) - Fecha mínima de emisión/atención.
**                @FechaHasta     (DATE, IN, OPT) - Fecha máxima de emisión/atención.
**                @IdEspecialidad (INT, IN, OPT)  - Identificador de especialidad a filtrar.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ReporteDemandaEspecialidades
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @IdEspecialidad INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        e.IdEspecialidad,
        e.Nombre AS Especialidad,
        COUNT(t.IdTurno) AS TotalTurnos,
        SUM(CASE WHEN t.Estado IN ('Atendido', 'Finalizado') THEN 1 ELSE 0 END) AS TurnosAtendidos,
        SUM(CASE WHEN t.Estado IN ('En Espera', 'Llamado', 'En Consulta') THEN 1 ELSE 0 END) AS TurnosEnEspera,
        CAST(
            CASE 
                WHEN COUNT(t.IdTurno) > 0 
                THEN (CAST(SUM(CASE WHEN t.Estado IN ('Atendido', 'Finalizado') THEN 1 ELSE 0 END) AS DECIMAL(10,2)) / COUNT(t.IdTurno)) * 100.0
                ELSE 0.0 
            END AS DECIMAL(5,2)
        ) AS PorcentajeAtencion
    FROM Especialidades e
    LEFT JOIN Turnos t ON e.IdEspecialidad = t.IdEspecialidad 
                       AND (@FechaDesde IS NULL OR CAST(t.Fecha AS DATE) >= @FechaDesde)
                       AND (@FechaHasta IS NULL OR CAST(t.Fecha AS DATE) <= @FechaHasta)
                       AND t.Activo = 1
    WHERE (@IdEspecialidad IS NULL OR e.IdEspecialidad = @IdEspecialidad)
      AND e.Activo = 1
    GROUP BY e.IdEspecialidad, e.Nombre
    ORDER BY TotalTurnos DESC, e.Nombre ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ReporteProductividadMedicos
** Sección       : 6.13
** Propósito     : Genera estadísticas consolidadas de productividad médica,
**                 totalizando consultas atendidas y pacientes únicos por profesional
**                 en un rango de fechas opcional.
** Entidad/Tablas: Usuarios, Roles, HistoriasClinicas, MedicoEspecialidades, Especialidades
** Invocado por  : FrmReportesAdmin (Panel de Reportes Gerenciales)
** Acción        : Carga de métricas de productividad profesional
** Estado        : EN USO
** Retorno       : Consultas realizadas y pacientes atendidos por médico
** Parámetros   :
**                @FechaDesde     (DATE, IN, OPT) - Fecha mínima de consulta.
**                @FechaHasta     (DATE, IN, OPT) - Fecha máxima de consulta.
**                @IdEspecialidad (INT, IN, OPT)  - Filtrar por especialidad asignada.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ReporteProductividadMedicos
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @IdEspecialidad INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        u.IdUsuario,
        u.Nombre AS NombreMedico,
        u.Apellido AS ApellidoMedico,
        ISNULL(u.NroMatricula, '--') AS Matricula,
        ISNULL(e.Nombre, 'General') AS Especialidad,
        COUNT(hc.IdHistoria) AS ConsultasAtendidas,
        COUNT(DISTINCT hc.IdPaciente) AS PacientesUnicos
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    LEFT JOIN HistoriasClinicas hc ON u.IdUsuario = hc.IdUsuario 
                                   AND (@FechaDesde IS NULL OR CAST(hc.Fecha AS DATE) >= @FechaDesde)
                                   AND (@FechaHasta IS NULL OR CAST(hc.Fecha AS DATE) <= @FechaHasta)
                                   AND hc.Activo = 1
    LEFT JOIN MedicosEspecialidades me ON u.IdUsuario = me.IdUsuario AND me.Activo = 1
    LEFT JOIN Especialidades e ON me.IdEspecialidad = e.IdEspecialidad
    WHERE (u.IdRol = 1 OR r.Descripcion LIKE '%Médic%' OR r.Descripcion LIKE '%Medic%')
      AND u.Activo = 1
      AND (@IdEspecialidad IS NULL OR e.IdEspecialidad = @IdEspecialidad)
    GROUP BY u.IdUsuario, u.Nombre, u.Apellido, u.NroMatricula, e.Nombre
    ORDER BY ConsultasAtendidas DESC, u.Apellido ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ReporteGuardiaTriage_Resumen
** Sección       : 6.14
** Propósito     : Genera métricas e indicadores clave de guardia (KPIs):
**                 volumen de ingresos por severidad de triage (Alta, Media, Baja),
**                 turnos atendidos, pendientes, cancelados y tasa de resolución.
** Entidad/Tablas: Turnos, Prioridades
** Invocado por  : FrmReporteGuardiaAdmin (Panel Operativo de Guardia)
** Acción        : Carga de tarjetas/indicadores superiores de triage
** Estado        : EN USO
** Retorno       : TotalEmergencias, TotalAlta, TotalMedia, TotalBaja, TotalAtendidos, TotalEnEspera, TotalCancelados, TasaResolucion
** Parámetros   :
**                @FechaDesde   (DATETIME, IN, OPT) - Fecha y hora mínima de ingreso/turno.
**                @FechaHasta   (DATETIME, IN, OPT) - Fecha y hora máxima de ingreso/turno.
**                @IdPrioridad  (INT, IN, OPT)      - Filtrar por nivel de prioridad específico (1=Alta, 2=Media, 3=Baja).
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ReporteGuardiaTriage_Resumen
    @FechaDesde DATETIME = NULL,
    @FechaHasta DATETIME = NULL,
    @IdPrioridad INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            COUNT(t.IdTurno) AS TotalEmergencias,
            SUM(CASE WHEN t.IdPrioridad = 1 THEN 1 ELSE 0 END) AS TotalAlta,
            SUM(CASE WHEN t.IdPrioridad = 2 THEN 1 ELSE 0 END) AS TotalMedia,
            SUM(CASE WHEN t.IdPrioridad = 3 THEN 1 ELSE 0 END) AS TotalBaja,
            SUM(CASE WHEN t.Estado IN ('Atendido', 'En Consulta', 'Finalizado') THEN 1 ELSE 0 END) AS TotalAtendidos,
            SUM(CASE WHEN t.Estado IN ('En Espera', 'Llamado') THEN 1 ELSE 0 END) AS TotalEnEspera,
            SUM(CASE WHEN t.Estado IN ('Cancelado', 'Baja') THEN 1 ELSE 0 END) AS TotalCancelados,
            CAST(
                CASE 
                    WHEN COUNT(t.IdTurno) > 0 
                    THEN (CAST(SUM(CASE WHEN t.Estado IN ('Atendido', 'En Consulta', 'Finalizado') THEN 1 ELSE 0 END) AS DECIMAL(10,2)) / COUNT(t.IdTurno)) * 100.0
                    ELSE 0.0 
                END AS DECIMAL(5,2)
            ) AS TasaResolucion
        FROM Turnos t
        WHERE t.TipoTurno = 'Emergencia'
          AND t.Activo = 1
          AND (@FechaDesde IS NULL OR CAST(COALESCE(t.Fecha, t.FechaCreacion) AS DATE) >= CAST(@FechaDesde AS DATE))
          AND (@FechaHasta IS NULL OR CAST(COALESCE(t.Fecha, t.FechaCreacion) AS DATE) <= CAST(@FechaHasta AS DATE))
          AND (@IdPrioridad IS NULL OR t.IdPrioridad = @IdPrioridad);
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ReporteGuardiaTriage_RankingSintomas
** Sección       : 6.15
** Propósito     : Genera el ranking de motivos de consulta y síntomas más frecuentes
**                 manifestados por los pacientes durante el triage de guardia, con conteo y %.
** Entidad/Tablas: TurnoSintomas, Sintomas, Turnos
** Invocado por  : FrmReporteGuardiaAdmin (Panel Operativo de Guardia)
** Acción        : Carga de grilla/tabla de síntomas predominantes
** Estado        : EN USO
** Retorno       : IdSintoma, Sintoma, Gravedad, CantidadCasos, Porcentaje
** Parámetros   :
**                @FechaDesde   (DATETIME, IN, OPT) - Fecha mínima de consulta.
**                @FechaHasta   (DATETIME, IN, OPT) - Fecha máxima de consulta.
**                @IdPrioridad  (INT, IN, OPT)      - Filtrar por nivel de prioridad específico.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ReporteGuardiaTriage_RankingSintomas
    @FechaDesde DATETIME = NULL,
    @FechaHasta DATETIME = NULL,
    @IdPrioridad INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @TotalSintomas INT = 0;

        SELECT @TotalSintomas = COUNT(ts.IdTurnoSintoma)
        FROM TurnoSintomas ts
        INNER JOIN Sintomas s ON ts.IdSintoma = s.IdSintoma
        INNER JOIN Turnos t ON ts.IdTurno = t.IdTurno
        WHERE t.TipoTurno = 'Emergencia'
          AND t.Activo = 1
          AND ts.Activo = 1
          AND s.Activo = 1
          AND (@FechaDesde IS NULL OR CAST(COALESCE(t.Fecha, t.FechaCreacion) AS DATE) >= CAST(@FechaDesde AS DATE))
          AND (@FechaHasta IS NULL OR CAST(COALESCE(t.Fecha, t.FechaCreacion) AS DATE) <= CAST(@FechaHasta AS DATE))
          AND (@IdPrioridad IS NULL OR t.IdPrioridad = @IdPrioridad);

        SELECT 
            s.IdSintoma,
            s.Descripcion AS Sintoma,
            s.Gravedad,
            COUNT(ts.IdTurnoSintoma) AS CantidadCasos,
            CAST(
                CASE 
                    WHEN @TotalSintomas > 0 
                    THEN (CAST(COUNT(ts.IdTurnoSintoma) AS DECIMAL(10,2)) / @TotalSintomas) * 100.0
                    ELSE 0.0 
                END AS DECIMAL(5,2)
            ) AS Porcentaje
        FROM TurnoSintomas ts
        INNER JOIN Sintomas s ON ts.IdSintoma = s.IdSintoma
        INNER JOIN Turnos t ON ts.IdTurno = t.IdTurno
        WHERE t.TipoTurno = 'Emergencia'
          AND t.Activo = 1
          AND ts.Activo = 1
          AND s.Activo = 1
          AND (@FechaDesde IS NULL OR CAST(COALESCE(t.Fecha, t.FechaCreacion) AS DATE) >= CAST(@FechaDesde AS DATE))
          AND (@FechaHasta IS NULL OR CAST(COALESCE(t.Fecha, t.FechaCreacion) AS DATE) <= CAST(@FechaHasta AS DATE))
          AND (@IdPrioridad IS NULL OR t.IdPrioridad = @IdPrioridad)
        GROUP BY s.IdSintoma, s.Descripcion, s.Gravedad
        ORDER BY CantidadCasos DESC, s.Descripcion ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

/* =========================================================================
** Procedimiento : sp_ReporteGuardiaTriage_Detalle
** Sección       : 6.16
** Propósito     : Consulta detallada e individual de turnos de guardia médica:
**                 número de orden, paciente, DNI, nivel de triage, síntomas asociados,
**                 estado de atención y consultorio/sala asignada.
** Entidad/Tablas: Turnos, Pacientes, Prioridades, Salas, TurnoSintomas, Sintomas
** Invocado por  : FrmReporteGuardiaAdmin (Panel Operativo de Guardia)
** Acción        : Carga de grilla interactiva de turnos de guardia
** Estado        : EN USO
** Retorno       : IdTurno, NroOrden, Fecha, NombrePaciente, ApellidoPaciente, DniPaciente, ObraSocial, Prioridad, IdPrioridad, Sintomas, Estado, NombreSala
** Parámetros   :
**                @FechaDesde   (DATETIME, IN, OPT) - Fecha mínima de consulta.
**                @FechaHasta   (DATETIME, IN, OPT) - Fecha máxima de consulta.
**                @IdPrioridad  (INT, IN, OPT)      - Filtrar por nivel de prioridad específico.
** ========================================================================= */
CREATE OR ALTER PROCEDURE sp_ReporteGuardiaTriage_Detalle
    @FechaDesde DATETIME = NULL,
    @FechaHasta DATETIME = NULL,
    @IdPrioridad INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            t.IdTurno,
            t.NroOrden,
            COALESCE(t.FechaCreacion, t.Fecha) AS Fecha,
            p.Nombre AS NombrePaciente,
            p.Apellido AS ApellidoPaciente,
            p.Dni AS DniPaciente,
            ISNULL(p.ObraSocial, 'Particular') AS ObraSocial,
            ISNULL(pr.Descripcion, 'Baja') AS Prioridad,
            ISNULL(t.IdPrioridad, 3) AS IdPrioridad,
            ISNULL(
                (
                    SELECT STRING_AGG(s.Descripcion, ', ')
                    FROM TurnoSintomas ts
                    INNER JOIN Sintomas s ON ts.IdSintoma = s.IdSintoma
                    WHERE ts.IdTurno = t.IdTurno AND ts.Activo = 1 AND s.Activo = 1
                ),
                'Sin síntomas registrados'
            ) AS Sintomas,
            t.Estado,
            ISNULL(sa.NombreSala, 'Guardia') AS NombreSala
        FROM Turnos t
        INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
        LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
        LEFT JOIN Salas sa ON t.IdSala = sa.IdSala
        WHERE t.TipoTurno = 'Emergencia'
          AND t.Activo = 1
          AND (@FechaDesde IS NULL OR CAST(COALESCE(t.Fecha, t.FechaCreacion) AS DATE) >= CAST(@FechaDesde AS DATE))
          AND (@FechaHasta IS NULL OR CAST(COALESCE(t.Fecha, t.FechaCreacion) AS DATE) <= CAST(@FechaHasta AS DATE))
          AND (@IdPrioridad IS NULL OR t.IdPrioridad = @IdPrioridad)
        ORDER BY t.IdPrioridad ASC, COALESCE(t.FechaCreacion, t.Fecha) DESC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

USE dbGestionTurnos;

GO

-- 1. Roles Base del Sistema
INSERT INTO Roles (Descripcion, Activo, FechaCreacion)
VALUES 
('Administrador', 1, GETDATE()),
('Personal médico', 1, GETDATE()),
('Recepcionista', 1, GETDATE()),
('Usuario Ventana', 1, GETDATE());

GO

-- 2. Catálogo Base de Síntomas para Triage
INSERT INTO Sintomas (Descripcion, Gravedad, Activo, FechaCreacion)
VALUES 
-- Prioridad Alta (Triage 1: Riesgo vital)
('Dolor de pecho opresivo', 'Alta', 1, GETDATE()),
('Dificultad respiratoria severa', 'Alta', 1, GETDATE()),
('Pérdida de conocimiento', 'Alta', 1, GETDATE()),
('Hemorragia abundante', 'Alta', 1, GETDATE()),

-- Prioridad Media (Triage 2: Urgencias moderadas)
('Fiebre alta persistente', 'Media', 1, GETDATE()),
('Dolor abdominal agudo', 'Media', 1, GETDATE()),
('Fractura expuesta o trauma fuerte', 'Media', 1, GETDATE()),
('Reacción alérgica moderada', 'Media', 1, GETDATE()),

-- Prioridad Baja (Triage 3: Guardia regular)
('Dolor de cabeza leve/moderado', 'Baja', 1, GETDATE()),
('Tos y síntomas de resfrío', 'Baja', 1, GETDATE()),
('Dolor muscular o articular', 'Baja', 1, GETDATE()),
('Malestar estomacal leve', 'Baja', 1, GETDATE());

GO

-- 3. Usuario Administrador Inicial
EXEC sp_InsertarUsuario 
    @Nombre = 'Admin', 
    @Apellido = 'Principal', 
    @Correo = 'admin@consultorio.com', 
    @Contrasena = '123456', 
    @Dni = '11223344', 
    @Telefono = '3794000000', 
    @NroMatricula = '0',
    @IdRol = 1;

GO

-- 4. Usuario Ventana / Pantalla de Sala de Espera Inicial
EXEC sp_InsertarUsuario 
    @Nombre = 'Visor', 
    @Apellido = 'SalaEspera', 
    @Correo = 'ventana@consultorio.com', 
    @Contrasena = '123456', 
    @Dni = '99999999', 
    @Telefono = '0000000000', 
    @NroMatricula = '0',
    @IdRol = 4;

GO
