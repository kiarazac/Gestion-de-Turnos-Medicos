using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_de_Turnos_Medicos
{
    // ==========================================
    // CLASE BASE PARA AUDITORÍA
    // ==========================================
    // Al heredar de esta clase, todas las tablas en la base de datos 
    // tendrán automáticamente estas dos columnas para registrar cuándo
    // se creó el registro y cuándo fue la última vez que se alteró.
    public abstract class EntidadAuditable
    {
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }

        // --- NUEVOS CAMPOS PARA BORRADO LÓGICO ---
        // Todos los registros nacen activos por defecto
        public bool Activo { get; set; } = true;

        // Registra exactamente cuándo se "eliminó" el registro
        public DateTime? FechaBaja { get; set; }
    }

    // ==========================================
    // MODELOS ESTRICTOS 
    // ==========================================

    public class Prioridad : EntidadAuditable
    {
        [Key]
        public int IdPrioridad { get; set; }
        public string Descripcion { get; set; }

        // Navegación: Una prioridad tiene muchos turnos.
        public ICollection<Turno> Turnos { get; set; }
    }

    public class Paciente : EntidadAuditable
    {
        [Key]
        public int IdPaciente { get; set; }

        // Se eliminó 'Edad' y los atributos NotMapped para apegarse al diagrama.
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string ObraSocial { get; set; }

        // Relaciones: Un paciente tiene muchos turnos e historias clínicas.
        public ICollection<Turno> Turnos { get; set; }
        public ICollection<HistoriaClinica> HistoriasClinicas { get; set; }
    }

    public class Rol : EntidadAuditable
    {
        [Key]
        public int IdRol { get; set; }
        public string Descripcion { get; set; }

        // Navegación: Un mismo rol (ej. "Médico") puede pertenecer a muchos usuarios.
        public ICollection<Usuario> Usuarios { get; set; }
    }
    public class Usuario : EntidadAuditable
    {
        [Key]
        public int IdUsuario { get; set; }

        public string Correo { get; set; }
        public string Nombre { get; set; }
        public string Contrasena { get; set; }

        [ForeignKey("RolNav")] // El nombre en el ForeignKey debe coincidir con la propiedad de navegación de abajo
        public int IdRol { get; set; }
        public Rol RolNav { get; set; } 

        public string NroMatricula { get; set; }
        public string Telefono { get; set; }
        public string Dni { get; set; }
        public string Apellido { get; set; }

        // Relaciones que ya tenías
        public ICollection<Turno> Turnos { get; set; }
        public ICollection<HistoriaClinica> HistoriasClinicas { get; set; }
        public ICollection<MedicoEspecialidad> MedicoEspecialidades { get; set; }
        public ICollection<DetalleSala> DetallesSala { get; set; }
    }

    public class Especialidad : EntidadAuditable
    {
        [Key]
        public int IdEspecialidad { get; set; }
        public string Nombre { get; set; }

        public ICollection<Turno> Turnos { get; set; }
        public ICollection<MedicoEspecialidad> MedicoEspecialidades { get; set; }
    }

    public class MedicoEspecialidad : EntidadAuditable
    {
        [Key]
        public int IdMedicoEsp { get; set; }

        [ForeignKey("Especialidad")]
        public int IdEspecialidad { get; set; }
        public Especialidad Especialidad { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }

    public class Turno : EntidadAuditable
    {
        [Key]
        public int IdTurno { get; set; }

        public string NroOrden { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Horario { get; set; }
        public string TipoTurno { get; set; }

        // --- RELACIONES OBLIGATORIAS (No pueden ser nulas al crear) ---

        [ForeignKey("Prioridad")]
        public int IdPrioridad { get; set; }
        public Prioridad Prioridad { get; set; }

        [ForeignKey("Paciente")]
        public int IdPaciente { get; set; }
        public Paciente Paciente { get; set; }

        [ForeignKey("Especialidad")]
        public int IdEspecialidad { get; set; }
        public Especialidad Especialidad { get; set; }


        // --- RELACIONES OPCIONALES (Se asignan en plena ejecución) ---

        // Médico (Usuario): Puede estar vacío al principio y asignarse cuando el médico lo llama.
        [ForeignKey("Usuario")]
        public int? IdUsuario { get; set; }
        public Usuario Usuario { get; set; }

        // Sala: Puede estar vacía al crear el turno en recepción, y llenarse cuando 
        // se decide en qué consultorio se va a atender al paciente.
        [ForeignKey("Sala")]
        public int? IdSala { get; set; }
        public Sala Sala { get; set; }

        // -------------------------------------------------------------

        // Colecciones dependientes
        public ICollection<TurnoSintoma> TurnoSintomas { get; set; }
        public ICollection<HistoriaClinica> HistoriasClinicas { get; set; }
    }

    public class Sintoma : EntidadAuditable
    {
        [Key]
        public int IdSintoma { get; set; }
        public string Descripcion { get; set; }
        public string Gravedad { get; set; }

        public ICollection<TurnoSintoma> TurnoSintomas { get; set; }
    }

    public class TurnoSintoma : EntidadAuditable
    {
        [Key]
        public int IdTurnoSintoma { get; set; }
        public string EstadoActual { get; set; }

        [ForeignKey("Turno")]
        public int IdTurno { get; set; }
        public Turno Turno { get; set; }

        [ForeignKey("Sintoma")]
        public int IdSintoma { get; set; }
        public Sintoma Sintoma { get; set; }
    }

    public class HistoriaClinica : EntidadAuditable
    {
        [Key]
        public int IdHistoria { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoTurno { get; set; }
        public string DiagRapido { get; set; }
        public string DescripHistoriaClinica { get; set; }
        public string RecetaMedicamentos { get; set; }

        [ForeignKey("Paciente")]
        public int IdPaciente { get; set; }
        public Paciente Paciente { get; set; }

        [ForeignKey("Turno")]
        public int IdTurno { get; set; }
        public Turno Turno { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }

    public class Sala : EntidadAuditable
    {
        [Key]
        public int IdSala { get; set; }

        public string NombreSala { get; set; }
        public string EstadoSala { get; set; }

        public ICollection<Turno> Turnos { get; set; }

        public ICollection<DetalleSala> DetallesSala { get; set; }
    }

    public class DetalleSala : EntidadAuditable
    {
        [Key]
        public int IdDetalleSala { get; set; }
        public string DescripcionAtencion { get; set; }

        [ForeignKey("Sala")]
        public int IdSala { get; set; }
        public Sala Sala { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }
}