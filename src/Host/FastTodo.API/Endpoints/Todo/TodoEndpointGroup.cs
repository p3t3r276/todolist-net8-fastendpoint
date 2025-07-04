using FastEndpoints;
using FastEndpoints.AspVersioning;

namespace FastTodo.API.Endpoints.Todo;

public class TodoEndpointGroup : Group
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