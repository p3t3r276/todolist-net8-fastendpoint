using FastEndpoints;
using FastEndpoints.Swagger;
using FastTodo.Application;
using Serilog;

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#endif
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Host.UseSerilog();

builder.Services
    .AddApplication(builder.Configuration)
    .AddFastEndpoints()
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

app.UseHttpsRedirection();
app.UseDefaultExceptionHandler();
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Versioning.Prefix = "v";
    c.Versioning.PrependToRoute = true;
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();
