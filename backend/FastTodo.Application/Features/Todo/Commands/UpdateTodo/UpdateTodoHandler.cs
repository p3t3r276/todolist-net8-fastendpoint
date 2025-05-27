using FastTodo.Domain.Entities;
using FastTodo.Infrastructure.Repositories;
using FastTodo.Persistence.SQLite;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FastTodo.Application.Features.Todo;

public class UpdateTodoHandler(
    IRepository<TodoItem> repository
) : IRequestHandler<UpdateTodoRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public async Task<Results<NoContent, Ok<TodoItemDto>>> Handle(UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id!, cancellationToken);
        if (item is null)
        {
            return TypedResults.NoContent();
        }
        item.Name = request.Name;
        await repository.UpdateAsync(item, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(item.Adapt<TodoItemDto>());
    }
}