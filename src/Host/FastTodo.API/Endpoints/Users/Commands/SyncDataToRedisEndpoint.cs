using FastEndpoints;
using FastEndpoints.AspVersioning;
using MediatR;

namespace FastTodo.API.Endpoints.Users;

public class SyncDataToRedisEndpoint(IMediator mediator) : EndpointWithoutRequest<bool>
{
    public override void Configure()
    {
        Get("/");
        Group<UserEndpointGroup>();
        Options(x => x
            .WithVersionSet("Users")
            .MapToApiVersion(1.0));
    }

    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        if (User.Identity is { IsAuthenticated: true })
        {
            Console.WriteLine(User.Identity.Name);
        }
        return await mediator.Send(, cancellationToken: cancellationToken);
    }
}
