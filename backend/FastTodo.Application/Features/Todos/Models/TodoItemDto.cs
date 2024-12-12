namespace FastTodo.Application.Features.Todos;

public class TodoItemDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool IsDone { get; set; }
}