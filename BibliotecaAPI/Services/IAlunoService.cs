using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Services;

public interface IAlunoService
{
    Task<IEnumerable<AlunoResponseDto>> GetAllAsync();
    Task<AlunoResponseDto> GetByIdAsync(int id);
    Task<AlunoResponseDto> CreateAsync(CriarAlunoDto dto);
}
