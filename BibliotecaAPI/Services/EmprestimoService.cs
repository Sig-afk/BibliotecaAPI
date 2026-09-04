using BibliotecaAPI.DTOs;
using BibliotecaAPI.Data;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Mappings;
using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;

namespace BibliotecaAPI.Services;

public sealed class EmprestimoService(
    IEmprestimoRepository emprestimoRepository,
    ILivroRepository livroRepository,
    IAlunoRepository alunoRepository,
    IUnitOfWork unitOfWork) : IEmprestimoService
{
    public async Task<IEnumerable<EmprestimoResponseDto>> GetAllAsync()
    {
        var emprestimos = await emprestimoRepository.GetAllAsync();
        return emprestimos.Select(emprestimo => emprestimo.ToResponseDto());
    }

    public async Task<EmprestimoResponseDto> GetByIdAsync(int id)
    {
        var emprestimo = await emprestimoRepository.GetByIdAsync(id);
        if (emprestimo is null)
            throw new NotFoundException($"Empréstimo com ID {id} não encontrado.");

        return emprestimo.ToResponseDto();
    }

    public async Task<EmprestimoResponseDto> CreateAsync(CriarEmprestimoDto dto)
    {
        // Verifica se aluno existe
        var aluno = await alunoRepository.GetByIdAsync(dto.AlunoId);
        if (aluno is null)
            throw new NotFoundException($"Aluno com ID {dto.AlunoId} não encontrado.");

        // Verifica se livro existe
        var livro = await livroRepository.GetByIdAsync(dto.LivroId);
        if (livro is null)
            throw new NotFoundException($"Livro com ID {dto.LivroId} não encontrado.");

        // Regra 1: Estoque disponível
        if (livro.Quantidade <= 0)
            throw new ConflictException("O livro não possui exemplares disponíveis.");

        // Regra 2: Empréstimo duplicado
        var emprestimoAtivo = await emprestimoRepository.GetEmprestimoAtivoAsync(dto.AlunoId, dto.LivroId);
        if (emprestimoAtivo is not null)
            throw new ConflictException("O aluno já possui um empréstimo ativo deste livro.");

        // Desconta estoque
        livro.Quantidade -= 1;

        var emprestimo = new Emprestimo
        {
            AlunoId = dto.AlunoId,
            LivroId = dto.LivroId,
            DataEmprestimo = DateTime.UtcNow,
            DataPrevistaDevolucao = dto.DataPrevistaDevolucao,
            Status = StatusEmprestimo.Ativo
        };

        emprestimoRepository.Add(emprestimo);
        await unitOfWork.SaveChangesAsync();

        emprestimo.Aluno = aluno;
        emprestimo.Livro = livro;

        return emprestimo.ToResponseDto();
    }

    public async Task<EmprestimoResponseDto> DevolverAsync(int id)
    {
        var emprestimo = await emprestimoRepository.GetByIdAsync(id);
        if (emprestimo is null)
            throw new NotFoundException($"Empréstimo com ID {id} não encontrado.");

        // Regra 3: Devolução duplicada
        if (emprestimo.Status == StatusEmprestimo.Devolvido)
            throw new ConflictException("Este empréstimo já foi devolvido.");

        // Incrementa estoque
        var livro = await livroRepository.GetByIdAsync(emprestimo.LivroId);
        if (livro is not null)
            livro.Quantidade += 1;

        emprestimo.DataDevolucao = DateTime.UtcNow;
        emprestimo.Status = StatusEmprestimo.Devolvido;

        await unitOfWork.SaveChangesAsync();

        return emprestimo.ToResponseDto();
    }
}
