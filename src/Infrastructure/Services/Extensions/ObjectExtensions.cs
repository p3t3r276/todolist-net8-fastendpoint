using Microsoft.Data.SqlClient;

namespace FastTodo.Infrastructure.Extensions;

public static class ObjectExtensions
{
    public static SqlParameter[] ToSqlParmeterArray(this object? obj)
    {
        if (obj == null) return [];

        var properties = obj.GetType().GetProperties();
        return properties
            .Select(p => new SqlParameter($"@{p.Name}", p.GetValue(obj)))
            .ToArray();
    }
}
