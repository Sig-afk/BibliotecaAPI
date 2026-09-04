namespace BibliotecaAPI.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; init; } = "Sqlite";

    public bool UsesPostgres => Provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase);
}
