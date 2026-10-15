# Gestión de Turnos Médicos

Sistema integral para la administración clínica, turnos médicos, triage de guardia, control de consultorios y llamado en sala de espera, desarrollado en **C# (.NET 10 / Windows Forms)** bajo una estricta **arquitectura en tres capas** (`UI -> BLL -> DAL -> SQL Server`).

---

## 🏛️ Arquitectura del Sistema

El sistema implementa una separación rigurosa de responsabilidades:

1. **Capa de Presentación (UI - Windows Forms)**:
   - Captura eventos e interactúa con el usuario final.
   - **No** accede a la base de datos ni a la Capa de Datos (`DAL`). Se comunica exclusivamente mediante la Capa de Negocio (`BLL`) consumiendo objetos DTO tipados.
2. **Capa de Lógica de Negocio (BLL)**:
   - Ubicada en el espacio de nombres `Gestion_de_Turnos_Medicos.Negocio`.
   - Aplica validaciones de reglas de negocio, integridad de estados y seguridad de credenciales:
     - **Hasheo Centralizado SHA-256:** `UsuarioBLL` computa el hash SHA-256 (`Seguridad.HashearContrasenia`) en la capa de negocio antes de invocar a `sp_ValidarLogin`, `sp_InsertarUsuario` y `sp_ModificarUsuario`. De igual manera, `BackupBLL.ValidarDobleAutorizacion` hashea en BLL antes de validar credenciales administrativas. Las claves existentes en `dbGestionTurnos` fueron migradas a sus hashes SHA-256 correspondientes.
   - Orquesta flujos entre múltiples entidades antes de delegar la persistencia.
3. **Capa de Acceso a Datos (DAL)**:
   - Ubicada en el espacio de nombres `Gestion_de_Turnos_Medicos.CapaDeDatos`.
   - Utiliza exclusivamente **Entity Framework Core** (`dbTurnosMedicos`) mediante `SqlQueryRaw` y `ExecuteSqlRaw`.
   - Invoca Stored Procedures de forma parametrizada y atómica preservando la integridad de esquemas.
4. **Base de Datos (SQL Server)**:
   - Base de datos relacional `dbGestionTurnos` sobre `localhost\SQLEXPRESS`.
   - Lógica de persistencia encapsulada en Stored Procedures documentados en `README_StoredProcedures.md`.

---

## 🚀 Módulos Principales

### 1. Gestión de Personal y Usuarios (`FrmGestionUsuarios2`)
- **Formulario Oficial Activo:** Es el formulario predeterminado ejecutado desde el menú de administración (`FrmAdmin`).
- **Flujo Guiado por DNI:**
  - Solicita en primer término el DNI del profesional o empleado.
  - **Modo Modificación:** Si el DNI ya está registrado en `dbGestionTurnos`, precarga automáticamente datos de contacto, rol, matrícula, especialidades asignadas y consultorios vinculados.
  - **Modo Alta:** Si el DNI no existe, habilita los campos limpios bloqueando duplicaciones accidentales.
- **Asignación Múltiple Atómica:** Permite seleccionar y reasignar múltiples salas (`DetallesSalas`) y especialidades médicas (`MedicosEspecialidades`) sin alterar los Stored Procedures originales de la base de datos.
- **Actualización de Claves:** Permite blanquear o modificar la contraseña del usuario con hasheo seguro (`Seguridad.HashearContrasenia`), o conservarla intacta si el campo se deja vacío.

### 2. Gestión de Salas y Consultorios (`FrmSalasAdmin`)
- **Administración Operativa:** Alta, baja lógica y modificación de consultorios físicos.
- **Edición Interactiva y por Formulario:** Admite edición directa sobre las celdas del DataGrid (`CellEndEdit`) y mediante selección interactiva (`CellClick` / `SelectionChanged`).
- **Estados Operativos Soportados:** `Disponible`, `Libre`, `Ocupada`, `En Mantenimiento` y `Cerrada`.
- **Reasignación de Personal:** Asignación y desasignación de médicos responsables del consultorio mediante `sp_AsignarSalaMedico`.

### 3. Gestión de Especialidades (`FrmGestionEspecialidades`)
- Catálogo de especialidades médicas (Clínica General, Pediatría, Traumatología, etc.) con altas y bajas lógicas.

### 4. Módulo de Atención y Turnos
- **`FrmTurnoEmergencia`:** Admite triage con escala de gravedad Manchester / prioridades y síntomas del paciente.
  - **Selector de Obra Social (`ComboBox`):** Sustituye la entrada de texto libre por un desplegable de selección obligatoria (`cmbObraSocial`) alimentado desde `ObrasSociales` mediante `ObraSocialBLL.ObtenerObrasSociales()`. Lista obras sociales de Argentina activas en Corrientes (IOSCOR, PAMI, ISSUNE, OSDE, Swiss Medical, OSECAC, Sancor Salud, Medifé, Galeno, OSPRERA, Unión Personal, OSDEPYM, OSUTHGRA, UOCRA, SPS Salud, Jerárquicos Salud, Prevención Salud) y la opción "Particular / Sin Obra Social".
  - **Detección Automática por DNI:** Al ingresar el DNI del paciente, si ya se encuentra registrado, selecciona y bloquea automáticamente su obra social vinculada.
  - **Carga dinámica:** Consume el catálogo de síntomas activos desde `sp_ObtenerSintomas` mediante `TurnoBLL.ObtenerSintomas()` separando síntomas de gravedad **Alta** y **Media**.
  - **Regla de Mayor Gravedad:** En caso de que se seleccionen múltiples síntomas de diferente severidad (ej. un síntoma de gravedad Alta junto con síntomas de gravedad Media o la opción "Otro"), el sistema garantiza que la prioridad asignada al turno corresponda a la del **síntoma con la gravedad más alta** (`1 = Alta`, `2 = Media`, `3 = Baja`).
  - **Persistencia atómica:** Persiste todos los síntomas seleccionados en la tabla `TurnoSintomas` (`sp_GuardarTurnoSintoma`).
  - **Retroalimentación visual:** Informa en pantalla y mediante código de color el nivel de prioridad resultante (Rojo para Alta, Naranja para Media, Verde para Baja).
  - **Numeración Correlativa Diaria y Unicidad de Turnos:** Generación secuencial diaria (`E-001`, `E-002`...) que impide colisiones entre turnos no atendidos (`Activo = 1 AND Estado NOT IN ('Atendido', 'Cancelado')`), tanto a nivel de SP (`sp_CrearTurnoEmergencia`, `sp_InsertarTurno`) como en la capa BLL (`ExisteNroOrdenNoAtendido`).
  - **Cancelación Ágil con Doble Factor (2FA):**
    - Al emitir cada turno de guardia se autogenera una clave de seguridad (`CAN-XXXX`), persistida en `Turnos.CodigoCancelacion` y exhibida tanto en pantalla como en el comprobante `.txt`.
    - En el propio formulario se incorpora el panel de cancelación directa en 1 solo paso: búsqueda instantánea por N° de orden (`E-001`) o DNI del paciente (`sp_BuscarTurnoActivoEmergencia`).
    - Al ingresar la clave 2FA y pulsar *"CANCELAR TURNO (2FA)"*, `sp_CancelarTurnoEmergencia` valida la autenticidad y pasa el turno a estado `Cancelado` (`Activo = 0`), retirándolo de la guardia y sala de espera.
- **`FrmTurnoEspecialidad`:** Programación y gestión integral de turnos por especialidad médica.
  - **Numeración Correlativa y Control de Unicidad:** Asignación secuencial (`[Prefijo]-001`, `[Prefijo]-002`...) por especialidad y fecha de atención, garantizando que no se repitan identificadores entre turnos activos no atendidos.
  - **Selector Desplegable de Obra Social:** Incorpora el mismo selector controlado (`cmbObraSocial`) con sincronización por DNI, garantizando consistencia relacional y eliminando errores de tipeo.
  - **Matriz de Horarios y Disponibilidad Visible:** El selector desplegable de horarios exhibe la totalidad de franjas horarias de atención distinguiendo claramente las vacantes (`XX:XX (Disponible)`) de las asignadas (`XX:XX [OCUPADO - NroTurno]`) mediante `sp_ObtenerHorariosConEstado`.
  - **Consulta de Detalle de Turno Ocupado:** Al seleccionar un horario ocupado, se despliega en tiempo real la ficha de la cita: nombre y apellido del paciente, DNI, obra social, número de orden y estado.
  - **Cancelación con Doble Factor (2FA):**
    - Al emitir un turno, se genera una palabra clave alfanumérica única (`CAN-XXXX`), la cual se persiste en la base de datos (`Turnos.CodigoCancelacion`) y se imprime en el comprobante descargable `.txt`.
    - La cancelación solo se autoriza tras ingresar la clave 2FA exacta emitida en el comprobante.
    - Al confirmarse la cancelación (`sp_CancelarTurnoEspecialidad`), el turno pasa a estado `Cancelado` (`Activo = 0`), liberando de inmediato la franja horaria para nuevas reservas.
- **`FrmListaTurnos`:** Tablero y contadores de turnos en espera.
  - **Refresco Automático Continuo:** Dispone de un `Timer` en segundo plano (intervalo de 5 segundos) que refresca de forma silenciosa la cola de emergencias y los turnos de la especialidad seleccionada preservando el índice seleccionado y contadores sin parpadeos visuales.
- **`FrmListaTurnosAtencion`:** Monitor de consultorio para el médico:
  - **Filtrado Exclusivo por Fecha de Hoy:** `sp_ListarTurnosAtencion` filtra estrictamente los turnos pertenecientes a la fecha en curso (`CAST(COALESCE(t.Fecha, t.FechaCreacion) AS DATE) = CAST(GETDATE() AS DATE)`).
  - **Visualización de Sintomatología de Emergencia al Iniciar Atención:** Al iniciar la atención de un turno del sector emergencia (`btnIniciarAtencion`), el médico visualiza inmediatamente la sintomatología registrada en triage (`sp_ObtenerSintomasPorTurno` / `TurnoBLL.ObtenerSintomasTextoTurno`) tanto en el encabezado del panel (`lblInfoMotivo`) como en el visor de historia clínica previa (`txtHistoriaPrevia`), antes de proceder a la evolución diagnóstica.
  - **Botón "No se presentó" y Estado `Ausente`:** Incorpora el botón interactivo *"❌  No se presentó"* (`btnNoSePresento`), el cual solicita confirmación médica y ejecuta `sp_MarcarTurnoAusente`, estableciendo el turno en estado `Ausente` y liberando el puesto para continuar con el siguiente paciente.
  - **Máquina de Estados de Origen Estricta y Validación de `Activo = 1`:**
    - `sp_LlamarSiguienteTurno`: Exige que el turno se encuentre en estado de origen `'En Espera'` y `Activo = 1` (error 50081 ante estado incompatible).
    - `sp_IniciarAtencionTurno`: Exige que el turno haya sido llamado previamente (`'Llamado'`) y `Activo = 1` (error 50083 ante estado incompatible).
    - `sp_FinalizarAtencionTurno`: Exige que el turno se encuentre en consulta médica (`'En Consulta'`) y `Activo = 1` (error 50087 ante estado incompatible).
  - **Refresco Automático de la Cola del Médico:** Temporizador automático de 5 segundos (`tmrRefrescoCola`) que actualiza la cola de turnos en espera mientras el consultorio se encuentra en estado `SinPaciente`, garantizando que el profesional visualice de inmediato a los pacientes recién ingresados.
- **`MisSalas_PM`:** Panel para que el médico autenticado gestione la apertura y cierre de sus consultorios designados.

> [!NOTE]
> **Estructura de Base de Datos:**
> La estructura física de la base de datos `dbGestionTurnos` (tablas, columnas y relaciones foráneas) se mantiene intacta. La tabla `Prioridades` (`1 = Alta`, `2 = Media`, `3 = Baja`) y los Stored Procedures existentes (`sp_CrearTurnoEmergencia`, `sp_ObtenerSintomas`, `sp_ObtenerGravedadSintoma` y `sp_GuardarTurnoSintoma`) no requirieron alteraciones de esquema DDL, operando en total compatibilidad con la jerarquía de triage implementada.

### 5. Pantalla Pública de Sala de Espera (`FrmUsuarioVentana`)
- Monitor visual a pantalla completa (o visor incrustable) para pacientes en sala de espera, con actualización en tiempo real de llamados activos a consultorios y estado de la guardia.

### 6. Módulos de Reportería Especializada por Perfil
- **`FrmReporteGuardiaAdmin` (Reporte Operativo de Guardia):** Tablero integral de triage y urgencias con tarjetas de KPIs (Total, Alta, Media, Baja, Tasa de Resolución), colorimetría dinámica por severidad de triage, ranking de síntomas predominantes, búsqueda en vivo y exportación a `.xlsx`, `.csv` y `.txt`.
- **`FrmMisAtenciones` (Historial y Reportería Especializada del Personal Médico):** Módulo individualizado según el perfil médico:
  - **Médicos Especialistas:** Auditoría de atenciones, detalle individualizado de diagnósticos, evoluciones y prescripciones farmacológicas con exportación oficial a PDF.
  - **Médicos Clínicos (Urgencias):** Incluye pestaña interactiva de **Triage Clínico** con ranking y distribución de pacientes atendidos por severidad (`Alta`, `Media`, `Baja`) y ranking epidemiológico de sintomatología clínica atendida, integrando apéndice estadístico en la exportación a PDF.
- **Administrador Técnico:** Sin facultades de reportería médica ni gerencial, enfocado en administración de usuarios, salas, especialidades y copias de seguridad.

### 7. Servicios de Exportación Corporativa (`ClosedXML`)
- **`Servicios/ExportadorExcel.cs`:** Servicio centralizado que confecciona libros nativos de Microsoft Excel (`.xlsx`) mediante OpenXML (`ClosedXML`), aplicando paleta corporativa médica, colores semánticos Manchester (Rojo, Amarillo, Verde), bordes sutiles, auto-ajuste inteligente de columnas y formato numérico tipado sin recortes visuales.

---

## 📋 Documentación Técnica Complementaria

- **[README_Arquitectura_en_capas.md](file:///c:/Users/USUARIO/source/repos/Gestion%20de%20Turnos%20Medicos/README_Arquitectura_en_capas.md):** Especificación completa de directivas arquitectónicas, catálogo de DTOs y matriz Form ↔ BLL ↔ DAL ↔ SP.
- **[README_StoredProcedures.md](file:///c:/Users/USUARIO/source/repos/Gestion%20de%20Turnos%20Medicos/README_StoredProcedures.md):** Catálogo formal de todos los Stored Procedures existentes en `dbGestionTurnos`, parámetros exactos y ejemplos de ejecución.

---

## ⚙️ Compilación y Ejecución

```bash
# Compilar la solución completa
dotnet build "Gestion de Turnos Medicos.sln"

# Ejecutar la aplicación
dotnet run --project "Gestion de Turnos Medicos.csproj"
```