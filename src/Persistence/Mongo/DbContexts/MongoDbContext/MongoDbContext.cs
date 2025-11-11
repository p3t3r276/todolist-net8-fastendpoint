using FastTodo.Infrastructure.Domain;
using FastTodo.Persistence.Mongo.DbContexts.MongoDbContext.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Reflection;

namespace FastTodo.Persistence.Mongo.DbContexts.MongoDbContext;

public class MongoDbContext : BaseDbContext
{
    public virtual IMongoDatabase? MongoDatabase { get; internal set; }

    public virtual IMongoClient? MongoClient { get; internal set; }

    protected override Assembly ExecutingAssembly => typeof(FastTodoApplyFilterConfiguration).Assembly;

    protected override Func<Type, bool> RegisterConfigurationsPredicate =>
        type => type.Namespace == typeof(FastTodoApplyFilterConfiguration).Namespace;

    public MongoDbContext(DbContextOptions<MongoDbContext> options, IConfiguration configuration) : base(options)
    {
        MongoOptionsExtension? mongoOptionsExtension = options.Extensions.OfType<MongoOptionsExtension>().FirstOrDefault();
        if (mongoOptionsExtension is not null)
        {
            MongoClient = mongoOptionsExtension.MongoClient;
            MongoDatabase = mongoOptionsExtension.MongoClient?.GetDatabase(mongoOptionsExtension.DatabaseName);
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }
}
