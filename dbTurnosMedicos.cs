using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Gestion_de_Turnos_Medicos
{
    public class dbTurnosMedicos : DbContext
    {
        // Cada DbSet representa una tabla en la base de datos MariaDB. 
        // A través de estas propiedades podremos hacer consultas LINQ (ej. db.Pacientes.ToList()).
        public DbSet<Prioridad> Prioridades { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<MedicoEspecialidad> MedicoEspecialidades { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Sintoma> Sintomas { get; set; }
        public DbSet<TurnoSintoma> TurnoSintomas { get; set; }
        public DbSet<HistoriaClinica> HistoriasClinicas { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<DetalleSala> DetallesSala { get; set; }

        // Este método se ejecuta al inicializar el contexto. Define cómo y a dónde conectarse.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Tu cadena de conexión con el puerto 3307 y tu contraseña
                string connectionString = "Server=localhost;Port=3307;Database=TurnosHospital;Uid=root;Pwd=dbGestorDeTurnosSecure;";

                // Le indicamos a EF Core que use MySQL/MariaDB
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }
    }
}
