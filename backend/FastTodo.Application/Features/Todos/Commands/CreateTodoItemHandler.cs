using MediatR;

namespace FastTodo.Application.Features.Todos;

public class CreateTodoItemHandler : IRequestHandler<CreateTodoRequest, TodoItemDto>
{
    public async Task<TodoItemDto> Handle(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new TodoItemDto()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsDone = false
        });
    }
}