namespace FastTodo.Domain.Entities;

public class Task
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool IsDone { get; set; }
}
