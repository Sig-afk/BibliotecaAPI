using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaAPI.Models;

public class Livro
{
    public int Id { get; set; }

    [Required]
    public string ISBN { get; set; } = string.Empty;

    [Required]
    public string Titulo { get; set; } = string.Empty;

    public int AnoPublicacao { get; set; }

    public int Quantidade { get; set; }

    public int AutorId { get; set; }

    [ForeignKey(nameof(AutorId))]
    public Autor? Autor { get; set; }

    // Navigation property
    public ICollection<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();
}
