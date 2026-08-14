using BibliotecaAPI.DTOs;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;

namespace BibliotecaAPI.Services;

public class AutorService : IAutorService
{
    private readonly IAutorRepository _repo;

    public AutorService(IAutorRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<AutorResponseDto>> GetAllAsync()
    {
        var autores = await _repo.GetAllAsync();
        return autores.Select(a => new AutorResponseDto
        {
            Id = a.Id,
            Nome = a.Nome,
            DataNascimento = a.DataNascimento,
            Nacionalidade = a.Nacionalidade
        });
    }

    public async Task<AutorResponseDto> GetByIdAsync(int id)
    {
        var autor = await _repo.GetByIdAsync(id);
        if (autor is null)
            throw new NotFoundException($"Autor com ID {id} não encontrado.");

        return new AutorResponseDto
        {
            Id = autor.Id,
            Nome = autor.Nome,
            DataNascimento = autor.DataNascimento,
            Nacionalidade = autor.Nacionalidade
        };
    }

    public async Task<AutorResponseDto> CreateAsync(CriarAutorDto dto)
    {
        var autor = new Autor
        {
            Nome = dto.Nome,
            DataNascimento = dto.DataNascimento,
            Nacionalidade = dto.Nacionalidade
        };

        await _repo.AddAsync(autor);
        await _repo.SaveChangesAsync();

        return new AutorResponseDto
        {
            Id = autor.Id,
            Nome = autor.Nome,
            DataNascimento = autor.DataNascimento,
            Nacionalidade = autor.Nacionalidade
        };
    }
}

public class LivroService : ILivroService
{
    private readonly ILivroRepository _livroRepo;
    private readonly IAutorRepository _autorRepo;

    public LivroService(ILivroRepository livroRepo, IAutorRepository autorRepo)
    {
        _livroRepo = livroRepo;
        _autorRepo = autorRepo;
    }

    public async Task<IEnumerable<LivroResponseDto>> GetAllAsync(string? titulo, string? autor)
    {
        var livros = await _livroRepo.GetAllAsync(titulo, autor);
        return livros.Select(l => new LivroResponseDto
        {
            Id = l.Id,
            ISBN = l.ISBN,
            Titulo = l.Titulo,
            AnoPublicacao = l.AnoPublicacao,
            Quantidade = l.Quantidade,
            AutorId = l.AutorId,
            NomeAutor = l.Autor?.Nome ?? string.Empty
        });
    }

    public async Task<LivroResponseDto> GetByIdAsync(int id)
    {
        var livro = await _livroRepo.GetByIdAsync(id);
        if (livro is null)
            throw new NotFoundException($"Livro com ID {id} não encontrado.");

        return new LivroResponseDto
        {
            Id = livro.Id,
            ISBN = livro.ISBN,
            Titulo = livro.Titulo,
            AnoPublicacao = livro.AnoPublicacao,
            Quantidade = livro.Quantidade,
            AutorId = livro.AutorId,
            NomeAutor = livro.Autor?.Nome ?? string.Empty
        };
    }

    public async Task<LivroResponseDto> CreateAsync(CriarLivroDto dto)
    {
        var autor = await _autorRepo.GetByIdAsync(dto.AutorId);
        if (autor is null)
            throw new NotFoundException($"Autor com ID {dto.AutorId} não encontrado.");

        var livro = new Livro
        {
            ISBN = dto.ISBN,
            Titulo = dto.Titulo,
            AnoPublicacao = dto.AnoPublicacao,
            Quantidade = dto.Quantidade,
            AutorId = dto.AutorId
        };

        await _livroRepo.AddAsync(livro);
        await _livroRepo.SaveChangesAsync();

        return new LivroResponseDto
        {
            Id = livro.Id,
            ISBN = livro.ISBN,
            Titulo = livro.Titulo,
            AnoPublicacao = livro.AnoPublicacao,
            Quantidade = livro.Quantidade,
            AutorId = livro.AutorId,
            NomeAutor = autor.Nome
        };
    }
}

public class AlunoService : IAlunoService
{
    private readonly IAlunoRepository _repo;

    public AlunoService(IAlunoRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<AlunoResponseDto>> GetAllAsync()
    {
        var alunos = await _repo.GetAllAsync();
        return alunos.Select(a => new AlunoResponseDto
        {
            Id = a.Id,
            Nome = a.Nome,
            Matricula = a.Matricula,
            Email = a.Email
        });
    }

    public async Task<AlunoResponseDto> GetByIdAsync(int id)
    {
        var aluno = await _repo.GetByIdAsync(id);
        if (aluno is null)
            throw new NotFoundException($"Aluno com ID {id} não encontrado.");

        return new AlunoResponseDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Matricula = aluno.Matricula,
            Email = aluno.Email
        };
    }

    public async Task<AlunoResponseDto> CreateAsync(CriarAlunoDto dto)
    {
        var existente = await _repo.GetByMatriculaAsync(dto.Matricula);
        if (existente is not null)
            throw new ConflictException($"Já existe um aluno com a matrícula '{dto.Matricula}'.");

        var aluno = new Aluno
        {
            Nome = dto.Nome,
            Matricula = dto.Matricula,
            Email = dto.Email
        };

        await _repo.AddAsync(aluno);
        await _repo.SaveChangesAsync();

        return new AlunoResponseDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Matricula = aluno.Matricula,
            Email = aluno.Email
        };
    }
}

public class EmprestimoService : IEmprestimoService
{
    private readonly IEmprestimoRepository _emprestimoRepo;
    private readonly ILivroRepository _livroRepo;
    private readonly IAlunoRepository _alunoRepo;

    public EmprestimoService(
        IEmprestimoRepository emprestimoRepo,
        ILivroRepository livroRepo,
        IAlunoRepository alunoRepo)
    {
        _emprestimoRepo = emprestimoRepo;
        _livroRepo = livroRepo;
        _alunoRepo = alunoRepo;
    }

    public async Task<IEnumerable<EmprestimoResponseDto>> GetAllAsync()
    {
        var emprestimos = await _emprestimoRepo.GetAllAsync();
        return emprestimos.Select(MapToDto);
    }

    public async Task<EmprestimoResponseDto> GetByIdAsync(int id)
    {
        var emprestimo = await _emprestimoRepo.GetByIdAsync(id);
        if (emprestimo is null)
            throw new NotFoundException($"Empréstimo com ID {id} não encontrado.");

        return MapToDto(emprestimo);
    }

    public async Task<EmprestimoResponseDto> CreateAsync(CriarEmprestimoDto dto)
    {
        // Verifica se aluno existe
        var aluno = await _alunoRepo.GetByIdAsync(dto.AlunoId);
        if (aluno is null)
            throw new NotFoundException($"Aluno com ID {dto.AlunoId} não encontrado.");

        // Verifica se livro existe
        var livro = await _livroRepo.GetByIdAsync(dto.LivroId);
        if (livro is null)
            throw new NotFoundException($"Livro com ID {dto.LivroId} não encontrado.");

        // Regra 1: Estoque disponível
        if (livro.Quantidade <= 0)
            throw new ConflictException("O livro não possui exemplares disponíveis.");

        // Regra 2: Empréstimo duplicado
        var emprestimoAtivo = await _emprestimoRepo.GetEmprestimoAtivoAsync(dto.AlunoId, dto.LivroId);
        if (emprestimoAtivo is not null)
            throw new ConflictException("O aluno já possui um empréstimo ativo deste livro.");

        // Desconta estoque
        livro.Quantidade -= 1;
        await _livroRepo.SaveChangesAsync();

        var emprestimo = new Emprestimo
        {
            AlunoId = dto.AlunoId,
            LivroId = dto.LivroId,
            DataEmprestimo = DateTime.UtcNow,
            DataPrevistaDevolucao = dto.DataPrevistaDevolucao,
            Status = StatusEmprestimo.Ativo
        };

        await _emprestimoRepo.AddAsync(emprestimo);
        await _emprestimoRepo.SaveChangesAsync();

        emprestimo.Aluno = aluno;
        emprestimo.Livro = livro;

        return MapToDto(emprestimo);
    }

    public async Task<EmprestimoResponseDto> DevolverAsync(int id)
    {
        var emprestimo = await _emprestimoRepo.GetByIdAsync(id);
        if (emprestimo is null)
            throw new NotFoundException($"Empréstimo com ID {id} não encontrado.");

        // Regra 3: Devolução duplicada
        if (emprestimo.Status == StatusEmprestimo.Devolvido)
            throw new ConflictException("Este empréstimo já foi devolvido.");

        // Incrementa estoque
        var livro = await _livroRepo.GetByIdAsync(emprestimo.LivroId);
        if (livro is not null)
        {
            livro.Quantidade += 1;
            await _livroRepo.SaveChangesAsync();
        }

        emprestimo.DataDevolucao = DateTime.UtcNow;
        emprestimo.Status = StatusEmprestimo.Devolvido;

        await _emprestimoRepo.SaveChangesAsync();

        return MapToDto(emprestimo);
    }

    private static EmprestimoResponseDto MapToDto(Emprestimo e) => new()
    {
        Id = e.Id,
        AlunoId = e.AlunoId,
        NomeAluno = e.Aluno?.Nome ?? string.Empty,
        LivroId = e.LivroId,
        TituloLivro = e.Livro?.Titulo ?? string.Empty,
        DataEmprestimo = e.DataEmprestimo,
        DataPrevistaDevolucao = e.DataPrevistaDevolucao,
        DataDevolucao = e.DataDevolucao,
        Status = e.Status.ToString()
    };
}
