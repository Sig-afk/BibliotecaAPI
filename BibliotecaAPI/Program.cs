using BibliotecaAPI;
using BibliotecaAPI.Data;
using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;
using BibliotecaAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=biblioteca.db";

builder.Services.AddDbContext<BibliotecaContext>(options =>
{
    if (databaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
        options.UseNpgsql(connectionString);
    else
        options.UseSqlite(connectionString);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "BibliotecaAPI:";
});

builder.Services.AddScoped<IAutorRepository, AutorRepository>();
builder.Services.AddScoped<ILivroRepository, LivroRepository>();
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
builder.Services.AddScoped<IEmprestimoRepository, EmprestimoRepository>();
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ILivroService, LivroService>();
builder.Services.AddScoped<IAlunoService, AlunoService>();
builder.Services.AddScoped<IEmprestimoService, EmprestimoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new()
{
    Title = "Biblioteca API",
    Version = "v1",
    Description = "API RESTful para gerenciamento de biblioteca escolar."
}));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<BibliotecaContext>();

    // As migrations existentes pertencem ao SQLite. PostgreSQL cria o mesmo
    // modelo diretamente, evitando SQL específico de outro provider.
    if (databaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
        database.Database.EnsureCreated();
    else
        database.Database.Migrate();

    if (!database.Autores.Any())
    {
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
        database.SaveChanges();
    }

    if (!database.Alunos.Any())
    {
        database.Alunos.AddRange(
            new Aluno { Nome = "Ana Souza", Matricula = "2026001", Email = "ana@aluno.local" },
            new Aluno { Nome = "Carlos Lima", Matricula = "2026002", Email = "carlos@aluno.local" });
        database.SaveChanges();
    }

    if (!database.Livros.Any())
    {
        var autores = database.Autores.OrderBy(a => a.Id).Take(2).ToArray();
        database.Livros.AddRange(
            new Livro { ISBN = "9788535910663", Titulo = "Dom Casmurro", AnoPublicacao = 1899, Quantidade = 4, AutorId = autores[0].Id },
            new Livro { ISBN = "9788532508126", Titulo = "Memórias Póstumas de Brás Cubas", AnoPublicacao = 1881, Quantidade = 3, AutorId = autores[0].Id },
            new Livro { ISBN = "9788532508133", Titulo = "A Hora da Estrela", AnoPublicacao = 1977, Quantidade = 5, AutorId = autores.ElementAtOrDefault(1)?.Id ?? autores[0].Id });
        database.SaveChanges();
    }
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Biblioteca API v1");
    c.RoutePrefix = "docs";
});
app.UseAuthorization();
app.MapControllers();

app.Run();
