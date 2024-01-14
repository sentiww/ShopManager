using FluentValidation;
using ShopManager.Client.Models;

namespace ShopManager.Client.Validators;

public class ConfirmDeleteModelValidator : AbstractValidator<ConfirmDeleteModel>
{
    public ConfirmDeleteModelValidator(string correctName)
    {
        RuleFor(request => request.Name)
            .Equal(correctName);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ConfirmDeleteModel>.CreateWithOptions((ConfirmDeleteModel)model,
            strategy => strategy.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(failure => failure.ErrorMessage);
    };
}