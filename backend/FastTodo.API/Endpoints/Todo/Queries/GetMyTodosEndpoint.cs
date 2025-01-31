using FastEndpoints;
using FastTodo.Application.Features.Todos;
using FastTodo.Domain.Entities;
using MediatR;

namespace FastTodo.API.Endpoints.Todo;

public class GetMyTodosEndpoint (IMediator mediator) : Endpoint<GetMyTodosRequest, List<TodoItem>>
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public async Task<List<TodoItemDto>> HandleAsync(GetMyTodosRequest req, CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}