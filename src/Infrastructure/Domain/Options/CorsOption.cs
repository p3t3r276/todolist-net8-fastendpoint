namespace FastTodo.Infrastructure.Domain.Options;

public sealed class CorsOption
{
    public static string Cors => nameof(Cors);

    public string[] Origins { get; init; }

    public bool IsAllowLocalhost { get; init; }

    public CorsOption(string[] origins, bool isAllowLocalhost)
    {
        ArgumentNullException.ThrowIfNull(origins, nameof(origins));

        Origins = origins;
        IsAllowLocalhost = isAllowLocalhost;
    }
}
