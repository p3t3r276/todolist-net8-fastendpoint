using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.Application.Features.Todo;

public class GetTodoByIdRequest : IRequest<Results<NoContent, Ok<TodoItemDto>>>
{
    [RouteParam]
    public Guid? Id { get; set; }
}