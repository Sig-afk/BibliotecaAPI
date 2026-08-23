using BibliotecaAPI.Models;

namespace BibliotecaAPI.Repositories;

public interface IAlunoRepository
{
    Task<IEnumerable<Aluno>> GetAllAsync();
    Task<Aluno?> GetByIdAsync(int id);
    Task<Aluno?> GetByMatriculaAsync(string matricula);
    Task<Aluno> AddAsync(Aluno aluno);
    Task SaveChangesAsync();
}
