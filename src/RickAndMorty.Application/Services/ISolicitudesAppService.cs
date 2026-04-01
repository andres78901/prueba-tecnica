using RickAndMorty.Application.Dtos.Solicitudes;

namespace RickAndMorty.Application.Services;

public interface ISolicitudesAppService
{
    Task<SolicitudResponseDto> CrearAsync(CrearSolicitudRequestDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SolicitudResponseDto>> ListarAsync(CancellationToken cancellationToken = default);

    Task<SolicitudResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<SolicitudResponseDto> CambiarEstadoAsync(
        int id,
        CambiarEstadoSolicitudRequestDto request,
        CancellationToken cancellationToken = default);
}
