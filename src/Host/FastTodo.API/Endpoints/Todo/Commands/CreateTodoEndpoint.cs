using FastEndpoints;
using FastTodo.Application.Features.Todo;
using MediatR;

namespace FastTodo.API.Endpoints.Todo;

public class CreateTodoEndpoint(IMediator mediator) : Endpoint<CreateTodoRequest, TodoItemDto>
{
    public override void Configure()
    {
        Post("/");
        Group<TodoEndpointGroup>();
        Version(1);
    }

    public override async Task<TodoItemDto> ExecuteAsync(CreateTodoRequest req, CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}
