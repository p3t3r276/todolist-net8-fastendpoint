using Asp.Versioning;
using Asp.Versioning.Conventions;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpoints.Swagger;
using FastTodo.Application;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication();

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;

        // Clear all known networks and proxies to trust the Docker network's internal proxy
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear(); 
    });

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
#if DEBUG
        .MinimumLevel.Debug()
#else
        .MinimumLevel.Information()
#endif
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
    });

    VersionSets.CreateApi("Todos", v => v
        .HasApiVersion(1.0));

    VersionSets.CreateApi("Users", v => v
        .HasApiVersion(1.0));

    builder.Services
        .AddApplication(builder.Configuration)
        .AddFastEndpoints()
        .AddVersioning(o =>
        {
            o.DefaultApiVersion = new(1.0);
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
        })
        .SwaggerDocument(o =>
        {
            o.AutoTagPathSegmentIndex = 0;
            o.EndpointFilter = ep => ep.EndpointTags?.Contains("Account") is true;
            o.DocumentSettings = x =>
            {
                x.DocumentName = "User managmenet";
                x.Title = "User";
            };
        })
        .SwaggerDocument(o =>
        {
            o.AutoTagPathSegmentIndex = 0;
            o.MaxEndpointVersion = 1;
            o.DocumentSettings = x =>
            {
                x.DocumentName = "v1";
                x.ApiVersion(new(1.0));
                x.Title = "Fast Todo API v1";
            };
        });

    var app = builder.Build();

    app.UseForwardedHeaders();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerGen();
    }

    app.UseApplication();

    app.UseSerilogRequestLogging();

    app.UseFastEndpoints(c =>
    {
        c.Endpoints.RoutePrefix = "api";

        //c.Endpoints.Configurator = ep =>
        //{
        //    ep.PreProcessor<RequestLoggerProcessor>(Order.Before);
        //};
    });

    Log.Information("Starting FastTodo...");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}