using BibliotecaAPI.DTOs;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;

namespace BibliotecaAPI.Services;

public class LivroService : ILivroService
{
    private readonly ILivroRepository _livroRepo;
    private readonly IAutorRepository _autorRepo;

    public LivroService(ILivroRepository livroRepo, IAutorRepository autorRepo)
    {
        _livroRepo = livroRepo;
        _autorRepo = autorRepo;
    }

    public async Task<IEnumerable<LivroResponseDto>> GetAllAsync(string? titulo, string? autor)
    {
        var livros = await _livroRepo.GetAllAsync(titulo, autor);
        return livros.Select(l => new LivroResponseDto
        {
            Id = l.Id,
            ISBN = l.ISBN,
            Titulo = l.Titulo,
            AnoPublicacao = l.AnoPublicacao,
            Quantidade = l.Quantidade,
            AutorId = l.AutorId,
            NomeAutor = l.Autor?.Nome ?? string.Empty
        });
    }

    public async Task<LivroResponseDto> GetByIdAsync(int id)
    {
        var livro = await _livroRepo.GetByIdAsync(id);
        if (livro is null)
            throw new NotFoundException($"Livro com ID {id} não encontrado.");

        return new LivroResponseDto
        {
            Id = livro.Id,
            ISBN = livro.ISBN,
            Titulo = livro.Titulo,
            AnoPublicacao = livro.AnoPublicacao,
            Quantidade = livro.Quantidade,
            AutorId = livro.AutorId,
            NomeAutor = livro.Autor?.Nome ?? string.Empty
        };
    }

    public async Task<LivroResponseDto> CreateAsync(CriarLivroDto dto)
    {
        var autor = await _autorRepo.GetByIdAsync(dto.AutorId);
        if (autor is null)
            throw new NotFoundException($"Autor com ID {dto.AutorId} não encontrado.");

        var livro = new Livro
        {
            ISBN = dto.ISBN,
            Titulo = dto.Titulo,
            AnoPublicacao = dto.AnoPublicacao,
            Quantidade = dto.Quantidade,
            AutorId = dto.AutorId
        };

        await _livroRepo.AddAsync(livro);
        await _livroRepo.SaveChangesAsync();

        return new LivroResponseDto
        {
            Id = livro.Id,
            ISBN = livro.ISBN,
            Titulo = livro.Titulo,
            AnoPublicacao = livro.AnoPublicacao,
            Quantidade = livro.Quantidade,
            AutorId = livro.AutorId,
            NomeAutor = autor.Nome
        };
    }
}
