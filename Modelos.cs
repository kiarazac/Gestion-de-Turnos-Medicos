using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Clase abstracta base que provee propiedades estándar de auditoría y borrado lógico a todas las entidades persistidas.
    /// </summary>
    public abstract class EntidadAuditable
    {
        /// <summary>Fecha y hora exacta en la que se dio de alta el registro en el sistema.</summary>
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        /// <summary>Fecha y hora de la última modificación sufrida por el registro (gestionada automáticamente por EF Core).</summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>Indica el estado lógico del registro. <c>true</c> si se encuentra activo y operativo; <c>false</c> si fue dado de baja lógica.</summary>
        public bool Activo { get; set; } = true;

        /// <summary>Fecha y hora en que se ejecutó la baja lógica del registro.</summary>
        public DateTime? FechaBaja { get; set; }
    }

    /// <summary>
    /// Representa un nivel de prioridad de atención en el triage médico (ej. Alta, Media, Baja).
    /// </summary>
    public class Prioridad : EntidadAuditable
    {
        /// <summary>Identificador único de la prioridad.</summary>
        [Key]
        public int IdPrioridad { get; set; }

        /// <summary>Descripción textual del nivel de urgencia o prioridad.</summary>
        public string Descripcion { get; set; }

        /// <summary>Colección de turnos asociados a esta prioridad de atención.</summary>
        public ICollection<Turno> Turnos { get; set; }
    }

    /// <summary>
    /// Representa a un paciente registrado en el centro médico.
    /// </summary>
    public class Paciente : EntidadAuditable
    {
        /// <summary>Identificador único del paciente.</summary>
        [Key]
        public int IdPaciente { get; set; }

        /// <summary>Nombre(s) del paciente.</summary>
        public string Nombre { get; set; }

        /// <summary>Apellido(s) del paciente.</summary>
        public string Apellido { get; set; }

        /// <summary>Documento Nacional de Identidad (DNI) del paciente (único para búsqueda y verificación).</summary>
        public string Dni { get; set; }

        /// <summary>Nombre de la obra social o cobertura médica prepaga del paciente.</summary>
        public string ObraSocial { get; set; }

        /// <summary>Historial de turnos solicitados o recibidos por el paciente.</summary>
        public ICollection<Turno> Turnos { get; set; }

        /// <summary>Historial clínico acumulado del paciente.</summary>
        public ICollection<HistoriaClinica> HistoriasClinicas { get; set; }
    }

    /// <summary>
    /// Representa un perfil o rol de acceso al sistema (Administrador, Personal Médico, Recepcionista).
    /// </summary>
    public class Rol : EntidadAuditable
    {
        /// <summary>Identificador único del rol.</summary>
        [Key]
        public int IdRol { get; set; }

        /// <summary>Descripción o nombre del rol funcional.</summary>
        public string Descripcion { get; set; }

        /// <summary>Colección de usuarios que poseen este rol.</summary>
        public ICollection<Usuario> Usuarios { get; set; }
    }

    /// <summary>
    /// Representa un usuario del sistema (personal administrativo o profesional médico).
    /// </summary>
    public class Usuario : EntidadAuditable
    {
        /// <summary>Identificador único del usuario.</summary>
        [Key]
        public int IdUsuario { get; set; }

        /// <summary>Correo electrónico utilizado para el inicio de sesión.</summary>
        public string Correo { get; set; }

        /// <summary>Nombre(s) de pila del usuario.</summary>
        public string Nombre { get; set; }

        /// <summary>Contraseña hasheada (o texto plano legado) para comprobación criptográfica.</summary>
        public string Contrasena { get; set; }

        /// <summary>Identificador foráneo del rol asignado.</summary>
        [ForeignKey("RolNav")]
        public int IdRol { get; set; }

        /// <summary>Propiedad de navegación hacia el rol asignado.</summary>
        public Rol RolNav { get; set; } 

        /// <summary>Número de matrícula profesional (obligatorio para médicos, nulo para otros roles).</summary>
        public string? NroMatricula { get; set; }

        /// <summary>Número telefónico de contacto del usuario.</summary>
        public string Telefono { get; set; }

        /// <summary>Documento Nacional de Identidad (DNI) del usuario.</summary>
        public string Dni { get; set; }

        /// <summary>Apellido(s) del usuario.</summary>
        public string Apellido { get; set; }

        /// <summary>Turnos médicos atendidos o asignados a este profesional.</summary>
        public ICollection<Turno> Turnos { get; set; }

        /// <summary>Registros de evolución en historias clínicas redactados por el profesional.</summary>
        public ICollection<HistoriaClinica> HistoriasClinicas { get; set; }

        /// <summary>Especialidades médicas asociadas al profesional.</summary>
        public ICollection<MedicoEspecialidad> MedicoEspecialidades { get; set; }

        /// <summary>Historial de asignaciones de consultorios o salas del usuario.</summary>
        public ICollection<DetalleSala> DetallesSala { get; set; }
    }

    /// <summary>
    /// Representa una rama o especialidad de la medicina atendida en el establecimiento.
    /// </summary>
    public class Especialidad : EntidadAuditable
    {
        /// <summary>Identificador único de la especialidad.</summary>
        [Key]
        public int IdEspecialidad { get; set; }

        /// <summary>Nombre de la especialidad (ej. Pediatría, Cardiología, Traumatología).</summary>
        public string Nombre { get; set; }

        /// <summary>Turnos solicitados en esta especialidad.</summary>
        public ICollection<Turno> Turnos { get; set; }

        /// <summary>Profesionales médicos que atienden en esta especialidad.</summary>
        public ICollection<MedicoEspecialidad> MedicoEspecialidades { get; set; }
    }

    /// <summary>
    /// Entidad asociativa muchos a muchos entre un profesional médico (<see cref="Usuario"/>) y una <see cref="Especialidad"/>.
    /// </summary>
    public class MedicoEspecialidad : EntidadAuditable
    {
        /// <summary>Identificador único de la asignación médico-especialidad.</summary>
        [Key]
        public int IdMedicoEsp { get; set; }

        /// <summary>Identificador foráneo de la especialidad.</summary>
        [ForeignKey("Especialidad")]
        public int IdEspecialidad { get; set; }

        /// <summary>Propiedad de navegación hacia la especialidad.</summary>
        public Especialidad Especialidad { get; set; }

        /// <summary>Identificador foráneo del profesional médico.</summary>
        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        /// <summary>Propiedad de navegación hacia el usuario médico.</summary>
        public Usuario Usuario { get; set; }
    }

    /// <summary>
    /// Representa un turno de atención médica, ya sea programado por especialidad o de emergencia por triage.
    /// </summary>
    public class Turno : EntidadAuditable
    {
        /// <summary>Identificador único del turno.</summary>
        [Key]
        public int IdTurno { get; set; }

        /// <summary>Código alfanumérico visible de orden para llamada en pantallas (ej. 'E-001', 'T-045').</summary>
        public string NroOrden { get; set; }

        /// <summary>Estado operativo del turno ('En Espera', 'En Atencion', 'Finalizado', 'Cancelado').</summary>
        public string Estado { get; set; }

        /// <summary>Fecha asignada para la atención médica.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Franja horaria programada para la consulta.</summary>
        public TimeSpan Horario { get; set; }

        /// <summary>Tipo de turno ('Emergencia' o 'Especialidad').</summary>
        public string TipoTurno { get; set; }

        /// <summary>Identificador foráneo del nivel de prioridad asignado.</summary>
        [ForeignKey("Prioridad")]
        public int IdPrioridad { get; set; }

        /// <summary>Propiedad de navegación de prioridad del turno.</summary>
        public Prioridad Prioridad { get; set; }

        /// <summary>Identificador foráneo del paciente atendido.</summary>
        [ForeignKey("Paciente")]
        public int IdPaciente { get; set; }

        /// <summary>Propiedad de navegación del paciente asignado.</summary>
        public Paciente Paciente { get; set; }

        /// <summary>Identificador foráneo de la especialidad médica.</summary>
        [ForeignKey("Especialidad")]
        public int IdEspecialidad { get; set; }

        /// <summary>Propiedad de navegación de la especialidad asignada.</summary>
        public Especialidad Especialidad { get; set; }

        /// <summary>Identificador foráneo del médico que toma la atención (opcional hasta ser llamado).</summary>
        [ForeignKey("Usuario")]
        public int? IdUsuario { get; set; }

        /// <summary>Propiedad de navegación hacia el profesional que realiza la atención.</summary>
        public Usuario Usuario { get; set; }

        /// <summary>Identificador foráneo de la sala o consultorio de atención (opcional hasta asignación física).</summary>
        [ForeignKey("Sala")]
        public int? IdSala { get; set; }

        /// <summary>Propiedad de navegación hacia la sala o consultorio.</summary>
        public Sala Sala { get; set; }

        /// <summary>Colección de síntomas manifestados por el paciente en este turno.</summary>
        public ICollection<TurnoSintoma> TurnoSintomas { get; set; }

        /// <summary>Registros médicos e historias clínicas generadas a partir de este turno.</summary>
        public ICollection<HistoriaClinica> HistoriasClinicas { get; set; }
    }

    /// <summary>
    /// Catálogo de síntomas médicos evaluados en el proceso de triage de emergencia.
    /// </summary>
    public class Sintoma : EntidadAuditable
    {
        /// <summary>Identificador único del síntoma.</summary>
        [Key]
        public int IdSintoma { get; set; }

        /// <summary>Descripción clínica o manifestación del síntoma.</summary>
        public string Descripcion { get; set; }

        /// <summary>Nivel de gravedad asociado al síntoma (Alta, Media, Baja).</summary>
        public string Gravedad { get; set; }

        /// <summary>Colección de relaciones intermedias con turnos que presentaron este síntoma.</summary>
        public ICollection<TurnoSintoma> TurnoSintomas { get; set; }
    }

    /// <summary>
    /// Entidad asociativa intermedia que vincula los síntomas diagnosticados o informados con un turno médico específico.
    /// </summary>
    public class TurnoSintoma : EntidadAuditable
    {
        /// <summary>Identificador único del registro turno-síntoma.</summary>
        [Key]
        public int IdTurnoSintoma { get; set; }

        /// <summary>Estado o condición del síntoma al momento de la admisión (ej. 'Presente', 'Crónico').</summary>
        public string EstadoActual { get; set; }

        /// <summary>Identificador foráneo del turno asociado.</summary>
        [ForeignKey("Turno")]
        public int IdTurno { get; set; }

        /// <summary>Propiedad de navegación hacia el turno.</summary>
        public Turno Turno { get; set; }

        /// <summary>Identificador foráneo del síntoma asociado.</summary>
        [ForeignKey("Sintoma")]
        public int IdSintoma { get; set; }

        /// <summary>Propiedad de navegación hacia el síntoma.</summary>
        public Sintoma Sintoma { get; set; }
    }

    /// <summary>
    /// Representa un registro de evolución clínica, diagnóstico y prescripción médica de una consulta.
    /// </summary>
    public class HistoriaClinica : EntidadAuditable
    {
        /// <summary>Identificador único del registro de historia clínica.</summary>
        [Key]
        public int IdHistoria { get; set; }

        /// <summary>Fecha y hora en que se redactó la evolución clínica.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Tipo de consulta o turno atendido ('Emergencia' o 'Especialidad').</summary>
        public string TipoTurno { get; set; }

        /// <summary>Diagnóstico rápido o presuntivo determinado por el médico.</summary>
        public string DiagRapido { get; set; }

        /// <summary>Descripción detallada de la atención, signos vitales y anamnesis del paciente.</summary>
        public string DescripHistoriaClinica { get; set; }

        /// <summary>Medicamentos recetados, posología e indicaciones terapéuticas.</summary>
        public string RecetaMedicamentos { get; set; }

        /// <summary>Identificador foráneo del paciente al que pertenece la historia.</summary>
        [ForeignKey("Paciente")]
        public int IdPaciente { get; set; }

        /// <summary>Propiedad de navegación hacia el paciente.</summary>
        public Paciente Paciente { get; set; }

        /// <summary>Identificador foráneo del turno médico que originó esta atención.</summary>
        [ForeignKey("Turno")]
        public int IdTurno { get; set; }

        /// <summary>Propiedad de navegación hacia el turno médico.</summary>
        public Turno Turno { get; set; }

        /// <summary>Identificador foráneo del médico responsable que firmó la atención.</summary>
        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        /// <summary>Propiedad de navegación hacia el médico actuante.</summary>
        public Usuario Usuario { get; set; }
    }

    /// <summary>
    /// Representa un espacio físico de atención (consultorio médico, box de guardia o sala de triage).
    /// </summary>
    public class Sala : EntidadAuditable
    {
        /// <summary>Identificador único de la sala.</summary>
        [Key]
        public int IdSala { get; set; }

        /// <summary>Nombre o denominación física de la sala (ej. 'Consultorio 1', 'Guardia 2').</summary>
        public string NombreSala { get; set; }

        /// <summary>Estado operativo actual de la sala ('Disponible', 'Ocupado', 'Mantenimiento').</summary>
        public string EstadoSala { get; set; }

        /// <summary>Colección de turnos que fueron atendidos en esta sala.</summary>
        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();

        /// <summary>Historial de asignaciones de profesionales médicos que prestaron servicio en esta sala.</summary>
        public ICollection<DetalleSala> DetallesSala { get; set; } = new List<DetalleSala>();
    }

    /// <summary>
    /// Registra el detalle histórico y descriptivo de la asignación y uso de una sala por parte de un profesional de la salud.
    /// </summary>
    public class DetalleSala : EntidadAuditable
    {
        /// <summary>Identificador único del registro de detalle de sala.</summary>
        [Key]
        public int IdDetalleSala { get; set; }

        /// <summary>Notas o descripción del tipo de atención llevada a cabo en la sala.</summary>
        public string DescripcionAtencion { get; set; }

        /// <summary>Identificador foráneo de la sala asignada.</summary>
        [ForeignKey("Sala")]
        public int IdSala { get; set; }

        /// <summary>Propiedad de navegación hacia la sala.</summary>
        public Sala Sala { get; set; }

        /// <summary>Identificador foráneo del usuario profesional asignado.</summary>
        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        /// <summary>Propiedad de navegación hacia el usuario médico.</summary>
        public Usuario Usuario { get; set; }
    }
}