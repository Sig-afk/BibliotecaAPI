using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Services;

public interface IHealthService
{
    Task<HealthResponseDto> CheckAsync(CancellationToken cancellationToken = default);
}
