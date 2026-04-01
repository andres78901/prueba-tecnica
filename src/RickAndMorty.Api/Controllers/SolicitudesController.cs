using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Application.Dtos.Solicitudes;
using RickAndMorty.Application.Services;

namespace RickAndMorty.Api.Controllers;

[ApiController]
[Route("api/solicitudes")]
public sealed class SolicitudesController : ControllerBase
{
    private readonly ISolicitudesAppService _solicitudesAppService;

    public SolicitudesController(ISolicitudesAppService solicitudesAppService)
    {
        _solicitudesAppService = solicitudesAppService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(SolicitudResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SolicitudResponseDto>> Crear(
        [FromBody] CrearSolicitudRequestDto request,
        CancellationToken cancellationToken)
    {
        var created = await _solicitudesAppService.CrearAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Obtener), new { id = created.Id }, created);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SolicitudResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SolicitudResponseDto>>> Listar(CancellationToken cancellationToken)
    {
        var list = await _solicitudesAppService.ListarAsync(cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SolicitudResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolicitudResponseDto>> Obtener(int id, CancellationToken cancellationToken)
    {
        var dto = await _solicitudesAppService.ObtenerPorIdAsync(id, cancellationToken);
        return Ok(dto);
    }

    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(SolicitudResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SolicitudResponseDto>> CambiarEstado(
        int id,
        [FromBody] CambiarEstadoSolicitudRequestDto request,
        CancellationToken cancellationToken)
    {
        var updated = await _solicitudesAppService.CambiarEstadoAsync(id, request, cancellationToken);
        return Ok(updated);
    }
}
