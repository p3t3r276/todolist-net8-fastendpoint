using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Persistence.EF;

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddSQLEFPersistence(this IServiceCollection services, IConfiguration configuration)
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
        services.AddDbContext<FastTodoSQLDbContext>(
            options => options.UseSqlServer(configuration.GetConnectionString("Sql")));

        return services;
    }
}