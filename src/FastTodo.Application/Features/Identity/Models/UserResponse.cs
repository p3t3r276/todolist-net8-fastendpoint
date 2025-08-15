namespace FastTodo.Application.Features.Identity;

public class UserResponse
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string NormalizedUserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}