using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Application.Abstractions.External;
using RickAndMorty.Application.Abstractions.Repositories;
using RickAndMorty.Infrastructure.External;
using RickAndMorty.Infrastructure.Persistence;
using RickAndMorty.Infrastructure.Persistence.Repositories;

namespace RickAndMorty.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection no configurada. Use variables de entorno o User Secrets.");
        }
        var provider = configuration["Database:Provider"] ?? "Sqlite";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlServer(
                    connectionString,
                    sql =>
                    {
                        sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        sql.EnableRetryOnFailure(
                            maxRetryCount: 6,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
            }
            else
            {
                options.UseSqlite(connectionString, sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            }
        });

        services.AddScoped<IPersonajeRepository, PersonajeRepository>();
        services.AddScoped<ISolicitudRepository, SolicitudRepository>();

        services.AddHttpClient<IRickAndMortyApiClient, RickAndMortyHttpApiClient>(client =>
        {
            var baseUrl = configuration["RickAndMortyApi:BaseUrl"] ?? "https://rickandmortyapi.com/api/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
