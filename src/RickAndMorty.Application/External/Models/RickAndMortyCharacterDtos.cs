using System.Text.Json.Serialization;

namespace RickAndMorty.Application.External.Models;

public sealed class RickAndMortyCharacterListDto
{
    [JsonPropertyName("info")]
    public RickAndMortyPageInfoDto? Info { get; init; }

    [JsonPropertyName("results")]
    public IReadOnlyList<RickAndMortyCharacterDto>? Results { get; init; }
}

public sealed class RickAndMortyPageInfoDto
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("pages")]
    public int Pages { get; init; }

    [JsonPropertyName("next")]
    public string? Next { get; init; }

    [JsonPropertyName("prev")]
    public string? Prev { get; init; }
}

public sealed class RickAndMortyCharacterDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("species")]
    public string Species { get; init; } = string.Empty;

    [JsonPropertyName("gender")]
    public string Gender { get; init; } = string.Empty;

    [JsonPropertyName("origin")]
    public RickAndMortyNamedResourceDto? Origin { get; init; }

    [JsonPropertyName("image")]
    public string Image { get; init; } = string.Empty;
}

public sealed class RickAndMortyNamedResourceDto
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
