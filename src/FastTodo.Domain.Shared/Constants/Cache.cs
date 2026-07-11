namespace FastTodo.Domain.Shared.Constants;

public enum CacheType
{
    InMemory = 0,

    Redis = 1
}

public struct CacheKeys
{
    public const string USERS_LIST = "Users";
}

public struct CACHE_TIME_IN_MINUTES
{
    public const int EVERY_DAY = 24 * 60;

    public const int EVERY_MONTH = 24 * 60 * 30;
}

public enum CacheKeySearchOperator
{
    StartsWith = 0,
    EndsWith = 1,
    Contains = 2
}
