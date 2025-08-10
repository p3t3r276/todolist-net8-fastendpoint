using FastTodo.Infrastructure.Domain.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Infrastructure.Domain.Configurations;

public static class CorsConfiguration
{
    private const string CorsPolicy = "AllowSpecificOrigin";

    public static void AddAPICors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection(CorsOption.Cors).Get<CorsOption>();
        ArgumentNullException.ThrowIfNull(corsOptions);

        Console.WriteLine("Hahaha");

        services.AddCors(option =>
        {
            option.AddPolicy(CorsPolicy,
                p => p.WithOrigins(corsOptions.Origins)
                    .SetIsOriginAllowed(origin =>
                        (new Uri(origin).Host == "localhost" && corsOptions.IsAllowLocalhost)
                        || corsOptions.Origins.Contains(origin.TrimEnd('/')))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                );
        });
    }

    public static void UseAPICors(this IApplicationBuilder app)
    {
        app.UseCors(CorsPolicy);
    }
}