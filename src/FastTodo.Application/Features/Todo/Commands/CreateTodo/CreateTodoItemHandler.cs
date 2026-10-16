using FastTodo.Domain.Entities;
using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Application.Features.Todo;

public class CreateTodoItemHandler(
    [FromKeyedServices(ServiceKeys.FastTodoEFUnitOfWork)]
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateTodoRequest, TodoItemDto>
{
    public async Task<TodoItemDto> Handle(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var newTodo = request.Adapt<TodoItem>();

        await unitOfWork.AddAsync(newTodo);
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return newTodo.Adapt<TodoItemDto>();
    }
}
