namespace BibliotecaAPI.Data;

public sealed class EfUnitOfWork(BibliotecaContext database) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await database.SaveChangesAsync(cancellationToken);
    }
}
