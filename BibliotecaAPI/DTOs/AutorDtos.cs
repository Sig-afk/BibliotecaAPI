using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs;

public sealed class CriarAutorDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    public DateTime DataNascimento { get; set; }
    public string Nacionalidade { get; set; } = string.Empty;
}

public sealed class AutorResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Nacionalidade { get; set; } = string.Empty;
}
