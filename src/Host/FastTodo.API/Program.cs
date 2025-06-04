using Asp.Versioning;
using Asp.Versioning.Builder;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpoints.Swagger;
using FastTodo.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader());
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});;

VersionSets.CreateApi(">>Todos<<", v => v
    .HasApiVersion(new(1.0))
    .HasApiVersion(new(2.0))
    .ReportApiVersions());

builder.Services
    .AddApplication(builder.Configuration)
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.AutoTagPathSegmentIndex = 0;
        o.DocumentSettings = x =>
        {
            x.DocumentName = "v1";
            x.Version = "v1";
            x.Title = "Fast Todo API v1";
            x.Version = "1.0";
        };
    })
    .SwaggerDocument(o =>
    {
        o.AutoTagPathSegmentIndex = 0;
        o.DocumentSettings = x =>
        {
            x.DocumentName = "v2";
            x.Title = "Fast Todo API v2";
            x.Version = "2.0";
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
    c.Endpoints.ShortNames = false;
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();
