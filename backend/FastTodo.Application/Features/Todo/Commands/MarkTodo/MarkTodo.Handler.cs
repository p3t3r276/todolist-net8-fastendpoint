using FastTodo.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using FastTodo.Infrastructure.Repositories;

namespace FastTodo.Application.Features.Todo;

public class MarkTodoHandler(
    IRepository<TodoItem, Guid> repository
) : IRequestHandler<MarkTodoRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public async Task<Results<NoContent, Ok<TodoItemDto>>> Handle(MarkTodoRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id!.Value, cancellationToken: cancellationToken);
        if (item is null)
        {
            return TypedResults.NoContent();
        }
        item.IsDone = !item.IsDone;
        await repository.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(item.Adapt<TodoItemDto>());
    }
}