using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Application.Dtos.Reportes;
using RickAndMorty.Application.Services;

namespace RickAndMorty.Api.Controllers;

[ApiController]
[Route("api/reportes")]
public sealed class ReportesController : ControllerBase
{
    private readonly IReportesAppService _reportesAppService;

    public ReportesController(IReportesAppService reportesAppService)
    {
        _reportesAppService = reportesAppService;
    }

    [HttpGet("solicitudes-resumen")]
    [ProducesResponseType(typeof(SolicitudesResumenDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SolicitudesResumenDto>> SolicitudesResumen(CancellationToken cancellationToken)
    {
        var resumen = await _reportesAppService.ObtenerResumenSolicitudesAsync(cancellationToken);
        return Ok(resumen);
    }
}
