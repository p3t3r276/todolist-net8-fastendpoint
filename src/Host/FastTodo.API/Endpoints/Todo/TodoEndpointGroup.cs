using FastEndpoints;

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
                        .AllowAnonymous()
                        .ProducesProblemDetails()
                        .WithTags("Todos"));
            });
    }
}