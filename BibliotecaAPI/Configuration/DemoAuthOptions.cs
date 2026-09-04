namespace BibliotecaAPI.Configuration;

public sealed class DemoAuthOptions
{
    public const string SectionName = "DemoAuth";

    public string Email { get; init; } = "admin@biblioteca.local";
    public string Password { get; init; } = "admin123";
    public string Name { get; init; } = "Bibliotecário(a)";
}
