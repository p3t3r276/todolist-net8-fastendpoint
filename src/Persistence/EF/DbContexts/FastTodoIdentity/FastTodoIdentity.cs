using FastTodo.Domain.Constants;
using FastTodo.Domain.Entities.Identity;
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

        optionsBuilder.UseSqlServer(configuration.GetConnectionString(nameof(ConnectionStrings.Identity)));
    }
}
