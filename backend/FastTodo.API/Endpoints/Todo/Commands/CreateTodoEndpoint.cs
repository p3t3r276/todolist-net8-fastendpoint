using FastEndpoints;
using FastTodo.Application.Features.Todos;
using MediatR;

namespace FastTodo.API.Endpoints.Todo;

public class CreateTodoEndpoint : Endpoint<CreateTodoRequest, TodoItemDto>
{
    private IMediator _mediator { get; }

    public CreateTodoEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    public override void Configure()
    {
        Post("/");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public override async Task<TodoItemDto> HandleAsync(CreateTodoRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        return result;
    }
}