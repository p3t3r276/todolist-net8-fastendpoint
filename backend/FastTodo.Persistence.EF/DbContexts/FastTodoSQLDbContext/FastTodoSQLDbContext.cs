using Microsoft.EntityFrameworkCore;

namespace FastTodo.Persistence.EF.DbContexts.FastTodoSQLDbContext;

public class FastTodoSqliteDbContext(DbContextOptions<FastTodoSqliteDbContext> options)
    : DbContext(options)
{
    
}
