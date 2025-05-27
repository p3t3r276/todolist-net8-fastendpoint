using MediatR;
using FastEndpoints;

namespace FastTodo.Application.Features.Todo;

public class GetMyTodosRequest : IRequest<List<TodoItemDto>>
{ 
    [QueryParam]
    public string? Search { get; set; }
}
