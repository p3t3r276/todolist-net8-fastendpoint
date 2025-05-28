using FastTodo.Domain.Entities;
using Mapster;
using MediatR;
using FastTodo.Infrastructure.Repositories;

namespace FastTodo.Application.Features.Todo;

public class GetMyTodosHandler (
    IRepository<TodoItem, Guid> repository
): IRequestHandler<GetMyTodosRequest, List<TodoItemDto>>
{
    public async Task<List<TodoItemDto>> Handle(GetMyTodosRequest request, CancellationToken cancellationToken)
    {
        var items = await repository.ListAsync(predicate: null, enableTracking: false, cancellationToken: cancellationToken);
        return items.Select(x => x.Adapt<TodoItemDto>()).ToList();
    }
}