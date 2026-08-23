using BibliotecaAPI.Models;

namespace BibliotecaAPI.Repositories;

public interface ILivroRepository
{
    Task<IEnumerable<Livro>> GetAllAsync(string? titulo, string? autor);
    Task<Livro?> GetByIdAsync(int id);
    Task<Livro> AddAsync(Livro livro);
    Task SaveChangesAsync();
}
