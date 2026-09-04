using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs;

public sealed class CriarEmprestimoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "O AlunoId é obrigatório.")]
    public int AlunoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "O LivroId é obrigatório.")]
    public int LivroId { get; set; }

    public DateTime DataPrevistaDevolucao { get; set; }
}

public sealed class EmprestimoResponseDto
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public string NomeAluno { get; set; } = string.Empty;
    public int LivroId { get; set; }
    public string TituloLivro { get; set; } = string.Empty;
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataPrevistaDevolucao { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public string Status { get; set; } = string.Empty;
}
