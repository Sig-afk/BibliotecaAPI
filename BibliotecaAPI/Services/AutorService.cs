using BibliotecaAPI.DTOs;
using BibliotecaAPI.Data;
using BibliotecaAPI.Exceptions;
using BibliotecaAPI.Mappings;
using BibliotecaAPI.Repositories;

namespace BibliotecaAPI.Services;

public sealed class AutorService(
    IAutorRepository repository,
    IUnitOfWork unitOfWork) : IAutorService
{
    public async Task<IEnumerable<AutorResponseDto>> GetAllAsync()
    {
        var autores = await repository.GetAllAsync();
        return autores.Select(autor => autor.ToResponseDto());
    }

    public async Task<AutorResponseDto> GetByIdAsync(int id)
    {
        var autor = await repository.GetByIdAsync(id);
        if (autor is null)
            throw new NotFoundException($"Autor com ID {id} não encontrado.");

        return autor.ToResponseDto();
    }

    public async Task<AutorResponseDto> CreateAsync(CriarAutorDto dto)
    {
        var autor = dto.ToEntity();
        repository.Add(autor);
        await unitOfWork.SaveChangesAsync();

        return autor.ToResponseDto();
    }
}
