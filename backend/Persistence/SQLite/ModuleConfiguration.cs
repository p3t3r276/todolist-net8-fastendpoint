using FastTodo.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Persistence.SQLite;

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddSQLiteEFPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFrameworkDbContexts(configuration);

        // services.AddKeyedScoped<IUnitOfWork, DefaultEfCommandUnitOfWork>(ServiceKeys.DefaultEFCommandUnitOfWork);
        //
        // services.TryAddScoped(typeof(IQueryRepository<,>), typeof(DefaultQueryRepository<,>));
        return services;
    }
    
    private static IServiceCollection AddFrameworkDbContexts(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FastTodoSqliteDbContext>();
        services.AddScoped<BaseDbContext, FastTodoSqliteDbContext>();
        return services;
    }
}
