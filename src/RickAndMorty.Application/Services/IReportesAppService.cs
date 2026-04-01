using RickAndMorty.Application.Dtos.Reportes;

namespace RickAndMorty.Application.Services;

public interface IReportesAppService
{
    Task<SolicitudesResumenDto> ObtenerResumenSolicitudesAsync(CancellationToken cancellationToken = default);
}
