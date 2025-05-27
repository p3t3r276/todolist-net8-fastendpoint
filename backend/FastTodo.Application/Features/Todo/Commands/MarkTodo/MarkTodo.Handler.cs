using FastTodo.Domain.Entities;
using FastTodo.Persistence.SQLite;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using FastTodo.Infrastructure.Repositories;

namespace FastTodo.Application.Features.Todo;

public class MarkTodoHandler(
    IRepository<TodoItem> repository
) : IRequestHandler<MarkTodoRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public async Task<Results<NoContent, Ok<TodoItemDto>>> Handle(MarkTodoRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id!, cancellationToken);
        if (item is null)
        {
            return TypedResults.NoContent();
        }
        item.IsDone = !item.IsDone;
        await repository.UpdateAsync(item, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(item.Adapt<TodoItemDto>());
    }
}