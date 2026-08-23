using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Services;

public interface IAutorService
{
    Task<IEnumerable<AutorResponseDto>> GetAllAsync();
    Task<AutorResponseDto> GetByIdAsync(int id);
    Task<AutorResponseDto> CreateAsync(CriarAutorDto dto);
}
