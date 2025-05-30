using MediatR;

namespace FastTodo.Application.Features.Todo;

public class CreateTodoRequest : IRequest<TodoItemDto>
{
    public string? Name { get; set; }
}
