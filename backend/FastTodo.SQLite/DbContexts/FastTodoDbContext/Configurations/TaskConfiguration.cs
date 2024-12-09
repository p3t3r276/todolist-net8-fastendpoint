using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskEntity = FastTodo.Domain.Entities.Task;

namespace FastTodo.SQLite.DbContexts.FastTodoDbContext.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.ToTable("Tasks");

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasKey(x => x.Id);

        builder.Property(x  => x.Name).HasMaxLength(250).IsRequired();
    }
}