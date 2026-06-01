using FastTodo.Infrastructure.Domain.Entities;
using FastTodo.Infrastructure.Domain.Repositories.Builder;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace FastTodo.Infrastructure.Domain.Repositories;

public interface IUnitOfWork
{
    Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class;

    Task<IEnumerable<TEntity>> AddRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class, IEntity;

    void Update<TEntity>(TEntity entit, Action<IEntitySetter<TEntity>>? setter = default) where TEntity : class, IEntity;

    Task UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        Action<IEntitySetter<TEntity>>? setter = default) where TEntity : class, IEntity;

    void UpdateRange<TEntity>(
        IEnumerable<TEntity> entities) where TEntity : class;

    EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;

    void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    Task BeginTransactionAsync();

    Task SaveChangeAsync(CancellationToken cancellation = default);
    
    Task CommitTransactionAsync();

    Task<int> ExecuteRawQuery(string sqlQuery, object? param= null);
}
