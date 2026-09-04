using Moq;
using BibliotecaAPI.Services;
using BibliotecaAPI.Repositories;
using BibliotecaAPI.Models;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Data;

namespace BibliotecaAPI.Tests.Services;

public class AutorServiceTests
{
    private readonly Mock<IAutorRepository> _repoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AutorService _service;

    public AutorServiceTests()
    {
        _repoMock = new Mock<IAutorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _service = new AutorService(_repoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarAutor_QuandoExiste()
    {
        // Arrange
        var autor = new Autor { Id = 1, Nome = "Robert Martin", Nacionalidade = "Americano", DataNascimento = new DateTime(1952, 12, 5) };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(autor);

        // Act
        var resultado = await _service.GetByIdAsync(1);

        // Assert
        Assert.Equal(1, resultado.Id);
        Assert.Equal("Robert Martin", resultado.Nome);
        Assert.Equal("Americano", resultado.Nacionalidade);
    }

    [Fact]
    public async Task GetByIdAsync_DeveLancarNotFoundException_QuandoAutorNaoExiste()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(77)).ReturnsAsync((Autor?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(77));
        Assert.Contains("77", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_DeveCriarAutor_ComDadosCorretos()
    {
        // Arrange
        var dto = new CriarAutorDto
        {
            Nome = "Martin Fowler",
            Nacionalidade = "Britânico",
            DataNascimento = new DateTime(1963, 12, 18)
        };

        // Act
        var resultado = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal("Martin Fowler", resultado.Nome);
        Assert.Equal("Britânico", resultado.Nacionalidade);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosOsAutores()
    {
        // Arrange
        var autores = new List<Autor>
        {
            new() { Id = 1, Nome = "Autor A", Nacionalidade = "BR", DataNascimento = DateTime.Today },
            new() { Id = 2, Nome = "Autor B", Nacionalidade = "EUA", DataNascimento = DateTime.Today },
            new() { Id = 3, Nome = "Autor C", Nacionalidade = "BR", DataNascimento = DateTime.Today },
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(autores);

        // Act
        var resultado = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(3, resultado.Count);
    }

    [Theory]
    [InlineData("Kent Beck", "Americano")]
    [InlineData("Erich Gamma", "Suíço")]
    [InlineData("Donald Knuth", "Americano")]
    public async Task CreateAsync_DeveMaperarNomeENacionalidadeCorretamente(string nome, string nacionalidade)
    {
        // Arrange
        var dto = new CriarAutorDto { Nome = nome, Nacionalidade = nacionalidade, DataNascimento = DateTime.Today };

        // Act
        var resultado = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(nome, resultado.Nome);
        Assert.Equal(nacionalidade, resultado.Nacionalidade);
    }
}
