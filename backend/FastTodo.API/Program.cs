using FastEndpoints;
using FastEndpoints.Swagger;
using FastTodo.Application;
using FastTodo.Persistence.SQLite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services
    .AddApplication(builder.Configuration)
    .AddSQLiteEFPersistence(builder.Configuration)
    .AddFastEndpoints()
    .SwaggerDocument(o => o.AutoTagPathSegmentIndex = 0);

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
