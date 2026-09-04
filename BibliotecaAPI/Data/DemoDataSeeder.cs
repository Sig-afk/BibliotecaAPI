using BibliotecaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Data;

public sealed class DemoDataSeeder(BibliotecaContext database) : IDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedAutoresAsync(cancellationToken);
        await SeedAlunosAsync(cancellationToken);
        await SeedLivrosAsync(cancellationToken);
    }

    private async Task SeedAutoresAsync(CancellationToken cancellationToken)
    {
        if (await database.Autores.AnyAsync(cancellationToken))
            return;

        database.Autores.AddRange(
            new Autor
            {
                Nome = "Machado de Assis",
                Nacionalidade = "Brasileira",
                DataNascimento = new DateTime(1839, 6, 21, 0, 0, 0, DateTimeKind.Utc)
            },
            new Autor
            {
                Nome = "Clarice Lispector",
                Nacionalidade = "Brasileira",
                DataNascimento = new DateTime(1920, 12, 10, 0, 0, 0, DateTimeKind.Utc)
            });

        await database.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedAlunosAsync(CancellationToken cancellationToken)
    {
        if (await database.Alunos.AnyAsync(cancellationToken))
            return;

        database.Alunos.AddRange(
            new Aluno { Nome = "Ana Souza", Matricula = "2026001", Email = "ana@aluno.local" },
            new Aluno { Nome = "Carlos Lima", Matricula = "2026002", Email = "carlos@aluno.local" });

        await database.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedLivrosAsync(CancellationToken cancellationToken)
    {
        if (await database.Livros.AnyAsync(cancellationToken))
            return;

        var autores = await database.Autores
            .OrderBy(autor => autor.Id)
            .Take(2)
            .ToArrayAsync(cancellationToken);

        if (autores.Length == 0)
            return;

        database.Livros.AddRange(
            new Livro { ISBN = "9788535910663", Titulo = "Dom Casmurro", AnoPublicacao = 1899, Quantidade = 4, AutorId = autores[0].Id },
            new Livro { ISBN = "9788532508126", Titulo = "Memórias Póstumas de Brás Cubas", AnoPublicacao = 1881, Quantidade = 3, AutorId = autores[0].Id },
            new Livro { ISBN = "9788532508133", Titulo = "A Hora da Estrela", AnoPublicacao = 1977, Quantidade = 5, AutorId = autores.ElementAtOrDefault(1)?.Id ?? autores[0].Id });

        await database.SaveChangesAsync(cancellationToken);
    }
}
