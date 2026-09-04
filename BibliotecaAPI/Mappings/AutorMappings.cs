using BibliotecaAPI.DTOs;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Mappings;

public static class AutorMappings
{
    public static AutorResponseDto ToResponseDto(this Autor autor) => new()
    {
        Id = autor.Id,
        Nome = autor.Nome,
        DataNascimento = autor.DataNascimento,
        Nacionalidade = autor.Nacionalidade
    };

    public static Autor ToEntity(this CriarAutorDto dto) => new()
    {
        Nome = dto.Nome,
        DataNascimento = dto.DataNascimento,
        Nacionalidade = dto.Nacionalidade
    };
}
