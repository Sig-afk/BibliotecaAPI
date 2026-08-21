using BibliotecaAPI.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public sealed class TestDatabaseFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public BibliotecaContext Context { get; }

    public TestDatabaseFixture()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<BibliotecaContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new BibliotecaContext(options);

        Context.Database.EnsureCreated();

        // Context.Database.Migrate();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}