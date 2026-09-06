using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Gestion_de_Turnos_Medicos
{
    public class dbTurnosMedicos : DbContext
    {
        // 1. DEFINICIÓN DE LAS TABLAS (DbSets)
        // Cada DbSet representa una tabla en tu base de datos MiPrimeraBase.
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Prioridad> Prioridades { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<MedicoEspecialidad> MedicosEspecialidades { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Sintoma> Sintomas { get; set; }
        public DbSet<TurnoSintoma> TurnoSintomas { get; set; }
        public DbSet<HistoriaClinica> HistoriasClinicas { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<DetalleSala> DetallesSalas { get; set; }

        // 2. CONFIGURACIÓN DE LA CONEXIÓN
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Usamos los mismos datos con los que te conectaste exitosamente en DBeaver
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=dbGestionTurnos;Integrated Security=True;TrustServerCertificate=True;");
            }
        }

        // 3. CONFIGURACIÓN DEL MODELO (Evitar errores de borrado en cascada)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // En bases de datos relacionales complejas como la tuya, SQL Server bloquea la creación 
            // de tablas si detecta que borrar un registro (ej. Paciente) podría causar un borrado 
            // múltiple en cascada (ej. en Turnos e HistoriasClinicas al mismo tiempo).
            // Con este código deshabilitamos el borrado en cascada para evitar ese error.
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
            // --- NUEVO: FILTROS GLOBALES PARA BORRADO LÓGICO ---
            // Le decimos a EF Core que por defecto SIEMPRE ignore los registros que tengan Activo = false
            // Lo aplicamos principalmente a las tablas que mencionaste que sufrirán bajas.
            modelBuilder.Entity<Usuario>().HasQueryFilter(u => u.Activo);
            modelBuilder.Entity<Sala>().HasQueryFilter(s => s.Activo);
            modelBuilder.Entity<Especialidad>().HasQueryFilter(e => e.Activo);
        }

        // 4. AUTOMATIZACIÓN DE LA AUDITORÍA
        // Sobrescribimos el método SaveChanges para interceptar los datos justo antes de ir a SQL Server.
        public override int SaveChanges()
        {
            // Buscamos todas las entidades que hereden de EntidadAuditable y que hayan sido modificadas
            var entidadesModificadas = ChangeTracker.Entries<EntidadAuditable>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entidad in entidadesModificadas)
            {
                // Le asignamos la fecha y hora exacta del sistema en el momento de la modificación
                entidad.Entity.FechaModificacion = DateTime.Now;
            }

            // Ejecutamos el guardado normal
            return base.SaveChanges();
        }
    }
}