using FluentValidation;

namespace FastTodo.Application.Features.Todo;

public class CreateTodoValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty()
            .WithMessage(m => $"{nameof(m.Name)} is required")
            .MaximumLength(250)
            .WithMessage(m => $"{nameof(m.Name)} must be less than 250 characters");
    }
}