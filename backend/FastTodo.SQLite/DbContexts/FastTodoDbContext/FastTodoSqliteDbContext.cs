using Microsoft.EntityFrameworkCore;

namespace FastTodo.SQLite.DbContexts.FastTodoDbContext;

public class FastTodoSqliteDbContext (DbContextOptions<FastTodoSqliteDbContext> options) 
    : DbContext(options)
{
    
}
