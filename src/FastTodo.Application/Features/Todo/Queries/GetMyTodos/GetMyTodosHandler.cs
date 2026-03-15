using MediatR;
using FastTodo.Domain.Shared;
using FastTodo.Infrastructure.Domain.Repositories;
using System.Collections.Immutable;
using FastTodo.Application.Features.Identity;
using FastTodo.Domain.Entities.Mongo;
using MongoDB.Bson;
using FastTodo.Infrastructure.Domain;
using FastTodo.Application.Features.Identity.Services;
using FastTodo.Domain.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace FastTodo.Application.Features.Todo;

public class GetMyTodosHandler(
    ILogger<GetMyTodosHandler> logger,
    IMongoQueryRepository<TodoItemSchema, ObjectId> mongoRepository,
    IUserContext userContext,
    IUserService userService
    ) : IRequestHandler<GetMyTodosRequest, PaginatedList<TodoItemDto>>
{
    public async Task<PaginatedList<TodoItemDto>> Handle(
        GetMyTodosRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var items = await mongoRepository.FindAllAsync<TodoItemDto>(
                request.PageIndex,
                request.PageSize,
                predicate: item => item.CreatedBy == userContext.UserId,
                enableNoTracking: true,
                orderBy: qb => qb.CreatedAt!,
                cancellationToken: cancellationToken);

            if (items.Data.Count is 0)
            {
                return items;
            }

            var todoItems = items.Data;

            var userList = todoItems.SelectMany(u => new[] { u.CreatedBy, u.ModifiedBy }).ToList();

            var users = await userService.GetAll<UserResponse>(CacheKeys.USERS_LIST, cancellationToken);

            if (users is null || users.Count is 0)
            {
                return items;
            }

            var userDict = users.ToImmutableDictionary(u => u.Id.ToString(), u => u);

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
