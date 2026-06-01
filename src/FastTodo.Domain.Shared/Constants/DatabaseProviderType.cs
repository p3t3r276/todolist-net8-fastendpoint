namespace FastTodo.Domain.Constants;

public enum DatabaseProviderType
{
    SQLite,

    SQLServer,

    MySql,

    Postgres
}

public enum ConnectionStrings
{
    Default,

    Identity,

    Redis
}
