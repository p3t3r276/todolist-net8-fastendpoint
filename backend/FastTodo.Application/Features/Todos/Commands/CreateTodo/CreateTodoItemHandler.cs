using FastTodo.Domain.Entities;
using FastTodo.Persistence.SQLite;
using Mapster;
using MediatR;

namespace FastTodo.Application.Features.Todos;

public class CreateTodoItemHandler(
    FastTodoSqliteDbContext dbContext
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

        _ = await dbContext.Set<TodoItem>().AddAsync(newTodo, cancellationToken);
        _ = await dbContext.SaveChangesAsync(cancellationToken);
        return newTodo.Adapt<TodoItemDto>();
    }
}