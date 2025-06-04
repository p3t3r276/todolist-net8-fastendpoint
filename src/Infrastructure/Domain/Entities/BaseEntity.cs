namespace FastTodo.Infrastructure.Domain.Entities;

public abstract class BaseEntity<TKey> : TrackedEntity, IEntity<TKey>
{
    public TKey Id { get; set; } = default!;
}