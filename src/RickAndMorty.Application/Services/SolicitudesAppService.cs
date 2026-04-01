using Microsoft.Extensions.Logging;
using RickAndMorty.Application.Abstractions.Repositories;
using RickAndMorty.Application.Common.Exceptions;
using RickAndMorty.Application.Dtos.Solicitudes;
using RickAndMorty.Application.Mapping;
using RickAndMorty.Domain.Rules;

namespace RickAndMorty.Application.Services;

public sealed class SolicitudesAppService : ISolicitudesAppService
{
    private readonly ISolicitudRepository _solicitudRepository;
    private readonly IPersonajeRepository _personajeRepository;
    private readonly ILogger<SolicitudesAppService> _logger;

    public SolicitudesAppService(
        ISolicitudRepository solicitudRepository,
        IPersonajeRepository personajeRepository,
        ILogger<SolicitudesAppService> logger)
    {
        _solicitudRepository = solicitudRepository;
        _personajeRepository = personajeRepository;
        _logger = logger;
    }

    public async Task<SolicitudResponseDto> CrearAsync(
        CrearSolicitudRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var personaje = await _personajeRepository.GetByIdAsync(request.PersonajeId, cancellationToken);
        if (personaje is null)
        {
            throw new NotFoundException($"No existe personaje con id {request.PersonajeId}.");
        }

        var now = DateTime.UtcNow;
        var entity = SolicitudMapper.ToEntity(request, now);
        var created = await _solicitudRepository.AddAsync(entity, cancellationToken);

        created.Personaje = personaje;
        _logger.LogInformation("Solicitud creada con id {Id}", created.Id);

        return SolicitudMapper.ToResponseDto(created);
    }

    public async Task<IReadOnlyList<SolicitudResponseDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var list = await _solicitudRepository.GetAllAsync(cancellationToken);
        return list.Select(SolicitudMapper.ToResponseDto).ToList();
    }

    public async Task<SolicitudResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _solicitudRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException($"No existe solicitud con id {id}.");
        }

        return SolicitudMapper.ToResponseDto(entity);
    }

    public async Task<SolicitudResponseDto> CambiarEstadoAsync(
        int id,
        CambiarEstadoSolicitudRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _solicitudRepository.GetByIdAsync(id, includePersonaje: false, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException($"No existe solicitud con id {id}.");
        }

        if (entity.Estado == request.Estado)
        {
            _logger.LogInformation("Solicitud {Id} ya está en estado {Estado}; sin cambios.", id, entity.Estado);
            var sinCambios = await _solicitudRepository.GetByIdAsync(id, includePersonaje: true, cancellationToken);
            return SolicitudMapper.ToResponseDto(sinCambios!);
        }

        if (!SolicitudEstadoTransitions.IsValid(entity.Estado, request.Estado))
        {
            throw new BusinessRuleException(
                $"Transición de estado no permitida: de {entity.Estado} a {request.Estado}.");
        }

        entity.Estado = request.Estado;
        entity.FechaActualizacion = DateTime.UtcNow;
        await _solicitudRepository.UpdateAsync(entity, cancellationToken);

        _logger.LogInformation("Solicitud {Id} actualizada a estado {Estado}", id, entity.Estado);

        var actualizada = await _solicitudRepository.GetByIdAsync(id, includePersonaje: true, cancellationToken);
        return SolicitudMapper.ToResponseDto(actualizada!);
    }
}
