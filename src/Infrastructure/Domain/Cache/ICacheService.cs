using FastTodo.Domain.Shared.Constants;

namespace FastTodo.Infrastructure.Domain;

public interface ICacheService
{
    /// <summary>
    /// Retrieves a cached value by key with automatic JSON deserialization.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the cached data to.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached value or default if not found.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all cached values from a Redis hash by group key.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the cached data to.</typeparam>
    /// <param name="key">The hash group key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary of all cached values in the hash.</returns>
    Task<Dictionary<string, T?>?> GetAllAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a value in the cache with automatic JSON serialization.
    /// </summary>
    /// <typeparam name="T">The type of data to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="data">The data to cache.</param>
    /// <param name="cacheTimeInMinutes">Cache expiration time in minutes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync<T>(string key, T data, int cacheTimeInMinutes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a cached value by key and updates the cache key outbox.
    /// </summary>
    /// <param name="key">The cache key to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a range of cached values based on a key pattern.
    /// Note: Causes performance issues; avoid using the "Contains" operator if your cache provider is Redis.
    /// </summary>
    /// <param name="keyPattern">The key pattern to match. Do not include wildcards in the keyword.</param>
    /// <param name="searchOperator">The search operator to use (e.g., StartsWith, EndsWith, Contains).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveRangeAsync(string keyPattern, CacheKeySearchOperator searchOperator = CacheKeySearchOperator.StartsWith, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to retrieve a cached value without throwing exceptions on cache miss.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the cached data to.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple indicating success and the cached data if found.</returns>
    Task<(bool, T? cacheData)> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Implements the cache-aside pattern: retrieves from cache or executes the function and caches the result.
    /// </summary>
    /// <typeparam name="T">The type of data to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="func">The function to execute if cache miss occurs.</param>
    /// <param name="cacheTimeInMinutes">Cache expiration time in minutes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached or newly computed value.</returns>
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> func, int cacheTimeInMinutes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores multiple key-value pairs in a Redis hash for bulk operations.
    /// </summary>
    /// <typeparam name="TRedisDto">The type of data to store in the hash.</typeparam>
    /// <param name="group">The hash group key.</param>
    /// <param name="fields">Dictionary of field names and values to store.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation succeeds.</returns>
    Task<bool> SetBulkAsync<TRedisDto>(string group, IDictionary<string, TRedisDto> fields, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a composite cache key from multiple key segments.
    /// </summary>
    /// <param name="keys">The key segments to combine.</param>
    /// <returns>A composite cache key.</returns>
    string GenerateKey(params string[] keys);
}
