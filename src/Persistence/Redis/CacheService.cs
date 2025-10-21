using FastTodo.Infrastructure.Domain;

namespace FastTodo.Persistence.Redis;

public class CacheService : ICacheService
{
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
