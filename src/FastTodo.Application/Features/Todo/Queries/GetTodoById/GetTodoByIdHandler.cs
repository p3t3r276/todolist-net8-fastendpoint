using FastTodo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Mapster;
using FastTodo.Infrastructure.Domain.Repositories;

namespace FastTodo.Application.Features.Todo;

public class GetTodoByIdHandler (
    IRepository<TodoItem, Guid> repository
): IRequestHandler<GetTodoByIdRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public async Task<Results<NoContent, Ok<TodoItemDto>>> Handle(
        GetTodoByIdRequest request,
        CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(
            request.Id!.Value,
            enableTracking: false,
            cancellationToken: cancellationToken);
        if (item is null)
        {
            return TypedResults.NoContent();
        }
        return TypedResults.Ok(item.Adapt<TodoItemDto>());
    }
}