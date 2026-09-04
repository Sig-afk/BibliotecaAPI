using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.Data;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Repositories;

public sealed class EmprestimoRepository : IEmprestimoRepository
{
    private readonly BibliotecaContext _context;

    public EmprestimoRepository(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Emprestimo>> GetAllAsync()
    {
        return await _context.Emprestimos
            .AsNoTracking()
            .Include(e => e.Aluno)
            .Include(e => e.Livro)
            .ToListAsync();
    }

    public async Task<Emprestimo?> GetByIdAsync(int id)
    {
        return await _context.Emprestimos
            .Include(e => e.Aluno)
            .Include(e => e.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Emprestimo?> GetEmprestimoAtivoAsync(int alunoId, int livroId)
    {
        return await _context.Emprestimos
            .FirstOrDefaultAsync(e =>
                e.AlunoId == alunoId &&
                e.LivroId == livroId &&
                e.Status == StatusEmprestimo.Ativo);
    }

    public void Add(Emprestimo emprestimo)
    {
        _context.Emprestimos.Add(emprestimo);
    }

}
