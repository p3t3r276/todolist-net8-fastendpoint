using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastTodo.Application.Features.Todo;
using MediatR;

namespace FastTodo.API.Endpoints.Todo;

public class CreateTodoEndpoint(IMediator mediator) : Endpoint<CreateTodoRequest, TodoItemDto>
{
    public override void Configure()
    {
        Post("/");
        Group<TodoEndpointGroup>();
        Options(x => x
            .WithVersionSet("Todos")
            .MapToApiVersion(1.0));
    }

    public override async Task<TodoItemDto> ExecuteAsync(CreateTodoRequest req, CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}
