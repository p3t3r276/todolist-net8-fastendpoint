using FastTodo.Domain.Entities;
using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Application.Features.Todo;

public class UpdateTodoHandler(
    IRepository<TodoItem, Guid> repository,
    [FromKeyedServices(ServiceKeys.FastTodoEFUnitOfWork)]
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateTodoRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public async Task<Results<NoContent, Ok<TodoItemDto>>> Handle(UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id!.Value, cancellationToken: cancellationToken);
        if (item is null)
        {
            return TypedResults.NoContent();
        }
        item.Name = request.Body!.Name;
        unitOfWork.Update(item);
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return TypedResults.Ok(item.Adapt<TodoItemDto>());
    }
}
