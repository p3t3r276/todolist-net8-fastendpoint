using FastTodo.Domain.Entities;
using FastTodo.Persistence.SQLite;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FastTodo.Application.Features.Todo;

public class MarkTodoHandler(
    FastTodoSqliteDbContext dbContext
) : IRequestHandler<MarkTodoRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public async Task<Results<NoContent, Ok<TodoItemDto>>> Handle(MarkTodoRequest request, CancellationToken cancellationToken)
    {
        var item = await dbContext.Set<TodoItem>()
            .FirstOrDefaultAsync(item => item.Id == request.Id, cancellationToken: cancellationToken);

        if (item is null)
        {
            return TypedResults.NoContent();
        }

        item.IsDone = !item.IsDone;

        _ = await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(item.Adapt<TodoItemDto>());
    }
}