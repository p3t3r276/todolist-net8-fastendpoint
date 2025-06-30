using FastTodo.Infrastructure.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Persistence.EF;

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddSQLEFPersistence(this IServiceCollection services)
    {
        services.AddFrameworkDbContexts();

        // services.AddKeyedScoped<IUnitOfWork, DefaultEfCommandUnitOfWork>(ServiceKeys.DefaultEFCommandUnitOfWork);
        //
        // services.TryAddScoped(typeof(IQueryRepository<,>), typeof(DefaultQueryRepository<,>));
        return services;
    }

    private static IServiceCollection AddFrameworkDbContexts(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
            options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
            options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
        }).AddBearerToken(IdentityConstants.BearerScheme);

        services.AddDbContext<FastTodoSQLDbContext>();
        services.AddScoped<BaseDbContext, FastTodoSQLDbContext>();

        services.AddDbContext<FastTodoIdentity>();
        services.AddIdentityCore<AppUser>()
            .AddEntityFrameworkStores<FastTodoIdentity>()
            .AddApiEndpoints();

        return services;
    }

    public static WebApplication UseEFPersistence(this WebApplication application)
    {
        application.MapGroup("accounts").MapIdentityApi<AppUser>();
        return application;
    }
}
