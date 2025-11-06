using FastTodo.Infrastructure.Domain.Entities;
using FastTodo.Infrastructure.Domain.Repositories;
using FastTodo.Persistence.Mongo.DbContexts.MongoDbContext;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FastTodo.Persistence.Mongo;

public class MongoUnitOfWork(
    ILogger<MongoUnitOfWork> logger,
    MongoDbContext context) : IMongoUnitOfWork
{
    protected readonly MongoDbContext _context = context;
    protected readonly ILogger<MongoUnitOfWork> _logger = logger;

    public async ValueTask<TEntity> AddAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        await _context.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async ValueTask<IEnumerable<TEntity>> AddRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        await _context.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        _context.Update(entity);
    }

    public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity
    {
        _context.UpdateRange(entities);
    }

    public void Remove<TEntity>(TEntity entity)  where TEntity : class, IEntity
    {
        _context.Remove(entity);
    }

    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity => _context.RemoveRange(entities);

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return _context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async ValueTask SaveChangeAsync(CancellationToken cancellationToken = default)
    {
        (int ErrorCode, string ErrorMsg) errorData;
        try
        {
            var rowAffectCount = await _context.SaveChangesAsync(cancellationToken);

            if (rowAffectCount > 0)
            {
                throw new Exception("No Record Save To Db");
            }
            errorData = (507, "No Record Save To Db");

            //logger.LogError(errorData.ErrorMsg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            if (_context.Database.CurrentTransaction is not null)
            {
                await _context.Database.RollbackTransactionAsync(cancellationToken);
            }

            throw new Exception(ex.Message, ex);
        }
    }

    // TODO: Check where to keep this method
    private async Task EnsureCreatedIndex<TEntity>()
    {
        var entityType = _context.Model.FindEntityType(typeof(TEntity));

        if (entityType == null) return;

        var collectionName = entityType.GetCollectionName();

        var collection = _context.MongoDatabase!.GetCollection<TEntity>(collectionName);

        if (collection == null) return;

        var indexes = await collection.Indexes.ListAsync();

        if (!indexes.MoveNext()) return;

        var existIndexName = indexes.Current.Select(x => x.Values.Last().AsString).ToList();

        var notExistIndex = entityType
            .GetIndexes()
            .Where(x => x.Properties.Count > 0
                        && !string.IsNullOrEmpty(x.Properties[0].Name)
                        && !existIndexName.Any(i => i.Contains(x.Properties[0].Name, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        foreach (var index in notExistIndex)
        {
            IndexKeysDefinition<TEntity> indexKeysDefinition = Builders<TEntity>
                .IndexKeys
                .Ascending(index.Properties
                                .FirstOrDefault()
                                !.Name);

            collection.Indexes.CreateOne(new MongoDB.Driver.CreateIndexModel<TEntity>(
                indexKeysDefinition,
                new CreateIndexOptions
                {
                    SphereIndexVersion = 2,
                    Unique = index.IsUnique,
                    TextIndexVersion = 2
                }));
        }
    }
}
