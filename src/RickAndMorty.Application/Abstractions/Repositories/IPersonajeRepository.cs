using RickAndMorty.Application.Common.Pagination;
using RickAndMorty.Domain.Entities;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Application.Abstractions.Repositories;

public interface IPersonajeRepository
{
    Task<Personaje?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Personaje?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default);

    Task<PagedResult<Personaje>> GetPagedAsync(
        int page,
        int pageSize,
        string? nombre,
        PersonajeEstado? estado,
        string? especie,
        string? genero,
        string? origen,
        CancellationToken cancellationToken = default);

    Task<int> UpsertManyAsync(IReadOnlyCollection<Personaje> personajes, CancellationToken cancellationToken = default);
}
