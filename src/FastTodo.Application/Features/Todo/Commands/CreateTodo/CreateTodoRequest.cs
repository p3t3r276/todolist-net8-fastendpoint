using MediatR;

namespace FastTodo.Application.Features.Todo;

public sealed class CreateTodoRequest : IRequest<TodoItemDto>
{
    public string? Name { get; set; }
    
    public DateTimeOffset? DueDate { get; set; }
}
