using FluentValidation;
using RickAndMorty.Application.Dtos.Solicitudes;

namespace RickAndMorty.Application.Validation;

public sealed class CambiarEstadoSolicitudRequestValidator : AbstractValidator<CambiarEstadoSolicitudRequestDto>
{
    public CambiarEstadoSolicitudRequestValidator()
    {
        RuleFor(x => x.Estado).IsInEnum();
    }
}
