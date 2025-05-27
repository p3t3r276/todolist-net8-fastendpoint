using FastTodo.Domain.Entities;
using FastTodo.Persistence.SQLite;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FastTodo.Application.Features.Todo;

public class GetTodoByIdHandler (
    FastTodoSqliteDbContext dbContext
): IRequestHandler<GetTodoByIdRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public async Task<Results<NoContent, Ok<TodoItemDto>>> Handle(GetTodoByIdRequest request, CancellationToken cancellationToken)
    {
        var item = await dbContext.Set<TodoItem>()
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.Id, cancellationToken: cancellationToken);

        if (item is null)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok(new TodoItemDto()
        {
            Id = item.Id,
            Name = item.Name,
            IsDone = item.IsDone
        });
    }
}