using BibliotecaAPI.Models;

namespace BibliotecaAPI.Repositories;

public interface IEmprestimoRepository
{
    Task<IEnumerable<Emprestimo>> GetAllAsync();
    Task<Emprestimo?> GetByIdAsync(int id);
    Task<Emprestimo?> GetEmprestimoAtivoAsync(int alunoId, int livroId);
    void Add(Emprestimo emprestimo);
}
