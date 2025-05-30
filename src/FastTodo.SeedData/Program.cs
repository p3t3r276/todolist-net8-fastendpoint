using Bogus;
using FastTodo.Domain.Entities;
using FastTodo.Infrastructure;
using FastTodo.Infrastructure.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

int totalCount = 1_000_000;

var itemFaker = new Faker<TodoItem>()
    .RuleFor(x => x.Name, f => f.Lorem.Sentence())
    .RuleFor(x => x.IsDone, f => f.Random.Bool())
    .RuleFor(x => x.CreatedAt, f => f.Date.PastOffset(1))
    .RuleFor(x => x.DueDate, f => f.Date.BetweenOffset(new DateTimeOffset(DateTime.UtcNow.AddMonths(-6), TimeSpan.Zero), new DateTimeOffset(DateTime.UtcNow.AddMonths(6), TimeSpan.Zero)))
    .RuleFor(x => x.StartDate, f => f.Date.BetweenOffset(new DateTimeOffset(DateTime.UtcNow.AddMonths(-6), TimeSpan.Zero), new DateTimeOffset(DateTime.UtcNow.AddMonths(6), TimeSpan.Zero)))
    .RuleFor(x => x.EndDate, f => f.Date.BetweenOffset(new DateTimeOffset(DateTime.UtcNow.AddMonths(-2), TimeSpan.Zero), new DateTimeOffset(DateTime.UtcNow.AddMonths(2), TimeSpan.Zero)))
    .RuleFor(x => x.ModifiedAt, (f, item) => f.Date.BetweenOffset(item.CreatedAt, item.CreatedAt.AddMonths(6)));

Console.WriteLine("Starting geneating...");
var watch = System.Diagnostics.Stopwatch.StartNew();
var items = itemFaker.Generate(totalCount);
Console.WriteLine("Finish generating.");
Console.WriteLine($"Generated {totalCount} items in {watch.ElapsedMilliseconds} ms.");
watch.Stop();
var configuration = new ConfigurationBuilder()  
    .AddJsonFile("appsettings.json").Build();
var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);

services.AddInfrastructure(configuration);
services.AddInfrastructure(configuration);

var serviceProvider = services.BuildServiceProvider();
var dbContext = serviceProvider.GetService<BaseDbContext>() ?? throw new Exception("BaseDbContext not available");

Console.WriteLine("Starting bulk insert...");
var watch2 = System.Diagnostics.Stopwatch.StartNew();
await dbContext.BulkInsertAsync(items);
watch2.Stop();
Console.WriteLine("Insert in " + watch2.ElapsedMilliseconds + " ms.");