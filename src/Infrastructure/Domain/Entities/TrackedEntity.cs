namespace FastTodo.Infrastructure.Domain.Entities;

public class TrackedEntity
{
    public string CreatedBy { get; set; } = string.Empty;

    public DateTimeOffset? CreatedAt { get; set; }

    public string ModifiedBy { get; set; } = string.Empty;

    public DateTimeOffset? ModifiedAt { get; set; }
}