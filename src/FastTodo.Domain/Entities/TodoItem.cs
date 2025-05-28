using FastTodo.Domain.Common;

namespace FastTodo.Domain.Entities;

public sealed class TodoItem : BaseEntity<Guid>
{
    public string? Name { get; set; }

    public bool IsDone { get; set; }
}
