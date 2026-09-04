using BibliotecaAPI.Data;

namespace BibliotecaAPI.Extensions;

public static class ApplicationInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();

        await initializer.InitializeAsync();
        await seeder.SeedAsync();
    }
}
