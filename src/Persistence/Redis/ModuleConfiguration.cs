using FastTodo.Domain.Constants;
using FastTodo.Infrastructure.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FastTodo.Persistence.Redis;

public static class ModuleConfiguration
{
    public static void AddRedisPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString(nameof(ConnectionStrings.Redis));

        if (redisConnectionString is null)
        {
            services.AddMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString(redisConnectionString);
            });
            services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect(redisConnectionString));
        }
        services.AddScoped<ICacheService, CacheService>();
    }
}
