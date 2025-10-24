using FastTodo.Domain.Shared;
using System.Linq.Expressions;
using FastTodo.Infrastructure.Domain.Entities;
using FastTodo.Infrastructure.Domain.Repositories.Builder;

namespace FastTodo.Infrastructure.Domain.Repositories;

public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id, bool enableTracking = true, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetByIdsAsync(TKey[] ids, bool enableTracking = true, CancellationToken cancellationToken = default);

    Task<List<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Action<IQueryBuilder<TEntity>>? querybuilder = default,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    Task<PaginatedList<TEntity>> ListAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Action<IQueryBuilder<TEntity>>? querybuilder = default,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    Task<PaginatedList<TProjector>> ListAsync<TProjector>(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Action<IQueryBuilder<TEntity>>? querybuilder = default,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);
}
