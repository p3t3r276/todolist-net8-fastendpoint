using FastTodo.Domain.Constants;
using FastTodo.Domain.Shared.Constants;

namespace FastTodo.Infrastructure.Domain.Options;

public sealed record FastTodoOption
{
    public DatabaseProviderType SqlProvider { get; set; }

    public OpenAPI OpenAPI { get; set; }
}