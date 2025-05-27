using System.Reflection;
using FastTodo.Infrastructure;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Application;

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(ModuleConfiguration).Assembly, Assembly.GetExecutingAssembly());

        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), 
        includeInternalTypes: true);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ModuleConfiguration).Assembly));

        services.AddInfrastructure(configuration);
        return services;
    }
}