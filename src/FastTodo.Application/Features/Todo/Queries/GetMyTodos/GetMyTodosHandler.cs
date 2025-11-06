using FastTodo.Domain.Entities;
using MediatR;
using FastTodo.Domain.Shared;
using FastTodo.Infrastructure.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using FastTodo.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using FastTodo.Application.Features.Identity;
using Mapster;
using FastTodo.Domain.Entities.Mongo;
using MongoDB.Bson;

namespace FastTodo.Application.Features.Todo;

public class GetMyTodosHandler(
    IRepository<TodoItem, Guid> repository,
    IMongoQueryRepository<TodoItemSchema, ObjectId> mongoRepository,
    UserManager<AppUser> userManager
) : IRequestHandler<GetMyTodosRequest, PaginatedList<TodoItemDto>>
{
    public async Task<PaginatedList<TodoItemDto>> Handle(
        GetMyTodosRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tempitems = await mongoRepository.FindAllAsync(
            request.PageIndex,
            request.PageSize,
            predicate: null,
            enableNoTracking: true,
            // orderBy: qb => qb.OrderByDescending(ti => ti.CreatedAt),
            cancellationToken: cancellationToken);

        var items = tempitems.Adapt<PaginatedList<TodoItemDto>>();

        if (items.Data.Count is 0)
        {
            return items;
        }

        var todoItems = items.Data;

        var userList = todoItems.SelectMany(u => new[] { u.CreatedBy, u.ModifiedBy }).ToList();

        var users = await userManager.Users
            .Where(u => userList.Contains(u.Id))
            .ToListAsync(cancellationToken: cancellationToken);

        if (users.Count is 0)
        {
            return items;
        }

        var userDict = users.ToImmutableDictionary(u => u.Id,
            u => u.Adapt<UserResponse>());

        todoItems.ForEach(entity =>
        {
            entity.CreatedByUser = userDict.TryGetValue(entity.CreatedBy, out var createdByUser) ? createdByUser : null;
            entity.ModifiedByUser = userDict.TryGetValue(entity.ModifiedBy, out var modifiedByUser) ? modifiedByUser : null;
        });

        return items;
    }
}
