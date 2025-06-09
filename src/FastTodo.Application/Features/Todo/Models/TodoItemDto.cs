namespace FastTodo.Application.Features.Todo;

public class TodoItemDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public bool IsDone { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

}
