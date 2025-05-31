using FastEndpoints;
using FastTodo.Application.Features.Todo;
using FastTodo.Domain.Shared;
using MediatR;

namespace FastTodo.API.Endpoints.Todo;

public class GetMyTodosEndpoint(IMediator mediator) : Endpoint<GetMyTodosRequest, PaginatedList<TodoItemDto>>
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public override async Task<PaginatedList<TodoItemDto>> ExecuteAsync(GetMyTodosRequest req, CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}
