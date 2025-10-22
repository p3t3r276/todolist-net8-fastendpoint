using FastTodo.Domain.Constants;
using FastTodo.Domain.Shared.Constants;

namespace FastTodo.Infrastructure.Domain.Options;

public sealed record FastTodoOption
{
    public DatabaseProviderType SQLProvider { get; set; }

    public OpenAPI OpenAPI { get; set; }

    public CacheType CacheType { get; set; } = CacheType.InMemory;
}
