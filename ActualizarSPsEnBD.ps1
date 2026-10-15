$conn = New-Object System.Data.SqlClient.SqlConnection('Server=localhost\SQLEXPRESS;Database=dbGestionTurnos;Integrated Security=True;TrustServerCertificate=True;')
$conn.Open()

$sps = @(
@"
CREATE OR ALTER PROCEDURE sp_ListarTurnosAtencion
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    SELECT 
        t.IdTurno,
        t.NroOrden,
        CAST(CAST(t.Fecha AS DATE) AS DATETIME) + CAST(ISNULL(t.Horario, ISNULL(CAST(t.FechaCreacion AS TIME), '00:00')) AS DATETIME) AS Fecha,
        t.Estado,
        ISNULL(e.Nombre, 'Emergencias / Guardia') AS Especialidad,
        ISNULL(pr.Descripcion, 'MEDIA') AS Triage,
        p.Nombre AS NombrePaciente,
        p.Apellido AS ApellidoPaciente,
        p.Dni AS DniPaciente,
        ISNULL(os.Nombre, 'Particular / Sin Obra Social') AS ObraSocial,
        ISNULL(s.NombreSala, '') AS NombreSala,
        ISNULL(
            (
                SELECT STRING_AGG(s2.Descripcion, ', ')
                FROM TurnoSintomas ts2
                INNER JOIN Sintomas s2 ON ts2.IdSintoma = s2.IdSintoma
                WHERE ts2.IdTurno = t.IdTurno AND ts2.Activo = 1 AND s2.Activo = 1
            ),
            ''
        ) AS Sintomas,
        t.IdUsuario,
        t.IdSala
    FROM Turnos t
    INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
    LEFT JOIN ObrasSociales os ON p.IdObraSocial = os.IdObraSocial
    LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Prioridades pr ON t.IdPrioridad = pr.IdPrioridad
    LEFT JOIN Salas s ON t.IdSala = s.IdSala
    WHERE t.Activo = 1 
      AND t.Estado IN ('En Espera', 'Llamado', 'En Consulta', 'En Atención', 'En Atencion')
      AND t.Estado NOT IN ('Atendido', 'Ausente', 'Cancelado')
    ORDER BY 
        CASE WHEN t.Estado IN ('Llamado', 'En Consulta', 'En Atención', 'En Atencion') THEN 0 ELSE 1 END ASC,
        t.IdPrioridad ASC, 
        t.FechaCreacion ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@,
@"
CREATE OR ALTER PROCEDURE sp_MarcarTurnoAusente
    @IdTurno INT,
    @SalaAsignada NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Turnos WHERE IdTurno = @IdTurno AND Activo = 1)
        BEGIN
            THROW 50086, 'El turno a marcar como ausente no existe o se encuentra inactivo.', 1;
        END

        DECLARE @IdSala INT = NULL;

        SELECT @IdSala = IdSala FROM Turnos WHERE IdTurno = @IdTurno AND Activo = 1;

        IF @IdSala IS NULL AND @SalaAsignada IS NOT NULL AND LTRIM(RTRIM(@SalaAsignada)) <> '' AND @SalaAsignada <> '--' AND @SalaAsignada NOT LIKE '%Sin sala abierta%'
        BEGIN
            SELECT TOP 1 @IdSala = IdSala FROM Salas WHERE NombreSala = @SalaAsignada AND Activo = 1;
            IF @IdSala IS NULL
            BEGIN
                SELECT TOP 1 @IdSala = IdSala FROM Salas WHERE (NombreSala LIKE '%' + @SalaAsignada + '%' OR @SalaAsignada LIKE '%' + NombreSala + '%') AND Activo = 1;
            END
        END

        BEGIN TRANSACTION;

        UPDATE Turnos
        SET Estado = 'Ausente',
            FechaModificacion = GETDATE()
        WHERE IdTurno = @IdTurno AND Activo = 1;

        IF @IdSala IS NOT NULL
        BEGIN
            UPDATE Salas
            SET EstadoSala = 'Disponible',
                FechaModificacion = GETDATE()
            WHERE IdSala = @IdSala AND Activo = 1;
        END

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@,
@"
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
      AND t.Estado IN ('En Espera', 'Llamado', 'En Consulta', 'En Atención', 'En Atencion')
      AND t.Estado NOT IN ('Atendido', 'Ausente', 'Cancelado')
    ORDER BY 
        CASE 
            WHEN t.Estado = 'En Espera' THEN 1 
            WHEN t.Estado = 'Llamado' THEN 2 
            WHEN t.Estado IN ('En Consulta', 'En Atención', 'En Atencion') THEN 3 
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
"@,
@"
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
      AND t.Estado IN ('En Espera', 'Llamado', 'En Consulta', 'En Atención', 'En Atencion')
      AND t.Estado NOT IN ('Atendido', 'Ausente', 'Cancelado')
    ORDER BY t.Fecha ASC, t.Horario ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@,
@"
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
      AND t.Estado IN ('En Espera', 'Llamado', 'En Consulta', 'En Atención', 'En Atencion')
      AND t.Estado NOT IN ('Atendido', 'Ausente', 'Cancelado')
    ORDER BY t.Fecha ASC, t.Horario ASC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@,
@"
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

        -- 2. Validar el estado de origen (debe ser estrictamente 'En Espera')
        DECLARE @EstadoActual NVARCHAR(50);
        SELECT @EstadoActual = Estado FROM Turnos WHERE IdTurno = @IdTurno AND Activo = 1;

        IF @EstadoActual <> 'En Espera'
        BEGIN
            THROW 50081, 'El turno no puede ser llamado porque su estado de origen es incompatible (debe estar en estado ''En Espera'').', 1;
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

        BEGIN TRANSACTION;

        UPDATE Turnos
        SET Estado = 'Llamado',
            IdSala = COALESCE(@IdSala, IdSala),
            FechaModificacion = GETDATE()
        WHERE IdTurno = @IdTurno AND Activo = 1;

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
    END CATCH;
END;
"@,
@"
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

        -- 2. Validar el estado de origen (debe ser estrictamente 'Llamado')
        DECLARE @EstadoActual NVARCHAR(50);
        SELECT @EstadoActual = Estado FROM Turnos WHERE IdTurno = @IdTurno AND Activo = 1;

        IF @EstadoActual <> 'Llamado'
        BEGIN
            THROW 50083, 'No se puede iniciar la atención porque el turno no ha sido llamado previamente (debe estar en estado ''Llamado'').', 1;
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

        UPDATE Turnos
        SET Estado = 'En Consulta',
            IdSala = COALESCE(@IdSala, IdSala),
            FechaModificacion = GETDATE()
        WHERE IdTurno = @IdTurno AND Activo = 1;

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
    END CATCH;
END;
"@,
@"
CREATE OR ALTER PROCEDURE sp_FinalizarAtencionTurno
    @IdTurno INT,
    @Diagnostico NVARCHAR(MAX),
    @NombreMedico NVARCHAR(100),
    @SalaAsignada NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 1. Validar que el turno exista y esté activo
        IF NOT EXISTS (SELECT 1 FROM Turnos WHERE IdTurno = @IdTurno AND Activo = 1)
        BEGIN
            THROW 50084, 'El turno a finalizar no existe o se encuentra inactivo en el sistema.', 1;
        END

        -- 2. Validar el estado de origen (debe ser estrictamente 'En Consulta')
        DECLARE @EstadoActual NVARCHAR(50);
        SELECT @EstadoActual = Estado FROM Turnos WHERE IdTurno = @IdTurno AND Activo = 1;

        IF @EstadoActual <> 'En Consulta'
        BEGIN
            THROW 50087, 'No se puede finalizar la atención porque el estado de origen es incompatible (debe estar en estado ''En Consulta'').', 1;
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

        -- 3. Marcar el turno como Atendido y registrar la sala
        UPDATE Turnos
        SET Estado = 'Atendido',
            IdSala = COALESCE(@IdSala, IdSala),
            FechaModificacion = GETDATE()
        WHERE IdTurno = @IdTurno AND Activo = 1;

        -- 4. Liberar la sala a Disponible
        IF @IdSala IS NOT NULL
        BEGIN
            UPDATE Salas
            SET EstadoSala = 'Disponible',
                FechaModificacion = GETDATE()
            WHERE IdSala = @IdSala AND Activo = 1;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@
)

foreach ($sp in $sps) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sp
    $cmd.ExecuteNonQuery()
    Write-Host "[OK] Procedimiento almacenado actualizado exitosamente."
}

$conn.Close()
Write-Host "Todos los SPs fueron aplicados a la base de datos."
