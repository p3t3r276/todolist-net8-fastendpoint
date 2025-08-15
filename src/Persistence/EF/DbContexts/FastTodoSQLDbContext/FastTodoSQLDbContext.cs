using System.Reflection;
using FastTodo.Domain.Constants;
using FastTodo.Infrastructure.Domain;
using FastTodo.Persistence.EF.DbContexts.FastTodoDbContext.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FastTodo.Persistence.EF;

public class FastTodoSQLDbContext(
    DbContextOptions<FastTodoSQLDbContext> options, 
    IConfiguration configuration)
    : BaseDbContext(options)
{
    protected override Assembly ExecutingAssembly => typeof(FastTodoApplyFilterConfiguration).Assembly;

    protected override Func<Type, bool> RegisterConfigurationsPredicate =>
        type => type.Namespace == typeof(FastTodoApplyFilterConfiguration).Namespace;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString(nameof(DatabaseProviderType.SQLServer)),
            options => options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: System.TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                ));
    }
}
