using FluentValidation;
using RickAndMorty.Application.Dtos.Solicitudes;

namespace RickAndMorty.Application.Validation;

public sealed class CrearSolicitudRequestValidator : AbstractValidator<CrearSolicitudRequestDto>
{
    public CrearSolicitudRequestValidator()
    {
        RuleFor(x => x.PersonajeId).GreaterThan(0);

        RuleFor(x => x.Solicitante)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Evento)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.FechaEvento)
            .Must(dt => dt <= DateTime.UtcNow.AddMinutes(5))
            .WithMessage("FechaEvento no puede ser futura.");

        RuleFor(x => x.IdExterno).MaximumLength(100).When(x => x.IdExterno is not null);
    }
}
