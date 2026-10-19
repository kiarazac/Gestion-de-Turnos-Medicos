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

        -- 3. Marcar el turno como Atendido, registrar la sala y persistir IdUsuario
        UPDATE Turnos
        SET Estado = 'Atendido',
            IdSala = COALESCE(@IdSala, IdSala),
            IdUsuario = COALESCE(@IdUsuario, IdUsuario),
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
"@,
@"
CREATE OR ALTER PROCEDURE sp_ReporteGerencialRecaudacionAuditoriaResumen
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @FechaDesde IS NULL SET @FechaDesde = CAST(GETDATE() AS DATE);
        IF @FechaHasta IS NULL SET @FechaHasta = @FechaDesde;

        SELECT 
            @FechaDesde AS FechaDesde,
            @FechaHasta AS FechaHasta,
            COUNT(t.IdTurno) AS TotalTurnos,
            ISNULL(SUM(t.Monto), 0.00) AS TotalRecaudado,
            ISNULL(SUM(CASE WHEN t.TipoTurno = 'Emergencia' THEN 1 ELSE 0 END), 0) AS TurnosEmergencia,
            ISNULL(SUM(CASE WHEN t.TipoTurno = 'Emergencia' THEN t.Monto ELSE 0 END), 0.00) AS MontoEmergencia,
            ISNULL(SUM(CASE WHEN t.TipoTurno <> 'Emergencia' THEN 1 ELSE 0 END), 0) AS TurnosEspecialidad,
            ISNULL(SUM(CASE WHEN t.TipoTurno <> 'Emergencia' THEN t.Monto ELSE 0 END), 0.00) AS MontoEspecialidad,
            ISNULL(SUM(CASE WHEN os.Nombre IS NOT NULL AND os.Nombre NOT LIKE '%Particular%' THEN 1 ELSE 0 END), 0) AS TurnosObraSocial,
            ISNULL(SUM(CASE WHEN os.Nombre IS NOT NULL AND os.Nombre NOT LIKE '%Particular%' THEN t.Monto ELSE 0 END), 0.00) AS MontoObraSocial,
            ISNULL(SUM(CASE WHEN os.Nombre IS NULL OR os.Nombre LIKE '%Particular%' THEN 1 ELSE 0 END), 0) AS TurnosParticulares,
            ISNULL(SUM(CASE WHEN os.Nombre IS NULL OR os.Nombre LIKE '%Particular%' THEN t.Monto ELSE 0 END), 0.00) AS MontoParticulares
        FROM Turnos t
        INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
        LEFT JOIN ObrasSociales os ON p.IdObraSocial = os.IdObraSocial
        WHERE CAST(COALESCE(t.FechaCreacion, t.Fecha) AS DATE) BETWEEN @FechaDesde AND @FechaHasta
          AND t.Activo = 1
          AND t.Estado <> 'Cancelado';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@,
@"
CREATE OR ALTER PROCEDURE sp_ReporteGerencialRecaudacionAuditoriaDetalle
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @FechaDesde IS NULL SET @FechaDesde = CAST(GETDATE() AS DATE);
        IF @FechaHasta IS NULL SET @FechaHasta = @FechaDesde;

        SELECT 
            t.IdTurno,
            ISNULL(t.NroOrden, 'S/N') AS NroOrden,
            ISNULL(t.FechaCreacion, t.Fecha) AS FechaEmision,
            ISNULL(CONCAT(p.Apellido, ', ', p.Nombre), 'Sin Paciente') AS Paciente,
            ISNULL(p.Dni, '--') AS DniPaciente,
            ISNULL(t.TipoTurno, 'Consulta') AS TipoTurno,
            ISNULL(e.Nombre, 'General') AS Especialidad,
            ISNULL(os.Nombre, 'Particular / Sin Obra Social') AS ObraSocial,
            CASE 
                WHEN os.Nombre IS NULL OR os.Nombre LIKE '%Particular%' THEN 'Particular' 
                ELSE 'Obra Social / Prepaga' 
            END AS TipoCobertura,
            ISNULL(t.Monto, 0.00) AS MontoCobrado,
            ISNULL(t.Estado, 'En Espera') AS EstadoTurno,
            ISNULL(CONCAT('Dr. ', u.Apellido, ' ', u.Nombre), 'Sin Asignar') AS MedicoAsignado
        FROM Turnos t
        INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
        LEFT JOIN ObrasSociales os ON p.IdObraSocial = os.IdObraSocial
        LEFT JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
        LEFT JOIN Usuarios u ON t.IdUsuario = u.IdUsuario
        WHERE CAST(COALESCE(t.FechaCreacion, t.Fecha) AS DATE) BETWEEN @FechaDesde AND @FechaHasta
          AND t.Activo = 1
          AND t.Estado <> 'Cancelado'
        ORDER BY ISNULL(t.FechaCreacion, t.Fecha) DESC, t.IdTurno DESC;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@,
@"
CREATE OR ALTER PROCEDURE sp_ReporteGerencialRankingMedicos
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @FechaDesde IS NULL SET @FechaDesde = '2000-01-01';
        IF @FechaHasta IS NULL SET @FechaHasta = CAST(GETDATE() AS DATE);

        DECLARE @TotalAtencionesPeriodo INT = 0;

        SELECT @TotalAtencionesPeriodo = COUNT(DISTINCT t.IdTurno)
        FROM Turnos t
        LEFT JOIN HistoriasClinicas hc ON t.IdTurno = hc.IdTurno
        WHERE t.Estado IN ('Atendido', 'Finalizado')
          AND t.Activo = 1
          AND CAST(COALESCE(hc.Fecha, t.FechaModificacion, t.Fecha) AS DATE) BETWEEN @FechaDesde AND @FechaHasta;

        SELECT 
            ROW_NUMBER() OVER(
                ORDER BY COUNT(DISTINCT t.IdTurno) DESC, 
                         ISNULL(SUM(t.Monto), 0.00) DESC, 
                         u.Apellido ASC
            ) AS Posicion,
            u.IdUsuario,
            ISNULL(CONCAT(u.Apellido, ', ', u.Nombre), 'Médico Desconocido') AS NombreMedico,
            ISNULL(u.NroMatricula, '--') AS Matricula,
            ISNULL((
                SELECT STRING_AGG(e.Nombre, ', ')
                FROM MedicosEspecialidades me
                INNER JOIN Especialidades e ON me.IdEspecialidad = e.IdEspecialidad
                WHERE me.IdUsuario = u.IdUsuario AND me.Activo = 1 AND e.Activo = 1
            ), 'General') AS Especialidad,
            COUNT(DISTINCT t.IdTurno) AS AtencionesRealizadas,
            ISNULL(SUM(t.Monto), 0.00) AS TotalRecaudado,
            CASE 
                WHEN @TotalAtencionesPeriodo > 0 
                THEN CAST((COUNT(DISTINCT t.IdTurno) * 100.0 / @TotalAtencionesPeriodo) AS DECIMAL(5,2))
                ELSE 0.00 
            END AS PorcentajeParticipacion
        FROM Usuarios u
        INNER JOIN Roles r ON u.IdRol = r.IdRol
        LEFT JOIN Turnos t ON (t.IdUsuario = u.IdUsuario OR t.IdTurno IN (SELECT hc.IdTurno FROM HistoriasClinicas hc WHERE hc.IdUsuario = u.IdUsuario))
                           AND t.Estado IN ('Atendido', 'Finalizado')
                           AND t.Activo = 1
                           AND CAST(COALESCE(
                               (SELECT TOP 1 hc2.Fecha FROM HistoriasClinicas hc2 WHERE hc2.IdTurno = t.IdTurno ORDER BY hc2.IdHistoria DESC),
                               t.FechaModificacion, 
                               t.Fecha
                           ) AS DATE) BETWEEN @FechaDesde AND @FechaHasta
        WHERE (u.IdRol = 1 OR r.Descripcion LIKE '%Médic%' OR r.Descripcion LIKE '%Medic%')
          AND u.Activo = 1
        GROUP BY u.IdUsuario, u.Apellido, u.Nombre, u.NroMatricula
        ORDER BY AtencionesRealizadas DESC, TotalRecaudado DESC, u.Apellido ASC;
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
