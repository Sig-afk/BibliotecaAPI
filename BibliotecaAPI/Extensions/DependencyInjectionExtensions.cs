using BibliotecaAPI.Configuration;
using BibliotecaAPI.Data;
using BibliotecaAPI.Repositories;
using BibliotecaAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBibliotecaApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        ValidateDatabaseProvider(databaseOptions);

        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddOptions<DemoAuthOptions>()
            .Bind(configuration.GetSection(DemoAuthOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Email), "DemoAuth:Email é obrigatório.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Password), "DemoAuth:Password é obrigatório.")
            .ValidateOnStart();

        services.AddDatabase(configuration, databaseOptions);
        services.AddRepositories();
        services.AddApplicationServices();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => options.SwaggerDoc("v1", new()
        {
            Title = "Biblioteca API",
            Version = "v1",
            Description = "API RESTful para gerenciamento de biblioteca escolar."
        }));

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        DatabaseOptions options)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=biblioteca.db";

        services.AddDbContext<BibliotecaContext>(builder =>
        {
            if (options.UsesPostgres)
                builder.UseNpgsql(connectionString);
            else
                builder.UseSqlite(connectionString);
        });

        services.AddStackExchangeRedisCache(redis =>
        {
            redis.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            redis.InstanceName = "BibliotecaAPI:";
        });

        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        services.AddScoped<IDataSeeder, DemoDataSeeder>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAutorRepository, AutorRepository>();
        services.AddScoped<ILivroRepository, LivroRepository>();
        services.AddScoped<IAlunoRepository, AlunoRepository>();
        services.AddScoped<IEmprestimoRepository, EmprestimoRepository>();
        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAutorService, AutorService>();
        services.AddScoped<ILivroService, LivroService>();
        services.AddScoped<IAlunoService, AlunoService>();
        services.AddScoped<IEmprestimoService, EmprestimoService>();
        services.AddScoped<IHealthService, HealthService>();
        services.AddSingleton<IAuthService, AuthService>();
        return services;
    }

    private static void ValidateDatabaseProvider(DatabaseOptions options)
    {
        var supported = options.UsesPostgres
            || options.Provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase);

        if (!supported)
            throw new InvalidOperationException(
                $"Database:Provider '{options.Provider}' não é suportado. Use 'Sqlite' ou 'Postgres'.");
    }
}
