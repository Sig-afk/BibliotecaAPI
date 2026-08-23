using Moq;
using BibliotecaAPI.Services;
using BibliotecaAPI.Repositories;
using BibliotecaAPI.Models;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Exceptions;

namespace BibliotecaAPI.Tests.Services;

public class EmprestimoServiceTests
{
    private readonly Mock<IEmprestimoRepository> _emprestimoRepoMock;
    private readonly Mock<ILivroRepository> _livroRepoMock;
    private readonly Mock<IAlunoRepository> _alunoRepoMock;
    private readonly EmprestimoService _service;

    public EmprestimoServiceTests()
    {
        _emprestimoRepoMock = new Mock<IEmprestimoRepository>();
        _livroRepoMock = new Mock<ILivroRepository>();
        _alunoRepoMock = new Mock<IAlunoRepository>();

        _service = new EmprestimoService(
            _emprestimoRepoMock.Object,
            _livroRepoMock.Object,
            _alunoRepoMock.Object
        );
    }

    // ─── CreateAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_DeveCriarEmprestimo_QuandoDadosValidos()
    {
        // Arrange
        var aluno = new Aluno { Id = 1, Nome = "João", Matricula = "MAT001", Email = "joao@email.com" };
        var livro = new Livro { Id = 1, Titulo = "Clean Code", ISBN = "123", Quantidade = 3, AutorId = 1 };
        var dto = new CriarEmprestimoDto
        {
            AlunoId = 1,
            LivroId = 1,
            DataPrevistaDevolucao = DateTime.UtcNow.AddDays(14)
        };

        _alunoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(aluno);
        _livroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(livro);
        _emprestimoRepoMock.Setup(r => r.GetEmprestimoAtivoAsync(1, 1)).ReturnsAsync((Emprestimo?)null);
        _emprestimoRepoMock.Setup(r => r.AddAsync(It.IsAny<Emprestimo>()))
            .ReturnsAsync((Emprestimo e) => e);

        // Act
        var resultado = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("João", resultado.NomeAluno);
        Assert.Equal("Clean Code", resultado.TituloLivro);
        Assert.Equal("Ativo", resultado.Status);
    }

    [Fact]
    public async Task CreateAsync_DeveDescontarEstoque_QuandoEmprestimoRealizado()
    {
        // Arrange
        var livro = new Livro { Id = 1, Titulo = "DDD", ISBN = "456", Quantidade = 5, AutorId = 1 };
        var aluno = new Aluno { Id = 2, Nome = "Maria", Matricula = "MAT002", Email = "maria@email.com" };
        var dto = new CriarEmprestimoDto { AlunoId = 2, LivroId = 1, DataPrevistaDevolucao = DateTime.UtcNow.AddDays(7) };

        _alunoRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(aluno);
        _livroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(livro);
        _emprestimoRepoMock.Setup(r => r.GetEmprestimoAtivoAsync(2, 1)).ReturnsAsync((Emprestimo?)null);
        _emprestimoRepoMock.Setup(r => r.AddAsync(It.IsAny<Emprestimo>()))
            .ReturnsAsync((Emprestimo e) => e);

        // Act
        await _service.CreateAsync(dto);

        // Assert: estoque deve ter sido decrementado de 5 para 4
        Assert.Equal(4, livro.Quantidade);
    }

    [Fact]
    public async Task CreateAsync_DeveLancarNotFoundException_QuandoAlunoNaoExiste()
    {
        // Arrange
        var dto = new CriarEmprestimoDto { AlunoId = 99, LivroId = 1, DataPrevistaDevolucao = DateTime.UtcNow.AddDays(7) };
        _alunoRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Aluno?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_DeveLancarNotFoundException_QuandoLivroNaoExiste()
    {
        // Arrange
        var aluno = new Aluno { Id = 1, Nome = "João", Matricula = "MAT001", Email = "joao@email.com" };
        var dto = new CriarEmprestimoDto { AlunoId = 1, LivroId = 99, DataPrevistaDevolucao = DateTime.UtcNow.AddDays(7) };

        _alunoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(aluno);
        _livroRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Livro?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_DeveLancarConflictException_QuandoLivroSemEstoque()
    {
        // Arrange
        var aluno = new Aluno { Id = 1, Nome = "João", Matricula = "MAT001", Email = "joao@email.com" };
        var livro = new Livro { Id = 1, Titulo = "Livro Esgotado", ISBN = "789", Quantidade = 0, AutorId = 1 };
        var dto = new CriarEmprestimoDto { AlunoId = 1, LivroId = 1, DataPrevistaDevolucao = DateTime.UtcNow.AddDays(7) };

        _alunoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(aluno);
        _livroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(livro);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(dto));
        Assert.Contains("disponíveis", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_DeveLancarConflictException_QuandoEmprestimoDuplicado()
    {
        // Arrange
        var aluno = new Aluno { Id = 1, Nome = "João", Matricula = "MAT001", Email = "joao@email.com" };
        var livro = new Livro { Id = 1, Titulo = "Clean Code", ISBN = "123", Quantidade = 3, AutorId = 1 };
        var emprestimoAtivo = new Emprestimo { Id = 5, AlunoId = 1, LivroId = 1, Status = StatusEmprestimo.Ativo };
        var dto = new CriarEmprestimoDto { AlunoId = 1, LivroId = 1, DataPrevistaDevolucao = DateTime.UtcNow.AddDays(7) };

        _alunoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(aluno);
        _livroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(livro);
        _emprestimoRepoMock.Setup(r => r.GetEmprestimoAtivoAsync(1, 1)).ReturnsAsync(emprestimoAtivo);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(dto));
        Assert.Contains("empréstimo ativo", ex.Message);
    }

    // ─── DevolverAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task DevolverAsync_DeveMarcarComoDevolvido_QuandoEmprestimoAtivo()
    {
        // Arrange
        var livro = new Livro { Id = 1, Titulo = "Clean Code", ISBN = "123", Quantidade = 2, AutorId = 1 };
        var emprestimo = new Emprestimo
        {
            Id = 1,
            AlunoId = 1,
            LivroId = 1,
            Status = StatusEmprestimo.Ativo,
            DataEmprestimo = DateTime.UtcNow.AddDays(-7),
            DataPrevistaDevolucao = DateTime.UtcNow.AddDays(7),
            Aluno = new Aluno { Id = 1, Nome = "João", Matricula = "MAT001", Email = "joao@email.com" },
            Livro = livro
        };

        _emprestimoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emprestimo);
        _livroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(livro);

        // Act
        var resultado = await _service.DevolverAsync(1);

        // Assert
        Assert.Equal("Devolvido", resultado.Status);
        Assert.NotNull(resultado.DataDevolucao);
    }

    [Fact]
    public async Task DevolverAsync_DeveIncrementarEstoque_QuandoLivroDevolvido()
    {
        // Arrange
        var livro = new Livro { Id = 1, Titulo = "Clean Code", ISBN = "123", Quantidade = 2, AutorId = 1 };
        var emprestimo = new Emprestimo
        {
            Id = 1,
            AlunoId = 1,
            LivroId = 1,
            Status = StatusEmprestimo.Ativo,
            DataEmprestimo = DateTime.UtcNow.AddDays(-7),
            DataPrevistaDevolucao = DateTime.UtcNow.AddDays(7),
            Aluno = new Aluno { Id = 1, Nome = "João", Matricula = "MAT001", Email = "joao@email.com" },
            Livro = livro
        };

        _emprestimoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emprestimo);
        _livroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(livro);

        // Act
        await _service.DevolverAsync(1);

        // Assert: estoque deve ter sido incrementado de 2 para 3
        Assert.Equal(3, livro.Quantidade);
    }

    [Fact]
    public async Task DevolverAsync_DeveLancarNotFoundException_QuandoEmprestimoNaoExiste()
    {
        // Arrange
        _emprestimoRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Emprestimo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DevolverAsync(999));
    }

    [Fact]
    public async Task DevolverAsync_DeveLancarConflictException_QuandoJaDevolvido()
    {
        // Arrange
        var emprestimo = new Emprestimo
        {
            Id = 1,
            AlunoId = 1,
            LivroId = 1,
            Status = StatusEmprestimo.Devolvido,
            DataEmprestimo = DateTime.UtcNow.AddDays(-14),
            DataPrevistaDevolucao = DateTime.UtcNow.AddDays(-7),
            DataDevolucao = DateTime.UtcNow.AddDays(-5)
        };

        _emprestimoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emprestimo);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(() => _service.DevolverAsync(1));
        Assert.Contains("já foi devolvido", ex.Message);
    }

    // ─── GetByIdAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_DeveLancarNotFoundException_QuandoEmprestimoNaoExiste()
    {
        // Arrange
        _emprestimoRepoMock.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Emprestimo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(42));
    }
}
