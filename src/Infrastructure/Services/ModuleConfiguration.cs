using FastTodo.Domain.Constants;
using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain.Repositories;
using FastTodo.Infrastructure.Repositories;
using FastTodo.Infrastructure.Services.MediaR.Behaviors;
using FastTodo.Persistence.EF;
using FastTodo.Persistence.SQLite;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            case DatabaseProviderType.SQLite:
                services.AddSQLiteEFPersistence();
                break;
            case DatabaseProviderType.SQLServer:
                services.AddSQLEFPersistence();
                break;
            default:
                throw new Exception($"Unsupported SqlProvider: {provider}");
        }

        services.AddKeyedScoped<IUnitOfWork, EFUnitOfWork>(ServiceKeys.FastTodoEFUnitOfWork);
        services.AddTransient(typeof(IRepository<,>), typeof(EfRepository<,>));
        return services;
    }
}