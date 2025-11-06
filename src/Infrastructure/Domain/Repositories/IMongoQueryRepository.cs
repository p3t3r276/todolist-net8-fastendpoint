using System.Linq.Expressions;
using FastTodo.Domain.Shared;
using FastTodo.Infrastructure.Domain.Entities;
using FastTodo.Infrastructure.Domain.Repositories.Builder;

namespace FastTodo.Infrastructure.Domain.Repositories;

public interface IMongoQueryRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    Task<TEntity?> FindAsync(
        TKey id,
        bool enableNoTracking = true,
        CancellationToken cancellationToken = default);

    Task<PaginatedList<TEntity>> FindAllAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool enableNoTracking = true,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool isAscending = true,
        CancellationToken cancellationToken = default);

    Task<PaginatedList<TProjector>> FindAllAsync<TProjector>(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool enableNoTracking = true,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool isAscending = true,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool shouldIgnoreFilter = false,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate,
        bool shouldIgnoreFilter = false,
        CancellationToken cancellationToken = default);

    Task<bool> AllAsync(
        Expression<Func<TEntity, bool>>? predicate,
        bool shouldIgnoreFilter = false,
        CancellationToken cancellationToken = default);

    // <summary>
    /// return a MongoCollection Queryable.
    /// </summary>
    /// <param name="shouldIgnoreFilter"></param>
    /// <returns></returns>
    IQueryable<TEntity> AsQueryable(bool shouldIgnoreFilter = false, bool enableNoTracking = true);

    Task<TProperty> MinAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector);

    Task<TProperty> MaxAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector);

    Task<TProperty> MinAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector,
        Expression<Func<TEntity, bool>>? predicate,
        bool shouldIgnoreFilter = false,
        CancellationToken cancellationToken = default);

    Task<TProperty> MaxAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector,
        Expression<Func<TEntity, bool>>? predicate,
        bool shouldIgnoreFilter = false,
        CancellationToken cancellationToken = default);

    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool enableNoTracking = true,
        CancellationToken cancellationToken = default);

    string? CollectionName { get; }
}
