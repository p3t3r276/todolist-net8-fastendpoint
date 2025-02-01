using MediatR;
using FastEndpoints;

namespace FastTodo.Application.Features.Todos;

public class GetMyTodosRequest : IRequest<List<TodoItemDto>>
{ 
    [QueryParam]  // Mark the property as a query parameter
    public string? SearchTerm { get; set; } // Simple query parameter
    
    [QueryParam]
    public string? Name { get; set; }
}
