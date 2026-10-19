-- =========================================================================
-- Script: ActualizarTurnosParticularesHistoricos.sql
-- Propósito: Corrige los aranceles históricos de turnos emitidos para
--            pacientes particulares (sin obra social) que fueron grabados
--            con el copago del 30% ($4.500 / $7.500) en lugar del 100%
--            ($15.000 en Especialidad y $25.000 en Emergencia).
-- Base de Datos: dbGestionTurnos / GestionTurnosMedicos
-- =========================================================================

SET NOCOUNT ON;

BEGIN TRANSACTION;

BEGIN TRY
    PRINT '>> Iniciando actualización de turnos históricos particulares...';

    -- 1. Actualización de turnos de consulta / especialidad ($15.000,00)
    DECLARE @ActualizadosEspecialidad INT = 0;

    UPDATE t
    SET t.Monto = 15000.00
    FROM Turnos t
    INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
    LEFT JOIN ObrasSociales os ON p.IdObraSocial = os.IdObraSocial
    WHERE (os.Nombre = 'Particular / Sin Obra Social' 
           OR os.Nombre LIKE '%Particular%' 
           OR p.IdObraSocial IS NULL 
           OR p.IdObraSocial = 1)
      AND t.TipoTurno <> 'Emergencia'
      AND (t.Monto = 4500.00 OR t.Monto IS NULL OR t.Monto = 0.00);

    SET @ActualizadosEspecialidad = @@ROWCOUNT;
    PRINT CONCAT('✓ Turnos de Especialidad corregidos a $15.000,00: ', @ActualizadosEspecialidad);

    -- 2. Actualización de turnos de emergencia / guardia ($25.000,00)
    DECLARE @ActualizadosEmergencia INT = 0;

    UPDATE t
    SET t.Monto = 25000.00
    FROM Turnos t
    INNER JOIN Pacientes p ON t.IdPaciente = p.IdPaciente
    LEFT JOIN ObrasSociales os ON p.IdObraSocial = os.IdObraSocial
    WHERE (os.Nombre = 'Particular / Sin Obra Social' 
           OR os.Nombre LIKE '%Particular%' 
           OR p.IdObraSocial IS NULL 
           OR p.IdObraSocial = 1)
      AND t.TipoTurno = 'Emergencia'
      AND (t.Monto = 7500.00 OR t.Monto IS NULL OR t.Monto = 0.00);

    SET @ActualizadosEmergencia = @@ROWCOUNT;
    PRINT CONCAT('✓ Turnos de Emergencia corregidos a $25.000,00: ', @ActualizadosEmergencia);

    COMMIT TRANSACTION;
    PRINT '>> Actualización histórica finalizada exitosamente.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT CONCAT('ERROR durante la actualización: ', ERROR_MESSAGE());
    THROW;
END CATCH;
GO
