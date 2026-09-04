using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.Data;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Repositories;

public sealed class LivroRepository : ILivroRepository
{
    private readonly BibliotecaContext _context;

    public LivroRepository(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Livro>> GetAllAsync(string? titulo, string? autor)
    {
        var query = _context.Livros
            .AsNoTracking()
            .Include(l => l.Autor)
            .AsQueryable();

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

    public void Add(Livro livro)
    {
        _context.Livros.Add(livro);
    }

}
