# Especificación de Procedimientos Almacenados (Stored Procedures)

Este documento centraliza la especificación de todos los Stored Procedures requeridos por los formularios de la aplicación WinForms (`Gestion_de_Turnos_Medicos`), para ser creados o mantenidos en SQL Server (contenedor Docker).

---

## FrmLogin

### 1. `sp_ValidarUsuario`
- **Descripción**: Valida las credenciales de acceso del usuario (correo y contraseña) y retorna el nombre del rol asignado si coincide, o `NULL` si no es válido.
- **Tablas involucradas**: `Usuarios`, `Roles`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Correo` | `VARCHAR(100)` | IN | Correo electrónico ingresado. |
  | `@Clave` | `VARCHAR(100)` | IN | Contraseña ingresada. |
  | `@Rol` | `VARCHAR(50)` | OUT | Nombre del Rol obtenido (ej. 'Administrador', 'Personal Médico', 'Recepcionista'). |

---

## FrmTurnoEmergencia

### 1. `sp_ListarObrasSociales`
- **Descripción**: Consulta el catálogo de obras sociales y empresas prepagas activas en Argentina con presencia operativa en Corrientes para poblar los controles `ComboBox` desplegables de selección de cobertura.
- **Nota de reutilización**: Este procedimiento es **reutilizado** por `FrmTurnoEmergencia` y `FrmTurnoEspecialidad`.
- **Tablas involucradas**: `ObrasSociales`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IncluirInactivas` | `BIT` | IN | Si es 1 lista también obras sociales dadas de baja lógica. |

### 2. `sp_GuardarPaciente`
- **Descripción**: Busca al paciente por su DNI. Si ya existe, actualiza sus datos básicos, su cobertura (`IdObraSocial`) y su `Telefono` (solo si se envía no vacío) y retorna su `IdPaciente`. Si no existe, lo inserta en `Pacientes` y devuelve el nuevo `IdPaciente`.
- **Nota de reutilización**: Este procedimiento es **reutilizado** por `FrmTurnoEspecialidad`.
- **Tablas involucradas**: `Pacientes`, `ObrasSociales`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Nombre` | `VARCHAR(100)` | IN | Nombre del paciente. |
  | `@Apellido` | `VARCHAR(100)` | IN | Apellido del paciente. |
  | `@Dni` | `VARCHAR(20)` | IN | Número de documento del paciente. |
  | `@IdObraSocial` | `INT` | IN | Clave foránea de la obra social seleccionada. |
  | `@Telefono` | `NVARCHAR(20)` | IN | Teléfono de contacto (opcional, `NULL` por defecto). |
  | `@IdPaciente` | `INT` | OUT | ID único autoincremental del paciente. |

### 3. `sp_CrearTurno`
- **Descripción**: Registra un turno de urgencia en la tabla `Turnos`, asociando la prioridad calculada ('ALTA', 'MEDIA', 'BAJA') y el servicio de Guardia/Emergencia. Genera el código correlativo de turno (ej. `'E-001'`).
- **Tablas involucradas**: `Turnos`, `Prioridades`, `Especialidades`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdPaciente` | `INT` | IN | ID del paciente generado previamente. |
  | `@Prioridad` | `VARCHAR(50)` | IN | Urgencia calculada por triage ('ALTA', 'MEDIA', 'BAJA'). |
  | `@TipoTurno` | `VARCHAR(50)` | IN | Tipo de turno (ej. 'Emergencia'). |
  | `@Estado` | `VARCHAR(50)` | IN | Estado inicial (por defecto 'En Espera'). |
  | `@IdTurno` | `INT` | OUT | ID autoincremental del turno insertado. |
  | `@NroOrden` | `VARCHAR(20)` | OUT | Código de orden visible para el llamado (ej. 'E-046'). |

### 3. `sp_RegistrarTurnoSintoma`
- **Descripción**: Asocia un síntoma o condición del paciente al turno generado, insertando el registro en la tabla intermedia `Turno_Sintoma`.
- **Tablas involucradas**: `Turno_Sintoma`, `Sintomas`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno generado. |
  | `@DescripcionSintoma` | `VARCHAR(100)` | IN | Descripción del síntoma tildado en el formulario. |
  | `@EstadoActual` | `VARCHAR(50)` | IN | Estado del síntoma al ingresar (ej. 'Presente'). |

### 4. `sp_BuscarTurnoActivoEmergencia`
- **Descripción**: Localiza un turno de guardia (Emergencia) activo (`En Espera` o `Llamado`) para cancelación ágil, filtrando por Número de Orden (ej. `E-001`) o por DNI del paciente en un solo paso.
- **Tablas involucradas**: `Turnos`, `Pacientes`, `Prioridades`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Termino` | `NVARCHAR(50)` | IN | Número de orden o DNI del paciente. |
- **Devuelve**: `IdTurno`, `NroOrden`, `Apellido`, `Nombre`, `Dni`, `Prioridad`, `Hora`, `Estado`, `CodigoCancelacion`.

### 5. `sp_CancelarTurnoEmergencia`
- **Descripción**: Realiza la baja lógica del turno de guardia (`Estado = 'Cancelado'`, `Activo = 0`), retirándolo de la guardia tras validar de forma estricta la palabra clave alfanumérica 2FA (`CAN-XXXX`) emitida en el ticket.
- **Tablas involucradas**: `Turnos`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | Identificador único del turno a cancelar. |
  | `@CodigoCancelacion` | `NVARCHAR(50)` | IN | Clave alfanumérica de seguridad 2FA emitida en el comprobante. |
- **Devuelve**: `IdTurnoCancelado`, `Mensaje`.

---

## FrmTurnoEspecialidad

### 1. `sp_ObtenerEspecialidades`
- **Descripción**: Obtiene la lista de especialidades médicas activas para poblar los selectores de los formularios.
- **Nota de reutilización**: Este procedimiento es **reutilizado** por `FrmListaTurnos`.
- **Tablas involucradas**: `Especialidades`
- **Parámetros**: Ninguno. Retorna un conjunto de resultados con `Nombre` (y opcionalmente `IdEspecialidad`).

### 2. `sp_ObtenerHorariosConEstado`
- **Descripción**: Retorna todos los horarios estándar de atención médica (libres y ocupados) junto a su estado de disponibilidad (`EstaDisponible`) y los detalles del turno asignado (paciente, DNI, obra social, teléfono, número de orden y palabra clave 2FA).
- **Tablas involucradas**: `Turnos`, `Especialidades`, `Pacientes`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@NombreEspecialidad` | `VARCHAR(100)` | IN | Nombre de la especialidad seleccionada. |
  | `@Fecha` | `DATE` | IN | Fecha consultada en el calendario. |

### 3. `sp_CrearTurnoEspecialidad`
- **Descripción**: Registra un turno programado de consulta por especialidad, vinculando paciente, especialidad, fecha, horario asignado y la palabra clave/código alfanumérico 2FA de cancelación.
- **Tablas involucradas**: `Turnos`, `Especialidades`, `Pacientes`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdPaciente` | `INT` | IN | ID del paciente (obtenido de `sp_GuardarPaciente`). |
  | `@NombreEspecialidad` | `VARCHAR(100)` | IN | Especialidad a la que acude. |
  | `@Fecha` | `DATE` | IN | Fecha del turno. |
  | `@Horario` | `VARCHAR(10)` | IN | Horario asignado (ej. '10:30'). |
  | `@Estado` | `VARCHAR(50)` | IN | Estado inicial ('En Espera'). |
  | `@CodigoCancelacion` | `VARCHAR(50)` | IN | Palabra clave alfanumérica (2FA) para cancelación. |
  | `@IdTurno` | `INT` | OUT | ID autoincremental del turno insertado. |
  | `@NroOrden` | `VARCHAR(20)` | OUT | Código de llamado correlativo (ej. 'C-015'). |

### 4. `sp_CancelarTurnoEspecialidad`
- **Descripción**: Cancela lógicamente un turno activo (`Activo = 0`, `Estado = 'Cancelado'`), liberando el horario de consulta tras validar estrictamente la palabra clave alfanumérica de doble factor (2FA) emitida en el ticket.
- **Tablas involucradas**: `Turnos`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | Identificador del turno a cancelar. |
  | `@CodigoCancelacion` | `VARCHAR(50)` | IN | Palabra clave alfanumérica del comprobante para verificación 2FA. |

### 5. `sp_VerificarCoberturaEspecialidad`
- **Descripción**: Indica si la especialidad tiene al menos un médico activo, con la especialidad asignada, sin ausencia en la fecha. Si no, la fecha queda bloqueada para turnos nuevos (también lo valida `sp_CrearTurnoEspecialidad`, error `50100`).
- **Tablas involucradas**: `Especialidades`, `MedicosEspecialidades`, `Usuarios`, `AusenciasMedicas` (vía `fn_EspecialidadSinCobertura`)
- **Parámetros**: `@NombreEspecialidad` (`NVARCHAR(100)`, IN), `@Fecha` (`DATE`, IN). Retorna: `TieneCobertura`.

### 6. `sp_ListarTurnosAReprogramar`
- **Descripción**: Turnos de especialidad 'En Espera' (desde hoy) en fechas sin profesional disponible, con teléfono del paciente y próximo horario libre. Alimenta el botón de aviso y `FrmTurnosAReprogramar`; un turno cancelado deja de figurar.
- **Tablas involucradas**: `Turnos`, `Especialidades`, `Pacientes`, `ObrasSociales`
- **Parámetros**: Ninguno. Retorna: `IdTurno`, `NroOrden`, `Fecha`, `Horario`, `Especialidad`, `Paciente`, `Dni`, `Telefono`, `ObraSocial`, `ProximoHorarioLibre`.

---

## FrmMisAusencias

### 1. `sp_RegistrarAusencias`
- **Descripción**: Registra un día de ausencia por cada fecha del rango (mínimo desde mañana, máximo 90 días, omite días ya cargados). Solo médicos activos con especialidades.
- **Tablas involucradas**: `AusenciasMedicas`, `Usuarios`, `MedicosEspecialidades`, `Turnos`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT` | IN | Médico que se ausenta. |
  | `@FechaDesde` | `DATE` | IN | Primer día de ausencia. |
  | `@FechaHasta` | `DATE` | IN | Último día de ausencia. |
  | `@Motivo` | `NVARCHAR(200)` | IN | Motivo opcional. |
- Retorna: `DiasRegistrados`, `TurnosSinCobertura`.

### 2. `sp_ListarAusenciasPorMedico`
- **Descripción**: Ausencias vigentes (desde hoy) del médico.
- **Parámetros**: `@IdUsuario` (`INT`, IN). Retorna: `IdAusencia`, `Fecha`, `Motivo`.

### 3. `sp_AnularAusencia`
- **Descripción**: Baja lógica de una ausencia propia con fecha a partir de mañana.
- **Parámetros**: `@IdAusencia` (`INT`, IN), `@IdUsuario` (`INT`, IN).

---

## FrmTurnosAReprogramar

- Usa `sp_ListarTurnosAReprogramar` y `sp_CancelarTurnoEspecialidad` (clave 2FA). "Cancelar y reprogramar" devuelve el turno a `FrmTurnoEspecialidad`, que precarga paciente y especialidad (fecha y horario se eligen manualmente). Mientras la reprogramación esté pendiente, `FrmRecepcionista` impide cambiar de módulo o cerrar sesión; el botón "Cancelar reprogramación" la descarta (el turno original queda cancelado).

---

## FrmSalasGerente

### 1. `sp_ListarSalas`
- **Descripción**: Obtiene todas las salas activas con su estado actual y el personal que tienen asignado.
- **Tablas involucradas**: `Salas`, `detalle_sala`, `Usuarios`
- **Parámetros**: Ninguno. Retorna: `id_sala`, `nombreSala`, `estadoSala`, `personal_asignado`.

### 2. `sp_ListarPersonalMedico`
- **Descripción**: Retorna los médicos activos disponibles para asignar a las salas de atención.
- **Tablas involucradas**: `Usuarios`, `Roles`
- **Parámetros**: Ninguno. Retorna: `IdUsuario`, `NombreCompleto` (ej. 'Dr. Pérez (Cardiología)').

### 3. `sp_GuardarSala`
- **Descripción**: Registra una nueva sala física de consultorio o guardia y asigna al personal médico correspondiente.
- **Tablas involucradas**: `Salas`, `detalle_sala`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@NombreSala` | `VARCHAR(100)` | IN | Nombre identificatorio de la sala (ej. 'Consultorio 1'). |
  | `@EstadoSala` | `VARCHAR(50)` | IN | 'Disponible', 'Ocupada' o 'En Mantenimiento'. |
  | `@PersonalAsignado`| `VARCHAR(255)` | IN | Personal asignado en la sala. |
  | `@IdSala` | `INT` | OUT | ID autoincremental de la sala generada. |

### 4. `sp_DesactivarSala`
- **Descripción**: Realiza la baja lógica (`Activo = 0`, `FechaBaja = GETDATE()`) de la sala indicada.
- **Tablas involucradas**: `Salas`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala a desactivar. |

---

## FrmListaTurnos

### 1. `sp_ListarTurnosEmergencia`
- **Descripción**: Obtiene todos los turnos del sector de emergencia de la jornada para visualización del recepcionista y sala de espera.
- **Tablas involucradas**: `Turnos`, `Prioridades`, `Salas`
- **Parámetros**: Ninguno. Retorna: `Turno`, `Prioridad`, `Hora`, `Estado`, `Sala`.

### 2. `sp_ListarTurnosEspecialidad`
- **Descripción**: Obtiene los turnos registrados para una especialidad seleccionada, incluyendo el teléfono de contacto del paciente (`Telefono`).
- **Tablas involucradas**: `Turnos`, `Especialidades`, `Salas`, `Pacientes`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@NombreEspecialidad` | `VARCHAR(100)` | IN | Nombre de la especialidad seleccionada. |

---

## FrmListaTurnosAtencion

### 1. `sp_ListarTurnosAtencion`
- **Descripción**: Obtiene la cola de turnos en espera para el médico, filtrada o general por servicio/especialidad, con orden prioritario por triage para emergencias y FIFO para turnos programados.
- **Tablas involucradas**: `Turnos`, `Pacientes`, `Prioridades`, `Especialidades`
- **Parámetros**: Ninguno. Retorna: `IdTurno`, `NroOrden`, `Fecha`, `Estado`, `Especialidad`, `Triage`, `NombrePaciente`, `ApellidoPaciente`, `DniPaciente`, `ObraSocial`.

### 2. `sp_LlamarSiguientePaciente`
- **Descripción**: Cambia el estado del turno a 'Llamado' y vincula al médico y consultorio que realizaron el llamado.
- **Tablas involucradas**: `Turnos`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno llamado. |
  | `@NombreMedico` | `VARCHAR(100)` | IN | Nombre del profesional médico que llama. |
  | `@SalaAsignada` | `VARCHAR(100)` | IN | Sala/consultorio donde se atenderá. |

### 3. `sp_IniciarAtencionTurno`
- **Descripción**: Cambia el estado del turno a 'En Consulta' al momento de ingresar el paciente al consultorio.
- **Tablas involucradas**: `Turnos`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno en atención. |

### 4. `sp_FinalizarAtencionTurno`
- **Descripción**: Registra el cierre de la consulta médica (estado 'Atendido'), guardando el diagnóstico y las observaciones en el historial clínico del paciente.
- **Tablas involucradas**: `Turnos`, `Historia_Clinica`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdTurno` | `INT` | IN | ID del turno finalizado. |
  | `@Diagnostico` | `VARCHAR(MAX)` | IN | Diagnóstico u observaciones clínicas ingresadas por el médico. |
  | `@NombreMedico` | `VARCHAR(100)` | IN | Nombre del médico que finalizó la atención. |
  | `@SalaAsignada` | `VARCHAR(100)` | IN | Sala en la que se atendió. |

---

## MisSalas_PM

### 1. `sp_ListarMisSalas`
- **Descripción**: Lista las salas asignadas al personal médico logueado, indicando su estado operativo ('Disponible', 'Ocupada', 'En Mantenimiento').
- **Tablas involucradas**: `Salas`, `detalle_sala`
- **Parámetros**: Ninguno. Retorna: `id_sala`, `nombreSala`, `descripcion_atencion`, `estadoSala`.

## FrmGestionEspecialidades

### 1. `sp_ListarEspecialidades` (Reutilizado)
- **Descripción**: Obtiene la lista de especialidades médicas activas (`Activo = 1`) para poblar la grilla de administración de especialidades y los selectores médicos.
- **Tablas involucradas**: `Especialidad`
- **Parámetros**: Ninguno. Retorna: `IdEspecialidad`, `Nombre`.

### 2. `sp_GuardarEspecialidad`
- **Descripción**: Registra una nueva especialidad médica en la tabla `Especialidad`.
- **Tablas involucradas**: `Especialidad`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Nombre` | `VARCHAR(100)` | IN | Nombre de la especialidad médica. |
  | `@IdEspecialidad` | `INT` | OUT | ID autoincremental de la especialidad registrada. |

### 3. `sp_DesactivarEspecialidad`
- **Descripción**: Realiza la baja lógica (`Activo = 0`, `FechaBaja = GETDATE()`) de la especialidad especificada, y desactiva las relaciones activas en `MedicoEspecialidad`.
- **Tablas involucradas**: `Especialidad`, `MedicoEspecialidad`, `Turno`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdEspecialidad` | `INT` | IN | ID de la especialidad a desactivar. |

