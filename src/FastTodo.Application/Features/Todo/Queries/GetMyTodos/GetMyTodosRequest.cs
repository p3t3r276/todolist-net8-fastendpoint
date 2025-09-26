using System.ComponentModel;
using MediatR;
using FastEndpoints;
using FastTodo.Domain.Shared;

namespace FastTodo.Application.Features.Todo;

public class GetMyTodosRequest : IRequest<PaginatedList<TodoItemDto>>, ICollectionRequest
{
    [QueryParam]
    [DefaultValue(1)]
    public int PageIndex { get; set; } = 0;

    [QueryParam]
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;

    [QueryParam]
    public string? Search { get; set; }
}
