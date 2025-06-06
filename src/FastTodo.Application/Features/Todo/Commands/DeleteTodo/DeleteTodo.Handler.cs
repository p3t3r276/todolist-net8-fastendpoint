using FastTodo.Domain.Entities;
using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Application.Features.Todo;

public class DeleteTodoHandler(
    IRepository<TodoItem, Guid> repository,
    [FromKeyedServices(ServiceKeys.FastTodoEFUnitOfWork)]
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteTodoRequest, Results<NoContent, Ok>>
{
    public async Task<Results<NoContent, Ok>> Handle(DeleteTodoRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id!.Value, cancellationToken: cancellationToken);
        if (item is null)
        {
            return TypedResults.NoContent();
        }
        unitOfWork.Remove(item);
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return TypedResults.Ok();
    }
}