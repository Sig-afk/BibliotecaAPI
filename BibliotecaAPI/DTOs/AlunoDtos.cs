using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs;

public sealed class CriarAlunoDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A matrícula é obrigatória.")]
    public string Matricula { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;
}

public sealed class AlunoResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
