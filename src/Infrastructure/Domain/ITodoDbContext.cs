using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace FastTodo.Infrastructure.Domain;

public interface ITodoDbContext
{
    // Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}