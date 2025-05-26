namespace FastTodo.Domain.Entities;

public sealed class TodoItem
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public bool IsDone { get; set; }
}
