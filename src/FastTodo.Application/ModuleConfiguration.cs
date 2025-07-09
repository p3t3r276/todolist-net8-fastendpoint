using FastTodo.Infrastructure;
using FastTodo.Persistence.EF;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FastTodo.Application;

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(ModuleConfiguration).Assembly, Assembly.GetExecutingAssembly());

        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly(), typeof(ModuleConfiguration).Assembly],
        includeInternalTypes: true);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddInfrastructure(configuration);
        return services;
    }

    public static WebApplication UseApplication(this WebApplication application)
    {
        application.UseHttpsRedirection();
        application.UseAuthorization();

        application.UseEFPersistence();

        return application;
    }
}