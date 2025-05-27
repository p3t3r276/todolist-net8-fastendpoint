using FastEndpoints;
using FastTodo.Application.Features.Todos;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.API.Endpoints.Todo;

public class DeleteTodoEndpoint(IMediator mediator) : Endpoint<DeleteTodoRequest, Results<NoContent, Ok>>
{
    public override void Configure()
    {
        Delete("/{id:guid}");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public override async Task<Results<NoContent, Ok>> ExecuteAsync(DeleteTodoRequest req, CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}   