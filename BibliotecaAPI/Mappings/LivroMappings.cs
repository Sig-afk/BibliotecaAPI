using BibliotecaAPI.DTOs;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Mappings;

public static class LivroMappings
{
    public static LivroResponseDto ToResponseDto(this Livro livro, string? nomeAutor = null) => new()
    {
        Id = livro.Id,
        ISBN = livro.ISBN,
        Titulo = livro.Titulo,
        AnoPublicacao = livro.AnoPublicacao,
        Quantidade = livro.Quantidade,
        AutorId = livro.AutorId,
        NomeAutor = nomeAutor ?? livro.Autor?.Nome ?? string.Empty
    };

    public static Livro ToEntity(this CriarLivroDto dto) => new()
    {
        ISBN = dto.ISBN,
        Titulo = dto.Titulo,
        AnoPublicacao = dto.AnoPublicacao,
        Quantidade = dto.Quantidade,
        AutorId = dto.AutorId
    };
}
