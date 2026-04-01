using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Application.Dtos.Personajes;
using RickAndMorty.Application.Services;
using RickAndMorty.Domain.Enums;

namespace RickAndMorty.Api.Controllers;

[ApiController]
[Route("api/personajes")]
public sealed class PersonajesController : ControllerBase
{
    private readonly IPersonajesAppService _personajesAppService;

    public PersonajesController(IPersonajesAppService personajesAppService)
    {
        _personajesAppService = personajesAppService;
    }

    [HttpPost("importar")]
    [ProducesResponseType(typeof(ImportarPersonajesResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ImportarPersonajesResultDto>> Importar(
        [FromBody] ImportarPersonajesRequestDto? request,
        CancellationToken cancellationToken)
    {
        request ??= new ImportarPersonajesRequestDto();
        var result = await _personajesAppService.ImportarAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PersonajesListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PersonajesListResponse>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? nombre = null,
        [FromQuery] PersonajeEstado? estado = null,
        [FromQuery] string? especie = null,
        [FromQuery] string? genero = null,
        [FromQuery] string? origen = null,
        CancellationToken cancellationToken = default)
    {
        var paged = await _personajesAppService.ListarAsync(
            page,
            pageSize,
            nombre,
            estado,
            especie,
            genero,
            origen,
            cancellationToken);

        return Ok(new PersonajesListResponse
        {
            Items = paged.Items,
            Page = paged.Page,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount,
            TotalPages = paged.TotalPages,
            HasPreviousPage = paged.HasPreviousPage,
            HasNextPage = paged.HasNextPage,
        });
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PersonajeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonajeResponseDto>> Obtener(int id, CancellationToken cancellationToken)
    {
        var dto = await _personajesAppService.ObtenerPorIdAsync(id, cancellationToken);
        return Ok(dto);
    }
}
