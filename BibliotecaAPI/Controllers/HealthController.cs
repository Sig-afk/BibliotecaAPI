using BibliotecaAPI.DTOs;
using BibliotecaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController(IHealthService healthService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<HealthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<HealthResponseDto>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<HealthResponseDto>> Get(CancellationToken cancellationToken)
    {
        var result = await healthService.CheckAsync(cancellationToken);
        return result.IsHealthy
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }
}
