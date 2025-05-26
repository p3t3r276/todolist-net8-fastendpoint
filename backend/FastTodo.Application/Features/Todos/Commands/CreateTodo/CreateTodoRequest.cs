using MediatR;

namespace FastTodo.Application.Features.Todos;

public class CreateTodoRequest : IRequest<TodoItemDto>
{
    public string? Name { get; set; }
}
