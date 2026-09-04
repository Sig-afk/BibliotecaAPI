using BibliotecaAPI.DTOs;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Mappings;

public static class EmprestimoMappings
{
    public static EmprestimoResponseDto ToResponseDto(this Emprestimo emprestimo) => new()
    {
        Id = emprestimo.Id,
        AlunoId = emprestimo.AlunoId,
        NomeAluno = emprestimo.Aluno?.Nome ?? string.Empty,
        LivroId = emprestimo.LivroId,
        TituloLivro = emprestimo.Livro?.Titulo ?? string.Empty,
        DataEmprestimo = emprestimo.DataEmprestimo,
        DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao,
        DataDevolucao = emprestimo.DataDevolucao,
        Status = emprestimo.Status.ToString()
    };
}
