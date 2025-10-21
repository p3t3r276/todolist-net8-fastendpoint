using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
namespace FastTodo.Persistence.Redis;

public static class ModuleConfiguration
{
    public static void AddRedisPersistence(this IServiceCollection services)
    {
        services.Addstas(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("MyRedisConStr");
            options.InstanceName = "SampleInstance";
        });
    }
}
