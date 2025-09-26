using FastTodo.Domain.Entities.Identity;
using FastTodo.Infrastructure.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Persistence.EF;

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddSQLEFPersistence(this IServiceCollection services)
    {
        services.AddFrameworkDbContexts();
        return services;
    }

    private static IServiceCollection AddFrameworkDbContexts(this IServiceCollection services)
    {
        services.AddDbContext<FastTodoSQLDbContext>();
        services.AddScoped<BaseDbContext, FastTodoSQLDbContext>();

        services.AddDbContext<FastTodoIdentityDbContext>();
        services.AddIdentityCore<AppUser>()
            .AddEntityFrameworkStores<FastTodoIdentityDbContext>()
            .AddApiEndpoints();

        return services;
    }
}
