using BibliotecaAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace BibliotecaAPI.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(BibliotecaContext database, IDistributedCache cache) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var databaseOk = false;
        var redisOk = false;

        try
        {
            databaseOk = await database.Database.CanConnectAsync(cancellationToken);
        }
        catch
        {
            // O payload abaixo informa a indisponibilidade sem vazar detalhes internos.
        }

        try
        {
            const string key = "health:redis";
            var value = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            await cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            }, cancellationToken);
            redisOk = await cache.GetStringAsync(key, cancellationToken) == value;
        }
        catch
        {
            // O healthcheck deve responder mesmo quando o Redis estiver indisponível.
        }

        var result = new
        {
            api = "running",
            database = databaseOk ? "running" : "unavailable",
            redis = redisOk ? "running" : "unavailable",
            checkedAtUtc = DateTime.UtcNow
        };

        return databaseOk && redisOk
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }
}
