using FluentValidation;
using FastTodo.Infrastructure.Services;

namespace FastTodo.Application.Features.Todo;

public class MarkTodoValidator : AbstractValidator<ChangeTodoStastusRequest>
{
    public MarkTodoValidator()
    {
        RuleFor(x => x.Id)
            .ValidId();
    }
}
