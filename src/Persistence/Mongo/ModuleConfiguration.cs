using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain.Repositories;
using FastTodo.Persistence.Mongo.DbContexts.MongoDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Persistence.Mongo;

public static class ModuleConfiguration
{
    public static void AddMongoPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFrameworkDbContexts(configuration);
    }

    private static void AddFrameworkDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDBSettings = configuration.GetSection(nameof(MongoDBSettings)).Get<MongoDBSettings>();

        services.Configure<MongoDBSettings>(configuration.GetSection(nameof(MongoDBSettings)));
        ArgumentNullException.ThrowIfNull(mongoDBSettings);

        services.AddDbContext<MongoDbContext>(options =>
        {
            options.UseMongoDB(mongoDBSettings.AtlasURI ?? "", mongoDBSettings.DatabaseName ?? "");
        });

        services.AddKeyedScoped<IMongoUnitOfWork, MongoUnitOfWork>(ServiceKeys.FastTodoEFUnitOfWork);
        services.AddTransient(typeof(IMongoQueryRepository<,>), typeof(MongoQueryRepository<,>));
    }
}
