using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers;

public sealed class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;
}

[ApiController]
[Route("api/auth")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var email = configuration["DemoAuth:Email"] ?? "admin@biblioteca.local";
        var password = configuration["DemoAuth:Password"] ?? "admin123";
        var name = configuration["DemoAuth:Name"] ?? "Bibliotecário(a)";

        var validEmail = string.Equals(request.Email.Trim(), email, StringComparison.OrdinalIgnoreCase);
        var validPassword = string.Equals(request.Senha, password, StringComparison.Ordinal);

        if (!validEmail || !validPassword)
            return Unauthorized(new { message = "E-mail ou senha inválidos." });

        return Ok(new
        {
            authenticated = true,
            user = new { name, email, role = "Administrador" }
        });
    }
}
