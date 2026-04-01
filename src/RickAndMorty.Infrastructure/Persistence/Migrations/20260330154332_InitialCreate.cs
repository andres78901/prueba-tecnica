using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RickAndMorty.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
        {
            UpSqlServer(migrationBuilder);
        }
        else
        {
            UpSqlite(migrationBuilder);
        }
    }

    private static void UpSqlServer(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Personajes",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ExternalId = table.Column<int>(nullable: false),
                Nombre = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Estado = table.Column<int>(nullable: false),
                Especie = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Genero = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Origen = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                ImagenUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                FechaImport = table.Column<DateTime>(type: "datetime2", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Personajes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Solicitudes",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                IdExterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                PersonajeId = table.Column<int>(nullable: false),
                Solicitante = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Evento = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                Estado = table.Column<int>(nullable: false),
                FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Solicitudes", x => x.Id);
                table.ForeignKey(
                    name: "FK_Solicitudes_Personajes_PersonajeId",
                    column: x => x.PersonajeId,
                    principalTable: "Personajes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Personajes_ExternalId",
            table: "Personajes",
            column: "ExternalId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Solicitudes_PersonajeId",
            table: "Solicitudes",
            column: "PersonajeId");
    }

    private static void UpSqlite(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Personajes",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ExternalId = table.Column<int>(type: "INTEGER", nullable: false),
                Nombre = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                Estado = table.Column<int>(type: "INTEGER", nullable: false),
                Especie = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                Genero = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                Origen = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                ImagenUrl = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                FechaImport = table.Column<DateTime>(type: "TEXT", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Personajes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Solicitudes",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                IdExterno = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                PersonajeId = table.Column<int>(type: "INTEGER", nullable: false),
                Solicitante = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Evento = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                FechaEvento = table.Column<DateTime>(type: "TEXT", nullable: false),
                Estado = table.Column<int>(type: "INTEGER", nullable: false),
                FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Solicitudes", x => x.Id);
                table.ForeignKey(
                    name: "FK_Solicitudes_Personajes_PersonajeId",
                    column: x => x.PersonajeId,
                    principalTable: "Personajes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Personajes_ExternalId",
            table: "Personajes",
            column: "ExternalId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Solicitudes_PersonajeId",
            table: "Solicitudes",
            column: "PersonajeId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Solicitudes");
        migrationBuilder.DropTable(name: "Personajes");
    }
}
