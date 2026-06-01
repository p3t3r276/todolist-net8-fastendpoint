using System.Text.Json;
using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain;
using FastTodo.Infrastructure.Domain.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using static System.Threading.Tasks.Task;
using static System.Text.Encoding;

namespace FastTodo.Persistence.Redis;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly HybridCache _hybridCache;

    private const string cacheStoreKey = "CacheStore:Outbox:Keys";

    private readonly bool _isRedisCacheProvider;

    private readonly IConnectionMultiplexer? _connectionMultiplexer;

    private readonly IDatabase? _database;

    readonly ILogger<CacheService> _logger;

    public CacheService(
        ILogger<CacheService> logger,
        IDistributedCache distributedCache,
        HybridCache hybridCache,
        IServiceProvider serviceProvider,
        FastTodoOption options)
    {
        _logger = logger;
        _distributedCache = distributedCache;
        _hybridCache = hybridCache;
        _isRedisCacheProvider = options.CacheType == CacheType.Redis;

        if (_isRedisCacheProvider)
        {
            logger.LogInformation("Connect to redis");
            _connectionMultiplexer = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
            _database = _connectionMultiplexer.GetDatabase();
        }
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            return await _hybridCache.GetOrCreateAsync<T?>(
                key,
                _ => ValueTask.FromResult(default(T)),
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAsync-CacheService: {Key}", key);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, T?>?> GetAllAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (!_isRedisCacheProvider || _database == null)
            {
                throw new Exception("This method is only supported in Redis cache provider.");
            }

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
                        var fieldName = x.Name.ToString();

                        if (!result.ContainsKey(fieldName))
                        {
                            result.Add(fieldName, JsonSerializer.Deserialize<T>(UTF8.GetString(x.Value!)));
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

    /// <inheritdoc />
    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> func,
        int cacheTimeInMinutes,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var options = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(cacheTimeInMinutes),
                LocalCacheExpiration = TimeSpan.FromMinutes(cacheTimeInMinutes)
            };

            return await _hybridCache.GetOrCreateAsync(
                key,
                async ct => await func(),
                options,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOrSetAsync-CacheService: {Key}", key);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T data, int cacheTimeInMinutes, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var options = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(cacheTimeInMinutes),
                LocalCacheExpiration = TimeSpan.FromMinutes(cacheTimeInMinutes)
            };

            await _hybridCache.SetAsync(key, data, options, cancellationToken: cancellationToken);
            
            if (!_isRedisCacheProvider)
            {
                await SyncCacheKeyOutbox(key, false, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetAsync-CacheService: {Key}-{Data}-{CacheTimeInMinutes}", key, data?.Serialize() ?? "", cacheTimeInMinutes);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<(bool, T? cacheData)> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cacheData = await GetAsync<T>(key, cancellationToken);

        return (!IsNullOrDefault(cacheData), cacheData);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await Task.WhenAll(
                SyncCacheKeyOutbox(key, true, cancellationToken),
                _hybridCache.RemoveAsync(key, cancellationToken).AsTask());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoveAsync-CacheService: {Key}", key);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SetBulkAsync<TRedisDto>(
        string group,
        IDictionary<string, TRedisDto> fields,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (!_isRedisCacheProvider || _database == null)
            {
                throw new Exception("This method is only supported in Redis cache provider.");
            }

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetBulkAsync-RedisService-Exception: {Group} - {Fields}", group, fields.Serialize());

            throw;
        }
    }

    /// <inheritdoc />
    public string GenerateKey(params string[] keys)
    {
        try
        {
            return string.Join(":", keys);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GenerateKey-CacheService: {Keys}", keys.Serialize());
            throw;
        }
    }

    public async Task RemoveRangeAsync(string keyPattern, CacheKeySearchOperator searchOperator = CacheKeySearchOperator.StartsWith, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (!_isRedisCacheProvider)
            {
                await InMemoryRemoveRange(keyPattern, searchOperator, cancellationToken);

                return;
            }

            await RedisRemoveRange(keyPattern, searchOperator, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoveRangeAsync-CacheService: {KeyPattern}-{SearchOperator}", keyPattern, searchOperator);
            throw;
        }
    }

    private async Task RedisRemoveRange(string keyPattern, CacheKeySearchOperator searchOperator, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPattern, nameof(keyPattern));

        if (_connectionMultiplexer == null || _database == null) return;

        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());

        var searchPattern = searchOperator switch
        {
            CacheKeySearchOperator.StartsWith => $"{keyPattern}*",
            CacheKeySearchOperator.EndsWith => $"*{keyPattern}",
            CacheKeySearchOperator.Contains => $"*{keyPattern}*",
            _ => throw new ArgumentOutOfRangeException(nameof(searchOperator), searchOperator, null)
        };

        var keys = server.Keys(_database.Database, searchPattern).ToArray();

        if (keys.Length == 0)
        {
            return;
        }

        foreach (var key in keys)
        {
            await _hybridCache.RemoveAsync(key.ToString(), cancellationToken);
        }

        await _database.KeyDeleteAsync(keys);
    }

    private async Task InMemoryRemoveRange(string keyPattern, CacheKeySearchOperator searchOperator, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPattern, nameof(keyPattern));

        var outboxKeys = (await GetAsync<HashSet<string>>(cacheStoreKey, cancellationToken)) ?? [];

        if (outboxKeys.Count == 0)
        {
            return;
        }

        HashSet<string> keysToRemove = [];

        switch(searchOperator)
        {
            case CacheKeySearchOperator.StartsWith:
                keysToRemove = [.. outboxKeys.Where(x => x.StartsWith(keyPattern))];
                break;
            case CacheKeySearchOperator.EndsWith:
                keysToRemove = [.. outboxKeys.Where(x => x.EndsWith(keyPattern))];
                break;
            case CacheKeySearchOperator.Contains:
                keysToRemove = [.. outboxKeys.Where(x => x.Contains(keyPattern))];
                break;
        }

        if (keysToRemove.Count == 0)
        {
            return;
        }

        await Task.WhenAll(keysToRemove.Select(async key =>
        {
            await _hybridCache.RemoveAsync(key, cancellationToken);
        }));

        outboxKeys.RemoveWhere(x => keysToRemove.Contains(x));
        await SetAsync(cacheStoreKey, outboxKeys, (int)TimeSpan.FromDays(365 * 20).TotalMinutes, cancellationToken);
    }

    private static bool IsNullOrDefault<T>(T? value)
    {
        if (value is null) { return true; }

        return !typeof(T).IsValueTupleType() ? EqualityComparer<T>.Default.Equals(value, default) :
            typeof(T).GetFields().All(field => IsNullOrDefault(field.GetValue(value)));
    }

    private async Task SyncCacheKeyOutbox(string key, bool shouldRemove = false, CancellationToken cancellationToken = default)
    {
        if (_isRedisCacheProvider) return;

        var outboxKeys = (await GetAsync<HashSet<string>>(cacheStoreKey, cancellationToken)) ?? [];

        if (!shouldRemove && !outboxKeys.Add(key))
        {
            return;
        }

        if (shouldRemove && !outboxKeys.Remove(key))
        {
            return;
        }

        await SetAsync(cacheStoreKey, outboxKeys, (int)TimeSpan.FromDays(365 * 20).TotalMinutes, cancellationToken);
    }
}

public static class ReflectionExtention
{
    public static bool IsValueTupleType(this Type type)
        =>  type.IsGenericType && type.FullName?.StartsWith("System.ValueTuple") == true;
}