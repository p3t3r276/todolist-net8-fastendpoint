using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Redis;

public static class ModuleConfiguration
{
    public static IServiceCollection AddSQLiteEFPersistence(this IServiceCollection services)
    {
        services.AddFrameworkDbContexts();
        return services;
    }

    private static IServiceCollection AddFrameworkDbContexts(this IServiceCollection services)
    {
        services.AddDbContext<FastTodoSqliteDbContext>();
        services.AddScoped<BaseDbContext, FastTodoSqliteDbContext>();
        return services;
    }
}
