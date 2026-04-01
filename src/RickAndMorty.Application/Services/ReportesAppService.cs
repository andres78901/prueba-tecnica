using RickAndMorty.Application.Abstractions.Repositories;
using RickAndMorty.Application.Dtos.Reportes;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Application.Services;

public sealed class ReportesAppService : IReportesAppService
{
    private readonly ISolicitudRepository _solicitudRepository;

    public ReportesAppService(ISolicitudRepository solicitudRepository)
    {
        _solicitudRepository = solicitudRepository;
    }

    public async Task<SolicitudesResumenDto> ObtenerResumenSolicitudesAsync(
        CancellationToken cancellationToken = default)
    {
        var counts = await _solicitudRepository.CountByEstadoAsync(cancellationToken);
        var porEstado = Enum.GetValues<SolicitudEstado>()
            .ToDictionary(e => e, e => counts.TryGetValue(e, out var c) ? c : 0);

        return new SolicitudesResumenDto
        {
            Total = porEstado.Values.Sum(),
            PorEstado = porEstado,
        };
    }
}
