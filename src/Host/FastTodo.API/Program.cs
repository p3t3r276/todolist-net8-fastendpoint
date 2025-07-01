using FastEndpoints;
using FastEndpoints.Swagger;
using FastTodo.Application;
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

    builder.Services
        .AddApplication(builder.Configuration)
        .AddFastEndpoints()
        .SwaggerDocument(o =>
        {
            o.AutoTagPathSegmentIndex = 0;
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
                x.Version = "v1";
                x.Title = "Fast Todo API v1";
            };
        });

    var app = builder.Build();

    app.UseApplication();

    app.UseSerilogRequestLogging();

    app.UseFastEndpoints(c =>
    {
        c.Endpoints.RoutePrefix = "api";
        c.Versioning.Prefix = "v";
        c.Versioning.PrependToRoute = true;

        //c.Endpoints.Configurator = ep =>
        //{
        //    ep.PreProcessor<RequestLoggerProcessor>(Order.Before);
        //};
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerGen();
    }

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