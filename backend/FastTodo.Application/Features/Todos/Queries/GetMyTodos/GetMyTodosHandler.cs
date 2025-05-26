using FastTodo.Domain.Entities;
using FastTodo.Persistence.SQLite;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastTodo.Application.Features.Todos;

public class GetMyTodosHandler (
        FastTodoSqliteDbContext dbContext
    ): IRequestHandler<GetMyTodosRequest, List<TodoItemDto>>
{
    public async Task<List<TodoItemDto>> Handle(GetMyTodosRequest request, CancellationToken cancellationToken)
    {
        return await dbContext.Set<TodoItem>().AsNoTracking()
            .SelectMany(item => item.Adapt<IQueryable<TodoItemDto>>())
            .ToListAsync();
    }
}