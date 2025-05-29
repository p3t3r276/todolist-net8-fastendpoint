using FastTodo.Infrastructure.Services.MediaR.Behaviors;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FastTodo.Infrastructure.Repositories;
using FastTodo.Persistence.EF;
using FastTodo.Persistence.SQLite;

namespace FastTodo.Infrastructure;

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddDatabaseProvider(configuration);
        return services;
    }

    public static IServiceCollection AddDatabaseProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var providerString = configuration["SqlProvider"];
        if (!Enum.TryParse<DatabaseProviderType>(providerString, true, out var provider))
            throw new Exception($"Invalid SqlProvider configuration: {providerString}");

        switch (provider)
        {
            case DatabaseProviderType.Sqlite:
                services.AddSQLiteEFPersistence();
                break;
            case DatabaseProviderType.SqlServer:
                services.AddSQLEFPersistence();
                break;
            default:
                throw new Exception($"Unsupported SqlProvider: {provider}");
        }
        services.AddTransient(typeof(IRepository<,>), typeof(EfRepository<,>));
        return services;
    }
}