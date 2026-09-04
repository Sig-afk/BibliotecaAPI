using BibliotecaAPI.DTOs;
using BibliotecaAPI.Data;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Mappings;
using BibliotecaAPI.Repositories;

namespace BibliotecaAPI.Services;

public sealed class AlunoService(
    IAlunoRepository repository,
    IUnitOfWork unitOfWork) : IAlunoService
{
    public async Task<IEnumerable<AlunoResponseDto>> GetAllAsync()
    {
        var alunos = await repository.GetAllAsync();
        return alunos.Select(aluno => aluno.ToResponseDto());
    }

    public async Task<AlunoResponseDto> GetByIdAsync(int id)
    {
        var aluno = await repository.GetByIdAsync(id);
        if (aluno is null)
            throw new NotFoundException($"Aluno com ID {id} não encontrado.");

        return aluno.ToResponseDto();
    }

    public async Task<AlunoResponseDto> CreateAsync(CriarAlunoDto dto)
    {
        var existente = await repository.GetByMatriculaAsync(dto.Matricula);
        if (existente is not null)
            throw new ConflictException($"Já existe um aluno com a matrícula '{dto.Matricula}'.");

        var aluno = dto.ToEntity();
        repository.Add(aluno);
        await unitOfWork.SaveChangesAsync();

        return aluno.ToResponseDto();
    }
}
