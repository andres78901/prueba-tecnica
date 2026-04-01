using Microsoft.EntityFrameworkCore;
using RickAndMorty.Domain.Entities;

namespace RickAndMorty.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Personaje> Personajes => Set<Personaje>();

    public DbSet<Solicitud> Solicitudes => Set<Solicitud>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Personaje>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId).IsUnique();
            entity.Property(e => e.Nombre).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Especie).HasMaxLength(128);
            entity.Property(e => e.Genero).HasMaxLength(64);
            entity.Property(e => e.Origen).HasMaxLength(256);
            entity.Property(e => e.ImagenUrl).HasMaxLength(512);
        });

        modelBuilder.Entity<Solicitud>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdExterno).HasMaxLength(100);
            entity.Property(e => e.Solicitante).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Evento).HasMaxLength(500).IsRequired();
            entity.HasOne(e => e.Personaje)
                .WithMany(p => p.Solicitudes)
                .HasForeignKey(e => e.PersonajeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
