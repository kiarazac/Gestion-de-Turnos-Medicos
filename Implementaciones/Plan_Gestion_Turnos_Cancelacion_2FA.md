# Plan de Implementación: Visualización de Horarios Ocupados, Detalle de Turno y Cancelación con Doble Factor (2FA)

## Descripción del Objetivo
En el formulario de turnos de especialidades (`FrmTurnoEspecialidad`), anteriormente los horarios ya asignados desaparecían del desplegable `cmbHorarios`.
Con esta implementación:
1. **Visibilidad Total de Horarios**: El desplegable de horarios muestra todos los horarios de atención con distintivos claros:
   - `XX:XX (Disponible)` para horarios libres.
   - `XX:XX [OCUPADO - NroTurno]` para horarios reservados.
2. **Acceso al Detalle del Turno**: Al seleccionar un turno ocupado, se despliega la información del paciente (Nombre, DNI, Obra Social), el número de turno, el estado y el horario.
3. **Generación de Clave Alfanumérica 2FA**: Al crear un turno por especialidad, el sistema genera automáticamente un código alfanumérico seguro (ej. `CAN-7X8K`), el cual se almacena en la base de datos y se imprime en el comprobante `.txt` del paciente.
4. **Cancelación Segura por Doble Factor**: Un turno ocupado solo puede cancelarse si el operador/paciente ingresa la palabra clave alfanumérica exacta emitida en el comprobante. Al cancelarse, el turno pasa a estado `Cancelado` (`Activo = 0`), liberando el cupo horario inmediatamente para una nueva reserva.
5. **Procedimiento Almacenado Integral**: Se utiliza el procedimiento `sp_ObtenerHorariosConEstado` que reúne la matriz de horarios estándar con el estado de ocupación y los datos del turno asignado.

---

## Esquema de Base de Datos y Stored Procedures
1. **Alteración de Tabla `Turnos`**:
   - Incorporación de la columna `CodigoCancelacion NVARCHAR(50) NULL`.
2. **`sp_CrearTurnoEspecialidad`**:
   - Acepta `@CodigoCancelacion NVARCHAR(50) = NULL` y lo almacena en `Turnos`.
3. **`sp_ObtenerHorariosConEstado`**:
   - Devuelve todos los horarios estándar (`08:30`, `09:00`, `09:30`, `10:00`, `10:30`, `11:00`, `14:00`, `14:30`, `15:00`, `16:00`).
   - `LEFT JOIN` con `Turnos` (`Activo = 1 AND Estado <> 'Cancelado'`).
   - Retorna: `Horario`, `EstaDisponible`, `IdTurno`, `NroOrden`, `Paciente`, `Dni`, `ObraSocial`, `Estado`, `CodigoCancelacion`.
4. **`sp_CancelarTurnoEspecialidad`**:
   - Parámetros: `@IdTurno INT`, `@CodigoCancelacion NVARCHAR(50)`.
   - Valida existencia, estado activo y coincidencia exacta del código alfanumérico (2FA).
   - Actualiza `Estado = 'Cancelado'`, `Activo = 0`, `FechaBaja = GETDATE()`, `FechaModificacion = GETDATE()`.

---

## Arquitectura en Capas Involucrada
- **Modelos (`Modelos.cs`)**: Propiedad `CodigoCancelacion` en clase `Turno`.
- **DTOs (`ResultadosSQL/`)**:
  - `HorarioDisponibleDTO`: Enriquecido con propiedades de turno ocupado y método de visualización `TextoDisplay`.
  - `DatosComprobanteTurno`: Incorpora `CodigoCancelacion` en el formato impreso `.txt`.
- **Capa de Datos (`TurnoDAL`)**:
  - `ObtenerHorariosDisponibles`: Ejecuta `sp_ObtenerHorariosConEstado`.
  - `CrearTurnoEspecialidad`: Persiste el `codigoCancelacion`.
  - `CancelarTurnoEspecialidad`: Ejecuta `sp_CancelarTurnoEspecialidad`.
- **Capa de Negocio (`TurnoBLL`)**:
  - `GenerarCodigoCancelacion()`: Crea códigos alfanuméricos tipo `CAN-XXXX`.
  - `CrearTurnoEspecialidad(...)`: Coordina la generación y persistencia del código 2FA.
  - `CancelarTurnoEspecialidad(...)`: Valida parámetros y coordina la cancelación con DAL.
- **Capa de Presentación (`FrmTurnoEspecialidad`)**:
  - Interfaz visual con panel de detalle de turno ocupado y sección de ingreso de clave 2FA para cancelación.
