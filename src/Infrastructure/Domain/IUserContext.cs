namespace FastTodo.Infrastructure.Domain;

public interface IUserContext
{
    string? UserId { get; }

    public string? UserName { get; }
}
