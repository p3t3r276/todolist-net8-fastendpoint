using FastTodo.Infrastructure.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
        services.AddDbContext<FastTodoSQLDbContext>();
        services.AddScoped<BaseDbContext, FastTodoSQLDbContext>();

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<FastTodoIdentity>()
            .AddDefaultTokenProviders();
        return services;
    }
}