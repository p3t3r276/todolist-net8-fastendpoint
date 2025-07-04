using FastEndpoints;

namespace FastTodo.API.Endpoints.Todo;

public sealed class TodoEndpointGroup : Group
{
    public TodoEndpointGroup()
    {
        Configure(
            "todos",
            ep =>
            {
                ep.Description(
                    x => x.Produces(401)
                        .ProducesProblemDetails()
                        .WithTags("Todos"));
            });
    }
}