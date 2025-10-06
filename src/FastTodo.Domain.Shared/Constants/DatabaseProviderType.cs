namespace FastTodo.Domain.Constants;

public enum DatabaseProviderType
{
    SQLite,

    SQLServer,

    Identity,

    MySql,

    Postgres
}

public enum  ConnectionStrings
{
    Default,

    Identity
}