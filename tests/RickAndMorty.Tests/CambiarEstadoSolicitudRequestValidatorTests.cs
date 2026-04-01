using FluentAssertions;
using FluentValidation.TestHelper;
using RickAndMorty.Application.Dtos.Solicitudes;
using RickAndMorty.Application.Validation;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Tests;

/// <summary>
/// Pruebas unitarias del validador de cambio de estado de solicitud.
/// </summary>
public sealed class CambiarEstadoSolicitudRequestValidatorTests
{
    private readonly CambiarEstadoSolicitudRequestValidator _sut = new();

    [Fact]
    public void Debe_aprobar_cuando_estado_es_valor_del_enum()
    {
        var model = new CambiarEstadoSolicitudRequestDto { Estado = SolicitudEstado.EnProceso };

        var result = _sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Debe_fallar_cuando_estado_no_es_miembro_del_enum()
    {
        var model = new CambiarEstadoSolicitudRequestDto { Estado = (SolicitudEstado)999 };

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Estado);
    }
}
