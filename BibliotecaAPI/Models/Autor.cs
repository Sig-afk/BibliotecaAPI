using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Models;

public class Autor
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    public DateTime DataNascimento { get; set; }

    public string Nacionalidade { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Livro> Livros { get; set; } = new List<Livro>();
}
