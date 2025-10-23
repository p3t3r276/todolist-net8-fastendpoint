using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Persistence.Mongo;

public static class ModuleConfiguration
{
    public static void AddPostgresPersistence(this IServiceCollection services)
    {
        services.AddFrameworkDbContexts();
    }

    private static void AddFrameworkDbContexts(this IServiceCollection services)
    {
    }
}
