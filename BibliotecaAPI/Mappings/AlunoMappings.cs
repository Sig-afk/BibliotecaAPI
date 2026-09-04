using BibliotecaAPI.DTOs;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Mappings;

public static class AlunoMappings
{
    public static AlunoResponseDto ToResponseDto(this Aluno aluno) => new()
    {
        Id = aluno.Id,
        Nome = aluno.Nome,
        Matricula = aluno.Matricula,
        Email = aluno.Email
    };

    public static Aluno ToEntity(this CriarAlunoDto dto) => new()
    {
        Nome = dto.Nome,
        Matricula = dto.Matricula,
        Email = dto.Email
    };
}
