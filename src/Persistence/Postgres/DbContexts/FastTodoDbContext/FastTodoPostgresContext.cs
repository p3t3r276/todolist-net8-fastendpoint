using FastTodo.Persistence.Postgres.DbContexts.FastTodoDbContext.Configurations;
using FastTodo.Domain.Constants;
using FastTodo.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FastTodo.Persistence.Postgres;

public class FastTodoPostgresContext(DbContextOptions<FastTodoPostgresContext> options, IConfiguration configuration) : BaseDbContext(options)
{
    protected override Assembly ExecutingAssembly => typeof(FastTodoApplyFilterConfiguration).Assembly;

    protected override Func<Type, bool> RegisterConfigurationsPredicate =>
        type => type.Namespace == typeof(FastTodoApplyFilterConfiguration).Namespace;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNpgsql(configuration.GetConnectionString(nameof(ConnectionStrings.Default)));
    }
}
