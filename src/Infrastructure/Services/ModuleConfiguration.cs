using FastTodo.Domain.Constants;
using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain.Repositories;
using FastTodo.Infrastructure.Repositories;
using FastTodo.Infrastructure.Services.MediaR.Behaviors;
using FastTodo.Persistence.EF;
using FastTodo.Persistence.SQLite;
using FastTodo.Persistence.Postgres;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FastTodo.Infrastructure.Domain.Options;
using FastTodo.Infrastructure.Domain;
using FastTodo.Infrastructure.Domain.Configurations;
using Microsoft.AspNetCore.Builder;

namespace FastTodo.Infrastructure;

public static partial class ModuleConfiguration
{
    public static void AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddDatabaseProvider(configuration);

        services.AddAPICors(configuration);
    }

    public static void UseInFrastructure(this IApplicationBuilder app)
    {
        app.UseAPICors();
    }

    private static void AddDatabaseProvider(this IServiceCollection services,
        IConfiguration configuration)
    {
        var providerString = configuration.GetSection(nameof(FastTodoOption.SqlProvider)).Value;
        if (!Enum.TryParse<DatabaseProviderType>(providerString, true, out var provider))
            throw new Exception($"Invalid SqlProvider configuration: {providerString}");

        services.AddTransient<IUserContext, UserContext>();

        switch (provider)
        {
            case DatabaseProviderType.Postgres:
                services.AddPostgresPersistence();
                break;
            case DatabaseProviderType.SQLServer:
                services.AddSQLEFPersistence();
                break;
            default:
                services.AddSQLiteEFPersistence();
                break;
        }

        services.AddKeyedScoped<IUnitOfWork, EFUnitOfWork>(ServiceKeys.FastTodoEFUnitOfWork);
        services.AddTransient(typeof(IRepository<,>), typeof(EfRepository<,>));
    }
}