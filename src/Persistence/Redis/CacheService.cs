using System.Text.Json;
using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain;
using FastTodo.Infrastructure.Domain.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using static System.Threading.Tasks.Task;
using static System.Text.Encoding;

namespace FastTodo.Persistence.Redis;

/// <summary>
/// Provides distributed caching functionality with support for Redis and in-memory cache providers.
/// Implements the cache-aside pattern and supports both individual and bulk operations.
/// </summary>
public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    private const string cacheStoreKey = "CacheStore:Outbox:Keys";

    private readonly bool _isRedisCacheProvider;

    private readonly IConnectionMultiplexer? _connectionMultiplexer;

    private readonly IDatabase? _database;

    readonly ILogger<CacheService> _logger;

    public CacheService(
        ILogger<CacheService> logger,
        IDistributedCache distributedCache,
        IServiceProvider serviceProvider,
        FastTodoOption options)
    {
        _logger = logger;
        _distributedCache = distributedCache;
        _isRedisCacheProvider = options.CacheType == CacheType.Redis;

        if (_isRedisCacheProvider)
        {
            logger.LogInformation("Connect to redis");
            _connectionMultiplexer = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
            _database = _connectionMultiplexer.GetDatabase();
        }
    }

    /// <summary>
    /// Retrieves a cached value by key with automatic JSON deserialization.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the cached data to.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellation">Cancellation token.</param>
    /// <returns>The cached value or default if not found.</returns>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellation = default)
    {
        try
        {
            var cacheData = await _distributedCache.GetStringAsync(key, token: cancellation);

            if (string.IsNullOrEmpty(cacheData)) { return default; }

            return JsonSerializer.Deserialize<T?>(cacheData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAsync-CacheService: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all cached values from a Redis hash by group key.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the cached data to.</typeparam>
    /// <param name="key">The hash group key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary of all cached values in the hash.</returns>
    public async Task<Dictionary<string, T?>?> GetAllAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Group name cannot be null.");
            }

            var result = new Dictionary<string, T?>();
            var slim = new SemaphoreSlim(1);

            await WhenAll((await _database.HashGetAllAsync(key.ToLowerInvariant()).WaitAsync(cancellationToken))
                .Where(e => e.Name.HasValue && e.Value.HasValue).Select(async x =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (x.Value.HasValue)
                {
                    await slim.WaitAsync(cancellationToken);

                    try
                    {
                        var key = x.Name.ToString();

                        if (!result.ContainsKey(key))
                        {
                            result.Add(key, JsonSerializer.Deserialize<T>(UTF8.GetString(x.Value!)));
                        }
                    }
                    finally
                    {
                        _ = slim.Release();
                    }
                }
            }));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync-CacheService: {key}", key);
            throw;
        }
    }

    /// <summary>
    /// Implements the cache-aside pattern: retrieves from cache or executes the function and caches the result.
    /// </summary>
    /// <typeparam name="T">The type of data to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="func">The function to execute if cache miss occurs.</param>
    /// <param name="cacheTimeInMinutes">Cache expiration time in minutes.</param>
    /// <param name="cancellation">Cancellation token.</param>
    /// <returns>The cached or newly computed value.</returns>
    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> func,
        int cacheTimeInMinutes,
        CancellationToken cancellation = default)
    {
        try
        {
            var value = await GetAsync<T>(key, cancellation);

            if (!IsNullOrDefault(value))
            {
                return value;
            }

            value = await func();

            if (!IsNullOrDefault(value))
            {
                await SetAsync(key, value, cacheTimeInMinutes, cancellation);
            }

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOrSetAsync-CacheService: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// Stores a value in the cache with automatic JSON serialization.
    /// </summary>
    /// <typeparam name="T">The type of data to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="data">The data to cache.</param>
    /// <param name="cacheTimeInMinutes">Cache expiration time in minutes.</param>
    /// <param name="cancellation">Cancellation token.</param>
    public async Task SetAsync<T>(string key, T data, int cacheTimeInMinutes, CancellationToken cancellation = default)
    {
        try
        {
            var serializedData = JsonSerializer.Serialize(data);

            await _distributedCache.SetStringAsync(key, serializedData, GetTimeOutOption(cacheTimeInMinutes), cancellation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetAsync-CacheService: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// Attempts to retrieve a cached value without throwing exceptions on cache miss.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the cached data to.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellation">Cancellation token.</param>
    /// <returns>A tuple indicating success and the cached data if found.</returns>
    public Task<(bool, T? cacheData)> TryGetValueAsync<T>(string key, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes a cached value by key and updates the cache key outbox.
    /// </summary>
    /// <param name="key">The cache key to remove.</param>
    /// <param name="cancellation">Cancellation token.</param>
    public async Task RemoveAsync(string key, CancellationToken cancellation = default)
    {
        try
        {
            await Task.WhenAll(
                SyncCacheKeyOutbox(key, true, cancellation),
                _distributedCache.RemoveAsync(key, cancellation));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoveAsync-CacheService: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// Stores multiple key-value pairs in a Redis hash for bulk operations.
    /// </summary>
    /// <typeparam name="TRedisDto">The type of data to store in the hash.</typeparam>
    /// <param name="group">The hash group key.</param>
    /// <param name="fields">Dictionary of field names and values to store.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation succeeds.</returns>
    public async Task<bool> SetBulkAsync<TRedisDto>(
        string group,
        IDictionary<string, TRedisDto> fields,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (string.IsNullOrWhiteSpace(group) || fields is null || fields.Count is 0)
            {
                throw new Exception("BAD_REQUEST");
            }

            await _database.HashSetAsync(
                group.ToLowerInvariant(),
                [.. fields.Select(static p => new HashEntry(p.Key.ToLowerInvariant(), p.Value.Serialize()))])
                .WaitAsync(cancellationToken);

            return true;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning("Operation was canceled: {Method} - {Group}", nameof(SetBulkAsync), group);

            throw new OperationCanceledException($"Redis SetBulkAsync operation canceled for {group}", ex, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetBulkAsync-RedisService-Exception: {Group} - {Fields}", group, fields.Serialize());

            throw;
        }
    }

    /// <summary>
    /// Generates a composite cache key from multiple key segments.
    /// </summary>
    /// <param name="keys">The key segments to combine.</param>
    /// <returns>A composite cache key.</returns>
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

    private static bool IsNullOrDefault<T>(T? value)
    {
        if (value is null) { return true; }

        return !typeof(T).IsValueTupleType() ? EqualityComparer<T>.Default.Equals(value, default) :
            typeof(T).GetFields().All(field => IsNullOrDefault(field.GetValue(value)));
    }

    private async Task SyncCacheKeyOutbox(string key, bool shouldRemove = false, CancellationToken cancellation = default)
    {
        if (_isRedisCacheProvider) return;

        var outboxKeys = (await GetAsync<HashSet<string>>(cacheStoreKey, cancellation)) ?? [];

        if (!shouldRemove && !outboxKeys.Add(key))
        {
            return;
        }

        if (shouldRemove && !outboxKeys.Remove(key))
        {
            return;
        }

        await _distributedCache.SetStringAsync(cacheStoreKey, outboxKeys.Serialize(), GetOutBoxCacheTimeOut(), cancellation);
    }

    private static DistributedCacheEntryOptions GetOutBoxCacheTimeOut()
    {
        return new DistributedCacheEntryOptions()
        {
            AbsoluteExpiration = DateTime.UtcNow.AddYears(20)
        };
    }
}

public static class ReflectionExtention
{
    public static bool IsValueTupleType(this Type type)
        =>  type.IsGenericType && type.FullName?.StartsWith("System.ValueTuple") == true;
}
