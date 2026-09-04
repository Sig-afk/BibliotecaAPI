using BibliotecaAPI.DTOs;
using BibliotecaAPI.Data;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Mappings;
using BibliotecaAPI.Repositories;

namespace BibliotecaAPI.Services;

public sealed class LivroService(
    ILivroRepository livroRepository,
    IAutorRepository autorRepository,
    IUnitOfWork unitOfWork) : ILivroService
{
    public async Task<IEnumerable<LivroResponseDto>> GetAllAsync(string? titulo, string? autor)
    {
        var livros = await livroRepository.GetAllAsync(titulo, autor);
        return livros.Select(livro => livro.ToResponseDto());
    }

    public async Task<LivroResponseDto> GetByIdAsync(int id)
    {
        var livro = await livroRepository.GetByIdAsync(id);
        if (livro is null)
            throw new NotFoundException($"Livro com ID {id} não encontrado.");

        return livro.ToResponseDto();
    }

    public async Task<LivroResponseDto> CreateAsync(CriarLivroDto dto)
    {
        var autor = await autorRepository.GetByIdAsync(dto.AutorId);
        if (autor is null)
            throw new NotFoundException($"Autor com ID {dto.AutorId} não encontrado.");

        var livro = dto.ToEntity();
        livroRepository.Add(livro);
        await unitOfWork.SaveChangesAsync();

        return livro.ToResponseDto(autor.Nome);
    }
}
