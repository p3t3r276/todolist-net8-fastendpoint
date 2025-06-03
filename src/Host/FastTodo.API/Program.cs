using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpoints.Swagger;
using FastTodo.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

VersionSets.CreateApi(">>Todos<<", v => v
    .HasApiVersion(new(1.0))
    .HasApiVersion(new(2.0)));

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
        o.DocumentSettings = x =>
        {
            x.DocumentName = "Version one";
            x.ApiVersion(new(1.0));
        };
    })
    .SwaggerDocument(o =>
    {
        o.AutoTagPathSegmentIndex = 0;
        o.DocumentSettings = x =>
        {
            x.DocumentName = "Version two";
            x.ApiVersion(new(2.0));
        };
    });

var app = builder.Build();
    
app.UseHttpsRedirection();
app.UseDefaultExceptionHandler();
app.UseFastEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();
