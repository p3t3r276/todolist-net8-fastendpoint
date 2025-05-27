using FastTodo.Application.Features.Todo;
using FastTodo.Domain.Entities;
using FastTodo.Infrastructure.Repositories;
using Mapster;
using MediatR;

public class CreateTodoItemHandler(
    IRepository<TodoItem> repository
) : IRequestHandler<CreateTodoRequest, TodoItemDto>
{
    public async Task<TodoItemDto> Handle(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var newTodo = new TodoItem()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsDone = false
        };

        await repository.AddAsync(newTodo, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return newTodo.Adapt<TodoItemDto>();
    }
}