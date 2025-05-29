namespace FastTodo.Infrastructure.Domain.Entities;

public abstract class BaseEntity<TKey> : IEntity<TKey>
{
    public TKey Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ModifiedAt { get; set; } = DateTimeOffset.UtcNow;
}