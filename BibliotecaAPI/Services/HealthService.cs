using BibliotecaAPI.Data;
using BibliotecaAPI.DTOs;
using Microsoft.Extensions.Caching.Distributed;

namespace BibliotecaAPI.Services;

public sealed class HealthService(
    BibliotecaContext database,
    IDistributedCache cache,
    ILogger<HealthService> logger) : IHealthService
{
    public async Task<HealthResponseDto> CheckAsync(CancellationToken cancellationToken = default)
    {
        var databaseOk = await CanConnectToDatabaseAsync(cancellationToken);
        var redisOk = await CanConnectToRedisAsync(cancellationToken);

        return new HealthResponseDto(
            "running",
            databaseOk ? "running" : "unavailable",
            redisOk ? "running" : "unavailable",
            DateTime.UtcNow);
    }

    private async Task<bool> CanConnectToDatabaseAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await database.Database.CanConnectAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Falha ao verificar a conexão com o banco de dados.");
            return false;
        }
    }

    private async Task<bool> CanConnectToRedisAsync(CancellationToken cancellationToken)
    {
        try
        {
            const string key = "health:redis";
            var value = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var expiration = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            };

            await cache.SetStringAsync(key, value, expiration, cancellationToken);
            return await cache.GetStringAsync(key, cancellationToken) == value;
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Falha ao verificar a conexão com o Redis.");
            return false;
        }
    }
}
