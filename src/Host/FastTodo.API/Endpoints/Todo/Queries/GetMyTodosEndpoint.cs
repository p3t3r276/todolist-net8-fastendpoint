using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastTodo.Application.Features.Todo;
using FastTodo.Domain.Shared;
using MediatR;

namespace FastTodo.API.Endpoints.Todo;

public class GetMyTodosEndpoint(IMediator mediator) : Endpoint<GetMyTodosRequest, PaginatedList<TodoItemDto>>
{
    public override void Configure()
    {
        Get("/");
        Group<TodoEndpointGroup>();
        Options(x => x
            .WithVersionSet("Todos")
            .MapToApiVersion(1.0));
    }

    public override async Task<PaginatedList<TodoItemDto>> ExecuteAsync(GetMyTodosRequest req, CancellationToken ct)
    {
        if (User.Identity is { IsAuthenticated: true })
        {
            Console.WriteLine(User.Identity.Name);
        }
        return await mediator.Send(req, ct);
    }
}
