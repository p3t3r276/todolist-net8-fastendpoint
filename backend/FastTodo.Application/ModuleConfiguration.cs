using System.Reflection;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Application;

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(FastTodo.Application.ModuleConfiguration).Assembly, Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ModuleConfiguration).Assembly));
        return services;
    }
}