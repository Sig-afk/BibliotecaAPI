using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.Data;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Repositories;

public sealed class AlunoRepository : IAlunoRepository
{
    private readonly BibliotecaContext _context;

    public AlunoRepository(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Aluno>> GetAllAsync()
    {
        return await _context.Alunos.AsNoTracking().ToListAsync();
    }

    public async Task<Aluno?> GetByIdAsync(int id)
    {
        return await _context.Alunos.FindAsync(id);
    }

    public async Task<Aluno?> GetByMatriculaAsync(string matricula)
    {
        return await _context.Alunos.FirstOrDefaultAsync(a => a.Matricula == matricula);
    }

    public void Add(Aluno aluno)
    {
        _context.Alunos.Add(aluno);
    }

}
