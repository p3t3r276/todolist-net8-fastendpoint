using FastTodo.Domain.Shared;
using FastTodo.Infrastructure.Domain.Entities;
using FastTodo.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Mapster;
using FastTodo.Infrastructure.Domain.Repositories;
using FastTodo.Infrastructure.Repositories.Builder;
using FastTodo.Infrastructure.Domain.Repositories.Builder;

namespace FastTodo.Infrastructure.Repositories;

public class EfRepository<TEntity, TKey>(BaseDbContext dbContext) : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(TKey id, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        if (!enableTracking)
            query = query.AsNoTracking();

        var keyProperty = typeof(TEntity).GetProperty("Id") ?? throw new InvalidOperationException($"No 'Id' property found on type {typeof(TEntity).Name}");
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, keyProperty);
        var constant = Expression.Convert(Expression.Constant(id), keyProperty.PropertyType);
        var body = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await query.FirstOrDefaultAsync(lambda, cancellationToken);
    }

    public async Task<List<TEntity>> GetByIdsAsync(
        TKey[] ids,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        if (!enableTracking)
            query = query.AsNoTracking();

        var keyProperty = typeof(TEntity).GetProperty("Id") ?? throw new InvalidOperationException($"No 'Id' property found on type {typeof(TEntity).Name}");
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, keyProperty);
        var constant = Expression.Constant(ids);
        var body = Expression.Call(typeof(Enumerable), "Contains", [keyProperty.PropertyType], constant, property);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await query.Where(lambda).ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Action<IQueryBuilder<TEntity>>? querybuilder = default,
        bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        var builder = InvokeQueryBuilder(querybuilder, enableTracking);

        var query = predicate is null ? builder.Query : builder.Query.Where(predicate);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<TEntity>> ListAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Action<IQueryBuilder<TEntity>>? querybuilder = default,
        bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        var builder = InvokeQueryBuilder(querybuilder, enableTracking);

        var query = predicate is null ? builder.Query : builder.Query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageIndex < 0 ? 0 : pageIndex) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<TEntity>(items, totalCount, pageIndex, pageSize);
    }

    public async Task<PaginatedList<TProjector>> ListAsync<TProjector>(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Action<IQueryBuilder<TEntity>>? querybuilder = default,
        bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        var builder = InvokeQueryBuilder(querybuilder, enableTracking);

        var query = predicate is null ? builder.Query : builder.Query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageIndex < 0 ? 0 : pageIndex) * pageSize)
            .Take(pageSize)
            .ProjectToType<TProjector>()
            .ToListAsync(cancellationToken);

        return new PaginatedList<TProjector>(items, totalCount, pageIndex, pageSize);
    }

    private QueryBuilder<TEntity> InvokeQueryBuilder(Action<IQueryBuilder<TEntity>>? querybuilder, bool enableNoTracking = true)
    {
        IQueryable<TEntity> query = enableNoTracking ? _dbSet.AsNoTracking() : _dbSet;
        var qBuilder = new QueryBuilder<TEntity>(query);
        querybuilder?.Invoke(qBuilder);
        
        return qBuilder;
    }
}
