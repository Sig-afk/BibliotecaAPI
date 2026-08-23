using BibliotecaAPI.DTOs;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;

namespace BibliotecaAPI.Services;

public class EmprestimoService : IEmprestimoService
{
    private readonly IEmprestimoRepository _emprestimoRepo;
    private readonly ILivroRepository _livroRepo;
    private readonly IAlunoRepository _alunoRepo;

    public EmprestimoService(
        IEmprestimoRepository emprestimoRepo,
        ILivroRepository livroRepo,
        IAlunoRepository alunoRepo)
    {
        _emprestimoRepo = emprestimoRepo;
        _livroRepo = livroRepo;
        _alunoRepo = alunoRepo;
    }

    public async Task<IEnumerable<EmprestimoResponseDto>> GetAllAsync()
    {
        var emprestimos = await _emprestimoRepo.GetAllAsync();
        return emprestimos.Select(MapToDto);
    }

    public async Task<EmprestimoResponseDto> GetByIdAsync(int id)
    {
        var emprestimo = await _emprestimoRepo.GetByIdAsync(id);
        if (emprestimo is null)
            throw new NotFoundException($"Empréstimo com ID {id} não encontrado.");

        return MapToDto(emprestimo);
    }

    public async Task<EmprestimoResponseDto> CreateAsync(CriarEmprestimoDto dto)
    {
        // Verifica se aluno existe
        var aluno = await _alunoRepo.GetByIdAsync(dto.AlunoId);
        if (aluno is null)
            throw new NotFoundException($"Aluno com ID {dto.AlunoId} não encontrado.");

        // Verifica se livro existe
        var livro = await _livroRepo.GetByIdAsync(dto.LivroId);
        if (livro is null)
            throw new NotFoundException($"Livro com ID {dto.LivroId} não encontrado.");

        // Regra 1: Estoque disponível
        if (livro.Quantidade <= 0)
            throw new ConflictException("O livro não possui exemplares disponíveis.");

        // Regra 2: Empréstimo duplicado
        var emprestimoAtivo = await _emprestimoRepo.GetEmprestimoAtivoAsync(dto.AlunoId, dto.LivroId);
        if (emprestimoAtivo is not null)
            throw new ConflictException("O aluno já possui um empréstimo ativo deste livro.");

        // Desconta estoque
        livro.Quantidade -= 1;
        await _livroRepo.SaveChangesAsync();

        var emprestimo = new Emprestimo
        {
            AlunoId = dto.AlunoId,
            LivroId = dto.LivroId,
            DataEmprestimo = DateTime.UtcNow,
            DataPrevistaDevolucao = dto.DataPrevistaDevolucao,
            Status = StatusEmprestimo.Ativo
        };

        await _emprestimoRepo.AddAsync(emprestimo);
        await _emprestimoRepo.SaveChangesAsync();

        emprestimo.Aluno = aluno;
        emprestimo.Livro = livro;

        return MapToDto(emprestimo);
    }

    public async Task<EmprestimoResponseDto> DevolverAsync(int id)
    {
        var emprestimo = await _emprestimoRepo.GetByIdAsync(id);
        if (emprestimo is null)
            throw new NotFoundException($"Empréstimo com ID {id} não encontrado.");

        // Regra 3: Devolução duplicada
        if (emprestimo.Status == StatusEmprestimo.Devolvido)
            throw new ConflictException("Este empréstimo já foi devolvido.");

        // Incrementa estoque
        var livro = await _livroRepo.GetByIdAsync(emprestimo.LivroId);
        if (livro is not null)
        {
            livro.Quantidade += 1;
            await _livroRepo.SaveChangesAsync();
        }

        emprestimo.DataDevolucao = DateTime.UtcNow;
        emprestimo.Status = StatusEmprestimo.Devolvido;

        await _emprestimoRepo.SaveChangesAsync();

        return MapToDto(emprestimo);
    }

    private static EmprestimoResponseDto MapToDto(Emprestimo e) => new()
    {
        Id = e.Id,
        AlunoId = e.AlunoId,
        NomeAluno = e.Aluno?.Nome ?? string.Empty,
        LivroId = e.LivroId,
        TituloLivro = e.Livro?.Titulo ?? string.Empty,
        DataEmprestimo = e.DataEmprestimo,
        DataPrevistaDevolucao = e.DataPrevistaDevolucao,
        DataDevolucao = e.DataDevolucao,
        Status = e.Status.ToString()
    };
}
