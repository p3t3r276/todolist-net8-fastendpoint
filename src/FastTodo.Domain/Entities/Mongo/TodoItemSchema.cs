using FastTodo.Infrastructure.Domain.Entities;
using MongoDB.Bson;

namespace FastTodo.Domain.Entities.Mongo;

public class TodoItemSchema : BaseEntity<ObjectId>
{
    public Guid TaskId { get; set; }

    public string? Name { get; set; } = string.Empty;

    public bool IsDone { get; set; } = false;

    public DateTimeOffset? DueDate { get; set; }
}
