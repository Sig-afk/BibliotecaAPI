using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.Data;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Repositories;

public class AutorRepository : IAutorRepository
{
    private readonly BibliotecaContext _context;

    public AutorRepository(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Autor>> GetAllAsync()
    {
        return await _context.Autores.ToListAsync();
    }

    public async Task<Autor?> GetByIdAsync(int id)
    {
        return await _context.Autores.FindAsync(id);
    }

    public Task<Autor> AddAsync(Autor autor)
    {
        _context.Autores.Add(autor);
        return Task.FromResult(autor);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

public class LivroRepository : ILivroRepository
{
    private readonly BibliotecaContext _context;

    public LivroRepository(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Livro>> GetAllAsync(string? titulo, string? autor)
    {
        var query = _context.Livros.Include(l => l.Autor).AsQueryable();

        if (!string.IsNullOrEmpty(titulo))
            query = query.Where(l => l.Titulo.ToLower().Contains(titulo.ToLower()));

        if (!string.IsNullOrEmpty(autor))
            query = query.Where(l => l.Autor!.Nome.ToLower().Contains(autor.ToLower()));

        return await query.ToListAsync();
    }

    public async Task<Livro?> GetByIdAsync(int id)
    {
        return await _context.Livros.Include(l => l.Autor).FirstOrDefaultAsync(l => l.Id == id);
    }

    public Task<Livro> AddAsync(Livro livro)
    {
        _context.Livros.Add(livro);
        return Task.FromResult(livro);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

public class AlunoRepository : IAlunoRepository
{
    private readonly BibliotecaContext _context;

    public AlunoRepository(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Aluno>> GetAllAsync()
    {
        return await _context.Alunos.ToListAsync();
    }

    public async Task<Aluno?> GetByIdAsync(int id)
    {
        return await _context.Alunos.FindAsync(id);
    }

    public async Task<Aluno?> GetByMatriculaAsync(string matricula)
    {
        return await _context.Alunos.FirstOrDefaultAsync(a => a.Matricula == matricula);
    }

    public Task<Aluno> AddAsync(Aluno aluno)
    {
        _context.Alunos.Add(aluno);
        return Task.FromResult(aluno);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

public class EmprestimoRepository : IEmprestimoRepository
{
    private readonly BibliotecaContext _context;

    public EmprestimoRepository(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Emprestimo>> GetAllAsync()
    {
        return await _context.Emprestimos
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

    public Task<Emprestimo> AddAsync(Emprestimo emprestimo)
    {
        _context.Emprestimos.Add(emprestimo);
        return Task.FromResult(emprestimo);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
