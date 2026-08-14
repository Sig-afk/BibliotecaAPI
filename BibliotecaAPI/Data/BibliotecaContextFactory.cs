using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BibliotecaAPI.Data;

/// <summary>
/// Factory necessária para o EF Core CLI (dotnet ef migrations).
/// </summary>
public class BibliotecaContextFactory : IDesignTimeDbContextFactory<BibliotecaContext>
{
    public BibliotecaContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BibliotecaContext>();
        optionsBuilder.UseSqlite("Data Source=biblioteca.db");
        return new BibliotecaContext(optionsBuilder.Options);
    }
}
