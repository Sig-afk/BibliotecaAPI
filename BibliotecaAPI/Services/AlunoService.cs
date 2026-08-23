using BibliotecaAPI.DTOs;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;

namespace BibliotecaAPI.Services;

public class AlunoService : IAlunoService
{
    private readonly IAlunoRepository _repo;

    public AlunoService(IAlunoRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<AlunoResponseDto>> GetAllAsync()
    {
        var alunos = await _repo.GetAllAsync();
        return alunos.Select(a => new AlunoResponseDto
        {
            Id = a.Id,
            Nome = a.Nome,
            Matricula = a.Matricula,
            Email = a.Email
        });
    }

    public async Task<AlunoResponseDto> GetByIdAsync(int id)
    {
        var aluno = await _repo.GetByIdAsync(id);
        if (aluno is null)
            throw new NotFoundException($"Aluno com ID {id} não encontrado.");

        return new AlunoResponseDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Matricula = aluno.Matricula,
            Email = aluno.Email
        };
    }

    public async Task<AlunoResponseDto> CreateAsync(CriarAlunoDto dto)
    {
        var existente = await _repo.GetByMatriculaAsync(dto.Matricula);
        if (existente is not null)
            throw new ConflictException($"Já existe um aluno com a matrícula '{dto.Matricula}'.");

        var aluno = new Aluno
        {
            Nome = dto.Nome,
            Matricula = dto.Matricula,
            Email = dto.Email
        };

        await _repo.AddAsync(aluno);
        await _repo.SaveChangesAsync();

        return new AlunoResponseDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Matricula = aluno.Matricula,
            Email = aluno.Email
        };
    }
}
