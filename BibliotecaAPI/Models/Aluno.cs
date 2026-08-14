using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Models;

public class Aluno
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Matricula { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();
}
