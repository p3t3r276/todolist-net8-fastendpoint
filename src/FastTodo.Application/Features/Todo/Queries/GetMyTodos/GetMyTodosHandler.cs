using FastTodo.Domain.Entities;
using MediatR;
using FastTodo.Infrastructure.Repositories;
using FastTodo.Domain.Shared;

namespace FastTodo.Application.Features.Todo;

public class GetMyTodosHandler (
    IRepository<TodoItem, Guid> repository
): IRequestHandler<GetMyTodosRequest, PaginatedList<TodoItemDto>>
{
    public async Task<PaginatedList<TodoItemDto>> Handle(GetMyTodosRequest request, CancellationToken cancellationToken)
    {
        var items = await repository.ListAsync<TodoItemDto>(
            request.PageIndex,
            request.PageSize,
            enableTracking: false,
            cancellationToken: cancellationToken);

        return items;
    }
}