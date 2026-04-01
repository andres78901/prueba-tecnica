using RickAndMorty.Application.External.Models;

namespace RickAndMorty.Application.Abstractions.External;

public interface IRickAndMortyApiClient
{
    Task<RickAndMortyCharacterDto?> GetCharacterByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<RickAndMortyCharacterListDto?> GetCharacterPageAsync(int page, CancellationToken cancellationToken = default);
}
