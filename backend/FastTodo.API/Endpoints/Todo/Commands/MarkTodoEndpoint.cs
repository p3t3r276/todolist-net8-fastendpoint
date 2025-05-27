using FastEndpoints;
using FastTodo.Application.Features.Todo;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.API.Endpoints.Todo;

public class MarkTodoEndpoint(IMediator mediator) : Endpoint<MarkTodoRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public override void Configure()
    {
        Patch("/{id:guid}");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public override async Task<Results<NoContent, Ok<TodoItemDto>>> ExecuteAsync(MarkTodoRequest req, CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}
