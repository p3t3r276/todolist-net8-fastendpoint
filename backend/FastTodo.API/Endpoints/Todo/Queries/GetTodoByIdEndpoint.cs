using FastEndpoints;
using FastTodo.Application.Features.Todos;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.API.Endpoints.Todo;

public class GetTodoByIdEndpoint(IMediator mediator) : Endpoint<GetTodoByIdRequest, Results<NoContent, Ok<TodoItemDto>>>
{
    public override void Configure()
    {
        Get("/{id:guid}");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public override async Task<Results<NoContent, Ok<TodoItemDto>>> ExecuteAsync(GetTodoByIdRequest req, CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}