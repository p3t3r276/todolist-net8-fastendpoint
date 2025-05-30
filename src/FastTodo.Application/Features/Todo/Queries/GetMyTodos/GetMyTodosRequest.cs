using MediatR;
using FastEndpoints;
using FastTodo.Domain.Shared;

namespace FastTodo.Application.Features.Todo;

public class GetMyTodosRequest : IRequest<PaginatedList<TodoItemDto>>, ICollectionRequest
{
    [QueryParam]
    public int PageIndex { get; set; } = 1;

    [QueryParam]
    public int PageSize { get; set; } = 10;

    [QueryParam]
    public string? Search { get; set; }
}
