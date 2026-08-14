using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaAPI.Models;

public enum StatusEmprestimo
{
    Ativo = 0,
    Devolvido = 1,
    Atrasado = 2
}

public class Emprestimo
{
    public int Id { get; set; }

    public int AlunoId { get; set; }

    [ForeignKey(nameof(AlunoId))]
    public Aluno? Aluno { get; set; }

    public int LivroId { get; set; }

    [ForeignKey(nameof(LivroId))]
    public Livro? Livro { get; set; }

    public DateTime DataEmprestimo { get; set; }

    public DateTime DataPrevistaDevolucao { get; set; }

    public DateTime? DataDevolucao { get; set; }

    public StatusEmprestimo Status { get; set; }
}
