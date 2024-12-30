using FastEndpoints;
using FastTodo.Application.Features.Todos;
using MediatR;

namespace FastTodo.API.Endpoints.Todo;

public class CreateTodoEndpoint : Endpoint<CreateTodoRequest, TodoItemDto>
{
    private IMediator _mediator { get; set; }

    public CreateTodoEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    public override void Configure()
    {
        Post("/to-dos");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public override async Task HandleAsync(CreateTodoRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        await SendAsync(result);
    }
}