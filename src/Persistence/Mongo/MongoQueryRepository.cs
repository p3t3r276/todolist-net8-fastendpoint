using System.Linq.Expressions;
using FastTodo.Domain.Shared;
using FastTodo.Infrastructure.Domain.Entities;
using FastTodo.Infrastructure.Domain.Repositories;
using FastTodo.Persistence.Mongo.DbContexts.MongoDbContext;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FastTodo.Persistence.Mongo;

public class MongoQueryRepository<TEntity, TKey>(MongoDbContext context)
    : IMongoQueryRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    protected readonly MongoDbContext _context = context;

    private readonly DbSet<TEntity> _collection = context.Set<TEntity>()
        ?? throw new NullReferenceException("Cannot find collection");

    protected readonly string? _collectionName = context.Model.FindEntityType(typeof(TEntity))?.GetCollectionName();

    public string? CollectionName => _collectionName;

    public async Task<TEntity?> FindAsync(
        TKey id,
        bool enableNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        return await _collection.FindAsync([id], cancellationToken: cancellationToken);
    }

    public async Task<PaginatedList<TEntity>> FindAllAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool enableNoTracking = true,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool isAscending = true,
        CancellationToken cancellationToken = default)
    {
        var source = Queryable(enableNoTracking);

        source = BindPredicate(source, predicate);

        if (orderBy != null)
        {
            source = isAscending ? source.OrderBy(orderBy) : source.OrderByDescending(orderBy);
        }

        var totalCount = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip((pageIndex < 0 ? 0 : pageIndex) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<TEntity>(items, totalCount, pageIndex, pageSize);
    }
    
    private static IQueryable<TEntity> BindPredicate(
        IQueryable<TEntity> source,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool shouldIgnoreFilter = false)
    {
        if (predicate is null)
        {
            return source;
        }

        if (shouldIgnoreFilter)
        {
            source = source.IgnoreQueryFilters();
        }

        return source.Where(predicate);
    }

    private IQueryable<TEntity> Queryable(bool isNoTracking = true)
    {
        return isNoTracking ? _collection.AsNoTracking() : _collection.AsQueryable();
    }
}
