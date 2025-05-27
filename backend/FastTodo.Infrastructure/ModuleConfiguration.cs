using FastTodo.Infrastructure.Services.MediaR.Behaviors;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FastTodo.Infrastructure.Repositories;
using FastTodo.Persistence.EF;
using FastTodo.Persistence.SQLite;
using FastTodo.Infrastructure.Domain;

namespace FastTodo.Infrastructure;

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddDatabaseProvider(configuration);
        return services;
    }

    public enum DatabaseProviderType
    {
        Sqlite,
        SqlServer
    }

    public static IServiceCollection AddDatabaseProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var providerString = configuration["DatabaseProvider"];
        if (!Enum.TryParse<DatabaseProviderType>(providerString, true, out var provider))
            throw new Exception($"Invalid DatabaseProvider configuration: {providerString}");

        switch (provider)
        {
            case DatabaseProviderType.Sqlite:
                services.AddDbContext<FastTodoSqliteDbContext>();
                services.AddScoped<ITodoDbContext, FastTodoSqliteDbContext>();
                break;
            case DatabaseProviderType.SqlServer:
                services.AddDbContext<FastTodoSQLDbContext>();
                services.AddScoped<ITodoDbContext, FastTodoSQLDbContext>();
                break;
            default:
                throw new Exception($"Unsupported DatabaseProvider: {provider}");
        }
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        return services;
    }
}