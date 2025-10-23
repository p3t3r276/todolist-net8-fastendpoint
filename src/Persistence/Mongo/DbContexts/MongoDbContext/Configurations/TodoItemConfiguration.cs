using FastTodo.Domain.Entities.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FastTodo.Persistence.Mongo.DbContexts.MongoDbContext.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItemSchema>
{
    public void Configure(EntityTypeBuilder<TodoItemSchema> builder)
    {
        builder.ToCollection("Tasks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasElementName("_id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
    }
}