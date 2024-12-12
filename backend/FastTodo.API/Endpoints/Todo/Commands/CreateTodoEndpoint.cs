using FastEndpoints;

namespace FastTodo.API.Endpoints.Tasks;

public class CreateTodoEndpoint : Endpoint<string, string>
{
    public override void Configure()
    {
        base.Configure();
    }

    public override Task HandleAsync(string req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}