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

public struct CACHE_TIME_IN_MINUTES
{
    public const int EVERY_DAY = 24 * 60;

    public const int EVERY_MONTH = 24 * 60 * 30;
}
