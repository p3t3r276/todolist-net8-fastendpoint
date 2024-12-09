using System.Reflection;
using FastTodo.Infrastructure;
using Microsoft.EntityFrameworkCore;
using FastTodo.SQLite.DbContexts.FastTodoDbContext.Configurations;
using Microsoft.Extensions.Configuration;

namespace FastTodo.SQLite.DbContexts.FastTodoDbContext;

public class FastTodoSqliteDbContext (DbContextOptions<FastTodoSqliteDbContext> options, IConfiguration configuration) 
    : BaseDbContext<FastTodoSqliteDbContext>(options)
{
    protected override Assembly ExecutingAssembly => typeof(FastTodoApplyFilterConfiguration).Assembly;

    protected override Func<Type, bool> RegisterConfigurationsPredicate =>
        type => type.Namespace == typeof(FastTodoApplyFilterConfiguration).Namespace;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(configuration.GetConnectionString("Sqlite"));
        }
    }
}
