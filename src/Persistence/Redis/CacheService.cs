using FastTodo.Domain.Constants;
using FastTodo.Infrastructure.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FastTodo.Persistence.Redis;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    private readonly bool _isRedisCacheProvider;

    private readonly IConnectionMultiplexer? _connectionMultiplexer;

    public CacheService(
        ILogger<CacheService> logger,
        IDistributedCache distributedCache,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _distributedCache = distributedCache;
        var redisConnectionString = configuration.GetConnectionString(nameof(ConnectionStrings.Redis));
        _isRedisCacheProvider = redisConnectionString is not null;

        if (_isRedisCacheProvider)
        {
            logger.LogInformation("Connect to redis");
            _connectionMultiplexer = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
        }
    }

    public string GenerateKey(params string[] keys)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> func, int cacheTimeInMinutes, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync<T>(string key, T data, int cacheTimeInMinutes, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public Task<(bool, T? cacheData)> TryGetValueAsync<T>(string key, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
}
