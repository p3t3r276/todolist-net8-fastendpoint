using FastTodo.Domain.Entities;
using FastTodo.Infrastructure.Domain.ValueConverion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastTodo.Persistence.Postgres.DbContexts.FastTodoDbContext.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("Tasks", "todo");

        builder.Property(x => x.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();


        builder
            .Property(p => p.DueDate)
            .HasColumnType("timestamp with time zone")
            .HasConversion(
                v => v.HasValue ? v.Value.UtcDateTime : (DateTime?)null,
                v =>
                    v.HasValue
                        ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                        : (DateTimeOffset?)null
            );

        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasConversion(new DateTimeToDateTimeOffsetConverter());

        builder.Property(x => x.ModifiedAt)
            .HasColumnType("timestamp with time zone")
            .HasConversion(new DateTimeToDateTimeOffsetConverter());
    }
}
