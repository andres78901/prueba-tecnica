using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Application.Dtos.Reportes;

public sealed class SolicitudesResumenDto
{
    public int Total { get; init; }

    public IReadOnlyDictionary<SolicitudEstado, int> PorEstado { get; init; } =
        new Dictionary<SolicitudEstado, int>();
}
