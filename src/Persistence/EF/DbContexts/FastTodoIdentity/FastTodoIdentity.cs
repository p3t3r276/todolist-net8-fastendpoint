using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FastTodo.Persistence.EF;

public sealed class FastTodoIdentity : IdentityDbContext<AppUser>
{
    public FastTodoIdentity(DbContextOptions<FastTodoIdentity> options)
        : base(options)
    {
    }
}

public class AppUser : IdentityUser
{
    // Add customisations here later
}