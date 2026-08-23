using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Services;

public interface IEmprestimoService
{
    Task<IEnumerable<EmprestimoResponseDto>> GetAllAsync();
    Task<EmprestimoResponseDto> GetByIdAsync(int id);
    Task<EmprestimoResponseDto> CreateAsync(CriarEmprestimoDto dto);
    Task<EmprestimoResponseDto> DevolverAsync(int id);
}
