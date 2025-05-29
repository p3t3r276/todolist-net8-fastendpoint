using FastTodo.Domain.Shared;
using FastTodo.Infrastructure.Domain.Entities;
using FastTodo.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FastTodo.Infrastructure.Repositories;

public class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey> 
    where TEntity : class, IEntity<TKey>
{
    private readonly BaseDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public EfRepository(BaseDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        if (!enableTracking)
            query = query.AsNoTracking();

        var keyProperty = typeof(TEntity).GetProperty("Id");
        if (keyProperty == null)
            throw new InvalidOperationException($"No 'Id' property found on type {typeof(TEntity).Name}");

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, keyProperty);
        var constant = Expression.Convert(Expression.Constant(id), keyProperty.PropertyType);
        var body = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await query.FirstOrDefaultAsync(lambda, cancellationToken);
    }

    public async Task<List<TEntity>> GetByIdsAsync(TKey[] ids, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        if (!enableTracking)
            query = query.AsNoTracking();

        var keyProperty = typeof(TEntity).GetProperty("Id");
        if (keyProperty == null)
            throw new InvalidOperationException($"No 'Id' property found on type {typeof(TEntity).Name}");

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, keyProperty);
        var constant = Expression.Constant(ids);
        var body = Expression.Call(typeof(Enumerable), "Contains", new[] { keyProperty.PropertyType }, constant, property);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await query.Where(lambda).ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        if (!enableTracking)
            query = query.AsNoTracking();
        if (predicate != null)
            query = query.Where(predicate);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<TEntity>> ListAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        if (!enableTracking)
            query = query.AsNoTracking();
        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<TEntity>(items, totalCount, pageIndex, pageSize);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
