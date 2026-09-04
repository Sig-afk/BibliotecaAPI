using Moq;
using BibliotecaAPI.Services;
using BibliotecaAPI.Repositories;
using BibliotecaAPI.Models;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Data;

namespace BibliotecaAPI.Tests.Services;

public class LivroServiceTests
{
    private readonly Mock<ILivroRepository> _livroRepoMock;
    private readonly Mock<IAutorRepository> _autorRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly LivroService _service;

    public LivroServiceTests()
    {
        _livroRepoMock = new Mock<ILivroRepository>();
        _autorRepoMock = new Mock<IAutorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _service = new LivroService(
            _livroRepoMock.Object,
            _autorRepoMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarLivro_QuandoExiste()
    {
        // Arrange
        var autor = new Autor { Id = 1, Nome = "Robert Martin", Nacionalidade = "Americano", DataNascimento = new DateTime(1952, 12, 5) };
        var livro = new Livro { Id = 1, Titulo = "Clean Code", ISBN = "978-0", Quantidade = 5, AutorId = 1, Autor = autor };
        _livroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(livro);

        // Act
        var resultado = await _service.GetByIdAsync(1);

        // Assert
        Assert.Equal(1, resultado.Id);
        Assert.Equal("Clean Code", resultado.Titulo);
        Assert.Equal("Robert Martin", resultado.NomeAutor);
    }

    [Fact]
    public async Task GetByIdAsync_DeveLancarNotFoundException_QuandoLivroNaoExiste()
    {
        // Arrange
        _livroRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Livro?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
        Assert.Contains("99", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_DeveCriarLivro_QuandoAutorExiste()
    {
        // Arrange
        var autor = new Autor { Id = 1, Nome = "Robert Martin", Nacionalidade = "Americano", DataNascimento = new DateTime(1952, 12, 5) };
        var dto = new CriarLivroDto { Titulo = "Clean Code", ISBN = "978-0", AnoPublicacao = 2008, Quantidade = 10, AutorId = 1 };

        _autorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(autor);

        // Act
        var resultado = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal("Clean Code", resultado.Titulo);
        Assert.Equal(10, resultado.Quantidade);
        Assert.Equal("Robert Martin", resultado.NomeAutor);
    }

    [Fact]
    public async Task CreateAsync_DeveLancarNotFoundException_QuandoAutorNaoExiste()
    {
        // Arrange
        var dto = new CriarLivroDto { Titulo = "Livro X", ISBN = "000", AnoPublicacao = 2024, Quantidade = 1, AutorId = 999 };
        _autorRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Autor?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
        Assert.Contains("999", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    public async Task CreateAsync_DeveDefinirQuantidadeCorretamente(int quantidade)
    {
        // Arrange
        var autor = new Autor { Id = 1, Nome = "Autor X", Nacionalidade = "BR", DataNascimento = DateTime.Today };
        var dto = new CriarLivroDto { Titulo = "Livro Y", ISBN = "abc", AnoPublicacao = 2024, Quantidade = quantidade, AutorId = 1 };

        _autorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(autor);

        // Act
        var resultado = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(quantidade, resultado.Quantidade);
    }
}
