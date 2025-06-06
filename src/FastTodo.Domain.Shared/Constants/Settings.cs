using System.Text.Json;
using System.Text.Json.Serialization;

namespace FastTodo.Domain.Shared.Constants;

public struct Settings
{
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        MaxDepth = 256,
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        IncludeFields = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
