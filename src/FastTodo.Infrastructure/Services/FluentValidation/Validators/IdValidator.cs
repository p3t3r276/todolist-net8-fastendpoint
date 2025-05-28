using FluentValidation;
using FluentValidation.Validators;

namespace FastTodo.Infrastructure.Services.FluentValidation.Validators;

public class IdValidator<T> : PropertyValidator<T, Guid?>,
    IIdValidator<T>
{
    public override string Name => "IdValidator";

    public override bool IsValid(ValidationContext<T> context, Guid? value)
    {
        var isValid = Guid.TryParse(value?.ToString(), out var _);
        isValid= isValid && value.HasValue && value.Value != Guid.Empty;
        if (!isValid)
        {
            context.MessageFormatter.AppendArgument("Property", context.PropertyPath);
            context.MessageFormatter.AppendArgument("Value", value);
            context.MessageFormatter.AppendArgument("Error", "Invalid ID format or empty GUID.");
        }
        return isValid;
    }
}

public interface IIdValidator<T> : IPropertyValidator<T, Guid?> { }