using FastTodo.Infrastructure.Domain.Entities;

namespace FastTodo.Infrastructure.Domain.Repositories;

public interface IMongoUnitOfWork
{
    ValueTask<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

    ValueTask<IEnumerable<TEntity>> AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

    void Update<TEntity>(TEntity entity) where TEntity : class, IEntity;

    void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity;

    void Remove<TEntity>(TEntity entity) where TEntity : class, IEntity;

    void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity;

    ValueTask SaveChangeAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);
}