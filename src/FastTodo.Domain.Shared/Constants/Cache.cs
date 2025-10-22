namespace FastTodo.Domain.Shared.Constants;

public enum CacheType
{
    InMemory,

    Redis
}

public struct CacheKeys
{
    public const string TODO_LIST = "Todos";

    public const string USERS_LIST = "Users";
}
