using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastTodo.Application.Features.Todos;

public class DeleteTodoRequest : IRequest<Results<NoContent, Ok>>
{
    [RouteParam]
    public Guid? Id { get; set; }
}