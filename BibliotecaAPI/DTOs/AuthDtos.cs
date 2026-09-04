using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs;

public sealed class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;
}

public sealed record AuthenticatedUserDto(string Name, string Email, string Role);

public sealed record LoginResponseDto(bool Authenticated, AuthenticatedUserDto User);
