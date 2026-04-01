using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using RickAndMorty.Application.Abstractions.External;
using RickAndMorty.Application.External.Models;

namespace RickAndMorty.Infrastructure.External;

public sealed class RickAndMortyHttpApiClient : IRickAndMortyApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RickAndMortyHttpApiClient> _logger;

    public RickAndMortyHttpApiClient(HttpClient httpClient, ILogger<RickAndMortyHttpApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<RickAndMortyCharacterDto?> GetCharacterByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"character/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RickAndMortyCharacterDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<RickAndMortyCharacterListDto?> GetCharacterPageAsync(
        int page,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"character?page={page}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Fallo al obtener página {Page} de personajes: {Status}", page, response.StatusCode);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<RickAndMortyCharacterListDto>(
            cancellationToken: cancellationToken);
    }
}
