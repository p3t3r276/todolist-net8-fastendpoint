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
using FastTodo.Infrastructure.Domain;
using FastTodo.Domain.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace FastTodo.Application.Features.Todo;

public class GetMyTodosHandler (
    ILogger<GetMyTodosHandler> logger,
    IRepository<TodoItem, Guid> repository,
    UserManager<AppUser> userManager,
    ICacheService cacheService
) : IRequestHandler<GetMyTodosRequest, PaginatedList<TodoItemDto>>
{
    public async Task<PaginatedList<TodoItemDto>> Handle(GetMyTodosRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var items = await cacheService.GetOrSetAsync("TodoLIST",
                func: async () => await repository.ListAsync<TodoItemDto>(
                    request.PageIndex,
                    request.PageSize,
                    enableTracking: false,
                    cancellationToken: cancellationToken),
                CACHE_TIME_IN_MINUTES.EVERY_DAY,
                cancellationToken);

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
        catch(Exception ex)
        {
            logger.LogError(ex, "Handle-GetMyTodosHandler: {request}", request.Serialize());
            throw;
        }
    }
}
