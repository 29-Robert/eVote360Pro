using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eVote360.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PuestoElectivo> PuestosElectivos { get; set; }
        public DbSet<Ciudadano> Ciudadanos { get; set; }
        public DbSet<PartidoPolitico> PartidosPoliticos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<AsignacionDirigente> AsignacionesDirigentes { get; set; }
        public DbSet<Eleccion> Elecciones { get; set; }
        public DbSet<CodigoVerificacion> CodigosVerificacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PuestoElectivo>()
                .HasIndex(p => p.Nombre)
                .IsUnique();

            modelBuilder.Entity<Ciudadano>()
                .HasIndex(c => c.DocumentoIdentidad)
                .IsUnique();

            modelBuilder.Entity<Ciudadano>()
                .HasIndex(c => c.CorreoElectronico)
                .IsUnique();

            modelBuilder.Entity<PartidoPolitico>()
                .HasIndex(p => p.Siglas)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.CorreoElectronico)
                .IsUnique();

            modelBuilder.Entity<AsignacionDirigente>()
                .HasIndex(a => a.UsuarioId)
                .IsUnique();

            modelBuilder.Entity<AsignacionDirigente>()
                .HasIndex(a => a.PartidoPoliticoId)
                .IsUnique();
        }
    }
}