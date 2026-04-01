using RickAndMorty.Application.Common.Pagination;
using RickAndMorty.Application.Dtos.Personajes;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Application.Services;

public interface IPersonajesAppService
{
    Task<ImportarPersonajesResultDto> ImportarAsync(
        ImportarPersonajesRequestDto request,
        CancellationToken cancellationToken = default);

    Task<PagedResult<PersonajeResponseDto>> ListarAsync(
        int page,
        int pageSize,
        string? nombre,
        PersonajeEstado? estado,
        string? especie,
        string? genero,
        string? origen,
        CancellationToken cancellationToken = default);

    Task<PersonajeResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
}
