using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Services;

public interface ILivroService
{
    Task<IEnumerable<LivroResponseDto>> GetAllAsync(string? titulo, string? autor);
    Task<LivroResponseDto> GetByIdAsync(int id);
    Task<LivroResponseDto> CreateAsync(CriarLivroDto dto);
}
