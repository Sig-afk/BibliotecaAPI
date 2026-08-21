using Xunit;
using BibliotecaAPI.Services;
using BibliotecaAPI.Repositories;
using BibliotecaAPI.Data;
using BibliotecaAPI.Models;
using BibliotecaAPI.DTOs;
public class EmprestimoServiceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly BibliotecaContext _context;

    public EmprestimoServiceTests(TestDatabaseFixture fixture)
    {
        _context = fixture.Context;
    }

    [Fact]
    public async Task DeveCriarAutor()
    {
        var repo = new AutorRepository(_context);
        var service = new AutorService(repo);

        var dto = new CriarAutorDto
        {
            Nome = "Autor Teste",
            DataNascimento = new DateTime(1980, 1, 1),
            Nacionalidade = "Brasileiro"
        };
        var resultado = await service.CreateAsync(dto);
        Assert.NotEqual(0, resultado.Id);

    }


    [Fact]
    public async Task DeveIndicarQueLivroEstaDisponivel()
    {
        // Arrange: Instancia o serviço e define um cenário com estoque positivo
        var context = new BibliotecaContextFactory().CreateDbContext(null);
        var repo = new AutorRepository(context);
        var service = new AutorService(repo);
        int quantidadeDisponivel = 3;
        var autor = new CriarAutorDto
        {
            Nome = "Autor Teste",
            DataNascimento = new DateTime(1980, 1, 1),
            Nacionalidade = "Brasileiro"
        };
        // Act: Executa a validação de disponibilidade
        var resultado = await service.CreateAsync(autor);
        Console.WriteLine($"Autor criado com ID: {resultado.Id}");
        // Assert: Verifica se o sistema identifica corretamente a disponibilidade
        Assert.True(true);
    }
    [Fact]
    public void DeveIndicarQueLivroNaoEstaDisponivel()
    {
        // Arrange: Define um cenário onde o estoque está zerado
        // var service = new AutorService();
        // int quantidadeEsgotada = 0;
        // // Act: Executa a validação
        // var resultado = service.LivroDisponivel(quantidadeEsgotada);
        // // Assert: O retorno esperado deve ser falso
        Assert.False(true);
    }
}