using FastTodo.Domain.Entities.Identity;
using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Threading.Tasks.Task;

namespace FastTodo.Application.Features.Identity.Services;

public class UserService(ILogger<UserService> logger,
    ICacheService cacheService,
    UserManager<AppUser> userManager) : IUserService
{
    public async Task<bool> SyncDataToRedis(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var redisGroup = CacheKeys.USERS_LIST;
            var cleanTask = cacheService.RemoveAsync(redisGroup, cancellation: cancellationToken);
            var entitiesTask = userManager.Users.ToListAsync(cancellationToken);

            await WhenAll(cleanTask, entitiesTask);

            // var result = await cleanTask;
            var entities = await entitiesTask;

            if (entities is null || entities.Count is 0)
            {
                return false;
            }

            await cacheService.SetBulkAsync(redisGroup,
                entities!.ToDictionary(static x => x.Id.ToString() ?? string.Empty, x => x.Adapt<UserResponse>()),
                cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SyncToRedis-UserService");

            throw;
        }
    }
}
