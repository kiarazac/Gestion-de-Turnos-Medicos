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

### 1. `sp_GuardarPaciente`
- **Descripción**: Busca al paciente por su DNI. Si ya existe, actualiza sus datos básicos y retorna su `IdPaciente`. Si no existe, lo inserta en `Pacientes` y devuelve el nuevo `IdPaciente`.
- **Nota de reutilización**: Este procedimiento es **reutilizado** por `FrmTurnoEspecialidad`.
- **Tablas involucradas**: `Pacientes`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Nombre` | `VARCHAR(100)` | IN | Nombre del paciente. |
  | `@Apellido` | `VARCHAR(100)` | IN | Apellido del paciente. |
  | `@Dni` | `VARCHAR(20)` | IN | Número de documento del paciente. |
  | `@ObraSocial` | `VARCHAR(100)` | IN | Cobertura médica u obra social. |
  | `@IdPaciente` | `INT` | OUT | ID único autoincremental del paciente. |

### 2. `sp_CrearTurno`
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

---

## FrmTurnoEspecialidad

### 1. `sp_ObtenerEspecialidades`
- **Descripción**: Obtiene la lista de especialidades médicas activas para poblar los selectores de los formularios.
- **Nota de reutilización**: Este procedimiento es **reutilizado** por `FrmListaTurnos`.
- **Tablas involucradas**: `Especialidades`
- **Parámetros**: Ninguno. Retorna un conjunto de resultados con `Nombre` (y opcionalmente `IdEspecialidad`).

### 2. `sp_ObtenerHorariosDisponibles`
- **Descripción**: Retorna los horarios disponibles (no ocupados) para una especialidad y fecha determinadas.
- **Tablas involucradas**: `Turnos`, `Especialidades`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@NombreEspecialidad` | `VARCHAR(100)` | IN | Nombre de la especialidad seleccionada. |
  | `@Fecha` | `DATE` | IN | Fecha consultada en el calendario. |

### 3. `sp_CrearTurnoEspecialidad`
- **Descripción**: Registra un turno programado de consulta por especialidad, vinculando paciente, especialidad, fecha y horario asignado.
- **Tablas involucradas**: `Turnos`, `Especialidades`, `Prioridades`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdPaciente` | `INT` | IN | ID del paciente (obtenido de `sp_GuardarPaciente`). |
  | `@NombreEspecialidad` | `VARCHAR(100)` | IN | Especialidad a la que acude. |
  | `@Fecha` | `DATE` | IN | Fecha del turno. |
  | `@Horario` | `VARCHAR(10)` | IN | Horario asignado (ej. '10:30'). |
  | `@Estado` | `VARCHAR(50)` | IN | Estado inicial ('En Espera'). |
  | `@IdTurno` | `INT` | OUT | ID autoincremental del turno insertado. |
  | `@NroOrden` | `VARCHAR(20)` | OUT | Código de llamado correlativo (ej. 'T-015'). |

---

## FrmGestionUsuarios

### 1. `sp_ListarUsuarios`
- **Descripción**: Obtiene el listado de todos los usuarios activos del sistema para la grilla de administración.
- **Tablas involucradas**: `Usuarios`, `Roles`, `Medico_especialidad`, `Especialidades`, `detalle_sala`, `Sala`
- **Parámetros**: Ninguno. Retorna: `id_usuario`, `nombre`, `apellido`, `usuario`, `contrasenia`, `dni`, `email`, `telefono`, `sexo`, `rol`, `nro_matricula`, `especialidades`, `sala`.

### 2. `sp_GuardarUsuario`
- **Descripción**: Da de alta o actualiza un usuario en la tabla `Usuarios`. Si el rol es "Personal Médico", registra también su matrícula, especialidades asignadas y sala.
- **Tablas involucradas**: `Usuarios`, `Roles`, `Medico_especialidad`, `detalle_sala`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@Nombre` | `VARCHAR(100)` | IN | Nombre del usuario. |
  | `@Apellido` | `VARCHAR(100)` | IN | Apellido del usuario. |
  | `@Usuario` | `VARCHAR(100)` | IN | Nombre de usuario / login. |
  | `@Contrasenia` | `VARCHAR(100)` | IN | Contraseña. |
  | `@Dni` | `VARCHAR(20)` | IN | DNI del usuario. |
  | `@Email` | `VARCHAR(100)` | IN | Correo electrónico (puede ser NULL). |
  | `@Telefono` | `VARCHAR(50)` | IN | Teléfono de contacto (puede ser NULL). |
  | `@Sexo` | `VARCHAR(20)` | IN | 'Hombre' o 'Mujer'. |
  | `@Rol` | `VARCHAR(50)` | IN | Rol asignado (ej. 'Personal Médico'). |
  | `@NroMatricula` | `VARCHAR(50)` | IN | Matrícula (NULL si no es médico). |
  | `@Especialidades` | `VARCHAR(255)` | IN | Especialidades separadas por comas (NULL si no es médico). |
  | `@Sala` | `VARCHAR(100)` | IN | Sala asignada (NULL si no es médico). |
  | `@IdUsuario` | `INT` | OUT | ID autoincremental del usuario registrado. |

### 3. `sp_DesactivarUsuario`
- **Descripción**: Realiza la baja lógica (`Activo = 0`, `FechaBaja = GETDATE()`) del usuario especificado.
- **Tablas involucradas**: `Usuarios`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdUsuario` | `INT` | IN | ID del usuario a desactivar. |

---

## FrmSalasAdmin

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
- **Descripción**: Obtiene los turnos registrados para una especialidad seleccionada.
- **Tablas involucradas**: `Turnos`, `Especialidades`, `Salas`
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

### 2. `sp_ActualizarEstadoSala`
- **Descripción**: Actualiza el estado de una sala (ej. pasar a 'Disponible' al abrirla o 'En Mantenimiento' al cerrarla).
- **Tablas involucradas**: `Salas`
- **Parámetros**:
  | Parámetro | Tipo | Dirección | Descripción |
  | :--- | :--- | :--- | :--- |
  | `@IdSala` | `INT` | IN | ID de la sala a modificar. |
  | `@NuevoEstado` | `VARCHAR(50)` | IN | Nuevo estado asignado ('Disponible', 'Ocupada', 'En Mantenimiento'). |
