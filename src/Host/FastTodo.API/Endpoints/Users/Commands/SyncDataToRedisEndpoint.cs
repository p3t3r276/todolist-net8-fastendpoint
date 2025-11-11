using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastTodo.Application.Features.Identity;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.API.Endpoints.Users;

public class SyncDataToRedisEndpoint(IMediator mediator) : Endpoint<SyncUsersToRedisRequest, Ok<bool>>
{
    public override void Configure()
    {
        Get("/");
        Group<UserEndpointGroup>();
        Options(x => x
            .WithVersionSet("Users")
            .MapToApiVersion(1.0));
    }

    public override async Task<Ok<bool>> ExecuteAsync(SyncUsersToRedisRequest request, CancellationToken cancellationToken)
    {
        if (User.Identity is { IsAuthenticated: true })
        {
            Console.WriteLine(User.Identity.Name);
        }
        return await mediator.Send(request, cancellationToken: cancellationToken);
    }
}
