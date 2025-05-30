using FastTodo.Domain.Shared;
using System.Linq.Expressions;
using FastTodo.Infrastructure.Domain.Entities;

namespace FastTodo.Infrastructure.Repositories;

public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id, bool enableTracking = true, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetByIdsAsync(TKey[] ids, bool enableTracking = true, CancellationToken cancellationToken = default);

    Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, bool enableTracking = true, CancellationToken cancellationToken = default);

    Task<PaginatedList<TEntity>> ListAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, bool enableTracking = true, CancellationToken cancellationToken = default);

    Task<PaginatedList<TProjector>> ListAsync<TProjector>(int pageIndex, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, bool enableTracking = true, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
