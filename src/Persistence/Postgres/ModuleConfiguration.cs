﻿using FastTodo.Infrastructure.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace FastTodo.Persistence.Postgres;

public static partial class ModuleConfiguration
{
    public static IServiceCollection AddPostgresPersistence(this IServiceCollection services)
    {
        services.AddFrameworkDbContexts();

        // services.AddKeyedScoped<IUnitOfWork, DefaultEfCommandUnitOfWork>(ServiceKeys.DefaultEFCommandUnitOfWork);
        //
        // services.TryAddScoped(typeof(IQueryRepository<,>), typeof(DefaultQueryRepository<,>));
        return services;
    }

    private static IServiceCollection AddFrameworkDbContexts(this IServiceCollection services)
    {
        services.AddDbContext<FastTodoPostgresContext>();
        services.AddScoped<BaseDbContext, FastTodoPostgresContext>();
        return services;
    }
}
