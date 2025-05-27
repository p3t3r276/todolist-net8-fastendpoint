using FastTodo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using FastTodo.Infrastructure.Repositories;

namespace FastTodo.Application.Features.Todo;

public class GetTodoByIdHandler (
    IRepository<TodoItem> repository
): IRequestHandler<GetTodoByIdRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public async Task<Results<NoContent, Ok<TodoItemDto>>> Handle(GetTodoByIdRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id!, cancellationToken);
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