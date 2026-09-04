using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Services;

public interface IAuthService
{
    LoginResponseDto? Authenticate(LoginRequestDto request);
}
