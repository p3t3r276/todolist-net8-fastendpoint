namespace FastTodo.Infrastructure.Domain.Repositories;

public interface IUnitOfWork
{
    Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class;

    Task<IEnumerable<TEntity>> AddRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class;

    void Update<TEntity>(TEntity entity) where TEntity : class;

    void UpdateRange<TEntity>(
        IEnumerable<TEntity> entities) where TEntity : class;

    void Remove<TEntity>(TEntity entity) where TEntity : class;

    void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    Task BeginTransactionAsync();

    Task SaveChangeAsync(CancellationToken cancellation = default);
    
    Task CommitTransactionAsync();

    Task<int> ExecuteRawQuery(string sqlQuery, object? param= null);
}
