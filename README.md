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
   - Aplica validaciones de reglas de negocio, integridad de estados y seguridad (hasheo SHA-256 de credenciales).
   - Orquesta flujos entre múltiples entidades antes de delegar la persistencia.
3. **Capa de Acceso a Datos (DAL)**:
   - Ubicada en el espacio de nombres `Gestion_de_Turnos_Medicos.CapaDeDatos`.
   - Utiliza exclusivamente **Entity Framework Core** (`ConsultorioContext`) mediante `SqlQueryRaw` y `ExecuteSqlRaw`.
   - Invoca Stored Procedures de forma parametrizada y atómica mediante transacciones (`SqlTransaction`), preservando la integridad de esquemas.
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
- **`FrmTurnoEspecialidad`:** Turnos programados correlativos con selección de especialista y fecha.
- **`FrmListaTurnos`:** Tablero y contadores de turnos en espera.
- **`FrmListaTurnosAtencion`:** Monitor de consultorio para el médico (Llamar paciente, Iniciar atención, Finalizar atención y carga de Historia Clínica).
- **`MisSalas_PM`:** Panel para que el médico autenticado gestione la apertura y cierre de sus consultorios designados.

### 5. Pantalla Pública de Sala de Espera (`FrmUsuarioVentana`)
- Monitor visual a pantalla completa (o visor incrustable) para pacientes en sala de espera, con actualización en tiempo real de llamados activos a consultorios y estado de la guardia.

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