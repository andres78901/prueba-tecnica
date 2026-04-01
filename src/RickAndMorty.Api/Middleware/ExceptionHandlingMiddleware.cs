using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RickAndMorty.Application.Common.Exceptions;

namespace RickAndMorty.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, message, errors) = MapException(exception, _environment.IsDevelopment());
        if (status == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Error no controlado en {Path}", context.Request.Path.Value);
        }
        else
        {
            _logger.LogWarning(exception, "Petición rechazada: {Message}", message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var payload = new ErrorResponse(message, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private static (HttpStatusCode Status, string Message, IReadOnlyList<string>? Errors) MapException(
        Exception exception,
        bool isDevelopment) =>
        exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                "Error de validación",
                ve.Errors.Select(e => e.ErrorMessage).ToList()),

            NotFoundException nf => (HttpStatusCode.NotFound, nf.Message, null),
            BusinessRuleException br => (HttpStatusCode.Conflict, br.Message, null),
            AppException app => (HttpStatusCode.BadRequest, app.Message, null),
            DbUpdateException dbe when isDevelopment => (
                HttpStatusCode.InternalServerError,
                "Error al guardar en la base de datos (detalle en desarrollo).",
                new[] { GetInnermostMessage(dbe) }),
            _ => (
                HttpStatusCode.InternalServerError,
                "Ha ocurrido un error interno. Intente nuevamente más tarde.",
                null),
        };

    private static string GetInnermostMessage(Exception ex)
    {
        while (ex.InnerException is not null)
        {
            ex = ex.InnerException;
        }

        return ex.Message;
    }

    private sealed record ErrorResponse(string Message, IReadOnlyList<string>? Errors);
}
