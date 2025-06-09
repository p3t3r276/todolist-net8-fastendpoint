using FastTodo.Infrastructure.Domain;
using FastTodo.Infrastructure.Domain.Entities;
using FastTodo.Infrastructure.Domain.Repositories;
using FastTodo.Infrastructure.Domain.Repositories.Builder;
using FastTodo.Infrastructure.Extensions;
using FastTodo.Infrastructure.Repositories.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace FastTodo.Infrastructure.Repositories;

public class EFUnitOfWork(BaseDbContext context, ILogger<EFUnitOfWork> logger) : IUnitOfWork
{
    public Task BeginTransactionAsync()
    {
        return context.Database.BeginTransactionAsync();
    }

    public Task CommitTransactionAsync()
    {
        return context.Database.CommitTransactionAsync();
    }

    public async Task SaveChangeAsync(CancellationToken cancellation = default)
    {
        try
        {
            var rowAffected = await context.SaveChangesAsync(cancellation);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            if (context.Database.CurrentTransaction is not null)
            {
                await context.Database.CurrentTransaction.RollbackAsync(cancellation);
            }
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        // TODO: Set actor
        var data = await context.AddAsync(entity);
        return data.Entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        // TODO: set actor
        await context.AddRangeAsync(entities);
        return entities;
    }

    public void Update<TEntity>(TEntity entity, Action<IEntitySetter<TEntity>>? setter = default) where TEntity : class
    {
        UpdateEntryValueAndState(entity, actor: null, setter);
    }

    public async Task UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        Action<IEntitySetter<TEntity>>? setter = default) where TEntity : class
    {
        var item = await context
            .Set<TEntity>()
            .FirstOrDefaultAsync(predicate);

        if (item is null)
        {
            string errorMsg = $"{typeof(TEntity).Name} - Update Error: Not Found ";

            logger.LogError("{ErrorMsg}", errorMsg);
        }

        UpdateEntryValueAndState(item!, actor: null, setter);
    }    

    public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        context.UpdateRange(entities);
    }

    public void Remove<TEntity>(TEntity entity) where TEntity : class
    {
        context.Remove(entity);
    }

    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        context.RemoveRange(entities);
    }

    public Task<int> ExecuteRawQuery(string sqlQuery, object? param = null)
    {
        SqlParameter[] sqlParameters = param.ToSqlParmeterArray();
        return param != null ? context.Database.ExecuteSqlRawAsync(sqlQuery, sqlParameters) : context.Database.ExecuteSqlRawAsync(sqlQuery);
    }

    private void UpdateEntryValueAndState<TEntity>(TEntity item,
        object? actor = null,
        Action<IEntitySetter<TEntity>>? setter = default) 
        where TEntity : class
    {
        var dbEntry = context.Entry(item);

        if (item is TrackedEntity trackedEntity)
        {
            var utcNow = DateTimeOffset.UtcNow;

            var modifiedUtcProperty = dbEntry.Property(nameof(TrackedEntity.ModifiedAt));
            modifiedUtcProperty.CurrentValue = utcNow;
            modifiedUtcProperty.IsModified = true;

            trackedEntity.ModifiedAt = utcNow;
            // TODO: Set actor here
        }

        if (setter is null)
        {
            context.Update(item);
            return;
        }

        var entitySetter = new EntitySetter<TEntity>(item);
        setter.Invoke(entitySetter);

        foreach (var updatedProperty in entitySetter.UpdateProperties)
        {
            dbEntry.Property(updatedProperty).IsModified = true;
        }

#if DEBUG
        logger.LogInformation("Update Value with new state - Type: {Type} - Updated Data: {Data}", typeof(TEntity).FullName, item.Serialize());
#endif
    }
}
