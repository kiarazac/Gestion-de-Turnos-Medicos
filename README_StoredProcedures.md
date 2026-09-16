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

```sql
CREATE OR ALTER PROCEDURE sp_ValidarLogin
    @Correo NVARCHAR(150),
    @Contrasena NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

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
- **Descripción:** Obtiene el listado completo de usuarios activos del sistema para la grilla de administración, concatenando sus especialidades médicas y salas asignadas.
- **Entidad:** Usuario
- **Operación:** Listado / Grilla
- **Tablas:** `Usuarios`, `Roles`, `MedicosEspecialidades`, `Especialidades`, `DetallesSalas`, `Salas`
- **Forms que lo utilizan:** `FrmGestionUsuarios`
- **Acción:** Evento `Load` / `CargarUsuariosDesdeBD`
- **Estado:** `PENDIENTE DE IMPLEMENTACIÓN`
- **Parámetros:** Ninguno.
- **Devuelve:** `IdUsuario`, `Nombre`, `Apellido`, `Correo`, `Dni`, `Telefono`, `Rol`, `NroMatricula`, `Especialidades`, `Salas`.

```sql
CREATE OR ALTER PROCEDURE sp_ListarUsuarios
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
        ISNULL(STRING_AGG(e.Nombre, ', '), '') AS Especialidades,
        ISNULL(STRING_AGG(s.NombreSala, ', '), '') AS Salas
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    LEFT JOIN MedicosEspecialidades me ON u.IdUsuario = me.IdUsuario AND me.Activo = 1
    LEFT JOIN Especialidades e ON me.IdEspecialidad = e.IdEspecialidad AND e.Activo = 1
    LEFT JOIN DetallesSalas ds ON u.IdUsuario = ds.IdUsuario AND ds.Activo = 1
    LEFT JOIN Salas s ON ds.IdSala = s.IdSala AND s.Activo = 1
    WHERE u.Activo = 1
    GROUP BY u.IdUsuario, u.Nombre, u.Apellido, u.Correo, u.Dni, u.Telefono, r.Descripcion, u.NroMatricula
    ORDER BY u.Apellido, u.Nombre;
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
- **Devuelve:** `IdNuevoUsuario` (`SCOPE_IDENTITY()`).

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

    INSERT INTO Usuarios (Nombre, Apellido, Correo, Contrasena, Dni, Telefono, NroMatricula, IdRol, Activo, FechaCreacion)
    VALUES (@Nombre, @Apellido, @Correo, @Contrasena, @Dni, @Telefono, @NroMatricula, @IdRol, 1, GETDATE());

    SELECT SCOPE_IDENTITY() AS IdNuevoUsuario;
END;
GO
```

---

### 1.6 `sp_ModificarUsuario`
- **Descripción:** Actualiza los datos de filiación, contacto, identificación, matrícula profesional y rol de un usuario existente. Si se especifica una sala (`@IdSala`), gestiona de forma atómica y con borrado lógico la asignación del médico en la tabla `DetallesSalas`.
- **Entidad:** Usuario / DetalleSala
- **Operación:** Modificación / Reasignación de Sala
- **Tablas:** `Usuarios`, `DetallesSalas`
- **Forms que lo utilizan:** `FrmGestionUsuarios`
- **Acción:** Botón `btnModificar` y edición directa de celda en DataGridView (`dgvPersonal_CellEndEdit`)
- **Estado:** `EN USO`
- **Parámetros:**
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
  | `@IdSala` | `INT` | IN | Nueva sala asignada (opcional: NULL = no tocar salas, 0 = desasignar, >0 = reasignar sala). |
  | `@DescripcionAtencion` | `NVARCHAR(255)` | IN | Observaciones o notas de atención en la sala (opcional). |

```sql
CREATE OR ALTER PROCEDURE sp_ModificarUsuario
    @IdUsuario INT,
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Correo NVARCHAR(150),
    @Telefono NVARCHAR(20),
    @Dni NVARCHAR(20) = NULL,
    @NroMatricula NVARCHAR(50) = NULL,
    @IdRol INT = NULL,
    @IdSala INT = NULL,
    @DescripcionAtencion NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Usamos un bloque TRY...CATCH para garantizar atomicidad e integridad referencial
    BEGIN TRY
        -- Iniciamos una transacción explícita: o se aplican todos los cambios o ninguno
        BEGIN TRANSACTION;

        -- 1. Actualizamos los datos personales, contacto, identificación y matrícula en la tabla Usuarios
        UPDATE Usuarios
        SET Nombre = @Nombre,
            Apellido = @Apellido,
            Correo = @Correo,
            Telefono = @Telefono,
            Dni = ISNULL(@Dni, Dni),
            NroMatricula = @NroMatricula,
            IdRol = ISNULL(@IdRol, IdRol),
            FechaModificacion = GETDATE()
        WHERE IdUsuario = @IdUsuario;

        -- 2. Gestión de salas en la tabla DetallesSalas (solo si @IdSala no es NULL)
        -- Si @IdSala es NULL, no se modifican las salas existentes del usuario
        IF (@IdSala IS NOT NULL)
        BEGIN
            -- Desactivamos lógicamente cualquier asignación previa activa para este usuario
            UPDATE DetallesSalas
            SET Activo = 0,
                FechaBaja = GETDATE(),
                FechaModificacion = GETDATE()
            WHERE IdUsuario = @IdUsuario 
              AND Activo = 1;

            -- Si se envió un ID de sala mayor a 0, insertamos la nueva asignación activa
            -- Si es 0, simplemente queda desasignado de cualquier sala
            -- Se utiliza ISNULL ya que la columna DescripcionAtencion no admite valores NULL
            IF (@IdSala > 0)
            BEGIN
                INSERT INTO DetallesSalas (IdSala, IdUsuario, DescripcionAtencion, FechaCreacion, Activo)
                VALUES (@IdSala, @IdUsuario, ISNULL(@DescripcionAtencion, ''), GETDATE(), 1);
            END
        END

        -- Si no ocurrieron errores, confirmamos todos los cambios en la base de datos
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Si ocurre cualquier falla, revertimos los cambios para evitar datos inconsistentes
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Relanzamos el error hacia la capa C#
        THROW;
    END CATCH
END;
GO
```

---

### 1.7 `sp_EliminarUsuario` (Baja Lógica)
- **Descripción:** Realiza el borrado lógico (`Activo = 0`) del usuario registrando la fecha de baja.
- **Entidad:** Usuario
- **Operación:** Baja lógica
- **Tablas:** `Usuarios`
- **Forms que lo utilizan:** `FrmGestionUsuarios`
- **Acción:** Botón `btnEliminar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT` | IN | ID del usuario a desactivar. |

```sql
CREATE OR ALTER PROCEDURE sp_EliminarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuarios
    SET Activo = 0,
        FechaBaja = GETDATE()
    WHERE IdUsuario = @IdUsuario;
END;
GO
```

---

### 1.8 `sp_ListarPersonalMedico`
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
- **Descripción:** Registra un nuevo consultorio o sala física en el catálogo.
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
  | `@EstadoSala` | `NVARCHAR(50)` | IN | Estado inicial ('Disponible', 'Cerrada', etc.). |
- **Devuelve:** `IdNuevaSala` (`SCOPE_IDENTITY()`).

```sql
CREATE OR ALTER PROCEDURE sp_InsertarSala
    @NombreSala NVARCHAR(100),
    @EstadoSala NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Salas (NombreSala, EstadoSala, Activo, FechaCreacion)
    VALUES (@NombreSala, @EstadoSala, 1, GETDATE());

    SELECT SCOPE_IDENTITY() AS IdNuevaSala;
END;
GO
```

---

### 2.2 `sp_ModificarSala`
- **Descripción:** Actualiza el nombre identificatorio de la sala.
- **Entidad:** Sala
- **Operación:** Modificación
- **Tablas:** `Salas`
- **Forms que lo utilizan:** Ninguno actualmente.
- **Acción:** N/A
- **Estado:** `NO UTILIZADO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala. |
  | `@NombreSala` | `NVARCHAR(100)` | IN | Nuevo nombre. |

```sql
CREATE OR ALTER PROCEDURE sp_ModificarSala
    @IdSala INT,
    @NombreSala NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Salas
    SET NombreSala = @NombreSala,
        FechaModificacion = GETDATE()
    WHERE IdSala = @IdSala;
END;
GO
```

---

### 2.3 `sp_EliminarSala` (Baja Lógica)
- **Descripción:** Realiza la baja lógica de la sala (`Activo = 0`).
- **Entidad:** Sala
- **Operación:** Baja lógica
- **Tablas:** `Salas`
- **Forms que lo utilizan:** `FrmSalasAdmin`
- **Acción:** Botón `btnEliminar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala a desactivar. |

```sql
CREATE OR ALTER PROCEDURE sp_EliminarSala
    @IdSala INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Salas
    SET Activo = 0,
        FechaBaja = GETDATE()
    WHERE IdSala = @IdSala;
END;
GO
```

---

### 2.4 `sp_ObtenerSalas`
- **Descripción:** Lista las salas activas. Si `@IdUsuario` es `NULL`, devuelve todas las salas (vista administrativa); si se provee un médico, retorna exclusivamente las salas asignadas a su perfil.
- **Entidad:** Sala / DetalleSala
- **Operación:** Listado / Consulta
- **Tablas:** `Salas`, `DetallesSalas`, `Usuarios`
- **Forms que lo utilizan:** `FrmSalasAdmin`, `FrmGestionUsuarios`, `MisSalas_PM`
- **Acción:** Carga de grilla de salas (`CargarSalasDesdeBD`, `CargarMisSalas`)
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT = NULL` | IN | ID opcional del médico. |
- **Devuelve:** `IdSala`, `NombreSala`, `EstadoSala`, `IdUsuario`, `NombreMedico`, `ApellidoMedico`, `DescripcionAtencion`.

```sql
CREATE OR ALTER PROCEDURE sp_ObtenerSalas
    @IdUsuario INT = NULL
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
        ISNULL(ds.DescripcionAtencion, '') AS DescripcionAtencion
    FROM Salas s
    LEFT JOIN DetallesSalas ds ON s.IdSala = ds.IdSala AND ds.Activo = 1
    LEFT JOIN Usuarios u ON ds.IdUsuario = u.IdUsuario AND u.Activo = 1
    WHERE s.Activo = 1
      AND (@IdUsuario IS NULL OR ds.IdUsuario = @IdUsuario)
    ORDER BY s.NombreSala ASC;
END;
GO
```

---

### 2.5 `sp_AsignarSalaMedico`
- **Descripción:** Vincula un médico a un consultorio en la tabla `DetallesSalas` con observaciones de atención.
- **Entidad:** DetalleSala
- **Operación:** Asignación
- **Tablas:** `DetallesSalas`
- **Forms que lo utilizan:** `FrmGestionUsuarios`, `FrmSalasAdmin`
- **Acción:** Botón `btnGuardar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala asignada. |
  | `@IdUsuario` | `INT` | IN | ID del médico asignado. |
  | `@DescripcionAtencion` | `NVARCHAR(255)` | IN | Notas u horario de atención. |

```sql
CREATE OR ALTER PROCEDURE sp_AsignarSalaMedico
    @IdSala INT,
    @IdUsuario INT,
    @DescripcionAtencion NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO DetallesSalas (IdSala, IdUsuario, DescripcionAtencion, FechaCreacion, Activo)
    VALUES (@IdSala, @IdUsuario, @DescripcionAtencion, GETDATE(), 1);
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
- **Descripción:** Cambia el estado de la sala a `'Cerrada'` / `'En Mantenimiento'`. Bloquea si la sala está en consulta con un paciente (`'Ocupada'`).
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
    SET EstadoSala = 'En Mantenimiento',
        FechaModificacion = GETDATE()
    WHERE IdSala = @IdSala AND Activo = 1;
END;
GO
```

---

### 2.8 `sp_ActualizarEstadoSala`
- **Descripción:** Actualiza genéricamente el estado operativo de una sala (`Disponible`, `Ocupada`, `En Mantenimiento`, `Cerrada`).
- **Entidad:** Sala
- **Operación:** Actualización
- **Tablas:** `Salas`
- **Forms que lo utilizan:** `FrmListaTurnosAtencion`
- **Acción:** Inicio y finalización de consulta médica
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala. |
  | `@NuevoEstado` | `NVARCHAR(50)` | IN | Nuevo estado asignado. |

```sql
CREATE OR ALTER PROCEDURE sp_ActualizarEstadoSala
    @IdSala INT,
    @NuevoEstado NVARCHAR(50) 
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Salas
    SET EstadoSala = @NuevoEstado,
        FechaModificacion = GETDATE()
    WHERE IdSala = @IdSala AND Activo = 1;
END;
GO
```

---

## Módulo 3: Especialidades Médicas

### 3.1 `sp_ListarEspecialidades` / `sp_ObtenerEspecialidades`
- **Descripción:** Obtiene la lista completa de especialidades médicas activas para grillas de administración y selectores de turnos.
- **Entidad:** Especialidad
- **Operación:** Listado
- **Tablas:** `Especialidades`
- **Forms que lo utilizan:** `FrmGestionEspecialidades`, `FrmGestionUsuarios`, `FrmTurnoEspecialidad`, `FrmListaTurnos`, `FrmListaTurnosAtencion`
- **Acción:** Carga de catálogos y selectores
- **Estado:** `PENDIENTE DE IMPLEMENTACIÓN`
- **Parámetros:** Ninguno.
- **Devuelve:** `IdEspecialidad`, `Nombre`.

```sql
CREATE OR ALTER PROCEDURE sp_ListarEspecialidades
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdEspecialidad, Nombre
    FROM Especialidades
    WHERE Activo = 1
    ORDER BY Nombre ASC;
END;
GO
```

---

### 3.2 `sp_InsertarEspecialidad`
- **Descripción:** Registra una nueva especialidad médica en el catálogo.
- **Entidad:** Especialidad
- **Operación:** Alta
- **Tablas:** `Especialidades`
- **Forms que lo utilizan:** `FrmGestionEspecialidades`
- **Acción:** Botón `btnGuardar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Nombre` | `NVARCHAR(100)` | IN | Nombre de la especialidad. |

```sql
CREATE OR ALTER PROCEDURE sp_InsertarEspecialidad
    @Nombre NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Especialidades (Nombre, Activo, FechaCreacion)
    VALUES (@Nombre, 1, GETDATE());
END;
GO
```

---

### 3.3 `sp_ModificarEspecialidad`
- **Descripción:** Modifica el nombre de una especialidad existente.
- **Entidad:** Especialidad
- **Operación:** Modificación
- **Tablas:** `Especialidades`
- **Forms que lo utilizan:** Ninguno actualmente.
- **Acción:** N/A
- **Estado:** `NO UTILIZADO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdEspecialidad` | `INT` | IN | ID de la especialidad. |
  | `@Nombre` | `NVARCHAR(100)` | IN | Nuevo nombre. |

```sql
CREATE OR ALTER PROCEDURE sp_ModificarEspecialidad
    @IdEspecialidad INT,
    @Nombre NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Especialidades
    SET Nombre = @Nombre,
        FechaModificacion = GETDATE()
    WHERE IdEspecialidad = @IdEspecialidad;
END;
GO
```

---

### 3.4 `sp_EliminarEspecialidad` (Baja Lógica)
- **Descripción:** Realiza el borrado lógico de una especialidad médica (`Activo = 0`).
- **Entidad:** Especialidad
- **Operación:** Baja lógica
- **Tablas:** `Especialidades`
- **Forms que lo utilizan:** `FrmGestionEspecialidades`
- **Acción:** Botón `btnDesactivar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdEspecialidad` | `INT` | IN | ID de la especialidad a desactivar. |

```sql
CREATE OR ALTER PROCEDURE sp_EliminarEspecialidad
    @IdEspecialidad INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Especialidades
    SET Activo = 0,
        FechaBaja = GETDATE()
    WHERE IdEspecialidad = @IdEspecialidad;
END;
GO
```

---

### 3.5 `sp_AsignarEspecialidadMedico`
- **Descripción:** Asocia una especialidad a un profesional médico en la tabla intermedia `MedicosEspecialidades`.
- **Entidad:** MedicoEspecialidad
- **Operación:** Asignación
- **Tablas:** `MedicosEspecialidades`
- **Forms que lo utilizan:** `FrmGestionUsuarios`
- **Acción:** Botón `btnGuardar`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT` | IN | ID del médico. |
  | `@IdEspecialidad` | `INT` | IN | ID de la especialidad vinculada. |

```sql
CREATE OR ALTER PROCEDURE sp_AsignarEspecialidadMedico
    @IdUsuario INT,
    @IdEspecialidad INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO MedicosEspecialidades (IdUsuario, IdEspecialidad, Activo, FechaCreacion)
    VALUES (@IdUsuario, @IdEspecialidad, 1, GETDATE());
END;
GO
```

---

## Módulo 4: Pacientes

### 4.1 `sp_InsertarPaciente`
- **Descripción:** Registra un nuevo paciente en la tabla `Pacientes` y devuelve el ID recién generado.
- **Entidad:** Paciente
- **Operación:** Alta
- **Tablas:** `Pacientes`
- **Forms que lo utilizan:** `FrmTurnoEmergencia`, `FrmTurnoEspecialidad`
- **Acción:** Botón `BtnGenerarTurno`
- **Estado:** `EN USO`
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

### 4.2 `sp_GuardarPaciente` (Upsert Inteligente)
- **Descripción:** Busca al paciente por DNI. Si ya existe en el sistema, actualiza su cobertura médica y retorna su `IdPaciente`. Si no existe, lo inserta y retorna el nuevo ID.
- **Entidad:** Paciente
- **Operación:** Búsqueda / Alta / Actualización
- **Tablas:** `Pacientes`
- **Forms que lo utilizan:** `FrmTurnoEmergencia`, `FrmTurnoEspecialidad`
- **Acción:** Botón `BtnGenerarTurno`
- **Estado:** `PENDIENTE DE IMPLEMENTACIÓN`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Nombre` | `NVARCHAR(100)` | IN | Nombre del paciente. |
  | `@Apellido` | `NVARCHAR(100)` | IN | Apellido del paciente. |
  | `@Dni` | `NVARCHAR(20)` | IN | Documento de identidad. |
  | `@ObraSocial` | `NVARCHAR(100)` | IN | Cobertura médica. |
- **Devuelve:** 1 fila: `IdPaciente`.

```sql
CREATE OR ALTER PROCEDURE sp_GuardarPaciente
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni NVARCHAR(20),
    @ObraSocial NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

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
END;
GO
```

---

## Módulo 5: Turnos y Triage de Guardia

### 5.1 `sp_InsertarTurno`
- **Descripción:** Registra un turno estándar programado en recepción en estado `'En Espera'`.
- **Entidad:** Turno
- **Operación:** Alta
- **Tablas:** `Turnos`
- **Forms que lo utilizan:** `FrmTurnoEspecialidad`
- **Acción:** Botón `BtnGenerarTurno`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@NroOrden` | `NVARCHAR(50)` | IN | Código de orden (ej. 'T-001'). |
  | `@TipoTurno` | `NVARCHAR(50)` | IN | Tipo ('Consulta', 'Estudio'). |
  | `@IdPrioridad` | `INT` | IN | Prioridad (1 = Alta, 2 = Media, 3 = Baja). |
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

### 5.2 `sp_ObtenerSintomas`
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

### 5.3 `sp_RegistrarTurnoSintoma`
- **Descripción:** Asocia un síntoma del paciente al turno generado en la tabla `TurnoSintomas`.
- **Entidad:** TurnoSintoma
- **Operación:** Alta / Relación
- **Tablas:** `TurnoSintomas`
- **Forms que lo utilizan:** `FrmTurnoEmergencia`
- **Acción:** Botón `BtnGenerarTurno`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno generado. |
  | `@IdSintoma` | `INT` | IN | ID del síntoma. |
  | `@EstadoActual` | `NVARCHAR(100)` | IN | Observaciones clínicas iniciales. |

```sql
CREATE OR ALTER PROCEDURE sp_RegistrarTurnoSintoma
    @IdTurno INT,
    @IdSintoma INT,
    @EstadoActual NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO TurnoSintomas (IdTurno, IdSintoma, EstadoActual, FechaCreacion, Activo)
    VALUES (@IdTurno, @IdSintoma, @EstadoActual, GETDATE(), 1);
END;
GO
```

---

### 5.4 `sp_RegistrarTurnoEmergencia`
- **Descripción:** Motor de urgencias: calcula automáticamente la prioridad (1=Alta, 2=Media, 3=Baja) según la gravedad del síntoma, crea el turno de guardia e inserta la relación en `TurnoSintomas`.
- **Entidad:** Turno / Sintoma / TurnoSintoma
- **Operación:** Alta con Triage Automático
- **Tablas:** `Sintomas`, `Turnos`, `TurnoSintomas`
- **Forms que lo utilizan:** `FrmTurnoEmergencia`
- **Acción:** Botón `BtnGenerarTurno`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@NroOrden` | `NVARCHAR(50)` | IN | Número de ticket visible (ej. 'E-001'). |
  | `@IdPaciente` | `INT` | IN | ID del paciente asistido. |
  | `@IdSintoma` | `INT` | IN | ID del síntoma que motiva el ingreso. |
  | `@EstadoActual` | `NVARCHAR(255)` | IN | Descripción de estado inicial. |
- **Devuelve:** `IdNuevoTurno` (`SCOPE_IDENTITY()`).

```sql
CREATE OR ALTER PROCEDURE sp_RegistrarTurnoEmergencia
    @NroOrden NVARCHAR(50), 
    @IdPaciente INT,
    @IdSintoma INT,
    @EstadoActual NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdPrioridad INT;
    DECLARE @Gravedad NVARCHAR(50);
    DECLARE @IdTurnoGenerado INT;

    SELECT @Gravedad = Gravedad 
    FROM Sintomas 
    WHERE IdSintoma = @IdSintoma;

    IF @Gravedad = 'Alta' SET @IdPrioridad = 1;
    ELSE IF @Gravedad = 'Media' SET @IdPrioridad = 2;
    ELSE SET @IdPrioridad = 3;

    INSERT INTO Turnos (NroOrden, Estado, Fecha, TipoTurno, IdPrioridad, IdPaciente, Activo, FechaCreacion)
    VALUES (@NroOrden, 'En Espera', GETDATE(), 'Emergencia', @IdPrioridad, @IdPaciente, 1, GETDATE());

    SET @IdTurnoGenerado = SCOPE_IDENTITY();

    INSERT INTO TurnoSintomas (IdTurno, IdSintoma, EstadoActual, FechaCreacion, Activo)
    VALUES (@IdTurnoGenerado, @IdSintoma, @EstadoActual, GETDATE(), 1);
    
    SELECT @IdTurnoGenerado AS IdNuevoTurno;
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
- **Estado:** `PENDIENTE DE IMPLEMENTACIÓN`
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
- **Descripción:** Registra un turno programado vinculando paciente, especialidad, fecha y horario asignado, devolviendo el `IdTurno` y código correlativo.
- **Entidad:** Turno / Especialidad
- **Operación:** Alta Programada
- **Tablas:** `Turnos`, `Especialidades`
- **Forms que lo utilizan:** `FrmTurnoEspecialidad`
- **Acción:** Botón `BtnGenerarTurno`
- **Estado:** `PENDIENTE DE IMPLEMENTACIÓN`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdPaciente` | `INT` | IN | ID del paciente. |
  | `@NombreEspecialidad` | `NVARCHAR(100)` | IN | Especialidad elegida. |
  | `@Fecha` | `DATE` | IN | Fecha del turno. |
  | `@Horario` | `NVARCHAR(10)` | IN | Horario asignado (ej. '10:30'). |
  | `@Estado` | `NVARCHAR(50)` | IN | Estado inicial ('En Espera'). |
- **Devuelve:** `IdNuevoTurno`, `NroOrden`.

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

    DECLARE @IdEspecialidad INT;
    DECLARE @IdTurno INT;
    DECLARE @NroOrden NVARCHAR(20);

    SELECT @IdEspecialidad = IdEspecialidad 
    FROM Especialidades 
    WHERE Nombre = @NombreEspecialidad AND Activo = 1;

    INSERT INTO Turnos (NroOrden, Estado, Fecha, Horario, TipoTurno, IdPrioridad, IdPaciente, IdEspecialidad, Activo, FechaCreacion)
    VALUES ('TEMP', @Estado, @Fecha, CAST(@Horario AS TIME), 'Consulta', 3, @IdPaciente, @IdEspecialidad, 1, GETDATE());

    SET @IdTurno = SCOPE_IDENTITY();
    SET @NroOrden = CONCAT('T-', RIGHT('000' + CAST(@IdTurno AS VARCHAR(10)), 3));

    UPDATE Turnos SET NroOrden = @NroOrden WHERE IdTurno = @IdTurno;

    SELECT @IdTurno AS IdNuevoTurno, @NroOrden AS NroOrden;
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
- **Estado:** `PENDIENTE DE IMPLEMENTACIÓN`
- **Devuelve:** `IdTurno`, `NroOrden`, `Fecha`, `Estado`, `Especialidad`, `Triage`, `NombrePaciente`, `ApellidoPaciente`, `DniPaciente`, `ObraSocial`.

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
        p.ObraSocial
    FROM Turnos t
    INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
    LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
    WHERE t.Activo = 1 
      AND t.Estado = 'En Espera'
    ORDER BY t.IdPrioridad ASC, t.FechaCreacion ASC;
END;
GO
```

---

### 6.4 `sp_LlamarSiguienteTurno` / `sp_LlamarSiguientePaciente`
- **Descripción:** Actualiza el estado del turno a `'Llamado'`, asociándole el médico tratante y el consultorio donde se realizará la atención.
- **Entidad:** Turno
- **Operación:** Llamado Médico
- **Tablas:** `Turnos`
- **Forms que lo utilizan:** `FrmListaTurnosAtencion`
- **Acción:** Botón `btnSiguientePaciente`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno llamado. |
  | `@NombreMedico` | `NVARCHAR(100)` | IN | Nombre del profesional médico. |
  | `@SalaAsignada` | `NVARCHAR(100)` | IN | Consultorio/sala donde se atiende. |

```sql
CREATE OR ALTER PROCEDURE sp_LlamarSiguientePaciente
    @IdTurno INT,
    @NombreMedico NVARCHAR(100),
    @SalaAsignada NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Turnos
    SET Estado = 'Llamado',
        FechaModificacion = GETDATE()
    WHERE IdTurno = @IdTurno AND Activo = 1;
END;
GO
```

---

### 6.5 `sp_IniciarAtencionTurno`
- **Descripción:** Registra el inicio formal de la consulta médica pasando el turno al estado `'En Consulta'`.
- **Entidad:** Turno
- **Operación:** Inicio de Consulta
- **Tablas:** `Turnos`
- **Forms que lo utilizan:** `FrmListaTurnosAtencion`
- **Acción:** Botón `btnIniciarAtencion`
- **Estado:** `PENDIENTE DE IMPLEMENTACIÓN`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno en atención. |

```sql
CREATE OR ALTER PROCEDURE sp_IniciarAtencionTurno
    @IdTurno INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Turnos
    SET Estado = 'En Consulta',
        FechaModificacion = GETDATE()
    WHERE IdTurno = @IdTurno AND Activo = 1;
END;
GO
```

---

### 6.6 `sp_FinalizarAtencion` / `sp_FinalizarAtencionTurno`
- **Descripción:** Concluye la consulta médica: pasa el turno a estado `'Atendido'`, libera la sala y registra las observaciones clínicas en el historial.
- **Entidad:** Turno / Sala / HistoriaClinica
- **Operación:** Cierre de Consulta
- **Tablas:** `Turnos`, `Salas`, `HistoriasClinicas`
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

```sql
CREATE OR ALTER PROCEDURE sp_FinalizarAtencionTurno
    @IdTurno INT,
    @Diagnostico NVARCHAR(MAX),
    @NombreMedico NVARCHAR(100),
    @SalaAsignada NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Marcar el turno como Atendido
    UPDATE Turnos
    SET Estado = 'Atendido',
        FechaModificacion = GETDATE()
    WHERE IdTurno = @IdTurno;

    -- 2. Liberar la sala a Disponible
    UPDATE Salas
    SET EstadoSala = 'Disponible',
        FechaModificacion = GETDATE()
    WHERE NombreSala = @SalaAsignada AND Activo = 1;
END;
GO
```

---

### 6.7 `sp_InsertarHistoriaClinica`
- **Descripción:** Registra las notas clínicas, anamnesis y recetas farmacológicas emitidas por el médico.
- **Entidad:** HistoriaClinica
- **Operación:** Alta
- **Tablas:** `HistoriasClinicas`
- **Forms que lo utilizan:** `FrmListaTurnosAtencion`
- **Acción:** Botón `btnTerminarAtencion`
- **Estado:** `EN USO`
- **Parámetros:**
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@TipoTurno` | `NVARCHAR(50)` | IN | 'Consulta' o 'Emergencia'. |
  | `@DiagRapido` | `NVARCHAR(255)` | IN | Diagnóstico presuntivo. |
  | `@DescripHistoriaClinica` | `NVARCHAR(MAX)` | IN | Evolución médica. |
  | `@RecetaMedicamentos` | `NVARCHAR(MAX)` | IN | Recetas y posología. |
  | `@IdPaciente` | `INT` | IN | ID del paciente. |
  | `@IdTurno` | `INT` | IN | ID del turno atendido. |
  | `@IdUsuario` | `INT` | IN | ID del médico tratante. |

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

    INSERT INTO HistoriasClinicas (Fecha, TipoTurno, DiagRapido, DescripHistoriaClinica, RecetaMedicamentos, IdPaciente, IdTurno, IdUsuario, Activo, FechaCreacion)
    VALUES (GETDATE(), @TipoTurno, @DiagRapido, @DescripHistoriaClinica, @RecetaMedicamentos, @IdPaciente, @IdTurno, @IdUsuario, 1, GETDATE());
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

### 6.9 `sp_ListarTurnosGeneralesPantalla`
- **Descripción:** Obtiene los turnos programados y de especialidad del día para la grilla general de la pantalla pública de sala de espera (`FrmUsuarioVentana`). Muestra el código de turno, horario, fecha, especialidad médica, estado de atención y consultorio asignado.
- **Entidad:** Turno / Especialidad / Sala
- **Operación:** Monitor Público / Consulta Pantalla
- **Tablas:** `Turnos`, `Especialidades`, `Salas`
- **Forms que lo utilizan:** `FrmUsuarioVentana`
- **Acción:** Carga inicial y refresco automático periódico (`dgvGeneral`)
- **Estado:** `PENDIENTE DE IMPLEMENTACIÓN`
- **Devuelve:** `Turno`, `Hora`, `Fecha`, `Especialidad`, `Estado`, `Sala`.

```sql
CREATE OR ALTER PROCEDURE sp_ListarTurnosGeneralesPantalla
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.NroOrden AS Turno,
        CONVERT(VARCHAR(5), t.Horario, 108) AS Hora,
        CONVERT(VARCHAR(10), t.Fecha, 103) AS Fecha,
        ISNULL(e.Nombre, 'General') AS Especialidad,
        t.Estado,
        ISNULL(s.NombreSala, '--') AS Sala
    FROM Turnos t
    LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Salas s ON t.IdSala = s.IdSala
    WHERE t.Activo = 1 
      AND (t.TipoTurno <> 'Emergencia' OR t.TipoTurno IS NULL)
      AND CAST(t.Fecha AS DATE) = CAST(GETDATE() AS DATE)
    ORDER BY t.Horario ASC, t.FechaCreacion ASC;
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

## 8. Tabla Resumen General

| Stored Procedure | Entidad | Operación | Form(s) que lo utilizan | Acción dentro del Form | Estado |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `sp_ValidarLogin` | Usuario / Rol | Autenticación | `FrmLogin` | Botón `button1` (Iniciar sesión) | `EN USO` |
| `sp_ListarRoles` | Rol | Listado | `FrmGestionUsuarios` | Cargar combo de roles | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_InsertarRol` | Rol | Alta | Ninguno | Script inicial de seeds | `NO UTILIZADO` |
| `sp_ListarUsuarios` | Usuario | Listado / Grilla | `FrmGestionUsuarios` | Cargar grilla de usuarios | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_InsertarUsuario` | Usuario | Alta | `FrmGestionUsuarios` | Botón `btnGuardar` | `EN USO` |
| `sp_ModificarUsuario` | Usuario | Modificación | `FrmGestionUsuarios` | Botón `btnModificar` y celda DataGrid | `EN USO` |
| `sp_EliminarUsuario` | Usuario | Baja lógica | `FrmGestionUsuarios` | Botón `btnEliminar` | `EN USO` |
| `sp_ListarPersonalMedico`| Usuario | Selector | `FrmSalasAdmin` | Cargar lista de médicos asignables | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_InsertarSala` | Sala | Alta | `FrmSalasAdmin` | Botón `btnGuardar` | `EN USO` |
| `sp_ModificarSala` | Sala | Modificación | Ninguno | Preparado para renombrar salas | `NO UTILIZADO` |
| `sp_EliminarSala` | Sala | Baja lógica | `FrmSalasAdmin` | Botón `btnEliminar` | `EN USO` |
| `sp_ObtenerSalas` | Sala | Listado | `FrmSalasAdmin`, `FrmGestionUsuarios`, `MisSalas_PM` | Cargar grillas de salas activas | `EN USO` |
| `sp_AsignarSalaMedico` | DetalleSala | Asignación | `FrmGestionUsuarios`, `FrmSalasAdmin` | Botón `btnGuardar` | `EN USO` |
| `sp_AbrirSala` | Sala | Cambio de Estado | `MisSalas_PM` | Botón `btnAbrirSala` | `EN USO` |
| `sp_CerrarSala` | Sala | Cambio de Estado | `MisSalas_PM` | Botón `btnCerrarSala` | `EN USO` |
| `sp_ActualizarEstadoSala`| Sala | Actualización | `FrmListaTurnosAtencion` | Inicio/fin de consulta médica | `EN USO` |
| `sp_ListarEspecialidades`| Especialidad | Listado | `FrmGestionEspecialidades`, `FrmGestionUsuarios`, `FrmTurnoEspecialidad`, `FrmListaTurnos`, `FrmListaTurnosAtencion` | Carga de combos y grillas | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_InsertarEspecialidad`| Especialidad | Alta | `FrmGestionEspecialidades` | Botón `btnGuardar` | `EN USO` |
| `sp_ModificarEspecialidad`| Especialidad | Modificación | Ninguno | Preparado para edición | `NO UTILIZADO` |
| `sp_EliminarEspecialidad`| Especialidad | Baja lógica | `FrmGestionEspecialidades` | Botón `btnDesactivar` | `EN USO` |
| `sp_AsignarEspecialidadMedico`| MedicoEspecialidad | Asignación | `FrmGestionUsuarios` | Botón `btnGuardar` | `EN USO` |
| `sp_InsertarPaciente` | Paciente | Alta | `FrmTurnoEmergencia`, `FrmTurnoEspecialidad` | Botón `BtnGenerarTurno` | `EN USO` |
| `sp_GuardarPaciente` | Paciente | Upsert por DNI | `FrmTurnoEmergencia`, `FrmTurnoEspecialidad` | Botón `BtnGenerarTurno` | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_InsertarTurno` | Turno | Alta | `FrmTurnoEspecialidad` | Botón `BtnGenerarTurno` | `EN USO` |
| `sp_ObtenerSintomas` | Sintoma | Catálogo | `FrmTurnoEmergencia` | Carga dinámica de triage | `EN USO` |
| `sp_RegistrarTurnoSintoma`| TurnoSintoma | Relación | `FrmTurnoEmergencia` | Botón `BtnGenerarTurno` | `EN USO` |
| `sp_RegistrarTurnoEmergencia`| Turno / Triage | Alta urgencia | `FrmTurnoEmergencia` | Botón `BtnGenerarTurno` | `EN USO` |
| `sp_ObtenerHorariosDisponibles`| Turno | Disponibilidad | `FrmTurnoEspecialidad` | Cambio de fecha en calendario | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_CrearTurnoEspecialidad`| Turno | Alta programada | `FrmTurnoEspecialidad` | Botón `BtnGenerarTurno` | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_ListarTurnosEmergencia`| Turno | Monitor guardia / Pantalla | `FrmListaTurnos`, `FrmUsuarioVentana` | Cargar grilla y monitor de emergencias | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_ListarTurnosGeneralesPantalla`| Turno / Especialidad / Sala | Monitor público general | `FrmUsuarioVentana` | Carga y refresco periódico grilla general | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_ListarTurnosEspecialidad`| Turno | Consulta filtro | `FrmListaTurnos` | Selección de especialidad | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_ObtenerTurnosEnEspera`| Turno | Cola médica | `FrmListaTurnosAtencion` | Refrescar listado de espera | `EN USO` |
| `sp_ObtenerListaTurnos` | Turno | Listado dinámico | `FrmListaTurnos`, `FrmListaTurnosAtencion` | Carga de grillas de turnos | `EN USO` |
| `sp_ListarTurnosAtencion`| Turno / Paciente | Cola atención | `FrmListaTurnosAtencion` | Cargar turnos con datos paciente | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_LlamarSiguientePaciente`| Turno | Llamado | `FrmListaTurnosAtencion` | Botón `btnSiguientePaciente` | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_IniciarAtencionTurno`| Turno | En Consulta | `FrmListaTurnosAtencion` | Botón `btnIniciarAtencion` | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_FinalizarAtencionTurno`| Turno / Sala | Cierre atención | `FrmListaTurnosAtencion` | Botón `btnTerminarAtencion` | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_InsertarHistoriaClinica`| HistoriaClinica | Alta médica | `FrmListaTurnosAtencion` | Botón `btnTerminarAtencion` | `EN USO` |
| `sp_ObtenerTurnosPantallaPublica`| Turno / Sala | Monitor público | `FrmUsuarioVentana` / Pantalla TV | Refresco periódico de llamados | `PENDIENTE DE IMPLEMENTACIÓN` |
| `sp_ObtenerHistoriaClinicaPaciente`| HistoriaClinica | Historial | Visor Historia Clínica | Consulta por paciente | `SIN FORM ASOCIADO / PENDIENTE DE VISOR HC` |

---

## 9. Resumen por Formulario

### `FrmLogin`
- `sp_ValidarLogin` → Botón "Iniciar Sesión" (`button1_Click`). Valida credenciales activas y obtiene rol del usuario. Incluye ruteo dinámico para el rol `Usuario Ventana` (`IdRol = 4`), abriendo `FrmUsuarioVentana`.

### `FrmGestionUsuarios`
- `sp_ListarRoles` → Carga inicial del desplegable de roles (`CargarRolesDesdeBD`). Soporta los 4 roles: Administrador, Personal médico, Recepcionista y Usuario Ventana.
- `sp_ListarEspecialidades` → Carga inicial del listado de especialidades asignables (`CargarEspecialidadesDesdeBD`).
- `sp_ObtenerSalas` → Carga inicial del listado de consultorios asignables (`CargarSalasDesdeBD`).
- `sp_ListarUsuarios` → Carga y refresco de la grilla de usuarios activos (`CargarUsuariosDesdeBD`).
- `sp_InsertarUsuario` → Botón "Guardar" (`btnGuardar_Click`). Da de alta el usuario y devuelve su ID.
- `sp_ModificarUsuario` → Botón "Modificar" (`btnModificar_Click`) y edición directa de celdas en el DataGrid (`dgvPersonal_CellEndEdit`). Modifica los datos del usuario.
- `sp_AsignarEspecialidadMedico` → Botón "Guardar". Asocia cada especialidad tildada al usuario creado.
- `sp_AsignarSalaMedico` → Botón "Guardar". Asocia cada sala tildada al usuario creado.
- `sp_EliminarUsuario` → Botón "Eliminar" (`btnEliminar_Click`). Ejecuta la baja lógica del usuario seleccionado.

### `FrmSalasAdmin`
- `sp_ListarPersonalMedico` → Carga del listado de médicos asignables a consultorios (`CargarPersonalMedicoDesdeBD`).
- `sp_ObtenerSalas` → Carga y refresco de la grilla de salas (`CargarSalasDesdeBD`).
- `sp_InsertarSala` → Botón "Guardar" (`btnGuardar_Click`). Registra la nueva sala.
- `sp_AsignarSalaMedico` → Botón "Guardar". Vincula los médicos seleccionados a la sala creada.
- `sp_EliminarSala` → Botón "Eliminar" (`btnEliminar_Click`). Ejecuta la baja lógica de la sala.

### `FrmGestionEspecialidades`
- `sp_ListarEspecialidades` → Carga y refresco de la grilla de especialidades (`CargarEspecialidadesDesdeBD`).
- `sp_InsertarEspecialidad` → Botón "Guardar" (`btnGuardar_Click`). Da de alta una nueva especialidad.
- `sp_EliminarEspecialidad` → Botón "Desactivar" (`btnDesactivar_Click`). Ejecuta la baja lógica.

### `FrmTurnoEmergencia`
- `sp_ObtenerSintomas` → Carga del catálogo para categorización de triage (`CargarCatalogoSintomas`).
- `sp_GuardarPaciente` (o `sp_InsertarPaciente`) → Botón "Generar Turno". Registra o recupera el ID del paciente por DNI.
- `sp_RegistrarTurnoEmergencia` → Botón "Generar Turno". Calcula triage, crea turno de guardia y vincula síntoma.
- `sp_RegistrarTurnoSintoma` → Botón "Generar Turno". Asocia síntomas adicionales al turno generado.

### `FrmTurnoEspecialidad`
- `sp_ObtenerEspecialidades` (o `sp_ListarEspecialidades`) → Carga del combo de especialidades (`CargarEspecialidades`).
- `sp_ObtenerHorariosDisponibles` → Selección de fecha en calendario (`calFechaTurno_DateChanged`). Lista horarios libres.
- `sp_GuardarPaciente` (o `sp_InsertarPaciente`) → Botón "Generar Turno". Registra o recupera paciente.
- `sp_CrearTurnoEspecialidad` (o `sp_InsertarTurno`) → Botón "Generar Turno". Registra el turno programado correlativo.

### `FrmListaTurnos`
- `sp_ListarTurnosEmergencia` → Carga inicial y refresco de la grilla y contadores de guardia (`CargarTurnosEmergencia`).
- `sp_ObtenerEspecialidades` → Carga del selector de especialidades (`CargarEspecialidades`).
- `sp_ListarTurnosEspecialidad` (o `sp_ObtenerListaTurnos`) → Selección de especialidad (`cmbEspecialidades_SelectedIndexChanged`).

### `FrmListaTurnosAtencion`
- `sp_ListarEspecialidades` → Carga del selector de servicios del médico (`CargarServiciosDelMedico`).
- `sp_ListarTurnosAtencion` (o `sp_ObtenerTurnosEnEspera`) → Carga de la cola de pacientes en espera (`CargarTurnosDesdeBD`).
- `sp_LlamarSiguientePaciente` (o `sp_LlamarSiguienteTurno`) → Botón "Siguiente Paciente". Pasa a estado 'Llamado'.
- `sp_IniciarAtencionTurno` (o `sp_ActualizarEstadoSala`) → Botón "Iniciar Atención". Pasa a estado 'En Consulta' y sala a 'Ocupada'.
- `sp_FinalizarAtencionTurno` (o `sp_FinalizarAtencion`) → Botón "Terminar Atención". Pasa a 'Atendido' y libera la sala.
- `sp_InsertarHistoriaClinica` → Botón "Terminar Atención". Guarda diagnóstico y recetas en la historia clínica.

### `MisSalas_PM`
- `sp_ObtenerSalas` (`@IdUsuario = médico`) → Carga de la grilla de salas asignadas al médico logueado (`CargarMisSalas`).
- `sp_AbrirSala` → Botón "Abrir Sala" (`btnAbrirSala_Click`). Habilita la sala a 'Disponible' controlando validaciones.
- `sp_CerrarSala` → Botón "Cerrar Sala" (`btnCerrarSala_Click`). Pasa la sala a 'En Mantenimiento' controlando que no esté ocupada.

### `FrmAdmin`
- Sin llamadas directas a SPs (Contenedor MDI administrativo). Gestiona apertura de `FrmGestionUsuarios`, `FrmSalasAdmin`, `FrmGestionEspecialidades`.

### `Pantalla_Principal_PERSONAL_MEDICO` (`FrmPersonalMedico`)
- Sin llamadas directas a SPs (Contenedor MDI médico). Transfiere el contexto de `_usuarioActual` (médico logueado) hacia `MisSalas_PM` y `FrmListaTurnosAtencion`.

### `FrmRecepcionista`
- Sin llamadas directas a SPs (Contenedor MDI de recepción). Gestiona apertura de `FrmTurnoEmergencia`, `FrmTurnoEspecialidad`, `FrmListaTurnos`, y `FrmUsuarioVentana` a través del nuevo botón `btnUsuarioVentana` ("Pantalla Turnos").

### `FrmUsuarioVentana`
- `sp_ListarTurnosEmergencia` → Carga inicial y auto-refresco periódico de la grilla de emergencias/triage (`dgvEmergencias`).
- `sp_ListarTurnosGeneralesPantalla` → Carga inicial y auto-refresco periódico de la grilla de turnos generales y consultorios (`dgvGeneral`).
- `sp_ObtenerTurnosPantallaPublica` → Alternativa de consulta para monitor público de llamados activos.
- Rol asignado: `Usuario Ventana` (`IdRol = 4`). Puede iniciarse con sesión autenticada o como visor incrustado desde `FrmRecepcionista`.
