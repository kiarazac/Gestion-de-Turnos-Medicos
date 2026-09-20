# Documentación Oficial de Stored Procedures — Base de Datos `dbGestionTurnos`

Este documento centraliza y especifica todos los **Stored Procedures** del sistema `Gestion_de_Turnos_Medicos` en SQL Server. Se encuentran organizados por módulos funcionales y actúa como el **contrato formal y mapa bidireccional de integración entre la aplicación C# (Forms / BLL / DAL) y SQL Server**.

---

## Convenciones de Base de Datos y Auditoría
- Todas las tablas principales heredan los atributos de auditoría y borrado lógico:
  - `Activo BIT NOT NULL DEFAULT (1)`
  - `FechaCreacion DATETIME NOT NULL DEFAULT (GETDATE())`
  - `FechaModificacion DATETIME NULL`
  - `FechaBaja DATETIME NULL`
- **Baja lógica:** No se realizan sentencias `DELETE`. La baja se ejecuta mediante `UPDATE ... SET Activo = 0, FechaBaja = GETDATE()`.
- **Base de datos objetivo:** `dbGestionTurnos`
- **Estados de Stored Procedures:**
  - `EN USO`: Procedimiento implementado en base de datos y consumido activamente por la aplicación C#.
  - `PENDIENTE DE IMPLEMENTACIÓN`: Procedimiento requerido por la interfaz y lógica C# que debe ser implementado en SQL Server por la compañera de base de datos.
  - `NO UTILIZADO`: Procedimiento existente en base de datos pero que actualmente no es disparado por ningún formulario.
  - `SIN FORM ASOCIADO`: Procedimiento preparado para futuras ampliaciones (ej. pantallas públicas o visor histórico) sin pantalla actual.

---

## Tabla de Contenidos
1. [Módulo 1: Autenticación, Roles y Usuarios](#módulo-1-autenticación-roles-y-usuarios)
2. [Módulo 2: Salas y Consultorios](#módulo-2-salas-y-consultorios)
3. [Módulo 3: Especialidades Médicas](#módulo-3-especialidades-médicas)
4. [Módulo 4: Pacientes](#módulo-4-pacientes)
5. [Módulo 5: Turnos y Triage de Guardia](#módulo-5-turnos-y-triage-de-guardia)
6. [Módulo 6: Atención Médica, Pantalla e Historia Clínica](#módulo-6-atención-médica-pantalla-e-historia-clínica)
7. [Scripts de Datos Iniciales (Seeds)](#scripts-de-datos-iniciales-seeds)
8. [Tabla Resumen General (SP ↔ Form ↔ Estado)](#tabla-resumen-general)
9. [Resumen por Formulario (Form ↔ Stored Procedures)](#resumen-por-formulario)

---

## Módulo 1: Autenticación, Roles y Usuarios

### 1.1 `sp_ValidarLogin`
- **Descripción:** Valida las credenciales de acceso (correo y contraseña) comprobando que el usuario esté activo. Retorna la información personal y la descripción del rol.
- **Entidad:** Usuario / Rol
- **Operación:** Autenticación / Consulta
- **Tablas:** `Usuarios`, `Roles`
- **Forms que lo utilizan:** `FrmLogin`
- **Acción:** Botón `button1` ("Iniciar Sesión")
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Correo` | `NVARCHAR(150)` | IN | Correo electrónico de acceso. |
  | `@Contrasena` | `NVARCHAR(255)` | IN | Contraseña (o hash de contraseña). |
- **Devuelve:** `IdUsuario`, `Nombre`, `Apellido`, `Correo`, `IdRol`, `NombreRol`.
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50020` | *Debe ingresar obligatoriamente el correo electrónico y la contraseña.* | `@Correo` o `@Contrasena` vacíos o nulos. |
  | `50021` | *El correo electrónico ingresado no se encuentra registrado en el sistema.* | `@Correo` no existe en la tabla `Usuarios`. |
  | `50022` | *Este usuario se encuentra dado de baja en el sistema. Contacte al administrador.* | Usuario registrado pero con `Activo = 0`. |
  | `50023` | *La contraseña ingresada es incorrecta. Verifique sus datos e intente nuevamente.* | Contraseña no coincide para el usuario activo. |

```sql
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
```

---

### 1.2 `sp_ListarRoles`
- **Descripción:** Obtiene todos los roles activos configurados en el sistema para poblar el selector desplegable de asignación de roles.
- **Entidad:** Rol
- **Operación:** Listado / Carga de Combo
- **Tablas:** `Roles`
- **Forms que lo utilizan:** `FrmGestionUsuarios`
- **Acción:** Evento `Load` / `CargarRolesDesdeBD`
- **Estado:** `PENDIENTE DE IMPLEMENTACIÓN`
- **Parámetros:** Ninguno.
- **Devuelve:** `IdRol`, `Descripcion`.

```sql
CREATE OR ALTER PROCEDURE sp_ListarRoles
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdRol, Descripcion
    FROM Roles
    WHERE Activo = 1
    ORDER BY IdRol ASC;
END;
GO
```

---

### 1.3 `sp_InsertarRol`
- **Descripción:** Da de alta un nuevo perfil de rol en el sistema.
- **Entidad:** Rol
- **Operación:** Alta
- **Tablas:** `Roles`
- **Forms que lo utilizan:** Ninguno actualmente (se inicializan por script de Seeds).
- **Acción:** N/A
- **Estado:** `NO UTILIZADO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Descripcion` | `NVARCHAR(50)` | IN | Nombre identificatorio del rol. |

```sql
CREATE OR ALTER PROCEDURE sp_InsertarRol
    @Descripcion NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Roles (Descripcion, Activo, FechaCreacion)
    VALUES (@Descripcion, 1, GETDATE());
END;
GO
```

---

### 1.4 `sp_ListarUsuarios`
- **Descripción:** Obtiene el listado de usuarios del sistema para la grilla de administración, concatenando sus especialidades médicas y salas asignadas. Soporta el parámetro opcional `@IncluirInactivos` para listar tanto usuarios activos como aquellos dados de baja lógica.
- **Resolución de Producto Cartesiano:** Utiliza subconsultas correlacionadas con `DISTINCT` y `STRING_AGG` en lugar de `LEFT JOIN` simultáneos a `MedicosEspecialidades` y `DetallesSalas`, evitando la duplicación combinatoria de nombres de salas o especialidades (ej. *"Sala I, Sala I"*).
- **Preservación de Atributos:** Incluye la condición `(me2.Activo = 1 OR u.Activo = 0)` y `(ds2.Activo = 1 OR u.Activo = 0)` para que los usuarios inactivos conserven y muestren sus consultorios y especialidades asignadas históricamente en la grilla y en el formulario.
- **Entidad:** Usuario
- **Operación:** Listado / Grilla
- **Tablas:** `Usuarios`, `Roles`, `MedicosEspecialidades`, `Especialidades`, `DetallesSalas`, `Salas`
- **Forms que lo utilizan:** `FrmGestionUsuarios2`
- **Acción:** Evento `Load` / `CargarUsuariosDesdeBD` / CheckBox `chkMostrarInactivos`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IncluirInactivos` | `BIT` | IN | Opcional (por defecto `0`). Si es `0`, lista solo usuarios con `Activo = 1`. Si es `1`, incluye también usuarios con `Activo = 0`. |
- **Devuelve:** `IdUsuario`, `Nombre`, `Apellido`, `Correo`, `Dni`, `Telefono`, `Rol`, `NroMatricula`, `Especialidades`, `Salas`, `Activo`.

```sql
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
END;
GO
```

---

### 1.5 `sp_InsertarUsuario`
- **Descripción:** Registra un nuevo usuario en la tabla `Usuarios` asignándole su rol correspondiente y devuelve el `IdUsuario` generado.
- **Entidad:** Usuario
- **Operación:** Alta
- **Tablas:** `Usuarios`
- **Forms que lo utilizan:** `FrmGestionUsuarios`
- **Acción:** Botón `btnGuardar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Nombre` | `NVARCHAR(100)` | IN | Nombre del usuario. |
  | `@Apellido` | `NVARCHAR(100)` | IN | Apellido del usuario. |
  | `@Correo` | `NVARCHAR(150)` | IN | Correo electrónico único. |
  | `@Contrasena` | `NVARCHAR(255)` | IN | Contraseña (hasheada en C#). |
  | `@Dni` | `NVARCHAR(20)` | IN | Documento de identidad. |
  | `@Telefono` | `NVARCHAR(20)` | IN | Teléfono de contacto. |
  | `@NroMatricula` | `NVARCHAR(50)` | IN | Número de matrícula (opcional/médicos). |
  | `@IdRol` | `INT` | IN | ID del rol asignado. |
- **Devuelve:** `IdNuevoUsuario` (`CAST(SCOPE_IDENTITY() AS INT)`).
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50011` | *Ya existe un paciente registrado en el sistema con este mismo número de DNI. Un usuario del sistema no puede duplicar el DNI de un paciente.* | `@Dni` coincide con un paciente activo en `Pacientes`. |
  | `50012` | *Ya existe otro usuario registrado con este número de DNI.* | `@Dni` ya asignado a un usuario activo en `Usuarios`. |

```sql
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
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
```

---

### 1.6 `sp_ModificarUsuario`
- **Descripción:** Actualiza los datos de filiación, contacto, identificación, matrícula profesional y rol de un usuario existente en la tabla `Usuarios`. Cuenta con control de transacciones y validaciones de integridad de negocio.
- **Entidad:** Usuario / DetalleSala / MedicoEspecialidad
- **Operación:** Modificación / Reasignación de Salas y Especialidades
- **Tablas:** `Usuarios`, `Roles`, `Pacientes`
- **Forms que lo utilizan:** `FrmGestionUsuarios2` (versión oficial activa en el sistema de turnos; `FrmGestionUsuarios` mantenido como referencia/legado)
- **Acción:** Botón `btnModificar` en `FrmGestionUsuarios2` y edición en grilla
- **Estado:** `EN USO`
- **Parámetros del SP en Base de Datos (dbGestionTurnos):**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT` | IN | ID del usuario a modificar. |
  | `@Nombre` | `NVARCHAR(100)` | IN | Nuevo nombre. |
  | `@Apellido` | `NVARCHAR(100)` | IN | Nuevo apellido. |
  | `@Correo` | `NVARCHAR(150)` | IN | Nuevo correo electrónico. |
  | `@Telefono` | `NVARCHAR(20)` | IN | Nuevo teléfono. |
  | `@Dni` | `NVARCHAR(20)` | IN | Nuevo número de DNI (opcional, conserva anterior si es NULL). |
  | `@NroMatricula` | `NVARCHAR(50)` | IN | Nueva matrícula médica (opcional/médicos). |
  | `@IdRol` | `INT` | IN | Nuevo rol asignado (opcional, conserva anterior si es NULL). |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50030` | *El usuario a modificar no existe o se encuentra inactivo en el sistema.* | `@IdUsuario` inexistente o con `Activo = 0`. |
  | `50037` | *El nombre y el apellido del usuario son campos obligatorios.* | `@Nombre` o `@Apellido` nulos o en blanco. |
  | `50038` | *El correo electrónico es un campo obligatorio.* | `@Correo` nulo o en blanco. |
  | `50031` | *El rol asignado al usuario no existe en el catálogo de roles.* | `@IdRol` no coincide con un rol activo existente. |
  | `50032` | *El número de DNI ingresado ya pertenece a otro usuario registrado en el sistema.* | `@Dni` ya asignado a otro usuario activo. |
  | `50033` | *El número de DNI ingresado ya se encuentra registrado para un paciente en el sistema.* | `@Dni` coincide con un paciente activo. |
  | `50034` | *La dirección de correo electrónico ingresada ya se encuentra registrada por otro usuario.* | `@Correo` ya asignado a otro usuario activo. |

```sql
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
```

---

### 1.7 `sp_EliminarUsuario` (Baja Lógica y Preservación de Atributos)
- **Descripción:** Realiza el borrado lógico (`Activo = 0`) del usuario registrando la fecha de baja (`FechaBaja = GETDATE()`). Protege a la cuenta principal `admin@gmail.com` con inmunidad total y **preserva íntegramente los atributos y asignaciones de salas y especialidades** sin eliminarlos ni borrarlos, permitiendo reactivaciones futuras completas.
- **Inmunidad Administrativa:** La cuenta `admin@gmail.com` está blindada tanto a nivel de interfaz de usuario como en este Stored Procedure arrojando el error `50037`.
- **Preservación de Atributos:** A diferencia de versiones preliminares que desactivaban en cascada `DetallesSalas` y `MedicosEspecialidades`, ahora se conserva la vinculación de consultorios y especialidades intacta para que el usuario no pierda sus configuraciones médicas previas.
- **Entidad:** Usuario
- **Operación:** Baja lógica
- **Tablas:** `Usuarios`, `Turnos`, `HistoriasClinicas`
- **Forms que lo utilizan:** `FrmGestionUsuarios2`
- **Acción:** Botón `btnEliminar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT` | IN | ID del usuario a desactivar. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50035` | *El usuario a desactivar no existe o ya se encuentra inactivo.* | `@IdUsuario` inexistente o con `Activo = 0`. |
  | `50036` | *No se puede desactivar el usuario porque actualmente posee turnos en consulta o llamados activos.* | El médico tiene turnos en curso en estado 'Llamado' o 'En Consulta'. |
  | `50037` | *La cuenta administradora principal (admin@gmail.com) posee inmunidad total y no puede ser desactivada.* | Intento de baja sobre la cuenta con correo `admin@gmail.com`. |

```sql
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
```

---

### 1.8 `sp_ReactivarUsuario` (Re-dar de Alta / Reactivación Lógica)
- **Descripción:** Restablece el estado activo (`Activo = 1`) de un usuario que se encontraba previamente con baja lógica (`Activo = 0`), eliminando la marca temporal de baja (`FechaBaja = NULL`) y actualizando su fecha de modificación. Asimismo, reactiva y restaura sus asignaciones históricas de salas y especialidades médicas (`DetallesSalas` y `MedicosEspecialidades`).
- **Entidad:** Usuario / DetalleSala / MedicoEspecialidad
- **Operación:** Reactivación / Alta lógica
- **Tablas:** `Usuarios`, `DetallesSalas`, `MedicosEspecialidades`
- **Forms que lo utilizan:** `FrmGestionUsuarios2`
- **Acción:** Botón `btnReactivar` ("Re-dar de Alta")
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT` | IN | Identificador del usuario a reactivar. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50035` | *El usuario a reactivar no existe en el sistema.* | `@IdUsuario` inexistente en la tabla `Usuarios`. |
  | `50039` | *El usuario ya se encuentra activo en el sistema.* | `@IdUsuario` ya posee `Activo = 1`. |

```sql
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
```

---

### 1.9 `sp_ListarPersonalMedico`
- **Descripción:** Obtiene los usuarios activos con rol de "Personal médico" para la asignación de profesionales a consultorios y salas.
- **Entidad:** Usuario / Rol
- **Operación:** Listado / Selector
- **Tablas:** `Usuarios`, `Roles`
- **Forms que lo utilizan:** `FrmSalasAdmin`
- **Acción:** Evento `Load` / `CargarPersonalMedicoDesdeBD`
- **Estado:** `PENDIENTE DE IMPLEMENTACIÓN`
- **Parámetros:** Ninguno.
- **Devuelve:** `IdUsuario`, `NombreCompleto`.

```sql
CREATE OR ALTER PROCEDURE sp_ListarPersonalMedico
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.IdUsuario,
        CONCAT('Dr. ', u.Apellido, ', ', u.Nombre) AS NombreCompleto
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    WHERE u.Activo = 1 
      AND (r.Descripcion LIKE '%Médic%' OR r.Descripcion LIKE '%Medic%')
    ORDER BY u.Apellido, u.Nombre;
END;
GO
```

---

## Módulo 2: Salas y Consultorios

### 2.1 `sp_InsertarSala`
- **Descripción:** Registra un nuevo consultorio o sala física en el catálogo de `Salas`, con validación de nombre único y estado operativo válido.
- **Entidad:** Sala
- **Operación:** Alta
- **Tablas:** `Salas`
- **Forms que lo utilizan:** `FrmSalasAdmin`
- **Acción:** Botón `btnGuardar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@NombreSala` | `NVARCHAR(100)` | IN | Nombre de la sala (ej. 'Consultorio 1'). |
  | `@EstadoSala` | `NVARCHAR(50)` | IN | Estado inicial ('Disponible', 'Libre', 'Ocupada', 'En Mantenimiento', 'Cerrada'). |
- **Devuelve:** `IdNuevaSala` (`SCOPE_IDENTITY()`).
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50040` | *El nombre de la sala es obligatorio y no puede quedar vacío.* | `@NombreSala` nulo o en blanco. |
  | `50041` | *Ya existe una sala activa registrada con este mismo nombre.* | `@NombreSala` duplicado con otra sala activa. |
  | `50042` | *El estado especificado para la sala no es válido. Valores permitidos: Disponible, Libre, Ocupada, En Mantenimiento, Cerrada.* | `@EstadoSala` fuera del dominio válido. |

```sql
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
```

---

### 2.2 `sp_ModificarSala`
- **Descripción:** Actualiza el nombre identificatorio de una sala de atención médica en la tabla `Salas`.
- **Entidad:** Sala / DetalleSala
- **Operación:** Modificación / Reasignación de Personal
- **Tablas:** `Salas`
- **Forms que lo utilizan:** `FrmSalasAdmin`
- **Acción:** Botón `btnModificar` y edición interactiva en grilla DataGridView
- **Estado:** `EN USO`
- **Parámetros del SP en Base de Datos (dbGestionTurnos):**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala a modificar. |
  | `@NombreSala` | `NVARCHAR(100)` | IN | Nuevo nombre descriptivo de la sala. |
  | `@EstadoSala` | `NVARCHAR(50) = NULL` | IN | Nuevo estado operativo (opcional, conserva actual si es NULL). |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50043` | *La sala a modificar no existe o se encuentra inactiva.* | `@IdSala` inexistente o con `Activo = 0`. |
  | `50044` | *El nombre de la sala es un campo obligatorio.* | `@NombreSala` nulo o en blanco. |
  | `50045` | *Ya existe otra sala activa registrada con este mismo nombre.* | `@NombreSala` coincide con otra sala activa. |
  | `50042` | *El estado especificado para la sala no es válido. Valores permitidos: Disponible, Libre, Ocupada, En Mantenimiento, Cerrada.* | `@EstadoSala` no coincide con los estados permitidos. |

```sql
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
```

---

### 2.3 `sp_EliminarSala` (Baja Lógica)
- **Descripción:** Realiza la baja lógica de la sala (`Activo = 0`), verificando que no esté ocupada con atención médica activa y dando de baja en cascada las asignaciones de médicos en `DetallesSalas`.
- **Entidad:** Sala / DetalleSala
- **Operación:** Baja lógica / Desactivación en cascada
- **Tablas:** `Salas`, `DetallesSalas`
- **Forms que lo utilizan:** `FrmSalasAdmin`
- **Acción:** Botón `btnEliminar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala a desactivar. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50046` | *La sala a eliminar no existe o ya se encuentra inactiva.* | `@IdSala` inexistente o con `Activo = 0`. |
  | `50047` | *No se puede eliminar la sala porque se encuentra en estado Ocupada con una atención médica en curso.* | `EstadoSala = 'Ocupada'`. |

```sql
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
```

---

### 2.4 `sp_ObtenerSalas`
- **Descripción:** Lista las salas de atención médica. Si `@IdUsuario` es `NULL`, devuelve todas las salas (vista administrativa); si se provee un médico, retorna exclusivamente las salas asignadas a su perfil. Admite el parámetro opcional `@IncluirInactivas` para visualizar y reactivar salas dadas de baja lógica en `FrmSalasAdmin`.
- **Entidad:** Sala / DetalleSala
- **Operación:** Listado / Consulta
- **Tablas:** `Salas`, `DetallesSalas`, `Usuarios`
- **Forms que lo utilizan:** `FrmSalasAdmin`, `FrmGestionUsuarios2`, `MisSalas_PM`
- **Acción:** Carga de grilla de salas (`CargarSalasDesdeBD`, `CargarMisSalas`, `chkMostrarInactivas_CheckedChanged`)
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT = NULL` | IN | ID opcional del médico. Si es NULL, lista todos los consultorios. |
  | `@IncluirInactivas` | `BIT = 0` | IN | Si es 1, incluye salas dadas de baja lógica (`Activo = 0`). |
- **Devuelve:** `IdSala`, `NombreSala`, `EstadoSala`, `IdUsuario`, `NombreMedico`, `ApellidoMedico`, `DescripcionAtencion`, `Activo`.

```sql
CREATE OR ALTER PROCEDURE sp_ObtenerSalas
    @IdUsuario INT = NULL,
    @IncluirInactivas BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

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
END;
GO
```

---

### 2.5 `sp_AsignarSalaMedico`
- **Descripción:** Vincula un médico a un consultorio en la tabla `DetallesSalas`, validando existencia y rol médico, y previniendo duplicados activos.
- **Entidad:** DetalleSala
- **Operación:** Asignación
- **Tablas:** `DetallesSalas`, `Salas`, `Usuarios`
- **Forms que lo utilizan:** `FrmGestionUsuarios2`, `FrmSalasAdmin`
- **Acción:** Botón `btnGuardar` / `btnModificar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala asignada. |
  | `@IdUsuario` | `INT` | IN | ID del médico asignado. |
  | `@DescripcionAtencion` | `NVARCHAR(255) = NULL` | IN | Notas u horario de atención. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50060` | *La sala especificada para la asignación no existe o se encuentra inactiva.* | `@IdSala` inexistente o con `Activo = 0`. |
  | `50061` | *El usuario asignado no existe, está inactivo o no pertenece al rol de Personal Médico.* | `@IdUsuario` inexistente, inactivo o con rol que no corresponde a Personal Médico (`IdRol = 1` o descripción con 'médico'). |

```sql
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
```

---

### 2.6 `sp_AbrirSala`
- **Descripción:** Habilita el consultorio para atención (`EstadoSala = 'Libre'`). Valida que el médico no tenga otra sala abierta y que esté asignado a la sala.
- **Entidad:** Sala / DetalleSala
- **Operación:** Cambio de Estado / Validación de Negocio
- **Tablas:** `Salas`, `DetallesSalas`
- **Forms que lo utilizan:** `MisSalas_PM`
- **Acción:** Botón `btnAbrirSala`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala a abrir. |
  | `@IdUsuario` | `INT` | IN | ID del médico autenticado. |

```sql
CREATE OR ALTER PROCEDURE sp_AbrirSala
    @IdSala INT,
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 
        FROM DetallesSalas ds
        INNER JOIN Salas s ON ds.IdSala = s.IdSala
        WHERE ds.IdUsuario = @IdUsuario 
          AND s.EstadoSala IN ('Libre', 'Disponible', 'Ocupada') 
          AND s.Activo = 1
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

    UPDATE Salas
    SET EstadoSala = 'Disponible',
        FechaModificacion = GETDATE()
    WHERE IdSala = @IdSala AND Activo = 1;
END;
GO
```

---

### 2.7 `sp_CerrarSala`
- **Descripción:** Cambia el estado de la sala a `'Cerrada'`. Bloquea si la sala está en consulta con un paciente (`'Ocupada'`).
- **Entidad:** Sala
- **Operación:** Cambio de Estado / Validación de Negocio
- **Tablas:** `Salas`
- **Forms que lo utilizan:** `MisSalas_PM`
- **Acción:** Botón `btnCerrarSala`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala a cerrar. |

```sql
CREATE OR ALTER PROCEDURE sp_CerrarSala
    @IdSala INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 
        FROM Salas 
        WHERE IdSala = @IdSala AND EstadoSala = 'Ocupada' AND Activo = 1
    )
    BEGIN
        THROW 50001, 'No se puede cerrar la sala porque hay un paciente siendo atendido actualmente.', 1;
    END

    UPDATE Salas
    SET EstadoSala = 'Cerrada',
        FechaModificacion = GETDATE()
    WHERE IdSala = @IdSala AND Activo = 1;
END;
GO
```

---

### 2.8 `sp_ActualizarEstadoSala`
- **Descripción:** Actualiza de forma genérica el estado operativo de un consultorio o sala física (`Disponible`, `Libre`, `Ocupada`, `En Mantenimiento`, `Cerrada`) con control de transacciones y validaciones de integridad.
- **Entidad:** Sala
- **Operación:** Actualización de Estado
- **Tablas:** `Salas`
- **Forms que lo utilizan:** Utilizado por la capa DAL/BLL (`SalaDAL.ActualizarEstadoSala`, `SalaBLL.ActualizarEstadoSala`)
- **Acción:** Mantenimiento de salas y soporte operativo
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala a modificar. |
  | `@NuevoEstado` | `NVARCHAR(50)` | IN | Nuevo estado operativo a asignar. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50048` | *La sala a actualizar no existe o se encuentra inactiva.* | `@IdSala` no existe o tiene `Activo = 0`. |
  | `50049` | *El estado operativo asignado a la sala no es válido.* | `@NuevoEstado` no pertenece al conjunto permitido ('Disponible', 'Libre', 'Ocupada', 'En Mantenimiento', 'Cerrada'). |

```sql
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
```

---

### 2.9 `sp_ReactivarSala`
- **Descripción:** Reactiva lógicamente una sala dada de baja previamente (`Activo = 1`, `FechaBaja = NULL`), restaura sus asignaciones históricas en `DetallesSalas` y valida existencia y estado actual.
- **Entidad:** Sala / DetalleSala
- **Operación:** Reactivación / Alta lógica
- **Tablas:** `Salas`, `DetallesSalas`
- **Forms que lo utilizan:** `FrmSalasAdmin`
- **Acción:** Botón `btnReactivar` ("♻ Re-dar de Alta Sala")
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala a reactivar. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50043` | *La sala especificada no existe en el sistema.* | `@IdSala` inexistente en la tabla `Salas`. |
  | `50044` | *La sala ya se encuentra activa en el sistema.* | La sala posee `Activo = 1`. |

```sql
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
```

---

## Módulo 3: Especialidades Médicas

### 3.1 `sp_ListarEspecialidades` / `sp_ObtenerEspecialidades`
- **Descripción:** Obtiene la lista completa de especialidades médicas. Admite el parámetro opcional `@IncluirInactivas` para visualizar y reactivar especialidades dadas de baja lógica en `FrmGestionEspecialidades`.
- **Entidad:** Especialidad
- **Operación:** Listado
- **Tablas:** `Especialidades`
- **Forms que lo utilizan:** `FrmGestionEspecialidades`, `FrmGestionUsuarios2`, `FrmTurnoEspecialidad`, `FrmListaTurnos`, `FrmListaTurnosAtencion`
- **Acción:** Carga de catálogos, selectores y grilla administrativa (`chkMostrarInactivas_CheckedChanged`)
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IncluirInactivas` | `BIT = 0` | IN | Si es 1, incluye especialidades dadas de baja lógica (`Activo = 0`). |
- **Devuelve:** `IdEspecialidad`, `Nombre`, `Activo`.

```sql
CREATE OR ALTER PROCEDURE sp_ListarEspecialidades
    @IncluirInactivas BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        IdEspecialidad, 
        Nombre,
        Activo
    FROM Especialidades
    WHERE (@IncluirInactivas = 1 OR Activo = 1)
    ORDER BY Activo DESC, Nombre ASC;
END;
GO
```

---

### 3.2 `sp_InsertarEspecialidad`
- **Descripción:** Registra una nueva especialidad médica en el catálogo con validación de nombre no vacío y no duplicado. Si existía previamente dada de baja, la reactiva automáticamente.
- **Entidad:** Especialidad
- **Operación:** Alta / Reactivación
- **Tablas:** `Especialidades`
- **Forms que lo utilizan:** `FrmGestionEspecialidades`
- **Acción:** Botón `btnGuardar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Nombre` | `NVARCHAR(100)` | IN | Nombre de la especialidad. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50050` | *El nombre de la especialidad es obligatorio y no puede quedar vacío.* | `@Nombre` es nulo o cadena vacía/espacios. |
  | `50051` | *Ya existe una especialidad activa registrada con este mismo nombre.* | Ya existe otra especialidad con `Nombre = @Nombre` y `Activo = 1`. |

```sql
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
```

---

### 3.3 `sp_ModificarEspecialidad`
- **Descripción:** Modifica la denominación de una especialidad médica existente con validación de no duplicados y nombre no vacío.
- **Entidad:** Especialidad
- **Operación:** Modificación
- **Tablas:** `Especialidades`
- **Forms que lo utilizan:** `FrmGestionEspecialidades`
- **Acción:** Botón `btnModificar` ("💾 Modificar Especialidad")
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdEspecialidad` | `INT` | IN | ID de la especialidad a modificar. |
  | `@Nombre` | `NVARCHAR(100)` | IN | Nuevo nombre asignado. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50050` | *El nombre de la especialidad es obligatorio y no puede quedar vacío.* | `@Nombre` es nulo o cadena en blanco. |
  | `50054` | *La especialidad a modificar no existe en el sistema.* | `@IdEspecialidad` no existe. |
  | `50051` | *Ya existe otra especialidad activa registrada con este mismo nombre.* | `@Nombre` coincide con otra especialidad activa. |

```sql
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
```

---

### 3.4 `sp_EliminarEspecialidad` (Baja Lógica)
- **Descripción:** Realiza el borrado lógico de una especialidad médica (`Activo = 0`) previa validación de que no posea turnos pendientes o en atención en curso, y desactiva en cascada sus asignaciones médicas en `MedicosEspecialidades`.
- **Entidad:** Especialidad / MedicoEspecialidad
- **Operación:** Baja lógica transaccional en cascada
- **Tablas:** `Especialidades`, `MedicosEspecialidades`, `Turnos`
- **Forms que lo utilizan:** `FrmGestionEspecialidades`
- **Acción:** Botón `btnDesactivar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdEspecialidad` | `INT` | IN | ID de la especialidad a desactivar. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50052` | *La especialidad a desactivar no existe o ya se encuentra inactiva.* | `@IdEspecialidad` no existe o tiene `Activo = 0`. |
  | `50053` | *No se puede dar de baja la especialidad porque posee turnos pendientes de atención (En Espera, Llamado o En Consulta).* | Existen turnos activos en dichos estados para la especialidad. |

```sql
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
```

---

### 3.5 `sp_AsignarEspecialidadMedico`
- **Descripción:** Asocia una especialidad a un profesional médico en la tabla intermedia `MedicosEspecialidades`, validando la existencia de ambos y previniendo duplicados activos.
- **Entidad:** MedicoEspecialidad
- **Operación:** Asignación
- **Tablas:** `MedicosEspecialidades`, `Usuarios`, `Especialidades`
- **Forms que lo utilizan:** `FrmGestionUsuarios2`
- **Acción:** Botón `btnGuardar` / `btnModificar` en `FrmGestionUsuarios2`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT` | IN | ID del médico. |
  | `@IdEspecialidad` | `INT` | IN | ID de la especialidad vinculada. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50062` | *El profesional médico a asignar no existe, está inactivo o no pertenece al rol de Personal Médico.* | `@IdUsuario` inexistente, inactivo o con rol distinto de Personal Médico (`IdRol = 1` o descripción con 'médico'). |
  | `50063` | *La especialidad médica a asignar no existe o se encuentra inactiva.* | `@IdEspecialidad` no existe o no está activa. |

```sql
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
```

---

### 3.6 `sp_ObtenerEspecialidadesPorMedico`
- **Descripción:** Obtiene las especialidades activas asignadas a un médico en particular a través de la tabla intermedia `MedicosEspecialidades`.
- **Entidad:** Especialidad / MedicoEspecialidad
- **Operación:** Consulta por Médico
- **Tablas:** `Especialidades`, `MedicosEspecialidades`
- **Forms que lo utilizan:** `FrmGestionUsuarios2`, `FrmListaTurnosAtencion`
- **Acción:** Carga de especialidades vinculadas al profesional (`CargarServiciosDelMedico` y precarga en `FrmGestionUsuarios2`)
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT` | IN | ID del médico consultado. |
- **Devuelve:** `IdEspecialidad`, `Nombre`.

```sql
CREATE OR ALTER PROCEDURE sp_ObtenerEspecialidadesPorMedico
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.IdEspecialidad,
        e.Nombre
    FROM Especialidades e
    INNER JOIN MedicosEspecialidades me ON e.IdEspecialidad = me.IdEspecialidad
    WHERE me.IdUsuario = @IdUsuario 
      AND me.Activo = 1 
      AND e.Activo = 1
    ORDER BY e.Nombre ASC;
END;
GO
```

---

### 3.7 `sp_ReactivarEspecialidad`
- **Descripción:** Reactiva lógicamente una especialidad médica previamente desactivada (`Activo = 1`, `FechaBaja = NULL`), restaura sus vínculos históricos con profesionales médicos en `MedicosEspecialidades` y valida su existencia.
- **Entidad:** Especialidad / MedicoEspecialidad
- **Operación:** Reactivación / Alta lógica
- **Tablas:** `Especialidades`, `MedicosEspecialidades`
- **Forms que lo utilizan:** `FrmGestionEspecialidades`
- **Acción:** Botón `btnReactivar` ("♻ Re-dar de Alta")
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdEspecialidad` | `INT` | IN | ID de la especialidad a reactivar. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50055` | *La especialidad a reactivar no existe en el sistema.* | `@IdEspecialidad` inexistente en la tabla `Especialidades`. |
  | `50056` | *La especialidad ya se encuentra activa en el sistema.* | La especialidad posee `Activo = 1`. |

```sql
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
```

---

## Módulo 4: Pacientes

### 4.1 `sp_BuscarPacientePorDNI`
- **Descripción:** Busca y recupera la información de un paciente activo a partir de su número de documento de identidad (DNI).
- **Entidad:** Paciente
- **Operación:** Consulta / Búsqueda por DNI
- **Tablas:** `Pacientes`
- **Forms que lo utilizan:** `FrmTurnoEmergencia`, `FrmTurnoEspecialidad`
- **Acción:** Autocompletado y verificación de antecedentes al ingresar DNI
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@DNI` | `NVARCHAR(20)` | IN | Documento de identidad del paciente. |
- **Devuelve:** `IdPaciente`, `Nombre`, `Apellido`, `Dni`, `ObraSocial`.

```sql
CREATE OR ALTER PROCEDURE sp_BuscarPacientePorDNI
    @DNI NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdPaciente, Nombre, Apellido, Dni, ObraSocial 
    FROM Pacientes 
    WHERE DNI = @DNI AND Activo = 1;
END;
GO
```

---

### 4.2 `sp_GuardarPaciente` (Upsert Inteligente)
- **Descripción:** Busca al paciente por DNI. Valida que dicho DNI no pertenezca a un usuario del sistema (código `50010`). Si el paciente ya existe en el sistema, actualiza su nombre, apellido y cobertura médica y retorna su `IdPaciente`. Si no existe, lo inserta en `Pacientes` y retorna el nuevo ID autoincremental generado.
- **Entidad:** Paciente
- **Operación:** Búsqueda / Alta / Actualización con Integridad Cruzada
- **Tablas:** `Pacientes`, `Usuarios`
- **Forms que lo utilizan:** `FrmTurnoEmergencia`, `FrmTurnoEspecialidad`
- **Acción:** Botón `BtnGenerarTurno`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Nombre` | `NVARCHAR(100)` | IN | Nombre del paciente. |
  | `@Apellido` | `NVARCHAR(100)` | IN | Apellido del paciente. |
  | `@Dni` | `NVARCHAR(20)` | IN | Documento de identidad. |
  | `@ObraSocial` | `NVARCHAR(100)` | IN | Cobertura médica u obra social. |
- **Devuelve:** 1 fila: `IdPaciente`.
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50010` | *Ya existe un usuario registrado en el sistema con este mismo número de DNI. No se puede duplicar.* | El `@Dni` ingresado coincide con un usuario registrado y activo en `Usuarios`. |

```sql
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
```

---

### 4.3 `sp_InsertarPaciente`
- **Descripción:** Inserción simple y directa de un paciente sin validación de duplicidad por DNI ni verificación cruzada con la tabla de usuarios. Fue superado operativamente por `sp_GuardarPaciente`.
- **Entidad:** Paciente
- **Operación:** Alta simple
- **Tablas:** `Pacientes`
- **Forms que lo utilizan:** Ninguno actualmente (reemplazado por `sp_GuardarPaciente`).
- **Acción:** N/A
- **Estado:** `NO UTILIZADO (SUPERADO POR sp_GuardarPaciente)`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Nombre` | `NVARCHAR(100)` | IN | Nombre del paciente. |
  | `@Apellido` | `NVARCHAR(100)` | IN | Apellido del paciente. |
  | `@Dni` | `NVARCHAR(20)` | IN | Documento de identidad. |
  | `@ObraSocial` | `NVARCHAR(100)` | IN | Cobertura médica. |
- **Devuelve:** `IdNuevoPaciente` (`SCOPE_IDENTITY()`).

```sql
CREATE OR ALTER PROCEDURE sp_InsertarPaciente
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni NVARCHAR(20),
    @ObraSocial NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Pacientes (Nombre, Apellido, Dni, ObraSocial, FechaCreacion, Activo)
    VALUES (@Nombre, @Apellido, @Dni, @ObraSocial, GETDATE(), 1);

    SELECT SCOPE_IDENTITY() AS IdNuevoPaciente;
END;
GO
```

---

## Módulo 5: Turnos y Triage de Guardia

### 5.1 `sp_ObtenerSintomas`
- **Descripción:** Retorna el catálogo de síntomas activos con su nivel de gravedad para el triage dinámico de guardia.
- **Entidad:** Sintoma
- **Operación:** Consulta / Catálogo
- **Tablas:** `Sintomas`
- **Forms que lo utilizan:** `FrmTurnoEmergencia`
- **Acción:** Evento `Load` / `CargarCatalogoSintomas`
- **Estado:** `EN USO`
- **Devuelve:** `IdSintoma`, `Descripcion`, `Gravedad`.

```sql
CREATE OR ALTER PROCEDURE sp_ObtenerSintomas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdSintoma, Descripcion, Gravedad 
    FROM Sintomas 
    WHERE Activo = 1
    ORDER BY Gravedad ASC, Descripcion ASC;
END;
GO
```

---

### 5.2 `sp_ObtenerGravedadSintoma`
- **Descripción:** Obtiene la gravedad nativa de un síntoma específico para el cálculo de nivel de triage de emergencias.
- **Entidad:** Sintoma
- **Operación:** Consulta de Triage
- **Tablas:** `Sintomas`
- **Forms que lo utilizan:** `FrmTurnoEmergencia`
- **Acción:** Selección de síntoma en el selector de triage
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSintoma` | `INT` | IN | ID del síntoma seleccionado. |
- **Devuelve:** `Gravedad` (1 = Alta, 2 = Media, 3 = Baja).

```sql
CREATE OR ALTER PROCEDURE sp_ObtenerGravedadSintoma
    @IdSintoma INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Gravedad 
    FROM Sintomas 
    WHERE IdSintoma = @IdSintoma;
END;
GO
```

---

### 5.3 `sp_GuardarTurnoSintoma`
- **Descripción:** Vincula un síntoma al turno de atención médica en la tabla `TurnoSintomas`, validando que ambos existan y se encuentren activos, y previniendo la duplicación del mismo síntoma en el turno.
- **Entidad:** TurnoSintoma / Turno / Sintoma
- **Operación:** Alta de Relación con Integridad
- **Tablas:** `TurnoSintomas`, `Turnos`, `Sintomas`
- **Forms que lo utilizan:** `FrmTurnoEmergencia`
- **Acción:** Botón `BtnGenerarTurno`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno generado. |
  | `@IdSintoma` | `INT` | IN | ID del síntoma asociado. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50085` | *El turno especificado para vincular el síntoma no existe o se encuentra inactivo.* | `@IdTurno` no existe o tiene `Activo = 0`. |
  | `50086` | *El síntoma especificado no existe o se encuentra inactivo.* | `@IdSintoma` no existe o tiene `Activo = 0`. |

```sql
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
```

---

### 5.4 `sp_CrearTurnoEmergencia`
- **Descripción:** Genera un nuevo turno de guardia de urgencias/emergencias, asociándolo al servicio de Emergencia, asignando la prioridad de triage correspondiente y generando el ticket correlativo `E-001`. Cuenta con validaciones de negocio previas.
- **Entidad:** Turno / Paciente / Especialidad
- **Operación:** Alta con Triage y Numeración Correlativa
- **Tablas:** `Turnos`, `Pacientes`, `Especialidades`
- **Forms que lo utilizan:** `FrmTurnoEmergencia`
- **Acción:** Botón `BtnGenerarTurno`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdPaciente` | `INT` | IN | ID del paciente asistido. |
  | `@IdPrioridad` | `INT` | IN | Nivel de prioridad calculado (1=Alta, 2=Media, 3=Baja). |
- **Devuelve:** `IdNuevoTurno`, `NroOrden`.
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50001` | *El paciente seleccionado no existe o se encuentra inactivo en el sistema.* | `@IdPaciente` inexistente o inactivo en `Pacientes`. |
  | `50002` | *No se encontró configurada una especialidad activa para "Emergencia" en la base de datos.* | La especialidad "Emergencia" no está registrada activa. |

```sql
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
```

---

### 5.5 `sp_ObtenerHorariosDisponibles`
- **Descripción:** Retorna las franjas horarias libres (no reservadas) para una fecha y especialidad específicas.
- **Entidad:** Turno / Especialidad
- **Operación:** Consulta de Disponibilidad
- **Tablas:** `Turnos`, `Especialidades`
- **Forms que lo utilizan:** `FrmTurnoEspecialidad`
- **Acción:** Selección de fecha en calendario (`calFechaTurno_DateChanged`)
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@NombreEspecialidad` | `NVARCHAR(100)` | IN | Nombre de la especialidad. |
  | `@Fecha` | `DATE` | IN | Fecha consultada. |
- **Devuelve:** Filas con columna `Horario` (`VARCHAR(10)`).

```sql
CREATE OR ALTER PROCEDURE sp_ObtenerHorariosDisponibles
    @NombreEspecialidad NVARCHAR(100),
    @Fecha DATE
AS
BEGIN
    SET NOCOUNT ON;

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
          AND t.Estado <> 'Cancelado'
    )
    ORDER BY h.Horario ASC;
END;
GO
```

---

### 5.6 `sp_CrearTurnoEspecialidad`
- **Descripción:** Registra un turno programado de consultorio externo vinculando paciente, especialidad, fecha y horario asignado. Genera el código correlativo prefijado con la inicial de la especialidad (ej. `C-001` para Cardiología, `P-001` para Pediatría) y devuelve el ID recién creado.
- **Entidad:** Turno / Especialidad / Paciente
- **Operación:** Alta Programada con Validación de Negocio
- **Tablas:** `Turnos`, `Especialidades`, `Pacientes`
- **Forms que lo utilizan:** `FrmTurnoEspecialidad`
- **Acción:** Botón `BtnGenerarTurno`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdPaciente` | `INT` | IN | ID del paciente. |
  | `@NombreEspecialidad` | `NVARCHAR(100)` | IN | Especialidad elegida. |
  | `@Fecha` | `DATE` | IN | Fecha del turno. |
  | `@Horario` | `NVARCHAR(10)` | IN | Horario asignado (ej. '10:30'). |
  | `@Estado` | `NVARCHAR(50)` | IN | Estado inicial ('En Espera'). |
- **Devuelve:** `IdNuevoTurno`, `NroOrden`.
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50001` | *El paciente especificado no se encuentra registrado o está inactivo.* | `@IdPaciente` inexistente o inactivo en `Pacientes`. |
  | `50003` | *La especialidad seleccionada no existe o se encuentra inactiva.* | `@NombreEspecialidad` no encontrada o inactiva en `Especialidades`. |

```sql
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
```

---

### 5.7 `sp_ListarTurnosEmergencia`
- **Descripción:** Obtiene los turnos activos de guardia y del día para el monitor de recepción, triages y contadores de prioridad. Mapea todas las propiedades de `TurnoEmergenciaDTO`, incluyendo la columna `Edad` requerida por Entity Framework Core.
- **Entidad:** Turno / Prioridad / Sala / Paciente
- **Operación:** Listado / Monitor
- **Tablas:** `Turnos`, `Prioridades`, `Salas`, `Pacientes`
- **Forms que lo utilizan:** `FrmListaTurnos`, `FrmUsuarioVentana`
- **Acción:** Monitor de guardia / Pantalla pública de emergencias (`dgvEmergencias`)
- **Estado:** `EN USO`
- **Devuelve:** `IdTurno`, `NroOrden`, `Turno`, `Prioridad`, `Triage`, `Hora`, `Estado`, `Sala`, `TipoTurno`, `Fecha`, `IdPrioridad`, `IdPaciente`, `IdEspecialidad`, `Paciente`, `Edad`.

```sql
CREATE OR ALTER PROCEDURE sp_ListarTurnosEmergencia
AS
BEGIN
    SET NOCOUNT ON;

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
END;
GO
```

---

### 5.8 `sp_ListarTurnosEspecialidad`
- **Descripción:** Obtiene los turnos registrados para una especialidad médica determinada en consultorios externos (`TipoTurno = 'Consulta'`), incluyendo fecha y horario combinados, prioridad y descripción de especialidad.
- **Entidad:** Turno / Especialidad
- **Operación:** Consulta por Filtro
- **Tablas:** `Turnos`, `Especialidades`, `Prioridades`
- **Forms que lo utilizan:** `FrmListaTurnos`
- **Acción:** Cambio de especialidad (`cmbEspecialidades_SelectedIndexChanged`)
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@NombreEspecialidad` | `NVARCHAR(100)` | IN | Nombre de la especialidad consultada. |
- **Devuelve:** `IdTurno`, `NroOrden`, `Estado`, `TipoTurno`, `Fecha`, `NombreEspecialidad`, `PrioridadTexto`, `IdPrioridad`.

```sql
CREATE OR ALTER PROCEDURE sp_ListarTurnosEspecialidad
    @NombreEspecialidad NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.IdTurno,
        t.NroOrden,
        t.Estado,
        t.TipoTurno,
        CAST(CAST(t.Fecha AS DATE) AS DATETIME) + CAST(ISNULL(t.Horario, '00:00') AS DATETIME) AS Fecha,
        e.Nombre AS NombreEspecialidad,
        ISNULL(p.Descripcion, 'Normal') AS PrioridadTexto,
        t.IdPrioridad
    FROM Turnos t
    INNER JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Prioridades p ON t.IdPrioridad = p.IdPrioridad
    WHERE t.Activo = 1 
      AND t.TipoTurno = 'Consulta'
      AND (e.Nombre = @NombreEspecialidad OR @NombreEspecialidad IS NULL OR @NombreEspecialidad = '')
    ORDER BY t.Fecha ASC, t.Horario ASC;
END;
GO
```

---

### 5.9 `sp_ListarTurnosGeneralesPantalla`
- **Descripción:** Obtiene el listado de turnos de consultorio y especialidades generales para ser proyectados en la pantalla pública de sala de espera (`FrmUsuarioVentana`).
- **Entidad:** Turno / Especialidad / Sala
- **Operación:** Listado / Pantalla Pública
- **Tablas:** `Turnos`, `Especialidades`, `Salas`
- **Forms que lo utilizan:** `FrmUsuarioVentana`
- **Acción:** Carga inicial y auto-refresco de `dgvGeneral`
- **Estado:** `EN USO`
- **Parámetros:** Ninguno.
- **Devuelve:** `Turno`, `Hora`, `Fecha`, `Especialidad`, `Estado`, `Sala`.

```sql
CREATE OR ALTER PROCEDURE sp_ListarTurnosGeneralesPantalla
AS
BEGIN
    SET NOCOUNT ON;

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
END;
GO
```

---

### 5.10 `sp_InsertarTurno`
- **Descripción:** Inserción simple de turno estándar sin prefijos personalizados de especialidad ni triage automático de emergencias. Superado por `sp_CrearTurnoEspecialidad` y `sp_CrearTurnoEmergencia`.
- **Entidad:** Turno
- **Operación:** Alta simple
- **Tablas:** `Turnos`
- **Forms que lo utilizan:** Ninguno actualmente (superado).
- **Acción:** N/A
- **Estado:** `NO UTILIZADO (SUPERADO POR sp_CrearTurnoEspecialidad y sp_CrearTurnoEmergencia)`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@NroOrden` | `NVARCHAR(50)` | IN | Código de orden. |
  | `@TipoTurno` | `NVARCHAR(50)` | IN | Tipo de turno. |
  | `@IdPrioridad` | `INT` | IN | Nivel de prioridad. |
  | `@IdPaciente` | `INT` | IN | ID del paciente. |
  | `@IdEspecialidad` | `INT` | IN | ID de la especialidad. |

```sql
CREATE OR ALTER PROCEDURE sp_InsertarTurno
    @NroOrden NVARCHAR(50),
    @TipoTurno NVARCHAR(50),
    @IdPrioridad INT,
    @IdPaciente INT,
    @IdEspecialidad INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Turnos (NroOrden, Estado, Fecha, TipoTurno, IdPrioridad, IdPaciente, IdEspecialidad, Activo, FechaCreacion)
    VALUES (@NroOrden, 'En Espera', GETDATE(), @TipoTurno, @IdPrioridad, @IdPaciente, @IdEspecialidad, 1, GETDATE());
END;
GO
```

---

## Módulo 6: Atención Médica, Pantalla e Historia Clínica

### 6.1 `sp_ObtenerTurnosEnEspera`
- **Descripción:** Lista los turnos en espera para una especialidad específica, ordenados por triage (`IdPrioridad ASC`) y orden de llegada (`FechaCreacion ASC`).
- **Entidad:** Turno
- **Operación:** Consulta / Cola Médica
- **Tablas:** `Turnos`
- **Forms que lo utilizan:** `FrmListaTurnosAtencion`
- **Acción:** `RefrescarListado`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdEspecialidad` | `INT` | IN | ID de la especialidad. |

```sql
CREATE OR ALTER PROCEDURE sp_ObtenerTurnosEnEspera
    @IdEspecialidad INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM Turnos
    WHERE Activo = 1 
      AND Estado = 'En Espera'
      AND IdEspecialidad = @IdEspecialidad
    ORDER BY IdPrioridad ASC, FechaCreacion ASC;
END;
GO
```

---

### 6.2 `sp_ObtenerListaTurnos`
- **Descripción:** Consulta dinámica para la grilla general de turnos. Si `@IdEspecialidad` o `@Estado` se envían como `NULL`, se omite dicho filtro.
- **Entidad:** Turno / Especialidad / Prioridad
- **Operación:** Listado Dinámico
- **Tablas:** `Turnos`, `Especialidades`, `Prioridades`
- **Forms que lo utilizan:** `FrmListaTurnos`, `FrmListaTurnosAtencion`
- **Acción:** Carga de grillas de seguimiento
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdEspecialidad` | `INT = NULL` | IN | Filtro opcional por especialidad. |
  | `@Estado` | `NVARCHAR(50) = NULL` | IN | Filtro opcional por estado ('En Espera', 'Llamado', 'Atendido'). |
- **Devuelve:** `IdTurno`, `NroOrden`, `Estado`, `TipoTurno`, `Fecha`, `NombreEspecialidad`, `PrioridadTexto`, `IdPrioridad`.

```sql
CREATE OR ALTER PROCEDURE sp_ObtenerListaTurnos
    @IdEspecialidad INT = NULL, 
    @Estado NVARCHAR(50) = NULL  
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.IdTurno,
        t.NroOrden,
        t.Estado,
        t.TipoTurno,
        t.Fecha,
        e.Nombre AS NombreEspecialidad,
        pr.Descripcion AS PrioridadTexto,
        t.IdPrioridad
    FROM Turnos t
    LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
    WHERE t.Activo = 1
      AND (@IdEspecialidad IS NULL OR t.IdEspecialidad = @IdEspecialidad)
      AND (@Estado IS NULL OR t.Estado = @Estado)
    ORDER BY t.IdPrioridad ASC, t.FechaCreacion ASC;
END;
GO
```

---

### 6.3 `sp_ListarTurnosAtencion`
- **Descripción:** Obtiene la cola activa de turnos en espera para atención médica con datos completos del paciente para la ficha clínica.
- **Entidad:** Turno / Paciente / Prioridad / Especialidad
- **Operación:** Cola de Consulta Médica
- **Tablas:** `Turnos`, `Pacientes`, `Prioridades`, `Especialidades`
- **Forms que lo utilizan:** `FrmListaTurnosAtencion`
- **Acción:** Evento `Load` / `CargarTurnosDesdeBD`
- **Estado:** `EN USO`
- **Devuelve:** `IdTurno`, `NroOrden`, `Fecha`, `Estado`, `Especialidad`, `Triage`, `NombrePaciente`, `ApellidoPaciente`, `DniPaciente`, `ObraSocial`, `NombreSala`.

```sql
CREATE OR ALTER PROCEDURE sp_ListarTurnosAtencion
AS
BEGIN
    SET NOCOUNT ON;

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
        ISNULL(s.NombreSala, 'Sin Asignar') AS NombreSala
    FROM Turnos t
    INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
    LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
    LEFT JOIN Salas s ON t.IdSala = s.IdSala
    WHERE t.Activo = 1 
      AND t.Estado = 'En Espera'
    ORDER BY t.IdPrioridad ASC, t.FechaCreacion ASC;
END;
GO
```

---

### 6.4 `sp_LlamarSiguienteTurno`
- **Descripción:** Actualiza el estado del turno a `'Llamado'`, asociándole el consultorio de atención y validando que el turno no haya sido concluido ni cancelado.
- **Entidad:** Turno / Sala
- **Operación:** Llamado de Paciente a Consultorio
- **Tablas:** `Turnos`, `Salas`
- **Forms que lo utilizan:** `FrmListaTurnosAtencion`
- **Acción:** Botón `btnSiguientePaciente`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno llamado. |
  | `@NombreMedico` | `NVARCHAR(100)` | IN | Nombre del profesional médico. |
  | `@SalaAsignada` | `NVARCHAR(100)` | IN | Consultorio/sala donde se atiende. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50080` | *El turno a llamar no existe o se encuentra inactivo en el sistema.* | `@IdTurno` inexistente o con `Activo = 0`. |
  | `50081` | *El turno no puede ser llamado porque su estado actual es incompatible (ya se encuentra Atendido o Cancelado).* | El turno ya está en estado 'Atendido' o 'Cancelado'. |

```sql
CREATE OR ALTER PROCEDURE sp_LlamarSiguienteTurno
    @IdTurno INT,
    @NombreMedico NVARCHAR(100),
    @SalaAsignada NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el turno exista y esté activo
        IF NOT EXISTS (SELECT 1 FROM Turnos WHERE IdTurno = @IdTurno AND Activo = 1)
        BEGIN
            THROW 50080, 'El turno a llamar no existe o se encuentra inactivo en el sistema.', 1;
        END

        -- 2. Validar que su estado permita ser llamado (no debe estar Atendido ni Cancelado)
        DECLARE @EstadoActual NVARCHAR(50);
        SELECT @EstadoActual = Estado FROM Turnos WHERE IdTurno = @IdTurno;

        IF @EstadoActual IN ('Atendido', 'Cancelado')
        BEGIN
            THROW 50081, 'El turno no puede ser llamado porque su estado actual es incompatible (ya se encuentra Atendido o Cancelado).', 1;
        END

        DECLARE @IdSala INT = NULL;
        SELECT @IdSala = IdSala FROM Salas WHERE NombreSala = @SalaAsignada;

        UPDATE Turnos
        SET Estado = 'Llamado',
            IdSala = ISNULL(@IdSala, IdSala),
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
```

---

### 6.5 `sp_IniciarAtencionTurno`
- **Descripción:** Registra el inicio formal de la consulta médica pasando el turno al estado `'En Consulta'` y actualiza atómicamente la sala asignada al estado `'Ocupada'`.
- **Entidad:** Turno / Sala
- **Operación:** Inicio de Consulta Médica
- **Tablas:** `Turnos`, `Salas`
- **Forms que lo utilizan:** `FrmListaTurnosAtencion`
- **Acción:** Botón `btnIniciarAtencion`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno en atención. |
  | `@SalaAsignada` | `NVARCHAR(100) = NULL` | IN | Consultorio físico asignado (opcional). |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50082` | *El turno a iniciar atención no existe o se encuentra inactivo.* | `@IdTurno` no existe o tiene `Activo = 0`. |
  | `50083` | *No se puede iniciar la atención porque el turno ya ha sido finalizado previamente.* | El turno ya está en estado 'Atendido'. |

```sql
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

        IF @SalaAsignada IS NOT NULL AND LTRIM(RTRIM(@SalaAsignada)) <> '' AND @SalaAsignada <> '--'
        BEGIN
            SELECT TOP 1 @IdSala = IdSala FROM Salas WHERE NombreSala = @SalaAsignada;
            
            IF @IdSala IS NULL
            BEGIN
                SELECT TOP 1 @IdSala = IdSala FROM Salas WHERE NombreSala LIKE '%' + @SalaAsignada + '%' OR @SalaAsignada LIKE '%' + NombreSala + '%';
            END
        END

        BEGIN TRANSACTION;

        -- 3. Actualizar el turno a 'En Consulta'
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
```

---

### 6.6 `sp_FinalizarAtencionTurno`
- **Descripción:** Concluye formalmente la consulta médica: pasa el turno a estado `'Atendido'`, libera atómicamente la sala a estado `'Disponible'` y valida la existencia del turno.
- **Entidad:** Turno / Sala
- **Operación:** Cierre de Consulta y Liberación de Recursos
- **Tablas:** `Turnos`, `Salas`
- **Forms que lo utilizan:** `FrmListaTurnosAtencion`
- **Acción:** Botón `btnTerminarAtencion`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno finalizado. |
  | `@Diagnostico` | `NVARCHAR(MAX)` | IN | Diagnóstico o nota de evolución. |
  | `@NombreMedico` | `NVARCHAR(100)` | IN | Médico que finalizó la atención. |
  | `@SalaAsignada` | `NVARCHAR(100)` | IN | Consultorio donde se atendió. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50084` | *El turno a finalizar no existe en el sistema.* | `@IdTurno` no coincide con ningún turno existente. |

```sql
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

        BEGIN TRANSACTION;

        -- 2. Marcar el turno como Atendido
        UPDATE Turnos
        SET Estado = 'Atendido',
            FechaModificacion = GETDATE()
        WHERE IdTurno = @IdTurno;

        -- 3. Liberar la sala a Disponible
        IF @SalaAsignada IS NOT NULL AND LTRIM(RTRIM(@SalaAsignada)) <> '' AND @SalaAsignada <> '--'
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
```

---

### 6.7 `sp_InsertarHistoriaClinica`
- **Descripción:** Registra las notas clínicas, diagnóstico, evolución y recetas farmacológicas emitidas por el médico tratante en la tabla `HistoriasClinicas`. Cuenta con validación de paciente, turno, médico y diagnóstico obligatorio.
- **Entidad:** HistoriaClinica / Paciente / Turno / Usuario
- **Operación:** Alta Médica de Historia Clínica
- **Tablas:** `HistoriasClinicas`, `Pacientes`, `Turnos`, `Usuarios`
- **Forms que lo utilizan:** `FrmListaTurnosAtencion`
- **Acción:** Botón `btnTerminarAtencion`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@TipoTurno` | `NVARCHAR(50)` | IN | 'Consulta' o 'Emergencia'. |
  | `@DiagRapido` | `NVARCHAR(255)` | IN | Diagnóstico presuntivo obligatorio. |
  | `@DescripHistoriaClinica` | `NVARCHAR(MAX)` | IN | Evolución médica. |
  | `@RecetaMedicamentos` | `NVARCHAR(MAX)` | IN | Recetas y posología. |
  | `@IdPaciente` | `INT` | IN | ID del paciente atendido. |
  | `@IdTurno` | `INT` | IN | ID del turno atendido. |
  | `@IdUsuario` | `INT` | IN | ID del profesional médico tratante. |
- **Excepciones y Códigos de Error:**
  | Código | Mensaje al Operador | Condición de Disparo |
  | :--- | :--- | :--- |
  | `50070` | *El paciente vinculado a la historia clínica no existe o se encuentra inactivo.* | `@IdPaciente` no existe o no está activo en `Pacientes`. |
  | `50071` | *El turno asociado a la historia clínica no existe en el sistema.* | `@IdTurno` no existe en `Turnos`. |
  | `50072` | *El profesional médico que firma la atención no existe o se encuentra inactivo.* | `@IdUsuario` no existe o no está activo en `Usuarios`. |
  | `50073` | *El diagnóstico médico es un campo obligatorio para registrar la historia clínica.* | `@DiagRapido` nulo o en blanco. |

```sql
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
```

---

### 6.8 `sp_ObtenerTurnosPantallaPublica`
- **Descripción:** Alimenta los monitores públicos de sala de espera con los turnos recién llamados (sin exponer diagnósticos privados).
- **Entidad:** Turno / Sala / Usuario
- **Operación:** Consulta Pública
- **Tablas:** `Turnos`, `Salas`, `Usuarios`
- **Forms que lo utilizan:** Pantalla pública de sala de espera.
- **Acción:** Consulta periódica (Timer)
- **Estado:** `SIN FORM ASOCIADO / PENDIENTE`
- **Devuelve:** `NroOrden`, `NombreSala`, `MedicoApellido`.

```sql
CREATE OR ALTER PROCEDURE sp_ObtenerTurnosPantallaPublica
AS
BEGIN
    SET NOCOUNT ON;

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
END;
GO
```

---

### 6.9 `sp_FinalizarAtencion`
- **Descripción:** Versión previa para la finalización de atención de turnos y liberación de salas. Superado por `sp_FinalizarAtencionTurno` (que incorpora validación con THROW 50084, registro de evolución diagnóstica y control transaccional).
- **Entidad:** Turno / Sala
- **Operación:** Cierre Consulta (Legado)
- **Tablas:** `Turnos`, `Salas`
- **Forms que lo utilizan:** Ninguno actualmente (superado por `sp_FinalizarAtencionTurno`).
- **Acción:** N/A
- **Estado:** `NO UTILIZADO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno a finalizar. |
  | `@IdSala` | `INT` | IN | ID de la sala a liberar. |

```sql
CREATE OR ALTER PROCEDURE sp_FinalizarAtencion
    @IdTurno INT,
    @IdSala INT
AS
BEGIN
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
END;
GO
```

---

### 6.10 `sp_ObtenerHistoriaClinicaPaciente`
- **Descripción:** Permite consultar los antecedentes clínicos completos del paciente ordenados cronológicamente.
- **Entidad:** HistoriaClinica / Usuario
- **Operación:** Consulta Histórica
- **Tablas:** `HistoriasClinicas`, `Usuarios`
- **Forms que lo utilizan:** Visor de antecedentes médicos.
- **Acción:** N/A
- **Estado:** `SIN FORM ASOCIADO / PENDIENTE DE VISOR HC`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdPaciente` | `INT` | IN | ID del paciente consultado. |
- **Devuelve:** Registros de `HistoriasClinicas` con nombre del profesional médico tratante.

```sql
CREATE OR ALTER PROCEDURE sp_ObtenerHistoriaClinicaPaciente
    @IdPaciente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT hc.*, u.Apellido AS ApellidoMedico, u.Nombre AS NombreMedico
    FROM HistoriasClinicas hc
    INNER JOIN Usuarios u ON hc.IdUsuario = u.IdUsuario
    WHERE hc.IdPaciente = @IdPaciente AND hc.Activo = 1
    ORDER BY hc.Fecha DESC;
END;
GO
```

---

## 7. Scripts de Datos Iniciales (Seeds)

```sql
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
```

---

## 8. Tabla Resumen General de Stored Procedures (dbGestionTurnos)


A continuación se detalla el universo completo de los **47 Stored Procedures** existentes en la base de datos `dbGestionTurnos`, su clasificación funcional y su estado operativo real en el proyecto:

| N° | Stored Procedure | Entidad | Operación | Componente / Form(s) | Acción dentro del Sistema | Estado en Proyecto |
| :- | :--- | :--- | :--- | :--- | :--- | :--- |
| 1 | `sp_ValidarLogin` | Usuario / Rol | Autenticación | `FrmLogin` | Botón `button1` (Iniciar sesión) | `EN USO` |
| 2 | `sp_ListarRoles` | Rol | Listado | `FrmGestionUsuarios2` | Cargar selector desplegable de roles | `EN USO` |
| 3 | `sp_InsertarRol` | Rol | Alta | Ninguno | Script de datos iniciales (`Seeds`) | `NO UTILIZADO` |
| 4 | `sp_ListarUsuarios` | Usuario | Listado / Grilla | `FrmGestionUsuarios2` | Cargar grilla general de usuarios (activos e inactivos opcional) | `EN USO` |
| 5 | `sp_InsertarUsuario` | Usuario | Alta | `FrmGestionUsuarios2` | Botón `btnGuardar` (Registrar nuevo) | `EN USO` |
| 6 | `sp_ModificarUsuario` | Usuario | Modificación | `FrmGestionUsuarios2` | Botón `btnModificar` (Actualizar datos) | `EN USO` |
| 7 | `sp_EliminarUsuario` | Usuario | Baja lógica | `FrmGestionUsuarios2` | Botón `btnEliminar` (Desactivar) | `EN USO` |
| 8 | `sp_ReactivarUsuario` | Usuario | Reactivación / Alta | `FrmGestionUsuarios2` | Botón `btnReactivar` (Re-dar de Alta) | `EN USO` |
| 9 | `sp_ListarPersonalMedico`| Usuario | Selector | `FrmSalasAdmin` | Cargar lista de médicos asignables | `EN USO` |
| 10 | `sp_InsertarSala` | Sala | Alta | `FrmSalasAdmin` | Botón `btnGuardar` (Nueva sala) | `EN USO` |
| 11 | `sp_ModificarSala` | Sala | Modificación | `FrmSalasAdmin` | Botón `btnModificar` y edición interactiva | `EN USO` |
| 12 | `sp_EliminarSala` | Sala | Baja lógica | `FrmSalasAdmin` | Botón `btnEliminar` (Desactivar sala) | `EN USO` |
| 13 | `sp_ReactivarSala` | Sala | Reactivación / Alta | `FrmSalasAdmin` | Botón `btnReactivar` (Re-dar de Alta Sala) | `EN USO` |
| 14 | `sp_ObtenerSalas` | Sala | Listado | `FrmSalasAdmin`, `FrmGestionUsuarios2`, `MisSalas_PM` | Cargar grillas de consultorios (con soporte de inactivas) | `EN USO` |
| 15 | `sp_AsignarSalaMedico` | DetalleSala | Asignación | `FrmGestionUsuarios2`, `FrmSalasAdmin` | Botón `btnGuardar` y `btnModificar` | `EN USO` |
| 16 | `sp_AbrirSala` | Sala | Cambio de Estado | `MisSalas_PM` | Botón `btnAbrirSala` (Pasa a Disponible) | `EN USO` |
| 17 | `sp_CerrarSala` | Sala | Cambio de Estado | `MisSalas_PM` | Botón `btnCerrarSala` (Pasa a En Mantenimiento) | `EN USO` |
| 18 | `sp_ActualizarEstadoSala`| Sala | Actualización | Capa DAL (`SalaDAL`), Capa BLL (`SalaBLL`) | Mantenimiento y actualización genérica de salas | `EN USO` |
| 19 | `sp_ListarEspecialidades`| Especialidad | Listado | `FrmGestionEspecialidades`, `FrmGestionUsuarios2`, `FrmTurnoEspecialidad`, `FrmListaTurnos`, `FrmListaTurnosAtencion` | Carga de combos y grillas (con soporte de inactivas) | `EN USO` |
| 20 | `sp_InsertarEspecialidad`| Especialidad | Alta / Reactivación | `FrmGestionEspecialidades` | Botón `btnGuardar` | `EN USO` |
| 21 | `sp_ModificarEspecialidad`| Especialidad | Modificación | `FrmGestionEspecialidades` | Botón `btnModificar` (Actualizar denominación) | `EN USO` |
| 22 | `sp_EliminarEspecialidad`| Especialidad | Baja lógica | `FrmGestionEspecialidades` | Botón `btnDesactivar` | `EN USO` |
| 23 | `sp_ReactivarEspecialidad`| Especialidad | Reactivación / Alta | `FrmGestionEspecialidades` | Botón `btnReactivar` (Re-dar de Alta Especialidad) | `EN USO` |
| 24 | `sp_AsignarEspecialidadMedico`| MedicoEspecialidad | Asignación | `FrmGestionUsuarios2` | Botón `btnGuardar` y `btnModificar` | `EN USO` |
| 25 | `sp_ObtenerEspecialidadesPorMedico`| MedicoEspecialidad | Consulta por Médico | `FrmGestionUsuarios2`, `FrmListaTurnosAtencion` | Precarga de especialidades vinculadas | `EN USO` |
| 26 | `sp_BuscarPacientePorDNI`| Paciente | Búsqueda | `FrmTurnoEmergencia`, `FrmTurnoEspecialidad` | Búsqueda y autocompletado por DNI | `EN USO` |
| 27 | `sp_GuardarPaciente` | Paciente | Upsert por DNI | `FrmTurnoEmergencia`, `FrmTurnoEspecialidad` | Botón `BtnGenerarTurno` (Upsert con validación) | `EN USO` |
| 28 | `sp_InsertarPaciente` | Paciente | Alta directa | Ninguno | Sustituido por `sp_GuardarPaciente` | `NO UTILIZADO` |
| 29 | `sp_ObtenerSintomas` | Sintoma | Catálogo | `FrmTurnoEmergencia` | Carga dinámica de triage de guardia | `EN USO` |
| 30 | `sp_ObtenerGravedadSintoma`| Sintoma | Consulta Triage | `FrmTurnoEmergencia` | Cálculo del nivel de prioridad según síntoma | `EN USO` |
| 31 | `sp_GuardarTurnoSintoma`| TurnoSintoma | Relación | `FrmTurnoEmergencia` | Botón `BtnGenerarTurno` (Vínculo turno-síntoma) | `EN USO` |
| 32 | `sp_CrearTurnoEmergencia`| Turno | Alta urgencia | `FrmTurnoEmergencia` | Botón `BtnGenerarTurno` (Ticket `E-xxx`) | `EN USO` |
| 33 | `sp_ObtenerHorariosDisponibles`| Turno | Disponibilidad | `FrmTurnoEspecialidad` | Cambio de fecha en calendario | `EN USO` |
| 34 | `sp_CrearTurnoEspecialidad`| Turno | Alta programada | `FrmTurnoEspecialidad` | Botón `BtnGenerarTurno` (Ticket `[Letra]-xxx`) | `EN USO` |
| 35 | `sp_InsertarTurno` | Turno | Alta simple | Ninguno | Sustituido por `sp_CrearTurnoEspecialidad`/`Emergencia` | `NO UTILIZADO` |
| 36 | `sp_ListarTurnosEmergencia`| Turno | Monitor guardia / Pantalla | `FrmListaTurnos`, `FrmUsuarioVentana` | Grilla de emergencias y contadores | `EN USO` |
| 37 | `sp_ListarTurnosGeneralesPantalla`| Turno / Sala | Monitor público general | `FrmUsuarioVentana` | Pantalla pública TV de sala de espera | `EN USO` |
| 38 | `sp_ListarTurnosEspecialidad`| Turno | Consulta filtro | `FrmListaTurnos` | Selección de especialidad | `EN USO` |
| 39 | `sp_ObtenerTurnosEnEspera`| Turno | Cola médica | `FrmListaTurnosAtencion` | Refrescar listado de espera en consultorio | `EN USO` |
| 40 | `sp_ObtenerListaTurnos` | Turno | Listado dinámico | `FrmListaTurnos`, `FrmListaTurnosAtencion` | Carga de grillas de turnos filtradas | `EN USO` |
| 41 | `sp_ListarTurnosAtencion`| Turno / Paciente | Cola atención | `FrmListaTurnosAtencion` | Cargar turnos con ficha del paciente | `EN USO` |
| 42 | `sp_LlamarSiguienteTurno`| Turno / Sala | Llamado médico | `FrmListaTurnosAtencion` | Botón `btnSiguientePaciente` | `EN USO` |
| 43 | `sp_IniciarAtencionTurno`| Turno / Sala | Inicio Consulta | `FrmListaTurnosAtencion` | Botón `btnIniciarAtencion` (Sala a Ocupada) | `EN USO` |
| 44 | `sp_FinalizarAtencionTurno`| Turno / Sala | Cierre Consulta | `FrmListaTurnosAtencion` | Botón `btnTerminarAtencion` (Libera sala) | `EN USO` |
| 45 | `sp_FinalizarAtencion` | Turno / Sala | Cierre Consulta | Ninguno | Versión previa; superada por `sp_FinalizarAtencionTurno` | `NO UTILIZADO` |
| 46 | `sp_InsertarHistoriaClinica`| HistoriaClinica | Alta médica | `FrmListaTurnosAtencion` | Botón `btnTerminarAtencion` (Guardar evolución) | `EN USO` |
| 47 | `sp_ObtenerTurnosPantallaPublica`| Turno / Sala | Monitor público | `FrmUsuarioVentana` | Refresco alternativo de llamados públicos | `EN USO` |
| * | `sp_ObtenerHistoriaClinicaPaciente`| HistoriaClinica | Historial | Módulo Médico (DAL/BLL) | Consulta histórica por paciente (pendiente de visor UI) | `PREPARADO EN BD` |

---

## 9. Resumen por Formulario

### `FrmLogin`
- `sp_ValidarLogin` → Botón "Iniciar Sesión" (`button1_Click`). Valida credenciales activas y obtiene rol del usuario. Incluye ruteo dinámico para el rol `Usuario Ventana` (`IdRol = 4`), abriendo `FrmUsuarioVentana`.

### `FrmGestionUsuarios2` (Formulario Oficial de Gestión de Usuarios y Personal)
- **Estado en el Sistema:** Formulario único y activo ejecutado desde el menú principal de administración (`FrmAdmin`).
- `sp_ListarRoles` → Carga inicial del desplegable de roles (`CargarRolesDesdeBD`). Soporta los 4 roles: Administrador, Personal médico, Recepcionista y Usuario Ventana.
- `sp_ListarEspecialidades` → Carga del listado con selección múltiple (`CargarEspecialidadesDesdeBD`).
- `sp_ObtenerSalas` → Carga del listado con selección múltiple de consultorios (`CargarSalasDesdeBD`).
- `sp_ListarUsuarios` → Carga y refresco de la grilla de usuarios (`CargarUsuariosDesdeBD`). Resuelve productos cartesianos con `DISTINCT`, eliminando duplicaciones de salas/especialidades, y preserva asignaciones para registros inactivos.
- `sp_BuscarPacientePorDNI` → Utilizado internamente para validar que un DNI ingresado para un usuario no esté asignado a un paciente.
- `sp_ObtenerEspecialidadesPorMedico` → Precarga de las especialidades activas vinculadas al médico al seleccionarlo en la grilla o buscarlo por DNI.
- `sp_InsertarUsuario` → Botón "Registrar Nuevo Usuario" (`btnGuardar_Click`). Da de alta el usuario y orquesta asignaciones en `DetallesSalas` y `MedicosEspecialidades`.
- `sp_ModificarUsuario` → Botón "Modificar Datos" (`btnModificar_Click`). Actualiza datos personales y delega reasignación atómica de salas y especialidades con control de excepciones.
- `sp_AsignarEspecialidadMedico` → Invocado atómicamente por `UsuarioDAL` para asociar cada especialidad tildada al usuario.
- `sp_AsignarSalaMedico` → Invocado atómicamente por `UsuarioDAL` para asociar cada sala tildada al usuario.
- `sp_EliminarUsuario` → Botón "Desactivar Usuario" (`btnEliminar_Click`). Ejecuta la baja lógica del usuario protegiendo con inmunidad a `admin@gmail.com` y preservando íntegramente las salas y especialidades asignadas.
- `sp_ReactivarUsuario` → Botón "Re-dar de Alta" (`btnReactivar_Click`). Reactiva lógicamente a un usuario inactivo (`Activo = 1`, `FechaBaja = NULL`), restaurando sus asignaciones y permitiendo actualizar sus datos en la misma acción.

### `FrmSalasAdmin`
- `sp_ListarPersonalMedico` → Carga del listado de médicos asignables a consultorios (`CargarPersonalMedicoDesdeBD`).
- `sp_ObtenerSalas` → Carga y refresco de la grilla de salas (`CargarSalasDesdeBD`). Admite parámetro `@IncluirInactivas` según el estado del selector `chkMostrarInactivas`.
- `sp_InsertarSala` → Botón "Guardar" (`btnGuardar_Click`). Registra la nueva sala con control de duplicados y estado válido.
- `sp_ModificarSala` → Botón "Modificar" (`btnModificar_Click`) y edición directa en celdas de la grilla. Actualiza nombre y estado operativo atómicamente.
- `sp_AsignarSalaMedico` → Botón "Guardar" y "Modificar". Vincula los médicos seleccionados en el CheckedListBox a la sala física.
- `sp_EliminarSala` → Botón "Eliminar" (`btnEliminar_Click`). Ejecuta la baja lógica de la sala y de sus asignaciones en cascada.
- `sp_ReactivarSala` → Botón "Re-dar de Alta Sala" (`btnReactivar_Click`). Reactiva lógicamente salas dadas de baja lógica, restaurando sus vínculos históricos con médicos.

### `FrmGestionEspecialidades`
- `sp_ListarEspecialidades` → Carga y refresco de la grilla de especialidades (`CargarEspecialidadesDesdeBD`). Admite parámetro `@IncluirInactivas` mediante el selector `chkMostrarInactivas`.
- `sp_InsertarEspecialidad` → Botón "Guardar Especialidad" (`btnGuardar_Click`). Registra una nueva especialidad médica en el catálogo.
- `sp_ModificarEspecialidad` → Botón "Modificar Especialidad" (`btnModificar_Click`). Permite actualizar la denominación de la especialidad seleccionada con validaciones de unicidad.
- `sp_EliminarEspecialidad` → Botón "Desactivar Especialidad" (`btnDesactivar_Click`). Ejecuta la baja lógica validando que no posea turnos activos y desactivando asignaciones médicas en cascada.
- `sp_ReactivarEspecialidad` → Botón "Re-dar de Alta" (`btnReactivar_Click`). Restituye lógicamente una especialidad inactiva y reactiva en cascada sus asignaciones profesionales.

### `FrmTurnoEmergencia`
- `sp_ObtenerSintomas` → Carga del catálogo para categorización de triage (`CargarCatalogoSintomas`).
- `sp_ObtenerGravedadSintoma` → Consulta la gravedad del síntoma seleccionado para calcular la prioridad en pantalla.
- `sp_BuscarPacientePorDNI` → Autocompleta datos del paciente al ingresar su número de documento.
- `sp_GuardarPaciente` → Botón "Generar Turno". Registra o recupera el ID del paciente por DNI de forma atómica.
- `sp_CrearTurnoEmergencia` → Botón "Generar Turno". Genera el ticket de guardia correlativo `E-xxx` asociado a Emergencias.
- `sp_GuardarTurnoSintoma` → Botón "Generar Turno". Asocia los síntomas del paciente al turno de atención.

### `FrmTurnoEspecialidad`
- `sp_ListarEspecialidades` → Carga del desplegable de especialidades médicas (`CargarEspecialidades`).
- `sp_ObtenerHorariosDisponibles` → Selección de fecha en calendario (`calFechaTurno_DateChanged`). Lista horarios libres disponibles.
- `sp_BuscarPacientePorDNI` → Autocompleta los datos filiatorios del paciente por su DNI.
- `sp_GuardarPaciente` → Botón "Generar Turno". Registra o actualiza al paciente por DNI.
- `sp_CrearTurnoEspecialidad` → Botón "Generar Turno". Registra el turno programado correlativo (ej. `C-001`).

### `FrmListaTurnos`
- `sp_ListarTurnosEmergencia` → Carga inicial y refresco de la grilla y contadores de guardia (`CargarTurnosEmergencia`).
- `sp_ListarEspecialidades` → Carga del selector de especialidades (`CargarEspecialidades`).
- `sp_ListarTurnosEspecialidad` / `sp_ObtenerListaTurnos` → Selección de especialidad (`cmbEspecialidades_SelectedIndexChanged`).

### `FrmListaTurnosAtencion`
- `sp_ObtenerEspecialidadesPorMedico` / `sp_ListarEspecialidades` → Carga del selector de servicios del médico (`CargarServiciosDelMedico`).
- `sp_ListarTurnosAtencion` / `sp_ObtenerTurnosEnEspera` → Carga de la cola de pacientes en espera (`CargarTurnosDesdeBD`).
- `sp_LlamarSiguienteTurno` → Botón "Siguiente Paciente". Pasa el turno a estado 'Llamado'.
- `sp_IniciarAtencionTurno` → Botón "Iniciar Atención". Pasa el turno a 'En Consulta' y marca el consultorio como 'Ocupada'.
- `sp_FinalizarAtencionTurno` → Botón "Terminar Atención". Pasa el turno a 'Atendido' y libera el consultorio a 'Disponible'.
- `sp_InsertarHistoriaClinica` → Botón "Terminar Atención". Guarda diagnóstico, evolución y recetas farmacológicas.

### `MisSalas_PM`
- `sp_ObtenerSalas` (`@IdUsuario = médico`) → Carga de la grilla de salas asignadas al médico logueado (`CargarMisSalas`).
- `sp_AbrirSala` → Botón "Abrir Sala" (`btnAbrirSala_Click`). Habilita la sala a 'Disponible' controlando que el médico no tenga otra abierta.
- `sp_CerrarSala` → Botón "Cerrar Sala" (`btnCerrarSala_Click`). Pasa la sala a 'Cerrada' controlando que no esté ocupada con atención activa.

### `FrmAdmin`
- Contenedor MDI administrativo. Gestiona navegación a `FrmGestionUsuarios2` (formulario oficial activo), `FrmSalasAdmin` y `FrmGestionEspecialidades`.

### `Pantalla_Principal_PERSONAL_MEDICO` (`FrmPersonalMedico`)
- Contenedor MDI médico. Transfiere el contexto de `_usuarioActual` hacia `MisSalas_PM` y `FrmListaTurnosAtencion`.

### `FrmRecepcionista`
- Contenedor MDI de recepción. Gestiona acceso a `FrmTurnoEmergencia`, `FrmTurnoEspecialidad`, `FrmListaTurnos` y apertura del visor `FrmUsuarioVentana`.

### `FrmUsuarioVentana`
- `sp_ListarTurnosEmergencia` → Carga inicial y auto-refresco periódico de la grilla de emergencias/triage (`dgvEmergencias`).
- `sp_ListarTurnosGeneralesPantalla` → Carga inicial y auto-refresco periódico de la grilla de consultorios externos (`dgvGeneral`).
- `sp_ObtenerTurnosPantallaPublica` → Alternativa de consulta para monitor público de llamados activos.
- Rol asignado: `Usuario Ventana` (`IdRol = 4`).

---

## 9.1 Registro de Correcciones y Ajustes en Flujos de Modificación (Salas, Usuarios y Especialidades)

A partir de la revisión técnica del flujo de modificación de entidades se detectaron y corrigieron los siguientes comportamientos en SQL Server y la Capa DAL/UI:

### 1. Desfase de Identificador de Rol en `sp_AsignarSalaMedico` (Error 50061)
- **Problema detectado:** `sp_AsignarSalaMedico` contenía una condición fija `AND IdRol = 2`. Sin embargo, en el esquema real de `dbGestionTurnos`, `IdRol = 1` corresponde a `Personal médico` y `IdRol = 2` a `Recepcionista`. Al intentar modificar una sala asignando médicos (`FrmSalasAdmin`) o al modificar un usuario médico reasignándole salas (`FrmGestionUsuarios2`), la base de datos abortaba la operación arrojando:
  > `THROW 50061, 'El usuario asignado no existe, está inactivo o no pertenece al rol de Personal Médico.', 1;`
- **Solución implementada:** Se modificó `sp_AsignarSalaMedico` vinculando dinámicamente con la tabla `Roles` mediante `INNER JOIN Roles r ON u.IdRol = r.IdRol`, permitiendo `(u.IdRol = 1 OR r.Descripcion LIKE '%Médic%' OR r.Descripcion LIKE '%Medic%')`. Esto desacopla el procedimiento de IDs estáticos y garantiza la asignación para cualquier profesional médico.
- **Manejo de borrado lógico:** Se incorporó reactivación automática de registros previos: si en `DetallesSalas` existía una vinculación con `Activo = 0` (por haber sido desasignada previamente), se reactiva con `Activo = 1, FechaBaja = NULL` y se actualiza su descripción, impidiendo duplicidad de registros y errores de clave.

### 2. Validación Robusta en `sp_AsignarEspecialidadMedico` (Error 50062)
- **Mejora aplicada:** Se actualizó la validación del usuario médico en `sp_AsignarEspecialidadMedico` para verificar que el usuario esté activo y pertenezca efectivamente al rol médico (`u.IdRol = 1 OR r.Descripcion LIKE '%Médic%'`), protegiendo la integridad clínica de la tabla `MedicosEspecialidades`. Además, reactiva asignaciones inactivas previas ante reasignaciones consecutivas en `UsuarioDAL`.

### 3. Modificación Atómica en `sp_ModificarSala` (Soporte de `@EstadoSala`)
- **Mejora aplicada:** Se incorporó el parámetro opcional `@EstadoSala NVARCHAR(50) = NULL` con validación de estados permitidos (`Disponible`, `Libre`, `Ocupada`, `En Mantenimiento`, `Cerrada` - error `50042`). De este modo, la Capa DAL (`SalaDAL.ModificarSala`) actualiza tanto el nombre como el estado operativo en una única sentencia atómica contra SQL Server, manteniendo compatibilidad total hacia atrás con clientes que envían 2 parámetros.

### 4. Salvaguardas en `sp_ModificarUsuario` (Roles y DNI)
- **Mejora aplicada:** Se garantizó que la comprobación de rol valide que el rol exista y esté activo (`Roles.Activo = 1`). Se protegió la actualización con `CASE WHEN @IdRol IS NOT NULL AND @IdRol > 0 THEN @IdRol ELSE IdRol END` para impedir que valores nulos o cero corrompan la clave foránea del usuario.

### 5. Resiliencia en Interfaz de Usuario (`FrmGestionUsuarios2.cs`)
- **Mejora aplicada:** El método `EsPersonalMedico()` se generalizó para evaluar coincidencias insensibles a mayúsculas y acentos (`Contains("médic") || Contains("medic")`), asegurando que la sección médica (matrícula, especialidades y consultorios) se despliegue y valide con exactitud ante cualquier variación de catálogo. Además, se reubicó la barra de búsqueda en `pnlPaso1Dni` en sustitución de la etiqueta estática de espera, optimizando el espacio visual de los botones de acción en `pnlAcciones`.

### 6. Habilitación de Modificación y Re-dar de Alta en Salas y Especialidades
- **Mejora aplicada:**
  - En **Salas (`FrmSalasAdmin`)**: Se añadió el botón `btnReactivar` y el filtro `chkMostrarInactivas` para listar salas inactivas con estilo visual diferenciado tenue y reactivarlas mediante `sp_ReactivarSala`. Se habilitó la modificación completa mediante `sp_ModificarSala` y `ReasignarMedicosASala`.
  - En **Especialidades (`FrmGestionEspecialidades`)**: Se activó `sp_ModificarEspecialidad` vinculado al nuevo botón `btnModificar`, se incorporó `sp_ReactivarEspecialidad` con el botón `btnReactivar`, se agregó la columna "Estado" y el filtro `chkMostrarInactivas` para auditar e interactuar con especialidades inactivas.

---

## 10. Procedimientos Almacenados No Utilizados en el Proyecto

A continuación se detallan exhaustivamente los **4 procedimientos almacenados** que se encuentran creados en la base de datos `dbGestionTurnos` pero que **NO son utilizados hasta el momento por el código del proyecto** (ni en la capa de interfaz WinForms ni en la lógica de negocio):

### 1. `sp_FinalizarAtencion`
- **Firma en Base de Datos:** `@IdTurno INT, @IdSala INT`
- **Motivo de no uso:** Es una implementación temprana y elemental que únicamente modificaba el estado del turno y requería el identificador numérico de la sala. 
- **Procedimiento que lo sustituye:** Fue completamente reemplazado en la aplicación por **`sp_FinalizarAtencionTurno`** (`@IdTurno, @Diagnostico, @NombreMedico, @SalaAsignada`), el cual interactúa directamente con el flujo de cierre clínico, libera atómicamente el consultorio por nombre textual y valida la existencia del turno con código de error `50084`.

### 2. `sp_InsertarPaciente`
- **Firma en Base de Datos:** `@Nombre NVARCHAR(100), @Apellido NVARCHAR(100), @Dni NVARCHAR(20), @ObraSocial NVARCHAR(100)`
- **Motivo de no uso:** Realiza una inserción simple y ciega (`INSERT INTO Pacientes...`) sin verificar si el DNI ya pertenecía a un paciente existente, lo que generaba registros duplicados con historias clínicas fragmentadas ante visitas sucesivas.
- **Procedimiento que lo sustituye:** Fue reemplazado por **`sp_GuardarPaciente`**, el cual implementa una lógica de *upsert* inteligente: busca por DNI, actualiza la cobertura si ya existe, valida que el DNI no pertenezca a un usuario del sistema (código `50010`), o inserta si es un paciente nuevo.

### 3. `sp_InsertarRol`
- **Firma en Base de Datos:** `@Descripcion NVARCHAR(50)`
- **Motivo de no uso:** Es un procedimiento utilitario de catálogo. El sistema no provee una interfaz gráfica de usuario para el alta o modificación dinámica de roles, ya que los cuatro roles de la clínica (`Administrador`, `Personal médico`, `Recepcionista`, `Usuario Ventana`) son estructuras estáticas predeterminadas del negocio.
- **Procedimiento que lo sustituye:** Se inicializan mediante el script de datos iniciales (`Seeds`) en la sección 7.

### 4. `sp_InsertarTurno`
- **Firma en Base de Datos:** `@NroOrden NVARCHAR(50), @TipoTurno NVARCHAR(50), @IdPrioridad INT, @IdPaciente INT, @IdEspecialidad INT`
- **Motivo de no uso:** Procedimiento genérico temprano que requería que el código de orden fuera calculado o provisto externamente y no contemplaba el flujo diferenciado de guardia vs. consultorio programado.
- **Procedimiento que lo sustituye:** Fue reemplazado por dos procedimientos especializados:
  - **`sp_CrearTurnoEspecialidad`**: Genera automáticamente el código con la inicial de la especialidad (ej. `C-001`, `P-001`), asocia fecha y franja horaria y valida paciente y especialidad (códigos `50001` y `50003`).
  - **`sp_CrearTurnoEmergencia`**: Genera automáticamente el código de guardia `E-001`, asigna el nivel de triage y vincula el servicio de emergencias (códigos `50001` y `50002`).

*(Nota complementaria: el procedimiento `sp_ObtenerHistoriaClinicaPaciente` se encuentra definido e implementado en la base de datos y referenciado en las capas de acceso a datos, pero la pantalla de visor de antecedentes médicos aún no ha sido incorporada al frontend WinForms).*

*(Nota complementaria: el procedimiento `sp_ObtenerHistoriaClinicaPaciente` se encuentra definido e implementado en la base de datos y referenciado en las capas de acceso a datos, pero la pantalla de visor de antecedentes médicos aún no ha sido incorporada al frontend WinForms).*

