using Microsoft.Extensions.Logging;
using RickAndMorty.Application.Abstractions.External;
using RickAndMorty.Application.Abstractions.Repositories;
using RickAndMorty.Application.Common.Exceptions;
using RickAndMorty.Application.Common.Pagination;
using RickAndMorty.Application.Dtos.Personajes;
using RickAndMorty.Application.External.Models;
using RickAndMorty.Application.Mapping;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Application.Services;

public sealed class PersonajesAppService : IPersonajesAppService
{
    private readonly IRickAndMortyApiClient _apiClient;
    private readonly IPersonajeRepository _personajeRepository;
    private readonly ILogger<PersonajesAppService> _logger;

    public PersonajesAppService(
        IRickAndMortyApiClient apiClient,
        IPersonajeRepository personajeRepository,
        ILogger<PersonajesAppService> logger)
    {
        _apiClient = apiClient;
        _personajeRepository = personajeRepository;
        _logger = logger;
    }

    public async Task<ImportarPersonajesResultDto> ImportarAsync(
        ImportarPersonajesRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var fechaUtc = DateTime.UtcNow;
        var dtos = new List<RickAndMortyCharacterDto>();

        if (request.ExternalIds is { Count: > 0 })
        {
            foreach (var id in request.ExternalIds.Distinct())
            {
                var c = await _apiClient.GetCharacterByIdAsync(id, cancellationToken);
                if (c is null)
                {
                    _logger.LogWarning("Personaje externo {ExternalId} no encontrado en la API", id);
                    continue;
                }

                dtos.Add(c);
            }
        }
        else
        {
            var maxPages = Math.Clamp(request.MaxPaginasApi ?? 1, 1, 50);
            for (var page = 1; page <= maxPages; page++)
            {
                var listPage = await _apiClient.GetCharacterPageAsync(page, cancellationToken);
                if (listPage?.Results is not { Count: > 0 })
                {
                    break;
                }

                dtos.AddRange(listPage.Results!);
            }
        }

        var entities = dtos.Select(d => PersonajeMapper.ToEntity(d, fechaUtc)).ToList();
        var guardados = await _personajeRepository.UpsertManyAsync(entities, cancellationToken);

        _logger.LogInformation("Importación finalizada: {Count} registros persistidos", guardados);

        return new ImportarPersonajesResultDto
        {
            Importados = guardados,
            ExternalIdsProcesados = entities.Select(e => e.ExternalId).Distinct().ToList(),
        };
    }

    public async Task<PagedResult<PersonajeResponseDto>> ListarAsync(
        int page,
        int pageSize,
        string? nombre,
        PersonajeEstado? estado,
        string? especie,
        string? genero,
        string? origen,
        CancellationToken cancellationToken = default)
    {
        var paged = await _personajeRepository.GetPagedAsync(
            page,
            pageSize,
            nombre,
            estado,
            especie,
            genero,
            origen,
            cancellationToken);

        return new PagedResult<PersonajeResponseDto>
        {
            Items = paged.Items.Select(PersonajeMapper.ToResponseDto).ToList(),
            Page = paged.Page,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount,
        };
    }

    public async Task<PersonajeResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _personajeRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException($"No existe personaje con id {id}.");
        }

        return PersonajeMapper.ToResponseDto(entity);
    }
}
