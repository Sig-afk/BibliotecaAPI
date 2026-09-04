using BibliotecaAPI.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BibliotecaAPI.Data;

public sealed class DatabaseInitializer(
    BibliotecaContext database,
    IOptions<DatabaseOptions> options) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // As migrations versionadas foram geradas para SQLite. O PostgreSQL
        // cria o mesmo modelo a partir das entidades para não executar SQL
        // específico de outro provider.
        if (options.Value.UsesPostgres)
            await database.Database.EnsureCreatedAsync(cancellationToken);
        else
            await database.Database.MigrateAsync(cancellationToken);
    }
}
