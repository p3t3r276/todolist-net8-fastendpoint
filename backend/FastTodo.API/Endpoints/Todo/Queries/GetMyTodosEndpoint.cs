using FastEndpoints;
using FastTodo.Application.Features.Todos;
using MediatR;

namespace FastTodo.API.Endpoints.Todo;

public class GetMyTodosEndpoint(IMediator mediator) : Endpoint<GetMyTodosRequest, List<TodoItemDto>>
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public override async Task<List<TodoItemDto>> ExecuteAsync(GetMyTodosRequest req, CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}