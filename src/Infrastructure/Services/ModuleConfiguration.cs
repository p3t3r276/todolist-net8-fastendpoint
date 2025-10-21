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
using Microsoft.AspNetCore.Identity;
using FastTodo.Infrastructure.Services;
using FastTodo.Persistence.Redis;

namespace FastTodo.Infrastructure;

public static partial class ModuleConfiguration
{
    public static void AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
            options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
            options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
        }).AddBearerToken(IdentityConstants.BearerScheme);

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
        var options = configuration.GetSection(nameof(FastTodoOption)).Get<FastTodoOption>();

        ArgumentNullException.ThrowIfNull(options);

        var sqlProvider = Enum.TryParse<DatabaseProviderType>(options.SQLProvider.ToString(), true, out var provider) 
            ? provider 
            : throw new Exception($"Invalid SqlProvider configuration: {options.Serialize()}");

        services.AddTransient<IUserContext, UserContext>();

        switch (sqlProvider)
        {
            case DatabaseProviderType.Postgres:
                services.AddPostgresPersistence();
                break;
            case DatabaseProviderType.SQLServer:
                services.AddSQLEFPersistence();
                break;
            default:
                services.AddSQLitePersistence();
                break;
        }

        services.AddKeyedScoped<IUnitOfWork, EFUnitOfWork>(ServiceKeys.FastTodoEFUnitOfWork);
        services.AddTransient(typeof(IRepository<,>), typeof(EfRepository<,>));
        services.AddTransient<ICacheService, CacheService>();
    }
}
