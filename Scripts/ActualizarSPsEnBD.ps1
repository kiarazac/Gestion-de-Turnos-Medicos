$conn = New-Object System.Data.SqlClient.SqlConnection('Server=localhost\SQLEXPRESS;Database=dbGestionTurnos;Integrated Security=True;TrustServerCertificate=True;')
$conn.Open()

$sps = @(
@"
IF OBJECT_ID('Pacientes') IS NOT NULL AND COL_LENGTH('Pacientes', 'Telefono') IS NULL
    ALTER TABLE Pacientes ADD Telefono NVARCHAR(20) NULL;
"@,
@"
UPDATE Turnos SET TipoTurno = 'Especialidad' WHERE TipoTurno = 'Consulta';
UPDATE HistoriasClinicas SET TipoTurno = 'Especialidad' WHERE TipoTurno = 'Consulta';
"@,
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
      AND t.TipoTurno = 'Especialidad'
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
        ISNULL(s.NombreSala, '--') AS NombreSala,
        pac.Telefono
    FROM Turnos t
    INNER JOIN Especialidades e ON t.IdEspecialidad = e.IdEspecialidad
    LEFT JOIN Prioridades p ON t.IdPrioridad = p.IdPrioridad
    LEFT JOIN Salas s ON t.IdSala = s.IdSala
    LEFT JOIN Pacientes pac ON t.IdPaciente = pac.IdPaciente
    WHERE t.Activo = 1
      AND t.TipoTurno = 'Especialidad'
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
DROP PROCEDURE IF EXISTS sp_ReporteGerencialIngresosPorMedico;
DROP PROCEDURE IF EXISTS sp_ReporteGerencialObrasSocialesVsParticulares;
DROP PROCEDURE IF EXISTS sp_ReporteGerencialDemandaMedicos;
DROP PROCEDURE IF EXISTS sp_ReporteGerencialRecaudacionAuditoriaResumen;
DROP PROCEDURE IF EXISTS sp_ReporteGerencialRecaudacionAuditoriaDetalle;
"@,
@"
CREATE OR ALTER FUNCTION fn_ReporteGerencialTurnosFacturados
(
    @FechaDesde DATE,
    @FechaHasta DATE,
    @ArancelEspecialidad DECIMAL(18,2),
    @ArancelEmergencia DECIMAL(18,2)
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        t.IdTurno,
        CAST(COALESCE(t.FechaCreacion, t.Fecha) AS DATE) AS FechaEmision,
        t.Estado,
        x.EsEmergencia,
        COALESCE(t.IdUsuario, (
            SELECT TOP 1 hc.IdUsuario FROM HistoriasClinicas hc
            WHERE hc.IdTurno = t.IdTurno AND hc.Activo = 1
            ORDER BY hc.IdHistoria DESC
        )) AS IdMedico,
        t.IdEspecialidad,
        x.EsObraSocial,
        CASE WHEN x.EsObraSocial = 1 THEN os.IdObraSocial END AS IdObraSocial,
        CASE WHEN x.EsObraSocial = 1 THEN os.Nombre ELSE 'Particular / Sin Obra Social' END AS ObraSocial,
        CASE WHEN x.EsObraSocial = 1 THEN os.PorcentajeCobertura ELSE 0.00 END AS PorcentajeCoberturaVigente,
        x.ArancelBase,
        ISNULL(t.Monto, 0.00) AS CobradoCaja,
        CASE WHEN x.EsObraSocial = 1 AND x.ArancelBase > ISNULL(t.Monto, 0.00)
             THEN x.ArancelBase - ISNULL(t.Monto, 0.00)
             ELSE 0.00
        END AS ACobrarObraSocial
    FROM Turnos t
    INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
    LEFT JOIN ObrasSociales os ON p.IdObraSocial = os.IdObraSocial
    CROSS APPLY (
        SELECT
            CASE WHEN t.TipoTurno LIKE '%Emergencia%' OR t.TipoTurno LIKE '%Guardia%' THEN 1 ELSE 0 END AS EsEmergencia,
            CASE WHEN t.TipoTurno LIKE '%Emergencia%' OR t.TipoTurno LIKE '%Guardia%' THEN @ArancelEmergencia ELSE @ArancelEspecialidad END AS ArancelBase,
            CASE WHEN os.Nombre IS NOT NULL AND os.Nombre NOT LIKE '%Particular%' THEN 1 ELSE 0 END AS EsObraSocial
    ) x
    WHERE CAST(COALESCE(t.FechaCreacion, t.Fecha) AS DATE) BETWEEN @FechaDesde AND @FechaHasta
      AND t.Activo = 1
      AND t.Estado <> 'Cancelado'
);
"@,
@"
CREATE OR ALTER PROCEDURE sp_ReporteGerencialFinanzasResumen
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @ArancelEspecialidad DECIMAL(18,2),
    @ArancelEmergencia DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @FechaDesde IS NULL SET @FechaDesde = CAST(GETDATE() AS DATE);
        IF @FechaHasta IS NULL SET @FechaHasta = @FechaDesde;

        SELECT
            COUNT(f.IdTurno) AS TotalTurnos,
            ISNULL(SUM(f.CobradoCaja), 0.00) AS CobradoCaja,
            ISNULL(SUM(f.ACobrarObraSocial), 0.00) AS ACobrarObraSocial,
            ISNULL(SUM(f.CobradoCaja + f.ACobrarObraSocial), 0.00) AS TotalFacturado,
            CASE WHEN COUNT(f.IdTurno) > 0
                 THEN CAST(SUM(f.CobradoCaja + f.ACobrarObraSocial) / COUNT(f.IdTurno) AS DECIMAL(18,2))
                 ELSE 0.00
            END AS TicketPromedio,
            ISNULL(SUM(CASE WHEN f.Estado IN ('Atendido', 'Finalizado') THEN 1 ELSE 0 END), 0) AS TurnosAtendidos,
            ISNULL(SUM(CASE WHEN f.Estado NOT IN ('Atendido', 'Finalizado', 'Ausente') THEN 1 ELSE 0 END), 0) AS TurnosPendientes,
            ISNULL(SUM(CASE WHEN f.Estado = 'Ausente' THEN 1 ELSE 0 END), 0) AS TurnosAusentes,
            ISNULL(SUM(CASE WHEN f.Estado = 'Ausente' THEN f.CobradoCaja + f.ACobrarObraSocial ELSE 0 END), 0.00) AS FacturadoAusentes
        FROM dbo.fn_ReporteGerencialTurnosFacturados(@FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia) f;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@,
@"
CREATE OR ALTER PROCEDURE sp_ReporteGerencialRecaudacionDiaria
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @ArancelEspecialidad DECIMAL(18,2),
    @ArancelEmergencia DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @FechaDesde IS NULL SET @FechaDesde = CAST(GETDATE() AS DATE);
        IF @FechaHasta IS NULL SET @FechaHasta = @FechaDesde;

        DECLARE @PorMes BIT = CASE WHEN DATEDIFF(DAY, @FechaDesde, @FechaHasta) > 62 THEN 1 ELSE 0 END;

        SELECT
            CAST(per.Periodo AS DATETIME) AS Fecha,
            @PorMes AS AgrupadoPorMes,
            COUNT(f.IdTurno) AS CantidadTurnos,
            ISNULL(SUM(f.CobradoCaja), 0.00) AS CobradoCaja,
            ISNULL(SUM(f.ACobrarObraSocial), 0.00) AS ACobrarObraSocial,
            ISNULL(SUM(f.CobradoCaja + f.ACobrarObraSocial), 0.00) AS TotalFacturado
        FROM dbo.fn_ReporteGerencialTurnosFacturados(@FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia) f
        CROSS APPLY (
            SELECT CASE WHEN @PorMes = 1 THEN DATEFROMPARTS(YEAR(f.FechaEmision), MONTH(f.FechaEmision), 1) ELSE f.FechaEmision END AS Periodo
        ) per
        GROUP BY per.Periodo
        ORDER BY per.Periodo ASC;

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
    @FechaHasta DATE = NULL,
    @ArancelEspecialidad DECIMAL(18,2),
    @ArancelEmergencia DECIMAL(18,2),
    @IdEspecialidad INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @FechaDesde IS NULL SET @FechaDesde = CAST(GETDATE() AS DATE);
        IF @FechaHasta IS NULL SET @FechaHasta = @FechaDesde;

        WITH Facturacion AS (
            SELECT
                f.IdMedico,
                COUNT(f.IdTurno) AS CantidadTurnos,
                SUM(f.CobradoCaja) AS CobradoCaja,
                SUM(f.ACobrarObraSocial) AS ACobrarObraSocial
            FROM dbo.fn_ReporteGerencialTurnosFacturados(@FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia) f
            WHERE f.Estado IN ('Atendido', 'Finalizado')
              AND f.IdMedico IS NOT NULL
              AND (@IdEspecialidad IS NULL OR f.IdEspecialidad = @IdEspecialidad)
            GROUP BY f.IdMedico
        ),
        Total AS (
            SELECT SUM(CobradoCaja + ACobrarObraSocial) AS GranTotal FROM Facturacion
        )
        SELECT
            ROW_NUMBER() OVER(
                ORDER BY ISNULL(fa.CobradoCaja + fa.ACobrarObraSocial, 0.00) DESC,
                         ISNULL(fa.CantidadTurnos, 0) DESC,
                         u.Apellido ASC
            ) AS Posicion,
            u.IdUsuario AS Id,
            CONCAT(u.Apellido, ', ', u.Nombre) AS Nombre,
            ISNULL((
                SELECT STRING_AGG(e.Nombre, ', ')
                FROM MedicosEspecialidades me
                INNER JOIN Especialidades e ON me.IdEspecialidad = e.IdEspecialidad
                WHERE me.IdUsuario = u.IdUsuario AND me.Activo = 1 AND e.Activo = 1
            ), 'General') AS Detalle,
            ISNULL(u.NroMatricula, '--') AS Matricula,
            ISNULL(fa.CantidadTurnos, 0) AS CantidadTurnos,
            ISNULL(fa.CobradoCaja, 0.00) AS CobradoCaja,
            ISNULL(fa.ACobrarObraSocial, 0.00) AS ACobrarObraSocial,
            ISNULL(fa.CobradoCaja + fa.ACobrarObraSocial, 0.00) AS TotalFacturado,
            CASE WHEN fa.CantidadTurnos > 0
                 THEN CAST((fa.CobradoCaja + fa.ACobrarObraSocial) / fa.CantidadTurnos AS DECIMAL(18,2))
                 ELSE 0.00
            END AS TicketPromedio,
            CASE WHEN tot.GranTotal > 0
                 THEN CAST(ISNULL(fa.CobradoCaja + fa.ACobrarObraSocial, 0.00) * 100.0 / tot.GranTotal AS DECIMAL(5,2))
                 ELSE 0.00
            END AS PorcentajeParticipacion,
            CAST(NULL AS DECIMAL(5,2)) AS PorcentajeCoberturaVigente,
            CAST(NULL AS DECIMAL(5,2)) AS PorcentajeCoberturaAplicada
        FROM Usuarios u
        INNER JOIN Roles r ON u.IdRol = r.IdRol
        LEFT JOIN Facturacion fa ON fa.IdMedico = u.IdUsuario
        CROSS JOIN Total tot
        WHERE (u.IdRol = 1 OR r.Descripcion LIKE '%Médic%' OR r.Descripcion LIKE '%Medic%')
          AND (u.Activo = 1 OR fa.IdMedico IS NOT NULL)
          AND (@IdEspecialidad IS NULL OR fa.IdMedico IS NOT NULL OR EXISTS (
                SELECT 1 FROM MedicosEspecialidades me
                WHERE me.IdUsuario = u.IdUsuario AND me.IdEspecialidad = @IdEspecialidad AND me.Activo = 1
          ))
        ORDER BY Posicion ASC;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@,
@"
CREATE OR ALTER PROCEDURE sp_ReporteGerencialRankingEspecialidades
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @ArancelEspecialidad DECIMAL(18,2),
    @ArancelEmergencia DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @FechaDesde IS NULL SET @FechaDesde = CAST(GETDATE() AS DATE);
        IF @FechaHasta IS NULL SET @FechaHasta = @FechaDesde;

        DECLARE @GranTotal DECIMAL(18,2);

        SELECT @GranTotal = SUM(f.CobradoCaja + f.ACobrarObraSocial)
        FROM dbo.fn_ReporteGerencialTurnosFacturados(@FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia) f;

        SELECT
            ROW_NUMBER() OVER(ORDER BY SUM(f.CobradoCaja + f.ACobrarObraSocial) DESC, COUNT(f.IdTurno) DESC) AS Posicion,
            f.IdEspecialidad AS Id,
            ISNULL(e.Nombre, 'Sin especialidad') AS Nombre,
            CASE WHEN SUM(f.EsEmergencia) = COUNT(f.IdTurno) THEN 'Emergencia / Guardia'
                 WHEN SUM(f.EsEmergencia) = 0 THEN 'Especialidad'
                 ELSE 'Mixta'
            END AS Detalle,
            CAST(NULL AS NVARCHAR(50)) AS Matricula,
            COUNT(f.IdTurno) AS CantidadTurnos,
            SUM(f.CobradoCaja) AS CobradoCaja,
            SUM(f.ACobrarObraSocial) AS ACobrarObraSocial,
            SUM(f.CobradoCaja + f.ACobrarObraSocial) AS TotalFacturado,
            CAST(SUM(f.CobradoCaja + f.ACobrarObraSocial) / COUNT(f.IdTurno) AS DECIMAL(18,2)) AS TicketPromedio,
            CASE WHEN @GranTotal > 0
                 THEN CAST(SUM(f.CobradoCaja + f.ACobrarObraSocial) * 100.0 / @GranTotal AS DECIMAL(5,2))
                 ELSE 0.00
            END AS PorcentajeParticipacion,
            CAST(NULL AS DECIMAL(5,2)) AS PorcentajeCoberturaVigente,
            CAST(NULL AS DECIMAL(5,2)) AS PorcentajeCoberturaAplicada
        FROM dbo.fn_ReporteGerencialTurnosFacturados(@FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia) f
        LEFT JOIN Especialidades e ON f.IdEspecialidad = e.IdEspecialidad
        GROUP BY f.IdEspecialidad, e.Nombre
        ORDER BY Posicion ASC;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@,
@"
CREATE OR ALTER PROCEDURE sp_ReporteGerencialRankingObrasSociales
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @ArancelEspecialidad DECIMAL(18,2),
    @ArancelEmergencia DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @FechaDesde IS NULL SET @FechaDesde = CAST(GETDATE() AS DATE);
        IF @FechaHasta IS NULL SET @FechaHasta = @FechaDesde;

        DECLARE @GranTotal DECIMAL(18,2);

        SELECT @GranTotal = SUM(f.CobradoCaja + f.ACobrarObraSocial)
        FROM dbo.fn_ReporteGerencialTurnosFacturados(@FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia) f;

        SELECT
            ROW_NUMBER() OVER(ORDER BY f.EsObraSocial DESC, SUM(f.CobradoCaja + f.ACobrarObraSocial) DESC, COUNT(f.IdTurno) DESC) AS Posicion,
            f.IdObraSocial AS Id,
            f.ObraSocial AS Nombre,
            CASE WHEN f.EsObraSocial = 1 THEN 'Obra Social / Prepaga' ELSE 'Particular' END AS Detalle,
            CAST(NULL AS NVARCHAR(50)) AS Matricula,
            COUNT(f.IdTurno) AS CantidadTurnos,
            SUM(f.CobradoCaja) AS CobradoCaja,
            SUM(f.ACobrarObraSocial) AS ACobrarObraSocial,
            SUM(f.CobradoCaja + f.ACobrarObraSocial) AS TotalFacturado,
            CAST(SUM(f.CobradoCaja + f.ACobrarObraSocial) / COUNT(f.IdTurno) AS DECIMAL(18,2)) AS TicketPromedio,
            CASE WHEN @GranTotal > 0
                 THEN CAST(SUM(f.CobradoCaja + f.ACobrarObraSocial) * 100.0 / @GranTotal AS DECIMAL(5,2))
                 ELSE 0.00
            END AS PorcentajeParticipacion,
            CAST(f.PorcentajeCoberturaVigente AS DECIMAL(5,2)) AS PorcentajeCoberturaVigente,
            CASE WHEN SUM(f.ArancelBase) > 0
                 THEN CAST(100.0 - SUM(f.CobradoCaja) * 100.0 / SUM(f.ArancelBase) AS DECIMAL(5,2))
                 ELSE 0.00
            END AS PorcentajeCoberturaAplicada
        FROM dbo.fn_ReporteGerencialTurnosFacturados(@FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia) f
        GROUP BY f.EsObraSocial, f.IdObraSocial, f.ObraSocial, f.PorcentajeCoberturaVigente
        ORDER BY Posicion ASC;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
"@,
@"
CREATE OR ALTER PROCEDURE sp_ReporteGerencialFacturacionPorSegmento
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @ArancelEspecialidad DECIMAL(18,2),
    @ArancelEmergencia DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @FechaDesde IS NULL SET @FechaDesde = CAST(GETDATE() AS DATE);
        IF @FechaHasta IS NULL SET @FechaHasta = @FechaDesde;

        DECLARE @GranTotal DECIMAL(18,2);

        SELECT @GranTotal = SUM(f.CobradoCaja + f.ACobrarObraSocial)
        FROM dbo.fn_ReporteGerencialTurnosFacturados(@FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia) f;

        SELECT
            CAST(seg.Orden AS BIGINT) AS Posicion,
            CAST(NULL AS INT) AS Id,
            seg.Nombre,
            seg.Grupo AS Detalle,
            CAST(NULL AS NVARCHAR(50)) AS Matricula,
            COUNT(f.IdTurno) AS CantidadTurnos,
            ISNULL(SUM(f.CobradoCaja), 0.00) AS CobradoCaja,
            ISNULL(SUM(f.ACobrarObraSocial), 0.00) AS ACobrarObraSocial,
            ISNULL(SUM(f.CobradoCaja + f.ACobrarObraSocial), 0.00) AS TotalFacturado,
            CASE WHEN COUNT(f.IdTurno) > 0
                 THEN CAST(SUM(f.CobradoCaja + f.ACobrarObraSocial) / COUNT(f.IdTurno) AS DECIMAL(18,2))
                 ELSE 0.00
            END AS TicketPromedio,
            CASE WHEN @GranTotal > 0
                 THEN CAST(ISNULL(SUM(f.CobradoCaja + f.ACobrarObraSocial), 0.00) * 100.0 / @GranTotal AS DECIMAL(5,2))
                 ELSE 0.00
            END AS PorcentajeParticipacion,
            CAST(NULL AS DECIMAL(5,2)) AS PorcentajeCoberturaVigente,
            CAST(NULL AS DECIMAL(5,2)) AS PorcentajeCoberturaAplicada
        FROM (VALUES
            (1, 'Emergencia / Guardia', 'Modalidad'),
            (2, 'Especialidad', 'Modalidad'),
            (3, 'Obra Social / Prepaga', 'Cobertura'),
            (4, 'Particular', 'Cobertura')
        ) seg(Orden, Nombre, Grupo)
        LEFT JOIN dbo.fn_ReporteGerencialTurnosFacturados(@FechaDesde, @FechaHasta, @ArancelEspecialidad, @ArancelEmergencia) f
            ON (seg.Orden = 1 AND f.EsEmergencia = 1)
            OR (seg.Orden = 2 AND f.EsEmergencia = 0)
            OR (seg.Orden = 3 AND f.EsObraSocial = 1)
            OR (seg.Orden = 4 AND f.EsObraSocial = 0)
        GROUP BY seg.Orden, seg.Nombre, seg.Grupo
        ORDER BY seg.Orden ASC;

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
