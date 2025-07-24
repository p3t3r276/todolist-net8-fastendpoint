using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FastTodo.Infrastructure.Domain;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? UserName => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
}
