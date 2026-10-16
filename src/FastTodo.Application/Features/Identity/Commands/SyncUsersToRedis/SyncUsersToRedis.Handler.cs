using FastTodo.Application.Features.Identity.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace FastTodo.Application.Features.Identity;

public class SyncUsersToRedisHandler(
    ILogger<SyncUsersToRedisHandler> logger,
    IUserService userService) 
    : IRequestHandler<SyncUsersToRedisRequest, Ok<bool>>
{
    public async Task<Ok<bool>> Handle(SyncUsersToRedisRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var result = await userService.SyncDataToRedis(cancellationToken);

            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Handle-SyncUsersToRedisHandler-Exception");

            throw;
        }
    }
}
