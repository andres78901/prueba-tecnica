using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Application.Dtos.Personajes;

public sealed class ImportarPersonajesRequestDto
{
    public IReadOnlyList<int>? ExternalIds { get; init; }

    public int? MaxPaginasApi { get; init; }
}

public sealed class ImportarPersonajesResultDto
{
    public int Importados { get; init; }

    public IReadOnlyList<int> ExternalIdsProcesados { get; init; } = Array.Empty<int>();
}

public sealed class PersonajeResponseDto
{
    public int Id { get; init; }

    public int ExternalId { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public PersonajeEstado Estado { get; init; }

    public string Especie { get; init; } = string.Empty;

    public string Genero { get; init; } = string.Empty;

    public string Origen { get; init; } = string.Empty;

    public string ImagenUrl { get; init; } = string.Empty;

    public DateTime FechaImport { get; init; }
}

/// <summary>
/// Respuesta paginada del listado de personajes expuesta por la API.
/// </summary>
public sealed class PersonajesListResponse
{
    public IReadOnlyList<PersonajeResponseDto> Items { get; init; } = Array.Empty<PersonajeResponseDto>();

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }

    public bool HasPreviousPage { get; init; }

    public bool HasNextPage { get; init; }
}
