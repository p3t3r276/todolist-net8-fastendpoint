using FastEndpoints;
using FastTodo.Application.Features.Todo;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.API.Endpoints.Todo;

public class UpdateTodoEndpoint(IMediator mediator) : Endpoint<UpdateTodoRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public override void Configure()
    {
        Put("/{id:guid}");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public override async Task<Results<NoContent, Ok<TodoItemDto>>> ExecuteAsync(UpdateTodoRequest req, CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}
