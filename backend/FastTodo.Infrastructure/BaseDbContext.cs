using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace FastTodo.Infrastructure;

public abstract class BaseDbContext<TDbContext> : DbContext where TDbContext : DbContext
{
    protected virtual Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();

    protected abstract Func<Type, bool> RegisterConfigurationsPredicate { get; }

    protected BaseDbContext(DbContextOptions<TDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(ExecutingAssembly, RegisterConfigurationsPredicate);
    }
}
