using FluentValidation;
using FastTodo.Infrastructure.Services;

namespace FastTodo.Application.Features.Todo;

public class UpdateTodoValidator : AbstractValidator<UpdateTodoRequest>
{
    public UpdateTodoValidator()
    {
        RuleFor(x => x.Id)
            .ValidId();

        RuleFor(x => x.Body)
            .NotNull()
            .WithMessage("Please provide updated payload.");
    }
}
