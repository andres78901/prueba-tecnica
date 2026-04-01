using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RickAndMorty.Infrastructure.Persistence;

#nullable disable

namespace RickAndMorty.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

            modelBuilder.Entity("RickAndMorty.Domain.Entities.Personaje", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Especie")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<int>("Estado")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExternalId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FechaImport")
                        .HasColumnType("TEXT");

                    b.Property<string>("Genero")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("ImagenUrl")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("Origen")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.ToTable("Personajes");
                });

            modelBuilder.Entity("RickAndMorty.Domain.Entities.Solicitud", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Estado")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Evento")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FechaActualizacion")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FechaEvento")
                        .HasColumnType("TEXT");

                    b.Property<string>("IdExterno")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("PersonajeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Solicitante")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PersonajeId");

                    b.ToTable("Solicitudes");
                });

            modelBuilder.Entity("RickAndMorty.Domain.Entities.Solicitud", b =>
                {
                    b.HasOne("RickAndMorty.Domain.Entities.Personaje", "Personaje")
                        .WithMany("Solicitudes")
                        .HasForeignKey("PersonajeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Personaje");
                });

            modelBuilder.Entity("RickAndMorty.Domain.Entities.Personaje", b =>
                {
                    b.Navigation("Solicitudes");
                });
#pragma warning restore 612, 618
        }
    }
}
