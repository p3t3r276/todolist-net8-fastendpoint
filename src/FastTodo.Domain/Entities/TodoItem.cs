using FastTodo.Infrastructure.Domain.Entities;

namespace FastTodo.Domain.Entities;

public sealed class TodoItem : BaseEntity<Guid>
{
    public string? Name { get; set; }

    public bool IsDone { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }
}
