using BibliotecaAPI.DTOs;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;

namespace BibliotecaAPI.Services;

public class AutorService : IAutorService
{
    private readonly IAutorRepository _repo;

    public AutorService(IAutorRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<AutorResponseDto>> GetAllAsync()
    {
        var autores = await _repo.GetAllAsync();
        return autores.Select(a => new AutorResponseDto
        {
            Id = a.Id,
            Nome = a.Nome,
            DataNascimento = a.DataNascimento,
            Nacionalidade = a.Nacionalidade
        });
    }

    public async Task<AutorResponseDto> GetByIdAsync(int id)
    {
        var autor = await _repo.GetByIdAsync(id);
        if (autor is null)
            throw new NotFoundException($"Autor com ID {id} não encontrado.");

        return new AutorResponseDto
        {
            Id = autor.Id,
            Nome = autor.Nome,
            DataNascimento = autor.DataNascimento,
            Nacionalidade = autor.Nacionalidade
        };
    }

    public async Task<AutorResponseDto> CreateAsync(CriarAutorDto dto)
    {
        var autor = new Autor
        {
            Nome = dto.Nome,
            DataNascimento = dto.DataNascimento,
            Nacionalidade = dto.Nacionalidade
        };

        await _repo.AddAsync(autor);
        await _repo.SaveChangesAsync();

        return new AutorResponseDto
        {
            Id = autor.Id,
            Nome = autor.Nome,
            DataNascimento = autor.DataNascimento,
            Nacionalidade = autor.Nacionalidade
        };
    }
}
