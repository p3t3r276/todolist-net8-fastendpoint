using FastEndpoints;
using FastTodo.Application.Features.Todos;
using MediatR;

namespace FastTodo.API.Endpoints.Todo;

public class CreateTodoEndpoint(IMediator mediator) : Endpoint<CreateTodoRequest, TodoItemDto>
{
    public override void Configure()
    {
        Post("/");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public override async Task<TodoItemDto> HandleAsync(CreateTodoRequest req, CancellationToken ct)
    {
        return await mediator.Send(req, ct);
    }
}   