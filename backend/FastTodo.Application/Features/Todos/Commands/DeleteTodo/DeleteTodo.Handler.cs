using FastTodo.Domain.Entities;
using FastTodo.Persistence.SQLite;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FastTodo.Application.Features.Todos;

public class DeleteTodoHandler(
    FastTodoSqliteDbContext dbContext
) : IRequestHandler<DeleteTodoRequest, Results<NoContent, Ok>>
{
    public async Task<Results<NoContent, Ok>> Handle(DeleteTodoRequest request, CancellationToken cancellationToken)
    {
        var item = await dbContext.Set<TodoItem>()
            .FirstOrDefaultAsync(item => item.Id == request.Id, cancellationToken: cancellationToken);

        if (item is null)
        {
            return TypedResults.NoContent();
        }

        dbContext.Entry(item).State = EntityState.Deleted;
        _ = await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}