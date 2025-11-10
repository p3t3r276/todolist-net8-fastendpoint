namespace FastTodo.Application.Features.Identity.Services;

public interface IUserService
{
    Task<bool> SyncDataToRedis(CancellationToken cancellationToken = default);
}