using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace FastTodo.Infrastructure.Domain;

public abstract class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    protected virtual Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();

    protected abstract Func<Type, bool> RegisterConfigurationsPredicate { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(ExecutingAssembly, RegisterConfigurationsPredicate);
    }
}
