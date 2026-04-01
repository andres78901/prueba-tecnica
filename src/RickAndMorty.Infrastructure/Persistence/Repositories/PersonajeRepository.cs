using Microsoft.EntityFrameworkCore;
using RickAndMorty.Application.Abstractions.Repositories;
using RickAndMorty.Application.Common.Pagination;
using RickAndMorty.Domain.Entities;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Infrastructure.Persistence.Repositories;

public sealed class PersonajeRepository : IPersonajeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PersonajeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Personaje?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _dbContext.Personajes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Personaje?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default) =>
        _dbContext.Personajes.AsNoTracking().FirstOrDefaultAsync(p => p.ExternalId == externalId, cancellationToken);

    public async Task<PagedResult<Personaje>> GetPagedAsync(
        int page,
        int pageSize,
        string? nombre,
        PersonajeEstado? estado,
        string? especie,
        string? genero,
        string? origen,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Personajes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            var n = nombre.Trim();
            query = query.Where(p => p.Nombre.Contains(n));
        }

        if (estado.HasValue)
        {
            var e = estado.Value;
            query = query.Where(p => p.Estado == e);
        }

        if (!string.IsNullOrWhiteSpace(especie))
        {
            var esp = especie.Trim();
            query = query.Where(p => p.Especie.Contains(esp));
        }

        if (!string.IsNullOrWhiteSpace(genero))
        {
            var g = genero.Trim();
            query = query.Where(p => p.Genero.Contains(g));
        }

        if (!string.IsNullOrWhiteSpace(origen))
        {
            var o = origen.Trim();
            query = query.Where(p => p.Origen.Contains(o));
        }

        var total = await query.CountAsync(cancellationToken);
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);
        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Personaje>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        };
    }

    public async Task<int> UpsertManyAsync(
        IReadOnlyCollection<Personaje> personajes,
        CancellationToken cancellationToken = default)
    {
        var processed = 0;
        foreach (var p in personajes)
        {
            var existing = await _dbContext.Personajes
                .FirstOrDefaultAsync(x => x.ExternalId == p.ExternalId, cancellationToken);

            if (existing is null)
            {
                await _dbContext.Personajes.AddAsync(p, cancellationToken);
            }
            else
            {
                existing.Nombre = p.Nombre;
                existing.Estado = p.Estado;
                existing.Especie = p.Especie;
                existing.Genero = p.Genero;
                existing.Origen = p.Origen;
                existing.ImagenUrl = p.ImagenUrl;
                existing.FechaImport = p.FechaImport;
            }

            processed++;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return processed;
    }
}
