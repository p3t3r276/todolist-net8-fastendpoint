using System.Reflection;
using FastTodo.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;
using FastTodo.Persistence.SQLite.DbContexts.FastTodoDbContext.Configurations;
using Microsoft.Extensions.Configuration;
using FastTodo.Infrastructure.Domain.ValueConverion;

namespace FastTodo.Persistence.SQLite;

public class FastTodoSqliteDbContext (DbContextOptions<FastTodoSqliteDbContext> options, IConfiguration configuration) 
    : BaseDbContext(options)
{
    protected override Assembly ExecutingAssembly => typeof(FastTodoApplyFilterConfiguration).Assembly;

    protected override Func<Type, bool> RegisterConfigurationsPredicate =>
        type => type.Namespace == typeof(FastTodoApplyFilterConfiguration).Namespace;
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                    || p.PropertyType == typeof(DateTimeOffset?));

            foreach (var property in properties)
            {
                modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasConversion(new DateTimeOffsetToUtcDateTimeTicksConverter());
            }
        }
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite(configuration.GetConnectionString("Sqlite"));
    }
}
