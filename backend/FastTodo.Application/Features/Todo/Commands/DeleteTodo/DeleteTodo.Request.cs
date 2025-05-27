using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.Application.Features.Todo;

public class DeleteTodoRequest : IRequest<Results<NoContent, Ok>>
{
    [RouteParam]
    public Guid? Id { get; set; }
}