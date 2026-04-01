using FluentValidation;
using RickAndMorty.Application.Dtos.Personajes;

namespace RickAndMorty.Application.Validation;

public sealed class ImportarPersonajesRequestValidator : AbstractValidator<ImportarPersonajesRequestDto>
{
    public ImportarPersonajesRequestValidator()
    {
        RuleFor(x => x.MaxPaginasApi)
            .InclusiveBetween(1, 50)
            .When(x => x.MaxPaginasApi.HasValue);

        RuleForEach(x => x.ExternalIds!)
            .GreaterThan(0)
            .When(x => x.ExternalIds is { Count: > 0 });
    }
}
