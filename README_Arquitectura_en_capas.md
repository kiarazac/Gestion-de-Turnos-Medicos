# Documentación Técnica: Arquitectura en 3 Capas y Acceso a Datos con EF Core

Este documento establece el diseño arquitectónico oficial, las directivas de desarrollo y la guía de implementación técnica para el proyecto **`Gestion_de_Turnos_Medicos`**.

Define la separación estricta de responsabilidades entre la **Capa de Presentación (UI / Windows Forms)**, la **Capa de Lógica de Negocio (BLL)** y la **Capa de Acceso a Datos (DAL)**, así como el uso exclusivo de **Entity Framework Core** (`ConsultorioContext`) como motor de comunicación con los **Stored Procedures** en SQL Server.

---

## 1. Principios y Directivas Innegociables

1. **Arquitectura estricta de 3 capas (`UI -> BLL -> DAL -> SQL Server`)**:
   - La interfaz gráfica (Forms) **nunca** accede a la base de datos ni a la Capa de Datos (`DAL`). Solo dialoga con la Capa de Negocio (`BLL`).
   - La Capa de Negocio (`BLL`) valida reglas de dominio, orquesta operaciones y delega la persistencia en la Capa de Datos (`DAL`).
   - La Capa de Datos (`DAL`) es la única autorizada a interactuar con el contexto de Entity Framework Core y ejecutar los Stored Procedures.
2. **Entity Framework Core exclusivo (Cero ADO.NET tradicional)**:
   - Se elimina y prohíbe el uso de `Conexion.ObtenerConexion()`, `SqlCommand`, `SqlConnection` y `SqlDataReader` en todo el proyecto.
   - La persistencia se realiza exclusivamente mediante `ConsultorioContext` (derivado de `dbTurnosMedicos`) utilizando:
     - `context.Database.SqlQueryRaw<TDTO>("EXEC sp_...", params)` para consultas, listados y result sets.
     - `context.Database.ExecuteSqlRaw("EXEC sp_...", params)` para altas, modificaciones, bajas y cambios de estado.
3. **Mapeo seguro mediante DTOs (Data Transfer Objects)**:
   - Para toda consulta de Stored Procedure que retorne columnas combinadas, cruces (`JOIN`), campos calculados o nombres de entidades relacionadas, se emplean clases DTO específicas en una carpeta/espacio de nombres dedicado (`DTOs/` o `ResultadosSQL/`), desacoplando la UI de los modelos relacionales puros de Entity Framework.
4. **Respeto absoluto del esquema y Stored Procedures**:
   - No se crean migraciones, tablas, columnas ni índices.
   - El archivo `README_StoredProcedures.md` constituye el contrato formal entre la aplicación C# y SQL Server.

---

## 2. Diagrama Arquitectónico

```text
┌────────────────────────────────────────────────────────────────────────┐
│                        CAPA DE PRESENTACIÓN (UI)                       │
│  Namespace: Gestion_de_Turnos_Medicos                                  │
│  Archivos: FrmLogin, FrmGestionUsuarios2 (Gestor Oficial Usuarios),    │
│            FrmSalasAdmin, FrmGestionEspecialidades,                    │
│            FrmTurnoEmergencia, FrmTurnoEspecialidad, FrmListaTurnos,    │
│            FrmListaTurnosAtencion, MisSalas_PM, FrmAdmin, etc.         │
│                                                                        │
│  Responsabilidades:                                                    │
│  - Captura de eventos visuales (Click, Load, SelectedIndexChanged).   │
│  - Validación visual de entradas (campos vacíos, regex, formatos).     │
│  - Llamada exclusiva a métodos de la Capa BLL.                         │
│  - Binding de DataGridViews y ComboBoxes mediante DTOs recibidos.      │
│  - Feedback visual (MessageBox, colores de triage, mensajes de error). │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │ (Invoca métodos BLL / Recibe DTOs)
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                      CAPA DE LÓGICA DE NEGOCIO (BLL)                   │
│  Namespace: Gestion_de_Turnos_Medicos.Negocio                          │
│  Archivos: UsuarioBLL, SalaBLL, EspecialidadBLL,                       │
│            PacienteBLL, TurnoBLL, HistoriaClinicaBLL                   │
│                                                                        │
│  Responsabilidades:                                                    │
│  - Aplicar reglas de negocio médicas y administrativas.               │
│  - Validaciones de dominio (evitar valores incoherentes, verificar    │
│    estados de salas antes de abrir/cerrar, reglas de triage).          │
│  - Hasheo de contraseñas (Seguridad.HashearContrasenia).               │
│  - Invocar a las clases correspondientes de la Capa DAL.               │
│  - Manejo y transformación de excepciones de negocio.                  │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │ (Invoca métodos DAL / Transfiere datos)
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                      CAPA DE ACCESO A DATOS (DAL)                      │
│  Namespace: Gestion_de_Turnos_Medicos.CapaDeDatos                      │
│  Archivos: UsuarioDAL, SalaDAL, EspecialidadDAL,                       │
│            PacienteDAL, TurnoDAL, HistoriaClinicaDAL                   │
│                                                                        │
│  Responsabilidades:                                                    │
│  - Uso exclusivo de ConsultorioContext (DbContext EF Core).            │
│  - Creación de SqlParameter tipados para evitar inyección SQL.         │
│  - context.Database.SqlQueryRaw<TDTO>("EXEC sp_...", params).ToList() │
│  - context.Database.ExecuteSqlRaw("EXEC sp_...", params)              │
│  - Mapeo de parámetros OUTPUT y retornos de IDs.                       │
│  - Captura de SqlException (códigos de THROW 50001, 50002, etc.).      │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │ (Llamada a Stored Procedures)
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                     BASE DE DATOS (SQL Server / Docker)                │
│  Base: dbGestionTurnos                                                 │
│  Stored Procedures especificados en README_StoredProcedures.md         │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Catálogo de DTOs (Data Transfer Objects)

Los DTOs se ubican en la carpeta `DTOs/` (compartiendo ámbito con `ResultadosSQL`). Permiten proyectar con seguridad los resultados de Stored Procedures que realizan cruces entre tablas:

### 3.1 `UsuarioLoginResult` (Existente en `ResultadosSQL`)
- **Uso**: Retorno de `sp_ValidarLogin`.
- **Propiedades**:
  ```csharp
  public int IdUsuario { get; set; }
  public string Nombre { get; set; }
  public string Apellido { get; set; }
  public string Correo { get; set; }
  public int IdRol { get; set; }
  public string NombreRol { get; set; }
  ```

### 3.2 `UsuarioListadoDTO`
- **Uso**: Retorno de `sp_ListarUsuarios` para la grilla de `FrmGestionUsuarios`.
- **Propiedades**:
  ```csharp
  public int IdUsuario { get; set; }
  public string Nombre { get; set; }
  public string Apellido { get; set; }
  public string Correo { get; set; }
  public string Dni { get; set; }
  public string Telefono { get; set; }
  public string Rol { get; set; }
  public string NroMatricula { get; set; }
  public string Especialidades { get; set; }
  public string Salas { get; set; }
  ```

### 3.3 `RolDTO`
- **Uso**: Retorno de `sp_ListarRoles` para el ComboBox de roles en `FrmGestionUsuarios`.
- **Propiedades**:
  ```csharp
  public int IdRol { get; set; }
  public string Descripcion { get; set; }
  ```

### 3.4 `MedicoDTO`
- **Uso**: Retorno de `sp_ListarPersonalMedico` para el selector de asignación en `FrmSalasAdmin`.
- **Propiedades**:
  ```csharp
  public int IdUsuario { get; set; }
  public string NombreCompleto { get; set; }
  ```

### 3.5 `SalaDTO`
- **Uso**: Retorno de `sp_ObtenerSalas` (`@IdUsuario = NULL` para admin, `@IdUsuario = ID` para médico).
- **Propiedades**:
  ```csharp
  public int IdSala { get; set; }
  public string NombreSala { get; set; }
  public string EstadoSala { get; set; }
  public int? IdUsuario { get; set; }
  public string NombreMedico { get; set; }
  public string ApellidoMedico { get; set; }
  public string DescripcionAtencion { get; set; }
  ```

### 3.6 `EspecialidadDTO`
- **Uso**: Retorno de `sp_ListarEspecialidades` / `sp_ObtenerEspecialidades` para grillas y combos.
- **Propiedades**:
  ```csharp
  public int IdEspecialidad { get; set; }
  public string Nombre { get; set; }
  ```

### 3.7 `SintomaDTO`
- **Uso**: Retorno de `sp_ObtenerSintomas` para el triage dinámico en `FrmTurnoEmergencia`.
- **Propiedades**:
  ```csharp
  public int IdSintoma { get; set; }
  public string Descripcion { get; set; }
  public string Gravedad { get; set; } // 'Alta', 'Media', 'Baja'
  ```

### 3.8 `TurnoListadoDTO`
- **Uso**: Retorno de `sp_ObtenerListaTurnos` y `sp_ListarTurnosEspecialidad`.
- **Propiedades**:
  ```csharp
  public int IdTurno { get; set; }
  public string NroOrden { get; set; }
  public string Estado { get; set; }
  public string TipoTurno { get; set; }
  public DateTime Fecha { get; set; }
  public string NombreEspecialidad { get; set; }
  public string PrioridadTexto { get; set; }
  public int IdPrioridad { get; set; }
  ```

### 3.9 `TurnoEmergenciaDTO`
- **Uso**: Retorno de `sp_ListarTurnosEmergencia` para monitores, grilla de guardia en `FrmListaTurnos` y pantalla de sala de espera en `FrmUsuarioVentana`.
- **Propiedades**:
  ```csharp
  public string Turno { get; set; }
  public string Prioridad { get; set; }
  public string Hora { get; set; }
  public string Estado { get; set; }
  public string Sala { get; set; }
  ```

### 3.10 `TurnoAtencionDTO`
- **Uso**: Retorno de `sp_ListarTurnosAtencion` para la lista de pacientes en espera en `FrmListaTurnosAtencion`.
- **Propiedades**:
  ```csharp
  public int IdTurno { get; set; }
  public string NroOrden { get; set; }
  public DateTime Fecha { get; set; }
  public string Estado { get; set; }
  public string Especialidad { get; set; }
  public string Triage { get; set; }
  public string NombrePaciente { get; set; }
  public string ApellidoPaciente { get; set; }
  public string DniPaciente { get; set; }
  public string ObraSocial { get; set; }
  ```

### 3.11 `TurnoPantallaDTO`
- **Uso**: Retorno de `sp_ObtenerTurnosPantallaPublica`.
- **Propiedades**:
  ```csharp
  public string NroOrden { get; set; }
  public string NombreSala { get; set; }
  public string MedicoApellido { get; set; }
  ```

### 3.12 `HistoriaClinicaDTO`
- **Uso**: Retorno de `sp_ObtenerHistoriaClinicaPaciente`.
- **Propiedades**:
  ```csharp
  public int IdHistoria { get; set; }
  public DateTime Fecha { get; set; }
  public string TipoTurno { get; set; }
  public string DiagRapido { get; set; }
  public string DescripHistoriaClinica { get; set; }
  public string RecetaMedicamentos { get; set; }
  public int IdPaciente { get; set; }
  public int IdTurno { get; set; }
  public int IdUsuario { get; set; }
  public string NombreMedico { get; set; }
  public string ApellidoMedico { get; set; }
  ```

### 3.13 `HorarioDisponibleDTO`
- **Uso**: Retorno de `sp_ObtenerHorariosDisponibles`.
- **Propiedades**:
  ```csharp
  public string Horario { get; set; }
  ```

### 3.14 `TurnoGeneralPantallaDTO`
- **Uso**: Retorno de `sp_ListarTurnosGeneralesPantalla` para la grilla general de la pantalla pública de sala de espera en `FrmUsuarioVentana`.
- **Propiedades**:
  ```csharp
  public string Turno { get; set; }
  public string Hora { get; set; }
  public string Fecha { get; set; }
  public string Especialidad { get; set; }
  public string Estado { get; set; }
  public string Sala { get; set; }
  ```

### 3.15 `ResultadoTurnoDTO`
- **Uso**: Retorno de procedimientos de inserción de turnos (`sp_RegistrarTurnoEmergencia` y `sp_CrearTurnoEspecialidad`) con el ID generado y el número de orden asignado.
- **Propiedades**:
  ```csharp
  public int IdNuevoTurno { get; set; }
  public string? NroOrden { get; set; }
  ```

### 3.16 `UsuarioListadoDTO`
- **Uso**: Retorno de `sp_ListarUsuarios` para la administración de personal y roles en `FrmGestionUsuarios`.
- **Propiedades**:
  ```csharp
  public int IdUsuario { get; set; }
  public string Nombre { get; set; }
  public string Apellido { get; set; }
  public string Correo { get; set; }
  public string Dni { get; set; }
  public string Telefono { get; set; }
  public string Rol { get; set; }
  public string NroMatricula { get; set; }
  public string Especialidades { get; set; }
  public string Salas { get; set; }
  ```

---

## 4. Guía Técnica de Implementación: Capa DAL con Entity Framework Core

Toda clase DAL debe instanciar `ConsultorioContext` (`using (var context = new ConsultorioContext())`). A continuación se detallan los patrones estándar de ejecución:

### 4.1 Patrón 1: Consulta que retorna una lista de DTOs (`SqlQueryRaw`)
Utilizado para listar registros con columnas combinadas o múltiples tablas:

```csharp
public List<SalaDTO> ObtenerSalas(int? idUsuario = null)
{
    using (var context = new ConsultorioContext())
    {
        var paramUsuario = new SqlParameter("@IdUsuario", (object)idUsuario ?? DBNull.Value);

        return context.Database
            .SqlQueryRaw<SalaDTO>("EXEC sp_ObtenerSalas @IdUsuario", paramUsuario)
            .ToList();
    }
}
```

### 4.2 Patrón 2: Ejecución de comando sin retorno de filas (`ExecuteSqlRaw`)
Utilizado para modificaciones, bajas lógicas, asignaciones y cambios de estado:

```csharp
public void EliminarUsuario(int idUsuario)
{
    using (var context = new ConsultorioContext())
    {
        var paramId = new SqlParameter("@IdUsuario", idUsuario);

        context.Database.ExecuteSqlRaw("EXEC sp_EliminarUsuario @IdUsuario", paramId);
    }
}
```

### 4.3 Patrón 3: Inserción que retorna un ID autogenerado (`SCOPE_IDENTITY`)
Utilizado en procedimientos como `sp_InsertarPaciente` o `sp_RegistrarTurnoEmergencia`:

```csharp
public int InsertarPaciente(string nombre, string apellido, string dni, string obraSocial)
{
    using (var context = new ConsultorioContext())
    {
        var pNombre = new SqlParameter("@Nombre", nombre);
        var pApellido = new SqlParameter("@Apellido", apellido);
        var pDni = new SqlParameter("@Dni", dni);
        var pObraSocial = new SqlParameter("@ObraSocial", obraSocial);

        var result = context.Database
            .SqlQueryRaw<int>("EXEC sp_InsertarPaciente @Nombre, @Apellido, @Dni, @ObraSocial",
                pNombre, pApellido, pDni, pObraSocial)
            .AsEnumerable()
            .FirstOrDefault();

        return result;
    }
}
```

### 4.4 Patrón 4: Manejo de Excepciones de Negocio de SQL Server (`THROW 50001/50002/50003`)
Procedimientos como `sp_AbrirSala` y `sp_CerrarSala` contienen reglas de negocio en la base de datos que lanzan errores personalizados. La DAL debe permitir que `SqlException` viaje a la BLL o capturarla para transformar el mensaje:

```csharp
public void AbrirSala(int idSala, int idUsuario)
{
    using (var context = new ConsultorioContext())
    {
        var pIdSala = new SqlParameter("@IdSala", idSala);
        var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);

        context.Database.ExecuteSqlRaw("EXEC sp_AbrirSala @IdSala, @IdUsuario", pIdSala, pIdUsuario);
    }
}
```

---

## 5. Guía Técnica de Implementación: Capa BLL

La Capa BLL (`Gestion_de_Turnos_Medicos.Negocio`) encapsula la lógica de negocio y desacopla la UI de la DAL:

### Estructura de ejemplo (`UsuarioBLL`):
```csharp
namespace Gestion_de_Turnos_Medicos.Negocio
{
    public class UsuarioBLL
    {
        private readonly UsuarioDAL _usuarioDAL = new UsuarioDAL();

        public UsuarioLoginResult Login(string correo, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
                throw new ArgumentException("El correo y la contraseña son campos obligatorios.");

            return _usuarioDAL.ValidarLogin(correo, contrasena);
        }

        public List<RolDTO> ObtenerRoles()
        {
            return _usuarioDAL.ListarRoles();
        }

        public List<UsuarioListadoDTO> ObtenerUsuarios()
        {
            return _usuarioDAL.ListarUsuarios();
        }

        public void RegistrarUsuario(string nombre, string apellido, string correo, string contrasena, 
                                     string dni, string telefono, int idRol, string matricula, 
                                     List<int> especialidadesIds, List<int> salasIds, string notaSala)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || 
                string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Los datos básicos del usuario son obligatorios.");

            string passHash = Seguridad.HashearContrasenia(contrasena);
            int nuevoId = _usuarioDAL.InsertarUsuario(nombre, apellido, correo, passHash, dni, telefono, matricula, idRol);

            if (especialidadesIds != null)
            {
                foreach (int idEsp in especialidadesIds)
                    _usuarioDAL.AsignarEspecialidadMedico(nuevoId, idEsp);
            }

            if (salasIds != null)
            {
                foreach (int idSala in salasIds)
                    _usuarioDAL.AsignarSalaMedico(idSala, nuevoId, notaSala);
            }
        }

        public void EliminarUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("ID de usuario inválido.");

            _usuarioDAL.EliminarUsuario(idUsuario);
        }
    }
}
```

---

## 6. Matriz Formulario ↔ Capa BLL ↔ Capa DAL ↔ Stored Procedure

| Formulario (Capa UI) | Método BLL invocado | Método DAL subyacente | Stored Procedure (`README_StoredProcedures.md`) |
| :--- | :--- | :--- | :--- |
| **`FrmLogin`** | `UsuarioBLL.Login` | `UsuarioDAL.ValidarLogin` | `sp_ValidarLogin` (Rutea a `FrmAdmin`, `Pantalla_Principal_PERSONAL_MEDICO`, `FrmRecepcionista` o `FrmUsuarioVentana` según rol 1, 2, 3 o 4) |
| **`FrmGestionUsuarios2`** *(Form Oficial)* | `UsuarioBLL.ObtenerRoles` | `UsuarioDAL.ListarRoles` | `sp_ListarRoles` (Soporta rol Usuario Ventana) |
| | `EspecialidadBLL.ObtenerEspecialidades` | `EspecialidadDAL.ListarEspecialidades` | `sp_ListarEspecialidades` |
| | `SalaBLL.ObtenerSalas` | `SalaDAL.ObtenerSalas` | `sp_ObtenerSalas` |
| | `UsuarioBLL.ObtenerUsuarios` | `UsuarioDAL.ListarUsuarios` | `sp_ListarUsuarios` |
| | `UsuarioBLL.RegistrarUsuario` | `UsuarioDAL.InsertarUsuario` | `sp_InsertarUsuario` |
| | | `UsuarioDAL.AsignarEspecialidad` | `sp_AsignarEspecialidadMedico` |
| | | `UsuarioDAL.AsignarSala` | `sp_AsignarSalaMedico` |
| | `UsuarioBLL.ModificarUsuario` | `UsuarioDAL.ModificarUsuario` | `sp_ModificarUsuario` + `sp_AsignarSalaMedico` + `sp_AsignarEspecialidadMedico` + `sp_ActualizarContrasenaHash` |
| | `UsuarioBLL.EliminarUsuario` | `UsuarioDAL.EliminarUsuario` | `sp_EliminarUsuario` |
| | `UsuarioBLL.ReactivarUsuario` | `UsuarioDAL.ReactivarUsuario` | `sp_ReactivarUsuario` |
| | `UsuarioBLL.ObtenerUsuarioPorDni` | `UsuarioDAL.ObtenerUsuarioPorDni` | Consulta atómica por DNI (activos e inactivos) |
| **`FrmSalasAdmin`** | `UsuarioBLL.ObtenerMedicos` | `UsuarioDAL.ListarPersonalMedico` | `sp_ListarPersonalMedico` |
| | `SalaBLL.ObtenerSalas` | `SalaDAL.ObtenerSalas` | `sp_ObtenerSalas` |
| | `SalaBLL.RegistrarSala` | `SalaDAL.InsertarSala` | `sp_InsertarSala` |
| | | `SalaDAL.AsignarSalaMedico` | `sp_AsignarSalaMedico` |
| | `SalaBLL.ModificarSala` | `SalaDAL.ModificarSala` | `sp_ModificarSala` + `sp_AsignarSalaMedico` |
| | `SalaBLL.EliminarSala` | `SalaDAL.EliminarSala` | `sp_EliminarSala` |
| **`FrmGestionEspecialidades`** | `EspecialidadBLL.ObtenerEspecialidades`| `EspecialidadDAL.ListarEspecialidades` | `sp_ListarEspecialidades` |
| | `EspecialidadBLL.RegistrarEspecialidad`| `EspecialidadDAL.InsertarEspecialidad` | `sp_InsertarEspecialidad` |
| | `EspecialidadBLL.EliminarEspecialidad` | `EspecialidadDAL.EliminarEspecialidad` | `sp_EliminarEspecialidad` |
| **`FrmTurnoEmergencia`** | `TurnoBLL.ObtenerSintomas` | `TurnoDAL.ObtenerSintomas` | `sp_ObtenerSintomas` |
| | `PacienteBLL.GuardarPaciente` | `PacienteDAL.GuardarPaciente` | `sp_GuardarPaciente` / `sp_InsertarPaciente` |
| | `TurnoBLL.RegistrarTurnoEmergencia` | `TurnoDAL.RegistrarTurnoEmergencia` | `sp_CrearTurnoEmergencia` |
| | `TurnoBLL.RegistrarTurnoSintoma` | `TurnoDAL.RegistrarTurnoSintoma` | `sp_GuardarTurnoSintoma` |
| **`FrmTurnoEspecialidad`** | `EspecialidadBLL.ObtenerEspecialidades`| `EspecialidadDAL.ListarEspecialidades` | `sp_ListarEspecialidades` |
| | `TurnoBLL.ObtenerHorariosDisponibles` | `TurnoDAL.ObtenerHorariosDisponibles` | `sp_ObtenerHorariosDisponibles` |
| | `PacienteBLL.GuardarPaciente` | `PacienteDAL.GuardarPaciente` | `sp_GuardarPaciente` |
| | `TurnoBLL.CrearTurnoEspecialidad` | `TurnoDAL.CrearTurnoEspecialidad` | `sp_CrearTurnoEspecialidad` |
| **`FrmListaTurnos`** | `TurnoBLL.ListarTurnosEmergencia` | `TurnoDAL.ListarTurnosEmergencia` | `sp_ListarTurnosEmergencia` |
| | `EspecialidadBLL.ObtenerEspecialidades`| `EspecialidadDAL.ListarEspecialidades` | `sp_ListarEspecialidades` |
| | `TurnoBLL.ListarTurnosEspecialidad` | `TurnoDAL.ListarTurnosEspecialidad` | `sp_ListarTurnosEspecialidad` |
| **`FrmListaTurnosAtencion`** | `EspecialidadBLL.ObtenerEspecialidades`| `EspecialidadDAL.ListarEspecialidades` | `sp_ObtenerEspecialidadesPorMedico` / `sp_ListarEspecialidades` |
| | `TurnoBLL.ObtenerTurnosAtencion` | `TurnoDAL.ListarTurnosAtencion` | `sp_ListarTurnosAtencion` |
| | `TurnoBLL.LlamarSiguientePaciente` | `TurnoDAL.LlamarSiguienteTurno` | `sp_LlamarSiguienteTurno` |
| | `TurnoBLL.IniciarAtencionTurno` | `TurnoDAL.IniciarAtencionTurno` | `sp_IniciarAtencionTurno` / `sp_ActualizarEstadoSala` |
| | `TurnoBLL.FinalizarAtencion` | `TurnoDAL.FinalizarAtencion` | `sp_FinalizarAtencionTurno` |
| | `HistoriaClinicaBLL.RegistrarHistoria` | `HistoriaClinicaDAL.InsertarHistoria` | `sp_InsertarHistoriaClinica` |
| **`MisSalas_PM`** | `SalaBLL.ObtenerSalas` (`@IdUsuario`) | `SalaDAL.ObtenerSalas` | `sp_ObtenerSalas` |
| | `SalaBLL.AbrirSala` | `SalaDAL.AbrirSala` | `sp_AbrirSala` |
| | `SalaBLL.CerrarSala` | `SalaDAL.CerrarSala` | `sp_CerrarSala` |
| **`FrmRecepcionista`** | *(Navegación UI / Contenedor)* | N/A | Botón `btnUsuarioVentana` ("Pantalla Turnos") para proyectar o incrustar `FrmUsuarioVentana` |
| **`FrmUsuarioVentana`** | `TurnoBLL.ListarTurnosEmergencia` | `TurnoDAL.ListarTurnosEmergencia` | `sp_ListarTurnosEmergencia` (Refresco cada 5s) |
| | `TurnoBLL.ObtenerTurnosPantallaGeneral` | `TurnoDAL.ListarTurnosGeneralesPantalla` | `sp_ListarTurnosGeneralesPantalla` (Refresco cada 5s) |

---

## 7. Directivas para la Capa de Presentación (UI / Forms)

1. **Invocación exclusiva de la BLL**: Los formularios deben instanciar únicamente su clase de negocio correspondiente (ej. `private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();`).
2. **Sin referencias a DAL ni ADO.NET**: Queda terminantemente prohibido importar namespaces de datos (`using Microsoft.Data.SqlClient;`, `using Gestion_de_Turnos_Medicos.CapaDeDatos;`) dentro de los Forms.
3. **Cero datos simulados**: Ningún bloque `catch` o método auxiliar debe cargar datos ficticios de contingencia. Las fallas de conexión o ejecución de SP deben notificarse al usuario con `MessageBox.Show` explicando el error devuelto por la BLL o el motor.
4. **Paso de contexto de sesión**: Formularios operativos como `MisSalas_PM` y `FrmListaTurnosAtencion` reciben al médico autenticado (`UsuarioLoginResult`) desde su formulario contenedor (`Pantalla_Principal_PERSONAL_MEDICO`), garantizando trazabilidad y filtrado por profesional.
