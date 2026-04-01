using RickAndMorty.Domain.Entities;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Application.Abstractions.Repositories;

public interface ISolicitudRepository
{
    Task<Solicitud?> GetByIdAsync(
        int id,
        bool includePersonaje = true,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Solicitud>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Solicitud> AddAsync(Solicitud solicitud, CancellationToken cancellationToken = default);

    Task UpdateAsync(Solicitud solicitud, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<SolicitudEstado, int>> CountByEstadoAsync(CancellationToken cancellationToken = default);
}
