using FastTodo.Infrastructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FastTodo.Infrastructure.Domain;

public class AuditingInterceptor : SaveChangesInterceptor
{
    private readonly IUserContext _currentUser;
    private readonly TimeProvider _timeProvider;

    public AuditingInterceptor(IUserContext currentUser, TimeProvider timeProvider)
    {
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        //UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        //UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null) return;

        string userId = _currentUser.UserId;
        var now = _timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries<TrackedEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
                entry.Entity.ModifiedAt = now;
                entry.Entity.ModifiedBy = userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                // Ensure CreatedAt and CreatedBy are not accidentally changed
                entry.Property(nameof(TrackedEntity.CreatedAt)).IsModified = false;
                entry.Property(nameof(TrackedEntity.CreatedBy)).IsModified = false;

                entry.Entity.ModifiedAt = now;
                entry.Entity.ModifiedBy = userId;
            }
        }
    }
}
