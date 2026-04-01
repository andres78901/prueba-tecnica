using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Application.Services;
using RickAndMorty.Application.Validation;

namespace RickAndMorty.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPersonajesAppService, PersonajesAppService>();
        services.AddScoped<ISolicitudesAppService, SolicitudesAppService>();
        services.AddScoped<IReportesAppService, ReportesAppService>();

        services.AddValidatorsFromAssemblyContaining<ImportarPersonajesRequestValidator>();

        return services;
    }
}
