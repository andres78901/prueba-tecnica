using Microsoft.EntityFrameworkCore;
using RickAndMorty.Application.Abstractions.Repositories;
using RickAndMorty.Domain.Entities;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Infrastructure.Persistence.Repositories;

public sealed class SolicitudRepository : ISolicitudRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SolicitudRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Solicitud?> GetByIdAsync(
        int id,
        bool includePersonaje = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Solicitudes.AsQueryable();
        if (includePersonaje)
        {
            query = query.Include(s => s.Personaje);
        }

        return query.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Solicitud>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Solicitudes
            .AsNoTracking()
            .Include(s => s.Personaje)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync(cancellationToken);

    public async Task<Solicitud> AddAsync(Solicitud solicitud, CancellationToken cancellationToken = default)
    {
        await _dbContext.Solicitudes.AddAsync(solicitud, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return solicitud;
    }

    public async Task UpdateAsync(Solicitud solicitud, CancellationToken cancellationToken = default)
    {
        _dbContext.Solicitudes.Update(solicitud);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<SolicitudEstado, int>> CountByEstadoAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await _dbContext.Solicitudes
            .AsNoTracking()
            .GroupBy(s => s.Estado)
            .Select(g => new { Estado = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(x => x.Estado, x => x.Count);
    }
}
