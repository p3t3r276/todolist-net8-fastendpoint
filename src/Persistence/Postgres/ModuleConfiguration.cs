using FastTodo.Domain.Entities.Identity;
using FastTodo.Infrastructure.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace FastTodo.Persistence.Postgres;    

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddPostgresPersistence(this IServiceCollection services)
    {
        services.AddFrameworkDbContexts();
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

        services.AddDbContext<FastTodoPostgresContext>();
        services.AddScoped<BaseDbContext, FastTodoPostgresContext>();

        services.AddDbContext<FastTodoIdentityDbContext>();
        services.AddIdentityCore<AppUser>()
            .AddEntityFrameworkStores<FastTodoIdentityDbContext>()
            .AddApiEndpoints();

        return services;
    }
}
