using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Contexto de base de datos Entity Framework Core para el sistema de Gestión de Turnos Médicos.
    /// Administra la sesión con SQL Server, la configuración del modelo de entidades,
    /// las políticas de borrado en cascada, los filtros globales de borrado lógico y la auditoría automática.
    /// </summary>
    public class dbTurnosMedicos : DbContext
    {
        /// <summary>Conjunto de entidades para los roles de usuario (Administrador, Personal Médico, Recepcionista).</summary>
        public DbSet<Rol> Roles { get; set; }

        /// <summary>Conjunto de entidades para las prioridades de triage médico (Alta, Media, Baja).</summary>
        public DbSet<Prioridad> Prioridades { get; set; }

        /// <summary>Conjunto de entidades para pacientes registrados en el sistema.</summary>
        public DbSet<Paciente> Pacientes { get; set; }

        /// <summary>Conjunto de entidades para los usuarios del sistema (operadores y profesionales).</summary>
        public DbSet<Usuario> Usuarios { get; set; }

        /// <summary>Conjunto de entidades para las especialidades médicas de atención.</summary>
        public DbSet<Especialidad> Especialidades { get; set; }

        /// <summary>Conjunto de entidades de relación intermedia entre médicos y sus especialidades habilitadas.</summary>
        public DbSet<MedicoEspecialidad> MedicosEspecialidades { get; set; }

        /// <summary>Conjunto de entidades para los turnos médicos (emergencias y consultas programadas).</summary>
        public DbSet<Turno> Turnos { get; set; }

        /// <summary>Conjunto de entidades para el catálogo de síntomas y gravedades de triage.</summary>
        public DbSet<Sintoma> Sintomas { get; set; }

        /// <summary>Conjunto de entidades de relación intermedia entre turnos y síntomas manifestados.</summary>
        public DbSet<TurnoSintoma> TurnoSintomas { get; set; }

        /// <summary>Conjunto de entidades para el registro histórico de atenciones y evoluciones médicas.</summary>
        public DbSet<HistoriaClinica> HistoriasClinicas { get; set; }

        /// <summary>Conjunto de entidades para las salas y consultorios físicos del centro médico.</summary>
        public DbSet<Sala> Salas { get; set; }

        /// <summary>Conjunto de entidades de auditoría y detalle de asignación/uso de salas por médicos.</summary>
        public DbSet<DetalleSala> DetallesSalas { get; set; }

        /// <summary>
        /// Configura el proveedor de base de datos y la cadena de conexión si no ha sido configurada previamente.
        /// </summary>
        /// <param name="optionsBuilder">Generador de opciones de configuración de contexto de base de datos.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Conexión por defecto a la instancia local SQL Server
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=dbGestionTurnos;Integrated Security=True;TrustServerCertificate=True;");
            }
        }

        /// <summary>
        /// Personaliza las reglas del modelo relacional, desactiva borrados en cascada para evitar ciclos
        /// y aplica filtros globales de consulta (Query Filters) para soportar borrado lógico transparente.
        /// </summary>
        /// <param name="modelBuilder">Generador de modelos de Entity Framework Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Deshabilitar el borrado en cascada en todas las relaciones foráneas para prevenir
            // ciclos de eliminación y asegurar integridad referencial estricta.
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Filtros globales para borrado lógico: por defecto EF Core excluye registros inactivos (Activo == false)
            modelBuilder.Entity<Usuario>().HasQueryFilter(u => u.Activo);
            modelBuilder.Entity<Sala>().HasQueryFilter(s => s.Activo);
            modelBuilder.Entity<Especialidad>().HasQueryFilter(e => e.Activo);
        }

        /// <summary>
        /// Intercepta el guardado de cambios en la base de datos para actualizar automáticamente
        /// las marcas de tiempo de auditoría (<see cref="EntidadAuditable.FechaModificacion"/>) en entidades modificadas.
        /// </summary>
        /// <returns>Número de registros de estado afectados en la base de datos.</returns>
        public override int SaveChanges()
        {
            // Detecta entidades auditables que han sido alteradas en el contexto actual
            var entidadesModificadas = ChangeTracker.Entries<EntidadAuditable>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entidad in entidadesModificadas)
            {
                entidad.Entity.FechaModificacion = DateTime.Now;
            }

            return base.SaveChanges();
        }
    }
}