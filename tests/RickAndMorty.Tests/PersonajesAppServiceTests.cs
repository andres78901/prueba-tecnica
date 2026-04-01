using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RickAndMorty.Application.Abstractions.External;
using RickAndMorty.Application.Abstractions.Repositories;
using RickAndMorty.Application.Dtos.Personajes;
using RickAndMorty.Application.External.Models;
using RickAndMorty.Application.Services;
using RickAndMorty.Domain.Entities;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Tests;

public sealed class PersonajesAppServiceTests
{
    [Fact]
    public async Task ImportarAsync_con_ids_externos_consulta_api_y_persiste_en_repositorio()
    {
        var api = new Mock<IRickAndMortyApiClient>();
        api.Setup(x => x.GetCharacterByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new RickAndMortyCharacterDto
                {
                    Id = 1,
                    Name = "Rick Sanchez",
                    Status = "Alive",
                    Species = "Human",
                    Gender = "Male",
                    Origin = new RickAndMortyNamedResourceDto { Name = "Earth" },
                    Image = "https://example.com/rick.png",
                });

        var repo = new Mock<IPersonajeRepository>();
        repo.Setup(x => x.UpsertManyAsync(It.IsAny<IReadOnlyCollection<Personaje>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var logger = new Mock<ILogger<PersonajesAppService>>();
        var sut = new PersonajesAppService(api.Object, repo.Object, logger.Object);

        var result = await sut.ImportarAsync(
            new ImportarPersonajesRequestDto { ExternalIds = new[] { 1 } });

        result.Importados.Should().Be(1);
        result.ExternalIdsProcesados.Should().Contain(1);
        repo.Verify(
            x => x.UpsertManyAsync(
                It.Is<IReadOnlyCollection<Personaje>>(c => c.Count == 1 && c.First().ExternalId == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportarAsync_sin_ids_externos_consulta_paginacion_y_persiste()
    {
        var api = new Mock<IRickAndMortyApiClient>();
        api.Setup(x => x.GetCharacterPageAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new RickAndMortyCharacterListDto
                {
                    Results = new[]
                    {
                        new RickAndMortyCharacterDto
                        {
                            Id = 42,
                            Name = "Morty Smith",
                            Status = "Alive",
                            Species = "Human",
                            Gender = "Male",
                            Origin = new RickAndMortyNamedResourceDto { Name = "Earth" },
                            Image = "https://example.com/morty.png",
                        },
                    },
                });
        api.Setup(x => x.GetCharacterPageAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RickAndMortyCharacterListDto?)null);

        var repo = new Mock<IPersonajeRepository>();
        repo.Setup(x => x.UpsertManyAsync(It.IsAny<IReadOnlyCollection<Personaje>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var logger = new Mock<ILogger<PersonajesAppService>>();
        var sut = new PersonajesAppService(api.Object, repo.Object, logger.Object);

        var result = await sut.ImportarAsync(
            new ImportarPersonajesRequestDto { ExternalIds = null, MaxPaginasApi = 1 });

        result.Importados.Should().Be(1);
        result.ExternalIdsProcesados.Should().Contain(42);
        api.Verify(x => x.GetCharacterPageAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(
            x => x.UpsertManyAsync(
                It.Is<IReadOnlyCollection<Personaje>>(c =>
                    c.Count == 1 && c.Single().ExternalId == 42 && c.Single().Nombre == "Morty Smith"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
