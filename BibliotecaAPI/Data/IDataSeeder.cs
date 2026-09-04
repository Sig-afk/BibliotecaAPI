namespace BibliotecaAPI.Data;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
