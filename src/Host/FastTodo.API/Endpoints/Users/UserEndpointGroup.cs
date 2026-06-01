using FastEndpoints;

namespace FastTodo.API.Endpoints.Users;

public sealed class UserEndpointGroup : Group
{
    public UserEndpointGroup()
    {
        Configure(
            "users",
            ep =>
            {
                ep.Description(
                    x => x.Produces(401)
                        .ProducesProblemDetails()
                        .WithTags("Users"));
            });
    }
}
