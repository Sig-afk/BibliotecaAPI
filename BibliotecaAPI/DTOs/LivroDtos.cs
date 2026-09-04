using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs;

public sealed class CriarLivroDto
{
    [Required(ErrorMessage = "O ISBN é obrigatório.")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "O título é obrigatório.")]
    public string Titulo { get; set; } = string.Empty;

    public int AnoPublicacao { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa.")]
    public int Quantidade { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "O AutorId é obrigatório.")]
    public int AutorId { get; set; }
}

public sealed class LivroResponseDto
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public int AnoPublicacao { get; set; }
    public int Quantidade { get; set; }
    public int AutorId { get; set; }
    public string NomeAutor { get; set; } = string.Empty;
}
