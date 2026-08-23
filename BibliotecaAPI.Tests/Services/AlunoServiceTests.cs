using Moq;
using BibliotecaAPI.Services;
using BibliotecaAPI.Repositories;
using BibliotecaAPI.Models;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Exceptions;

namespace BibliotecaAPI.Tests.Services;

public class AlunoServiceTests
{
    private readonly Mock<IAlunoRepository> _repoMock;
    private readonly AlunoService _service;

    public AlunoServiceTests()
    {
        _repoMock = new Mock<IAlunoRepository>();
        _service = new AlunoService(_repoMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarAluno_QuandoExiste()
    {
        // Arrange
        var aluno = new Aluno { Id = 1, Nome = "Ana", Matricula = "MAT999", Email = "ana@email.com" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(aluno);

        // Act
        var resultado = await _service.GetByIdAsync(1);

        // Assert
        Assert.Equal(1, resultado.Id);
        Assert.Equal("Ana", resultado.Nome);
        Assert.Equal("MAT999", resultado.Matricula);
    }

    [Fact]
    public async Task GetByIdAsync_DeveLancarNotFoundException_QuandoAlunoNaoExiste()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((Aluno?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(404));
        Assert.Contains("404", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_DeveCriarAluno_QuandoMatriculaDisponivel()
    {
        // Arrange
        var dto = new CriarAlunoDto { Nome = "Carlos", Matricula = "MAT123", Email = "carlos@email.com" };
        _repoMock.Setup(r => r.GetByMatriculaAsync("MAT123")).ReturnsAsync((Aluno?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Aluno>())).ReturnsAsync((Aluno a) => a);

        // Act
        var resultado = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal("Carlos", resultado.Nome);
        Assert.Equal("MAT123", resultado.Matricula);
    }

    [Fact]
    public async Task CreateAsync_DeveLancarConflictException_QuandoMatriculaJaExiste()
    {
        // Arrange
        var alunoExistente = new Aluno { Id = 5, Nome = "Pedro", Matricula = "MAT123", Email = "pedro@email.com" };
        var dto = new CriarAlunoDto { Nome = "Outro", Matricula = "MAT123", Email = "outro@email.com" };
        _repoMock.Setup(r => r.GetByMatriculaAsync("MAT123")).ReturnsAsync(alunoExistente);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(dto));
        Assert.Contains("MAT123", ex.Message);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarListaDeAlunos()
    {
        // Arrange
        var alunos = new List<Aluno>
        {
            new() { Id = 1, Nome = "Ana", Matricula = "M1", Email = "ana@email.com" },
            new() { Id = 2, Nome = "Bob", Matricula = "M2", Email = "bob@email.com" },
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(alunos);

        // Act
        var resultado = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Ana", resultado[0].Nome);
        Assert.Equal("Bob", resultado[1].Nome);
    }
}
