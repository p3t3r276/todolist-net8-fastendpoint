using FastEndpoints;

namespace FastTodo.API.Endpoints.Todo;

public class CreateTodoEndpoint : Endpoint<string, string>
{
    public override void Configure()
    {
        Post("/to-dos");
        AllowAnonymous();
        Group<TodoEndpointGroup>();
    }

    public override async Task HandleAsync(string req, CancellationToken ct)
    {
        await SendAsync("hahah");
    }
}