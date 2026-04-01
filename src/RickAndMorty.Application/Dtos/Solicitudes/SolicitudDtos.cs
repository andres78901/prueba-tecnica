using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Application.Dtos.Solicitudes;

public sealed class CrearSolicitudRequestDto
{
    public string? IdExterno { get; init; }

    public int PersonajeId { get; init; }

    public string Solicitante { get; init; } = string.Empty;

    public string Evento { get; init; } = string.Empty;

    public DateTime FechaEvento { get; init; }
}

public sealed class CambiarEstadoSolicitudRequestDto
{
    public SolicitudEstado Estado { get; init; }
}

public sealed class SolicitudResponseDto
{
    public int Id { get; init; }

    public string? IdExterno { get; init; }

    public int PersonajeId { get; init; }

    public string? PersonajeNombre { get; init; }

    public string Solicitante { get; init; } = string.Empty;

    public string Evento { get; init; } = string.Empty;

    public DateTime FechaEvento { get; init; }

    public SolicitudEstado Estado { get; init; }

    public DateTime FechaCreacion { get; init; }

    public DateTime FechaActualizacion { get; init; }
}
