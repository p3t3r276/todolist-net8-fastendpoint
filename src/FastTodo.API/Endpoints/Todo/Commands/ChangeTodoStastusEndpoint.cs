using FastEndpoints;
using FastTodo.Application.Features.Todo;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.API.Endpoints.Todo;

public class ChangeTodoStastusEndpoint(IMediator mediator) 
    : Endpoint<ChangeTodoStastusRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public override void Configure()
    {
        Patch("/{id:guid}/status");
        Description(x => x.Accepts<ChangeTodoStastusRequest>());
        Group<TodoEndpointGroup>();
    }

    public override async Task<Results<NoContent, Ok<TodoItemDto>>> ExecuteAsync(
        ChangeTodoStastusRequest req,
        CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}
