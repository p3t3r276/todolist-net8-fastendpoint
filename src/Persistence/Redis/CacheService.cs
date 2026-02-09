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

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var cacheData = await _distributedCache.GetStringAsync(key, token: cancellationToken);

            if (string.IsNullOrEmpty(cacheData)) { return default; }

            return JsonSerializer.Deserialize<T?>(cacheData);
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
            var value = await GetAsync<T>(key, cancellationToken);

            if (!IsNullOrDefault(value))
            {
                return value;
            }

            value = await func();

            if (!IsNullOrDefault(value))
            {
                await SetAsync(key, value, cacheTimeInMinutes, cancellationToken);
            }

            return value;
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
            var serializedData = data.Serialize();

            await _distributedCache.SetStringAsync(key, serializedData, GetTimeOutOption(cacheTimeInMinutes), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetAsync-CacheService: {Key}-{Data}-{CacheTimeInMinutes}", key, data.Serialize(), cacheTimeInMinutes);
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
                _distributedCache.RemoveAsync(key, cancellationToken));
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

        var server = _connectionMultiplexer!.GetServer(_connectionMultiplexer!.GetEndPoints().First());

        var searchPattern = searchOperator switch
        {
            CacheKeySearchOperator.StartsWith => $"{keyPattern}*",
            CacheKeySearchOperator.EndsWith => $"*{keyPattern}",
            CacheKeySearchOperator.Contains => $"*{keyPattern}*",
            _ => throw new ArgumentOutOfRangeException(nameof(searchOperator), searchOperator, null)
        };

        var keys = server.Keys(_database!.Database, searchPattern);

        if (!keys.Any())
        {
            return;
        }

        await _database!.KeyDeleteAsync([.. keys]);
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
            await _distributedCache.RemoveAsync(key);
        }));

        outboxKeys.RemoveWhere(x => keysToRemove.Contains(x));
        await _distributedCache.SetStringAsync(cacheStoreKey, outboxKeys.Serialize(), GetOutBoxCacheTimeOut(), cancellationToken);
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

        await _distributedCache.SetStringAsync(cacheStoreKey, outboxKeys.Serialize(), GetOutBoxCacheTimeOut(), cancellationToken);
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
