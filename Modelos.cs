using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_de_Turnos_Medicos
{
    // Prioridad: representa la urgencia del turno (Alta, Media, Baja, etc.).
    public class Prioridad
    {
        [Key]
        public int IdPrioridad { get; set; }
        public string Descripcion { get; set; }

        // Navegación: una prioridad puede aplicarse a muchos turnos.
        public ICollection<Turno> Turnos { get; set; }
    }

    // Paciente: entidad persistente que almacena la información básica del paciente.
    // Cuando se registra un turno, primero se crea (o se busca) el Paciente y luego
    // se asocia el Turno a ese Paciente (IdPaciente / Paciente).
    public class Paciente
    {
        [Key]
        public int IdPaciente { get; set; }

        // Datos personales básicos
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public int Edad { get; set; }
        public string ObraSocial { get; set; }

        // Relaciones
        public ICollection<Turno> Turnos { get; set; }
        public ICollection<HistoriaClinica> HistoriasClinicas { get; set; }
    }

    // Usuario: médico, recepcionista u otro actor del sistema.
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }
        public string NroMatricula { get; set; }
        public string Telefono { get; set; }
        public string Dni { get; set; }

        // Relaciones
        public ICollection<Turno> Turnos { get; set; }
        public ICollection<HistoriaClinica> HistoriasClinicas { get; set; }
        public ICollection<MedicoEspecialidad> MedicoEspecialidades { get; set; }
        public ICollection<DetalleSala> DetallesSala { get; set; }
    }

    public class Especialidad
    {
        [Key]
        public int IdEspecialidad { get; set; }
        public string Nombre { get; set; }

        public ICollection<Turno> Turnos { get; set; }
        public ICollection<MedicoEspecialidad> MedicoEspecialidades { get; set; }
    }

    // Tabla intermedia Medico <-> Especialidad
    public class MedicoEspecialidad
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

    // Turno: entidad central. Diseñada para mapear directamente a la tabla de turnos
    // en la base de datos. Contiene claves foráneas a Paciente, Prioridad, Especialidad y Usuario.
    // Para mantener el modelo prolijo, aquí guardamos solo los campos relevantes y
    // relaciones; las transformaciones para la UI deben hacerse en ViewModels o
    // mappers fuera de la entidad.
    public class Turno
    {
        [Key]
        public int IdTurno { get; set; }

        // Número interno / orden del turno
        public string NroOrden { get; set; }

        // Estado (En Espera, Llamado, En Consulta, Atendido, etc.)
        public string Estado { get; set; }

        // Fecha + hora de registro del turno (almacenamos Fecha; si se requiere hora
        // separada, se puede usar Horario de tipo TimeSpan).
        public DateTime Fecha { get; set; }
        public TimeSpan Horario { get; set; }

        // Información clínica breve que puede almacenarse directamente en el turno
        // o en una HistoriaClinica relacionada. Aquí permitimos guardar Motivo y
        // Triage para búsquedas y filtros rápidos sin exigir una tabla adicional.
        public string Motivo { get; set; }
        public string Triage { get; set; }

        public string TipoTurno { get; set; }

        // Foreign keys y propiedades de navegación
        [ForeignKey("Prioridad")]
        public int? IdPrioridad { get; set; }
        public Prioridad Prioridad { get; set; }

        [ForeignKey("Paciente")]
        public int IdPaciente { get; set; }
        public Paciente Paciente { get; set; }

        [ForeignKey("Especialidad")]
        public int? IdEspecialidad { get; set; }
        public Especialidad Especialidad { get; set; }

        [ForeignKey("Usuario")]
        public int? IdUsuario { get; set; }
        public Usuario Usuario { get; set; }

        // Tiempos de atención (persistidos). Se usan en la UI para medir duración.
        public DateTime? HoraInicioAtencion { get; set; }
        public DateTime? HoraFinAtencion { get; set; }

        // Colecciones relacionadas
        public ICollection<TurnoSintoma> TurnoSintomas { get; set; }
        public ICollection<Sala> Salas { get; set; }
        public ICollection<HistoriaClinica> HistoriasClinicas { get; set; }

        // ------------------------
        // Propiedades de compatibilidad mínima (NotMapped)
        // ------------------------
        // Para no romper el código existente en los forms (inicialmente
        // escritos contra un POCO diferente) exponemos unos pocos atajos
        // NotMapped. Estos atajos delegan la lectura/escritura a las entidades
        // relacionadas (Paciente, Especialidad). Sólo se añaden los necesarios
        // para mantener la compatibilidad y facilitar el registro desde forms
        // sin introducir un gran número de propiedades "sueltas".

        [NotMapped]
        public int Id { get => IdTurno; set => IdTurno = value; }

        [NotMapped]
        public DateTime HoraRegistro { get => Fecha; set => Fecha = value; }

        [NotMapped]
        public string HoraRegistroTexto => Fecha.ToString("HH:mm");

        [NotMapped]
        public string Nombre
        {
            get => Paciente?.Nombre ?? string.Empty;
            set
            {
                if (Paciente == null) Paciente = new Paciente();
                Paciente.Nombre = value;
            }
        }

        [NotMapped]
        public string Apellido
        {
            get => Paciente?.Apellido ?? string.Empty;
            set
            {
                if (Paciente == null) Paciente = new Paciente();
                Paciente.Apellido = value;
            }
        }

        [NotMapped]
        public string DNI
        {
            get => Paciente?.Dni ?? string.Empty;
            set
            {
                if (Paciente == null) Paciente = new Paciente();
                Paciente.Dni = value;
            }
        }

        [NotMapped]
        public int Edad
        {
            get => Paciente?.Edad ?? 0;
            set
            {
                if (Paciente == null) Paciente = new Paciente();
                Paciente.Edad = value;
            }
        }

        [NotMapped]
        public string Cobertura
        {
            get => Paciente?.ObraSocial ?? string.Empty;
            set
            {
                if (Paciente == null) Paciente = new Paciente();
                Paciente.ObraSocial = value;
            }
        }

        [NotMapped]
        public string Servicio
        {
            get => Especialidad?.Nombre ?? string.Empty;
            set
            {
                if (Especialidad == null) Especialidad = new Especialidad();
                Especialidad.Nombre = value;
            }
        }

        [NotMapped]
        public string PacienteTexto => Paciente != null ? $"{Paciente.Apellido}, {Paciente.Nombre} ({Paciente.Dni})" : string.Empty;

        // Campos clínicos y de trazabilidad que se usan en la UI.
        // Se mantienen en la entidad Turno para simplificar la edición rápida
        // desde los formularios. Si se busca un diseño más normalizado, mover
        // estos campos a HistoriaClinica.
        public string DiagnosticoRapido { get; set; }
        public string MedicoQueAtendio { get; set; }
        public string SalaDeAtencion { get; set; }

        [NotMapped]
        public string PacienteCorto => Paciente != null ? $"{Paciente.Apellido}, {Paciente.Nombre}" : string.Empty;

        [NotMapped]
        public string NombreCompleto => Paciente != null ? $"{Paciente.Nombre} {Paciente.Apellido}" : string.Empty;
    }

    public class Sintoma
    {
        [Key]
        public int IdSintoma { get; set; }
        public string Descripcion { get; set; }
        public string Gravedad { get; set; }

        public ICollection<TurnoSintoma> TurnoSintomas { get; set; }
    }

    // Tabla intermedia entre Turno y Sintoma.
    public class TurnoSintoma
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

    // HistoriaClinica: registros completos de atención. Un Turno puede apuntar
    // a varias HistoriasClínicas (por ejemplo: evolución, notas, recetas).
    public class HistoriaClinica
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

    public class Sala
    {
        [Key]
        public int IdSala { get; set; }
        public string NombreSala { get; set; }
        public string EstadoSala { get; set; }

        [ForeignKey("Turno")]
        public int? IdTurno { get; set; }
        public Turno Turno { get; set; }

        public ICollection<DetalleSala> DetallesSala { get; set; }
    }

    public class DetalleSala
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

    /*
     * Notas sobre cómo registrar un turno correctamente con este modelo:
     * - Al crear un turno desde un formulario se recomienda:
     *   1) Buscar o crear la entidad Paciente y obtener su IdPaciente.
     *   2) Crear la entidad Turno rellenando IdPaciente (y opcionalmente IdEspecialidad/IdPrioridad/IdUsuario).
     *   3) Guardar el Turno y luego crear las entradas TurnoSintoma que referencien IdTurno.
     *
     * De esta forma las entidades quedan normalizadas y los datos clínicos (observaciones,
     * diagnósticos extensos) pueden almacenarse en HistoriaClinica cuando se requiera trazabilidad.
     */
}
