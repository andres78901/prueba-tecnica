using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RickAndMorty.Infrastructure.Persistence;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "RickAndMorty.Api");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=rickandmorty.db";

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var provider = configuration["Database:Provider"] ?? "Sqlite";
        if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            builder.UseSqlServer(connectionString);
        }
        else
        {
            builder.UseSqlite(connectionString);
        }

        return new ApplicationDbContext(builder.Options);
    }
}
