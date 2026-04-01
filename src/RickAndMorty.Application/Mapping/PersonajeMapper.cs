using RickAndMorty.Application.Dtos.Personajes;
using RickAndMorty.Application.External.Models;
using RickAndMorty.Domain.Entities;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Application.Mapping;

public static class PersonajeMapper
{
    public static Personaje ToEntity(RickAndMortyCharacterDto dto, DateTime fechaImportUtc)
    {
        return new Personaje
        {
            ExternalId = dto.Id,
            Nombre = dto.Name,
            Estado = MapEstado(dto.Status),
            Especie = dto.Species,
            Genero = dto.Gender,
            Origen = dto.Origin?.Name ?? string.Empty,
            ImagenUrl = dto.Image,
            FechaImport = fechaImportUtc,
        };
    }

    public static PersonajeResponseDto ToResponseDto(Personaje entity) =>
        new()
        {
            Id = entity.Id,
            ExternalId = entity.ExternalId,
            Nombre = entity.Nombre,
            Estado = entity.Estado,
            Especie = entity.Especie,
            Genero = entity.Genero,
            Origen = entity.Origen,
            ImagenUrl = entity.ImagenUrl,
            FechaImport = entity.FechaImport,
        };

    private static PersonajeEstado MapEstado(string status)
    {
        return status.Trim().ToLowerInvariant() switch
        {
            "alive" => PersonajeEstado.Alive,
            "dead" => PersonajeEstado.Dead,
            _ => PersonajeEstado.Unknown,
        };
    }
}
