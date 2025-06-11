namespace FastTodo.Infrastructure.Domain.Entities;

public class TrackedEntity
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ModifiedAt { get; set; } = DateTimeOffset.UtcNow;
}