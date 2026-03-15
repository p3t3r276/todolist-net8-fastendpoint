using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.Application.Features.Identity;

public class SyncUsersToRedisRequest : IRequest<Ok<bool>>
{
}
