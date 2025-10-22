using FastTodo.Infrastructure.Domain;
using FastTodo.Infrastructure.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FastTodo.Persistence.Redis;

public static class ModuleConfiguration
{
    public static void AddRedisPersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        FastTodoOption option)
    {
        if (option.CacheType == Domain.Shared.Constants.CacheType.InMemory)
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = option.RedisConnectionString;
            });
            services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect(option.RedisConnectionString!));
        }

        services.AddScoped<ICacheService, CacheService>();
    }
}
