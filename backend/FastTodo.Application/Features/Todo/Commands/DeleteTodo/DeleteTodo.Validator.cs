using FluentValidation;
using FastTodo.Infrastructure.Services;

namespace FastTodo.Application.Features.Todo;

public class DeleteTodoValidator : AbstractValidator<DeleteTodoRequest>
{
    public DeleteTodoValidator()
    {
        RuleFor(x => x.Id)
            .ValidId();
    }
}
