using FastTodo.Domain.Entities;
using FastTodo.Persistence.SQLite;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FastTodo.Infrastructure.Repositories;

namespace FastTodo.Application.Features.Todo;

public class GetMyTodosHandler (
    IRepository<TodoItem> repository
): IRequestHandler<GetMyTodosRequest, List<TodoItemDto>>
{
    public async Task<List<TodoItemDto>> Handle(GetMyTodosRequest request, CancellationToken cancellationToken)
    {
        var items = await repository.ListAsync(null, cancellationToken);
        return items.Select(x => x.Adapt<TodoItemDto>()).ToList();
    }
}