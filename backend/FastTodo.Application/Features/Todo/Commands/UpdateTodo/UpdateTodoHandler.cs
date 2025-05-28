using FastTodo.Domain.Entities;
using FastTodo.Infrastructure.Repositories;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.Application.Features.Todo;

public class UpdateTodoHandler(
    IRepository<TodoItem, Guid> repository
) : IRequestHandler<UpdateTodoRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public async Task<Results<NoContent, Ok<TodoItemDto>>> Handle(UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id!.Value, cancellationToken: cancellationToken);
        if (item is null)
        {
            return TypedResults.NoContent();
        }
        item.Name = request.Name;
        await repository.UpdateAsync(item, cancellationToken);
        return TypedResults.Ok(item.Adapt<TodoItemDto>());
    }
}