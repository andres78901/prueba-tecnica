using FluentAssertions;
using FluentValidation.TestHelper;
using RickAndMorty.Application.Dtos.Solicitudes;
using RickAndMorty.Application.Validation;

namespace RickAndMorty.Tests;

/// <summary>
/// Pruebas unitarias del validador de creación de solicitud.
/// </summary>
public sealed class CrearSolicitudRequestValidatorTests
{
    private readonly CrearSolicitudRequestValidator _sut = new();

    [Fact]
    public void Debe_fallar_cuando_solicitante_esta_vacio()
    {
        var model = new CrearSolicitudRequestDto
        {
            PersonajeId = 1,
            Solicitante = string.Empty,
            Evento = "Evento",
            FechaEvento = DateTime.UtcNow,
        };

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Solicitante);
    }

    [Fact]
    public void Debe_fallar_cuando_fecha_evento_es_muy_futura()
    {
        var model = new CrearSolicitudRequestDto
        {
            PersonajeId = 1,
            Solicitante = "Ana",
            Evento = "Evento",
            FechaEvento = DateTime.UtcNow.AddHours(2),
        };

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.FechaEvento)
            .WithErrorMessage("FechaEvento no puede ser futura.");
    }

    [Fact]
    public void Debe_aprobar_cuando_datos_son_validos()
    {
        var model = new CrearSolicitudRequestDto
        {
            PersonajeId = 10,
            Solicitante = "Ana López",
            Evento = "Revisión",
            FechaEvento = DateTime.UtcNow,
        };

        var result = _sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
