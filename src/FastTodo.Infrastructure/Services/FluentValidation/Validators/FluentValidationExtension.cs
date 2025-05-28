using FastTodo.Infrastructure.Services.FluentValidation.Validators;
using FluentValidation;
using FluentValidation.Validators;

namespace FastTodo.Infrastructure.Services;

public static class FluentValidationExtension
{
    public static IRuleBuilderOptions<T, Guid?> ValidId<T>(this IRuleBuilder<T, Guid?> rule)
    {
        // return rule
        //     .NotEmpty()
        //     .WithMessage("{Property} cannot be empty.");

        PropertyValidator<T, Guid?> validator = new IdValidator<T>();

        return rule.SetValidator(validator);
    }
}