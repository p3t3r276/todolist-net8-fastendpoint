using FastTodo.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FastTodo.Persistence.EF;

public sealed class FastTodoIdentityDbContext(
    DbContextOptions<FastTodoIdentityDbContext> options,
    IConfiguration configuration) 
    : IdentityDbContext<AppUser>(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(configuration.GetConnectionString(nameof(DatabaseProviderType.Identity)));
    }
}

public class AppUser : IdentityUser
{
    // Add customisations here later
}