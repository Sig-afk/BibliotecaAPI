using BibliotecaAPI.Configuration;
using BibliotecaAPI.DTOs;
using Microsoft.Extensions.Options;

namespace BibliotecaAPI.Services;

public sealed class AuthService(IOptions<DemoAuthOptions> options) : IAuthService
{
    private readonly DemoAuthOptions _settings = options.Value;

    public LoginResponseDto? Authenticate(LoginRequestDto request)
    {
        var validEmail = string.Equals(
            request.Email.Trim(),
            _settings.Email,
            StringComparison.OrdinalIgnoreCase);

        var validPassword = string.Equals(
            request.Senha,
            _settings.Password,
            StringComparison.Ordinal);

        if (!validEmail || !validPassword)
            return null;

        var user = new AuthenticatedUserDto(_settings.Name, _settings.Email, "Administrador");
        return new LoginResponseDto(true, user);
    }
}
