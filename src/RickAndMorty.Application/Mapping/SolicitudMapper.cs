using RickAndMorty.Application.Dtos.Solicitudes;
using RickAndMorty.Domain.Entities;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Application.Mapping;

public static class SolicitudMapper
{
    public static Solicitud ToEntity(CrearSolicitudRequestDto dto, DateTime nowUtc) =>
        new()
        {
            IdExterno = dto.IdExterno,
            PersonajeId = dto.PersonajeId,
            Solicitante = dto.Solicitante,
            Evento = dto.Evento,
            FechaEvento = dto.FechaEvento,
            Estado = SolicitudEstado.Pendiente,
            FechaCreacion = nowUtc,
            FechaActualizacion = nowUtc,
        };

    public static SolicitudResponseDto ToResponseDto(Solicitud entity) =>
        new()
        {
            Id = entity.Id,
            IdExterno = entity.IdExterno,
            PersonajeId = entity.PersonajeId,
            PersonajeNombre = entity.Personaje?.Nombre,
            Solicitante = entity.Solicitante,
            Evento = entity.Evento,
            FechaEvento = entity.FechaEvento,
            Estado = entity.Estado,
            FechaCreacion = entity.FechaCreacion,
            FechaActualizacion = entity.FechaActualizacion,
        };
}
