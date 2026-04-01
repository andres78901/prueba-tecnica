using System.Text.Json.Serialization;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RickAndMorty.Api.Middleware;
using RickAndMorty.Application;
using RickAndMorty.Infrastructure;
using RickAndMorty.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Key Vault opcional: si hay URI, se cargan secretos encima del resto (auth con DefaultAzureCredential).
var keyVaultUri =
    builder.Configuration["KeyVault:VaultUri"]
    ?? builder.Configuration["KEYVAULT_VAULTURI"];
if (Uri.TryCreate(keyVaultUri, UriKind.Absolute, out var vaultUri))
{
    builder.Configuration.AddAzureKeyVault(
        vaultUri,
        new DefaultAzureCredential());
}

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Rick and Morty - API interna",
        Version = "v1",
        Description = "Importación desde la API pública, solic y reportes.",
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "Error de validación." : e.ErrorMessage)
            .ToList();
        return new BadRequestObjectResult(new { message = "Error de validación", errors });
    };
});

builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var migrateLogger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Database.Migrate");

    const int maxAttempts = 12;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await db.Database.MigrateAsync();
            if (attempt > 1)
            {
                migrateLogger.LogInformation("Migraciones aplicadas tras {Attempt} intento(s).", attempt);
            }

            break;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            var delay = TimeSpan.FromSeconds(Math.Min(20, 2 * attempt));
            migrateLogger.LogWarning(
                ex,
                "No se pudieron aplicar migraciones (intento {Attempt}/{Max}). Reintentando en {Delay}s…",
                attempt,
                maxAttempts,
                delay.TotalSeconds);
            await Task.Delay(delay);
        }
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// En desarrollo suele usarse solo HTTP (p. ej. puerto 5294); sin puerto HTTPS el middleware emite el warning "Failed to determine the https port".
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();

public partial class Program
{
}
