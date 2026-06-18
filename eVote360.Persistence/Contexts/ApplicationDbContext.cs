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
        public DbSet<CandidatoPuesto> CandidatosPuestos { get; set; }
        public DbSet<Candidato> Candidatos { get; set; }
       public DbSet<SolicitudAlianza> SolicitudesAlianzas { get; set; }
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

            modelBuilder.Entity<Candidato>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Apellido).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Foto).IsRequired();

                entity.HasOne(c => c.PartidoPolitico)
                      .WithMany(p => p.Candidatos)
                      .HasForeignKey(c => c.PartidoPoliticoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SolicitudAlianza>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.FechaSolicitud).IsRequired();
                entity.Property(s => s.Estado).IsRequired();

                entity.HasOne(s => s.PartidoSolicitante)
                      .WithMany(p => p.SolicitudesEnviadas)
                      .HasForeignKey(s => s.PartidoSolicitanteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.PartidoReceptor)
                      .WithMany(p => p.SolicitudesRecibidas)
                      .HasForeignKey(s => s.PartidoReceptorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CandidatoPuesto>(entity =>
            {
                entity.HasKey(cp => cp.Id);
                entity.Property(cp => cp.EsAliado).IsRequired();

                entity.HasOne(cp => cp.Candidato)
                      .WithMany(c => c.CandidatosPuestos)
                      .HasForeignKey(cp => cp.CandidatoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cp => cp.PuestoElectivo)
                      .WithMany()
                      .HasForeignKey(cp => cp.PuestoElectivoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cp => cp.PartidoPolitico)
                      .WithMany(p => p.CandidatosPuestos)
                      .HasForeignKey(cp => cp.PartidoPoliticoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}