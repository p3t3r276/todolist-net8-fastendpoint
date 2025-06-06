using FastTodo.Domain.Entities;
using FastTodo.Infrastructure.Domain.Repositories;
using Mapster;
using MediatR;

namespace FastTodo.Application.Features.Todo;

public class CreateTodoItemHandler(
    IRepository<TodoItem, Guid> repository
) : IRequestHandler<CreateTodoRequest, TodoItemDto>
{
    public async Task<TodoItemDto> Handle(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var newTodo = request.Adapt<TodoItem>();

        await repository.AddAsync(newTodo, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return newTodo.Adapt<TodoItemDto>();
    }
}