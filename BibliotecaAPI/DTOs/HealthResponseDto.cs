using System.Text.Json.Serialization;

namespace BibliotecaAPI.DTOs;

public sealed record HealthResponseDto(
    string Api,
    string Database,
    string Redis,
    DateTime CheckedAtUtc)
{
    [JsonIgnore]
    public bool IsHealthy => Database == "running" && Redis == "running";
}
