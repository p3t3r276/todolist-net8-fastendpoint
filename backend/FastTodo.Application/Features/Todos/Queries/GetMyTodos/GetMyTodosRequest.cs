using MediatR;

namespace FastTodo.Application.Features.Todos;

public class GetMyTodosRequest : IRequest<List<TodoItemDto>>
{
    
}