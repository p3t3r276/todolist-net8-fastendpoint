using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FastTodo.Domain.Entities;

namespace FastTodo.Persistence.EF.DbContexts.FastTodoDbContext.Configurations;

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

        builder.Property(x => x.DueDate)
            .HasColumnType("datetimeoffset");

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetimeoffset");

        builder.Property(x => x.ModifiedAt)
            .HasColumnType("datetimeoffset");
    }
}