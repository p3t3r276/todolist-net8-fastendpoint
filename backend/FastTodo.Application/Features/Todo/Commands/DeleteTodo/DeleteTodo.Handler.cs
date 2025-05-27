using FastTodo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using FastTodo.Infrastructure.Repositories;

namespace FastTodo.Application.Features.Todo;

public class DeleteTodoHandler(
    IRepository<TodoItem> repository
) : IRequestHandler<DeleteTodoRequest, Results<NoContent, Ok>>
{
    public async Task<Results<NoContent, Ok>> Handle(DeleteTodoRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id!, cancellationToken);
        if (item is null)
        {
            return TypedResults.NoContent();
        }
        await repository.DeleteAsync(item, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}