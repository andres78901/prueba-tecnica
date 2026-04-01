using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Domain.Entities;

public class Solicitud
{
    public int Id { get; set; }

    public string? IdExterno { get; set; }

    public int PersonajeId { get; set; }

    public Personaje? Personaje { get; set; }

    public string Solicitante { get; set; } = string.Empty;

    public string Evento { get; set; } = string.Empty;

    public DateTime FechaEvento { get; set; }

    public SolicitudEstado Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }
}
