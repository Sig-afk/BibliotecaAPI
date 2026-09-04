using BibliotecaAPI.Configuration;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Services;
using Microsoft.Extensions.Options;

namespace BibliotecaAPI.Tests.Services;

public class AuthServiceTests
{
    private readonly AuthService _service = new(Options.Create(new DemoAuthOptions
    {
        Email = "admin@biblioteca.local",
        Password = "segredo",
        Name = "Bibliotecária"
    }));

    [Fact]
    public void Authenticate_DeveRetornarUsuario_QuandoCredenciaisValidas()
    {
        var request = new LoginRequestDto
        {
            Email = "ADMIN@biblioteca.local",
            Senha = "segredo"
        };

        var result = _service.Authenticate(request);

        Assert.NotNull(result);
        Assert.True(result.Authenticated);
        Assert.Equal("Bibliotecária", result.User.Name);
        Assert.Equal("Administrador", result.User.Role);
    }

    [Theory]
    [InlineData("invalido@biblioteca.local", "segredo")]
    [InlineData("admin@biblioteca.local", "senha-invalida")]
    public void Authenticate_DeveRetornarNulo_QuandoCredenciaisInvalidas(
        string email,
        string senha)
    {
        var request = new LoginRequestDto { Email = email, Senha = senha };

        var result = _service.Authenticate(request);

        Assert.Null(result);
    }
}
