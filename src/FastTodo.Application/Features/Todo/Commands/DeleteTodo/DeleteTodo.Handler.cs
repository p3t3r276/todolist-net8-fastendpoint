using FastTodo.Domain.Entities;
using FastTodo.Domain.Shared.Constants;
using FastTodo.Infrastructure.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FastTodo.Application.Features.Todo;

public class DeleteTodoHandler(
    ILogger<DeleteTodoHandler> logger,

    [FromKeyedServices(ServiceKeys.FastTodoEFUnitOfWork)] IUnitOfWork unitOfWork

) : IRequestHandler<DeleteTodoRequest, Results<NoContent, Ok>>
{
    public async Task<Results<NoContent, Ok>> Handle(DeleteTodoRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        try
        {
            var result = unitOfWork.Remove(new TodoItem { Id = request.Id!.Value });
            if (result is null)
            {
                return TypedResults.NoContent();
            }
            return TypedResults.Ok();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "DeleteTodoHandler: {id}", request.Id);

            throw;
        }
    }
}
