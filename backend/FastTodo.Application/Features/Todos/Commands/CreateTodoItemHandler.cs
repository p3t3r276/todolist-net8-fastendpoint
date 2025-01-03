using FastTodo.Domain.Entities;
using FastTodo.Persistence.SQLite.DbContexts.FastTodoDbContext;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastTodo.Application.Features.Todos;

public class CreateTodoItemHandler(
    FastTodoSqliteDbContext dbContext
    ) : IRequestHandler<CreateTodoRequest, TodoItemDto>
{
    public async Task<TodoItemDto> Handle(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var newTodo = (new TodoItem()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsDone = false
        });
        
        // collection
        DbSet<TodoItem> collection = dbContext.Set<TodoItem>();
        await collection.AddAsync(newTodo);
        await dbContext.SaveChangesAsync();
        return newTodo.Adapt<TodoItemDto>();
    }
}