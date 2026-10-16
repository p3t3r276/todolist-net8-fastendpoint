namespace FastTodo.Application.Features.Identity.Services;

public interface IUserService
{
    Task<List<UserResponse>> GetAll<T>(string redisGroup, CancellationToken cancellationToken = default);

    Task<bool> SyncDataToRedis(CancellationToken cancellationToken = default);
}
