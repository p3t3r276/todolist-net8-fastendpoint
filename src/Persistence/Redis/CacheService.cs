using System.Text.Json;
using FastTodo.Domain.Constants;
using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain;
using FastTodo.Infrastructure.Domain.Options;
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
        FastTodoOption options)
    {
        _distributedCache = distributedCache;
        _isRedisCacheProvider = options.CacheType == CacheType.Redis;

        if (_isRedisCacheProvider)
        {
            logger.LogInformation("Connect to redis");
            _connectionMultiplexer = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellation = default)
    {
        var cacheData = await _distributedCache.GetStringAsync(key, token: cancellation);

        if (string.IsNullOrEmpty(cacheData)) { return default; }

        return JsonSerializer.Deserialize<T?>(cacheData);
    }

    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> func,
        int cacheTimeInMinutes,
        CancellationToken cancellation = default)
    {
        var value = await GetAsync<T?>(key, cancellation);

        if (value is not null)
        {
            return value;
        }

        value = await func();

        if (value is not null)
        {
            await SetAsync(key, value, cacheTimeInMinutes, cancellation);
        }

        return value;
    }

    public async Task SetAsync<T>(string key, T data, int cacheTimeInMinutes, CancellationToken cancellation = default)
    {
        var serializedData = JsonSerializer.Serialize(data);

        await _distributedCache.SetStringAsync(key, serializedData, GetTimeOutOption(cacheTimeInMinutes), cancellation);
    }

    public Task<(bool, T? cacheData)> TryGetValueAsync<T>(string key, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public string GenerateKey(params string[] keys)
    {
        throw new NotImplementedException();
    }

    private static DistributedCacheEntryOptions GetTimeOutOption(int cacheTimeInMinutes)
    {
        DistributedCacheEntryOptions option = new();
        option.SetAbsoluteExpiration(DateTime.UtcNow.AddMinutes(cacheTimeInMinutes));

        return option;
    }
}
