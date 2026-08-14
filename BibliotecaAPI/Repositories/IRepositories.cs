using BibliotecaAPI.Models;

namespace BibliotecaAPI.Repositories;

public interface IAutorRepository
{
    Task<IEnumerable<Autor>> GetAllAsync();
    Task<Autor?> GetByIdAsync(int id);
    Task<Autor> AddAsync(Autor autor);
    Task SaveChangesAsync();
}

public interface ILivroRepository
{
    Task<IEnumerable<Livro>> GetAllAsync(string? titulo, string? autor);
    Task<Livro?> GetByIdAsync(int id);
    Task<Livro> AddAsync(Livro livro);
    Task SaveChangesAsync();
}

public interface IAlunoRepository
{
    Task<IEnumerable<Aluno>> GetAllAsync();
    Task<Aluno?> GetByIdAsync(int id);
    Task<Aluno?> GetByMatriculaAsync(string matricula);
    Task<Aluno> AddAsync(Aluno aluno);
    Task SaveChangesAsync();
}

public interface IEmprestimoRepository
{
    Task<IEnumerable<Emprestimo>> GetAllAsync();
    Task<Emprestimo?> GetByIdAsync(int id);
    Task<Emprestimo?> GetEmprestimoAtivoAsync(int alunoId, int livroId);
    Task<Emprestimo> AddAsync(Emprestimo emprestimo);
    Task SaveChangesAsync();
}
