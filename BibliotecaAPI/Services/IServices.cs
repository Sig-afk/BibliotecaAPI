using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Services;

public interface IAutorService
{
    Task<IEnumerable<AutorResponseDto>> GetAllAsync();
    Task<AutorResponseDto> GetByIdAsync(int id);
    Task<AutorResponseDto> CreateAsync(CriarAutorDto dto);
}

public interface ILivroService
{
    Task<IEnumerable<LivroResponseDto>> GetAllAsync(string? titulo, string? autor);
    Task<LivroResponseDto> GetByIdAsync(int id);
    Task<LivroResponseDto> CreateAsync(CriarLivroDto dto);
}

public interface IAlunoService
{
    Task<IEnumerable<AlunoResponseDto>> GetAllAsync();
    Task<AlunoResponseDto> GetByIdAsync(int id);
    Task<AlunoResponseDto> CreateAsync(CriarAlunoDto dto);
}

public interface IEmprestimoService
{
    Task<IEnumerable<EmprestimoResponseDto>> GetAllAsync();
    Task<EmprestimoResponseDto> GetByIdAsync(int id);
    Task<EmprestimoResponseDto> CreateAsync(CriarEmprestimoDto dto);
    Task<EmprestimoResponseDto> DevolverAsync(int id);
}
