using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.Application.Features.Todo;

public class UpdateTodoRequest : IRequest<Results<NoContent, Ok<TodoItemDto>>>
{
    [RouteParam]
    public Guid? Id { get; set; }

    [FromBody]
    public UpdateTodoBody? Body { get; set; }
}

public record UpdateTodoBody(
    string? Name
);