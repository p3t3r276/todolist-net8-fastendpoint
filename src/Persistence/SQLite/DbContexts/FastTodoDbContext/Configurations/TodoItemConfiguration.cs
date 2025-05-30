using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FastTodo.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FastTodo.Persistence.SQLite.DbContexts.FastTodoDbContext.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("Tasks");

        builder.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedOnAdd();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        
        builder.Property(x => x.DueDate);

        builder.Property(x => x.StartDate);

        builder.Property(x => x.EndDate);

        builder.Property(x => x.CreatedAt);

        builder.Property(x => x.ModifiedAt);
    }
}