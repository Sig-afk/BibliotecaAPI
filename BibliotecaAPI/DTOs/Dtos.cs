using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs;

// ---- Autor ----
public class CriarAutorDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    public DateTime DataNascimento { get; set; }

    public string Nacionalidade { get; set; } = string.Empty;
}

public class AutorResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Nacionalidade { get; set; } = string.Empty;
}

// ---- Livro ----
public class CriarLivroDto
{
    [Required(ErrorMessage = "O ISBN é obrigatório.")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "O título é obrigatório.")]
    public string Titulo { get; set; } = string.Empty;

    public int AnoPublicacao { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa.")]
    public int Quantidade { get; set; }

    [Required(ErrorMessage = "O AutorId é obrigatório.")]
    public int AutorId { get; set; }
}

public class LivroResponseDto
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public int AnoPublicacao { get; set; }
    public int Quantidade { get; set; }
    public int AutorId { get; set; }
    public string NomeAutor { get; set; } = string.Empty;
}

// ---- Aluno ----
public class CriarAlunoDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A matrícula é obrigatória.")]
    public string Matricula { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;
}

public class AlunoResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// ---- Empréstimo ----
public class CriarEmprestimoDto
{
    [Required]
    public int AlunoId { get; set; }

    [Required]
    public int LivroId { get; set; }

    public DateTime DataPrevistaDevolucao { get; set; }
}

public class EmprestimoResponseDto
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
