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
