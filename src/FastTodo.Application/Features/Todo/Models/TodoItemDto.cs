using System.Text.Json.Serialization;
using FastTodo.Application.Features.Identity;

namespace FastTodo.Application.Features.Todo;

public class TodoItemDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public bool IsDone { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    [JsonIgnore]
    public string CreatedBy { get; set; } = string.Empty;

    public UserResponse? CreatedByUser { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    [JsonIgnore]
    public string ModifiedBy { get; set; } = string.Empty;

    public UserResponse? ModifiedByUser { get; set; }
}
