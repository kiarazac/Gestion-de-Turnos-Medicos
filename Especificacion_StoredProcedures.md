# Especificación de Procedimientos Almacenados (Stored Procedures)

Este documento detalla los Stored Procedures requeridos por los formularios de la aplicación C# WinForms (`Gestion_de_Turnos_Medicos`), para ser implementados en la base de datos SQL Server.

---

## 1. `sp_GuardarPaciente`

- **Objetivo**: Registra o actualiza los datos básicos del paciente y devuelve su identificador único (`IdPaciente`).
- **Tablas involucradas**: `Pacientes`
- **Lógica sugerida**:
  - Buscar si existe un paciente con el mismo `@Dni`.
  - Si existe: actualizar sus datos (Nombre, Apellido, ObraSocial) si hubiese cambios y retornar su `IdPaciente`.
  - Si no existe: realizar un `INSERT` en la tabla `Pacientes` y retornar el nuevo `IdPaciente` (mediante `SCOPE_IDENTITY()`).

### Parámetros:
| Parámetro | Tipo de Dato | Dirección | Descripción |
| :--- | :--- | :--- | :--- |
| `@Nombre` | `VARCHAR(100)` | IN | Nombre de pila del paciente. |
| `@Apellido` | `VARCHAR(100)` | IN | Apellido del paciente. |
| `@Dni` | `VARCHAR(20)` | IN | Documento Nacional de Identidad del paciente. |
| `@ObraSocial` | `VARCHAR(100)` | IN | Obra social o cobertura médica del paciente. |
| `@IdPaciente` | `INT` | OUT | Identificador único generado o recuperado del paciente. |

---

## 2. `sp_CrearTurno`

- **Objetivo**: Da de alta un nuevo turno médico (en este caso para el sector de Emergencia / Guardia), vinculando al paciente y determinando su prioridad inicial y número correlativo de atención.
- **Tablas involucradas**: `Turnos`, `Prioridades`, `Especialidades`
- **Lógica sugerida**:
  - Obtener el `IdPrioridad` correspondiente al texto `@Prioridad` ('ALTA', 'MEDIA', 'BAJA').
  - Obtener el `IdEspecialidad` correspondiente a Guardia/Emergencia (o el servicio correspondiente).
  - Generar el número de orden `@NroOrden` (ejemplo correlativo: `'E-001'`, `'E-002'`, etc., o correlativo diario).
  - Insertar el nuevo registro en la tabla `Turnos` con `Fecha = CAST(GETDATE() AS DATE)`, `Horario = CAST(GETDATE() AS TIME)`, `Estado = @Estado` (ej. `'En Espera'`), `TipoTurno = @TipoTurno` (ej. `'Emergencia'`), `IdPaciente`, `IdPrioridad` e `IdEspecialidad`.
  - Retornar el `@IdTurno` generado (`SCOPE_IDENTITY()`) y el `@NroOrden`.

### Parámetros:
| Parámetro | Tipo de Dato | Dirección | Descripción |
| :--- | :--- | :--- | :--- |
| `@IdPaciente` | `INT` | IN | ID del paciente (obtenido previamente de `sp_GuardarPaciente`). |
| `@Prioridad` | `VARCHAR(50)` | IN | Nivel de urgencia calculado ('ALTA', 'MEDIA', 'BAJA'). |
| `@TipoTurno` | `VARCHAR(50)` | IN | Tipo de atención ('Emergencia' o 'Especialidad'). |
| `@Estado` | `VARCHAR(50)` | IN | Estado inicial del turno (por defecto 'En Espera'). |
| `@IdTurno` | `INT` | OUT | ID numérico del turno insertado. |
| `@NroOrden` | `VARCHAR(20)` | OUT | Código de llamado del turno visible para el paciente (ej: 'E-046'). |

---

## 3. `sp_RegistrarTurnoSintoma`

- **Objetivo**: Asocia un síntoma o condición clínica seleccionada en el triage de emergencia al turno recién creado.
- **Tablas involucradas**: `Turno_Sintoma`, `Sintomas`
- **Lógica sugerida**:
  - Buscar el `IdSintoma` en la tabla `Sintomas` a través de `@DescripcionSintoma` (o insertarlo si no existe).
  - Insertar en la tabla intermedia `Turno_Sintoma` (`IdTurno`, `IdSintoma`, `EstadoActual`).

### Parámetros:
| Parámetro | Tipo de Dato | Dirección | Descripción |
| :--- | :--- | :--- | :--- |
| `@IdTurno` | `INT` | IN | ID del turno al que pertenece el síntoma. |
| `@DescripcionSintoma` | `VARCHAR(100)` | IN | Nombre/descripción del síntoma o condición (ej: 'Dolor de Pecho (ALTA)', 'Fiebre Alta (MEDIA)', etc.). |
| `@EstadoActual` | `VARCHAR(50)` | IN | Estado del síntoma al ingreso (ej: 'Presente', 'Activo'). |
