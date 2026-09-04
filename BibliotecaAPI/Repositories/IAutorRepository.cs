using BibliotecaAPI.Models;

namespace BibliotecaAPI.Repositories;

public interface IAutorRepository
{
    Task<IEnumerable<Autor>> GetAllAsync();
    Task<Autor?> GetByIdAsync(int id);
    void Add(Autor autor);
}
